using System;
using System.Collections.Generic;
using ZPL2PDF.Domain.Services;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF
{
    /// <summary>
    /// Renderer that tries Labelary first and falls back to BinaryKits offline rendering per label.
    /// </summary>
    public class AutoRenderer : ILabelRenderer
    {
        private readonly LabelaryRenderer _labelaryRenderer;
        private readonly LabelRenderer _offlineRenderer;

        public AutoRenderer(
            double labelWidth,
            double labelHeight,
            string unit,
            int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            _labelaryRenderer = new LabelaryRenderer(labelWidth, labelHeight, unit, dpi);
            // Align the offline fallback with Labelary's dpmm so both produce the same pixel scale.
            _offlineRenderer = new LabelRenderer(labelWidth, labelHeight,
                RendererEngineParser.ToEffectiveDpi(dpi, RendererEngine.Auto), unit, fontsDirectory, fontMappings);
        }

        public AutoRenderer(
            LabelDimensions dimensions,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            _labelaryRenderer = new LabelaryRenderer(dimensions);
            _offlineRenderer = new LabelRenderer(new LabelDimensions
            {
                Width = dimensions.Width,
                Height = dimensions.Height,
                WidthMm = dimensions.WidthMm,
                HeightMm = dimensions.HeightMm,
                Dpi = RendererEngineParser.ToEffectiveDpi(dimensions.Dpi, RendererEngine.Auto)
            }, fontsDirectory, fontMappings);
        }

        public (double width, double height, string unit, int dpi) GetDimensions()
            => _offlineRenderer.GetDimensions();

        public List<byte[]> RenderLabels(List<string> labels)
        {
            if (labels == null) throw new ArgumentNullException(nameof(labels));

            var images = new List<byte[]>();
            foreach (var label in labels)
            {
                images.Add(RenderSingleLabel(label));
            }
            return images;
        }

        private byte[] RenderSingleLabel(string zpl)
        {
            try
            {
                var res = _labelaryRenderer.RenderLabels(new List<string> { zpl });
                return res.Count > 0 ? res[0] : Array.Empty<byte>();
            }
            catch
            {
                var res = _offlineRenderer.RenderLabels(new List<string> { zpl });
                return res.Count > 0 ? res[0] : Array.Empty<byte>();
            }
        }
    }
}

