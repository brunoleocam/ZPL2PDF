using System;
using System.Collections.Generic;
using System.IO;
using ZPL2PDF.Shared;
using ZPL2PDF.Domain.Services;
using ZPL2PDF.Application.Interfaces;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF
{
    /// <summary>
    /// Handles conversion mode operations
    /// </summary>
    public class ConversionModeHandler
    {
        private readonly IConversionService _conversionService;
        private readonly IPathService _pathService;

        /// <summary>
        /// Initializes a new instance of the ConversionModeHandler
        /// </summary>
        public ConversionModeHandler()
        {
            _conversionService = new ConversionService();
            _pathService = new PathService();
        }

        /// <summary>
        /// Handles conversion mode operations
        /// </summary>
        /// <param name="argumentProcessor">The argument processor with conversion configuration</param>
        public void HandleConversion(ArgumentProcessor argumentProcessor)
        {
            try
            {
                // Read file content
                string fileContent;
                if (!string.IsNullOrEmpty(argumentProcessor.InputFilePath))
                {
                    fileContent = LabelFileReader.ReadFile(argumentProcessor.InputFilePath);
                }
                else
                {
                    fileContent = argumentProcessor.ZplContent;
                }

                // Convert using the conversion service
                var imageDataList = _conversionService.Convert(
                    fileContent,
                    argumentProcessor.Width,
                    argumentProcessor.Height,
                    argumentProcessor.Unit,
                    argumentProcessor.Dpi
                );

                // Process the generated images
                ProcessImages(imageDataList, argumentProcessor);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found - {ex.Message}");
                throw;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: Invalid argument - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during conversion: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Processes the generated images and creates PDF
        /// </summary>
        /// <param name="imageDataList">List of image data</param>
        /// <param name="argumentProcessor">Argument processor with configuration</param>
        private void ProcessImages(List<byte[]> imageDataList, ArgumentProcessor argumentProcessor)
        {
            if (imageDataList == null || imageDataList.Count == 0)
            {
                Console.WriteLine("No images generated from ZPL content!");
                return;
            }

            // Ensure the output folder exists
            _pathService.EnsureDirectoryExists(argumentProcessor.OutputFolderPath);

            string outputPdf = Path.Combine(argumentProcessor.OutputFolderPath, argumentProcessor.OutputFileName);
            PdfGenerator.GeneratePdf(imageDataList, outputPdf);

            Console.WriteLine($"PDF generated successfully: {outputPdf}");
            Console.WriteLine($"   Images processed: {imageDataList.Count}");
        }
    }
}
