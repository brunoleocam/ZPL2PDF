using System.Collections.Generic;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ZPL2PDF {
    /// <summary>
    /// Responsável por gerar um PDF, adicionando cada imagem (dados em byte[]) em uma página.
    /// </summary>
    public static class PdfGenerator {
        /// <summary>
        /// Gera um PDF com uma imagem por página e salva o arquivo no caminho especificado.
        /// </summary>
        public static void GeneratePdf(List<byte[]> imageDataList, string outputPdfPath) {
            var document = new PdfDocument();
            foreach (var imageData in imageDataList) {
                PdfPage page = document.AddPage();
                using (var ms = new MemoryStream(imageData)) {
                    XImage image = XImage.FromStream(() => ms);
                    page.Width = image.PixelWidth;
                    page.Height = image.PixelHeight;
                    using (XGraphics gfx = XGraphics.FromPdfPage(page)) {
                        gfx.DrawImage(image, 0, 0, image.PixelWidth, image.PixelHeight);
                    }
                }
            }
            document.Save(outputPdfPath);
        }
    }
}