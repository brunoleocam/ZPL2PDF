using System;
using System.IO;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for processing command line arguments.
    /// </summary>
    public class ArgumentProcessor {
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
        /// Processes the command line arguments.
        /// </summary>
        /// <param name="args">Array of command line arguments.</param>
        public void ProcessArguments(string[] args) {
            if (args.Length == 0 || args[0].Equals("-h", StringComparison.OrdinalIgnoreCase)) {
                ShowHelp();
                Environment.Exit(0);
            }

            for (int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    case "-i":
                        if (!string.IsNullOrEmpty(ZplContent)) {
                            throw new ArgumentException("Cannot specify both -i and -z parameters.");
                        }
                        if (i + 1 < args.Length && File.Exists(args[i + 1]) && Path.GetExtension(args[i + 1]).Equals(".txt", StringComparison.OrdinalIgnoreCase)) {
                            InputFilePath = args[i + 1];
                        } else {
                            throw new ArgumentException("Invalid input file path or the file is not a .txt file.");
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
        }

        /// <summary>
        /// Displays the help message.
        /// </summary>
        private void ShowHelp() {
            Console.WriteLine("Usage: ZPL2PDF.exe -i <input_file_path.txt> -o <output_folder_path> [-n <output_file_name>] | -z <zpl_content>");
            Console.WriteLine("Parameters:");
            Console.WriteLine("  -i <input_file_path.txt>   Path to the input .txt file");
            Console.WriteLine("  -z <zpl_content>           ZPL content as a string");
            Console.WriteLine("  -o <output_folder_path>    Path to the folder where the PDF file will be saved");
            Console.WriteLine("  -n <output_file_name>      Name of the output PDF file (optional)");
            Console.WriteLine("  -h                         Show this help message");
        }
    }
}