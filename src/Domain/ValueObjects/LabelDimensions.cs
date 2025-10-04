using System;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF.Domain.ValueObjects
{
    /// <summary>
    /// Represents label dimensions with validation and conversion methods
    /// </summary>
    public class LabelDimensions
    {
        /// <summary>
        /// Gets or sets the width
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height
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
        /// Initializes a new instance of LabelDimensions
        /// </summary>
        public LabelDimensions()
        {
        }

        /// <summary>
        /// Initializes a new instance of LabelDimensions with values
        /// </summary>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        public LabelDimensions(double width, double height, string unit = "mm", int dpi = 203)
        {
            Width = width;
            Height = height;
            Unit = unit;
            Dpi = dpi;
        }

        /// <summary>
        /// Validates the label dimensions
        /// </summary>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValid()
        {
            if (Width <= 0)
                return false;

            if (Height <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(Unit))
                return false;

            if (Dpi <= 0)
                return false;

            return IsValidUnit(Unit);
        }

        /// <summary>
        /// Gets validation error message
        /// </summary>
        /// <returns>Error message if invalid, empty string if valid</returns>
        public string GetValidationError()
        {
            if (Width <= 0)
                return "Width must be greater than 0";

            if (Height <= 0)
                return "Height must be greater than 0";

            if (string.IsNullOrWhiteSpace(Unit))
                return "Unit cannot be null or empty";

            if (Dpi <= 0)
                return "DPI must be greater than 0";

            if (!IsValidUnit(Unit))
                return $"Invalid unit: {Unit}. Valid units are: mm, cm, in";

            return string.Empty;
        }

        /// <summary>
        /// Checks if the unit is valid
        /// </summary>
        /// <param name="unit">Unit to check</param>
        /// <returns>True if valid, False otherwise</returns>
        public static bool IsValidUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return false;

            var validUnits = new[] { "mm", "cm", "in" };
            return Array.Exists(validUnits, u => 
                u.Equals(unit, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Converts dimensions to millimeters
        /// </summary>
        /// <returns>New LabelDimensions instance in millimeters</returns>
        public LabelDimensions ToMillimeters()
        {
            if (Unit.Equals("mm", StringComparison.OrdinalIgnoreCase))
                return Clone();

            var widthMm = ConvertToMillimeters(Width, Unit);
            var heightMm = ConvertToMillimeters(Height, Unit);

            return new LabelDimensions(widthMm, heightMm, "mm", Dpi);
        }

        /// <summary>
        /// Converts dimensions to centimeters
        /// </summary>
        /// <returns>New LabelDimensions instance in centimeters</returns>
        public LabelDimensions ToCentimeters()
        {
            var mmDimensions = ToMillimeters();
            return new LabelDimensions(
                mmDimensions.Width / 10.0,
                mmDimensions.Height / 10.0,
                "cm",
                Dpi
            );
        }

        /// <summary>
        /// Converts dimensions to inches
        /// </summary>
        /// <returns>New LabelDimensions instance in inches</returns>
        public LabelDimensions ToInches()
        {
            var mmDimensions = ToMillimeters();
            return new LabelDimensions(
                mmDimensions.Width / 25.4,
                mmDimensions.Height / 25.4,
                "in",
                Dpi
            );
        }

        /// <summary>
        /// Converts a value to millimeters based on unit
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="fromUnit">Source unit</param>
        /// <returns>Value in millimeters</returns>
        private static double ConvertToMillimeters(double value, string fromUnit)
        {
            return fromUnit.ToLowerInvariant() switch
            {
                "mm" => value,
                "cm" => value * 10.0,
                "in" => value * 25.4,
                _ => value
            };
        }

        /// <summary>
        /// Converts dimensions to points (for PDF generation)
        /// </summary>
        /// <returns>Tuple with width and height in points</returns>
        public (int widthPoints, int heightPoints) ToPoints()
        {
            var mmDimensions = ToMillimeters();
            var widthPoints = (int)Math.Round((mmDimensions.Width / 25.4) * Dpi);
            var heightPoints = (int)Math.Round((mmDimensions.Height / 25.4) * Dpi);
            return (widthPoints, heightPoints);
        }

        /// <summary>
        /// Creates a copy of the label dimensions
        /// </summary>
        /// <returns>New instance with same values</returns>
        public LabelDimensions Clone()
        {
            return new LabelDimensions(Width, Height, Unit, Dpi);
        }

        /// <summary>
        /// Returns a string representation of the label dimensions
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"LabelDimensions: {Width}x{Height} {Unit}, Print Density: {ApplicationConstants.ConvertDpiToDpmm(ApplicationConstants.DEFAULT_DPI):F1} dpmm ({ApplicationConstants.DEFAULT_DPI} dpi)";
        }

        /// <summary>
        /// Determines if two LabelDimensions instances are equal
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if equal, False otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj is LabelDimensions other)
            {
                return Math.Abs(Width - other.Width) < 0.001 &&
                       Math.Abs(Height - other.Height) < 0.001 &&
                       Unit.Equals(other.Unit, StringComparison.OrdinalIgnoreCase) &&
                       Dpi == other.Dpi;
            }
            return false;
        }

        /// <summary>
        /// Gets hash code for the label dimensions
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height, Unit, Dpi);
        }
    }
}
