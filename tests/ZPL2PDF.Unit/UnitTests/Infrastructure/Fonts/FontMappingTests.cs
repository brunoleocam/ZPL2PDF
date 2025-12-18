using FluentAssertions;
using Xunit;
using ZPL2PDF.Infrastructure.Fonts;

namespace ZPL2PDF.Unit.UnitTests.Infrastructure.Fonts
{
    /// <summary>
    /// Tests for FontMapping class
    /// </summary>
    public class FontMappingTests
    {
        #region FromString Tests

        [Fact]
        public void FromString_ValidMapping_ReturnsFontMapping()
        {
            // Arrange
            var mappingString = "A=C:\\Fonts\\Arial.ttf";

            // Act
            var result = FontMapping.FromString(mappingString);

            // Assert
            result.Should().NotBeNull();
            result!.ZplFontId.Should().Be("A");
            result.FontFile.Should().Be("C:\\Fonts\\Arial.ttf");
            result.Name.Should().Be("Arial");
        }

        [Fact]
        public void FromString_LowercaseFontId_ConvertsToUppercase()
        {
            // Arrange
            var mappingString = "a=C:\\Fonts\\Arial.ttf";

            // Act
            var result = FontMapping.FromString(mappingString);

            // Assert
            result.Should().NotBeNull();
            result!.ZplFontId.Should().Be("A");
        }

        [Fact]
        public void FromString_WithSpaces_TrimsValues()
        {
            // Arrange
            var mappingString = " P = C:\\Fonts\\OCRA.ttf ";

            // Act
            var result = FontMapping.FromString(mappingString);

            // Assert
            result.Should().NotBeNull();
            result!.ZplFontId.Should().Be("P");
            result.FontFile.Should().Be("C:\\Fonts\\OCRA.ttf");
        }

        [Fact]
        public void FromString_NumericFontId_Works()
        {
            // Arrange
            var mappingString = "0=C:\\Fonts\\DejaVuSansMono.ttf";

            // Act
            var result = FontMapping.FromString(mappingString);

            // Assert
            result.Should().NotBeNull();
            result!.ZplFontId.Should().Be("0");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void FromString_NullOrEmpty_ReturnsNull(string? input)
        {
            // Act
            var result = FontMapping.FromString(input!);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("InvalidNoEquals")]
        [InlineData("OnlyFontId")]
        public void FromString_InvalidFormat_ReturnsNull(string input)
        {
            // Act
            var result = FontMapping.FromString(input);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FromString_PathWithEquals_HandlesProperly()
        {
            // Arrange - Path contains = sign
            var mappingString = "A=C:\\Path=Test\\Font.ttf";

            // Act
            var result = FontMapping.FromString(mappingString);

            // Assert
            result.Should().NotBeNull();
            result!.ZplFontId.Should().Be("A");
            result.FontFile.Should().Be("C:\\Path=Test\\Font.ttf");
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void FontMapping_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var mapping = new FontMapping();

            // Assert
            mapping.ZplFontId.Should().BeEmpty();
            mapping.Name.Should().BeEmpty();
            mapping.FontFile.Should().BeEmpty();
            mapping.Style.Should().Be("Regular");
            mapping.WidthFactor.Should().Be(1.0);
            mapping.HeightFactor.Should().Be(1.0);
            mapping.IsBitmapStyle.Should().BeFalse();
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            var mapping = new FontMapping
            {
                ZplFontId = "A",
                Name = "Arial",
                FontFile = "C:\\Fonts\\Arial.ttf"
            };

            // Act
            var result = mapping.ToString();

            // Assert
            result.Should().Contain("A");
            result.Should().Contain("Arial");
            result.Should().Contain("Arial.ttf");
        }

        #endregion
    }
}

