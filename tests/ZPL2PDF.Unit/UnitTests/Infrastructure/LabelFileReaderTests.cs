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

        [Fact]
        public void PreprocessZpl_WithFhAndUtf8HexInTextField_DecodesToUtf8()
        {
            // Arrange
            // _C3_A3 is UTF-8 for "ã"
            var zpl = "^XA^FH^FD_C3_A3^FS^XZ";

            // Act
            var processed = LabelFileReader.PreprocessZpl(zpl);

            // Assert
            processed.Should().Contain("^FDã^FS");
        }

        [Fact]
        public void PreprocessZpl_WithAztecB0_DoesWorkaroundAndDoesNotDecodeBarcodePayload()
        {
            // Arrange
            // Minimal Aztec-like sequence from the reported label.
            var zpl = "^XA^B0N,4,N,0,N,1^FH^FD[)>_1D03_1D75^FS^XZ";

            // Act
            var processed = LabelFileReader.PreprocessZpl(zpl);

            // Assert: workaround for viewer typo
            processed.Should().Contain("^BON");

            // Assert: barcode payload must keep _XX sequences intact
            processed.Should().Contain("_1D03");
            processed.Should().Contain("_1D75");
        }

        [Fact]
        public void PreprocessZpl_WithCode128AndUnderscoreHexPattern_KeepsBarcodePayloadUntouched()
        {
            // Arrange
            var zpl = "^XA^BY2,3,80^BCN,80,Y,N^FH\\^FD123_ba456^FS^XZ";

            // Act
            var processed = LabelFileReader.PreprocessZpl(zpl);

            // Assert
            processed.Should().Contain("^FD123_ba456^FS");
        }

        [Fact]
        public void PreprocessZpl_WithFhBackslashIndicator_UnderscorePlusHexInText_StaysLiteral()
        {
            // ZPL: optional ^FH parameter sets hex indicator to '\' — underscores in field data are not hex escapes.
            var zpl = "^XA^FH\\^CI28^FDLote: 123_ba456^FS^CI27^XZ";

            var processed = LabelFileReader.PreprocessZpl(zpl);

            processed.Should().Contain("^FDLote: 123_ba456^FS");
        }

        [Fact]
        public void PreprocessZpl_WithFhBackslashIndicator_BackslashHexPairs_DecodeToUtf8()
        {
            // UTF-8 for "ã" using '\' as hex indicator (same bytes as _C3_A3 with default indicator).
            var zpl = "^XA^FH\\^FD\\C3\\A3^FS^XZ";

            var processed = LabelFileReader.PreprocessZpl(zpl);

            processed.Should().Contain("^FDã^FS");
        }

        [Fact]
        public void PreprocessZpl_UserReportLabel_FhBackslashCi28Bcn_PlaceholdersPreserved()
        {
            // Full label from user report (GitHub): ^FH\ + ^CI28 text line + Code 128 + same placeholder in barcode.
            var zpl = "^XA\n"
                + "^PW2400\n"
                + "^LL1920\n"
                + "^LS0\n\n"
                + "^FO50,50^AAN,50,50^FDTest^FS\n"
                + "^FT45,180^AAN,42,37^FH\\^CI28^FDLote: {{nf_item_batch_number}}^FS^CI27\n"
                + "^BY6,3,128^FT57,400^BCN,,Y,N\n"
                + "^FH\\^FD{{nf_item_batch_number}}^FS\n"
                + "^XZ";

            var processed = LabelFileReader.PreprocessZpl(zpl);

            processed.Should().Contain("^FDLote: {{nf_item_batch_number}}^FS");
            processed.Should().Contain("^FD{{nf_item_batch_number}}^FS");
            processed.Should().NotContain("\uFFFD", because: "replacement char must not appear after preprocess for this ZPL.");
        }

        [Fact]
        public void PreprocessZpl_UserReportLabel_FhBackslashCi28Bcn_SubstitutedBatchWithUnderscoreHexPattern_Preserved()
        {
            // Same layout as user report; substituted value contains _ba (underscore + hex digits) — must stay literal with ^FH\.
            var zpl = "^XA\n"
                + "^PW2400\n"
                + "^LL1920\n"
                + "^LS0\n\n"
                + "^FO50,50^AAN,50,50^FDTest^FS\n"
                + "^FT45,180^AAN,42,37^FH\\^CI28^FDLote: BATCH_ba99_x^FS^CI27\n"
                + "^BY6,3,128^FT57,400^BCN,,Y,N\n"
                + "^FH\\^FDBATCH_ba99_x^FS\n"
                + "^XZ";

            var processed = LabelFileReader.PreprocessZpl(zpl);

            processed.Should().Contain("^FDLote: BATCH_ba99_x^FS");
            processed.Should().Contain("^FDBATCH_ba99_x^FS");
            processed.Should().NotContain("\uFFFD");
        }
    }
}
