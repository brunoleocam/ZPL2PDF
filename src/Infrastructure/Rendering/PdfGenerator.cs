using System.Collections.Generic;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for generating a PDF, adding each image (byte[] data) to a page.
    /// </summary>
    public static class PdfGenerator {
        /// <summary>
        /// Default print density used when the caller does not know the DPI of the rendered images.
        /// </summary>
        public const int DefaultDpi = 203;

        private const double PointsPerInch = 72.0;
        private const double MmPerInch = 25.4;

        /// <summary>
        /// Converts a size in rendered pixels to PDF points (1/72 inch).
        /// Renderers draw at an integer dots-per-mm value (<c>round(dpi / 25.4)</c>, e.g. 203 dpi -> 8 dpmm),
        /// so the effective density of the image is <c>dpmm * 25.4</c>, not the nominal DPI.
        /// </summary>
        /// <param name="pixels">Size in pixels.</param>
        /// <param name="dpi">Nominal print density used to render the image.</param>
        public static double PixelsToPoints(int pixels, int dpi) {
            int dpmm = (int)System.Math.Round((dpi > 0 ? dpi : DefaultDpi) / MmPerInch);
            if (dpmm < 1) dpmm = 1;
            return pixels * PointsPerInch / (dpmm * MmPerInch);
        }

        /// <summary>
        /// Generates a PDF with one image per page and saves the file to the specified path.
        /// Page size is the label's physical size (pixels converted to points using <paramref name="dpi"/>).
        /// </summary>
        /// <param name="imageDataList">List of image data in byte arrays.</param>
        /// <param name="outputPdf">Path to save the generated PDF file.</param>
        /// <param name="dpi">Print density the images were rendered at.</param>
        public static void GeneratePdf(List<byte[]> imageDataList, string outputPdf, int dpi = DefaultDpi) {
            using (var document = new PdfDocument()) {
                AddImagePages(document, imageDataList, dpi);
                document.Save(outputPdf);
            }
        }

        /// <summary>
        /// Generates a PDF with one image per page and returns it as a byte array.
        /// Page size is the label's physical size (pixels converted to points using <paramref name="dpi"/>).
        /// </summary>
        /// <param name="imageDataList">List of image data in byte arrays.</param>
        /// <param name="dpi">Print density the images were rendered at.</param>
        /// <returns>PDF file as byte array.</returns>
        public static byte[] GeneratePdfToBytes(List<byte[]> imageDataList, int dpi = DefaultDpi) {
            using (var document = new PdfDocument()) {
                AddImagePages(document, imageDataList, dpi);

                using (var stream = new MemoryStream()) {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }

        private static void AddImagePages(PdfDocument document, List<byte[]> imageDataList, int dpi) {
            foreach (var imageData in imageDataList) {
                using (var image = XImage.FromStream(new MemoryStream(imageData))) {
                    double widthPt = PixelsToPoints(image.PixelWidth, dpi);
                    double heightPt = PixelsToPoints(image.PixelHeight, dpi);

                    var page = document.AddPage();
                    page.Width = XUnit.FromPoint(widthPt);
                    page.Height = XUnit.FromPoint(heightPt);

                    using (var graphics = XGraphics.FromPdfPage(page)) {
                        graphics.DrawImage(image, 0, 0, widthPt, heightPt);
                    }
                }
            }
        }

        /// <summary>
        /// Merges multiple PDF documents (given as bytes) into a single PDF.
        /// </summary>
        public static byte[] MergePdfsToBytes(List<byte[]> pdfDocuments)
        {
            using var outputDocument = new PdfDocument();

            foreach (var pdfBytes in pdfDocuments)
            {
                if (pdfBytes == null || pdfBytes.Length == 0)
                    continue;

                using var ms = new MemoryStream(pdfBytes);
                using var inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import);

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    outputDocument.AddPage(inputDocument.Pages[i]);
                }
            }

            using var outStream = new MemoryStream();
            outputDocument.Save(outStream, false);
            return outStream.ToArray();
        }
    }
}