using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using SkiaSharp;
using ZPL2PDF.Domain.Services;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for processing labels, generating images in memory, and returning image data.
    /// </summary>
    public class LabelRenderer : ILabelRenderer {
        private readonly IPrinterStorage _printerStorage;
        private readonly ZplAnalyzer _analyzer;
        private readonly ZplElementDrawer _drawer;
        private readonly double _labelWidthMm;
        private readonly double _labelHeightMm;
        private readonly int _printDpi;
        private readonly double _labelWidthInput;
        private readonly double _labelHeightInput;
        private readonly string _labelUnitInput;
        private readonly int _labelDpi;
        private readonly string? _fontsDirectory;
        private readonly IReadOnlyList<(string Id, string Path)>? _fontMappings;

        private const double InchesToMm = 25.4;
        private const double CmToMm = 10.0;
        private const double DpiToDpmm = 25.4;

        /// <summary>
        /// Sans (narrow first) families appended after BinaryKits' own font-0 stack, so hosts without
        /// Arial/Helvetica (Linux/Docker) still get a sans-serif face instead of the system default (often a wide serif).
        /// </summary>
        private static readonly string[] FallbackStack0 = {
            "Liberation Sans Narrow", "Arial Narrow", "Liberation Sans", "Arimo", "Noto Sans", "Open Sans",
            "DejaVu Sans Condensed", "DejaVu Sans", "FreeSans"
        };

        /// <summary>Monospace families appended after BinaryKits' own stack used for ZPL fonts A-Z.</summary>
        private static readonly string[] FallbackStackA = {
            "Liberation Mono", "Noto Sans Mono", "Ubuntu Mono", "FreeMono", "Courier New"
        };

        private static readonly Lazy<HashSet<string>> InstalledFamilies = new Lazy<HashSet<string>>(() =>
            new HashSet<string>(SKFontManager.Default.GetFontFamilies(), StringComparer.OrdinalIgnoreCase));

        /// <summary>
        /// Creates DrawerOptions with high quality settings, the extended font fallback stacks and,
        /// when --fonts-dir / --font are used, a font loader for ^A0N, ^AAN, ^ABN, etc.
        /// </summary>
        private DrawerOptions CreateDrawerOptions() {
            var fontManager = CreateFontManager();
            return new DrawerOptions(fontManager) {
                RenderFormat = SKEncodedImageFormat.Png,
                RenderQuality = 100,
                PdfOutput = false,
                OpaqueBackground = false
            };
        }

        /// <summary>
        /// Builds the BinaryKits FontManager. Note: since BinaryKits.Zpl.Viewer 1.3.x the custom loader must be set on
        /// <see cref="FontManager.FontLoader"/>; <c>DrawerOptions.FontLoader</c> is no longer consulted.
        /// </summary>
        private FontManager CreateFontManager() {
            var fontManager = new FontManager();
            AppendMissing(fontManager.FontStack0, FallbackStack0);
            AppendMissing(fontManager.FontStackA, FallbackStackA);
            if (_fontsDirectory != null || (_fontMappings != null && _fontMappings.Count > 0)) {
                fontManager.FontLoader = CreateFontLoader(fontManager);
            }
            return fontManager;
        }

        public static void AppendMissing(List<string> stack, IEnumerable<string> fallbacks) {
            foreach (var family in fallbacks) {
                if (!stack.Contains(family, StringComparer.OrdinalIgnoreCase)) {
                    stack.Add(family);
                }
            }
        }

        /// <summary>
        /// Picks the first installed family of the stack (font 0: bold semi-condensed, like BinaryKits; others: regular).
        /// </summary>
        private static SKTypeface ResolveFromStack(string fontId, FontManager fontManager) {
            bool isFont0 = fontId == "0";
            var style = isFont0
                ? new SKFontStyle(SKFontStyleWeight.Bold, SKFontStyleWidth.SemiCondensed, SKFontStyleSlant.Upright)
                : SKFontStyle.Normal;
            foreach (var family in isFont0 ? fontManager.FontStack0 : fontManager.FontStackA) {
                if (InstalledFamilies.Value.Contains(family)) {
                    var typeface = SKTypeface.FromFamilyName(family, style);
                    if (typeface != null) return typeface;
                }
            }
            return SKTypeface.FromFamilyName(null, style) ?? SKTypeface.Default;
        }

        /// <summary>
        /// Builds a delegate that resolves ZPL font ID (0, A, B, ...) to SKTypeface from --fonts-dir and --font mappings.
        /// Unmapped IDs use the default font stacks.
        /// </summary>
        private Func<string, SKTypeface> CreateFontLoader(FontManager fontManager) {
            var fontsDir = _fontsDirectory ?? string.Empty;
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (_fontMappings != null) {
                foreach (var (id, path) in _fontMappings) {
                    if (string.IsNullOrEmpty(id)) continue;
                    var resolved = ResolveFontPath(path, fontsDir);
                    mappings[id.Trim()] = resolved;
                }
            }
            return fontId => {
                if (string.IsNullOrEmpty(fontId)) return ResolveFromStack("0", fontManager);
                var key = fontId.Trim();
                if (mappings.TryGetValue(key, out var path) && File.Exists(path)) {
                    try { return SKTypeface.FromFile(path) ?? ResolveFromStack(key, fontManager); } catch { /* fallback */ }
                }
                if (!string.IsNullOrWhiteSpace(fontsDir)) {
                    var byName = Path.Combine(fontsDir, key + ".ttf");
                    if (File.Exists(byName)) {
                        try { var tf = SKTypeface.FromFile(byName); if (tf != null) return tf; } catch { }
                    }
                    var byNameOtf = Path.Combine(fontsDir, key + ".otf");
                    if (File.Exists(byNameOtf)) {
                        try { var tf = SKTypeface.FromFile(byNameOtf); if (tf != null) return tf; } catch { }
                    }
                }
                return ResolveFromStack(key, fontManager);
            };
        }

        /// <summary>
        /// Resolves font path declared in --font mapping.
        /// Supports absolute paths and relative paths (with or without nested folders) against --fonts-dir.
        /// </summary>
        private static string ResolveFontPath(string declaredPath, string fontsDir) {
            if (string.IsNullOrWhiteSpace(declaredPath)) {
                return declaredPath;
            }

            if (Path.IsPathRooted(declaredPath) || string.IsNullOrWhiteSpace(fontsDir)) {
                return declaredPath;
            }

            // Keep nested relative path first (e.g. custom/arial.ttf), then fallback to only filename for compatibility.
            var combined = Path.Combine(fontsDir, declaredPath);
            if (File.Exists(combined)) {
                return combined;
            }

            return Path.Combine(fontsDir, Path.GetFileName(declaredPath));
        }

        /// <summary>
        /// Initializes a new instance of the LabelRenderer class, setting up the necessary dependencies for rendering labels into images.
        /// </summary>
        public LabelRenderer(double labelWidth, double labelHeight, int printDpi, string unit,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null) {
            _fontsDirectory = fontsDirectory;
            _fontMappings = fontMappings;
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);

            var drawerOptions = CreateDrawerOptions();
            _drawer = new ZplElementDrawer(_printerStorage, drawerOptions);

            // Convert width and height to millimeters based on the unit
            switch (unit) {
                case "in":
                    _labelWidthMm = labelWidth * InchesToMm;
                    _labelHeightMm = labelHeight * InchesToMm;
                    break;
                case "cm":
                    _labelWidthMm = labelWidth * CmToMm;
                    _labelHeightMm = labelHeight * CmToMm;
                    break;
                case "mm":
                    _labelWidthMm = labelWidth;
                    _labelHeightMm = labelHeight;
                    break;
                default:
                    _labelWidthMm = 60;   // 60 mm
                    _labelHeightMm = 120;  // 120 mm
                    break;
            }

            // Store DPI (will be converted to DPMM when rendering)
            _printDpi = printDpi;
            _labelWidthInput = labelWidth;
            _labelHeightInput = labelHeight;
            _labelUnitInput = unit;
            _labelDpi = printDpi;
        }

        /// <summary>
        /// Initializes a new instance of the LabelRenderer class using LabelDimensions (for daemon mode).
        /// </summary>
        public LabelRenderer(LabelDimensions dimensions,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null) {
            _fontsDirectory = fontsDirectory;
            _fontMappings = fontMappings;
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);

            var drawerOptions = CreateDrawerOptions();
            _drawer = new ZplElementDrawer(_printerStorage, drawerOptions);

            _labelWidthMm = dimensions.WidthMm;
            _labelHeightMm = dimensions.HeightMm;
            _printDpi = dimensions.Dpi;
            _labelWidthInput = dimensions.WidthMm;
            _labelHeightInput = dimensions.HeightMm;
            _labelUnitInput = "mm";
            _labelDpi = dimensions.Dpi;
        }

        /// <summary>
        /// Processes a list of ZPL labels and returns a list of images (in byte[]).
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <returns>List of images in byte arrays.</returns>
        /// <exception cref="ArgumentNullException">Thrown when labels is null.</exception>
        public List<byte[]> RenderLabels(List<string> labels) {
            if (labels == null) {
                throw new ArgumentNullException(nameof(labels));
            }

            var images = new List<byte[]>();
            
            // Convert DPI to DPMM for the drawer
            int dpmm = RendererEngineParser.ToDpmm(_printDpi, RendererEngine.Offline);
            
            for (int i = 0; i < labels.Count; i++) {
                var labelText = labels[i];
                
                // Create a fresh PrinterStorage and Analyzer for each label to avoid state pollution
                // This ensures that graphics from one label don't affect another
                var printerStorage = new PrinterStorage();
                var analyzer = new ZplAnalyzer(printerStorage);
                
                // Process the complete label (graphics + label together)
                // The ZplAnalyzer will process graphics first and load them into PrinterStorage
                // Then it will process the label which can reference those graphics
                var analyzeInfo = analyzer.Analyze(labelText);
                
                // Process all LabelInfos - the ZplAnalyzer should only generate one per ^XA...^XZ
                if (analyzeInfo.LabelInfos != null) {
                    foreach (var labelInfo in analyzeInfo.LabelInfos) {
                        // Create a fresh drawer for this label to use the correct PrinterStorage
                        var drawerOptions = CreateDrawerOptions();
                        var drawer = new ZplElementDrawer(printerStorage, drawerOptions);
                        
                        // Use DPMM for Draw (BinaryKits library expects DPMM)
                        byte[] imageData = drawer.Draw(labelInfo.ZplElements, _labelWidthMm, _labelHeightMm, dpmm);

                        // BinaryKits ignores ^PO (print orientation); honor ^POI by rotating the whole label 180 degrees.
                        if (IsInvertedOrientation(labelText)) {
                            imageData = RotateImage180(imageData);
                        }
                        images.Add(imageData);
                    }
                }
            }
            return images;
        }

        private static readonly System.Text.RegularExpressions.Regex FieldDataRegex = new System.Text.RegularExpressions.Regex(
            @"\^FD.*?\^FS", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled,
            TimeSpan.FromSeconds(5));

        private static readonly System.Text.RegularExpressions.Regex PrintOrientationRegex = new System.Text.RegularExpressions.Regex(
            @"\^PO([NI])", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled,
            TimeSpan.FromSeconds(5));

        /// <summary>
        /// Returns true when the label's effective ^PO (print orientation) is I (inverted, 180 degrees).
        /// The last ^PO in the label wins; ^FD...^FS field data is ignored so text like "^POI" is not misread.
        /// </summary>
        public static bool IsInvertedOrientation(string zpl) {
            if (string.IsNullOrEmpty(zpl)) return false;
            var withoutFields = FieldDataRegex.Replace(zpl, string.Empty);
            var matches = PrintOrientationRegex.Matches(withoutFields);
            return matches.Count > 0 &&
                   matches[matches.Count - 1].Groups[1].Value.Equals("I", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Rotates a PNG image by 180 degrees (used for ^POI).
        /// </summary>
        public static byte[] RotateImage180(byte[] pngData) {
            using var source = SKBitmap.Decode(pngData);
            if (source == null) return pngData;
            using var rotated = new SKBitmap(source.Info);
            using (var canvas = new SKCanvas(rotated)) {
                canvas.Clear(SKColors.Transparent);
                canvas.Translate(source.Width, source.Height);
                canvas.RotateDegrees(180);
                canvas.DrawBitmap(source, 0, 0);
            }
            using var image = SKImage.FromBitmap(rotated);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        /// <inheritdoc />
        public (double width, double height, string unit, int dpi) GetDimensions()
        {
            return (_labelWidthInput, _labelHeightInput, _labelUnitInput, _labelDpi);
        }
    }
}