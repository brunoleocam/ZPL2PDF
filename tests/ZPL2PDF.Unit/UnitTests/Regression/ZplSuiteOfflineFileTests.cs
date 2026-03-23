using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using ZPL2PDF;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF.Tests.UnitTests.Regression
{
    /// <summary>
    /// Offline regression over versioned ZPL files under TestData/ZplSuite (copied to output for CI).
    /// </summary>
    public class ZplSuiteOfflineFileTests
    {
        private const int Dpi = 203;
        private const string Unit = "mm";

        private static string SuiteDirectory =>
            Path.Combine(AppContext.BaseDirectory, "TestData", "ZplSuite");

        public static IEnumerable<object[]> PreprocessCases()
        {
            yield return new object[]
            {
                "utf8_fh.zpl",
                new[] { "^FDã^FS" },
                Array.Empty<string>()
            };
            yield return new object[]
            {
                "aztec_b0.zpl",
                new[] { "^BON", "_1D03", "_1D75" },
                new[] { "^B0" }
            };
            yield return new object[]
            {
                "user_label.zpl",
                new[] { "^BON", "_1D03", "_1D75" },
                new[] { "^B0" }
            };
        }

        public static IEnumerable<object[]> ConversionCases()
        {
            // file, useExtractedDimensions, widthMm, heightMm (ignored when extracted)
            yield return new object[] { "utf8_fh.zpl", false, 80, 40 };
            yield return new object[] { "aztec_b0.zpl", false, 120, 60 };
            yield return new object[] { "user_label.zpl", true, 0, 0 };
        }

        [Theory]
        [MemberData(nameof(PreprocessCases))]
        public void PreprocessZpl_SuiteFiles_ShouldMatchExpectations(
            string fileName,
            string[] mustContain,
            string[] mustNotContain)
        {
            var path = Path.Combine(SuiteDirectory, fileName);
            File.Exists(path).Should().BeTrue($"Missing suite file at {path} (ensure csproj copies TestData/ZplSuite).");

            var zpl = File.ReadAllText(path);
            var processed = LabelFileReader.PreprocessZpl(zpl);

            foreach (var fragment in mustContain)
            {
                processed.Should().Contain(fragment, $"file: {fileName}");
            }

            foreach (var fragment in mustNotContain)
            {
                processed.Should().NotContain(fragment, $"file: {fileName}");
            }
        }

        [Theory]
        [MemberData(nameof(ConversionCases))]
        public void OfflineConversion_SuiteFiles_ShouldGenerateValidPngAndPdf(
            string fileName,
            bool useExtractedDimensions,
            int widthMm,
            int heightMm)
        {
            var path = Path.Combine(SuiteDirectory, fileName);
            File.Exists(path).Should().BeTrue($"Missing suite file at {path}.");

            var zpl = File.ReadAllText(path);
            var conversionService = new ConversionService();

            var sw = Stopwatch.StartNew();

            List<byte[]> pngImages = useExtractedDimensions
                ? conversionService.ConvertWithExtractedDimensions(zpl, Unit, Dpi)
                : conversionService.ConvertWithExplicitDimensions(zpl, widthMm, heightMm, Unit, Dpi);

            var pdfBytes = PdfGenerator.GeneratePdfToBytes(pngImages);

            sw.Stop();

            pngImages.Should().NotBeNull().And.NotBeEmpty($"file: {fileName}");
            AssertPngBytesAreValid(pngImages);

            pdfBytes.Should().NotBeNull();
            pdfBytes!.Length.Should().BeGreaterThan(100, $"file: {fileName}");
            Encoding.ASCII.GetString(pdfBytes, 0, 5).Should().Be("%PDF-");

            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(30));

            if (Environment.GetEnvironmentVariable("ZPL2PDF_EXPORT_REGRESSION_ARTIFACTS") == "1")
            {
                ExportArtifacts(fileName, zpl, pngImages, pdfBytes);
            }
        }

        [Fact]
        public void SuiteDirectory_ShouldContainAtLeastExpectedFiles()
        {
            Directory.Exists(SuiteDirectory).Should().BeTrue($"Expected {SuiteDirectory} after build.");
            var names = Directory.GetFiles(SuiteDirectory, "*.zpl").Select(Path.GetFileName).OrderBy(n => n).ToArray();
            names.Should().Contain("user_label.zpl");
            names.Should().Contain("utf8_fh.zpl");
            names.Should().Contain("aztec_b0.zpl");
        }

        private static void AssertPngBytesAreValid(IReadOnlyList<byte[]> pngImages)
        {
            foreach (var png in pngImages)
            {
                png.Should().NotBeNull();
                png!.Length.Should().BeGreaterThan(50);
                png.Take(8).Should().Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
            }
        }

        private static void ExportArtifacts(string fileName, string zpl, List<byte[]> pngImages, byte[] pdfBytes)
        {
            var processed = LabelFileReader.PreprocessZpl(zpl);
            var safeName = Path.GetFileNameWithoutExtension(fileName);

            var outDir = Path.Combine(
                Path.GetTempPath(),
                "ZPL2PDF_RegressionArtifacts",
                $"offline_{safeName}_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Directory.CreateDirectory(outDir);

            File.WriteAllText(Path.Combine(outDir, "input.zpl"), zpl, Encoding.UTF8);
            File.WriteAllText(Path.Combine(outDir, "preprocessed.zpl"), processed, Encoding.UTF8);
            File.WriteAllBytes(Path.Combine(outDir, "output.pdf"), pdfBytes);

            if (pngImages.Count > 0)
            {
                File.WriteAllBytes(Path.Combine(outDir, "output_0.png"), pngImages[0]);
            }
        }
    }
}
