using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for LabelFileReader
    /// </summary>
    public class LabelFileReaderTests
    {
        [Fact]
        public void SplitLabels_WithValidZpl_ReturnsLabels()
        {
            // Arrange
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().HaveCount(1);
            labels[0].Should().Contain("^XA");
            labels[0].Should().Contain("^XZ");
        }

        [Fact]
        public void SplitLabels_WithMultipleLabels_ReturnsAllLabels()
        {
            // Arrange
            var zplContent = "^XA^FO50,50^A0N,50,50^FDLabel 1^FS^XZ^XA^FO50,50^A0N,50,50^FDLabel 2^FS^XZ";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().HaveCount(2);
            labels[0].Should().Contain("Label 1");
            labels[1].Should().Contain("Label 2");
        }

        [Fact]
        public void SplitLabels_WithEmptyContent_ReturnsEmptyList()
        {
            // Arrange
            var zplContent = "";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().BeEmpty();
        }

        [Fact]
        public void SplitLabels_WithNoLabels_ReturnsEmptyList()
        {
            // Arrange
            var zplContent = "This is not ZPL content";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().BeEmpty();
        }

        [Fact]
        public void SplitLabels_WithIncompleteLabel_ReturnsEmptyList()
        {
            // Arrange
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().BeEmpty();
        }

        [Fact]
        public void SplitLabels_WithGraphicElements_IncludesGraphicElements()
        {
            // Arrange
            var zplContent = "^PR2^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().HaveCount(1);
            labels[0].Should().Contain("^PR2");
            labels[0].Should().Contain("^XA");
            labels[0].Should().Contain("^XZ");
        }

        [Theory]
        [InlineData("^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ", 1)]
        [InlineData("^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ^XA^FO50,50^A0N,50,50^FDTest Label 2^FS^XZ", 2)]
        [InlineData("^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ^XA^FO50,50^A0N,50,50^FDTest Label 2^FS^XZ^XA^FO50,50^A0N,50,50^FDTest Label 3^FS^XZ", 3)]
        public void SplitLabels_WithVariousInputs_ReturnsCorrectCount(string zplContent, int expectedCount)
        {
            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert
            labels.Should().HaveCount(expectedCount);
        }
    }
}
