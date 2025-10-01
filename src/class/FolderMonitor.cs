using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZPL2PDF
{
    /// <summary>
    /// Monitors a folder to detect .txt and .prn files and adds them to the processing queue
    /// </summary>
    public class FolderMonitor : IDisposable
    {
        private FileSystemWatcher? _fileSystemWatcher;
        private readonly string _listenFolder;
        private readonly ProcessingQueue _processingQueue;
        private readonly ZplDimensionExtractor _dimensionExtractor;
        private readonly ConfigManager _configManager;
        private readonly LabelDimensions _fixedDimensions;
        private readonly bool _useFixedDimensions;
        private bool _isDisposed = false;

        /// <summary>
        /// Event fired when a file is detected
        /// </summary>
        public event EventHandler<FileDetectedEventArgs>? FileDetected;

        /// <summary>
        /// Event fired when an error occurs in monitoring
        /// </summary>
        public event EventHandler<ErrorEventArgs>? ErrorOccurred;

        /// <summary>
        /// FolderMonitor constructor
        /// </summary>
        /// <param name="listenFolder">Folder to monitor</param>
        /// <param name="processingQueue">Processing queue</param>
        /// <param name="dimensionExtractor">ZPL dimension extractor</param>
        /// <param name="configManager">Configuration manager</param>
        /// <param name="fixedDimensions">Fixed dimensions (if applicable)</param>
        /// <param name="useFixedDimensions">Whether to use fixed dimensions for all files</param>
        public FolderMonitor(
            string listenFolder,
            ProcessingQueue processingQueue,
            ZplDimensionExtractor dimensionExtractor,
            ConfigManager configManager,
            LabelDimensions? fixedDimensions = null,
            bool useFixedDimensions = false)
        {
            _listenFolder = listenFolder;
            _processingQueue = processingQueue;
            _dimensionExtractor = dimensionExtractor;
            _configManager = configManager;
            _fixedDimensions = fixedDimensions ?? new LabelDimensions();
            _useFixedDimensions = useFixedDimensions;
        }

        /// <summary>
        /// Starts folder monitoring
        /// </summary>
        public void StartWatching()
        {
            try
            {
                if (!Directory.Exists(_listenFolder))
                {
                    Directory.CreateDirectory(_listenFolder);
                    Console.WriteLine($"Folder created: {_listenFolder}");
                }

                _fileSystemWatcher = new FileSystemWatcher(_listenFolder)
                {
                    Filter = "*.*", // Monitor all files
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true
                };

                // Configure events
                _fileSystemWatcher.Created += OnFileCreated;
                _fileSystemWatcher.Changed += OnFileChanged;
                _fileSystemWatcher.Error += OnError;

                Console.WriteLine($"Monitoring folder: {_listenFolder}");
                Console.WriteLine($"File types: .txt, .prn");
                Console.WriteLine($"Dimensions: {(_useFixedDimensions ? "Fixed" : "Extracted from ZPL")}");
                
                if (_useFixedDimensions)
                {
                    Console.WriteLine($"   Width: {_fixedDimensions.WidthMm:F1}mm");
                    Console.WriteLine($"   Height: {_fixedDimensions.HeightMm:F1}mm");
                }
            }
            catch (Exception ex)
            {
                OnError(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Stops folder monitoring
        /// </summary>
        public void StopWatching()
        {
            try
            {
                if (_fileSystemWatcher != null)
                {
                    _fileSystemWatcher.EnableRaisingEvents = false;
                    _fileSystemWatcher.Dispose();
                    _fileSystemWatcher = null;
                }
                Console.WriteLine("Monitoring stopped");
            }
            catch (Exception ex)
            {
                OnError(this, new ErrorEventArgs(ex));
            }
        }


        /// <summary>
        /// Event fired when a file is created
        /// </summary>
        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            await HandleFileEvent(e.FullPath, "created");
        }

        /// <summary>
        /// Event fired when a file is modified
        /// </summary>
        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            await HandleFileEvent(e.FullPath, "modified");
        }

        /// <summary>
        /// Event fired when an error occurs
        /// </summary>
        private void OnError(object sender, ErrorEventArgs e)
        {
            ErrorOccurred?.Invoke(this, e);
            Console.WriteLine($"Monitoring error: {e.GetException().Message}");
        }

        /// <summary>
        /// Handles file events (created/modified)
        /// </summary>
        private async Task HandleFileEvent(string filePath, string eventType)
        {
            try
            {
                // Check if it's a valid file
                if (!IsValidFile(filePath))
                    return;

                // Wait a bit to ensure the file was completely written
                await Task.Delay(500);

                // Check if the file still exists and is not being used
                if (!File.Exists(filePath) || IsFileLocked(filePath))
                {
                    Console.WriteLine($"File in use, waiting: {Path.GetFileName(filePath)}");
                    return;
                }

                Console.WriteLine($"File {eventType}: {Path.GetFileName(filePath)}");
                
                // Fire event
                FileDetected?.Invoke(this, new FileDetectedEventArgs(filePath, eventType));

                // Process file
                await ProcessFileAsync(filePath);
            }
            catch (Exception ex)
            {
                OnError(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Processes an individual file
        /// </summary>
        private async Task ProcessFileAsync(string filePath)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

                // Check valid extension
                if (!IsValidFile(filePath))
                {
                    Console.WriteLine($"File ignored (invalid extension): {fileName}");
                    return;
                }

                // Read file content
                var content = await File.ReadAllTextAsync(filePath);
                if (string.IsNullOrWhiteSpace(content))
                {
                    Console.WriteLine($"Empty file ignored: {fileName}");
                    return;
                }

                // Determine dimensions
                LabelDimensions dimensions;
                if (_useFixedDimensions)
                {
                    // Use fixed dimensions
                    dimensions = _fixedDimensions;
                    Console.WriteLine($"Using fixed dimensions: {dimensions.WidthMm:F1}mm x {dimensions.HeightMm:F1}mm");
                }
                else
                {
                    // Extract dimensions from ZPL
                    var zplDimensions = _dimensionExtractor.ExtractDimensions(content);
                    if (zplDimensions.Count == 0)
                    {
                        Console.WriteLine($"No ZPL labels found in: {fileName}");
                        return;
                    }

                    // Use dimensions from first label (or apply priority logic)
                    var firstLabelDimensions = zplDimensions[0];
                    dimensions = _dimensionExtractor.ApplyPriorityLogic(null, null, "mm", firstLabelDimensions);
                    Console.WriteLine($"Extracted dimensions: {dimensions.WidthMm:F1}mm x {dimensions.HeightMm:F1}mm [{dimensions.Source}]");
                }

                // Add to processing queue
                var processingItem = new ProcessingItem
                {
                    FilePath = filePath,
                    FileName = fileName,
                    Content = content,
                    Dimensions = dimensions,
                    CreatedAt = DateTime.Now,
                    RetryCount = 0
                };

                await _processingQueue.AddFileAsync(processingItem);
                Console.WriteLine($"File added to queue: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {Path.GetFileName(filePath)}: {ex.Message}");
                OnError(this, new ErrorEventArgs(ex));
            }
        }

        /// <summary>
        /// Checks if the file is valid for processing
        /// </summary>
        private bool IsValidFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".txt" || extension == ".prn";
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
        /// Releases resources
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                StopWatching();
                _isDisposed = true;
            }
        }
    }

    /// <summary>
    /// Arguments for the file detected event
    /// </summary>
    public class FileDetectedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public string EventType { get; }

        public FileDetectedEventArgs(string filePath, string eventType)
        {
            FilePath = filePath;
            EventType = eventType;
        }
    }

    /// <summary>
    /// Processing item in the queue
    /// </summary>
    public class ProcessingItem
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public LabelDimensions Dimensions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
