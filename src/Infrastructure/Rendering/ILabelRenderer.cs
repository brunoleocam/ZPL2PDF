using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Rendering
{
    /// <summary>
    /// Interface for label rendering engines.
    /// Supports both offline (BinaryKits) and online (Labelary API) rendering.
    /// </summary>
    public interface ILabelRenderer
    {
        /// <summary>
        /// Gets the name of the renderer (e.g., "BinaryKits", "Labelary")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Checks if the renderer is currently available.
        /// For online renderers, this may check network connectivity.
        /// </summary>
        /// <returns>True if the renderer is available, false otherwise.</returns>
        bool IsAvailable();

        /// <summary>
        /// Renders ZPL labels to images synchronously.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI (e.g., 203, 300, 600).</param>
        /// <returns>List of rendered images as byte arrays (PNG format).</returns>
        List<byte[]> RenderLabels(List<string> labels, double widthMm, double heightMm, int dpi);

        /// <summary>
        /// Renders ZPL labels to images asynchronously.
        /// Preferred method for online renderers to avoid blocking.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI (e.g., 203, 300, 600).</param>
        /// <returns>Task containing list of rendered images as byte arrays (PNG format).</returns>
        Task<List<byte[]>> RenderLabelsAsync(List<string> labels, double widthMm, double heightMm, int dpi);

        /// <summary>
        /// Renders ZPL labels directly to PDF format.
        /// For Labelary renderer, this generates vectorial PDF with optimal quality.
        /// For BinaryKits renderer, this generates images and converts to PDF.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI (e.g., 203, 300, 600).</param>
        /// <returns>List of PDF byte arrays. Labelary returns one PDF per batch (up to 50 labels).</returns>
        List<byte[]> RenderLabelsToPdf(List<string> labels, double widthMm, double heightMm, int dpi);

        /// <summary>
        /// Indicates whether this renderer can generate PDF directly (without intermediate PNG).
        /// </summary>
        bool CanRenderDirectToPdf { get; }
    }

    /// <summary>
    /// Enum representing available renderer modes.
    /// </summary>
    public enum RendererMode
    {
        /// <summary>
        /// Offline rendering using BinaryKits.Zpl.Viewer (default).
        /// Works without internet connection but may have less font fidelity.
        /// </summary>
        Offline,

        /// <summary>
        /// Online rendering using Labelary API.
        /// Provides high-fidelity rendering but requires internet connection.
        /// Subject to rate limits (3 req/s, 5000 req/day on free tier).
        /// </summary>
        Labelary,

        /// <summary>
        /// Automatic mode: tries Labelary first, falls back to BinaryKits if unavailable.
        /// Best of both worlds: high fidelity when online, reliability when offline.
        /// </summary>
        Auto
    }
}

