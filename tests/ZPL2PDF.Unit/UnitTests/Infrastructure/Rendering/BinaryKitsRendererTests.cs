using FluentAssertions;
using Xunit;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF.Unit.UnitTests.Infrastructure.Rendering
{
    /// <summary>
    /// Tests for BinaryKitsRenderer
    /// </summary>
    public class BinaryKitsRendererTests
    {
        private readonly BinaryKitsRenderer _renderer;

        public BinaryKitsRendererTests()
        {
            _renderer = new BinaryKitsRenderer();
        }

        [Fact]
        public void Name_ReturnsBinaryKits()
        {
            // Assert
            _renderer.Name.Should().Be("BinaryKits");
        }

        [Fact]
        public void IsAvailable_AlwaysReturnsTrue()
        {
            // Act
            var result = _renderer.IsAvailable();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void RenderLabels_WithValidZpl_ReturnsImages()
        {
            // Arrange
            var labels = new List<string>
            {
                "^XA^FO50,50^A0N,30,30^FDTest^FS^XZ"
            };

            // Act
            var result = _renderer.RenderLabels(labels, 100, 150, 203);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Should().NotBeEmpty();
        }

        [Fact]
        public void RenderLabels_WithMultipleLabels_ReturnsMultipleImages()
        {
            // Arrange
            var labels = new List<string>
            {
                "^XA^FO50,50^FDLabel 1^FS^XZ",
                "^XA^FO50,50^FDLabel 2^FS^XZ"
            };

            // Act
            var result = _renderer.RenderLabels(labels, 100, 150, 203);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public void RenderLabels_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var labels = new List<string>();

            // Act
            var result = _renderer.RenderLabels(labels, 100, 150, 203);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task RenderLabelsAsync_WithValidZpl_ReturnsImages()
        {
            // Arrange
            var labels = new List<string>
            {
                "^XA^FO50,50^A0N,30,30^FDAsync Test^FS^XZ"
            };

            // Act
            var result = await _renderer.RenderLabelsAsync(labels, 100, 150, 203);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(203)]
        [InlineData(300)]
        [InlineData(600)]
        public void RenderLabels_WithDifferentDpi_ReturnsImages(int dpi)
        {
            // Arrange
            var labels = new List<string>
            {
                "^XA^FO50,50^FDTest^FS^XZ"
            };

            // Act
            var result = _renderer.RenderLabels(labels, 100, 150, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }
    }
}

