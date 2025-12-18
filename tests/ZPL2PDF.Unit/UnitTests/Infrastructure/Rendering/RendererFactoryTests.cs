using FluentAssertions;
using Xunit;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF.Unit.UnitTests.Infrastructure.Rendering
{
    /// <summary>
    /// Tests for RendererFactory
    /// </summary>
    public class RendererFactoryTests
    {
        #region ParseMode Tests

        [Theory]
        [InlineData("offline", RendererMode.Offline)]
        [InlineData("OFFLINE", RendererMode.Offline)]
        [InlineData("Offline", RendererMode.Offline)]
        [InlineData("binarykits", RendererMode.Offline)]
        public void ParseMode_OfflineVariants_ReturnsOffline(string input, RendererMode expected)
        {
            // Act
            var result = RendererFactory.ParseMode(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("labelary", RendererMode.Labelary)]
        [InlineData("LABELARY", RendererMode.Labelary)]
        [InlineData("Labelary", RendererMode.Labelary)]
        public void ParseMode_LabelaryVariants_ReturnsLabelary(string input, RendererMode expected)
        {
            // Act
            var result = RendererFactory.ParseMode(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("auto", RendererMode.Auto)]
        [InlineData("AUTO", RendererMode.Auto)]
        [InlineData("Auto", RendererMode.Auto)]
        public void ParseMode_AutoVariants_ReturnsAuto(string input, RendererMode expected)
        {
            // Act
            var result = RendererFactory.ParseMode(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid")]
        [InlineData("unknown")]
        public void ParseMode_InvalidOrEmpty_ReturnsOffline(string? input)
        {
            // Act
            var result = RendererFactory.ParseMode(input);

            // Assert
            result.Should().Be(RendererMode.Offline);
        }

        #endregion

        #region Create Tests

        [Fact]
        public void Create_OfflineMode_ReturnsBinaryKitsRenderer()
        {
            // Act
            var renderer = RendererFactory.Create(RendererMode.Offline);

            // Assert
            renderer.Should().NotBeNull();
            renderer.Name.Should().Be("BinaryKits");
            renderer.IsAvailable().Should().BeTrue();
        }

        [Fact]
        public void Create_LabelaryMode_ReturnsLabelaryRenderer()
        {
            // Act
            var renderer = RendererFactory.Create(RendererMode.Labelary);

            // Assert
            renderer.Should().NotBeNull();
            renderer.Name.Should().Be("Labelary");
        }

        [Fact]
        public void Create_AutoMode_ReturnsRenderer()
        {
            // Act
            var renderer = RendererFactory.Create(RendererMode.Auto);

            // Assert
            renderer.Should().NotBeNull();
            // Could be either BinaryKits or Labelary depending on network
        }

        #endregion

        #region CreateWithFallback Tests

        [Fact]
        public void CreateWithFallback_AutoMode_ReturnsFallbackRenderer()
        {
            // Act
            var renderer = RendererFactory.CreateWithFallback(RendererMode.Auto);

            // Assert
            renderer.Should().NotBeNull();
            renderer.Name.Should().Contain("fallback");
        }

        [Fact]
        public void CreateWithFallback_LabelaryMode_ReturnsFallbackRenderer()
        {
            // Act
            var renderer = RendererFactory.CreateWithFallback(RendererMode.Labelary);

            // Assert
            renderer.Should().NotBeNull();
            renderer.Name.Should().Contain("fallback");
        }

        [Fact]
        public void CreateWithFallback_OfflineMode_ReturnsBinaryKitsRenderer()
        {
            // Act
            var renderer = RendererFactory.CreateWithFallback(RendererMode.Offline);

            // Assert
            renderer.Should().NotBeNull();
            renderer.Name.Should().Be("BinaryKits");
        }

        #endregion
    }
}

