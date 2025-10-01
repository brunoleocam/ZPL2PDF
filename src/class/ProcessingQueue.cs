using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ZPL2PDF
{
    /// <summary>
    /// Asynchronous queue for processing ZPL files
    /// </summary>
    public class ProcessingQueue : IDisposable
    {
        private readonly ConcurrentQueue<ProcessingItem> _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _processingTask;
        private readonly ZplDimensionExtractor _dimensionExtractor;
        private readonly ConfigManager _configManager;
        private readonly string? _customOutputFolder;
        private bool _isDisposed = false;
        private bool _isProcessing = false;

        /// <summary>
        /// Event fired when a file is processed successfully
        /// </summary>
        public event EventHandler<FileProcessedEventArgs>? FileProcessed;

        /// <summary>
        /// Event fired when an error occurs during processing
        /// </summary>
        public event EventHandler<FileErrorEventArgs>? FileError;

        /// <summary>
        /// ProcessingQueue constructor
        /// </summary>
        /// <param name="dimensionExtractor">ZPL dimension extractor</param>
        /// <param name="configManager">Configuration manager</param>
        /// <param name="maxConcurrentFiles">Maximum number of files processed simultaneously</param>
        /// <param name="customOutputFolder">Custom folder to save PDFs (optional)</param>
        public ProcessingQueue(ZplDimensionExtractor dimensionExtractor, ConfigManager configManager, int maxConcurrentFiles = 1, string? customOutputFolder = null)
        {
            _queue = new ConcurrentQueue<ProcessingItem>();
            _semaphore = new SemaphoreSlim(maxConcurrentFiles, maxConcurrentFiles);
            _cancellationTokenSource = new CancellationTokenSource();
            _dimensionExtractor = dimensionExtractor;
            _configManager = configManager;
            _customOutputFolder = customOutputFolder;

            // Start processing task
            _processingTask = Task.Run(ProcessQueueAsync);
        }

        /// <summary>
        /// Adds a file to the processing queue
        /// </summary>
        /// <param name="item">Processing item</param>
        public async Task AddFileAsync(ProcessingItem item)
        {
            if (_isDisposed)
                return;

            _queue.Enqueue(item);
            Console.WriteLine($" File added to queue: {item.FileName} (Position: {_queue.Count})");
            
            // Wait a bit to avoid immediate processing
            await Task.Delay(100);
        }

        /// <summary>
        /// Processes the queue continuously
        /// </summary>
        private async Task ProcessQueueAsync()
        {
            _isProcessing = true;
            Console.WriteLine("Starting queue processing...");

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var item))
                    {
                        await ProcessFileAsync(item);
                    }
                    else
                    {
                        // Empty queue, wait a bit
                        await Task.Delay(1000, _cancellationTokenSource.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Queue processing error: {ex.Message}");
                    await Task.Delay(5000); // Wait before trying again
                }
            }

            _isProcessing = false;
            Console.WriteLine("Queue processing stopped");
        }

        /// <summary>
        /// Processes an individual file
        /// </summary>
        private async Task ProcessFileAsync(ProcessingItem item)
        {
            await _semaphore.WaitAsync(_cancellationTokenSource.Token);
            
            try
            {
                Console.WriteLine($"Processing: {item.FileName}");
                
                // Check if file still exists
                if (!File.Exists(item.FilePath))
                {
                    Console.WriteLine($"File not found: {item.FileName}");
                    return;
                }

                // Check if file is not being used
                if (IsFileLocked(item.FilePath))
                {
                    Console.WriteLine($"File in use, waiting: {item.FileName}");
                    await RetryFileAsync(item);
                    return;
                }

                // Process the file
                var success = await ConvertFileAsync(item);
                
                if (success)
                {
                    // Delete original file (PDF was generated successfully)
                    await DeleteOriginalFileAsync(item);
                    FileProcessed?.Invoke(this, new FileProcessedEventArgs(item));
                    Console.WriteLine($"File processed successfully: {item.FileName}");
                }
                else
                {
                    // File with error - do nothing (keep in original folder)
                    FileError?.Invoke(this, new FileErrorEventArgs(item, "Conversion failed"));
                    Console.WriteLine($"Failed to process file: {item.FileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {item.FileName}: {ex.Message}");
                item.ErrorMessage = ex.Message;
                FileError?.Invoke(this, new FileErrorEventArgs(item, ex.Message));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Converts ZPL file to PDF
        /// </summary>
        private async Task<bool> ConvertFileAsync(ProcessingItem item)
        {
            try
            {
                // Split labels
                var labels = LabelFileReader.SplitLabels(item.Content);
                if (labels.Count == 0)
                {
                    Console.WriteLine($"No ZPL labels found in: {item.FileName}");
                    return false;
                }

                // Extract dimensions from each label individually
                var extractedDimensionsList = _dimensionExtractor.ExtractDimensions(item.Content);
                
                // Process each label with its own dimensions
                var allImageData = new List<byte[]>();
                
                for (int i = 0; i < labels.Count; i++)
                {
                    var label = labels[i];
                    var labelDimensions = i < extractedDimensionsList.Count ? extractedDimensionsList[i] : extractedDimensionsList[0];
                    
                    // Apply priority logic for this label
                    var finalDimensions = _dimensionExtractor.ApplyPriorityLogic(
                        null, 
                        null, 
                        "mm", 
                        labelDimensions
                    );
                    
                    // Create specific renderer for this label
                    var labelRenderer = new LabelRenderer(finalDimensions);
                    var labelImages = labelRenderer.RenderLabels(new List<string> { label });
                    allImageData.AddRange(labelImages);
                    
                    Console.WriteLine($"Label {i + 1}: {finalDimensions.WidthMm:F1}mm x {finalDimensions.HeightMm:F1}mm [{finalDimensions.Source}]");
                }

                if (allImageData.Count == 0)
                {
                    Console.WriteLine($"No images generated for: {item.FileName}");
                    return false;
                }

                // Generate PDF
                var outputFileName = Path.ChangeExtension(item.FileName, ".pdf");
                var outputFolder = _customOutputFolder ?? Path.GetDirectoryName(item.FilePath)!;
                var outputPath = Path.Combine(outputFolder, outputFileName);
                
                // Ensure destination folder exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                
                PdfGenerator.GeneratePdf(allImageData, outputPath);
                
                Console.WriteLine($"PDF generated: {outputFileName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting {item.FileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes the original file after successful processing
        /// </summary>
        private async Task DeleteOriginalFileAsync(ProcessingItem item)
        {
            try
            {
                if (File.Exists(item.FilePath))
                {
                    File.Delete(item.FilePath);
                    Console.WriteLine($"Original file deleted: {item.FileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting original file {item.FileName}: {ex.Message}");
            }
        }


        /// <summary>
        /// Tries to process the file again after a delay
        /// </summary>
        private async Task RetryFileAsync(ProcessingItem item)
        {
            item.RetryCount++;
            var maxRetries = _configManager.GetConfig().MaxRetries;
            var retryDelay = _configManager.GetConfig().RetryDelay;

            if (item.RetryCount <= maxRetries)
            {
                Console.WriteLine($"Retry attempt {item.RetryCount}/{maxRetries} for: {item.FileName}");
                await Task.Delay(retryDelay);
                _queue.Enqueue(item); // Put back in queue
            }
            else
            {
                Console.WriteLine($"Max retries reached for: {item.FileName}");
                item.ErrorMessage = "File in use for too long";
                FileError?.Invoke(this, new FileErrorEventArgs(item, item.ErrorMessage));
            }
        }

        /// <summary>
        /// Checks if the file is being used by another process
        /// </summary>
        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }

        /// <summary>
        /// Gets queue statistics
        /// </summary>
        public QueueStats GetStats()
        {
            return new QueueStats
            {
                QueueLength = _queue.Count,
                IsProcessing = _isProcessing,
                MaxConcurrentFiles = _semaphore.CurrentCount
            };
        }

        /// <summary>
        /// Stops queue processing
        /// </summary>
        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            await _processingTask;
        }

        /// <summary>
        /// Libera recursos
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _cancellationTokenSource.Cancel();
                _semaphore.Dispose();
                _cancellationTokenSource.Dispose();
                _isDisposed = true;
            }
        }
    }

    /// <summary>
    /// Arguments for the file processed event
    /// </summary>
    public class FileProcessedEventArgs : EventArgs
    {
        public ProcessingItem Item { get; }

        public FileProcessedEventArgs(ProcessingItem item)
        {
            Item = item;
        }
    }

    /// <summary>
    /// Arguments for the file error event
    /// </summary>
    public class FileErrorEventArgs : EventArgs
    {
        public ProcessingItem Item { get; }
        public string ErrorMessage { get; }

        public FileErrorEventArgs(ProcessingItem item, string errorMessage)
        {
            Item = item;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Processing queue statistics
    /// </summary>
    public class QueueStats
    {
        public int QueueLength { get; set; }
        public bool IsProcessing { get; set; }
        public int MaxConcurrentFiles { get; set; }
    }
}
