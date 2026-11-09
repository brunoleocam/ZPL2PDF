using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SkiaSharp;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for <see cref="PdfGenerator"/> (PDF merge behavior).
    /// </summary>
    public class PdfGeneratorTests
    {
        private static byte[] CreatePdfWithPageCount(int pageCount)
        {
            pageCount.Should().BeGreaterThan(0, "test PDF must contain at least one page");

            var document = new PdfDocument();
            for (var i = 0; i < pageCount; i++)
                document.AddPage();

            using var ms = new MemoryStream();
            document.Save(ms, false);
            return ms.ToArray();
        }

        [Fact]
        public void MergePdfsToBytes_WithTwoValidPdfDocuments_ReturnsMergedPdfWithTwoPages()
        {
            // Arrange
            var pdf1 = CreatePdfWithPageCount(1);
            var pdf2 = CreatePdfWithPageCount(1);

            // Act
            var merged = PdfGenerator.MergePdfsToBytes(new List<byte[]> { pdf1, pdf2 });

            // Assert
            using var mergedMs = new MemoryStream(merged);
            using var mergedDoc = PdfReader.Open(mergedMs, PdfDocumentOpenMode.Import);
            mergedDoc.PageCount.Should().Be(2);
        }

        [Fact]
        public void MergePdfsToBytes_WithNullAndEmptyEntries_IgnoresThemAndMergesRemainingPages()
        {
            // Arrange
            var valid = CreatePdfWithPageCount(2);
            var empty = Array.Empty<byte>();

            // Act
            var merged = PdfGenerator.MergePdfsToBytes(new List<byte[]> { valid, null!, empty });

            // Assert
            using var mergedMs = new MemoryStream(merged);
            using var mergedDoc = PdfReader.Open(mergedMs, PdfDocumentOpenMode.Import);
            mergedDoc.PageCount.Should().Be(2);
        }

        private static byte[] CreatePng(int width, int height)
        {
            using var bitmap = new SKBitmap(width, height);
            bitmap.Erase(SKColors.White);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        [Theory]
        [InlineData(813, 1219, 203, 288.0, 432.0)]  // 4x6 in @ 203 dpi (8 dpmm) -> 288x432 pt
        [InlineData(1200, 1800, 300, 288.0, 432.0)] // 4x6 in @ 300 dpi (12 dpmm) -> ~284x426 pt
        public void GeneratePdfToBytes_WithDpi_ConvertsPixelsToPhysicalPageSize(
            int pixelWidth, int pixelHeight, int dpi, double approxWidthPt, double approxHeightPt)
        {
            // Arrange
            var png = CreatePng(pixelWidth, pixelHeight);

            // Act
            var pdf = PdfGenerator.GeneratePdfToBytes(new List<byte[]> { png }, dpi);

            // Assert: page must NOT equal the pixel size (issue #82) and must be near the 4x6 in label size
            using var ms = new MemoryStream(pdf);
            using var doc = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
            var page = doc.Pages[0];
            page.Width.Point.Should().BeLessThan(pixelWidth);
            page.Width.Point.Should().BeApproximately(approxWidthPt, 5.0);
            page.Height.Point.Should().BeApproximately(approxHeightPt, 8.0);
        }

        [Fact]
        public void PixelsToPoints_At203Dpi_Returns288PointsFor4Inches()
        {
            // 4 in at 8 dpmm = 812.8 px -> 288 pt
            PdfGenerator.PixelsToPoints(813, 203).Should().BeApproximately(288.0, 0.5);
        }
    }
}

