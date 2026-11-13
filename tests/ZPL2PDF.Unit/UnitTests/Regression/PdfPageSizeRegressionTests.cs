using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Xunit;
using ZPL2PDF;
using ZPL2PDF.Application.Services;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF.Tests.UnitTests.Regression
{
    /// <summary>
    /// Regression for issue #82: PDF page size must be the physical label size in points
    /// (1/72 in), not the rendered dot/pixel size. Runs the same ZPL -> PNG -> PDF path
    /// as the CLI (offline renderer, PDF sized with the engine's effective DPI).
    /// </summary>
    public class PdfPageSizeRegressionTests
    {
        // 4 x 6 in at 203 dpi (8 dpmm): 813 x 1219 dots.
        private const string FourBySixZpl =
            "^XA\n^PW813\n^LL1219\n^FO64,64^A0N,32,32^FD4x6 TEST^FS\n^XZ";

        // Exact page box: dots / 8 dpmm / 25.4 mm-per-in * 72 pt-per-in.
        private const double ExpectedWidthPt = 813 / 8.0 / 25.4 * 72;   // 288.071
        private const double ExpectedHeightPt = 1219 / 8.0 / 25.4 * 72; // 431.929
        private const double Tolerance = 0.01;

        [Fact]
        public void ZplWithPwLl_At203Dpi_ProducesExact4x6InchPageBox()
        {
            const int dpi = 203;
            var images = new ConversionService().ConvertWithExtractedDimensions(FourBySixZpl, "mm", dpi);

            var page = SinglePage(images, dpi);

            page.Width.Point.Should().BeApproximately(ExpectedWidthPt, Tolerance);
            page.Height.Point.Should().BeApproximately(ExpectedHeightPt, Tolerance);
        }

        [Fact]
        public void ZplWithPwLl_At300Dpi_ProducesExact4x6InchPageBox()
        {
            // 4 x 6 in at 300 dpi (12 dpmm): 1218 x 1828 dots.
            const string zpl = "^XA\n^PW1218\n^LL1828\n^FO64,64^A0N,32,32^FD4x6 TEST^FS\n^XZ";
            const int dpi = 300;
            var images = new ConversionService().ConvertWithExtractedDimensions(zpl, "mm", dpi);

            var page = SinglePage(images, dpi);

            page.Width.Point.Should().BeApproximately(1218 / 12.0 / 25.4 * 72, Tolerance);
            page.Height.Point.Should().BeApproximately(1828 / 12.0 / 25.4 * 72, Tolerance);
        }

        [Fact]
        public void ZplWithPwLl_At203Dpi_PageBoxIsNotTheDotSize()
        {
            const int dpi = 203;
            var images = new ConversionService().ConvertWithExtractedDimensions(FourBySixZpl, "mm", dpi);

            var page = SinglePage(images, dpi);

            // The bug wrote [0 0 813 1219] (dots as points, ~2.82x oversized).
            page.Width.Point.Should().BeLessThan(300);
            page.Height.Point.Should().BeLessThan(440);
        }

        private static PdfPage SinglePage(List<byte[]> images, int dpi)
        {
            images.Should().HaveCount(1);

            int effectiveDpi = RendererEngineParser.ToEffectiveDpi(dpi, RendererEngine.Offline);
            var pdf = PdfGenerator.GeneratePdfToBytes(images, effectiveDpi);

            var doc = PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import);
            doc.PageCount.Should().Be(1);
            return doc.Pages[0];
        }
    }
}
