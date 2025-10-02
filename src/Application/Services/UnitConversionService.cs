using System;
using ZPL2PDF.Application.Interfaces;

namespace ZPL2PDF.Application.Services
{
    /// <summary>
    /// Service responsible for unit conversions
    /// </summary>
    public class UnitConversionService : IUnitConversionService
    {
        // Conversion factors
        private const double POINTS_TO_MM_FACTOR = 25.4; // Conversion factor from points to mm
        private const double MM_TO_INCH_FACTOR = 0.0393701; // 1mm = 0.0393701 inches
        private const double MM_TO_CM_FACTOR = 0.1; // 1mm = 0.1 cm
        private const double INCH_TO_MM_FACTOR = 25.4; // 1 inch = 25.4 mm
        private const double CM_TO_MM_FACTOR = 10.0; // 1 cm = 10 mm

        /// <summary>
        /// Converts dimensions from one unit to another
        /// </summary>
        public double ConvertUnit(double value, string fromUnit, string toUnit)
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
        /// Converts millimeters to points
        /// </summary>
        public int ConvertMmToPoints(double mm, int dpi = 203)
        {
            return (int)Math.Round((mm * dpi) / POINTS_TO_MM_FACTOR);
        }

        /// <summary>
        /// Converts points to millimeters
        /// </summary>
        public double ConvertPointsToMm(int points, int dpi = 203)
        {
            return (points / (double)dpi) * POINTS_TO_MM_FACTOR;
        }

        /// <summary>
        /// Converts width and height to millimeters based on unit
        /// </summary>
        public (double widthMm, double heightMm) ConvertToMillimeters(double width, double height, string unit)
        {
            var widthMm = ConvertUnit(width, unit, "mm");
            var heightMm = ConvertUnit(height, unit, "mm");
            return (widthMm, heightMm);
        }
    }
}
