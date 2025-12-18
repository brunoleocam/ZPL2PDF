using System;

namespace ZPL2PDF.Infrastructure.Rendering
{
    /// <summary>
    /// Factory for creating label renderers based on the specified mode.
    /// Handles automatic fallback logic for the Auto mode.
    /// </summary>
    public class RendererFactory
    {
        private static readonly Lazy<BinaryKitsRenderer> _binaryKitsRenderer = new Lazy<BinaryKitsRenderer>(() => new BinaryKitsRenderer());
        private static readonly Lazy<LabelaryRenderer> _labelaryRenderer = new Lazy<LabelaryRenderer>(() => new LabelaryRenderer());

        /// <summary>
        /// Creates a label renderer based on the specified mode.
        /// </summary>
        /// <param name="mode">The renderer mode to use.</param>
        /// <returns>An ILabelRenderer instance appropriate for the mode.</returns>
        /// <remarks>
        /// - Offline: Always returns BinaryKitsRenderer
        /// - Labelary: Always returns LabelaryRenderer (may fail if API unavailable)
        /// - Auto: Returns LabelaryRenderer if available, otherwise BinaryKitsRenderer
        /// </remarks>
        public static ILabelRenderer Create(RendererMode mode)
        {
            switch (mode)
            {
                case RendererMode.Offline:
                    return _binaryKitsRenderer.Value;

                case RendererMode.Labelary:
                    return _labelaryRenderer.Value;

                case RendererMode.Auto:
                    // Try Labelary first, fall back to BinaryKits if unavailable
                    if (_labelaryRenderer.Value.IsAvailable())
                    {
                        Console.WriteLine("Using Labelary API for high-fidelity rendering");
                        return _labelaryRenderer.Value;
                    }
                    else
                    {
                        Console.WriteLine("Labelary API unavailable, using offline BinaryKits renderer");
                        return _binaryKitsRenderer.Value;
                    }

                default:
                    return _binaryKitsRenderer.Value;
            }
        }

        /// <summary>
        /// Creates a renderer with automatic fallback.
        /// Tries the primary renderer first, falls back to secondary if primary fails.
        /// </summary>
        /// <param name="primaryMode">Primary renderer mode to try first.</param>
        /// <returns>A FallbackRenderer that handles automatic failover.</returns>
        public static ILabelRenderer CreateWithFallback(RendererMode primaryMode)
        {
            if (primaryMode == RendererMode.Auto || primaryMode == RendererMode.Labelary)
            {
                return new FallbackRenderer(_labelaryRenderer.Value, _binaryKitsRenderer.Value);
            }

            return _binaryKitsRenderer.Value;
        }

        /// <summary>
        /// Parses a string to RendererMode enum.
        /// </summary>
        /// <param name="modeString">String representation of the mode (offline, labelary, auto).</param>
        /// <returns>The corresponding RendererMode enum value.</returns>
        public static RendererMode ParseMode(string? modeString)
        {
            if (string.IsNullOrWhiteSpace(modeString))
                return RendererMode.Offline;

            return modeString.ToLowerInvariant() switch
            {
                "labelary" => RendererMode.Labelary,
                "auto" => RendererMode.Auto,
                "offline" => RendererMode.Offline,
                "binarykits" => RendererMode.Offline,
                _ => RendererMode.Offline
            };
        }
    }
}

