using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Application.Services;
using ZPL2PDF.Tests.TestData;

namespace ZPL2PDF.Tests.UnitTests.Application
{
    /// <summary>
    /// Unit tests for ConversionService
    /// </summary>
    public class ConversionServiceTests
    {
        private readonly ConversionService _conversionService;

        public ConversionServiceTests()
        {
            _conversionService = new ConversionService();
        }

        #region ConvertWithExplicitDimensions Tests

        [Fact]
        public void ConvertWithExplicitDimensions_WithValidZpl_ReturnsImageData()
        {
            // Arrange
            var zplContent = SampleZplData.SimpleLabel;
            var width = 7.5;
            var height = 15.0;
            var unit = "in";
            var dpi = 203;

            // Act
            var result = _conversionService.ConvertWithExplicitDimensions(zplContent, width, height, unit, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1); // One label should produce one image
            result[0].Should().NotBeEmpty(); // Image data should not be empty
        }

        [Fact]
        public void ConvertWithExplicitDimensions_WithEmptyZpl_ThrowsArgumentException()
        {
            // Arrange
            var zplContent = "";
            var width = 7.5;
            var height = 15.0;
            var unit = "in";
            var dpi = 203;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, width, height, unit, dpi));
            
            exception.ParamName.Should().Be("zplContent");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public void ConvertWithExplicitDimensions_WithNullZpl_ThrowsArgumentException()
        {
            // Arrange
            string zplContent = null;
            var width = 7.5;
            var height = 15.0;
            var unit = "in";
            var dpi = 203;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, width, height, unit, dpi));
            
            exception.ParamName.Should().Be("zplContent");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public void ConvertWithExplicitDimensions_WithWhitespaceZpl_ThrowsArgumentException()
        {
            // Arrange
            var zplContent = "   ";
            var width = 7.5;
            var height = 15.0;
            var unit = "in";
            var dpi = 203;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _conversionService.ConvertWithExplicitDimensions(zplContent, width, height, unit, dpi));
            
            exception.ParamName.Should().Be("zplContent");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public void ConvertWithExplicitDimensions_WithMultipleLabels_ReturnsMultipleImages()
        {
            // Arrange
            var zplContent = SampleZplData.MultipleLabels;
            var width = 7.5;
            var height = 15.0;
            var unit = "in";
            var dpi = 203;

            // Act
            var result = _conversionService.ConvertWithExplicitDimensions(zplContent, width, height, unit, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2); // Two labels should produce two images
        }

        #endregion

        #region ConvertWithExtractedDimensions Tests

        [Fact]
        public void ConvertWithExtractedDimensions_WithValidZpl_ReturnsImageData()
        {
            // Arrange
            var zplContent = SampleZplData.LabelWithDimensions;
            var unit = "mm";
            var dpi = 203;

            // Act
            var result = _conversionService.ConvertWithExtractedDimensions(zplContent, unit, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1); // One label should produce one image
            result[0].Should().NotBeEmpty(); // Image data should not be empty
        }

        [Fact]
        public void ConvertWithExtractedDimensions_WithEmptyZpl_ThrowsArgumentException()
        {
            // Arrange
            var zplContent = "";
            var unit = "mm";
            var dpi = 203;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _conversionService.ConvertWithExtractedDimensions(zplContent, unit, dpi));
            
            exception.ParamName.Should().Be("zplContent");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public void ConvertWithExtractedDimensions_WithNullZpl_ThrowsArgumentException()
        {
            // Arrange
            string zplContent = null;
            var unit = "mm";
            var dpi = 203;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _conversionService.ConvertWithExtractedDimensions(zplContent, unit, dpi));
            
            exception.ParamName.Should().Be("zplContent");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        #endregion

        #region Convert (Mixed Approach) Tests

        [Fact]
        public void Convert_WithExplicitDimensions_UsesExplicitDimensions()
        {
            // Arrange
            var zplContent = SampleZplData.SimpleLabel;
            var explicitWidth = 7.5;
            var explicitHeight = 15.0;
            var unit = "in";
            var dpi = 203;

            // Act
            var result = _conversionService.Convert(zplContent, explicitWidth, explicitHeight, unit, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Convert_WithoutExplicitDimensions_UsesExtractedDimensions()
        {
            // Arrange
            var zplContent = SampleZplData.LabelWithDimensions;
            var explicitWidth = 0.0; // Zero means extract from ZPL
            var explicitHeight = 0.0; // Zero means extract from ZPL
            var unit = "mm";
            var dpi = 203;

            // Act
            var result = _conversionService.Convert(zplContent, explicitWidth, explicitHeight, unit, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Convert_WithZeroDimensions_UsesExtractedDimensions()
        {
            // Arrange
            var zplContent = SampleZplData.SimpleLabel;
            var explicitWidth = 0.0;
            var explicitHeight = 0.0;
            var unit = "mm";
            var dpi = 203;

            // Act
            var result = _conversionService.Convert(zplContent, explicitWidth, explicitHeight, unit, dpi);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
        }

        #endregion
    }
}
