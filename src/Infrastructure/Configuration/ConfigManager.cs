using System;
using System.IO;
using System.Text.Json;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF
{
    /// <summary>
    /// Manages ZPL2PDF configurations, including configuration file and default folders
    /// </summary>
    public class ConfigManager
    {
        private readonly string _configFilePath;
        private Zpl2PdfConfig _config = null!;

        /// <summary>
        /// ConfigManager constructor
        /// </summary>
        public ConfigManager()
        {
            // Determine configuration file location (in executable folder)
            var executablePath = System.AppContext.BaseDirectory;
            _configFilePath = Path.Combine(executablePath, "zpl2pdf.json");
            
            // Load configurations
            LoadConfig();
        }

        /// <summary>
        /// Gets the default monitoring folder based on OS
        /// </summary>
        /// <returns>Default folder path</returns>
        public string GetDefaultListenFolder()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "ZPL2PDF Auto Converter");
        }

        /// <summary>
        /// Gets the configuration folder based on OS
        /// </summary>
        /// <returns>Configuration folder path</returns>
        public string GetConfigFolder()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %APPDATA%\ZPL2PDF
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appDataPath, "ZPL2PDF");
            }
            else
            {
                // Linux: ~/.config/zpl2pdf
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homePath, ".config", "zpl2pdf");
            }
        }


        /// <summary>
        /// Gets the PID folder based on OS
        /// </summary>
        /// <returns>PID folder path</returns>
        public string GetPidFolder()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %TEMP%
                return Path.GetTempPath();
            }
            else
            {
                // Linux: /var/run
                return "/var/run";
            }
        }

        /// <summary>
        /// Gets the current configuration
        /// </summary>
        /// <returns>ZPL2PDF configuration</returns>
        public Zpl2PdfConfig GetConfig()
        {
            return _config;
        }

        /// <summary>
        /// Updates the configuration
        /// </summary>
        /// <param name="config">New configuration</param>
        public void UpdateConfig(Zpl2PdfConfig config)
        {
            _config = config;
            SaveConfig();
        }

        /// <summary>
        /// Loads configuration from file
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    var jsonContent = File.ReadAllText(_configFilePath);
                    _config = JsonSerializer.Deserialize<Zpl2PdfConfig>(jsonContent) ?? CreateDefaultConfig();
                }
                else
                {
                    _config = CreateDefaultConfig();
                    SaveConfig(); // Create file with default configurations
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error loading configuration: {ex.Message}");
                _config = CreateDefaultConfig();
            }
        }

        /// <summary>
        /// Saves configuration to file
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(_config, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                
                File.WriteAllText(_configFilePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error saving configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates default configuration
        /// </summary>
        /// <returns>Default configuration</returns>
        private Zpl2PdfConfig CreateDefaultConfig()
        {
            return new Zpl2PdfConfig
            {
                DefaultListenFolder = GetDefaultListenFolder(),
                LabelWidth = DefaultSettings.GetDefaultWidth("mm"),
                LabelHeight = DefaultSettings.GetDefaultHeight("mm"),
                Unit = DefaultSettings.DEFAULT_UNIT,
                Dpi = DefaultSettings.DEFAULT_DPI,
                LogLevel = "Info",
                RetryDelay = 2000,
                MaxRetries = 3,
                AutoStart = false
            };
        }

        /// <summary>
        /// Creates necessary folders if they don't exist
        /// </summary>
        public void EnsureFoldersExist()
        {
            try
            {
                // Monitoring folder
                if (!Directory.Exists(_config.DefaultListenFolder))
                {
                    Directory.CreateDirectory(_config.DefaultListenFolder);
                    Console.WriteLine($"Monitoring folder created: {_config.DefaultListenFolder}");
                }

                // Processed and error file folders are created on demand when needed

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating folders: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates current configuration
        /// </summary>
        /// <returns>True if valid, False otherwise</returns>
        public bool ValidateConfig()
        {
            try
            {
                if (string.IsNullOrEmpty(_config.DefaultListenFolder))
                {
                    Console.WriteLine("Monitoring folder not configured");
                    return false;
                }

                if (_config.LabelWidth <= 0)
                {
                    Console.WriteLine("Label width must be greater than zero");
                    return false;
                }

                if (_config.LabelHeight <= 0)
                {
                    Console.WriteLine("Label height must be greater than zero");
                    return false;
                }

                if (_config.Dpi <= 0)
                {
                    Console.WriteLine("DPI must be greater than zero");
                    return false;
                }

                if (!new[] { "mm", "cm", "in", "pt" }.Contains(_config.Unit.ToLowerInvariant()))
                {
                    Console.WriteLine("Invalid unit. Use: mm, cm, in, pt");
                    return false;
                }

                if (!new[] { "Debug", "Info", "Warning", "Error" }.Contains(_config.LogLevel))
                {
                    Console.WriteLine("Invalid log level. Use: Debug, Info, Warning, Error");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating configuration: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Displays current configuration
        /// </summary>
        public void ShowConfig()
        {
            Console.WriteLine("Current ZPL2PDF configuration:");
            Console.WriteLine($"   Monitoring folder: {_config.DefaultListenFolder}");
            Console.WriteLine($"   Label dimensions: {_config.LabelWidth} x {_config.LabelHeight} {_config.Unit}");
            Console.WriteLine($"   Print Density: {ApplicationConstants.ConvertDpiToDpmm(ApplicationConstants.DEFAULT_DPI):F1} dpmm ({ApplicationConstants.DEFAULT_DPI} dpi)");
            Console.WriteLine($"   Log level: {_config.LogLevel}");
            Console.WriteLine($"   Retry delay: {_config.RetryDelay}ms");
            Console.WriteLine($"   Max retries: {_config.MaxRetries}");
            Console.WriteLine($"   Auto-start: {(_config.AutoStart ? "Yes" : "No")}");
        }
    }

    /// <summary>
    /// ZPL2PDF configuration class
    /// </summary>
    public class Zpl2PdfConfig
    {
        /// <summary>
        /// Default monitoring folder
        /// </summary>
        public string DefaultListenFolder { get; set; } = string.Empty;

        /// <summary>
        /// Default label width
        /// </summary>
        public double LabelWidth { get; set; } = DefaultSettings.GetDefaultWidth("mm");

        /// <summary>
        /// Default label height
        /// </summary>
        public double LabelHeight { get; set; } = DefaultSettings.GetDefaultHeight("mm");

        /// <summary>
        /// Unit of measurement (mm, cm, in, pt)
        /// </summary>
        public string Unit { get; set; } = DefaultSettings.DEFAULT_UNIT;

        /// <summary>
        /// Default DPI
        /// </summary>
        public int Dpi { get; set; } = DefaultSettings.DEFAULT_DPI;

        /// <summary>
        /// Log level (Debug, Info, Warning, Error)
        /// </summary>
        public string LogLevel { get; set; } = "Info";

        /// <summary>
        /// Delay between retry attempts (ms)
        /// </summary>
        public int RetryDelay { get; set; } = 2000;

        /// <summary>
        /// Maximum number of attempts
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Start automatically with the system
        /// </summary>
        public bool AutoStart { get; set; } = false;

    }
}
