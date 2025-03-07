using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly int _printDensityDpmm;

        private const double InchesToMm = 25.4;
        private const double CmToMm = 10.0;

        /// <summary>
        /// Initializes a new instance of the LabelRenderer class, setting up the necessary dependencies for rendering labels into images.
        /// </summary>
        public LabelRenderer(double labelWidth, double labelHeight, int printDensityDpmm, string unit) {
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);

            // Define as opções de renderização com alta qualidade
            var drawerOptions = new DrawerOptions {
                RenderFormat = SKEncodedImageFormat.Png,
                RenderQuality = 100, // Qualidade máxima
                PdfOutput = false,
                OpaqueBackground = false
            };
            _drawer = new ZplElementDrawer(_printerStorage, drawerOptions);

            // Debugging: Print the input values
            //Console.WriteLine($"labelWidth: {labelWidth}, labelHeight: {labelHeight}, printDensityDpmm: {printDensityDpmm}, unit: {unit}");

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
                    _printDensityDpmm = 8;
                    break;
            }

            // Multiplicar a densidade de impressão por 2,25 para aumentar a resolução
            //Console.WriteLine($"_printDensityDpmm A: {_printDensityDpmm}, printDensityDpmm A: {printDensityDpmm}");
            
            _printDensityDpmm = printDensityDpmm;

            //_printDensityDpmm = (int)(printDensityDpmm * 2.25);
            //Console.WriteLine($"_printDensityDpmm D: {_printDensityDpmm}, printDensityDpmm D: {printDensityDpmm}");

            // Debugging: Print the calculated values
            //Console.WriteLine($"_labelWidthMm: {_labelWidthMm}, _labelHeightMm: {_labelHeightMm}, _printDensityDpmm: {_printDensityDpmm}");
        }

        /// <summary>
        /// Processes a list of ZPL labels and returns a list of images (in byte[]).
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <returns>List of images in byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels) {
            var images = new List<byte[]>();
            for (int i = 0; i < labels.Count; i++) {
                var labelText = labels[i];
                var analyzeInfo = _analyzer.Analyze(labelText);
                foreach (var labelInfo in analyzeInfo.LabelInfos) {
                    // Convert double to int safely
                    int widthUnits = (int)Math.Round(_labelWidthMm * _printDensityDpmm);
                    int heightUnits = (int)Math.Round(_labelHeightMm * _printDensityDpmm);
                    //Console.WriteLine($"widthUnits: {widthUnits}, heightUnits: {heightUnits}, _printDensityDpmm: {_printDensityDpmm}");
                    byte[] imageData = _drawer.Draw(labelInfo.ZplElements, _labelWidthMm, _labelHeightMm, _printDensityDpmm);
                    images.Add(imageData);

                    // Save the image to a file for testing
                    //SaveImageToFile(imageData, Path.Combine(@"C:\Dev", $"Imagem_{i + 1}.png"));
                }
            }
            return images;
        }

        /// <summary>
        /// Saves the image data to a file.
        /// </summary>
        /// <param name="imageData">Image data in byte array.</param>
        /// <param name="filePath">Path to save the image file.</param>
        private void SaveImageToFile(byte[] imageData, string filePath) {
            File.WriteAllBytes(filePath, imageData);
            //Console.WriteLine($"Image saved to {filePath}");
        }
    }
}