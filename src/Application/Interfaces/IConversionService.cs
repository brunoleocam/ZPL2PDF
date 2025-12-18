using System.Collections.Generic;
using ZPL2PDF.Infrastructure.Rendering;

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
        /// <returns>List of image data for PDF generation</returns>
        List<byte[]> ConvertWithExplicitDimensions(string zplContent, double width, double height, string unit, int dpi);

        /// <summary>
        /// Converts ZPL content to PDF by extracting dimensions from ZPL
        /// </summary>
        /// <param name="zplContent">ZPL content to convert</param>
        /// <param name="unit">Unit of measurement for extracted dimensions</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <returns>List of image data for PDF generation</returns>
        List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi);

        /// <summary>
        /// Converts ZPL content to PDF using mixed approach (explicit or extracted)
        /// </summary>
        /// <param name="zplContent">ZPL content to convert</param>
        /// <param name="explicitWidth">Explicit width (0 to extract from ZPL)</param>
        /// <param name="explicitHeight">Explicit height (0 to extract from ZPL)</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <returns>List of image data for PDF generation</returns>
        List<byte[]> Convert(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi);

        /// <summary>
        /// Converts ZPL content directly to PDF using the specified renderer.
        /// For Labelary renderer, generates vectorial PDF with optimal quality.
        /// For BinaryKits renderer, generates PNG images and converts to PDF.
        /// </summary>
        /// <param name="zplContent">ZPL content to convert</param>
        /// <param name="explicitWidth">Explicit width (0 to extract from ZPL)</param>
        /// <param name="explicitHeight">Explicit height (0 to extract from ZPL)</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density in DPI</param>
        /// <param name="rendererMode">Renderer mode (Offline, Labelary, Auto)</param>
        /// <param name="outputPath">Path to save the generated PDF</param>
        void ConvertToPdf(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi, RendererMode rendererMode, string outputPath);

        /// <summary>
        /// Extracts a custom file name from ZPL content using the ^FX FileName: comment syntax.
        /// </summary>
        /// <param name="zplContent">ZPL content that may contain a FileName comment.</param>
        /// <returns>The extracted file name (without extension) if found, or null if not found.</returns>
        string? ExtractFileName(string zplContent);

        /// <summary>
        /// Extracts a FORCED file name from ZPL content using the ^FX !FileName: comment syntax.
        /// The ! prefix indicates maximum priority, overriding even the -n parameter.
        /// </summary>
        /// <param name="zplContent">ZPL content that may contain a forced FileName comment.</param>
        /// <returns>The extracted file name (without extension) if found, or null if not found.</returns>
        string? ExtractForcedFileName(string zplContent);
    }
}
