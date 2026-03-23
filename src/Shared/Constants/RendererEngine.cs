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
}

