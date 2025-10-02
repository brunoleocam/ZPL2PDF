using System.Text.RegularExpressions;

namespace ZPL2PDF.Shared.Constants
{
    /// <summary>
    /// Centralized regex patterns used throughout the application
    /// </summary>
    public static class RegexPatterns
    {
        #region ZPL Patterns
        /// <summary>
        /// Pattern to match ZPL width command: ^PW followed by digits
        /// </summary>
        public static readonly Regex ZplWidthPattern = new Regex(@"\^PW(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match ZPL length command: ^LL followed by digits
        /// </summary>
        public static readonly Regex ZplLengthPattern = new Regex(@"\^LL(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match complete ZPL labels: ^XA...^XZ
        /// </summary>
        public static readonly Regex ZplLabelPattern = new Regex(@"\^XA.*?\^XZ", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match ZPL start command: ^XA
        /// </summary>
        public static readonly Regex ZplStartPattern = new Regex(@"\^XA", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match ZPL end command: ^XZ
        /// </summary>
        public static readonly Regex ZplEndPattern = new Regex(@"\^XZ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region File Patterns
        /// <summary>
        /// Pattern to match valid file extensions
        /// </summary>
        public static readonly Regex ValidFileExtensionPattern = new Regex(@"\.(txt|prn)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match PDF file extension
        /// </summary>
        public static readonly Regex PdfFilePattern = new Regex(@"\.pdf$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match configuration file extension
        /// </summary>
        public static readonly Regex ConfigFilePattern = new Regex(@"\.json$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Path Patterns
        /// <summary>
        /// Pattern to match Windows drive letters
        /// </summary>
        public static readonly Regex WindowsDrivePattern = new Regex(@"^[A-Za-z]:\\", RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match valid folder names (no invalid characters)
        /// </summary>
        public static readonly Regex ValidFolderNamePattern = new Regex(@"^[^<>:""/\\|?*]+$", RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match valid file names (no invalid characters)
        /// </summary>
        public static readonly Regex ValidFileNamePattern = new Regex(@"^[^<>:""/\\|?*]+$", RegexOptions.Compiled);
        #endregion

        #region Unit Patterns
        /// <summary>
        /// Pattern to match unit values with optional decimal places
        /// </summary>
        public static readonly Regex UnitValuePattern = new Regex(@"^\d+(\.\d+)?$", RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match dimension values (width x height)
        /// </summary>
        public static readonly Regex DimensionPattern = new Regex(@"^(\d+(?:\.\d+)?)\s*[xX]\s*(\d+(?:\.\d+)?)$", RegexOptions.Compiled);
        #endregion

        #region Validation Patterns
        /// <summary>
        /// Pattern to match positive numbers (including decimals)
        /// </summary>
        public static readonly Regex PositiveNumberPattern = new Regex(@"^\d+(\.\d+)?$", RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match integers
        /// </summary>
        public static readonly Regex IntegerPattern = new Regex(@"^\d+$", RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match valid DPI values (72-600)
        /// </summary>
        public static readonly Regex DpiPattern = new Regex(@"^(7[2-9]|[8-9]\d|[1-5]\d{2}|600)$", RegexOptions.Compiled);
        #endregion

        #region Log Patterns
        /// <summary>
        /// Pattern to match log level values
        /// </summary>
        public static readonly Regex LogLevelPattern = new Regex(@"^(Debug|Info|Warning|Error)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match timestamp in logs
        /// </summary>
        public static readonly Regex TimestampPattern = new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}", RegexOptions.Compiled);
        #endregion

        #region Error Patterns
        /// <summary>
        /// Pattern to match error messages in logs
        /// </summary>
        public static readonly Regex ErrorMessagePattern = new Regex(@"\[ERROR\]|\[FATAL\]|Exception|Error:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Pattern to match warning messages in logs
        /// </summary>
        public static readonly Regex WarningMessagePattern = new Regex(@"\[WARN\]|\[WARNING\]|Warning:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        #endregion

        #region Helper Methods
        /// <summary>
        /// Checks if a string matches any of the supported units
        /// </summary>
        /// <param name="unit">Unit string to check</param>
        /// <returns>True if valid unit, False otherwise</returns>
        public static bool IsValidUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return false;

            return System.Array.Exists(ApplicationConstants.SUPPORTED_UNITS, 
                u => string.Equals(u, unit, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if a string matches any of the supported log levels
        /// </summary>
        /// <param name="logLevel">Log level string to check</param>
        /// <returns>True if valid log level, False otherwise</returns>
        public static bool IsValidLogLevel(string logLevel)
        {
            if (string.IsNullOrWhiteSpace(logLevel))
                return false;

            return System.Array.Exists(ApplicationConstants.SUPPORTED_LOG_LEVELS, 
                l => string.Equals(l, logLevel, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if a file extension is valid
        /// </summary>
        /// <param name="extension">File extension to check</param>
        /// <returns>True if valid extension, False otherwise</returns>
        public static bool IsValidFileExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return false;

            return ValidFileExtensionPattern.IsMatch(extension);
        }

        /// <summary>
        /// Checks if a dimension value is valid
        /// </summary>
        /// <param name="value">Dimension value to check</param>
        /// <returns>True if valid dimension, False otherwise</returns>
        public static bool IsValidDimension(double value)
        {
            return value >= ApplicationConstants.MIN_DIMENSION_VALUE && 
                   value <= ApplicationConstants.MAX_DIMENSION_VALUE;
        }

        /// <summary>
        /// Checks if a DPI value is valid
        /// </summary>
        /// <param name="dpi">DPI value to check</param>
        /// <returns>True if valid DPI, False otherwise</returns>
        public static bool IsValidDpi(int dpi)
        {
            return dpi >= ApplicationConstants.MIN_DPI_VALUE && 
                   dpi <= ApplicationConstants.MAX_DPI_VALUE;
        }
        #endregion
    }
}
