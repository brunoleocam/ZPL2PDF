namespace ZPL2PDF.Presentation.Api.Models
{
    /// <summary>
    /// Response model for ZPL conversion API
    /// </summary>
    public class ConvertResponse
    {
        /// <summary>
        /// Indicates if the conversion was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Output format: "pdf" or "png"
        /// </summary>
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Base64 encoded PDF (when format is "pdf")
        /// </summary>
        public string? Pdf { get; set; }

        /// <summary>
        /// Base64 encoded PNG image (when format is "png" and single label)
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Base64 encoded PNG images array (when format is "png" and multiple labels)
        /// </summary>
        public List<string>? Images { get; set; }

        /// <summary>
        /// Number of pages/labels processed
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// Success or error message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
