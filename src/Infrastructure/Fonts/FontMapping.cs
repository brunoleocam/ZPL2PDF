using System;
using System.Collections.Generic;

namespace ZPL2PDF.Infrastructure.Fonts
{
    /// <summary>
    /// Represents a mapping between a ZPL font identifier and a TrueType font file.
    /// </summary>
    public class FontMapping
    {
        /// <summary>
        /// Gets or sets the ZPL font identifier (e.g., "0", "A", "B", "P", "Q").
        /// </summary>
        public string ZplFontId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the font.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the TrueType font file (.ttf).
        /// </summary>
        public string FontFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the font style (Regular, Bold, Italic, BoldItalic).
        /// </summary>
        public string Style { get; set; } = "Regular";

        /// <summary>
        /// Gets or sets the character width adjustment factor.
        /// </summary>
        public double WidthFactor { get; set; } = 1.0;

        /// <summary>
        /// Gets or sets the character height adjustment factor.
        /// </summary>
        public double HeightFactor { get; set; } = 1.0;

        /// <summary>
        /// Gets or sets whether this is a bitmap-style font (fixed width).
        /// </summary>
        public bool IsBitmapStyle { get; set; } = false;

        /// <summary>
        /// Creates a FontMapping from a simple string format (e.g., "A=C:\Fonts\Arial.ttf").
        /// </summary>
        /// <param name="mappingString">Mapping string in format "ID=PATH".</param>
        /// <returns>FontMapping instance or null if invalid format.</returns>
        public static FontMapping? FromString(string mappingString)
        {
            if (string.IsNullOrWhiteSpace(mappingString))
                return null;

            var parts = mappingString.Split('=', 2);
            if (parts.Length != 2)
                return null;

            return new FontMapping
            {
                ZplFontId = parts[0].Trim().ToUpperInvariant(),
                FontFile = parts[1].Trim(),
                Name = System.IO.Path.GetFileNameWithoutExtension(parts[1].Trim())
            };
        }

        /// <summary>
        /// Returns a string representation of the font mapping.
        /// </summary>
        public override string ToString()
        {
            return $"{ZplFontId}={Name} ({FontFile})";
        }
    }

    /// <summary>
    /// Configuration for font mappings, typically loaded from JSON.
    /// </summary>
    public class FontMappingConfiguration
    {
        /// <summary>
        /// Gets or sets the dictionary of font mappings keyed by ZPL font ID.
        /// </summary>
        public Dictionary<string, FontMapping> FontMappings { get; set; } = new Dictionary<string, FontMapping>();

        /// <summary>
        /// Gets or sets the default fonts directory.
        /// </summary>
        public string? DefaultFontsDirectory { get; set; }

        /// <summary>
        /// Gets or sets the default font ID to use when no font is specified.
        /// </summary>
        public string DefaultFontId { get; set; } = "0";
    }
}

