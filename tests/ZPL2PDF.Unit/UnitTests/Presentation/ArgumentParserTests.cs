using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Presentation
{
    public class ArgumentParserTests
    {
        [Fact]
        public void ParseConversionMode_WithExplicitOutputName_OutputFileNameIsRespected()
        {
            // Arrange
            var parser = new ArgumentParser();
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            var args = new[]
            {
                "-z", zplContent,
                "-o", "/tmp",
                "-n", "output.pdf"
            };

            // Act
            var result = parser.ParseConversionMode(args, 0);

            // Assert
            result.OutputFileName.Should().Be("output.pdf");
        }

        [Fact]
        public void ParseConversionMode_WithNoOutputName_GeneratesFileNameFromZplContent()
        {
            // Arrange
            var parser = new ArgumentParser();
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            var args = new[]
            {
                "-z", zplContent,
                "-o", "/tmp"
            };

            // Act
            var result = parser.ParseConversionMode(args, 0);

            // Assert
            result.OutputFileName.Should().StartWith("ZPL2PDF_");
            result.OutputFileName.Should().EndWith(".pdf");
        }

        [Fact]
        public void ParseConversionMode_WithStdoutFlag_SetsStandardOutput()
        {
            // Arrange
            var parser = new ArgumentParser();
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            var args = new[]
            {
                "-z", zplContent,
                "--stdout"
            };

            // Act
            var result = parser.ParseConversionMode(args, 0);

            // Assert
            result.StandardOutput.Should().BeTrue();
        }
    }
}

