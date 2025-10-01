using System;

namespace ZPL2PDF
{
    /// <summary>
    /// Centralized default settings for ZPL2PDF application
    /// </summary>
    public static class DefaultSettings
    {
        // Default dimensions
        public const double DEFAULT_WIDTH_MM = 100.0;  // 4 inches = 101.6mm ≈ 100mm
        public const double DEFAULT_HEIGHT_MM = 150.0; // 6 inches = 152.4mm ≈ 150mm
        public const double DEFAULT_WIDTH_IN = 4.0;    // 4 inches
        public const double DEFAULT_HEIGHT_IN = 6.0;   // 6 inches
        public const double DEFAULT_WIDTH_CM = 10.0;   // 10 cm
        public const double DEFAULT_HEIGHT_CM = 15.0;  // 15 cm

        // Default unit
        public const string DEFAULT_UNIT = "mm";

        // Default DPI
        public const int DEFAULT_DPI = 203; // Default DPI for Zebra printers

        // Conversion factors
        public const double POINTS_TO_MM_FACTOR = 25.4; // Conversion factor from points to mm
        public const double MM_TO_INCH_FACTOR = 0.0393701; // 1mm = 0.0393701 inches
        public const double MM_TO_CM_FACTOR = 0.1; // 1mm = 0.1 cm
        public const double INCH_TO_MM_FACTOR = 25.4; // 1 inch = 25.4 mm
        public const double CM_TO_MM_FACTOR = 10.0; // 1 cm = 10 mm

        /// <summary>
        /// Gets default width in the specified unit
        /// </summary>
        /// <param name="unit">Unit of measurement (mm, cm, in)</param>
        /// <returns>Default width in the specified unit</returns>
        public static double GetDefaultWidth(string unit)
        {
            return unit.ToLowerInvariant() switch
            {
                "mm" => DEFAULT_WIDTH_MM,
                "cm" => DEFAULT_WIDTH_CM,
                "in" => DEFAULT_WIDTH_IN,
                _ => DEFAULT_WIDTH_MM
            };
        }

        /// <summary>
        /// Gets default height in the specified unit
        /// </summary>
        /// <param name="unit">Unit of measurement (mm, cm, in)</param>
        /// <returns>Default height in the specified unit</returns>
        public static double GetDefaultHeight(string unit)
        {
            return unit.ToLowerInvariant() switch
            {
                "mm" => DEFAULT_HEIGHT_MM,
                "cm" => DEFAULT_HEIGHT_CM,
                "in" => DEFAULT_HEIGHT_IN,
                _ => DEFAULT_HEIGHT_MM
            };
        }

        /// <summary>
        /// Converts dimensions from one unit to another
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="fromUnit">Source unit</param>
        /// <param name="toUnit">Target unit</param>
        /// <returns>Converted value</returns>
        public static double ConvertUnit(double value, string fromUnit, string toUnit)
        {
            if (fromUnit == toUnit) return value;

            // Convert to mm first
            double valueInMm = fromUnit.ToLowerInvariant() switch
            {
                "mm" => value,
                "cm" => value * CM_TO_MM_FACTOR,
                "in" => value * INCH_TO_MM_FACTOR,
                _ => value
            };

            // Convert from mm to target unit
            return toUnit.ToLowerInvariant() switch
            {
                "mm" => valueInMm,
                "cm" => valueInMm * MM_TO_CM_FACTOR,
                "in" => valueInMm * MM_TO_INCH_FACTOR,
                _ => valueInMm
            };
        }

        /// <summary>
        /// Gets default dimensions as a LabelDimensions object
        /// </summary>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>Default dimensions</returns>
        public static LabelDimensions GetDefaultDimensions(string unit = DEFAULT_UNIT)
        {
            var width = GetDefaultWidth(unit);
            var height = GetDefaultHeight(unit);
            
            // Convert to mm for internal calculations
            var widthMm = ConvertUnit(width, unit, "mm");
            var heightMm = ConvertUnit(height, unit, "mm");

            return new LabelDimensions
            {
                Width = ConvertMmToPoints(widthMm),
                Height = ConvertMmToPoints(heightMm),
                WidthMm = widthMm,
                HeightMm = heightMm,
                Dpi = DEFAULT_DPI,
                HasDimensions = false
            };
        }

        /// <summary>
        /// Converts millimeters to points
        /// </summary>
        /// <param name="mm">Value in millimeters</param>
        /// <returns>Value in points</returns>
        public static int ConvertMmToPoints(double mm)
        {
            return (int)Math.Round((mm / POINTS_TO_MM_FACTOR) * DEFAULT_DPI);
        }

        /// <summary>
        /// Converts points to millimeters
        /// </summary>
        /// <param name="points">Value in points</param>
        /// <returns>Value in millimeters</returns>
        public static double ConvertPointsToMm(int points)
        {
            return (points / (double)DEFAULT_DPI) * POINTS_TO_MM_FACTOR;
        }
    }
}
