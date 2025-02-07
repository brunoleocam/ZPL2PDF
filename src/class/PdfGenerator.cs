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
            using (var document = new PdfDocument()) {
                foreach (var imageData in imageDataList) {
                    var page = document.AddPage();
                    using (var graphics = XGraphics.FromPdfPage(page)) {
                        using (var image = XImage.FromStream(() => new MemoryStream(imageData))) {
                            graphics.DrawImage(image, 0, 0, page.Width, page.Height);
                        }
                    }
                }
                document.Save(outputPdf);
            }
        }
    }
}