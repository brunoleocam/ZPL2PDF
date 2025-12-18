using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;
using SkiaSharp;

namespace ZPL2PDF.Infrastructure.Rendering
{
    /// <summary>
    /// Offline label renderer using BinaryKits.Zpl.Viewer library.
    /// Provides local rendering without internet dependency.
    /// </summary>
    public class BinaryKitsRenderer : ILabelRenderer
    {
        private readonly IPrinterStorage _printerStorage;
        private readonly ZplAnalyzer _analyzer;
        private readonly DrawerOptions _drawerOptions;

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
        /// Initializes a new instance of the BinaryKitsRenderer.
        /// </summary>
        public BinaryKitsRenderer()
        {
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);

            // Define rendering options with high quality
            _drawerOptions = new DrawerOptions
            {
                RenderFormat = SKEncodedImageFormat.Png,
                RenderQuality = 100, // Maximum quality
                PdfOutput = false,
                OpaqueBackground = false
            };
        }

        /// <summary>
        /// Checks if the renderer is available. BinaryKits is always available (offline).
        /// </summary>
        /// <returns>Always returns true as this is an offline renderer.</returns>
        public bool IsAvailable()
        {
            return true;
        }

        /// <summary>
        /// Renders ZPL labels to images synchronously.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List of rendered images as byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            var images = new List<byte[]>();
            var drawer = new ZplElementDrawer(_printerStorage, _drawerOptions);

            // Convert DPI to DPMM for the drawer
            int dpmm = (int)Math.Round(dpi / DpiToDpmm);

            foreach (var labelText in labels)
            {
                var analyzeInfo = _analyzer.Analyze(labelText);
                foreach (var labelInfo in analyzeInfo.LabelInfos)
                {
                    // Use DPMM for Draw (BinaryKits library expects DPMM)
                    byte[] imageData = drawer.Draw(labelInfo.ZplElements, widthMm, heightMm, dpmm);
                    images.Add(imageData);
                }
            }

            return images;
        }

        /// <summary>
        /// Renders ZPL labels to images asynchronously.
        /// For BinaryKits, this simply wraps the synchronous method.
        /// </summary>
        /// <param name="labels">List of ZPL label strings to render.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>Task containing list of rendered images as byte arrays.</returns>
        public Task<List<byte[]>> RenderLabelsAsync(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            // BinaryKits is synchronous, so we just wrap it in a Task
            return Task.FromResult(RenderLabels(labels, widthMm, heightMm, dpi));
        }

        /// <summary>
        /// Renders ZPL labels to PDF format.
        /// For BinaryKits, this generates PNG images and converts them to PDF.
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <param name="widthMm">Label width in millimeters.</param>
        /// <param name="heightMm">Label height in millimeters.</param>
        /// <param name="dpi">Print density in DPI.</param>
        /// <returns>List containing a single PDF byte array with all labels.</returns>
        public List<byte[]> RenderLabelsToPdf(List<string> labels, double widthMm, double heightMm, int dpi)
        {
            // BinaryKits generates PNG images, then we convert to PDF
            var images = RenderLabels(labels, widthMm, heightMm, dpi);
            
            // Generate PDF from images in memory
            var pdfBytes = PdfGenerator.GeneratePdfToMemory(images);
            
            return new List<byte[]> { pdfBytes };
        }
    }
}

