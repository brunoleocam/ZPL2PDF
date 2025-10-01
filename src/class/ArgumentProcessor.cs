using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ZPL2PDF {
    /// <summary>
    /// Operation modes for ZPL2PDF.
    /// </summary>
    public enum OperationMode {
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
    public class ArgumentProcessor {
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
        public string OutputFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label width. 0 means no explicit width provided.
        /// </summary>
        public double LabelWidth { get; set; } = 0;

        /// <summary>
        /// Gets or sets the label height. 0 means no explicit height provided.
        /// </summary>
        public double LabelHeight { get; set; } = 0;

        /// <summary>
        /// Gets or sets the print density in DPI (Dots Per Inch). Default: 203 DPI (Zebra standard).
        /// Note: Property name is PrintDensityDpmm for compatibility, but value is actually DPI.
        /// </summary>
        public int PrintDensityDpmm { get; set; } = 203;  // DPI padrão Zebra

        /// <summary>
        /// Gets or sets the unit of measurement for width and height.
        /// </summary>
        public string Unit { get; set; } = "mm";  // Unidade padrão

        /// <summary>
        /// Normalizes decimal numbers to accept both dot (.) and comma (,) as decimal separators.
        /// Converts to the system's decimal separator internally.
        /// </summary>
        /// <param name="value">The string value to normalize</param>
        /// <returns>Normalized string with system decimal separator</returns>
        private static string NormalizeDecimal(string value) {
            if (string.IsNullOrEmpty(value)) return value;
            
            // Get the system's decimal separator
            var systemDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            
            // If the value contains a different decimal separator, replace it
            if (systemDecimalSeparator == "." && value.Contains(",")) {
                return value.Replace(",", ".");
            } else if (systemDecimalSeparator == "," && value.Contains(".")) {
                return value.Replace(".", ",");
            }
            
            return value;
        }

        /// <summary>
        /// Tries to parse a double value, accepting both dot (.) and comma (,) as decimal separators.
        /// </summary>
        /// <param name="value">The string value to parse</param>
        /// <param name="result">The parsed double value</param>
        /// <returns>True if parsing was successful</returns>
        private static bool TryParseDouble(string value, out double result) {
            // First try with the normalized value
            var normalizedValue = NormalizeDecimal(value);
            if (double.TryParse(normalizedValue, NumberStyles.Float, CultureInfo.CurrentCulture, out result)) {
                return true;
            }
            
            // If that fails, try with invariant culture (dot as decimal separator)
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)) {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Processes the command line arguments.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        public void ProcessArguments(string[] args) {
            if (args.Length == 0) {
                // Sem argumentos = modo daemon start (comportamento padrão)
                Mode = OperationMode.Daemon;
                DaemonCommand = "start";
                ListenFolderPath = GetDefaultListenFolder();
                return;
            }
            
            if (args[0].Equals("-help", StringComparison.OrdinalIgnoreCase)) {
                ShowHelp();
                Environment.Exit(0);
            }

            // Detect operation mode
            DetectOperationMode(args);

            // Se é modo help, mostra ajuda e sai
            if (Mode == OperationMode.Help) {
                ShowHelp();
                Environment.Exit(0);
            }

            // Se é modo daemon mas não há comando explícito, processa como start
            if (Mode == OperationMode.Daemon && string.IsNullOrEmpty(DaemonCommand)) {
                ProcessDaemonStart(args, -1); // -1 indica que não há comando explícito
                return;
            }

            for (int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "start":
                        if (Mode != OperationMode.Daemon) {
                            throw new ArgumentException("Invalid command. 'start' can only be used in daemon mode.");
                        }
                        ProcessDaemonStart(args, i);
                        return; // Exit early for daemon commands
                    case "stop":
                        if (Mode != OperationMode.Daemon) {
                            throw new ArgumentException("Invalid command. 'stop' can only be used in daemon mode.");
                        }
                        ProcessDaemonStop();
                        return; // Exit early for daemon commands
                    case "status":
                        if (Mode != OperationMode.Daemon) {
                            throw new ArgumentException("Invalid command. 'status' can only be used in daemon mode.");
                        }
                        ProcessDaemonStatus();
                        return; // Exit early for daemon commands
                    case "run":
                        if (Mode != OperationMode.Daemon) {
                            throw new ArgumentException("Invalid command. 'run' can only be used in daemon mode.");
                        }
                        ProcessDaemonRun(args, i);
                        return; // Exit early for daemon commands
                    case "-i":
                        if (!string.IsNullOrEmpty(ZplContent)) {
                            throw new ArgumentException("Cannot specify both -i and -z parameters.");
                        }
                        if (i + 1 < args.Length && File.Exists(args[i + 1])) {
                            string extension = Path.GetExtension(args[i + 1]).ToLowerInvariant();
                            if (extension == ".txt" || extension == ".prn") {
                                InputFilePath = args[i + 1];
                            } else {
                                throw new ArgumentException("Invalid input file extension. Only .txt and .prn files are supported.");
                            }
                        } else {
                            throw new ArgumentException("Invalid input file path or the file does not exist.");
                        }
                        i++;
                        break;
                    case "-z":
                        if (!string.IsNullOrEmpty(InputFilePath)) {
                            throw new ArgumentException("Cannot specify both -i and -z parameters.");
                        }
                        if (i + 1 < args.Length) {
                            ZplContent = args[i + 1];
                        } else {
                            throw new ArgumentException("ZPL content not provided.");
                        }
                        i++;
                        break;
                    case "-o":
                        if (i + 1 < args.Length) {
                            OutputFolderPath = args[i + 1];
                        } else {
                            throw new ArgumentException("Output folder path not provided.");
                        }
                        i++;
                        break;
                    case "-n":
                        if (i + 1 < args.Length) {
                            OutputFileName = args[i + 1];
                        } else {
                            throw new ArgumentException("Output file name not provided.");
                        }
                        i++;
                        break;
                    case "-w":
                        if (i + 1 < args.Length && TryParseDouble(args[i + 1], out double width)) {
                            LabelWidth = width;
                        } else {
                            throw new ArgumentException("Invalid width value. Use dot (.) or comma (,) as decimal separator.");
                        }
                        i++;
                        break;
                    case "-h":
                        if (i + 1 < args.Length && TryParseDouble(args[i + 1], out double height)) {
                            LabelHeight = height;
                        } else {
                            throw new ArgumentException("Invalid height value. Use dot (.) or comma (,) as decimal separator.");
                        }
                        i++;
                        break;
                    case "-d":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int density)) {
                            PrintDensityDpmm = density;
                        } else {
                            throw new ArgumentException("Invalid print density value.");
                        }
                        i++;
                        break;
                    case "-u":
                        if (i + 1 < args.Length && (args[i + 1] == "in" || args[i + 1] == "cm" || args[i + 1] == "mm")) {
                            Unit = args[i + 1];
                        } else {
                            throw new ArgumentException("Invalid unit value. Must be 'in', 'cm', or 'mm'.");
                        }
                        i++;
                        break;
                    default:
                        throw new ArgumentException($"Unknown parameter: {args[i]}");
                }
            }

            if (string.IsNullOrEmpty(InputFilePath) && string.IsNullOrEmpty(ZplContent)) {
                throw new ArgumentException("Either -i or -z parameter must be specified.");
            }

            if (string.IsNullOrEmpty(OutputFolderPath)) {
                throw new ArgumentException("Parameter -o is mandatory.");
            }

            if (string.IsNullOrEmpty(OutputFileName)) {
                OutputFileName = $"ZPL2PDF_{DateTime.Now:ddMMyyyyHHmm}.pdf";
            }

            if ((LabelWidth != 60 / 8.0 || LabelHeight != 120 / 8.0) && string.IsNullOrEmpty(Unit)) {
                throw new ArgumentException("Parameter -u is mandatory when -w or -h is specified.");
            }
        }

        /// <summary>
        /// Detects the operation mode based on command line arguments.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        private void DetectOperationMode(string[] args) {
            if (args.Length > 0) {
                string firstArg = args[0].ToLowerInvariant();
                if (firstArg == "start" || firstArg == "stop" || firstArg == "status") {
                    Mode = OperationMode.Daemon;
                    DaemonCommand = firstArg;
                } else if (firstArg == "run") {
                    Mode = OperationMode.Daemon;
                    DaemonCommand = "run";
                } else if (firstArg == "-i" || firstArg == "-z" || firstArg == "-o") {
                    Mode = OperationMode.Conversion;
                } else if (firstArg == "-l") {
                    // -l sem start deve mostrar help
                    Mode = OperationMode.Help;
                } else {
                    // Se não é um comando explícito, verifica se são argumentos válidos
                    bool hasConversionArgs = args.Any(arg => 
                        arg.Equals("-i", StringComparison.OrdinalIgnoreCase) ||
                        arg.Equals("-z", StringComparison.OrdinalIgnoreCase) ||
                        arg.Equals("-o", StringComparison.OrdinalIgnoreCase) ||
                        arg.Equals("-n", StringComparison.OrdinalIgnoreCase));
                    
                    bool hasDaemonArgs = args.Any(arg => 
                        arg.Equals("-w", StringComparison.OrdinalIgnoreCase) ||
                        arg.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
                        arg.Equals("-u", StringComparison.OrdinalIgnoreCase) ||
                        arg.Equals("-d", StringComparison.OrdinalIgnoreCase));
                    
                    if (hasConversionArgs) {
                        Mode = OperationMode.Conversion;
                    } else if (hasDaemonArgs) {
                        // Argumentos de daemon sem start/run devem mostrar help
                        Mode = OperationMode.Help;
                    }
                }
            }
        }

        /// <summary>
        /// Processes the daemon start command.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        /// <param name="startIndex">Index where 'start' command was found.</param>
        private void ProcessDaemonStart(string[] args, int startIndex) {
            // Set default listen folder if not specified
            if (string.IsNullOrEmpty(ListenFolderPath)) {
                ListenFolderPath = GetDefaultListenFolder();
            }

            // Parse additional parameters for start command
            // Se startIndex é -1, começa do início (argumentos diretos)
            int startParsing = startIndex == -1 ? 0 : startIndex + 1;
            for (int i = startParsing; i < args.Length; i++) {
                switch (args[i]) {
                    case "-l":
                        if (i + 1 < args.Length) {
                            ListenFolderPath = args[i + 1];
                            i++; // Skip next argument as it's the folder path
                        } else {
                            throw new ArgumentException("Listen folder path not provided for -l parameter.");
                        }
                        break;
                    case "-w":
                        if (i + 1 < args.Length && TryParseDouble(args[i + 1], out double width)) {
                            LabelWidth = width;
                            i++; // Skip next argument
                        } else {
                            throw new ArgumentException("Invalid width value for daemon start. Use dot (.) or comma (,) as decimal separator.");
                        }
                        break;
                    case "-h":
                        if (i + 1 < args.Length && TryParseDouble(args[i + 1], out double height)) {
                            LabelHeight = height;
                            i++; // Skip next argument
                        } else {
                            throw new ArgumentException("Invalid height value for daemon start. Use dot (.) or comma (,) as decimal separator.");
                        }
                        break;
                    case "-u":
                        if (i + 1 < args.Length && (args[i + 1] == "in" || args[i + 1] == "cm" || args[i + 1] == "mm")) {
                            Unit = args[i + 1];
                            i++; // Skip next argument
                        } else {
                            throw new ArgumentException("Invalid unit value for daemon start. Must be 'in', 'cm', or 'mm'.");
                        }
                        break;
                    case "-d":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int density)) {
                            PrintDensityDpmm = density;
                            i++; // Skip next argument
                        } else {
                            throw new ArgumentException("Invalid print density value for daemon start.");
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unknown parameter for daemon start: {args[i]}");
                }
            }
        }

        /// <summary>
        /// Processes the daemon stop command.
        /// </summary>
        private void ProcessDaemonStop() {
            // Daemon stop logic will be implemented in DaemonManager
        }

        /// <summary>
        /// Processes the daemon status command.
        /// </summary>
        private void ProcessDaemonStatus() {
            // O status será exibido pelo DaemonManager, não precisamos carregar configurações aqui
            // As configurações reais do daemon são lidas do arquivo PID pelo DaemonManager
        }


        /// <summary>
        /// Processes the daemon run command (internal use).
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        /// <param name="startIndex">Index where the command starts.</param>
        private void ProcessDaemonRun(string[] args, int startIndex) {
            // Set default values
            if (string.IsNullOrEmpty(ListenFolderPath)) {
                ListenFolderPath = GetDefaultListenFolder();
            }

            // Process daemon run arguments
            for (int i = startIndex + 1; i < args.Length; i++) {
                switch (args[i]) {
                    case "-l":
                        if (i + 1 < args.Length) {
                            ListenFolderPath = args[i + 1];
                            i++;
                        } else {
                            throw new ArgumentException("Listen folder path not provided for -l parameter.");
                        }
                        break;
                    case "-w":
                        if (i + 1 < args.Length && TryParseDouble(args[i + 1], out double width)) {
                            LabelWidth = width;
                            i++;
                        } else {
                            throw new ArgumentException("Invalid width value for -w parameter. Use dot (.) or comma (,) as decimal separator.");
                        }
                        break;
                    case "-h":
                        if (i + 1 < args.Length && TryParseDouble(args[i + 1], out double height)) {
                            LabelHeight = height;
                            i++;
                        } else {
                            throw new ArgumentException("Invalid height value for -h parameter. Use dot (.) or comma (,) as decimal separator.");
                        }
                        break;
                    case "-u":
                        if (i + 1 < args.Length) {
                            Unit = args[i + 1];
                            i++;
                        } else {
                            throw new ArgumentException("Unit not provided for -u parameter.");
                        }
                        break;
                    case "-d":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int density)) {
                            PrintDensityDpmm = density;
                            i++;
                        } else {
                            throw new ArgumentException("Invalid density value for -d parameter. DPI must be an integer (e.g., 203, 300).");
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unknown parameter for daemon run: {args[i]}");
                }
            }
        }

        /// <summary>
        /// Gets the default listen folder path based on the operating system.
        /// </summary>
        /// <returns>Default listen folder path.</returns>
        private string GetDefaultListenFolder() {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "ZPL2PDF Auto Converter");
        }

        /// <summary>
        /// Displays the help message.
        /// </summary>
        private void ShowHelp() {
            Console.WriteLine("ZPL2PDF - ZPL to PDF Converter");
            Console.WriteLine();
            Console.WriteLine("DEFAULT BEHAVIOR:");
            Console.WriteLine("Usage: ZPL2PDF.exe [options]");
            Console.WriteLine("  Running without arguments starts daemon mode (same as 'start' command)");
            Console.WriteLine("  You can also pass daemon options directly: ZPL2PDF.exe -l \"folder\" -w 10 -h 5");
            Console.WriteLine();
            Console.WriteLine("CONVERSION MODE:");
            Console.WriteLine("Usage: ZPL2PDF.exe -i <input_file_path> -o <output_folder_path> [-n <output_file_name>] | -z <zpl_content>");
            Console.WriteLine("Parameters:");
            Console.WriteLine("  -i <input_file_path>       Path to the input .txt or .prn file");
            Console.WriteLine("  -z <zpl_content>           ZPL content as a string");
            Console.WriteLine("  -o <output_folder_path>    Path to the folder where the PDF file will be saved");
            Console.WriteLine("  -n <output_file_name>      Name of the output PDF file (optional)");
            Console.WriteLine("  -w <width>                 Width of the label (accepts . or , as decimal separator)");
            Console.WriteLine("  -h <height>                Height of the label (accepts . or , as decimal separator)");
            Console.WriteLine("  -d <density>               Print density in DPI (integer only, e.g., 203, 300)");
            Console.WriteLine("  -u <unit>                  Unit of measurement for width and height ('in', 'cm', 'mm')");
            Console.WriteLine();
            Console.WriteLine("DAEMON MODE:");
            Console.WriteLine("Usage: ZPL2PDF.exe start [options] | stop | status | run [options]");
            Console.WriteLine("Commands:");
            Console.WriteLine("  start                      Start daemon mode in background");
            Console.WriteLine("  stop                       Stop daemon mode");
            Console.WriteLine("  status                     Check daemon status");
            Console.WriteLine("  run                        Run daemon mode in current process (for testing)");
            Console.WriteLine("Daemon Options:");
            Console.WriteLine("  -l <listen_folder>          Folder to monitor (default: Documents/ZPL2PDF Auto Converter)");
            Console.WriteLine("  -w <width>                 Fixed width for all conversions (accepts . or , as decimal separator)");
            Console.WriteLine("  -h <height>                Fixed height for all conversions (accepts . or , as decimal separator)");
            Console.WriteLine("  -u <unit>                  Unit of measurement ('in', 'cm', 'mm')");
            Console.WriteLine("  -d <density>               Print density in DPI (integer only, e.g., 203, 300)");
            Console.WriteLine();
            Console.WriteLine("  -help                      Show this help message");
        }
    }
}