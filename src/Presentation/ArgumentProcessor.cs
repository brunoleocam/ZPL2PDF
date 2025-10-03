using System;
using System.IO;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF
{
    /// <summary>
    /// Operation modes for ZPL2PDF.
    /// </summary>
    public enum OperationMode
    {
        /// <summary>
        /// Conversion mode - converts file and exits.
        /// </summary>
        Conversion,
        /// <summary>
        /// Daemon mode - monitors folder and converts automatically.
        /// </summary>
        Daemon,
        /// <summary>
        /// Help mode - shows help message and exits.
        /// </summary>
        Help
    }

    /// <summary>
    /// Responsible for processing command line arguments.
    /// </summary>
    public class ArgumentProcessor
    {
        private readonly ModeDetector _modeDetector;
        private readonly ArgumentParser _argumentParser;
        private readonly ArgumentValidator _validator;
        private readonly HelpDisplay _helpDisplay;

        /// <summary>
        /// Initializes a new instance of the ArgumentProcessor
        /// </summary>
        public ArgumentProcessor()
        {
            _modeDetector = new ModeDetector();
            _argumentParser = new ArgumentParser();
            _validator = new ArgumentValidator();
            _helpDisplay = new HelpDisplay();
        }

        /// <summary>
        /// Gets the operation mode (conversion or daemon).
        /// </summary>
        public OperationMode Mode { get; private set; } = OperationMode.Conversion;

        /// <summary>
        /// Gets the daemon command (start, stop, status).
        /// </summary>
        public string DaemonCommand { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the listen folder path for daemon mode.
        /// </summary>
        public string ListenFolderPath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the input file path.
        /// </summary>
        public string InputFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ZPL content.
        /// </summary>
        public string ZplContent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the output folder path.
        /// </summary>
        public string OutputFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        public string OutputFileName { get; set; } = "output.pdf";

        /// <summary>
        /// Gets or sets the label width.
        /// </summary>
        public double Width { get; set; } = 0;

        /// <summary>
        /// Gets or sets the label height.
        /// </summary>
        public double Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the unit of measurement.
        /// </summary>
        public string Unit { get; set; } = "mm";

        /// <summary>
        /// Gets or sets the print density in DPI.
        /// </summary>
        public int Dpi { get; set; } = 203;

        /// <summary>
        /// Processes the command line arguments.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        public void ProcessArguments(string[] args)
        {
            if (args.Length == 0)
            {
                // No arguments = daemon start mode (default behavior)
                Mode = OperationMode.Daemon;
                DaemonCommand = "start";
                ListenFolderPath = _argumentParser.GetDefaultListenFolder();
                
                // Apply default dimensions when no arguments are provided
                ApplyDefaultDimensions();
                return;
            }
            
            if (args[0].Equals("-help", StringComparison.OrdinalIgnoreCase))
            {
                _helpDisplay.ShowHelp();
                Environment.Exit(0);
            }

            // Detect operation mode
            Mode = _modeDetector.DetectMode(args);
            DaemonCommand = _modeDetector.ExtractDaemonCommand(args);

            // If it's help mode, show help and exit
            if (Mode == OperationMode.Help)
            {
                _helpDisplay.ShowHelp();
                Environment.Exit(0);
            }

            // Process based on mode
            if (Mode == OperationMode.Conversion)
            {
                ProcessConversionMode(args);
            }
            else if (Mode == OperationMode.Daemon)
            {
                ProcessDaemonMode(args);
            }
        }

        /// <summary>
        /// Processes conversion mode arguments
        /// </summary>
        private void ProcessConversionMode(string[] args)
        {
            var conversionArgs = _argumentParser.ParseConversionMode(args, 0);

            // Set properties
            InputFilePath = conversionArgs.InputFilePath;
            ZplContent = conversionArgs.ZplContent;
            OutputFolderPath = conversionArgs.OutputFolderPath;
            OutputFileName = conversionArgs.OutputFileName;
            Width = conversionArgs.Width;
            Height = conversionArgs.Height;
            Unit = conversionArgs.Unit;
            Dpi = conversionArgs.Dpi;

            // Validate arguments
            var validation = _validator.ValidateConversionMode(InputFilePath, ZplContent, OutputFolderPath, Width, Height, Unit);
            if (!validation.IsValid)
            {
                Console.WriteLine($"Error: {validation.ErrorMessage}");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Processes daemon mode arguments
        /// </summary>
        private void ProcessDaemonMode(string[] args)
        {
            if (DaemonCommand == "stop")
            {
                ProcessDaemonStop();
                return;
            }

            if (DaemonCommand == "status")
            {
                ProcessDaemonStatus();
                return;
            }

            // For start, run, or default daemon mode
            int startIndex = 0;
            if (DaemonCommand == "start" || DaemonCommand == "run")
            {
                // Only use startIndex = 1 if there's at least 1 argument to skip
                startIndex = args.Length > 0 ? 1 : 0;
            }
            var daemonArgs = _argumentParser.ParseDaemonMode(args, startIndex);

        // Set properties
        ListenFolderPath = string.IsNullOrWhiteSpace(daemonArgs.ListenFolderPath) 
            ? _argumentParser.GetDefaultListenFolder() 
            : daemonArgs.ListenFolderPath;
        
        // Use default dimensions if not specified (Width or Height = 0)
        if (daemonArgs.Width <= 0 || daemonArgs.Height <= 0)
        {
            // Use default dimensions based on unit
            var defaultDimensions = GetDefaultDimensions(daemonArgs.Unit);
            Width = daemonArgs.Width > 0 ? daemonArgs.Width : defaultDimensions.Width;
            Height = daemonArgs.Height > 0 ? daemonArgs.Height : defaultDimensions.Height;
        }
        else
        {
            Width = daemonArgs.Width;
            Height = daemonArgs.Height;
        }
            
        Unit = daemonArgs.Unit;
        Dpi = daemonArgs.Dpi;

        // Only validate dimensions if they were explicitly provided (not auto-applied)
        // Check if dimensions are different from defaults (indicating explicit user input)
        var defaultDims = GetDefaultDimensions(daemonArgs.Unit);
        bool dimensionsWereExplicitlyProvided = (daemonArgs.Width > 0 && daemonArgs.Height > 0) && 
                                               (Math.Abs(daemonArgs.Width - defaultDims.Width) > 0.001 || 
                                                Math.Abs(daemonArgs.Height - defaultDims.Height) > 0.001);
        
        if (dimensionsWereExplicitlyProvided)
        {
            // Validate arguments
            var validation = _validator.ValidateDaemonMode(ListenFolderPath, Width, Height, Unit, Dpi);
            if (!validation.IsValid)
            {
                Console.WriteLine($"Error: {validation.ErrorMessage}");
                Environment.Exit(1);
            }
        }
        }

        /// <summary>
        /// Processes daemon stop command
        /// </summary>
        private void ProcessDaemonStop()
        {
            // Daemon stop logic will be handled by DaemonManager
            Console.WriteLine("Stopping daemon...");
        }

        /// <summary>
        /// Processes daemon status command
        /// </summary>
        private void ProcessDaemonStatus()
        {
            // Daemon status logic will be handled by DaemonManager
            Console.WriteLine("Checking daemon status...");
        }

        /// <summary>
        /// Applies default dimensions when no arguments are provided
        /// </summary>
        private void ApplyDefaultDimensions()
        {
            var defaultDimensions = GetDefaultDimensions("mm");
            Width = defaultDimensions.Width;
            Height = defaultDimensions.Height;
            Unit = "mm";
            Dpi = 203;
        }

        /// <summary>
        /// Gets default dimensions based on unit
        /// </summary>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>Default dimensions</returns>
        private (double Width, double Height) GetDefaultDimensions(string unit)
        {
            switch (unit.ToLowerInvariant())
            {
                case "mm":
                    return (ApplicationConstants.DEFAULT_WIDTH_MM, ApplicationConstants.DEFAULT_HEIGHT_MM);
                case "in":
                case "inch":
                    return (ApplicationConstants.DEFAULT_WIDTH_IN, ApplicationConstants.DEFAULT_HEIGHT_IN);
                case "cm":
                    return (ApplicationConstants.DEFAULT_WIDTH_CM, ApplicationConstants.DEFAULT_HEIGHT_CM);
                case "pt":
                    // Convert mm to points: 1mm = 2.834645669 points
                    return (ApplicationConstants.DEFAULT_WIDTH_MM * 2.834645669, ApplicationConstants.DEFAULT_HEIGHT_MM * 2.834645669);
                    default:
                    return (ApplicationConstants.DEFAULT_WIDTH_MM, ApplicationConstants.DEFAULT_HEIGHT_MM);
            }
        }
    }
}