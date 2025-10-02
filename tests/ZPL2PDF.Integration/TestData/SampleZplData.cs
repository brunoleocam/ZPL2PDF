using System.Collections.Generic;

namespace ZPL2PDF.Tests.TestData
{
    /// <summary>
    /// Sample ZPL data for integration testing
    /// </summary>
    public static class SampleZplData
    {
        public static readonly string SimpleLabel = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

        public static readonly string LabelWithDimensions = "^XA^PW400^LL200^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

        public static readonly string MultipleLabels = "^XA^FO50,50^A0N,50,50^FDLabel 1^FS^XZ^XA^FO50,50^A0N,50,50^FDLabel 2^FS^XZ";

        public static readonly string ComplexLabel = "^XA^FO50,50^A0N,50,50^FDComplex Label^FS^FO50,100^A0N,30,30^FDSubtitle^FS^XZ";

        public static readonly List<string> TestLabels = new List<string>
        {
            SimpleLabel,
            LabelWithDimensions,
            ComplexLabel
        };

        /// <summary>
        /// Generates a large ZPL content for performance testing
        /// </summary>
        public static string GenerateLargeZplContent(int lineCount = 100)
        {
            var zpl = "^XA";
            for (int i = 0; i < lineCount; i++)
            {
                zpl += $"^FO50,{50 + i * 20}^A0N,30,30^FDLine {i + 1}^FS";
            }
            zpl += "^XZ";
            return zpl;
        }

        /// <summary>
        /// Generates ZPL content with specific dimensions
        /// </summary>
        public static string GenerateZplWithDimensions(int width, int height)
        {
            return $"^XA^PW{width}^LL{height}^FO50,50^A0N,50,50^FDTest Label^FS^XZ";
        }

        /// <summary>
        /// Generates invalid ZPL content for error testing
        /// </summary>
        public static string GenerateInvalidZplContent()
        {
            return "INVALID_ZPL_CONTENT_WITHOUT_PROPER_FORMAT";
        }
    }
}
