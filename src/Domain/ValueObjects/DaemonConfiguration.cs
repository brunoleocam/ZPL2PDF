using System;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF.Domain.ValueObjects
{
    /// <summary>
    /// Represents daemon configuration
    /// </summary>
    public class DaemonConfiguration
    {
        /// <summary>
        /// Gets or sets the listen folder path
        /// </summary>
        public string ListenFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label width
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the label height
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement
        /// </summary>
        public string Unit { get; set; } = "mm";

        /// <summary>
        /// Gets or sets the print density in DPI
        /// </summary>
        public int Dpi { get; set; } = 203;

        /// <summary>
        /// Gets or sets whether to use fixed dimensions
        /// </summary>
        public bool UseFixedDimensions { get; set; }

        /// <summary>
        /// Gets or sets the retry delay in milliseconds
        /// </summary>
        public int RetryDelayMs { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the maximum number of retries
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the log level
        /// </summary>
        public string LogLevel { get; set; } = "Info";

        /// <summary>
        /// Gets or sets whether to include subdirectories
        /// </summary>
        public bool IncludeSubdirectories { get; set; } = false;

        /// <summary>
        /// Gets or sets the file filter pattern
        /// </summary>
        public string FileFilter { get; set; } = "*.txt;*.prn;*.zpl;*.imp";

        /// <summary>
        /// Gets or sets the language/culture code (e.g., "pt-BR", "en-US", "es-ES")
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Initializes a new instance of DaemonConfiguration
        /// </summary>
        public DaemonConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of DaemonConfiguration with values
        /// </summary>
        /// <param name="listenFolderPath">Listen folder path</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        public DaemonConfiguration(string listenFolderPath, double width, double height, string unit, int dpi)
        {
            ListenFolderPath = listenFolderPath;
            Width = width;
            Height = height;
            Unit = unit;
            Dpi = dpi;
            UseFixedDimensions = width > 0 && height > 0;
        }

        /// <summary>
        /// Validates the daemon configuration
        /// </summary>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ListenFolderPath))
                return false;

            if (string.IsNullOrWhiteSpace(Unit))
                return false;

            if (Dpi <= 0)
                return false;

            if (UseFixedDimensions)
            {
                if (Width <= 0 || Height <= 0)
                    return false;
            }

            if (RetryDelayMs <= 0)
                return false;

            if (MaxRetries < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Gets validation error message
        /// </summary>
        /// <returns>Error message if invalid, empty string if valid</returns>
        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(ListenFolderPath))
                return "Listen folder path cannot be null or empty";

            if (string.IsNullOrWhiteSpace(Unit))
                return "Unit cannot be null or empty";

            if (Dpi <= 0)
                return "DPI must be greater than 0";

            if (UseFixedDimensions)
            {
                if (Width <= 0)
                    return "Width must be greater than 0 when using fixed dimensions";
                if (Height <= 0)
                    return "Height must be greater than 0 when using fixed dimensions";
            }

            if (RetryDelayMs <= 0)
                return "Retry delay must be greater than 0";

            if (MaxRetries < 0)
                return "Max retries cannot be negative";

            return string.Empty;
        }

        /// <summary>
        /// Creates a copy of the daemon configuration
        /// </summary>
        /// <returns>New instance with same values</returns>
        public DaemonConfiguration Clone()
        {
            return new DaemonConfiguration
            {
                ListenFolderPath = ListenFolderPath,
                Width = Width,
                Height = Height,
                Unit = Unit,
                Dpi = Dpi,
                UseFixedDimensions = UseFixedDimensions,
                RetryDelayMs = RetryDelayMs,
                MaxRetries = MaxRetries,
                LogLevel = LogLevel,
                IncludeSubdirectories = IncludeSubdirectories,
                FileFilter = FileFilter,
                Language = Language
            };
        }

        /// <summary>
        /// Returns a string representation of the daemon configuration
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            var dimensions = UseFixedDimensions 
                ? $"{Width}x{Height} {Unit}" 
                : "Extract from ZPL";
            
            return $"DaemonConfiguration: {ListenFolderPath}, {dimensions}, Print Density: {ApplicationConstants.ConvertDpiToDpmm(ApplicationConstants.DEFAULT_DPI):F1} dpmm ({ApplicationConstants.DEFAULT_DPI} dpi)";
        }
    }
}
