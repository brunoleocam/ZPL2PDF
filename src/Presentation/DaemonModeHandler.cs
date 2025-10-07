using System;
using System.Threading.Tasks;
using ZPL2PDF.Application.Services;
using ZPL2PDF.Infrastructure;
using ZPL2PDF.Shared.Constants;
using ZPL2PDF.Shared.Localization;

namespace ZPL2PDF
{
    /// <summary>
    /// Handles daemon mode operations
    /// </summary>
    public class DaemonModeHandler
    {
        /// <summary>
        /// Handles daemon mode operations
        /// </summary>
        /// <param name="argumentProcessor">The argument processor with daemon configuration</param>
        public async Task HandleDaemon(ArgumentProcessor argumentProcessor)
        {
            var daemonManager = new DaemonManager(
                argumentProcessor.ListenFolderPath,
                argumentProcessor.Width.ToString(),
                argumentProcessor.Height.ToString(),
                argumentProcessor.Unit,
                argumentProcessor.Dpi.ToString()
            );

            switch (argumentProcessor.DaemonCommand)
            {
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
        /// Runs daemon mode in the current process
        /// </summary>
        /// <param name="argumentProcessor">The argument processor with daemon configuration</param>
        private async Task RunDaemonMode(ArgumentProcessor argumentProcessor)
        {
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.STARTING_DAEMON));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.MONITORING_FOLDER, argumentProcessor.ListenFolderPath));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DIMENSIONS_INFO, argumentProcessor.Width, argumentProcessor.Height, argumentProcessor.Unit));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.PRINT_DENSITY_INFO, ApplicationConstants.ConvertDpiToDpmm(argumentProcessor.Dpi), argumentProcessor.Dpi));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.PRESS_CTRL_C_TO_STOP));

            try
            {
                // Create required dependencies
                var dimensionExtractor = new ZplDimensionExtractor();
                var configManager = new ConfigManager();
                
                // Create processing queue
                var processingQueue = new ProcessingQueue(dimensionExtractor, configManager);
                
                // Create fixed dimensions if using fixed dimensions
                LabelDimensions? fixedDimensions = null;
                bool useFixedDimensions = argumentProcessor.Width > 0 && argumentProcessor.Height > 0;
                
                if (useFixedDimensions)
                {
                    // Debug: Log input parameters
                    //Console.WriteLine($"DEBUG - Input parameters: Width={argumentProcessor.Width}, Height={argumentProcessor.Height}, Unit={argumentProcessor.Unit}, Dpi={argumentProcessor.Dpi}");
                    
                    // Convert dimensions to millimeters for internal calculations
                    var unitConverter = new UnitConversionService();
                    var (widthMm, heightMm) = unitConverter.ConvertToMillimeters(
                        argumentProcessor.Width, 
                        argumentProcessor.Height, 
                        argumentProcessor.Unit
                    );
                    
                    // Debug: Log conversion results
                    //Console.WriteLine($"DEBUG - Converted to mm: WidthMm={widthMm:F1}, HeightMm={heightMm:F1}");
                    
                    // Convert mm to points for ZPL processing
                    var widthPoints = unitConverter.ConvertMmToPoints(widthMm, argumentProcessor.Dpi);
                    var heightPoints = unitConverter.ConvertMmToPoints(heightMm, argumentProcessor.Dpi);
                    
                    // Debug: Log points conversion
                    //Console.WriteLine($"DEBUG - Converted to points: Width={widthPoints}, Height={heightPoints}");
                    
                    fixedDimensions = new LabelDimensions
                    {
                        Width = widthPoints,
                        Height = heightPoints,
                        WidthMm = widthMm,
                        HeightMm = heightMm,
                        Dpi = argumentProcessor.Dpi,
                        HasDimensions = true,
                        Source = "explicit_parameters"
                    };
                    
                    // Debug: Log the created dimensions
                    //Console.WriteLine($"DEBUG - Created fixedDimensions: {fixedDimensions.WidthMm:F1}mm x {fixedDimensions.HeightMm:F1}mm [{fixedDimensions.Source}]");
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

                // Start monitoring
                folderMonitor.StartWatching();

                // Keep running until interrupted
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in daemon mode: {ex.Message}");
            }
        }
    }
}
