using System.Collections.Generic;
using BinaryKits.Zpl.Label;
using BinaryKits.Zpl.Viewer;
using BinaryKits.Zpl.Viewer.ElementDrawers;

namespace ZPL2PDF {
    /// <summary>
    /// Responsável por processar as etiquetas, gerar as imagens na memória e retornar os dados de imagem.
    /// </summary>
    public class LabelRenderer {
        private readonly IPrinterStorage _printerStorage;
        private readonly ZplAnalyzer _analyzer;
        private readonly ZplElementDrawer _drawer;
        private readonly int _labelWidthUnits;
        private readonly int _labelHeightUnits;
        private readonly int _printDensityDpmm;

        /// <summary>
        /// Inicializa uma nova instância da classe LabelRenderer, configurando as dependências necessárias para a renderização de etiquetas em imagens.
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
        /// Processa uma lista de etiquetas ZPL e retorna uma lista de imagens (em byte[]).
        /// </summary>
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