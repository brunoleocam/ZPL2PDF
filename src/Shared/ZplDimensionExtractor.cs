using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZPL2PDF
{
    /// <summary>
    /// Extracts ZPL label dimensions (^PW and ^LL) and converts points to mm
    /// </summary>
    public class ZplDimensionExtractor
    {

        /// <summary>
        /// Extracts dimensions from all labels in a ZPL file
        /// </summary>
        /// <param name="zplContent">Complete ZPL content</param>
        /// <returns>List of dimensions for each label</returns>
        public List<LabelDimensions> ExtractDimensions(string zplContent)
        {
            var dimensions = new List<LabelDimensions>();
            
            if (string.IsNullOrWhiteSpace(zplContent))
                return dimensions;

            // Split into individual labels (^XA...^XZ)
            var labels = SplitLabels(zplContent);
            
            foreach (var label in labels)
            {
                var labelDimensions = ExtractDimensionsFromLabel(label);
                dimensions.Add(labelDimensions);
            }

            return dimensions;
        }

        /// <summary>
        /// Extracts dimensions from an individual label
        /// </summary>
        /// <param name="labelContent">Content of a label (^XA...^XZ)</param>
        /// <returns>Label dimensions</returns>
        public LabelDimensions ExtractDimensionsFromLabel(string labelContent)
        {
            var dimensions = new LabelDimensions
            {
                Width = 0,
                Height = 0,
                WidthMm = 0,
                HeightMm = 0,
                Dpi = DefaultSettings.DEFAULT_DPI,
                HasDimensions = false
            };

            if (string.IsNullOrWhiteSpace(labelContent))
                return dimensions;

            // Extract ^PW (Print Width)
            var pwMatch = Regex.Match(labelContent, @"\^PW(\d+)", RegexOptions.IgnoreCase);
            if (pwMatch.Success && int.TryParse(pwMatch.Groups[1].Value, out int width))
            {
                dimensions.Width = width;
                dimensions.WidthMm = ConvertPointsToMm(width, DefaultSettings.DEFAULT_DPI);
                dimensions.HasDimensions = true;
            }

            // Extract ^LL (Label Length)
            var llMatch = Regex.Match(labelContent, @"\^LL(\d+)", RegexOptions.IgnoreCase);
            if (llMatch.Success && int.TryParse(llMatch.Groups[1].Value, out int height))
            {
                dimensions.Height = height;
                dimensions.HeightMm = ConvertPointsToMm(height, DefaultSettings.DEFAULT_DPI);
                dimensions.HasDimensions = true;
            }

            return dimensions;
        }

        /// <summary>
        /// Converts points to millimeters
        /// </summary>
        /// <param name="points">Value in points</param>
        /// <param name="dpi">Printer DPI</param>
        /// <returns>Value in millimeters</returns>
        public double ConvertPointsToMm(int points, int dpi = DefaultSettings.DEFAULT_DPI)
        {
            return (points / (double)dpi) * DefaultSettings.POINTS_TO_MM_FACTOR;
        }

        /// <summary>
        /// Converts millimeters to points
        /// </summary>
        /// <param name="mm">Value in millimeters</param>
        /// <param name="dpi">Printer DPI</param>
        /// <returns>Value in points</returns>
        public int ConvertMmToPoints(double mm, int dpi = DefaultSettings.DEFAULT_DPI)
        {
            return (int)Math.Round((mm / DefaultSettings.POINTS_TO_MM_FACTOR) * dpi);
        }

        /// <summary>
        /// Divides ZPL content into individual labels
        /// </summary>
        /// <param name="zplContent">Complete ZPL content</param>
        /// <returns>List of individual labels</returns>
        private List<string> SplitLabels(string zplContent)
        {
            var labels = new List<string>();
            
            if (string.IsNullOrWhiteSpace(zplContent))
                return labels;

            // Search for pattern ^XA...^XZ
            var pattern = @"\^XA.*?\^XZ";
            var matches = Regex.Matches(zplContent, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    labels.Add(match.Value);
                }
            }

            return labels;
        }

        /// <summary>
        /// Validates if the extracted dimensions are valid
        /// </summary>
        /// <param name="dimensions">Dimensions to be validated</param>
        /// <returns>True if valid, False otherwise</returns>
        public bool ValidateDimensions(LabelDimensions dimensions)
        {
            if (dimensions == null)
                return false;

            if (dimensions.Width <= 0 || dimensions.Height <= 0)
                return false;

            if (dimensions.WidthMm <= 0 || dimensions.HeightMm <= 0)
                return false;

            // Reasonable size validations (between 1mm and 1000mm)
            if (dimensions.WidthMm < 1 || dimensions.WidthMm > 1000)
                return false;

            if (dimensions.HeightMm < 1 || dimensions.HeightMm > 1000)
                return false;

            return true;
        }

        /// <summary>
        /// Gets default dimensions when it's not possible to extract from ZPL
        /// </summary>
        /// <returns>Default dimensions (100mm x 150mm)</returns>
        public LabelDimensions GetDefaultDimensions()
        {
            return DefaultSettings.GetDefaultDimensions();
        }

        /// <summary>
        /// Applies priority logic for dimensions
        /// </summary>
        /// <param name="explicitWidth">Explicit width (parameter -w)</param>
        /// <param name="explicitHeight">Explicit height (parameter -h)</param>
        /// <param name="explicitUnit">Explicit unit (parameter -u)</param>
        /// <param name="zplDimensions">Dimensions extracted from ZPL</param>
        /// <param name="dpi">DPI to use for conversions</param>
        /// <returns>Final dimensions to be used</returns>
        public LabelDimensions ApplyPriorityLogic(double? explicitWidth, double? explicitHeight, string explicitUnit, LabelDimensions zplDimensions, int dpi = 203)
        {
            var result = new LabelDimensions
            {
                Dpi = dpi,
                HasDimensions = true
            };

            // Priority 1: Dimensions extracted from ZPL (^PW and ^LL)
            if (zplDimensions.HasDimensions && ValidateDimensions(zplDimensions))
            {
                result.Width = zplDimensions.Width;
                result.Height = zplDimensions.Height;
                result.WidthMm = zplDimensions.WidthMm;
                result.HeightMm = zplDimensions.HeightMm;
                result.Dpi = dpi; // Use the provided DPI
                result.Source = "zpl_extraction";
                return result;
            }

            // Priority 2: Explicit parameters (-w and -h)
            if (explicitWidth.HasValue && explicitHeight.HasValue)
            {
                result.Width = ConvertMmToPoints(explicitWidth.Value, dpi);
                result.Height = ConvertMmToPoints(explicitHeight.Value, dpi);
                result.WidthMm = explicitWidth.Value;
                result.HeightMm = explicitHeight.Value;
                result.Dpi = dpi; // Use the provided DPI
                result.Source = "explicit_parameters";
                return result;
            }

            // Priority 3: Default dimensions
            result = GetDefaultDimensions();
            result.Dpi = dpi; // Use the provided DPI
            result.Source = "default";
            return result;
        }
    }

    /// <summary>
    /// Represents the dimensions of a label
    /// </summary>
    public class LabelDimensions
    {
        /// <summary>
        /// Width in points
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height in points
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Width in millimeters
        /// </summary>
        public double WidthMm { get; set; }

        /// <summary>
        /// Height in millimeters
        /// </summary>
        public double HeightMm { get; set; }

        /// <summary>
        /// DPI used for conversion
        /// </summary>
        public int Dpi { get; set; }

        /// <summary>
        /// Indicates if dimensions were successfully extracted
        /// </summary>
        public bool HasDimensions { get; set; }

        /// <summary>
        /// Source of dimensions (explicit_parameters, zpl_extraction, default)
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Returns a string representation of the dimensions
        /// </summary>
        /// <returns>Formatted string with dimensions</returns>
        public override string ToString()
        {
            return $"{WidthMm:F1}mm x {HeightMm:F1}mm ({Width} x {Height} pts @ {Dpi} DPI) [{Source}]";
        }
    }
}
