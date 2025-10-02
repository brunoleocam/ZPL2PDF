using System;
using System.Collections.Generic;

namespace ZPL2PDF.Shared.Constants
{
    /// <summary>
    /// Centralized default configurations for the application
    /// </summary>
    public static class DefaultConfigurations
    {
        #region Default Folder Paths
        /// <summary>
        /// Gets the default watch folder path based on the operating system
        /// </summary>
        /// <returns>Default watch folder path</returns>
        public static string GetDefaultWatchFolder()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return System.IO.Path.Combine(documentsPath, ApplicationConstants.APPLICATION_NAME + " Auto Converter");
        }

        /// <summary>
        /// Gets the default configuration file path
        /// </summary>
        /// <returns>Default configuration file path</returns>
        public static string GetDefaultConfigFilePath()
        {
            var executablePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var executableDirectory = System.IO.Path.GetDirectoryName(executablePath);
            return System.IO.Path.Combine(executableDirectory ?? string.Empty, ApplicationConstants.CONFIG_FILE_NAME);
        }

        /// <summary>
        /// Gets the default PID file path based on the operating system
        /// </summary>
        /// <returns>Default PID file path</returns>
        public static string GetDefaultPidFilePath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: Use temp folder
                return System.IO.Path.Combine(Environment.GetEnvironmentVariable("TEMP") ?? Environment.GetEnvironmentVariable("TMP") ?? string.Empty, ApplicationConstants.PID_FILE_NAME);
            }
            else
            {
                // Linux/macOS: Use /var/run
                return System.IO.Path.Combine("/var/run", ApplicationConstants.PID_FILE_NAME);
            }
        }
        #endregion

        #region Default ZPL2PDF Configuration
        /// <summary>
        /// Gets the default ZPL2PDF configuration
        /// </summary>
        /// <returns>Default configuration object</returns>
        public static Zpl2PdfConfig GetDefaultZpl2PdfConfig()
        {
            return new Zpl2PdfConfig
            {
                DefaultListenFolder = GetDefaultWatchFolder(),
                LabelWidth = ApplicationConstants.DEFAULT_WIDTH_MM,
                LabelHeight = ApplicationConstants.DEFAULT_HEIGHT_MM,
                Unit = ApplicationConstants.DEFAULT_UNIT,
                Dpi = ApplicationConstants.DEFAULT_DPI,
                LogLevel = ApplicationConstants.DEFAULT_LOG_LEVEL,
                RetryDelay = ApplicationConstants.DEFAULT_RETRY_DELAY_MS,
                MaxRetries = ApplicationConstants.DEFAULT_MAX_RETRIES,
                AutoStart = false,
                IncludeSubdirectories = ApplicationConstants.DEFAULT_INCLUDE_SUBDIRECTORIES,
                FileFilter = ApplicationConstants.DEFAULT_FILE_FILTER
            };
        }
        #endregion

        #region Default Daemon Configuration
        /// <summary>
        /// Gets the default daemon configuration
        /// </summary>
        /// <returns>Default daemon configuration</returns>
        public static Domain.ValueObjects.DaemonConfiguration GetDefaultDaemonConfiguration()
        {
            return new Domain.ValueObjects.DaemonConfiguration
            {
                ListenFolderPath = GetDefaultWatchFolder(),
                Width = ApplicationConstants.DEFAULT_WIDTH_MM,
                Height = ApplicationConstants.DEFAULT_HEIGHT_MM,
                Unit = ApplicationConstants.DEFAULT_UNIT,
                Dpi = ApplicationConstants.DEFAULT_DPI,
                UseFixedDimensions = false,
                RetryDelayMs = ApplicationConstants.DEFAULT_RETRY_DELAY_MS,
                MaxRetries = ApplicationConstants.DEFAULT_MAX_RETRIES,
                LogLevel = ApplicationConstants.DEFAULT_LOG_LEVEL,
                IncludeSubdirectories = ApplicationConstants.DEFAULT_INCLUDE_SUBDIRECTORIES,
                FileFilter = ApplicationConstants.DEFAULT_FILE_FILTER
            };
        }
        #endregion

        #region Default Conversion Options
        /// <summary>
        /// Gets the default conversion options
        /// </summary>
        /// <returns>Default conversion options</returns>
        public static Domain.ValueObjects.ConversionOptions GetDefaultConversionOptions()
        {
            return new Domain.ValueObjects.ConversionOptions(
                ApplicationConstants.DEFAULT_WIDTH_MM,
                ApplicationConstants.DEFAULT_HEIGHT_MM,
                ApplicationConstants.DEFAULT_UNIT,
                ApplicationConstants.DEFAULT_DPI
            );
        }
        #endregion

        #region Default File Processing Settings
        /// <summary>
        /// Gets the default file processing settings
        /// </summary>
        /// <returns>Dictionary with default file processing settings</returns>
        public static Dictionary<string, object> GetDefaultFileProcessingSettings()
        {
            return new Dictionary<string, object>
            {
                { "RetryDelayMs", ApplicationConstants.DEFAULT_RETRY_DELAY_MS },
                { "MaxRetries", ApplicationConstants.DEFAULT_MAX_RETRIES },
                { "FileLockCheckDelayMs", ApplicationConstants.FILE_LOCK_CHECK_DELAY_MS },
                { "MaxFileLockWaitMs", ApplicationConstants.MAX_FILE_LOCK_WAIT_MS },
                { "FolderMonitorIntervalMs", ApplicationConstants.FOLDER_MONITOR_INTERVAL_MS },
                { "ValidExtensions", ApplicationConstants.VALID_FILE_EXTENSIONS },
                { "FileFilter", ApplicationConstants.DEFAULT_FILE_FILTER }
            };
        }
        #endregion

        #region Default Logging Settings
        /// <summary>
        /// Gets the default logging settings
        /// </summary>
        /// <returns>Dictionary with default logging settings</returns>
        public static Dictionary<string, object> GetDefaultLoggingSettings()
        {
            return new Dictionary<string, object>
            {
                { "LogLevel", ApplicationConstants.DEFAULT_LOG_LEVEL },
                { "LogToFile", true },
                { "LogToConsole", true },
                { "LogFolder", ApplicationConstants.LOGS_FOLDER },
                { "MaxLogFileSize", 10485760 }, // 10MB
                { "MaxLogFiles", 5 },
                { "LogFormat", "{0:yyyy-MM-dd HH:mm:ss} [{1}] {2}" }
            };
        }
        #endregion

        #region Default Performance Settings
        /// <summary>
        /// Gets the default performance settings
        /// </summary>
        /// <returns>Dictionary with default performance settings</returns>
        public static Dictionary<string, object> GetDefaultPerformanceSettings()
        {
            return new Dictionary<string, object>
            {
                { "MaxConcurrentFiles", 5 },
                { "ProcessingTimeoutMs", 30000 }, // 30 seconds
                { "MemoryLimitMB", 512 },
                { "EnableCaching", true },
                { "CacheSizeMB", 64 },
                { "EnableCompression", true }
            };
        }
        #endregion

        #region Default Security Settings
        /// <summary>
        /// Gets the default security settings
        /// </summary>
        /// <returns>Dictionary with default security settings</returns>
        public static Dictionary<string, object> GetDefaultSecuritySettings()
        {
            return new Dictionary<string, object>
            {
                { "AllowFileOverwrite", false },
                { "ValidateFileExtensions", true },
                { "ScanForMaliciousContent", false },
                { "MaxFileSizeMB", 100 },
                { "AllowedFileTypes", ApplicationConstants.VALID_FILE_EXTENSIONS },
                { "BlockedFileTypes", new string[] { ".exe", ".bat", ".cmd", ".ps1", ".sh" } }
            };
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets a configuration value with fallback to default
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="value">Current value</param>
        /// <param name="defaultValue">Default value if current is null or invalid</param>
        /// <returns>Value or default</returns>
        public static T GetValueOrDefault<T>(T value, T defaultValue)
        {
            return value != null ? value : defaultValue;
        }

        /// <summary>
        /// Gets a configuration value from dictionary with fallback
        /// </summary>
        /// <typeparam name="T">Type of the configuration value</typeparam>
        /// <param name="settings">Settings dictionary</param>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Value or default</returns>
        public static T GetSettingOrDefault<T>(Dictionary<string, object> settings, string key, T defaultValue)
        {
            if (settings == null || !settings.ContainsKey(key))
                return defaultValue;

            try
            {
                return (T)settings[key];
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Validates a configuration value against constraints
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="minValue">Minimum allowed value</param>
        /// <param name="maxValue">Maximum allowed value</param>
        /// <param name="defaultValue">Default value if invalid</param>
        /// <returns>Validated value</returns>
        public static double ValidateAndClamp(double value, double minValue, double maxValue, double defaultValue)
        {
            if (value < minValue || value > maxValue)
                return defaultValue;
            return value;
        }

        /// <summary>
        /// Validates a configuration value against constraints
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="minValue">Minimum allowed value</param>
        /// <param name="maxValue">Maximum allowed value</param>
        /// <param name="defaultValue">Default value if invalid</param>
        /// <returns>Validated value</returns>
        public static int ValidateAndClamp(int value, int minValue, int maxValue, int defaultValue)
        {
            if (value < minValue || value > maxValue)
                return defaultValue;
            return value;
        }
        #endregion
    }

    /// <summary>
    /// ZPL2PDF configuration class (moved from ConfigManager for better organization)
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
        public double LabelWidth { get; set; } = ApplicationConstants.DEFAULT_WIDTH_MM;

        /// <summary>
        /// Default label height
        /// </summary>
        public double LabelHeight { get; set; } = ApplicationConstants.DEFAULT_HEIGHT_MM;

        /// <summary>
        /// Unit of measurement (mm, cm, in, pt)
        /// </summary>
        public string Unit { get; set; } = ApplicationConstants.DEFAULT_UNIT;

        /// <summary>
        /// Default DPI
        /// </summary>
        public int Dpi { get; set; } = ApplicationConstants.DEFAULT_DPI;

        /// <summary>
        /// Log level (Debug, Info, Warning, Error)
        /// </summary>
        public string LogLevel { get; set; } = ApplicationConstants.DEFAULT_LOG_LEVEL;

        /// <summary>
        /// Retry delay in milliseconds
        /// </summary>
        public int RetryDelay { get; set; } = ApplicationConstants.DEFAULT_RETRY_DELAY_MS;

        /// <summary>
        /// Maximum number of retries
        /// </summary>
        public int MaxRetries { get; set; } = ApplicationConstants.DEFAULT_MAX_RETRIES;

        /// <summary>
        /// Auto-start daemon on application launch
        /// </summary>
        public bool AutoStart { get; set; } = false;

        /// <summary>
        /// Include subdirectories in monitoring
        /// </summary>
        public bool IncludeSubdirectories { get; set; } = ApplicationConstants.DEFAULT_INCLUDE_SUBDIRECTORIES;

        /// <summary>
        /// File filter pattern
        /// </summary>
        public string FileFilter { get; set; } = ApplicationConstants.DEFAULT_FILE_FILTER;
    }
}
