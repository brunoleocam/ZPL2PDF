using System;
using System.Collections.Generic;

namespace ZPL2PDF.Shared.Constants
{
    /// <summary>
    /// Centralized application constants
    /// </summary>
    public static class ApplicationConstants
    {
        #region Application Information
        public const string APPLICATION_NAME = "ZPL2PDF";
        public const string APPLICATION_VERSION = "2.0.0";
        public const string APPLICATION_DESCRIPTION = "ZPL to PDF Converter";
        #endregion

        #region File Extensions
        public static readonly string[] VALID_FILE_EXTENSIONS = { ".txt", ".prn" };
        public const string DEFAULT_FILE_FILTER = "*.txt;*.prn";
        public const string PDF_EXTENSION = ".pdf";
        public const string CONFIG_EXTENSION = ".json";
        public const string PID_EXTENSION = ".pid";
        #endregion

        #region Default Values
        public const string DEFAULT_UNIT = "mm";
        public const int DEFAULT_DPI = 203;
        public const double DEFAULT_PRINT_DENSITY_DPMM = 8.0; // 203 DPI = 8 dpmm
        public const double DPI_TO_DPMM_FACTOR = 25.4; // 1 inch = 25.4mm
        public const int DEFAULT_RETRY_DELAY_MS = 2000;
        public const int DEFAULT_MAX_RETRIES = 3;
        public const string DEFAULT_LOG_LEVEL = "Info";
        public const bool DEFAULT_INCLUDE_SUBDIRECTORIES = false;
        #endregion

        #region Dimensions (Default Values)
        public const double DEFAULT_WIDTH_MM = 100.0;  // 4 inches = 101.6mm ≈ 100mm
        public const double DEFAULT_HEIGHT_MM = 150.0; // 6 inches = 152.4mm ≈ 150mm
        public const double DEFAULT_WIDTH_IN = 4.0;    // 4 inches
        public const double DEFAULT_HEIGHT_IN = 6.0;   // 6 inches
        public const double DEFAULT_WIDTH_CM = 10.0;   // 10 cm
        public const double DEFAULT_HEIGHT_CM = 15.0;  // 15 cm
        #endregion

        #region Conversion Factors
        public const double POINTS_TO_MM_FACTOR = 25.4; // Conversion factor from points to mm
        public const double MM_TO_INCH_FACTOR = 0.0393701; // 1mm = 0.0393701 inches
        public const double MM_TO_CM_FACTOR = 0.1; // 1mm = 0.1 cm
        public const double INCH_TO_MM_FACTOR = 25.4; // 1 inch = 25.4 mm
        public const double CM_TO_MM_FACTOR = 10.0; // 1 cm = 10 mm
        #endregion

        #region Units
        public static readonly string[] SUPPORTED_UNITS = { "mm", "cm", "in", "pt" };
        public static readonly string[] SUPPORTED_LOG_LEVELS = { "Debug", "Info", "Warning", "Error" };
        #endregion

        #region ZPL Commands
        public const string ZPL_START_COMMAND = "^XA";
        public const string ZPL_END_COMMAND = "^XZ";
        public const string ZPL_WIDTH_COMMAND = "^PW";
        public const string ZPL_LENGTH_COMMAND = "^LL";
        #endregion

        #region Folder Names
        public const string PROCESSED_FOLDER = "Processed";
        public const string ERROR_FOLDER = "Error";
        public const string CONFIG_FOLDER = "Config";
        public const string LOGS_FOLDER = "Logs";
        #endregion

        #region Configuration
        public const string CONFIG_FILE_NAME = "zpl2pdf.json";
        public const string PID_FILE_NAME = "zpl2pdf.pid";
        #endregion

        #region Timeouts and Delays
        public const int FILE_LOCK_CHECK_DELAY_MS = 100;
        public const int MAX_FILE_LOCK_WAIT_MS = 5000;
        public const int FOLDER_MONITOR_INTERVAL_MS = 1000;
        #endregion

        #region Validation
        public const double MIN_DIMENSION_VALUE = 0.1;
        public const double MAX_DIMENSION_VALUE = 10000.0;
        public const int MIN_DPI_VALUE = 72;
        public const int MAX_DPI_VALUE = 600;
        #endregion

        #region Error Codes
        public const int SUCCESS_EXIT_CODE = 0;
        public const int ERROR_EXIT_CODE = 1;
        public const int INVALID_ARGUMENTS_EXIT_CODE = 2;
        public const int FILE_NOT_FOUND_EXIT_CODE = 3;
        public const int CONVERSION_ERROR_EXIT_CODE = 4;
        public const int DAEMON_ERROR_EXIT_CODE = 5;
        #endregion

        #region Help and Usage
        public const string USAGE_EXAMPLES = @"
Examples:
  # Conversion mode:
  ZPL2PDF.exe -i input.txt -o output.pdf -w 7.5 -h 15 -u in
  ZPL2PDF.exe -z ""^XA...^XZ"" -o output.pdf -w 7.5 -h 15 -u in

  # Daemon mode:
  ZPL2PDF.exe start
  ZPL2PDF.exe start -l ""C:\Custom Path""
  ZPL2PDF.exe start -w 7.5 -h 15 -u in
  ZPL2PDF.exe stop
  ZPL2PDF.exe status";
        #endregion

        #region Conversion Methods
        /// <summary>
        /// Converts DPI to dpmm (dots per millimeter)
        /// </summary>
        /// <param name="dpi">DPI value</param>
        /// <returns>dpmm value</returns>
        public static double ConvertDpiToDpmm(int dpi)
        {
            return dpi / DPI_TO_DPMM_FACTOR;
        }
        #endregion
    }
}
