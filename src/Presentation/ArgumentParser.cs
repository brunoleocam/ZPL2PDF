using System;
using System.IO;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF
{
    /// <summary>
    /// Responsible for parsing command line arguments
    /// </summary>
    public class ArgumentParser
    {
        private readonly ArgumentValidator _validator;

        /// <summary>
        /// Initializes a new instance of the ArgumentParser
        /// </summary>
        public ArgumentParser()
        {
            _validator = new ArgumentValidator();
        }

        /// <summary>
        /// Parses conversion mode arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <param name="startIndex">Starting index to parse from</param>
        /// <returns>Parsed conversion arguments</returns>
        public ConversionArguments ParseConversionMode(string[] args, int startIndex)
        {
            var result = new ConversionArguments();

            for (int i = startIndex; i < args.Length; i++)
            {
                string arg = args[i].ToLowerInvariant();
                string nextArg = i + 1 < args.Length ? args[i + 1] : string.Empty;

                switch (arg)
                {
                    case "-i":
                        if (i + 1 < args.Length)
                        {
                            result.InputFilePath = args[i + 1];
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-z":
                        if (i + 1 < args.Length)
                        {
                            result.ZplContent = args[i + 1];
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-o":
                        if (i + 1 < args.Length)
                        {
                            result.OutputFolderPath = args[i + 1];
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-n":
                        if (i + 1 < args.Length)
                        {
                            result.OutputFileName = ProcessOutputFileName(args[i + 1]);
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-w":
                        if (i + 1 < args.Length && _validator.TryParseDouble(args[i + 1], out double width))
                        {
                            result.Width = width;
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-h":
                        if (i + 1 < args.Length && _validator.TryParseDouble(args[i + 1], out double height))
                        {
                            result.Height = height;
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-u":
                        if (i + 1 < args.Length)
                        {
                            result.Unit = args[i + 1];
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "--stdout":
                        result.StandardOutput = true;
                        break;
                    case "--renderer":
                        if (!string.IsNullOrWhiteSpace(nextArg))
                        {
                            var renderer = nextArg.Trim().ToLowerInvariant();
                            result.RendererEngine = renderer switch
                            {
                                "offline" => RendererEngine.Offline,
                                "labelary" => RendererEngine.Labelary,
                                "auto" => RendererEngine.Auto,
                                _ => result.RendererEngine
                            };
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-d":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int dpi))
                        {
                            result.Dpi = dpi;
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "--fonts-dir":
                        if (i + 1 < args.Length)
                        {
                            result.FontsDirectory = args[i + 1];
                            i++;
                        }
                        break;
                    case "--font":
                        if (i + 1 < args.Length)
                        {
                            var pair = args[i + 1];
                            var eq = pair.IndexOf('=');
                            if (eq > 0)
                            {
                                var id = pair.Substring(0, eq).Trim();
                                var path = pair.Substring(eq + 1).Trim();
                                if (!string.IsNullOrEmpty(id))
                                    result.FontMappings.Add((id, path));
                            }
                            i++;
                        }
                        break;
                }
            }

            // Generate output file name if not specified
            if (string.IsNullOrEmpty(result.OutputFileName))
            {
                if (!string.IsNullOrEmpty(result.InputFilePath))
                {
                    // Use input file name as base for output file name
                    var inputFileName = Path.GetFileNameWithoutExtension(result.InputFilePath);
                    result.OutputFileName = $"{inputFileName}.pdf";
                }
                else if (!string.IsNullOrEmpty(result.ZplContent))
                {
                    // Use timestamp for ZPL content
                    var timestamp = DateTime.Now.ToString("ddMMyyyyHHmm");
                    result.OutputFileName = $"ZPL2PDF_{timestamp}.pdf";
                }
            }

            return result;
        }

        /// <summary>
        /// Parses daemon mode arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <param name="startIndex">Starting index to parse from</param>
        /// <returns>Parsed daemon arguments</returns>
        public DaemonArguments ParseDaemonMode(string[] args, int startIndex)
        {
            var result = new DaemonArguments();

            for (int i = startIndex; i < args.Length; i++)
            {
                string arg = args[i].ToLowerInvariant();

                switch (arg)
                {
                    case "-l":
                        if (i + 1 < args.Length)
                        {
                            result.ListenFolderPath = args[i + 1];
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-w":
                        if (i + 1 < args.Length && _validator.TryParseDouble(args[i + 1], out double width))
                        {
                            result.Width = width;
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-h":
                        if (i + 1 < args.Length && _validator.TryParseDouble(args[i + 1], out double height))
                        {
                            result.Height = height;
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-u":
                        if (i + 1 < args.Length)
                        {
                            result.Unit = args[i + 1];
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "-d":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int dpi))
                        {
                            result.Dpi = dpi;
                            i++; // Skip next argument as it's the value
                        }
                        break;
                    case "--renderer":
                        if (i + 1 < args.Length && !string.IsNullOrWhiteSpace(args[i + 1]))
                        {
                            var renderer = args[i + 1].Trim().ToLowerInvariant();
                            result.RendererEngine = renderer switch
                            {
                                "offline" => RendererEngine.Offline,
                                "labelary" => RendererEngine.Labelary,
                                "auto" => RendererEngine.Auto,
                                _ => result.RendererEngine
                            };
                            i++; // Skip next argument as it's the value
                        }
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the default listen folder
        /// </summary>
        /// <returns>Default listen folder path</returns>
        public string GetDefaultListenFolder()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "ZPL2PDF Auto Converter");
        }

        /// <summary>
        /// Gets the default output folder for TCP server mode
        /// </summary>
        /// <returns>Default TCP server output folder path</returns>
        public string GetDefaultTcpOutputFolder()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "ZPL2PDF TCP Output");
        }

        /// <summary>
        /// Parses TCP server mode arguments (--port, -o, --foreground).
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <param name="startIndex">Starting index (skip "server" and subcommand)</param>
        /// <returns>Parsed server arguments</returns>
        public ServerArguments ParseServerMode(string[] args, int startIndex)
        {
            var result = new ServerArguments { Port = 9101 };

            for (int i = startIndex; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.Equals("--port", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && int.TryParse(args[i + 1], out int port))
                {
                    result.Port = port;
                    i++;
                }
                else if (arg.Equals("-o", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    result.OutputFolder = args[i + 1];
                    i++;
                }
                else if (arg.Equals("--renderer", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && !string.IsNullOrWhiteSpace(args[i + 1]))
                {
                    var renderer = args[i + 1].Trim().ToLowerInvariant();
                    result.RendererEngine = renderer switch
                    {
                        "offline" => RendererEngine.Offline,
                        "labelary" => RendererEngine.Labelary,
                        "auto" => RendererEngine.Auto,
                        _ => result.RendererEngine
                    };
                    i++;
                }
                else if (arg.Equals("--foreground", StringComparison.OrdinalIgnoreCase))
                {
                    result.Foreground = true;
                }
            }

            if (string.IsNullOrWhiteSpace(result.OutputFolder))
            {
                result.OutputFolder = GetDefaultTcpOutputFolder();
            }

            return result;
        }

        /// <summary>
        /// Processes the output file name to ensure it has .pdf extension
        /// </summary>
        /// <param name="fileName">Input file name</param>
        /// <returns>Processed file name with .pdf extension</returns>
        private string ProcessOutputFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "output.pdf";

            // Remove any existing extension
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            
            // Always add .pdf extension
            return $"{nameWithoutExtension}.pdf";
        }
    }

    /// <summary>
    /// Represents parsed conversion mode arguments
    /// </summary>
    public class ConversionArguments
    {
        public string InputFilePath { get; set; } = string.Empty;
        public string ZplContent { get; set; } = string.Empty;
        public string OutputFolderPath { get; set; } = string.Empty;
        public string OutputFileName { get; set; } = string.Empty;
        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;
        public string Unit { get; set; } = "mm";
        public int Dpi { get; set; } = 203;
        /// <summary>When enabled, the conversion writes PDF bytes to stdout.</summary>
        public bool StandardOutput { get; set; } = false;
        /// <summary>Rendering engine selection (offline/labelary/auto).</summary>
        public RendererEngine RendererEngine { get; set; } = RendererEngine.Offline;
        /// <summary>Directory containing TTF/OTF fonts (e.g. for --fonts-dir).</summary>
        public string FontsDirectory { get; set; } = string.Empty;
        /// <summary>Font ID to file path (e.g. A=arial.ttf, B=another.ttf).</summary>
        public List<(string Id, string Path)> FontMappings { get; set; } = new List<(string, string)>();
    }

    /// <summary>
    /// Represents parsed daemon mode arguments
    /// </summary>
    public class DaemonArguments
    {
        public string ListenFolderPath { get; set; } = string.Empty;
        public double Width { get; set; } = 0;
        public double Height { get; set; } = 0;
        public string Unit { get; set; } = "mm";
        public int Dpi { get; set; } = 203;
        /// <summary>Rendering engine selection (offline/labelary/auto).</summary>
        public RendererEngine RendererEngine { get; set; } = RendererEngine.Offline;
    }

    /// <summary>
    /// Represents parsed TCP server mode arguments
    /// </summary>
    public class ServerArguments
    {
        public int Port { get; set; } = 9101;
        public string OutputFolder { get; set; } = string.Empty;
        public bool Foreground { get; set; } = false;
        /// <summary>Rendering engine selection (offline/labelary/auto).</summary>
        public RendererEngine RendererEngine { get; set; } = RendererEngine.Offline;
    }
}
