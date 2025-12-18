using System;
using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Presentation
{
    /// <summary>
    /// Unit tests for ArgumentValidator
    /// </summary>
    public class ArgumentValidatorTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly ArgumentValidator _validator;

        public ArgumentValidatorTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_ArgumentValidatorTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _validator = new ArgumentValidator();
        }

        #region ValidateConversionMode Tests

        [Fact]
        public void ValidateConversionMode_WithValidInputFile_ReturnsValid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void ValidateConversionMode_WithValidZplContent_ReturnsValid()
        {
            // Arrange
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _validator.ValidateConversionMode("", zplContent, _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void ValidateConversionMode_WithBothInputFileAndZplContent_ReturnsInvalid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var result = _validator.ValidateConversionMode(testFile, zplContent, _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Cannot specify both input file (-i) and ZPL content (-z)");
        }

        [Fact]
        public void ValidateConversionMode_WithNeitherInputFileNorZplContent_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateConversionMode("", "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Either input file (-i) or ZPL content (-z) must be specified");
        }

        [Fact]
        public void ValidateConversionMode_WithNonExistentInputFile_ReturnsInvalid()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            var result = _validator.ValidateConversionMode(nonExistentFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Input file does not exist");
        }

        [Fact]
        public void ValidateConversionMode_WithInvalidFileExtension_ReturnsInvalid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.doc");
            File.WriteAllText(testFile, "This is not a ZPL file");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Input file must be .txt, .prn, .zpl, or .imp");
        }

        [Fact]
        public void ValidateConversionMode_WithTxtFile_ReturnsValid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void ValidateConversionMode_WithPrnFile_ReturnsValid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.prn");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void ValidateConversionMode_WithEmptyOutputFolder_ReturnsInvalid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", "", 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Output folder (-o) is required");
        }

        [Fact]
        public void ValidateConversionMode_WithNullOutputFolder_ReturnsInvalid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", null!, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Output folder (-o) is required");
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void ValidateConversionMode_WithWhitespaceInputFile_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateConversionMode("   ", "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Either input file (-i) or ZPL content (-z) must be specified");
        }

        [Fact]
        public void ValidateConversionMode_WithWhitespaceZplContent_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateConversionMode("", "   ", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Either input file (-i) or ZPL content (-z) must be specified");
        }

        [Fact]
        public void ValidateConversionMode_WithWhitespaceOutputFolder_ReturnsInvalid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", "   ", 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Output folder (-o) is required");
        }

        [Fact]
        public void ValidateConversionMode_WithNullInputFile_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateConversionMode(null!, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Either input file (-i) or ZPL content (-z) must be specified");
        }

        [Fact]
        public void ValidateConversionMode_WithNullZplContent_ReturnsInvalid()
        {
            // Act
            var result = _validator.ValidateConversionMode("", null!, _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Either input file (-i) or ZPL content (-z) must be specified");
        }

        #endregion

        #region File Extension Case Sensitivity Tests

        [Fact]
        public void ValidateConversionMode_WithUpperCaseTxtFile_ReturnsValid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.TXT");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void ValidateConversionMode_WithUpperCasePrnFile_ReturnsValid()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.PRN");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");

            // Act
            var result = _validator.ValidateConversionMode(testFile, "", _testDirectory, 0, 0, "mm");

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        #endregion

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
