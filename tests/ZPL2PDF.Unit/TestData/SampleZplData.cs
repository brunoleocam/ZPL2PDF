using System.Collections.Generic;

namespace ZPL2PDF.Tests.TestData
{
    /// <summary>
    /// Sample ZPL data for testing
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
    }
}
