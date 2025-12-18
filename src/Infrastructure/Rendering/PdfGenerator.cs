using System.Collections.Generic;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for generating a PDF, adding each image (byte[] data) to a page.
    /// </summary>
    public static class PdfGenerator {
        /// <summary>
        /// Generates a PDF with one image per page and saves the file to the specified path.
        /// </summary>
        /// <param name="imageDataList">List of image data in byte arrays.</param>
        /// <param name="outputPdf">Path to save the generated PDF file.</param>
        public static void GeneratePdf(List<byte[]> imageDataList, string outputPdf) {
            var pdfBytes = GeneratePdfToMemory(imageDataList);
            File.WriteAllBytes(outputPdf, pdfBytes);
        }

        /// <summary>
        /// Generates a PDF with one image per page and returns it as a byte array.
        /// </summary>
        /// <param name="imageDataList">List of image data in byte arrays (PNG format).</param>
        /// <returns>PDF document as a byte array.</returns>
        public static byte[] GeneratePdfToMemory(List<byte[]> imageDataList) {
            using (var document = new PdfDocument()) {
                foreach (var imageData in imageDataList) {
                    using (var image = XImage.FromStream(() => new MemoryStream(imageData))) {
                        // Create a new page with the same dimensions as the image
                        var page = document.AddPage();
                        page.Width = image.PixelWidth;
                        page.Height = image.PixelHeight;

                        using (var graphics = XGraphics.FromPdfPage(page)) {
                            // Draw the image with the correct dimensions
                            graphics.DrawImage(image, 0, 0, image.PixelWidth, image.PixelHeight);
                        }
                    }
                }

                using (var memoryStream = new MemoryStream()) {
                    document.Save(memoryStream, false);
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Saves a raw PDF byte array to a file.
        /// Used when the PDF is already generated (e.g., from Labelary API).
        /// </summary>
        /// <param name="pdfData">PDF data as byte array.</param>
        /// <param name="outputPdf">Path to save the PDF file.</param>
        public static void SavePdf(byte[] pdfData, string outputPdf) {
            File.WriteAllBytes(outputPdf, pdfData);
        }

        /// <summary>
        /// Merges multiple PDF byte arrays into a single PDF file.
        /// Used when Labelary generates multiple PDFs (batches of 50 labels).
        /// </summary>
        /// <param name="pdfDataList">List of PDF data byte arrays.</param>
        /// <param name="outputPdf">Path to save the merged PDF file.</param>
        public static void MergePdfs(List<byte[]> pdfDataList, string outputPdf) {
            if (pdfDataList == null || pdfDataList.Count == 0)
                return;

            // If only one PDF, just save it directly
            if (pdfDataList.Count == 1) {
                SavePdf(pdfDataList[0], outputPdf);
                return;
            }

            // Merge multiple PDFs
            using (var outputDocument = new PdfDocument()) {
                foreach (var pdfData in pdfDataList) {
                    using (var inputStream = new MemoryStream(pdfData)) {
                        using (var inputDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(inputStream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import)) {
                            // Copy all pages from input to output
                            for (int i = 0; i < inputDocument.PageCount; i++) {
                                outputDocument.AddPage(inputDocument.Pages[i]);
                            }
                        }
                    }
                }
                outputDocument.Save(outputPdf);
            }
        }
    }
}