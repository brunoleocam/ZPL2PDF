using System;
using System.Collections.Generic;
using ZPL2PDF.Application.Interfaces;
using ZPL2PDF.Shared;
using ZPL2PDF.Domain.Services;

namespace ZPL2PDF.Application.Services
{
    /// <summary>
    /// Service responsible for converting ZPL content to PDF images.
    /// </summary>
    public class ConversionService : IConversionService
    {
        private readonly ZplDimensionExtractor _dimensionExtractor;

        /// <summary>
        /// Initializes a new instance of the ConversionService.
        /// </summary>
        public ConversionService()
        {
            _dimensionExtractor = new ZplDimensionExtractor();
        }

        /// <summary>
        /// Preprocesses and splits ZPL content into individual labels.
        /// </summary>
        /// <param name="zplContent">Raw ZPL content.</param>
        /// <returns>List of preprocessed labels, or empty list if no valid labels found.</returns>
        private List<string> PrepareLabels(string zplContent)
        {
            if (string.IsNullOrWhiteSpace(zplContent))
                return new List<string>();

            // Preprocess ZPL to handle unsupported commands (e.g., ^FN)
            // This removes ^FN tags that BinaryKits.Zpl.Viewer doesn't fully support
            var processedContent = LabelFileReader.PreprocessZpl(zplContent);

            // Split into individual labels
            return LabelFileReader.SplitLabels(processedContent);
        }

        /// <summary>
        /// Converts ZPL content to PDF using explicit dimensions.
        /// </summary>
        public List<byte[]> ConvertWithExplicitDimensions(string zplContent, double width, double height, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            var labels = PrepareLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            var renderer = new LabelRenderer(width, height, dpi, unit, fontsDirectory, fontMappings);
            return renderer.RenderLabels(labels);
        }

        /// <summary>
        /// Converts ZPL content to PDF by extracting dimensions from ZPL.
        /// </summary>
        public List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            var labels = PrepareLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            var extractedDimensionsList = _dimensionExtractor.ExtractDimensions(zplContent);
            var allImageData = new List<byte[]>();

            for (int i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                var labelDimensions = i < extractedDimensionsList.Count ? extractedDimensionsList[i] : extractedDimensionsList[0];
                var finalDimensions = _dimensionExtractor.ApplyPriorityLogic(null, null, unit, labelDimensions, dpi);
                var labelRenderer = new LabelRenderer(finalDimensions, fontsDirectory, fontMappings);
                var labelImages = labelRenderer.RenderLabels(new List<string> { label });
                allImageData.AddRange(labelImages);
            }

            return allImageData;
        }

        /// <summary>
        /// Converts ZPL content to PDF using mixed approach (explicit or extracted)
        /// </summary>
        public List<byte[]> Convert(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            bool hasExplicitDimensions = explicitWidth > 0 && explicitHeight > 0;
            if (hasExplicitDimensions)
                return ConvertWithExplicitDimensions(zplContent, explicitWidth, explicitHeight, unit, dpi, fontsDirectory, fontMappings);
            return ConvertWithExtractedDimensions(zplContent, unit, dpi, fontsDirectory, fontMappings);
        }
    }
}
