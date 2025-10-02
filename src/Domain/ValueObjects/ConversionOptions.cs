using System;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF.Domain.ValueObjects
{
    /// <summary>
    /// Represents conversion options for ZPL to PDF conversion
    /// </summary>
    public class ConversionOptions
    {
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
        /// Gets or sets the output file name
        /// </summary>
        public string OutputFileName { get; set; } = "output.pdf";

        /// <summary>
        /// Gets or sets the output folder path
        /// </summary>
        public string OutputFolderPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to use explicit dimensions
        /// </summary>
        public bool UseExplicitDimensions { get; set; }

        /// <summary>
        /// Gets or sets whether to extract dimensions from ZPL
        /// </summary>
        public bool ExtractDimensionsFromZpl { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of ConversionOptions
        /// </summary>
        public ConversionOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of ConversionOptions with explicit dimensions
        /// </summary>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        public ConversionOptions(double width, double height, string unit, int dpi)
        {
            Width = width;
            Height = height;
            Unit = unit;
            Dpi = dpi;
            UseExplicitDimensions = true;
            ExtractDimensionsFromZpl = false;
        }

        /// <summary>
        /// Initializes a new instance of ConversionOptions for ZPL extraction
        /// </summary>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        public ConversionOptions(string unit, int dpi)
        {
            Unit = unit;
            Dpi = dpi;
            UseExplicitDimensions = false;
            ExtractDimensionsFromZpl = true;
        }

        /// <summary>
        /// Validates the conversion options
        /// </summary>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Unit))
                return false;

            if (Dpi <= 0)
                return false;

            if (UseExplicitDimensions)
            {
                if (Width <= 0 || Height <= 0)
                    return false;
            }

            if (string.IsNullOrWhiteSpace(OutputFolderPath))
                return false;

            return true;
        }

        /// <summary>
        /// Gets validation error message
        /// </summary>
        /// <returns>Error message if invalid, empty string if valid</returns>
        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(Unit))
                return "Unit cannot be null or empty";

            if (Dpi <= 0)
                return "DPI must be greater than 0";

            if (UseExplicitDimensions)
            {
                if (Width <= 0)
                    return "Width must be greater than 0";
                if (Height <= 0)
                    return "Height must be greater than 0";
            }

            if (string.IsNullOrWhiteSpace(OutputFolderPath))
                return "Output folder path cannot be null or empty";

            return string.Empty;
        }

        /// <summary>
        /// Creates a copy of the conversion options
        /// </summary>
        /// <returns>New instance with same values</returns>
        public ConversionOptions Clone()
        {
            return new ConversionOptions
            {
                Width = Width,
                Height = Height,
                Unit = Unit,
                Dpi = Dpi,
                OutputFileName = OutputFileName,
                OutputFolderPath = OutputFolderPath,
                UseExplicitDimensions = UseExplicitDimensions,
                ExtractDimensionsFromZpl = ExtractDimensionsFromZpl
            };
        }

        /// <summary>
        /// Returns a string representation of the conversion options
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            if (UseExplicitDimensions)
            {
                return $"ConversionOptions: {Width}x{Height} {Unit}, Print Density: {ApplicationConstants.ConvertDpiToDpmm(ApplicationConstants.DEFAULT_DPI):F1} dpmm ({ApplicationConstants.DEFAULT_DPI} dpi)";
            }
            else
            {
                return $"ConversionOptions: Extract from ZPL, Unit: {Unit}, Print Density: {ApplicationConstants.ConvertDpiToDpmm(ApplicationConstants.DEFAULT_DPI):F1} dpmm ({ApplicationConstants.DEFAULT_DPI} dpi)";
            }
        }
    }
}
