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
                    using (var image = XImage.FromStream(() => new MemoryStream(imageData))) {
                        // Debugging: Print the image dimensions
                        //Console.WriteLine($"Image Width: {image.PixelWidth}, Image Height: {image.PixelHeight}");

                        // Create a new page with the same dimensions as the image
                        var page = document.AddPage();
                        page.Width = image.PixelWidth;
                        page.Height = image.PixelHeight;

                        // Debugging: Print the page dimensions
                        //Console.WriteLine($"Page Width: {page.Width}, Page Height: {page.Height}");

                        using (var graphics = XGraphics.FromPdfPage(page)) {
                            // Draw the image with the correct dimensions
                            graphics.DrawImage(image, 0, 0, image.PixelWidth, image.PixelHeight);
                        }
                    }
                }
                document.Save(outputPdf);
            }
        }

        /// <summary>
        /// Generates a PDF with one image per page and returns it as a byte array.
        /// </summary>
        /// <param name="imageDataList">List of image data in byte arrays.</param>
        /// <returns>PDF file as byte array.</returns>
        public static byte[] GeneratePdfToBytes(List<byte[]> imageDataList) {
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
                
                using (var stream = new MemoryStream()) {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}