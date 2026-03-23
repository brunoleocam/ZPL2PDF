using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
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
    }
}

