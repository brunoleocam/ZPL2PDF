using System;
using System.Collections.Generic;
using ZPL2PDF.Domain.Services;

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
            _offlineRenderer = new LabelRenderer(labelWidth, labelHeight, dpi, unit, fontsDirectory, fontMappings);
        }

        public AutoRenderer(
            LabelDimensions dimensions,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            _labelaryRenderer = new LabelaryRenderer(dimensions);
            _offlineRenderer = new LabelRenderer(dimensions, fontsDirectory, fontMappings);
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

