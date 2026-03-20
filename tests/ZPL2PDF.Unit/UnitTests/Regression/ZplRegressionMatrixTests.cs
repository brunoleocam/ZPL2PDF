using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using ZPL2PDF;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF.Tests.UnitTests.Regression
{
    public class ZplRegressionMatrixTests
    {
        private const string Utf8SuiteZpl = "^XA^FH^FD_C3_A3^FS^XZ";

        // Minimal Aztec-like sequence from existing unit tests (covers ^B0 -> ^BO workaround + barcode payload _XX preservation).
        private const string AztecSuiteZpl = "^XA^B0N,4,N,0,N,1^FH^FD[)>_1D03_1D75^FS^XZ";

        [Fact]
        public void PreprocessZpl_WithUtf8AndAztecSuites_ShouldApplyNormalizationWithoutCorruptingPayload()
        {
            var processedUtf8 = LabelFileReader.PreprocessZpl(Utf8SuiteZpl);
            var processedAztec = LabelFileReader.PreprocessZpl(AztecSuiteZpl);

            processedUtf8.Should().Contain("^FDã^FS");

            processedAztec.Should().Contain("^BON");
            processedAztec.Should().NotContain("^B0", because: "Aztec 2D barcode command should be normalized to ^BO.");
            processedAztec.Should().Contain("_1D03");
            processedAztec.Should().Contain("_1D75");
        }

        [Fact]
        public void ConvertPipeline_Suites_ShouldGenerateValidPngAndPdfBytes()
        {
            var conversionService = new ConversionService();

            var sw = Stopwatch.StartNew();

            var pngImagesUtf8 = conversionService.ConvertWithExplicitDimensions(Utf8SuiteZpl, 80, 40, "mm", 203);
            var pngImagesAztec = conversionService.ConvertWithExplicitDimensions(AztecSuiteZpl, 120, 60, "mm", 203);

            var pdfBytesUtf8 = PdfGenerator.GeneratePdfToBytes(pngImagesUtf8);
            var pdfBytesAztec = PdfGenerator.GeneratePdfToBytes(pngImagesAztec);

            sw.Stop();

            pngImagesUtf8.Should().NotBeNull().And.NotBeEmpty();
            pngImagesAztec.Should().NotBeNull().And.NotBeEmpty();

            AssertPngBytesAreValid(pngImagesUtf8);
            AssertPngBytesAreValid(pngImagesAztec);

            pdfBytesUtf8.Should().NotBeNull();
            pdfBytesAztec.Should().NotBeNull();
            pdfBytesUtf8.Length.Should().BeGreaterThan(1000);
            pdfBytesAztec.Length.Should().BeGreaterThan(1000);

            // PDF header: %PDF-
            Encoding.ASCII.GetString(pdfBytesUtf8, 0, 5).Should().Be("%PDF-");
            Encoding.ASCII.GetString(pdfBytesAztec, 0, 5).Should().Be("%PDF-");

            // Keep regressions bounded (renderer should not hang).
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(20));
        }

        private static void AssertPngBytesAreValid(System.Collections.Generic.List<byte[]> pngImages)
        {
            foreach (var png in pngImages)
            {
                png.Should().NotBeNull();
                // Small labels can produce small PNGs; we only require a minimal payload.
                png.Length.Should().BeGreaterThan(50);

                // PNG header: 89 50 4E 47 0D 0A 1A 0A
                png.Take(8).Should().Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            }
        }
    }
}

