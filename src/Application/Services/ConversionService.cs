using System;
using System.Collections.Generic;
using ZPL2PDF.Application.Interfaces;
using ZPL2PDF.Shared;
using ZPL2PDF.Domain.Services;
using ZPL2PDF.Shared.Constants;

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
            IReadOnlyList<(string Id, string Path)>? fontMappings = null,
            RendererEngine rendererEngine = RendererEngine.Offline)
        {
            var labels = PrepareLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            return rendererEngine switch
            {
                RendererEngine.Labelary => new LabelaryRenderer(width, height, unit, dpi).RenderLabels(labels),
                RendererEngine.Auto => new AutoRenderer(width, height, unit, dpi, fontsDirectory, fontMappings).RenderLabels(labels),
                _ => new LabelRenderer(width, height, dpi, unit, fontsDirectory, fontMappings).RenderLabels(labels)
            };
        }

        /// <summary>
        /// Converts ZPL content to PDF by extracting dimensions from ZPL.
        /// </summary>
        public List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null,
            RendererEngine rendererEngine = RendererEngine.Offline)
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
                var labelImages = rendererEngine switch
                {
                    RendererEngine.Labelary => new LabelaryRenderer(finalDimensions).RenderLabels(new List<string> { label }),
                    RendererEngine.Auto => new AutoRenderer(finalDimensions, fontsDirectory, fontMappings).RenderLabels(new List<string> { label }),
                    _ => new LabelRenderer(finalDimensions, fontsDirectory, fontMappings).RenderLabels(new List<string> { label })
                };
                allImageData.AddRange(labelImages);
            }

            return allImageData;
        }

        /// <summary>
        /// Converts ZPL content to PDF using mixed approach (explicit or extracted)
        /// </summary>
        public List<byte[]> Convert(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null,
            RendererEngine rendererEngine = RendererEngine.Offline)
        {
            bool hasExplicitDimensions = explicitWidth > 0 && explicitHeight > 0;
            if (hasExplicitDimensions)
                return ConvertWithExplicitDimensions(zplContent, explicitWidth, explicitHeight, unit, dpi, fontsDirectory, fontMappings, rendererEngine);
            return ConvertWithExtractedDimensions(zplContent, unit, dpi, fontsDirectory, fontMappings, rendererEngine);
        }

        /// <summary>
        /// Converts the entire ZPL template directly to a PDF using the Labelary API
        /// (Labelary returns <c>application/pdf</c> when requested via Accept header).
        /// </summary>
        public byte[] ConvertPdfDirectWithLabelary(
            string zplContent,
            double explicitWidth,
            double explicitHeight,
            string unit,
            int dpi,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            // Labelary handles label splitting internally for PDF requests (index omitted).
            // We still preprocess for compatibility with BinaryKits workarounds (e.g. ^B0 -> ^BO).
            var processedContent = LabelFileReader.PreprocessZpl(zplContent);

            if (explicitWidth > 0 && explicitHeight > 0)
            {
                return new LabelaryRenderer(explicitWidth, explicitHeight, unit, dpi).RenderPdf(processedContent);
            }

            // Otherwise, extract dimensions and use the first label as a best-effort size for Labelary.
            var extractedDimensionsList = _dimensionExtractor.ExtractDimensions(zplContent);
            var firstExtracted = extractedDimensionsList.Count > 0
                ? extractedDimensionsList[0]
                : _dimensionExtractor.GetDefaultDimensions();

            var finalDimensions = _dimensionExtractor.ApplyPriorityLogic(
                explicitWidth: null,
                explicitHeight: null,
                explicitUnit: unit,
                zplDimensions: firstExtracted,
                dpi: dpi);

            // Labelary width/height are expressed in inches; we pass mm and let LabelaryRenderer convert.
            return new LabelaryRenderer(finalDimensions.WidthMm, finalDimensions.HeightMm, "mm", dpi)
                .RenderPdf(processedContent);
        }

        public bool TryConvertPdfDirectWithLabelary(
            string zplContent,
            double explicitWidth,
            double explicitHeight,
            string unit,
            int dpi,
            RendererEngine rendererEngine,
            out byte[]? pdfBytes,
            string? fontsDirectory = null,
            IReadOnlyList<(string Id, string Path)>? fontMappings = null)
        {
            pdfBytes = null;
            if (rendererEngine != RendererEngine.Labelary && rendererEngine != RendererEngine.Auto)
            {
                return false;
            }

            try
            {
                pdfBytes = ConvertPdfDirectWithLabelary(
                    zplContent,
                    explicitWidth,
                    explicitHeight,
                    unit,
                    dpi,
                    fontsDirectory,
                    fontMappings);
                return true;
            }
            catch
            {
                if (rendererEngine == RendererEngine.Labelary)
                {
                    throw;
                }

                return false;
            }
        }

    }
}
