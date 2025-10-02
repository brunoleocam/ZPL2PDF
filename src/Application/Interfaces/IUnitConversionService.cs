namespace ZPL2PDF.Application.Interfaces
{
    /// <summary>
    /// Interface for unit conversion service
    /// </summary>
    public interface IUnitConversionService
    {
        /// <summary>
        /// Converts dimensions from one unit to another
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="fromUnit">Source unit</param>
        /// <param name="toUnit">Target unit</param>
        /// <returns>Converted value</returns>
        double ConvertUnit(double value, string fromUnit, string toUnit);

        /// <summary>
        /// Converts millimeters to points
        /// </summary>
        /// <param name="mm">Value in millimeters</param>
        /// <param name="dpi">Printer DPI</param>
        /// <returns>Value in points</returns>
        int ConvertMmToPoints(double mm, int dpi = 203);

        /// <summary>
        /// Converts points to millimeters
        /// </summary>
        /// <param name="points">Value in points</param>
        /// <param name="dpi">Printer DPI</param>
        /// <returns>Value in millimeters</returns>
        double ConvertPointsToMm(int points, int dpi = 203);

        /// <summary>
        /// Converts width and height to millimeters based on unit
        /// </summary>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>Tuple with width and height in millimeters</returns>
        (double widthMm, double heightMm) ConvertToMillimeters(double width, double height, string unit);
    }
}
