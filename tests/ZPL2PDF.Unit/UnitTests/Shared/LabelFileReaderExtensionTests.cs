using FluentAssertions;
using Xunit;

namespace ZPL2PDF.Unit.UnitTests.Shared
{
    /// <summary>
    /// Tests for LabelFileReader new extension methods (v3.0 features)
    /// </summary>
    public class LabelFileReaderExtensionTests
    {
        #region ExtractFileName Tests

        [Fact]
        public void ExtractFileName_WithValidFileName_ReturnsFileName()
        {
            // Arrange
            var zplContent = @"^FX FileName: USPS-Shipping-Label
^XA
^FO50,50^A0N,30,30^FDTest Label^FS
^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().Be("USPS-Shipping-Label");
        }

        [Fact]
        public void ExtractFileName_WithFileNameBeforeXA_ReturnsFileName()
        {
            // Arrange
            var zplContent = @"^FX FileName: MyLabel
^XA^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().Be("MyLabel");
        }

        [Fact]
        public void ExtractFileName_WithFileNameAfterXA_ReturnsFileName()
        {
            // Arrange
            var zplContent = @"^XA
^FX FileName: InternalLabel
^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().Be("InternalLabel");
        }

        [Fact]
        public void ExtractFileName_WithSpacesInFileName_ReturnsFileName()
        {
            // Arrange
            var zplContent = @"^FX FileName: My Label Name
^XA^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().Be("My Label Name");
        }

        [Fact]
        public void ExtractFileName_CaseInsensitive_ReturnsFileName()
        {
            // Arrange
            var zplContent = @"^fx filename: CaseTest
^XA^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().Be("CaseTest");
        }

        [Fact]
        public void ExtractFileName_WithoutFileName_ReturnsNull()
        {
            // Arrange
            var zplContent = @"^XA^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractFileName_WithEmptyContent_ReturnsNull()
        {
            // Act
            var result = LabelFileReader.ExtractFileName(string.Empty);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractFileName_WithNullContent_ReturnsNull()
        {
            // Act
            var result = LabelFileReader.ExtractFileName(null!);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractFileName_WithInvalidCharacters_SanitizesFileName()
        {
            // Arrange
            var zplContent = @"^FX FileName: Test<>:Label
^XA^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotContain("<");
            result.Should().NotContain(">");
            result.Should().NotContain(":");
        }

        [Fact]
        public void ExtractFileName_WithNoFileNameComment_ReturnsNull()
        {
            // Arrange - No FileName comment at all
            var zplContent = @"^FX Some other comment^XA^FO50,50^FDTest^FS^XZ";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractFileName_FileNameEndingWithCaret_ReturnsFileName()
        {
            // Arrange
            var zplContent = @"^FX FileName: TestLabel^FO50,50^FDTest^FS";

            // Act
            var result = LabelFileReader.ExtractFileName(zplContent);

            // Assert
            result.Should().Be("TestLabel");
        }

        #endregion

        #region PreprocessZpl Tests (^FN removal)

        [Fact]
        public void PreprocessZpl_RemovesFNFollowedByFD()
        {
            // Arrange
            var zplContent = @"^FO90,12^A0N,20,20^FN6^FDHello World^FS";

            // Act
            var result = LabelFileReader.PreprocessZpl(zplContent);

            // Assert
            result.Should().Be(@"^FO90,12^A0N,20,20^FDHello World^FS");
        }

        [Fact]
        public void PreprocessZpl_RemovesMultipleFNTags()
        {
            // Arrange
            var zplContent = @"^FN1^FDFirst^FS^FN2^FDSecond^FS^FN3^FDThird^FS";

            // Act
            var result = LabelFileReader.PreprocessZpl(zplContent);

            // Assert
            result.Should().Be(@"^FDFirst^FS^FDSecond^FS^FDThird^FS");
        }

        [Fact]
        public void PreprocessZpl_PreservesFNNotFollowedByFD()
        {
            // Arrange
            var zplContent = @"^FN1^FS^FN2^FS";

            // Act
            var result = LabelFileReader.PreprocessZpl(zplContent);

            // Assert
            result.Should().Be(@"^FN1^FS^FN2^FS");
        }

        [Fact]
        public void PreprocessZpl_CaseInsensitive()
        {
            // Arrange
            var zplContent = @"^fn6^fdTest^FS";

            // Act
            var result = LabelFileReader.PreprocessZpl(zplContent);

            // Assert
            result.Should().Be(@"^fdTest^FS");
        }

        [Fact]
        public void PreprocessZpl_WithWhitespaceBetweenFNAndFD()
        {
            // Arrange
            var zplContent = @"^FN6  ^FDTest^FS";

            // Act
            var result = LabelFileReader.PreprocessZpl(zplContent);

            // Assert
            result.Should().Be(@"^FDTest^FS");
        }

        [Fact]
        public void PreprocessZpl_EmptyContent_ReturnsEmpty()
        {
            // Act
            var result = LabelFileReader.PreprocessZpl(string.Empty);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void PreprocessZpl_NullContent_ReturnsEmpty()
        {
            // Act
            var result = LabelFileReader.PreprocessZpl(null!);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion
    }
}

