using System.Collections.Generic;
using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for processing labels, generating images in memory, and returning image data.
    /// </summary>
    public class LabelRenderer {
        private readonly IPrinterStorage _printerStorage;
        private readonly ZplAnalyzer _analyzer;
        private readonly ZplElementDrawer _drawer;
        private readonly int _labelWidthUnits;
        private readonly int _labelHeightUnits;
        private readonly int _printDensityDpmm;

        /// <summary>
        /// Initializes a new instance of the LabelRenderer class, setting up the necessary dependencies for rendering labels into images.
        /// </summary>
        public LabelRenderer() {
            _printerStorage = new PrinterStorage();
            _analyzer = new ZplAnalyzer(_printerStorage);
            _drawer = new ZplElementDrawer(_printerStorage);
            _labelWidthUnits = 480 / 8;   // 60
            _labelHeightUnits = 960 / 8;  // 120
            _printDensityDpmm = 8;
        }

        /// <summary>
        /// Processes a list of ZPL labels and returns a list of images (in byte[]).
        /// </summary>
        /// <param name="labels">List of ZPL labels.</param>
        /// <returns>List of images in byte arrays.</returns>
        public List<byte[]> RenderLabels(List<string> labels) {
            var images = new List<byte[]>();
            foreach (var labelText in labels) {
                var analyzeInfo = _analyzer.Analyze(labelText);
                foreach (var labelInfo in analyzeInfo.LabelInfos) {
                    byte[] imageData = _drawer.Draw(labelInfo.ZplElements, _labelWidthUnits, _labelHeightUnits, _printDensityDpmm);
                    images.Add(imageData);
                }
            }
            return images;
        }
    }
}