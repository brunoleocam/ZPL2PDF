using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using ZPL2PDF;
using LabelDimensions = ZPL2PDF.LabelDimensions;

namespace ZPL2PDF.Tests.UnitTests.Domain.Services
{
    /// <summary>
    /// Unit tests for ZplDimensionExtractor
    /// </summary>
    public class ZplDimensionExtractorTests
    {
        private readonly ZplDimensionExtractor _extractor;

        public ZplDimensionExtractorTests()
        {
            _extractor = new ZplDimensionExtractor();
        }

        #region ExtractDimensions Tests

        [Fact]
        public void ExtractDimensions_WithValidZpl_ReturnsDimensions()
        {
            // Arrange
            var zplContent = "^XA^PW400^LL200^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Width.Should().Be(400);
            result[0].Height.Should().Be(200);
            result[0].HasDimensions.Should().BeTrue();
        }

        [Fact]
        public void ExtractDimensions_WithMultipleLabels_ReturnsMultipleDimensions()
        {
            // Arrange
            var zplContent = "^XA^PW400^LL200^FO50,50^A0N,50,50^FDLabel 1^FS^XZ^XA^PW300^LL150^FO50,50^A0N,50,50^FDLabel 2^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Width.Should().Be(400);
            result[0].Height.Should().Be(200);
            result[1].Width.Should().Be(300);
            result[1].Height.Should().Be(150);
        }

        [Fact]
        public void ExtractDimensions_WithEmptyZpl_ReturnsEmptyList()
        {
            // Arrange
            var zplContent = "";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ExtractDimensions_WithNullZpl_ReturnsEmptyList()
        {
            // Arrange
            string? zplContent = null;

            // Act
            var result = _extractor.ExtractDimensions(zplContent!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ExtractDimensions_WithWhitespaceZpl_ReturnsEmptyList()
        {
            // Arrange
            var zplContent = "   \t\n   ";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ExtractDimensions_WithNoDimensions_ReturnsDefaultDimensions()
        {
            // Arrange
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Width.Should().Be(0);
            result[0].Height.Should().Be(0);
            result[0].HasDimensions.Should().BeFalse();
        }

        [Fact]
        public void ExtractDimensions_WithOnlyWidth_ReturnsPartialDimensions()
        {
            // Arrange
            var zplContent = "^XA^PW400^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Width.Should().Be(400);
            result[0].Height.Should().Be(0);
            result[0].HasDimensions.Should().BeTrue();
        }

        [Fact]
        public void ExtractDimensions_WithOnlyHeight_ReturnsPartialDimensions()
        {
            // Arrange
            var zplContent = "^XA^LL200^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensions(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Width.Should().Be(0);
            result[0].Height.Should().Be(200);
            result[0].HasDimensions.Should().BeTrue();
        }

        #endregion

        #region ExtractDimensionsFromLabel Tests

        [Fact]
        public void ExtractDimensionsFromLabel_WithValidLabel_ReturnsDimensions()
        {
            // Arrange
            var labelContent = "^XA^PW400^LL200^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensionsFromLabel(labelContent);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(400);
            result.Height.Should().Be(200);
            result.HasDimensions.Should().BeTrue();
        }

        [Fact]
        public void ExtractDimensionsFromLabel_WithEmptyLabel_ReturnsDefaultDimensions()
        {
            // Arrange
            var labelContent = "";

            // Act
            var result = _extractor.ExtractDimensionsFromLabel(labelContent);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(0);
            result.Height.Should().Be(0);
            result.HasDimensions.Should().BeFalse();
        }

        [Fact]
        public void ExtractDimensionsFromLabel_WithNullLabel_ReturnsDefaultDimensions()
        {
            // Arrange
            string? labelContent = null;

            // Act
            var result = _extractor.ExtractDimensionsFromLabel(labelContent!);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(0);
            result.Height.Should().Be(0);
            result.HasDimensions.Should().BeFalse();
        }

        [Fact]
        public void ExtractDimensionsFromLabel_WithCaseInsensitiveCommands_ReturnsDimensions()
        {
            // Arrange
            var labelContent = "^XA^pw400^ll200^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _extractor.ExtractDimensionsFromLabel(labelContent);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(400);
            result.Height.Should().Be(200);
            result.HasDimensions.Should().BeTrue();
        }

        #endregion

        #region ConvertPointsToMm Tests

        [Fact]
        public void ConvertPointsToMm_WithValidPoints_ReturnsCorrectMm()
        {
            // Arrange
            var points = 203;
            var dpi = 203;

            // Act
            var result = _extractor.ConvertPointsToMm(points, dpi);

            // Assert
            result.Should().BeApproximately(25.4, 0.1);
        }

        [Fact]
        public void ConvertPointsToMm_WithZeroPoints_ReturnsZero()
        {
            // Arrange
            var points = 0;
            var dpi = 203;

            // Act
            var result = _extractor.ConvertPointsToMm(points, dpi);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void ConvertPointsToMm_WithCustomDpi_ReturnsCorrectMm()
        {
            // Arrange
            var points = 300;
            var dpi = 300;

            // Act
            var result = _extractor.ConvertPointsToMm(points, dpi);

            // Assert
            result.Should().BeApproximately(25.4, 0.1);
        }

        [Fact]
        public void ConvertPointsToMm_WithDefaultDpi_ReturnsCorrectMm()
        {
            // Arrange
            var points = 203;

            // Act
            var result = _extractor.ConvertPointsToMm(points);

            // Assert
            result.Should().BeApproximately(25.4, 0.1);
        }

        #endregion

        #region ConvertMmToPoints Tests

        [Fact]
        public void ConvertMmToPoints_WithValidMm_ReturnsCorrectPoints()
        {
            // Arrange
            var mm = 25.4;
            var dpi = 203;

            // Act
            var result = _extractor.ConvertMmToPoints(mm, dpi);

            // Assert
            result.Should().Be(203);
        }

        [Fact]
        public void ConvertMmToPoints_WithZeroMm_ReturnsZero()
        {
            // Arrange
            var mm = 0.0;
            var dpi = 203;

            // Act
            var result = _extractor.ConvertMmToPoints(mm, dpi);

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void ConvertMmToPoints_WithCustomDpi_ReturnsCorrectPoints()
        {
            // Arrange
            var mm = 25.4;
            var dpi = 300;

            // Act
            var result = _extractor.ConvertMmToPoints(mm, dpi);

            // Assert
            result.Should().Be(300);
        }

        [Fact]
        public void ConvertMmToPoints_WithDefaultDpi_ReturnsCorrectPoints()
        {
            // Arrange
            var mm = 25.4;

            // Act
            var result = _extractor.ConvertMmToPoints(mm);

            // Assert
            result.Should().Be(203);
        }

        #endregion

        #region ValidateDimensions Tests

        [Fact]
        public void ValidateDimensions_WithValidDimensions_ReturnsTrue()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 400,
                Height = 200,
                WidthMm = 50.0,
                HeightMm = 25.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateDimensions_WithNullDimensions_ReturnsFalse()
        {
            // Arrange
            LabelDimensions? dimensions = null;

            // Act
            var result = _extractor.ValidateDimensions(dimensions!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithZeroWidth_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 0,
                Height = 200,
                WidthMm = 0.0,
                HeightMm = 25.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithZeroHeight_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 400,
                Height = 0,
                WidthMm = 50.0,
                HeightMm = 0.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithNegativeWidth_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = -100,
                Height = 200,
                WidthMm = -10.0,
                HeightMm = 25.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithTooSmallWidth_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 10,
                Height = 200,
                WidthMm = 0.5,
                HeightMm = 25.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithTooLargeWidth_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 10000,
                Height = 200,
                WidthMm = 1001.0,
                HeightMm = 25.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithTooSmallHeight_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 400,
                Height = 10,
                WidthMm = 50.0,
                HeightMm = 0.5
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateDimensions_WithTooLargeHeight_ReturnsFalse()
        {
            // Arrange
            var dimensions = new LabelDimensions
            {
                Width = 400,
                Height = 10000,
                WidthMm = 50.0,
                HeightMm = 1001.0
            };

            // Act
            var result = _extractor.ValidateDimensions(dimensions);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetDefaultDimensions Tests

        [Fact]
        public void GetDefaultDimensions_ReturnsValidDimensions()
        {
            // Act
            var result = _extractor.GetDefaultDimensions();

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().BeGreaterThan(0);
            result.Height.Should().BeGreaterThan(0);
            result.WidthMm.Should().BeGreaterThan(0);
            result.HeightMm.Should().BeGreaterThan(0);
            result.Dpi.Should().BeGreaterThan(0);
        }

        #endregion

        #region ApplyPriorityLogic Tests

        [Fact]
        public void ApplyPriorityLogic_WithExplicitParameters_UsesExplicitParameters()
        {
            // Arrange
            var explicitWidth = 50.0;
            var explicitHeight = 25.0;
            var explicitUnit = "mm";
            var zplDimensions = new LabelDimensions
            {
                Width = 0,
                Height = 0,
                WidthMm = 0.0,
                HeightMm = 0.0,
                HasDimensions = false  // ZPL dimensions are invalid, so explicit parameters should be used
            };

            // Act
            var result = _extractor.ApplyPriorityLogic(explicitWidth, explicitHeight, explicitUnit, zplDimensions);

            // Assert
            result.Should().NotBeNull();
            result.WidthMm.Should().Be(explicitWidth);
            result.HeightMm.Should().Be(explicitHeight);
            result.Source.Should().Be("explicit_parameters");
        }

        [Fact]
        public void ApplyPriorityLogic_WithoutExplicitParameters_UsesZplDimensions()
        {
            // Arrange
            double? explicitWidth = null;
            double? explicitHeight = null;
            var explicitUnit = "mm";
            var zplDimensions = new LabelDimensions
            {
                Width = 400,
                Height = 200,
                WidthMm = 50.0,
                HeightMm = 25.0,
                HasDimensions = true
            };

            // Act
            var result = _extractor.ApplyPriorityLogic(explicitWidth, explicitHeight, explicitUnit, zplDimensions);

            // Assert
            result.Should().NotBeNull();
            result.Width.Should().Be(zplDimensions.Width);
            result.Height.Should().Be(zplDimensions.Height);
            result.WidthMm.Should().Be(zplDimensions.WidthMm);
            result.HeightMm.Should().Be(zplDimensions.HeightMm);
            result.Source.Should().Be("zpl_extraction");
        }

        [Fact]
        public void ApplyPriorityLogic_WithInvalidZplDimensions_UsesDefaultDimensions()
        {
            // Arrange
            double? explicitWidth = null;
            double? explicitHeight = null;
            var explicitUnit = "mm";
            var zplDimensions = new LabelDimensions
            {
                Width = 0,
                Height = 0,
                WidthMm = 0.0,
                HeightMm = 0.0,
                HasDimensions = false
            };

            // Act
            var result = _extractor.ApplyPriorityLogic(explicitWidth, explicitHeight, explicitUnit, zplDimensions);

            // Assert
            result.Should().NotBeNull();
            result.Source.Should().Be("default");
        }

        [Fact]
        public void ApplyPriorityLogic_WithPartialExplicitParameters_UsesZplDimensions()
        {
            // Arrange
            var explicitWidth = 50.0;
            double? explicitHeight = null;
            var explicitUnit = "mm";
            var zplDimensions = new LabelDimensions
            {
                Width = 400,
                Height = 200,
                WidthMm = 50.0,
                HeightMm = 25.0,
                HasDimensions = true
            };

            // Act
            var result = _extractor.ApplyPriorityLogic(explicitWidth, explicitHeight, explicitUnit, zplDimensions);

            // Assert
            result.Should().NotBeNull();
            result.Source.Should().Be("zpl_extraction");
        }

        #endregion
    }
}