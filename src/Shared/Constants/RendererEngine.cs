using System;

namespace ZPL2PDF.Shared.Constants
{
    /// <summary>
    /// Rendering engine selection for ZPL to PDF.
    /// </summary>
    public enum RendererEngine
    {
        /// <summary>
        /// Offline rendering using BinaryKits (no internet).
        /// </summary>
        Offline,

        /// <summary>
        /// Rendering via Labelary online API.
        /// </summary>
        Labelary,

        /// <summary>
        /// Try Labelary first, fallback to Offline on failure.
        /// </summary>
        Auto
    }

    /// <summary>
    /// Parsing and DPI helpers for <see cref="RendererEngine"/>.
    /// </summary>
    public static class RendererEngineParser
    {
        private static readonly int[] LabelaryDpmm = { 6, 8, 12, 24 };
        private const double MmPerInch = 25.4;

        /// <summary>
        /// Parses "offline", "labelary" or "auto" (case-insensitive). Returns false for anything else.
        /// </summary>
        public static bool TryParse(string? value, out RendererEngine engine)
        {
            switch ((value ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "offline": engine = RendererEngine.Offline; return true;
                case "labelary": engine = RendererEngine.Labelary; return true;
                case "auto": engine = RendererEngine.Auto; return true;
                default: engine = default; return false;
            }
        }

        /// <summary>
        /// Dots per millimeter the engine actually renders at for a nominal DPI.
        /// Offline renders at round(dpi / 25.4); Labelary only supports 6, 8, 12 and 24 dpmm,
        /// so the closest one is used. Auto follows Labelary (its offline fallback is aligned to it).
        /// </summary>
        public static int ToDpmm(int dpi, RendererEngine engine)
        {
            double raw = (dpi > 0 ? dpi : 203) / MmPerInch;
            if (engine == RendererEngine.Offline)
            {
                return Math.Max(1, (int)Math.Round(raw));
            }

            int best = LabelaryDpmm[0];
            foreach (var candidate in LabelaryDpmm)
            {
                if (Math.Abs(candidate - raw) < Math.Abs(best - raw)) best = candidate;
            }
            return best;
        }

        /// <summary>
        /// Effective DPI of the rendered images (dpmm * 25.4), to be used when sizing PDF pages.
        /// </summary>
        public static int ToEffectiveDpi(int dpi, RendererEngine engine)
            => (int)Math.Round(ToDpmm(dpi, engine) * MmPerInch);
    }
}
