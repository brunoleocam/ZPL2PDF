using System.Collections.Generic;

namespace ZPL2PDF.Domain.Services
{
    /// <summary>
    /// Interface for ZPL dimension extraction service
    /// </summary>
    public interface IDimensionExtractor
    {
        /// <summary>
        /// Extracts dimensions from ZPL content
        /// </summary>
        /// <param name="zplContent">ZPL content to analyze</param>
        /// <returns>List of extracted dimensions for each label</returns>
        List<LabelDimensions> ExtractDimensions(string zplContent);

        /// <summary>
        /// Extracts dimensions from a single ZPL label
        /// </summary>
        /// <param name="zplLabel">Single ZPL label string</param>
        /// <returns>Extracted dimensions or null if not found</returns>
        LabelDimensions ExtractDimensionsFromLabel(string zplLabel);

        /// <summary>
        /// Applies priority logic to determine final dimensions
        /// </summary>
        /// <param name="explicitWidth">Explicit width value</param>
        /// <param name="explicitHeight">Explicit height value</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="extractedDimensions">Extracted dimensions from ZPL</param>
        /// <param name="dpi">DPI to use for conversions</param>
        /// <returns>Final dimensions to use</returns>
        LabelDimensions ApplyPriorityLogic(double? explicitWidth, double? explicitHeight, string unit, LabelDimensions extractedDimensions, int dpi = 203);

        /// <summary>
        /// Converts points to millimeters
        /// </summary>
        /// <param name="points">Value in points</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <returns>Value in millimeters</returns>
        double ConvertPointsToMm(int points, int dpi = 203);
    }

    /// <summary>
    /// Represents label dimensions
    /// </summary>
    public class LabelDimensions
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public string Unit { get; set; } = "mm";
        public int Dpi { get; set; } = 203;
    }
}
