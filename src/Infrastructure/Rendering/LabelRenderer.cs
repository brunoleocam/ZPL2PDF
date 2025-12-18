using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using SkiaSharp;
using ZPL2PDF.Infrastructure.Rendering;
using ZPL2PDF.Shared;

namespace ZPL2PDF {
    /// <summary>
    /// Offline label renderer using BinaryKits.Zpl.Viewer library.
    /// Responsible for processing ZPL labels and generating images in memory.
    /// </summary>
    public class LabelRenderer : ILabelRenderer {
        private readonly IPrinterStorage _printerStorage;
        private readonly ZplAnalyzer _analyzer;
        private readonly ZplElementDrawer _drawer;
        private readonly double _labelWidthMm;
        private readonly double _labelHeightMm;
        private readonly int _printDpi;

        private const double InchesToMm = 25.4;
        private const double CmToMm = 10.0;
        private const double DpiToDpmm = 25.4;

        /// <summary>
        /// Gets the name of this renderer.
        /// </summary>
        public string Name => "BinaryKits";

        /// <summary>
        /// Indicates that BinaryKits cannot render directly to PDF (generates PNG first).
        /// </summary>
        public bool CanRenderDirectToPdf => false;

        /// <summary>
        /// Initializes a new instance of the LabelRenderer class, setting up the necessary dependencies for rendering labels into images.
        /// </summary>
        /// <param name="labelWidth">Label width</param>
        /// <param name="labelHeight">Label height</param>
        /// <param name="printDpi">Print density in DPI (e.g., 203)</param>
        /// <param name="unit">Unit of measurement (mm, cm, in)</param>
        public LabelRenderer(double labelWidth, double labelHeight, int printDpi, string unit) {
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);

            // Define rendering options with high quality
            var drawerOptions = new DrawerOptions {
                RenderFormat = SKEncodedImageFormat.Png,
                RenderQuality = 100, // Maximum quality
                PdfOutput = false,
                OpaqueBackground = false
            };
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
        /// <param name="dimensions">Label dimensions extracted from ZPL</param>
        public LabelRenderer(LabelDimensions dimensions) {
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);

            // Define rendering options with high quality
            var drawerOptions = new DrawerOptions {
                RenderFormat = SKEncodedImageFormat.Png,
                RenderQuality = 100, // Maximum quality
                PdfOutput = false,
                OpaqueBackground = false
            };
            _drawer = new ZplElementDrawer(_printerStorage, drawerOptions);

            // Use dimensions extracted from ZPL
            _labelWidthMm = dimensions.WidthMm;
            _labelHeightMm = dimensions.HeightMm;
            _printDpi = dimensions.Dpi;  // Store DPI
        }

        /// <summary>
        /// Checks if the renderer is available. BinaryKits is always available (offline).
        /// </summary>
        public bool IsAvailable() => true;

        /// <summary>
        /// Processes a list of ZPL labels and returns a list of images (in byte[]).
        /// Uses internally stored dimensions.
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <returns>List of images in byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels) {
            return RenderLabels(labels, _labelWidthMm, _labelHeightMm, _printDpi);
        }

        /// <summary>
        /// Processes a list of ZPL labels and returns a list of images (in byte[]).
        /// Implements ILabelRenderer interface.
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List of images in byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels, double widthMm, double heightMm, int dpi) {
            var images = new List<byte[]>();
            
            // Convert DPI to DPMM for the drawer
            int dpmm = (int)Math.Round(dpi / DpiToDpmm);
            
            for (int i = 0; i < labels.Count; i++) {
                var labelText = labels[i];
                var analyzeInfo = _analyzer.Analyze(labelText);
                foreach (var labelInfo in analyzeInfo.LabelInfos) {
                    // Use DPMM for Draw (BinaryKits library expects DPMM)
                    byte[] imageData = _drawer.Draw(labelInfo.ZplElements, widthMm, heightMm, dpmm);
                    images.Add(imageData);
                }
            }
            return images;
        }

        /// <summary>
        /// Renders ZPL labels to images asynchronously.
        /// For BinaryKits, this just wraps the synchronous method.
        /// </summary>
        public Task<List<byte[]>> RenderLabelsAsync(List<string> labels, double widthMm, double heightMm, int dpi) {
            return Task.FromResult(RenderLabels(labels, widthMm, heightMm, dpi));
        }

        /// <summary>
        /// Renders ZPL labels to PDF format.
        /// For BinaryKits, this generates PNG images and converts them to PDF using PdfGenerator.
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List containing a single PDF byte array with all labels.</returns>
        public List<byte[]> RenderLabelsToPdf(List<string> labels, double widthMm, double heightMm, int dpi) {
            // BinaryKits generates PNG images, then we convert to PDF
            var images = RenderLabels(labels, widthMm, heightMm, dpi);
            
            // Generate PDF from images in memory
            var pdfBytes = PdfGenerator.GeneratePdfToMemory(images);
            
            return new List<byte[]> { pdfBytes };
        }

        /// <summary>
        /// Saves the image data to a file.
        /// </summary>
        /// <param name="imageData">Image data in byte array.</param>
        /// <param name="filePath">Path to save the image file.</param>
        private void SaveImageToFile(byte[] imageData, string filePath) {
            File.WriteAllBytes(filePath, imageData);
        }
    }
}
