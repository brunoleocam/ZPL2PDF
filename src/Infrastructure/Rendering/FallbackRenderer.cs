using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Rendering
{
    /// <summary>
    /// A renderer wrapper that provides automatic fallback functionality.
    /// Tries the primary renderer first, falls back to secondary on failure.
    /// </summary>
    public class FallbackRenderer : ILabelRenderer
    {
        private readonly ILabelRenderer _primaryRenderer;
        private readonly ILabelRenderer _fallbackRenderer;

        /// <summary>
        /// Gets the name of this renderer (combination of primary and fallback).
        /// </summary>
        public string Name => $"{_primaryRenderer.Name} (fallback: {_fallbackRenderer.Name})";

        /// <summary>
        /// Indicates whether the primary renderer can render directly to PDF.
        /// </summary>
        public bool CanRenderDirectToPdf => _primaryRenderer.CanRenderDirectToPdf;

        /// <summary>
        /// Initializes a new instance of the FallbackRenderer.
        /// </summary>
        /// <param name="primaryRenderer">The primary renderer to try first.</param>
        /// <param name="fallbackRenderer">The fallback renderer to use if primary fails.</param>
        public FallbackRenderer(ILabelRenderer primaryRenderer, ILabelRenderer fallbackRenderer)
        {
            _primaryRenderer = primaryRenderer ?? throw new ArgumentNullException(nameof(primaryRenderer));
            _fallbackRenderer = fallbackRenderer ?? throw new ArgumentNullException(nameof(fallbackRenderer));
        }

        /// <summary>
        /// Checks if either renderer is available.
        /// </summary>
        /// <returns>True if at least one renderer is available.</returns>
        public bool IsAvailable()
        {
            return _primaryRenderer.IsAvailable() || _fallbackRenderer.IsAvailable();
        }

        /// <summary>
        /// Renders ZPL labels to images synchronously with automatic fallback.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List of rendered images as byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            return RenderLabelsAsync(labels, widthMm, heightMm, dpi).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Renders ZPL labels to images asynchronously with automatic fallback.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>Task containing list of rendered images as byte arrays.</returns>
        public async Task<List<byte[]>> RenderLabelsAsync(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            // Try primary renderer first
            if (_primaryRenderer.IsAvailable())
            {
                try
                {
                    return await _primaryRenderer.RenderLabelsAsync(labels, widthMm, heightMm, dpi);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Primary renderer ({_primaryRenderer.Name}) failed: {ex.Message}");
                    Console.WriteLine($"Falling back to {_fallbackRenderer.Name}...");
                }
            }
            else
            {
                Console.WriteLine($"Primary renderer ({_primaryRenderer.Name}) not available, using fallback...");
            }

            // Fall back to secondary renderer
            return await _fallbackRenderer.RenderLabelsAsync(labels, widthMm, heightMm, dpi);
        }

        /// <summary>
        /// Renders ZPL labels to PDF format with automatic fallback.
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List of PDF byte arrays.</returns>
        public List<byte[]> RenderLabelsToPdf(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            // Try primary renderer first
            if (_primaryRenderer.IsAvailable())
            {
                try
                {
                    return _primaryRenderer.RenderLabelsToPdf(labels, widthMm, heightMm, dpi);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Primary renderer ({_primaryRenderer.Name}) failed: {ex.Message}");
                    Console.WriteLine($"Falling back to {_fallbackRenderer.Name}...");
                }
            }
            else
            {
                Console.WriteLine($"Primary renderer ({_primaryRenderer.Name}) not available, using fallback...");
            }

            // Fall back to secondary renderer
            return _fallbackRenderer.RenderLabelsToPdf(labels, widthMm, heightMm, dpi);
        }
    }
}

