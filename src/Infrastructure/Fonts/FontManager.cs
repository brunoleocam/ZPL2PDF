using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SkiaSharp;

namespace ZPL2PDF.Infrastructure.Fonts
{
    /// <summary>
    /// Manages custom TrueType fonts for ZPL rendering.
    /// Allows loading fonts from a directory or individual files and mapping them to ZPL font identifiers.
    /// </summary>
    public class FontManager : IDisposable
    {
        private readonly Dictionary<string, FontMapping> _fontMappings;
        private readonly Dictionary<string, SKTypeface> _loadedFonts;
        private bool _disposed;

        /// <summary>
        /// Gets the number of loaded fonts.
        /// </summary>
        public int LoadedFontCount => _loadedFonts.Count;

        /// <summary>
        /// Gets the available font IDs.
        /// </summary>
        public IEnumerable<string> AvailableFontIds => _fontMappings.Keys;

        /// <summary>
        /// Initializes a new instance of the FontManager.
        /// </summary>
        public FontManager()
        {
            _fontMappings = new Dictionary<string, FontMapping>(StringComparer.OrdinalIgnoreCase);
            _loadedFonts = new Dictionary<string, SKTypeface>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Loads fonts from a directory. Each .ttf file is mapped to a font ID based on its filename.
        /// </summary>
        /// <param name="directoryPath">Path to the fonts directory.</param>
        /// <returns>Number of fonts loaded.</returns>
        public int LoadFontsFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Fonts directory not found: {directoryPath}");
                return 0;
            }

            var fontFiles = Directory.GetFiles(directoryPath, "*.ttf", SearchOption.TopDirectoryOnly);
            var otfFiles = Directory.GetFiles(directoryPath, "*.otf", SearchOption.TopDirectoryOnly);
            var allFontFiles = new List<string>(fontFiles);
            allFontFiles.AddRange(otfFiles);

            int loadedCount = 0;

            foreach (var fontFile in allFontFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(fontFile);
                
                // Try to extract font ID from filename (e.g., "FontA.ttf" -> "A", "Font0.ttf" -> "0")
                var fontId = ExtractFontIdFromFileName(fileName);
                
                if (LoadFont(fontId, fontFile))
                {
                    loadedCount++;
                }
            }

            Console.WriteLine($"Loaded {loadedCount} font(s) from {directoryPath}");
            return loadedCount;
        }

        /// <summary>
        /// Loads a single font and maps it to a ZPL font ID.
        /// </summary>
        /// <param name="zplFontId">ZPL font identifier (e.g., "0", "A", "P").</param>
        /// <param name="fontFilePath">Path to the TrueType font file.</param>
        /// <returns>True if loaded successfully, false otherwise.</returns>
        public bool LoadFont(string zplFontId, string fontFilePath)
        {
            if (string.IsNullOrWhiteSpace(zplFontId))
            {
                Console.WriteLine("Font ID cannot be empty");
                return false;
            }

            if (!File.Exists(fontFilePath))
            {
                Console.WriteLine($"Font file not found: {fontFilePath}");
                return false;
            }

            try
            {
                var typeface = SKTypeface.FromFile(fontFilePath);
                if (typeface == null)
                {
                    Console.WriteLine($"Failed to load font: {fontFilePath}");
                    return false;
                }

                var fontId = zplFontId.ToUpperInvariant();

                // Dispose existing font if present
                if (_loadedFonts.TryGetValue(fontId, out var existingFont))
                {
                    existingFont.Dispose();
                }

                _loadedFonts[fontId] = typeface;
                _fontMappings[fontId] = new FontMapping
                {
                    ZplFontId = fontId,
                    Name = typeface.FamilyName,
                    FontFile = fontFilePath,
                    Style = GetFontStyle(typeface)
                };

                Console.WriteLine($"Loaded font: {fontId} = {typeface.FamilyName} ({fontFilePath})");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading font {fontFilePath}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads fonts from a mapping string (e.g., "A=C:\Fonts\Arial.ttf").
        /// </summary>
        /// <param name="mappingString">Font mapping string.</param>
        /// <returns>True if loaded successfully, false otherwise.</returns>
        public bool LoadFontFromMapping(string mappingString)
        {
            var mapping = FontMapping.FromString(mappingString);
            if (mapping == null)
            {
                Console.WriteLine($"Invalid font mapping: {mappingString}");
                return false;
            }

            return LoadFont(mapping.ZplFontId, mapping.FontFile);
        }

        /// <summary>
        /// Loads font mappings from a JSON configuration file.
        /// </summary>
        /// <param name="configFilePath">Path to the JSON configuration file.</param>
        /// <returns>Number of fonts loaded.</returns>
        public int LoadFontsFromConfig(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                Console.WriteLine($"Font configuration file not found: {configFilePath}");
                return 0;
            }

            try
            {
                var json = File.ReadAllText(configFilePath);
                var config = JsonSerializer.Deserialize<FontMappingConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (config == null)
                {
                    Console.WriteLine("Failed to parse font configuration");
                    return 0;
                }

                // Load from default directory if specified
                int loadedCount = 0;
                if (!string.IsNullOrEmpty(config.DefaultFontsDirectory))
                {
                    var basePath = Path.GetDirectoryName(configFilePath) ?? ".";
                    var fontsDir = Path.IsPathRooted(config.DefaultFontsDirectory)
                        ? config.DefaultFontsDirectory
                        : Path.Combine(basePath, config.DefaultFontsDirectory);
                    
                    loadedCount += LoadFontsFromDirectory(fontsDir);
                }

                // Load individual mappings
                foreach (var mapping in config.FontMappings.Values)
                {
                    var basePath = Path.GetDirectoryName(configFilePath) ?? ".";
                    var fontPath = Path.IsPathRooted(mapping.FontFile)
                        ? mapping.FontFile
                        : Path.Combine(basePath, mapping.FontFile);

                    if (LoadFont(mapping.ZplFontId, fontPath))
                    {
                        loadedCount++;
                    }
                }

                return loadedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading font configuration: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Gets a loaded font by its ZPL font ID.
        /// </summary>
        /// <param name="zplFontId">ZPL font identifier.</param>
        /// <returns>SKTypeface if found, null otherwise.</returns>
        public SKTypeface? GetFont(string zplFontId)
        {
            if (string.IsNullOrWhiteSpace(zplFontId))
                return null;

            _loadedFonts.TryGetValue(zplFontId.ToUpperInvariant(), out var typeface);
            return typeface;
        }

        /// <summary>
        /// Gets the font mapping for a ZPL font ID.
        /// </summary>
        /// <param name="zplFontId">ZPL font identifier.</param>
        /// <returns>FontMapping if found, null otherwise.</returns>
        public FontMapping? GetFontMapping(string zplFontId)
        {
            if (string.IsNullOrWhiteSpace(zplFontId))
                return null;

            _fontMappings.TryGetValue(zplFontId.ToUpperInvariant(), out var mapping);
            return mapping;
        }

        /// <summary>
        /// Checks if a font is loaded for the given ZPL font ID.
        /// </summary>
        /// <param name="zplFontId">ZPL font identifier.</param>
        /// <returns>True if font is loaded, false otherwise.</returns>
        public bool HasFont(string zplFontId)
        {
            if (string.IsNullOrWhiteSpace(zplFontId))
                return false;

            return _loadedFonts.ContainsKey(zplFontId.ToUpperInvariant());
        }

        /// <summary>
        /// Lists all loaded fonts.
        /// </summary>
        public void ListLoadedFonts()
        {
            Console.WriteLine($"Loaded Fonts ({_loadedFonts.Count}):");
            foreach (var kvp in _fontMappings)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value.Name} ({kvp.Value.FontFile})");
            }
        }

        /// <summary>
        /// Extracts a font ID from a filename.
        /// </summary>
        private string ExtractFontIdFromFileName(string fileName)
        {
            // Try to find a single character that could be a font ID
            // e.g., "FontA" -> "A", "ZebraFont0" -> "0", "OCR-A" -> "A"
            
            // First, check if the filename ends with a single character
            if (fileName.Length > 0)
            {
                var lastChar = fileName[fileName.Length - 1];
                if (char.IsLetterOrDigit(lastChar))
                {
                    return lastChar.ToString().ToUpperInvariant();
                }
            }

            // Otherwise, use the first character
            if (fileName.Length > 0)
            {
                return fileName[0].ToString().ToUpperInvariant();
            }

            return "0";
        }

        /// <summary>
        /// Gets the font style string from a typeface.
        /// </summary>
        private string GetFontStyle(SKTypeface typeface)
        {
            var style = typeface.FontStyle;
            
            if (style.Weight >= 600 && style.Slant != SKFontStyleSlant.Upright)
                return "BoldItalic";
            if (style.Weight >= 600)
                return "Bold";
            if (style.Slant != SKFontStyleSlant.Upright)
                return "Italic";
            
            return "Regular";
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            foreach (var typeface in _loadedFonts.Values)
            {
                typeface.Dispose();
            }

            _loadedFonts.Clear();
            _fontMappings.Clear();
            _disposed = true;
        }
    }
}

