using System;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Domain.ValueObjects;

namespace ZPL2PDF.Tests.UnitTests.Domain.ValueObjects
{
    /// <summary>
    /// Unit tests for ConversionOptions
    /// </summary>
    public class ConversionOptionsTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_Default_InitializesWithDefaultValues()
        {
            // Act
            var options = new ConversionOptions();

            // Assert
            options.Width.Should().Be(0);
            options.Height.Should().Be(0);
            options.Unit.Should().Be("mm");
            options.Dpi.Should().Be(203);
            options.OutputFileName.Should().Be("output.pdf");
            options.OutputFolderPath.Should().Be(string.Empty);
            options.UseExplicitDimensions.Should().BeFalse();
            options.ExtractDimensionsFromZpl.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithExplicitDimensions_SetsPropertiesCorrectly()
        {
            // Arrange
            var width = 7.5;
            var height = 15.0;
            var unit = "in";
            var dpi = 300;

            // Act
            var options = new ConversionOptions(width, height, unit, dpi);

            // Assert
            options.Width.Should().Be(width);
            options.Height.Should().Be(height);
            options.Unit.Should().Be(unit);
            options.Dpi.Should().Be(dpi);
            options.UseExplicitDimensions.Should().BeTrue();
            options.ExtractDimensionsFromZpl.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithZplExtraction_SetsPropertiesCorrectly()
        {
            // Arrange
            var unit = "mm";
            var dpi = 203;

            // Act
            var options = new ConversionOptions(unit, dpi);

            // Assert
            options.Unit.Should().Be(unit);
            options.Dpi.Should().Be(dpi);
            options.UseExplicitDimensions.Should().BeFalse();
            options.ExtractDimensionsFromZpl.Should().BeTrue();
        }

        #endregion

        #region IsValid Tests

        [Fact]
        public void IsValid_WithValidExplicitDimensions_ReturnsTrue()
        {
            // Arrange
            var options = new ConversionOptions(7.5, 15.0, "in", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithValidZplExtraction_ReturnsTrue()
        {
            // Arrange
            var options = new ConversionOptions("mm", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithEmptyUnit_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions("", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithNullUnit_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions(null, 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithZeroDpi_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions("mm", 0)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithNegativeDpi_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions("mm", -1)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithExplicitDimensionsAndZeroWidth_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions(0, 15.0, "in", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithExplicitDimensionsAndZeroHeight_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions(7.5, 0, "in", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithEmptyOutputFolder_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions("mm", 203)
            {
                OutputFolderPath = ""
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithNullOutputFolder_ReturnsFalse()
        {
            // Arrange
            var options = new ConversionOptions("mm", 203)
            {
                OutputFolderPath = null
            };

            // Act
            var result = options.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetValidationError Tests

        [Fact]
        public void GetValidationError_WithValidOptions_ReturnsEmptyString()
        {
            // Arrange
            var options = new ConversionOptions(7.5, 15.0, "in", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.GetValidationError();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetValidationError_WithEmptyUnit_ReturnsUnitError()
        {
            // Arrange
            var options = new ConversionOptions("", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.GetValidationError();

            // Assert
            result.Should().Be("Unit cannot be null or empty");
        }

        [Fact]
        public void GetValidationError_WithZeroDpi_ReturnsDpiError()
        {
            // Arrange
            var options = new ConversionOptions("mm", 0)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.GetValidationError();

            // Assert
            result.Should().Be("DPI must be greater than 0");
        }

        [Fact]
        public void GetValidationError_WithZeroWidth_ReturnsWidthError()
        {
            // Arrange
            var options = new ConversionOptions(0, 15.0, "in", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.GetValidationError();

            // Assert
            result.Should().Be("Width must be greater than 0");
        }

        [Fact]
        public void GetValidationError_WithZeroHeight_ReturnsHeightError()
        {
            // Arrange
            var options = new ConversionOptions(7.5, 0, "in", 203)
            {
                OutputFolderPath = "C:\\Output"
            };

            // Act
            var result = options.GetValidationError();

            // Assert
            result.Should().Be("Height must be greater than 0");
        }

        [Fact]
        public void GetValidationError_WithEmptyOutputFolder_ReturnsOutputFolderError()
        {
            // Arrange
            var options = new ConversionOptions("mm", 203)
            {
                OutputFolderPath = ""
            };

            // Act
            var result = options.GetValidationError();

            // Assert
            result.Should().Be("Output folder path cannot be null or empty");
        }

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_WithValidOptions_ReturnsExactCopy()
        {
            // Arrange
            var original = new ConversionOptions(7.5, 15.0, "in", 203)
            {
                OutputFileName = "test.pdf",
                OutputFolderPath = "C:\\Output",
                UseExplicitDimensions = true,
                ExtractDimensionsFromZpl = false
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.Width.Should().Be(original.Width);
            clone.Height.Should().Be(original.Height);
            clone.Unit.Should().Be(original.Unit);
            clone.Dpi.Should().Be(original.Dpi);
            clone.OutputFileName.Should().Be(original.OutputFileName);
            clone.OutputFolderPath.Should().Be(original.OutputFolderPath);
            clone.UseExplicitDimensions.Should().Be(original.UseExplicitDimensions);
            clone.ExtractDimensionsFromZpl.Should().Be(original.ExtractDimensionsFromZpl);
        }

        [Fact]
        public void Clone_WithZplExtractionOptions_ReturnsExactCopy()
        {
            // Arrange
            var original = new ConversionOptions("mm", 203)
            {
                OutputFileName = "zpl.pdf",
                OutputFolderPath = "C:\\ZPL",
                UseExplicitDimensions = false,
                ExtractDimensionsFromZpl = true
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.Unit.Should().Be(original.Unit);
            clone.Dpi.Should().Be(original.Dpi);
            clone.OutputFileName.Should().Be(original.OutputFileName);
            clone.OutputFolderPath.Should().Be(original.OutputFolderPath);
            clone.UseExplicitDimensions.Should().Be(original.UseExplicitDimensions);
            clone.ExtractDimensionsFromZpl.Should().Be(original.ExtractDimensionsFromZpl);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_WithExplicitDimensions_ReturnsFormattedString()
        {
            // Arrange
            var options = new ConversionOptions(7.5, 15.0, "in", 203);

            // Act
            var result = options.ToString();

            // Assert
            result.Should().Contain("ConversionOptions");
            result.Should().MatchRegex(@"7[.,]5x15"); // Aceita tanto ponto quanto vírgula
            result.Should().Contain("in");
            result.Should().Contain("dpi");
        }

        [Fact]
        public void ToString_WithZplExtraction_ReturnsFormattedString()
        {
            // Arrange
            var options = new ConversionOptions("mm", 203);

            // Act
            var result = options.ToString();

            // Assert
            result.Should().Contain("ConversionOptions");
            result.Should().Contain("Extract from ZPL");
            result.Should().Contain("mm");
            result.Should().Contain("dpi");
        }

        #endregion

        #region Property Setting Tests

        [Fact]
        public void SetProperties_WithValidValues_UpdatesCorrectly()
        {
            // Arrange
            var options = new ConversionOptions();

            // Act
            options.Width = 10.5;
            options.Height = 20.0;
            options.Unit = "cm";
            options.Dpi = 300;
            options.OutputFileName = "custom.pdf";
            options.OutputFolderPath = "C:\\Custom";
            options.UseExplicitDimensions = true;
            options.ExtractDimensionsFromZpl = false;

            // Assert
            options.Width.Should().Be(10.5);
            options.Height.Should().Be(20.0);
            options.Unit.Should().Be("cm");
            options.Dpi.Should().Be(300);
            options.OutputFileName.Should().Be("custom.pdf");
            options.OutputFolderPath.Should().Be("C:\\Custom");
            options.UseExplicitDimensions.Should().BeTrue();
            options.ExtractDimensionsFromZpl.Should().BeFalse();
        }

        #endregion
    }
}
