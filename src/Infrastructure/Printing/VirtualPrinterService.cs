using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZPL2PDF.Application.Services;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF.Infrastructure.Printing
{
    /// <summary>
    /// Service that processes print jobs from the virtual printer.
    /// Receives ZPL data, converts to PDF, and opens in the default viewer.
    /// </summary>
    public class VirtualPrinterService
    {
        private readonly ConversionService _conversionService;
        private readonly RendererMode _rendererMode;
        private readonly double _widthMm;
        private readonly double _heightMm;
        private readonly int _dpi;
        private readonly string _outputDirectory;
        private readonly bool _openPdfAfterGeneration;

        /// <summary>
        /// Default output directory for generated PDFs.
        /// </summary>
        public static string DefaultOutputDirectory => Path.Combine(Path.GetTempPath(), "ZPL2PDF");

        /// <summary>
        /// Initializes a new instance of the VirtualPrinterService.
        /// </summary>
        /// <param name="rendererMode">Renderer mode to use.</param>
        /// <param name="widthMm">Default label width in mm.</param>
        /// <param name="heightMm">Default label height in mm.</param>
        /// <param name="dpi">Default DPI.</param>
        /// <param name="outputDirectory">Output directory for PDFs (null for temp folder).</param>
        /// <param name="openPdfAfterGeneration">Whether to open PDF after generation.</param>
        public VirtualPrinterService(
            RendererMode rendererMode = RendererMode.Offline,
            double widthMm = 100,
            double heightMm = 150,
            int dpi = 203,
            string? outputDirectory = null,
            bool openPdfAfterGeneration = true)
        {
            _conversionService = new ConversionService();
            _rendererMode = rendererMode;
            _widthMm = widthMm;
            _heightMm = heightMm;
            _dpi = dpi;
            _outputDirectory = outputDirectory ?? DefaultOutputDirectory;
            _openPdfAfterGeneration = openPdfAfterGeneration;

            // Ensure output directory exists
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }
        }

        /// <summary>
        /// Processes a print job from stdin.
        /// This is called by the port monitor/CUPS backend.
        /// </summary>
        /// <returns>Exit code (0 for success).</returns>
        public async Task<int> ProcessPrintJobFromStdinAsync()
        {
            try
            {
                Console.Error.WriteLine("[ZPL2PDF] Processing print job from stdin...");

                // Read ZPL content from stdin
                string zplContent;
                using (var stdin = Console.OpenStandardInput())
                using (var reader = new StreamReader(stdin, Encoding.UTF8))
                {
                    zplContent = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrWhiteSpace(zplContent))
                {
                    Console.Error.WriteLine("[ZPL2PDF] Error: No ZPL content received.");
                    return 1;
                }

                Console.Error.WriteLine($"[ZPL2PDF] Received {zplContent.Length} bytes of ZPL data.");

                return await ProcessZplContentAsync(zplContent);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ZPL2PDF] Error processing print job: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Processes ZPL content and generates a PDF.
        /// </summary>
        /// <param name="zplContent">ZPL content to convert.</param>
        /// <returns>Exit code (0 for success).</returns>
        public async Task<int> ProcessZplContentAsync(string zplContent)
        {
            try
            {
                // Generate unique filename
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var pdfFileName = $"ZPL2PDF_{timestamp}.pdf";
                var pdfPath = Path.Combine(_outputDirectory, pdfFileName);

                // Try to extract custom filename from ZPL
                var customFileName = _conversionService.ExtractForcedFileName(zplContent) 
                    ?? _conversionService.ExtractFileName(zplContent);
                
                if (!string.IsNullOrEmpty(customFileName))
                {
                    pdfFileName = Path.ChangeExtension(customFileName, ".pdf");
                    pdfPath = Path.Combine(_outputDirectory, pdfFileName);
                }

                Console.Error.WriteLine($"[ZPL2PDF] Converting to PDF: {pdfPath}");

                // Convert ZPL to PDF using the ConvertToPdf method
                _conversionService.ConvertToPdf(
                    zplContent,
                    _widthMm,
                    _heightMm,
                    "mm",
                    _dpi,
                    _rendererMode,
                    pdfPath);

                Console.Error.WriteLine($"[ZPL2PDF] PDF generated successfully: {pdfPath}");

                // Open PDF in default viewer
                if (_openPdfAfterGeneration)
                {
                    OpenPdf(pdfPath);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ZPL2PDF] Error converting ZPL to PDF: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Opens a PDF file in the default viewer.
        /// </summary>
        /// <param name="pdfPath">Path to the PDF file.</param>
        private void OpenPdf(string pdfPath)
        {
            try
            {
                Console.Error.WriteLine($"[ZPL2PDF] Opening PDF: {pdfPath}");

                ProcessStartInfo psi;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = pdfPath,
                        UseShellExecute = true
                    };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "open",
                        Arguments = $"\"{pdfPath}\"",
                        UseShellExecute = false
                    };
                }
                else // Linux
                {
                    psi = new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = $"\"{pdfPath}\"",
                        UseShellExecute = false
                    };
                }

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ZPL2PDF] Warning: Could not open PDF automatically: {ex.Message}");
                Console.Error.WriteLine($"[ZPL2PDF] PDF saved to: {pdfPath}");
            }
        }
    }
}

