namespace ZPL2PDF.Shared.Constants
{
    /// <summary>
    /// Centralized error messages used throughout the application
    /// </summary>
    public static class ErrorMessages
    {
        #region File System Errors
        public const string FILE_NOT_FOUND = "File not found: {0}";
        public const string DIRECTORY_NOT_FOUND = "Directory not found: {0}";
        public const string FILE_ACCESS_DENIED = "Access denied to file: {0}";
        public const string DIRECTORY_ACCESS_DENIED = "Access denied to directory: {0}";
        public const string FILE_IN_USE = "File is in use and cannot be accessed: {0}";
        public const string INVALID_FILE_PATH = "Invalid file path: {0}";
        public const string INVALID_DIRECTORY_PATH = "Invalid directory path: {0}";
        public const string FILE_TOO_LARGE = "File is too large: {0}";
        public const string INSUFFICIENT_DISK_SPACE = "Insufficient disk space to complete operation";
        #endregion

        #region Validation Errors
        public const string INVALID_FILE_EXTENSION = "Invalid file extension. Supported extensions: {0}";
        public const string INVALID_UNIT = "Invalid unit '{0}'. Supported units: {1}";
        public const string INVALID_LOG_LEVEL = "Invalid log level '{0}'. Supported levels: {1}";
        public const string INVALID_DIMENSION = "Invalid dimension value '{0}'. Must be between {1} and {2}";
        public const string INVALID_DPI = "Invalid DPI value '{0}'. Must be between {1} and {2}";
        public const string INVALID_WIDTH = "Width must be greater than 0";
        public const string INVALID_HEIGHT = "Height must be greater than 0";
        public const string INVALID_RETRY_DELAY = "Retry delay must be greater than 0";
        public const string INVALID_MAX_RETRIES = "Max retries cannot be negative";
        public const string INVALID_LISTEN_FOLDER = "Listen folder path cannot be null or empty";
        public const string INVALID_OUTPUT_PATH = "Output path cannot be null or empty";
        public const string INVALID_INPUT_PATH = "Input path cannot be null or empty";
        #endregion

        #region Configuration Errors
        public const string CONFIG_LOAD_ERROR = "Error loading configuration: {0}";
        public const string CONFIG_SAVE_ERROR = "Error saving configuration: {0}";
        public const string CONFIG_VALIDATION_ERROR = "Configuration validation failed: {0}";
        public const string CONFIG_FILE_NOT_FOUND = "Configuration file not found: {0}";
        public const string CONFIG_CORRUPTED = "Configuration file is corrupted: {0}";
        public const string CONFIG_MISSING_REQUIRED_FIELD = "Missing required configuration field: {0}";
        #endregion

        #region Conversion Errors
        public const string CONVERSION_FAILED = "Conversion failed: {0}";
        public const string ZPL_PARSE_ERROR = "Error parsing ZPL content: {0}";
        public const string NO_ZPL_LABELS_FOUND = "No ZPL labels found in content";
        public const string INVALID_ZPL_SYNTAX = "Invalid ZPL syntax: {0}";
        public const string PDF_GENERATION_FAILED = "PDF generation failed: {0}";
        public const string IMAGE_RENDERING_FAILED = "Image rendering failed: {0}";
        public const string DIMENSION_EXTRACTION_FAILED = "Failed to extract dimensions from ZPL: {0}";
        #endregion

        #region Daemon Errors
        public const string DAEMON_START_FAILED = "Failed to start daemon: {0}";
        public const string DAEMON_STOP_FAILED = "Failed to stop daemon: {0}";
        public const string DAEMON_ALREADY_RUNNING = "Daemon is already running";
        public const string DAEMON_NOT_RUNNING = "Daemon is not running";
        public const string DAEMON_PID_FILE_ERROR = "Error managing PID file: {0}";
        public const string DAEMON_PROCESS_ERROR = "Error managing daemon process: {0}";
        public const string DAEMON_FOLDER_MONITOR_ERROR = "Error monitoring folder: {0}";
        #endregion

        #region Processing Errors
        public const string PROCESSING_QUEUE_FULL = "Processing queue is full";
        public const string PROCESSING_TIMEOUT = "Processing timeout exceeded";
        public const string PROCESSING_RETRY_EXHAUSTED = "Maximum retry attempts exceeded";
        public const string PROCESSING_FILE_LOCKED = "File is locked and cannot be processed: {0}";
        public const string PROCESSING_INVALID_FILE = "Invalid file for processing: {0}";
        public const string PROCESSING_EMPTY_FILE = "File is empty: {0}";
        #endregion

        #region Argument Errors
        public const string INVALID_ARGUMENTS = "Invalid arguments: {0}";
        public const string MISSING_REQUIRED_ARGUMENT = "Missing required argument: {0}";
        public const string INVALID_ARGUMENT_VALUE = "Invalid argument value '{0}' for parameter '{1}'";
        public const string CONFLICTING_ARGUMENTS = "Conflicting arguments: {0}";
        public const string UNKNOWN_ARGUMENT = "Unknown argument: {0}";
        public const string INVALID_MODE = "Invalid operation mode: {0}";
        #endregion

        #region Network Errors
        public const string NETWORK_ERROR = "Network error: {0}";
        public const string CONNECTION_TIMEOUT = "Connection timeout";
        public const string CONNECTION_REFUSED = "Connection refused";
        public const string DNS_RESOLUTION_FAILED = "DNS resolution failed: {0}";
        #endregion

        #region System Errors
        public const string OUT_OF_MEMORY = "Out of memory";
        public const string SYSTEM_ERROR = "System error: {0}";
        public const string PERMISSION_DENIED = "Permission denied: {0}";
        public const string RESOURCE_UNAVAILABLE = "Resource unavailable: {0}";
        public const string TIMEOUT_ERROR = "Operation timeout: {0}";
        #endregion

        #region Generic Errors
        public const string UNKNOWN_ERROR = "Unknown error occurred: {0}";
        public const string OPERATION_FAILED = "Operation failed: {0}";
        public const string VALIDATION_FAILED = "Validation failed: {0}";
        public const string INTERNAL_ERROR = "Internal error: {0}";
        public const string NOT_IMPLEMENTED = "Feature not implemented: {0}";
        #endregion

        #region Success Messages
        public const string CONVERSION_SUCCESS = "Conversion completed successfully: {0}";
        public const string DAEMON_STARTED = "Daemon started successfully";
        public const string DAEMON_STOPPED = "Daemon stopped successfully";
        public const string FILE_PROCESSED = "File processed successfully: {0}";
        public const string CONFIG_SAVED = "Configuration saved successfully";
        public const string OPERATION_SUCCESS = "Operation completed successfully";
        #endregion

        #region Warning Messages
        public const string FILE_IGNORED = "File ignored: {0}";
        public const string DIMENSIONS_EXTRACTED = "Dimensions extracted from ZPL: {0}";
        public const string USING_DEFAULT_DIMENSIONS = "Using default dimensions: {0}";
        public const string RETRY_ATTEMPT = "Retry attempt {0} of {1} for file: {2}";
        public const string FOLDER_CREATED = "Folder created: {0}";
        #endregion

        #region Info Messages
        public const string PROCESSING_FILE = "Processing file: {0}";
        public const string MONITORING_FOLDER = "Monitoring folder: {0}";
        public const string DAEMON_STATUS_RUNNING = "Daemon is running (PID: {0})";
        public const string DAEMON_STATUS_STOPPED = "Daemon is stopped";
        public const string CONFIG_LOADED = "Configuration loaded from: {0}";
        #endregion

        #region Helper Methods
        /// <summary>
        /// Formats an error message with parameters
        /// </summary>
        /// <param name="message">Error message template</param>
        /// <param name="args">Parameters to format</param>
        /// <returns>Formatted error message</returns>
        public static string Format(string message, params object[] args)
        {
            return string.Format(message, args);
        }

        /// <summary>
        /// Gets a validation error for invalid units
        /// </summary>
        /// <param name="unit">Invalid unit</param>
        /// <returns>Formatted error message</returns>
        public static string GetInvalidUnitError(string unit)
        {
            return Format(INVALID_UNIT, unit, string.Join(", ", ApplicationConstants.SUPPORTED_UNITS));
        }

        /// <summary>
        /// Gets a validation error for invalid log levels
        /// </summary>
        /// <param name="logLevel">Invalid log level</param>
        /// <returns>Formatted error message</returns>
        public static string GetInvalidLogLevelError(string logLevel)
        {
            return Format(INVALID_LOG_LEVEL, logLevel, string.Join(", ", ApplicationConstants.SUPPORTED_LOG_LEVELS));
        }

        /// <summary>
        /// Gets a validation error for invalid file extensions
        /// </summary>
        /// <returns>Formatted error message</returns>
        public static string GetInvalidFileExtensionError()
        {
            return Format(INVALID_FILE_EXTENSION, string.Join(", ", ApplicationConstants.VALID_FILE_EXTENSIONS));
        }

        /// <summary>
        /// Gets a validation error for invalid dimensions
        /// </summary>
        /// <param name="value">Invalid dimension value</param>
        /// <returns>Formatted error message</returns>
        public static string GetInvalidDimensionError(double value)
        {
            return Format(INVALID_DIMENSION, value, ApplicationConstants.MIN_DIMENSION_VALUE, ApplicationConstants.MAX_DIMENSION_VALUE);
        }

        /// <summary>
        /// Gets a validation error for invalid DPI
        /// </summary>
        /// <param name="dpi">Invalid DPI value</param>
        /// <returns>Formatted error message</returns>
        public static string GetInvalidDpiError(int dpi)
        {
            return Format(INVALID_DPI, dpi, ApplicationConstants.MIN_DPI_VALUE, ApplicationConstants.MAX_DPI_VALUE);
        }
        #endregion
    }
}
