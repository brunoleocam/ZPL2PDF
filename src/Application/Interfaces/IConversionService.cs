using System.Collections.Generic;

namespace ZPL2PDF.Application.Interfaces
{
    /// <summary>
    /// Interface for ZPL to PDF conversion service
    /// </summary>
    public interface IConversionService
    {
        /// <summary>
        /// Converts ZPL content to PDF using explicit dimensions
        /// </summary>
        /// <param name="zplContent">ZPL content to convert</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <param name="fontsDirectory">Optional directory for TTF/OTF fonts (--fonts-dir)</param>
        /// <param name="fontMappings">Optional font ID to path mappings (--font "A=path.ttf")</param>
        /// <returns>List of image data for PDF generation</returns>
        List<byte[]> ConvertWithExplicitDimensions(string zplContent, double width, double height, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null);

        /// <summary>
        /// Converts ZPL content to PDF by extracting dimensions from ZPL
        /// </summary>
        /// <param name="zplContent">ZPL content to convert</param>
        /// <param name="unit">Unit of measurement for extracted dimensions</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <param name="fontsDirectory">Optional directory for TTF/OTF fonts</param>
        /// <param name="fontMappings">Optional font ID to path mappings</param>
        /// <returns>List of image data for PDF generation</returns>
        List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null);

        /// <summary>
        /// Converts ZPL content to PDF using mixed approach (explicit or extracted)
        /// </summary>
        /// <param name="zplContent">ZPL content to convert</param>
        /// <param name="explicitWidth">Explicit width (0 to extract from ZPL)</param>
        /// <param name="explicitHeight">Explicit height (0 to extract from ZPL)</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <param name="fontsDirectory">Optional directory for TTF/OTF fonts</param>
        /// <param name="fontMappings">Optional font ID to path mappings</param>
        /// <returns>List of image data for PDF generation</returns>
        List<byte[]> Convert(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null);
    }
}
