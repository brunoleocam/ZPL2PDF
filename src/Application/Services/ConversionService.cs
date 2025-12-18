using System;
using System.Collections.Generic;
using ZPL2PDF.Application.Interfaces;
using ZPL2PDF.Shared;
using ZPL2PDF.Domain.Services;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF.Application.Services
{
    /// <summary>
    /// Service responsible for converting ZPL content to PDF images.
    /// Supports both offline (BinaryKits) and online (Labelary) rendering.
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
        /// <exception cref="ArgumentException">Thrown when zplContent is null or whitespace.</exception>
        private List<string> PrepareLabels(string zplContent)
        {
            if (string.IsNullOrWhiteSpace(zplContent))
                throw new ArgumentException("ZPL content cannot be null or empty", nameof(zplContent));

            // Preprocess ZPL to handle unsupported commands (e.g., ^FN)
            // This removes ^FN tags that BinaryKits.Zpl.Viewer doesn't fully support
            var processedContent = LabelFileReader.PreprocessZpl(zplContent);

            // Split into individual labels
            return LabelFileReader.SplitLabels(processedContent);
        }

        /// <summary>
        /// Converts ZPL content to PDF using explicit dimensions.
        /// </summary>
        public List<byte[]> ConvertWithExplicitDimensions(string zplContent, double width, double height, string unit, int dpi)
        {
            var labels = PrepareLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            // Create renderer with explicit dimensions
            var renderer = new LabelRenderer(width, height, dpi, unit);
            return renderer.RenderLabels(labels);
        }

        /// <summary>
        /// Converts ZPL content to PDF by extracting dimensions from ZPL.
        /// </summary>
        public List<byte[]> ConvertWithExtractedDimensions(string zplContent, string unit, int dpi)
        {
            var labels = PrepareLabels(zplContent);
            if (labels.Count == 0)
                return new List<byte[]>();

            // Extract dimensions from ORIGINAL content (not preprocessed) because:
            // - ^FN removal doesn't affect dimension commands (^PW, ^LL, etc.)
            // - Using original ensures dimension parsing isn't affected by preprocessing
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

        /// <summary>
        /// Converts ZPL content directly to PDF using the specified renderer.
        /// For Labelary renderer, generates vectorial PDF with optimal quality.
        /// For BinaryKits renderer, generates PNG images and converts to PDF.
        /// </summary>
        public void ConvertToPdf(string zplContent, double explicitWidth, double explicitHeight, string unit, int dpi, RendererMode rendererMode, string outputPath)
        {
            var labels = PrepareLabels(zplContent);
            if (labels.Count == 0)
            {
                Console.WriteLine("No labels found in ZPL content.");
                return;
            }

            // Determine dimensions
            double widthMm, heightMm;
            if (explicitWidth > 0 && explicitHeight > 0)
            {
                // Convert to mm based on unit
                (widthMm, heightMm) = ConvertToMm(explicitWidth, explicitHeight, unit);
            }
            else
            {
                // Extract from first label
                var extractedDimensionsList = _dimensionExtractor.ExtractDimensions(zplContent);
                var dimensions = extractedDimensionsList.Count > 0 ? extractedDimensionsList[0] : new LabelDimensions();
                widthMm = dimensions.WidthMm > 0 ? dimensions.WidthMm : 100; // Default 100mm
                heightMm = dimensions.HeightMm > 0 ? dimensions.HeightMm : 150; // Default 150mm
            }

            // Select renderer based on mode
            Infrastructure.Rendering.ILabelRenderer renderer = rendererMode switch
            {
                RendererMode.Labelary => new LabelaryRenderer(),
                RendererMode.Auto => CreateAutoRenderer(),
                _ => new LabelRenderer(widthMm, heightMm, dpi, "mm")
            };

            Console.WriteLine($"Using renderer: {renderer.Name}");

            // Render to PDF
            if (renderer.CanRenderDirectToPdf)
            {
                // Labelary: Generate PDF directly (vectorial output)
                var pdfDataList = renderer.RenderLabelsToPdf(labels, widthMm, heightMm, dpi);
                
                if (pdfDataList.Count == 1)
                {
                    // Single PDF (up to 50 labels)
                    PdfGenerator.SavePdf(pdfDataList[0], outputPath);
                }
                else
                {
                    // Multiple PDFs (more than 50 labels) - merge them
                    PdfGenerator.MergePdfs(pdfDataList, outputPath);
                }
            }
            else
            {
                // BinaryKits: Generate PNG images and convert to PDF
                var imageDataList = renderer.RenderLabels(labels, widthMm, heightMm, dpi);
                PdfGenerator.GeneratePdf(imageDataList, outputPath);
            }
        }

        /// <summary>
        /// Creates an auto renderer that tries Labelary first, falls back to BinaryKits.
        /// </summary>
        private Infrastructure.Rendering.ILabelRenderer CreateAutoRenderer()
        {
            var labelaryRenderer = new LabelaryRenderer();
            if (labelaryRenderer.IsAvailable())
            {
                Console.WriteLine("Labelary API available - using online rendering for best quality.");
                return labelaryRenderer;
            }
            else
            {
                Console.WriteLine("Labelary API not available - falling back to offline rendering.");
                return new LabelRenderer(100, 150, 203, "mm"); // Default dimensions
            }
        }

        /// <summary>
        /// Converts dimensions to millimeters.
        /// </summary>
        private (double widthMm, double heightMm) ConvertToMm(double width, double height, string unit)
        {
            return unit.ToLowerInvariant() switch
            {
                "in" => (width * 25.4, height * 25.4),
                "cm" => (width * 10, height * 10),
                "mm" => (width, height),
                _ => (width, height) // Assume mm if unknown
            };
        }

        /// <summary>
        /// Extracts a custom file name from ZPL content using the ^FX FileName: comment syntax.
        /// This allows users to specify the output PDF file name directly in the ZPL code.
        /// </summary>
        /// <param name="zplContent">ZPL content that may contain a FileName comment.</param>
        /// <returns>The extracted file name (without extension) if found, or null if not found.</returns>
        /// <example>
        /// ZPL content with: ^FX FileName: USPS-Shipping-Label
        /// Returns: "USPS-Shipping-Label"
        /// </example>
        public string? ExtractFileName(string zplContent)
        {
            return LabelFileReader.ExtractFileName(zplContent);
        }

        /// <summary>
        /// Extracts a FORCED file name from ZPL content using the ^FX !FileName: comment syntax.
        /// The ! prefix indicates maximum priority, overriding even the -n parameter.
        /// </summary>
        /// <param name="zplContent">ZPL content that may contain a forced FileName comment.</param>
        /// <returns>The extracted file name (without extension) if found, or null if not found.</returns>
        /// <example>
        /// ZPL content with: ^FX !FileName: MANDATORY-Label
        /// Returns: "MANDATORY-Label" (overrides -n parameter)
        /// </example>
        public string? ExtractForcedFileName(string zplContent)
        {
            return LabelFileReader.ExtractForcedFileName(zplContent);
        }
    }
}
