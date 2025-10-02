using System;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF.Tests.UnitTests.Application
{
    /// <summary>
    /// Unit tests for UnitConversionService
    /// </summary>
    public class UnitConversionServiceTests
    {
        private readonly UnitConversionService _unitConversionService;

        public UnitConversionServiceTests()
        {
            _unitConversionService = new UnitConversionService();
        }

        #region ConvertUnit Tests

        [Theory]
        [InlineData(100.0, "mm", "cm", 10.0)]
        [InlineData(10.0, "cm", "mm", 100.0)]
        [InlineData(25.4, "mm", "in", 1.0)]
        [InlineData(1.0, "in", "mm", 25.4)]
        [InlineData(100.0, "mm", "mm", 100.0)]
        [InlineData(0.0, "mm", "cm", 0.0)]
        public void ConvertUnit_WithValidUnits_ReturnsCorrectValue(double value, string fromUnit, string toUnit, double expected)
        {
            // Act
            var result = _unitConversionService.ConvertUnit(value, fromUnit, toUnit);

            // Assert
            result.Should().BeApproximately(expected, 0.001);
        }

        [Fact]
        public void ConvertUnit_SameUnit_ReturnsSameValue()
        {
            // Arrange
            var value = 100.0;
            var unit = "mm";

            // Act
            var result = _unitConversionService.ConvertUnit(value, unit, unit);

            // Assert
            result.Should().Be(value);
        }

        [Theory]
        [InlineData("mm", "invalid")]
        [InlineData("invalid", "mm")]
        [InlineData("invalid", "invalid")]
        public void ConvertUnit_WithInvalidUnit_ReturnsOriginalValue(string fromUnit, string toUnit)
        {
            // Arrange
            var value = 100.0;

            // Act
            var result = _unitConversionService.ConvertUnit(value, fromUnit, toUnit);

            // Assert
            result.Should().Be(value);
        }

        [Theory]
        [InlineData("MM", "CM")] // Test case insensitive
        [InlineData("Mm", "Cm")]
        [InlineData("mm", "CM")]
        public void ConvertUnit_WithDifferentCase_ReturnsCorrectValue(string fromUnit, string toUnit)
        {
            // Arrange
            var value = 100.0;
            var expected = 10.0; // 100mm = 10cm

            // Act
            var result = _unitConversionService.ConvertUnit(value, fromUnit, toUnit);

            // Assert
            result.Should().BeApproximately(expected, 0.001);
        }

        #endregion

        #region ConvertMmToPoints Tests

        [Theory]
        [InlineData(25.4, 203, 203)] // 25.4mm = 1 inch = 203 points at 203 DPI
        [InlineData(12.7, 203, 102)] // 12.7mm = 0.5 inch = 101.5 points at 203 DPI (rounded to 102)
        [InlineData(0.0, 203, 0)] // 0mm = 0 points
        [InlineData(25.4, 300, 300)] // 25.4mm = 1 inch = 300 points at 300 DPI
        public void ConvertMmToPoints_WithValidMm_ReturnsCorrectPoints(double mm, int dpi, int expected)
        {
            // Act
            var result = _unitConversionService.ConvertMmToPoints(mm, dpi);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ConvertMmToPoints_WithDefaultDpi_ReturnsCorrectPoints()
        {
            // Arrange
            var mm = 25.4; // 1 inch
            var expected = 203; // 203 points at 203 DPI

            // Act
            var result = _unitConversionService.ConvertMmToPoints(mm);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ConvertMmToPoints_WithNegativeMm_ReturnsNegativePoints()
        {
            // Arrange
            var mm = -25.4;
            var dpi = 203;
            var expected = -203;

            // Act
            var result = _unitConversionService.ConvertMmToPoints(mm, dpi);

            // Assert
            result.Should().Be(expected);
        }

        #endregion

        #region ConvertPointsToMm Tests

        [Theory]
        [InlineData(203, 203, 25.4)] // 203 points = 1 inch = 25.4mm at 203 DPI
        [InlineData(101, 203, 12.637)] // 101 points = 0.5 inch = 12.637mm at 203 DPI (calculated: 101/203*25.4)
        [InlineData(0, 203, 0.0)] // 0 points = 0mm
        [InlineData(300, 300, 25.4)] // 300 points = 1 inch = 25.4mm at 300 DPI
        public void ConvertPointsToMm_WithValidPoints_ReturnsCorrectMm(int points, int dpi, double expected)
        {
            // Act
            var result = _unitConversionService.ConvertPointsToMm(points, dpi);

            // Assert
            result.Should().BeApproximately(expected, 0.001);
        }

        [Fact]
        public void ConvertPointsToMm_WithDefaultDpi_ReturnsCorrectMm()
        {
            // Arrange
            var points = 203;
            var expected = 25.4; // 203 points = 1 inch = 25.4mm at 203 DPI

            // Act
            var result = _unitConversionService.ConvertPointsToMm(points);

            // Assert
            result.Should().BeApproximately(expected, 0.001);
        }

        [Fact]
        public void ConvertPointsToMm_WithNegativePoints_ReturnsNegativeMm()
        {
            // Arrange
            var points = -203;
            var dpi = 203;
            var expected = -25.4;

            // Act
            var result = _unitConversionService.ConvertPointsToMm(points, dpi);

            // Assert
            result.Should().BeApproximately(expected, 0.001);
        }

        #endregion

        #region ConvertToMillimeters Tests

        [Theory]
        [InlineData(100.0, 50.0, "mm", 100.0, 50.0)]
        [InlineData(10.0, 5.0, "cm", 100.0, 50.0)]
        [InlineData(1.0, 0.5, "in", 25.4, 12.7)]
        public void ConvertToMillimeters_WithValidUnits_ReturnsCorrectValues(double width, double height, string unit, double expectedWidth, double expectedHeight)
        {
            // Act
            var result = _unitConversionService.ConvertToMillimeters(width, height, unit);

            // Assert
            result.widthMm.Should().BeApproximately(expectedWidth, 0.001);
            result.heightMm.Should().BeApproximately(expectedHeight, 0.001);
        }

        [Fact]
        public void ConvertToMillimeters_WithZeroValues_ReturnsZeroValues()
        {
            // Arrange
            var width = 0.0;
            var height = 0.0;
            var unit = "mm";

            // Act
            var result = _unitConversionService.ConvertToMillimeters(width, height, unit);

            // Assert
            result.widthMm.Should().Be(0.0);
            result.heightMm.Should().Be(0.0);
        }

        [Fact]
        public void ConvertToMillimeters_WithNegativeValues_ReturnsNegativeValues()
        {
            // Arrange
            var width = -10.0;
            var height = -5.0;
            var unit = "cm";

            // Act
            var result = _unitConversionService.ConvertToMillimeters(width, height, unit);

            // Assert
            result.widthMm.Should().BeApproximately(-100.0, 0.001);
            result.heightMm.Should().BeApproximately(-50.0, 0.001);
        }

        [Theory]
        [InlineData("MM")] // Test case insensitive
        [InlineData("Mm")]
        [InlineData("mm")]
        public void ConvertToMillimeters_WithDifferentCase_ReturnsCorrectValues(string unit)
        {
            // Arrange
            var width = 100.0;
            var height = 50.0;

            // Act
            var result = _unitConversionService.ConvertToMillimeters(width, height, unit);

            // Assert
            result.widthMm.Should().Be(100.0);
            result.heightMm.Should().Be(50.0);
        }

        #endregion
    }
}
