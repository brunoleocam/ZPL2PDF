using System;
using System.IO;
using BinaryKits.Zpl.Viewer;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ZPL2PDF {
    /// <summary>
    /// Main program for converting ZPL to PDF.
    /// 
    /// The program requires the following parameters:
    /// 1. Input file path: Specified with the -i parameter.
    /// 2. Output folder path: Specified with the -o parameter.
    /// 
    /// The output file name can be specified using the -n parameter.
    /// If not specified, the PDF file name will be "ZPL2PDF_DDMMYYYYHHMM.pdf".
    /// Alternatively, the ZPL content can be provided directly using the -z parameter.
    /// </summary>
    class Program {
        static void Main(string[] args) {
            try {
                var argumentProcessor = new ArgumentProcessor();
                argumentProcessor.ProcessArguments(args);

                string fileContent;
                if (!string.IsNullOrEmpty(argumentProcessor.InputFilePath)) {
                    fileContent = LabelFileReader.ReadFile(argumentProcessor.InputFilePath);
                } else {
                    fileContent = argumentProcessor.ZplContent;
                }

                var labels = LabelFileReader.SplitLabels(fileContent);
                var renderer = new LabelRenderer();
                var imageDataList = renderer.RenderLabels(labels);

                // Ensure the output folder exists
                if (!Directory.Exists(argumentProcessor.OutputFolderPath)) {
                    Directory.CreateDirectory(argumentProcessor.OutputFolderPath);
                }

                string outputPdf = Path.Combine(argumentProcessor.OutputFolderPath, argumentProcessor.OutputFileName);
                PdfGenerator.GeneratePdf(imageDataList, outputPdf);
                Console.WriteLine($"PDF successfully generated at {outputPdf}.");
            } catch (Exception ex) {
                Console.Error.WriteLine($"Error during execution: {ex.Message}");
            }
        }
    }
}