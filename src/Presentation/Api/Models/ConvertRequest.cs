using System.ComponentModel.DataAnnotations;

namespace ZPL2PDF.Presentation.Api.Models
{
    /// <summary>
    /// Request model for ZPL to PDF/PNG conversion API
    /// </summary>
    public class ConvertRequest
    {
        /// <summary>
        /// ZPL content to convert (required if ZplArray is not provided)
        /// Can contain multiple labels separated by ^XA...^XZ
        /// </summary>
        public string? Zpl { get; set; }

        /// <summary>
        /// Array of ZPL contents to convert (required if Zpl is not provided)
        /// Each element can contain one or more labels
        /// </summary>
        public List<string>? ZplArray { get; set; }

        /// <summary>
        /// Output format: "pdf" (default) or "png"
        /// </summary>
        public string Format { get; set; } = "pdf";

        /// <summary>
        /// Label width (optional, will be extracted from ZPL if not provided)
        /// </summary>
        public double? Width { get; set; }

        /// <summary>
        /// Label height (optional, will be extracted from ZPL if not provided)
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Unit of measurement: "mm", "cm", or "in" (optional, default: "mm")
        /// </summary>
        public string Unit { get; set; } = "mm";

        /// <summary>
        /// Print density in DPI (optional, default: 203)
        /// </summary>
        public int? Dpi { get; set; }
    }
}
