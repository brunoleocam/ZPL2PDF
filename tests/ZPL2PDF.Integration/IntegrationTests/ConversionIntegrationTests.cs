using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Application.Services;
using ZPL2PDF.Tests.TestData;

namespace ZPL2PDF.Integration.IntegrationTests
{
    /// <summary>
    /// Integration tests for ZPL to PDF conversion
    /// </summary>
    public class ConversionIntegrationTests : IDisposable
    {
        private readonly ConversionService _conversionService;
        private readonly string _testDirectory;

        public ConversionIntegrationTests()
        {
            _conversionService = new ConversionService();
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_IntegrationTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        #region End-to-End Conversion Tests

        [Fact]
        public async Task ConvertZplToPdf_EndToEnd_GeneratesValidPdf()
        {
            // Arrange
            var zplContent = SampleZplData.SimpleLabel;
            var outputPath = Path.Combine(_testDirectory, "test_output.pdf");

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, 7.5, 15, "in", 203));

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result[0].Should().NotBeEmpty(); // PDF should have content
        }

        [Fact]
        public async Task ConvertZplToPdf_WithMultipleLabels_GeneratesMultiplePages()
        {
            // Arrange
            var zplContent = SampleZplData.MultipleLabels;
            var outputPath = Path.Combine(_testDirectory, "multiple_labels_output.pdf");

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, 7.5, 15, "in", 203));

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Count.Should().BeGreaterThan(1); // Should have multiple pages
        }

        [Fact]
        public async Task ConvertZplToPdf_WithExtractedDimensions_WorksCorrectly()
        {
            // Arrange
            var zplContent = SampleZplData.LabelWithDimensions;

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExtractedDimensions(zplContent, "mm", 203));

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result[0].Should().NotBeEmpty(); // PDF should have content
        }

        [Fact]
        public async Task ConvertZplToPdf_WithComplexLabel_GeneratesValidPdf()
        {
            // Arrange
            var zplContent = SampleZplData.ComplexLabel;

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, 10, 5, "cm", 300));

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result[0].Should().NotBeEmpty(); // PDF should have content
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task ConvertZplToPdf_WithInvalidZpl_HandlesGracefully()
        {
            // Arrange
            var invalidZpl = "INVALID_ZPL_CONTENT";

            // Act & Assert
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(invalidZpl, 7.5, 15, "in", 203));
            
            // Should not throw exception, but may return empty or error result
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ConvertZplToPdf_WithEmptyZpl_ReturnsEmptyList()
        {
            // Arrange
            var emptyZpl = string.Empty;

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(emptyZpl, 7.5, 15, "in", 203));

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task ConvertZplToPdf_Performance_CompletesWithinReasonableTime()
        {
            // Arrange
            var zplContent = SampleZplData.ComplexLabel;
            var startTime = DateTime.Now;

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, 7.5, 15, "in", 203));

            // Assert
            var duration = DateTime.Now - startTime;
            duration.Should().BeLessThan(TimeSpan.FromSeconds(10)); // Should complete within 10 seconds
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ConvertZplToPdf_WithLargeZpl_HandlesCorrectly()
        {
            // Arrange
            var largeZpl = GenerateLargeZplContent();

            // Act
            var result = await Task.Run(() => 
                _conversionService.ConvertWithExplicitDimensions(largeZpl, 7.5, 15, "in", 203));

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        #endregion

        #region Helper Methods

        private string GenerateLargeZplContent()
        {
            var zpl = "^XA";
            for (int i = 0; i < 100; i++)
            {
                zpl += $"^FO50,{50 + i * 20}^A0N,30,30^FDLine {i + 1}^FS";
            }
            zpl += "^XZ";
            return zpl;
        }

        #endregion

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
