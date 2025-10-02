using System.Collections.Generic;

namespace ZPL2PDF.Domain.Services
{
    /// <summary>
    /// Interface for PDF generation service
    /// </summary>
    public interface IPdfGenerator
    {
        /// <summary>
        /// Generates a PDF file from image data
        /// </summary>
        /// <param name="imageDataList">List of image data as byte arrays</param>
        /// <param name="outputPath">Output file path for the PDF</param>
        void GeneratePdf(List<byte[]> imageDataList, string outputPath);

        /// <summary>
        /// Generates a PDF file from image data with custom settings
        /// </summary>
        /// <param name="imageDataList">List of image data as byte arrays</param>
        /// <param name="outputPath">Output file path for the PDF</param>
        /// <param name="pageWidth">Page width in points</param>
        /// <param name="pageHeight">Page height in points</param>
        void GeneratePdf(List<byte[]> imageDataList, string outputPath, double pageWidth, double pageHeight);
    }
}
