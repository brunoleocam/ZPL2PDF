using System;
using System.Globalization;
using System.IO;

namespace ZPL2PDF
{
    /// <summary>
    /// Responsible for validating command line arguments
    /// </summary>
    public class ArgumentValidator
    {
        /// <summary>
        /// Validates conversion mode arguments
        /// </summary>
        /// <param name="inputFilePath">Input file path</param>
        /// <param name="zplContent">ZPL content</param>
        /// <param name="outputFolderPath">Output folder path</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>Validation result with error message if invalid</returns>
        public (bool IsValid, string ErrorMessage) ValidateConversionMode(string inputFilePath, string zplContent, string outputFolderPath, double width, double height, string unit)
        {
            // Must have either input file or ZPL content
            if (string.IsNullOrWhiteSpace(inputFilePath) && string.IsNullOrWhiteSpace(zplContent))
            {
                return (false, "Either input file (-i) or ZPL content (-z) must be specified");
            }

            // Cannot have both input file and ZPL content
            if (!string.IsNullOrWhiteSpace(inputFilePath) && !string.IsNullOrWhiteSpace(zplContent))
            {
                return (false, "Cannot specify both input file (-i) and ZPL content (-z)");
            }

            // If input file is specified, validate it exists
            if (!string.IsNullOrWhiteSpace(inputFilePath))
            {
                if (!File.Exists(inputFilePath))
                {
                    return (false, $"Input file does not exist: {inputFilePath}");
                }

                var extension = Path.GetExtension(inputFilePath).ToLowerInvariant();
                var validExtensions = new[] { ".txt", ".prn", ".zpl", ".imp" };
                if (!Array.Exists(validExtensions, ext => ext == extension))
                {
                    return (false, "Input file must be .txt, .prn, .zpl, or .imp");
                }
            }

            // Output folder is required
            if (string.IsNullOrWhiteSpace(outputFolderPath))
            {
                return (false, "Output folder (-o) is required");
            }

            // Validate dimensions if any are specified
            var dimensionValidation = ValidateDimensions(width, height, unit);
            if (!dimensionValidation.IsValid)
            {
                return dimensionValidation;
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates daemon mode arguments
        /// </summary>
        /// <param name="listenFolderPath">Listen folder path</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        /// <returns>Validation result with error message if invalid</returns>
        public (bool IsValid, string ErrorMessage) ValidateDaemonMode(string listenFolderPath, double width, double height, string unit, int dpi)
        {
            // Listen folder is required
            if (string.IsNullOrWhiteSpace(listenFolderPath))
            {
                return (false, "Listen folder (-l) is required for daemon mode");
            }

            // Validate dimensions if any are specified
            var dimensionValidation = ValidateDimensions(width, height, unit);
            if (!dimensionValidation.IsValid)
            {
                return dimensionValidation;
            }

            // Validate DPI
            if (dpi <= 0)
            {
                return (false, "DPI must be greater than 0");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates dimension parameters (width, height, unit)
        /// </summary>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>Validation result with error message if invalid</returns>
        public (bool IsValid, string ErrorMessage) ValidateDimensions(double width, double height, string unit)
        {
            // Check if any dimension parameter is explicitly specified (not default values)
            bool hasWidth = width > 0;
            bool hasHeight = height > 0;
            bool hasUnit = !string.IsNullOrWhiteSpace(unit) && IsValidUnit(unit); // "mm" is valid and can be explicit

            // Check if any dimension is specified - only consider it specified if width OR height is > 0
            if (hasWidth || hasHeight)
            {
                // If any is specified, all must be specified
                if (!hasWidth)
                {
                    return (false, "Width (-w) is required when specifying dimensions");
                }

                if (!hasHeight)
                {
                    return (false, "Height (-h) is required when specifying dimensions");
                }

                if (!hasUnit)
                {
                    return (false, "Unit (-u) is required when specifying dimensions. Use 'in', 'cm', or 'mm'");
                }

                // Validate individual values
                if (width <= 0)
                {
                    return (false, "Width must be greater than 0");
                }

                if (height <= 0)
                {
                    return (false, "Height must be greater than 0");
                }
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Validates if a unit is valid
        /// </summary>
        /// <param name="unit">Unit to validate</param>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValidUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return false;

            return unit.Equals("in", StringComparison.OrdinalIgnoreCase) ||
                unit.Equals("cm", StringComparison.OrdinalIgnoreCase) ||
                   unit.Equals("mm", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validates if a double value can be parsed
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValidDouble(string value)
        {
            return TryParseDouble(value, out _);
        }

        /// <summary>
        /// Tries to parse a double value, accepting both dot (.) and comma (,) as decimal separators.
        /// </summary>
        /// <param name="value">The string value to parse</param>
        /// <param name="result">The parsed double value</param>
        /// <returns>True if parsing was successful</returns>
        public bool TryParseDouble(string value, out double result)
        {
            // First try with the normalized value
            var normalizedValue = NormalizeDecimal(value);
            if (double.TryParse(normalizedValue, NumberStyles.Float, CultureInfo.CurrentCulture, out result))
            {
                return true;
            }
            
            // If that fails, try with invariant culture (dot as decimal separator)
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Normalizes decimal separator to use dot (.) instead of comma (,).
        /// </summary>
        /// <param name="value">The string value to normalize</param>
        /// <returns>The normalized string value</returns>
        private static string NormalizeDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            
            return value.Replace(',', '.');
        }
    }
}
