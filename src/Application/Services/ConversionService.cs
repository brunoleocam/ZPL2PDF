using System;
using System.Collections.Generic;
using ZPL2PDF.Application.Interfaces;
using ZPL2PDF.Shared;
using ZPL2PDF.Domain.Services;

namespace ZPL2PDF.Application.Services
{
    /// <summary>
    /// Service responsible for converting ZPL content to PDF images
    /// </summary>
    public class ConversionService : IConversionService
    {
        private readonly ZplDimensionExtractor _dimensionExtractor;

        /// <summary>
        /// Initializes a new instance of the ConversionService
        /// </summary>
        public ConversionService()
        {
            _dimensionExtractor = new ZplDimensionExtractor();
        }

        /// <summary>
        /// Converts ZPL content to PDF using explicit dimensions
        /// </summary>
        public List<byte[]> ConvertWithExplicitDimensions(string zplContent, double width, double height, string unit, int dpi)
        {
            if (string.IsNullOrWhiteSpace(zplContent))
                throw new ArgumentException("ZPL content cannot be null or empty", nameof(zplContent));

            // Split labels
            var labels = LabelFileReader.SplitLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            // Create renderer with explicit dimensions
            var renderer = new LabelRenderer(width, height, dpi, unit);
            return renderer.RenderLabels(labels);
        }

        /// <summary>
        /// Converts ZPL content to PDF by extracting dimensions from ZPL
        /// </summary>
        public List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi)
        {
            if (string.IsNullOrWhiteSpace(zplContent))
                throw new ArgumentException("ZPL content cannot be null or empty", nameof(zplContent));

            // Split labels
            var labels = LabelFileReader.SplitLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            // Extract dimensions from each label individually
            var extractedDimensionsList = _dimensionExtractor.ExtractDimensions(zplContent);
            
            // Process each label with its own dimensions
            var allImageData = new List<byte[]>();
            
            for (int i = 0; i < labels.Count; i++)
            {
                var label = labels[i];
                var labelDimensions = i < extractedDimensionsList.Count ? extractedDimensionsList[i] : extractedDimensionsList[0];
                
                // Apply priority logic for this label
                var finalDimensions = _dimensionExtractor.ApplyPriorityLogic(
                    null, 
                    null, 
                    unit, 
                    labelDimensions,
                    dpi
                );
                
                // Create specific renderer for this label
                var labelRenderer = new LabelRenderer(finalDimensions);
                var labelImages = labelRenderer.RenderLabels(new List<string> { label });
                allImageData.AddRange(labelImages);
            }

            return allImageData;
        }

        /// <summary>
        /// Converts ZPL content to PDF using mixed approach (explicit or extracted)
        /// </summary>
        public List<byte[]> Convert(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi)
        {
            // Determine if should use explicit dimensions or extract from ZPL
            bool hasExplicitDimensions = explicitWidth > 0 && explicitHeight > 0;
            
            if (hasExplicitDimensions)
            {
                return ConvertWithExplicitDimensions(zplContent, explicitWidth, explicitHeight, unit, dpi);
            }
            else
            {
                return ConvertWithExtractedDimensions(zplContent, unit, dpi);
            }
        }
    }
}
