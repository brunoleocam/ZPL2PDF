using System.Collections.Generic;

namespace ZPL2PDF.Domain.Services
{
    /// <summary>
    /// Interface for label rendering service
    /// </summary>
    public interface ILabelRenderer
    {
        /// <summary>
        /// Renders ZPL labels to image data
        /// </summary>
        /// <param name="labels">List of ZPL label strings</param>
        /// <returns>List of image data as byte arrays</returns>
        List<byte[]> RenderLabels(List<string> labels);

        /// <summary>
        /// Gets the current label dimensions
        /// </summary>
        /// <returns>Label dimensions</returns>
        (double width, double height, string unit, int dpi) GetDimensions();
    }
}
