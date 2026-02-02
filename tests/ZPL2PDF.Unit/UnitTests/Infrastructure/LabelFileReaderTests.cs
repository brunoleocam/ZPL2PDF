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
            // Arrange: content before ^XA (e.g. ^PR2) should not prevent label from being found
            var zplContent = "^PR2^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert: we return the ^XA...^XZ label (prepended graphics come from ~DGR only)
            labels.Should().HaveCount(1);
            labels[0].Should().Contain("^XA");
            labels[0].Should().Contain("^XZ");
            labels[0].Should().Contain("Test Label");
        }

        [Fact]
        public void SplitLabels_WithFakeXaInsideDgrLine_IgnoresFakeLabelAndFindsRealLabel()
        {
            // Arrange: ^XA/^XZ inside ~DGR base64 must not be treated as label (issue #45)
            var zplContent = "~DGR:DEMO.GRF,100,1,:Z64:abc^XAxyz^XZ\r\n^XA^FO50,50^A0N,50,50^FDReal Label^FS^XZ";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert: only one label (the real ^XA...^XZ after newline); fake ^XA inside ~DGR is ignored
            labels.Should().HaveCount(1);
            labels[0].Should().Contain("Real Label");
            labels[0].Should().Contain("^XA^FO50,50"); // real label content
        }

        [Fact]
        public void SplitLabels_WithRealXaAfterDgrPayloadOnSameLine_FindsLabel()
        {
            // Arrange: thermal-style — ~DGR and ^XA on same line; ^XA after :checksum is valid (issue #45)
            var zplContent = "~DGR:DEMO.GRF,10,1,:Z64:ab:XX^XA^FO0,0^A0N,20,20^FDTest^FS^XZ";

            // Act
            var labels = LabelFileReader.SplitLabels(zplContent);

            // Assert: one label (^XA after payload on same line)
            labels.Should().HaveCount(1);
            labels[0].Should().Contain("Test");
            labels[0].Should().Contain("^XA^FO0,0");
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
