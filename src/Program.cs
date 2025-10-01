using System;
using System.Diagnostics; // Added for Process
using System.IO;
using System.Threading;
using System.Threading.Tasks; // Added for async/await
using BinaryKits.Zpl.Viewer;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ZPL2PDF {
    /// <summary>
    /// Main program for converting ZPL to PDF.
    /// 
    /// The program requires the following parameters:
    /// 1. Input file path: Specified with the -i parameter.
    /// 2. Output folder path: Specified with the -o parameter.
    /// 
    /// The output file name can be specified using the -n parameter.
    /// If not specified, the PDF file name will be "ZPL2PDF_DDMMYYYYHHMM.pdf".
    /// Alternatively, the ZPL content can be provided directly using the -z parameter.
    /// 
    /// Optional parameters:
    /// -w: Width of the label.
    /// -h: Height of the label.
    /// -d: Print density in dots per millimeter.
    /// -u: Unit of measurement for width and height ("in", "cm", "mm").
    /// </summary>
    class Program {
        static async Task Main(string[] args) {
            try {
                var argumentProcessor = new ArgumentProcessor();
                argumentProcessor.ProcessArguments(args);

                // Route to appropriate mode
                if (argumentProcessor.Mode == OperationMode.Daemon) {
                    await HandleDaemonMode(argumentProcessor);
                } else {
                    HandleConversionMode(argumentProcessor);
                }
            } catch (Exception ex) {
                Console.Error.WriteLine($"Error during execution: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles daemon mode operations.
        /// </summary>
        /// <param name="argumentProcessor">The argument processor with daemon configuration.</param>
        private static async Task HandleDaemonMode(ArgumentProcessor argumentProcessor) {
            var daemonManager = new DaemonManager(
                argumentProcessor.ListenFolderPath,
                argumentProcessor.LabelWidth.ToString(),
                argumentProcessor.LabelHeight.ToString(),
                argumentProcessor.Unit,
                argumentProcessor.PrintDensityDpmm.ToString()
            );

            switch (argumentProcessor.DaemonCommand) {
                case "start":
                    daemonManager.Start();
                    break;
                case "stop":
                    daemonManager.Stop();
                    break;
                case "status":
                    daemonManager.Status();
                    break;
                case "run":
                    // Modo interno do daemon - executar em background
                    await RunDaemonMode(argumentProcessor);
                    break;
            }
        }

        /// <summary>
        /// Handles conversion mode operations.
        /// </summary>
        /// <param name="argumentProcessor">The argument processor with conversion configuration.</param>
        private static void HandleConversionMode(ArgumentProcessor argumentProcessor) {
            // Read file content
                string fileContent;
                if (!string.IsNullOrEmpty(argumentProcessor.InputFilePath)) {
                    fileContent = LabelFileReader.ReadFile(argumentProcessor.InputFilePath);
                } else {
                    fileContent = argumentProcessor.ZplContent;
                }

            var labels = LabelFileReader.SplitLabels(fileContent);

            // Create dimension extractor
            var dimensionExtractor = new ZplDimensionExtractor();
            
            // Determine if should use fixed dimensions or extract from ZPL
            bool hasExplicitDimensions = argumentProcessor.LabelWidth > 0 && argumentProcessor.LabelHeight > 0;
            
            LabelRenderer renderer;
            
            if (hasExplicitDimensions)
            {
                // Use fixed dimensions provided by parameters
                renderer = new LabelRenderer(
                    argumentProcessor.LabelWidth,
                    argumentProcessor.LabelHeight,
                    argumentProcessor.PrintDensityDpmm,
                    argumentProcessor.Unit
                );
            }
            else
            {
                // Extract dimensions from each label individually
                var extractedDimensionsList = dimensionExtractor.ExtractDimensions(fileContent);
                
                // For conversion mode, process each label with its own dimensions
                // Create a temporary renderer to process each label
                var allImageData = new List<byte[]>();
                
                for (int i = 0; i < labels.Count; i++)
                {
                    var label = labels[i];
                    var labelDimensions = i < extractedDimensionsList.Count ? extractedDimensionsList[i] : extractedDimensionsList[0];
                    
                    // Apply priority logic for this label
                    var finalDimensions = dimensionExtractor.ApplyPriorityLogic(
                        null, 
                        null, 
                        argumentProcessor.Unit, 
                        labelDimensions
                    );
                    
                    // Create specific renderer for this label
                    var labelRenderer = new LabelRenderer(finalDimensions);
                    var labelImages = labelRenderer.RenderLabels(new List<string> { label });
                    allImageData.AddRange(labelImages);
                }
                
                // Processar as imagens geradas
                ProcessImages(allImageData, argumentProcessor);
                return; // Exit method here
            }
            
                var imageDataList = renderer.RenderLabels(labels);

                // Ensure the output folder exists
                if (!Directory.Exists(argumentProcessor.OutputFolderPath)) {
                    Directory.CreateDirectory(argumentProcessor.OutputFolderPath);
                }

                string outputPdf = Path.Combine(argumentProcessor.OutputFolderPath, argumentProcessor.OutputFileName);
                PdfGenerator.GeneratePdf(imageDataList, outputPdf);
        }

        /// <summary>
        /// Executes the complete daemon mode with folder monitoring
        /// </summary>
        /// <param name="argumentProcessor">The argument processor with daemon configuration.</param>
        private static async Task RunDaemonMode(ArgumentProcessor argumentProcessor) {
            try {
                // Initialize components
                var configManager = new ConfigManager();
                var dimensionExtractor = new ZplDimensionExtractor();
                var processingQueue = new ProcessingQueue(dimensionExtractor, configManager, 1, argumentProcessor.ListenFolderPath);

                // Determine if should use fixed dimensions
                bool useFixedDimensions = argumentProcessor.LabelWidth > 0 && argumentProcessor.LabelHeight > 0;
                LabelDimensions? fixedDimensions = null;

                if (useFixedDimensions) {
                    fixedDimensions = new LabelDimensions {
                        WidthMm = argumentProcessor.LabelWidth,
                        HeightMm = argumentProcessor.LabelHeight,
                        Dpi = argumentProcessor.PrintDensityDpmm,
                        Source = "explicit_parameters"
                    };
                }

                // Create folder monitor
                var folderMonitor = new FolderMonitor(
                    argumentProcessor.ListenFolderPath,
                    processingQueue,
                    dimensionExtractor,
                    configManager,
                    fixedDimensions,
                    useFixedDimensions
                );

                // Configure events
                folderMonitor.FileDetected += (sender, e) => {
                    Console.WriteLine($"File detected: {e.FilePath} ({e.EventType})");
                };

                folderMonitor.ErrorOccurred += (sender, e) => {
                    Console.WriteLine($"Monitoring error: {e.GetException()?.Message}");
                };

                processingQueue.FileProcessed += (sender, e) => {
                    Console.WriteLine($"File processed successfully: {e.Item.FileName}");
                };

                processingQueue.FileError += (sender, e) => {
                    Console.WriteLine($"Error processing file: {e.Item.FileName} - {e.ErrorMessage}");
                };

                // Ensure folders exist
                configManager.EnsureFoldersExist();

                // Save PID and daemon process configuration
                var pidFilePath = GetDaemonPidFilePath();
                try {
                    var daemonInfo = new {
                        Pid = Process.GetCurrentProcess().Id,
                        ListenFolder = argumentProcessor.ListenFolderPath,
                        LabelWidth = argumentProcessor.LabelWidth,
                        LabelHeight = argumentProcessor.LabelHeight,
                        Unit = argumentProcessor.Unit,
                        PrintDensity = argumentProcessor.PrintDensityDpmm,
                        StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    
                    var json = System.Text.Json.JsonSerializer.Serialize(daemonInfo, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(pidFilePath, json);
                } catch (Exception ex) {
                    Console.WriteLine($"Warning: Could not save PID: {ex.Message}");
                }

                // Start monitoring
                folderMonitor.StartWatching();


                // Show information
                Console.WriteLine("Daemon mode started successfully!");
                Console.WriteLine($"Monitoring folder: {argumentProcessor.ListenFolderPath}");
                Console.WriteLine($"Dimensions: {(useFixedDimensions ? "Fixed" : "Extracted from ZPL")}");
                if (useFixedDimensions) {
                    Console.WriteLine($"   Width: {argumentProcessor.LabelWidth} {argumentProcessor.Unit}");
                    Console.WriteLine($"   Height: {argumentProcessor.LabelHeight} {argumentProcessor.Unit}");
                }
                Console.WriteLine($"DPI: {argumentProcessor.PrintDensityDpmm}");
                Console.WriteLine("Press Ctrl+C to stop...");

                // Wait indefinitely
                var cancellationTokenSource = new CancellationTokenSource();
                Console.CancelKeyPress += (sender, e) => {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();
                };

                try {
                    await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
                } catch (OperationCanceledException) {
                    // Ctrl+C pressionado
                }

                // Stop components
                Console.WriteLine("\nStopping daemon mode...");
                folderMonitor.StopWatching();
                await processingQueue.StopAsync();
                Console.WriteLine("Daemon mode stopped");

                // Remove PID file
                try {
                    if (File.Exists(pidFilePath))
                        File.Delete(pidFilePath);
                } catch { }

                Console.WriteLine("Daemon mode stopped successfully!");
            } catch (Exception ex) {
                Console.WriteLine($"Error in daemon mode: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Processa uma lista de imagens e gera o PDF final
        /// </summary>
        /// <param name="imageDataList">Lista de dados de imagem</param>
        /// <param name="argumentProcessor">Processador de argumentos</param>
        private static void ProcessImages(List<byte[]> imageDataList, ArgumentProcessor argumentProcessor)
        {
            // Ensure the output folder exists
            if (!Directory.Exists(argumentProcessor.OutputFolderPath))
            {
                Directory.CreateDirectory(argumentProcessor.OutputFolderPath);
            }

            string outputPdf = Path.Combine(argumentProcessor.OutputFolderPath, argumentProcessor.OutputFileName);
            PdfGenerator.GeneratePdf(imageDataList, outputPdf);
        }

        /// <summary>
        /// Gets the daemon PID file path
        /// </summary>
        /// <returns>PID file path</returns>
        private static string GetDaemonPidFilePath()
        {
            var fileName = "zpl2pdf-daemon.pid";
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %TEMP%
                var tempPath = Path.GetTempPath();
                return Path.Combine(tempPath, fileName);
            }
            else
            {
                // Linux: /var/run
                return Path.Combine("/var/run", fileName);
            }
        }
    }
}