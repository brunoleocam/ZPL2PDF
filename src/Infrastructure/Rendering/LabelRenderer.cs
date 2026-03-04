using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using SkiaSharp;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for processing labels, generating images in memory, and returning image data.
    /// </summary>
    public class LabelRenderer {
        private readonly IPrinterStorage _printerStorage;
        private readonly ZplAnalyzer _analyzer;
        private readonly ZplElementDrawer _drawer;
        private readonly double _labelWidthMm;
        private readonly double _labelHeightMm;
        private readonly int _printDpi;
        private readonly string? _fontsDirectory;
        private readonly IReadOnlyList<(string Id, string Path)>? _fontMappings;

        private const double InchesToMm = 25.4;
        private const double CmToMm = 10.0;
        private const double DpiToDpmm = 25.4;

        /// <summary>
        /// Creates DrawerOptions with high quality settings. Optional custom font loader for ^A0N, ^AAN, ^ABN, etc.
        /// </summary>
        private DrawerOptions CreateDrawerOptions() {
            var options = new DrawerOptions {
                RenderFormat = SKEncodedImageFormat.Png,
                RenderQuality = 100,
                PdfOutput = false,
                OpaqueBackground = false
            };
            if (_fontsDirectory != null || (_fontMappings != null && _fontMappings.Count > 0)) {
                options.FontLoader = CreateFontLoader();
            }
            return options;
        }

        /// <summary>
        /// Builds a delegate that resolves ZPL font ID (0, A, B, ...) to SKTypeface from --fonts-dir and --font mappings.
        /// </summary>
        private Func<string, SKTypeface?> CreateFontLoader() {
            var fontsDir = _fontsDirectory ?? string.Empty;
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (_fontMappings != null) {
                foreach (var (id, path) in _fontMappings) {
                    if (string.IsNullOrEmpty(id)) continue;
                    var resolved = path;
                    if (!Path.IsPathRooted(resolved) && !string.IsNullOrWhiteSpace(fontsDir)) {
                        resolved = Path.Combine(fontsDir, Path.GetFileName(resolved));
                    }
                    mappings[id.Trim()] = resolved;
                }
            }
            return fontId => {
                if (string.IsNullOrEmpty(fontId)) return SKTypeface.Default;
                var key = fontId.Trim();
                if (mappings.TryGetValue(key, out var path) && File.Exists(path)) {
                    try { return SKTypeface.FromFile(path) ?? SKTypeface.Default; } catch { /* fallback */ }
                }
                if (!string.IsNullOrWhiteSpace(fontsDir)) {
                    var byName = Path.Combine(fontsDir, key + ".ttf");
                    if (File.Exists(byName)) {
                        try { return SKTypeface.FromFile(byName) ?? SKTypeface.Default; } catch { }
                    }
                    var byNameOtf = Path.Combine(fontsDir, key + ".otf");
                    if (File.Exists(byNameOtf)) {
                        try { return SKTypeface.FromFile(byNameOtf) ?? SKTypeface.Default; } catch { }
                    }
                }
                return SKTypeface.Default;
            };
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
            int dpmm = (int)Math.Round(_printDpi / DpiToDpmm);
            
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
                        images.Add(imageData);
                    }
                }
            }
            return images;
        }
    }
}