using System;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ZPL2PDF.Shared.Localization;
using ZPL2PDF.Shared.Constants;
using ZPL2PDF.Presentation.Api.Models;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF
{
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
    /// 
    /// Optional parameters:
    /// -w: Width of the label.
    /// -h: Height of the label.
    /// -d: Print density in dots per millimeter.
    /// -u: Unit of measurement for width and height ("in", "cm", "mm").
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // Check for language management commands first (before loading config)
                if (args.Length > 0)
                {
                    // --set-language <code>: Set language permanently
                    if (args[0] == "--set-language" && args.Length > 1)
                    {
                        // Initialize with English first to show messages
                        LocalizationManager.Initialize("en-US");
                        LanguageConfigManager.SetLanguagePermanently(args[1]);
                        return;
                    }
                    
                    // --reset-language: Reset to system default
                    if (args[0] == "--reset-language")
                    {
                        // Initialize with English first to show messages
                        LocalizationManager.Initialize("en-US");
                        LanguageConfigManager.ResetLanguage();
                        return;
                    }
                    
                    // --show-language: Show current configuration
                    if (args[0] == "--show-language")
                    {
                        // Initialize with current detection to show proper messages
                        LocalizationManager.Initialize();
                        LanguageConfigManager.ShowLanguageConfiguration();
                        return;
                    }
                }

                // Load configuration to get language setting
                var configManager = new ConfigManager();
                var config = configManager.Config;

                // Check for language override parameter (highest priority)
                string? languageOverride = null;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--language" && i + 1 < args.Length)
                    {
                        languageOverride = args[i + 1];
                        break;
                    }
                }

                // Initialize localization system with priority:
                // 1. --language parameter (temporary override)
                // 2. Environment variable ZPL2PDF_LANGUAGE
                // 3. Configuration file language setting
                // 4. System language detection
                if (!string.IsNullOrEmpty(languageOverride))
                {
                    LocalizationManager.Initialize(languageOverride);
                }
                else
                {
                    LocalizationManager.InitializeWithConfig(config.Language);
                }

                // Check for API mode
                if (args.Length > 0 && (args[0] == "--api" || args[0] == "--web"))
                {
                    await StartApiMode(args);
                    return;
                }

                var argumentProcessor = new ArgumentProcessor();
                argumentProcessor.ProcessArguments(args);

                if (argumentProcessor.Mode == OperationMode.Help)
                {
                    if (argumentProcessor.ExitCode != 0)
                    {
                        Environment.Exit(argumentProcessor.ExitCode);
                    }
                    return;
                }

                // Route to appropriate mode
                if (argumentProcessor.Mode == OperationMode.Daemon)
                {
                    var daemonHandler = new DaemonModeHandler();
                    await daemonHandler.HandleDaemon(argumentProcessor);
                }
                else if (argumentProcessor.Mode == OperationMode.Server)
                {
                    var tcpServerHandler = new TcpServerModeHandler();
                    tcpServerHandler.HandleServer(argumentProcessor);
                }
                else
                {
                    var conversionHandler = new ConversionModeHandler();
                    conversionHandler.HandleConversion(argumentProcessor);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(LocalizationManager.GetString(ResourceKeys.CONVERSION_ERROR, ex.Message));
            }
        }

        /// <summary>
        /// Starts the API mode with Minimal API endpoints
        /// </summary>
        static async Task StartApiMode(string[] args)
        {
            // Parse host and port from arguments
            string host = "0.0.0.0";  // Default: listen on all interfaces (Docker-friendly)
            int port = 5000;
            
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--host" && i + 1 < args.Length)
                {
                    host = args[i + 1];
                }
                else if (args[i] == "--port" && i + 1 < args.Length && int.TryParse(args[i + 1], out int parsedPort))
                {
                    port = parsedPort;
                }
            }

            var builder = WebApplication.CreateBuilder(args);
            
            // Add CORS services
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Enable CORS
            app.UseCors();

            // Convert endpoint
            app.MapPost("/api/convert", async (HttpContext context) =>
            {
                try
                {
                    // Read and deserialize request
                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    var request = JsonSerializer.Deserialize<ConvertRequest>(requestBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Validate request
                    if (request == null || 
                        (string.IsNullOrWhiteSpace(request.Zpl) && 
                         (request.ZplArray == null || request.ZplArray.Count == 0 || request.ZplArray.All(string.IsNullOrWhiteSpace))))
                    {
                        var errorResponse = new ConvertResponse
                        {
                            Success = false,
                            Message = "ZPL content is required (use 'zpl' or 'zplArray' field)"
                        };
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(errorResponse);
                        return;
                    }

                    // Validate format
                    var format = (request.Format ?? "pdf").ToLowerInvariant();
                    if (format != "pdf" && format != "png")
                    {
                        var errorResponse = new ConvertResponse
                        {
                            Success = false,
                            Message = "Format must be 'pdf' or 'png'"
                        };
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(errorResponse);
                        return;
                    }

                    // Prepare conversion parameters
                    var width = request.Width ?? 0;
                    var height = request.Height ?? 0;
                    var unit = request.Unit ?? "mm";
                    var dpi = request.Dpi ?? 203;

                    // Renderer selection ("offline" | "labelary" | "auto")
                    if (!TryGetRendererEngine(request.Renderer, out var rendererEngine))
                    {
                        await WriteBadRequest(context, "Renderer must be 'offline', 'labelary', or 'auto'");
                        return;
                    }

                    // Convert ZPL(s)
                    var conversionService = new ConversionService();

                    // When format=pdf, try direct PDF via Labelary according to renderer policy.
                    // In auto mode, failures fall back to PNG pipeline.
                    if (format == "pdf" && TryConvertDirectPdfResponse(request, conversionService, width, height, unit, dpi, rendererEngine, out var directPdfBytes, out var pages))
                    {
                        var directResponse = new ConvertResponse
                        {
                            Success = true,
                            Format = format,
                            Pdf = Convert.ToBase64String(directPdfBytes!),
                            Pages = pages,
                            Message = "Conversion successful"
                        };

                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsJsonAsync(directResponse);
                        return;
                    }

                    // PNG pipeline (also used as fallback for PDF).
                    var imageDataList = ConvertToImages(request, conversionService, width, height, unit, dpi, rendererEngine);

                    if (imageDataList == null || imageDataList.Count == 0)
                    {
                        await WriteBadRequest(context, "No images generated from ZPL content");
                        return;
                    }

                    // Generate response based on format
                    var response = new ConvertResponse
                    {
                        Success = true,
                        Format = format,
                        Pages = imageDataList.Count,
                        Message = "Conversion successful"
                    };

                    if (format == "pdf")
                    {
                        var pdfBytes = PdfGenerator.GeneratePdfToBytes(imageDataList);
                        response.Pdf = Convert.ToBase64String(pdfBytes);
                    }
                    else // png
                    {
                        if (imageDataList.Count == 1)
                        {
                            response.Image = Convert.ToBase64String(imageDataList[0]);
                        }
                        else
                        {
                            response.Images = imageDataList.Select(img => Convert.ToBase64String(img)).ToList();
                        }
                    }

                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsJsonAsync(response);
                }
                catch (Exception ex)
                {
                    var errorResponse = new ConvertResponse
                    {
                        Success = false,
                        Message = $"Conversion error: {ex.Message}"
                    };
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
            });

            // Health check endpoint
            app.MapGet("/api/health", () => new { status = "ok", service = "ZPL2PDF API" });

            Console.WriteLine($"ZPL2PDF API is starting on {host}:{port}");
            Console.WriteLine($"API endpoint: http://{host}:{port}/api/convert");
            Console.WriteLine($"Health check: http://{host}:{port}/api/health");
            Console.WriteLine("Press Ctrl+C to stop the API server");

            await app.RunAsync($"http://{host}:{port}");
        }

        private static async Task WriteBadRequest(HttpContext context, string message)
        {
            var errorResponse = new ConvertResponse
            {
                Success = false,
                Message = message
            };
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(errorResponse);
        }

        private static bool TryGetRendererEngine(string? rendererRaw, out RendererEngine rendererEngine)
        {
            var renderer = (rendererRaw ?? "offline").Trim().ToLowerInvariant();
            rendererEngine = renderer switch
            {
                "offline" => RendererEngine.Offline,
                "labelary" => RendererEngine.Labelary,
                "auto" => RendererEngine.Auto,
                _ => RendererEngine.Offline
            };

            return renderer == "offline" || renderer == "labelary" || renderer == "auto";
        }

        private static bool TryConvertDirectPdfResponse(
            ConvertRequest request,
            ConversionService conversionService,
            double width,
            double height,
            string unit,
            int dpi,
            RendererEngine rendererEngine,
            out byte[]? directPdfBytes,
            out int pages)
        {
            directPdfBytes = null;
            pages = 0;

            if (!string.IsNullOrWhiteSpace(request.Zpl))
            {
                var preprocessed = LabelFileReader.PreprocessZpl(request.Zpl);
                pages = LabelFileReader.SplitLabels(preprocessed).Count;

                conversionService.TryConvertPdfDirectWithLabelary(
                    request.Zpl,
                    width,
                    height,
                    unit,
                    dpi,
                    rendererEngine,
                    out directPdfBytes);
            }
            else if (request.ZplArray != null && request.ZplArray.Count > 0)
            {
                var pdfParts = new List<byte[]>();
                foreach (var zpl in request.ZplArray)
                {
                    if (string.IsNullOrWhiteSpace(zpl))
                        continue;

                    var preprocessed = LabelFileReader.PreprocessZpl(zpl);
                    pages += LabelFileReader.SplitLabels(preprocessed).Count;

                    if (!conversionService.TryConvertPdfDirectWithLabelary(
                        zpl,
                        width,
                        height,
                        unit,
                        dpi,
                        rendererEngine,
                        out var partPdfBytes))
                    {
                        pdfParts.Clear();
                        break;
                    }

                    pdfParts.Add(partPdfBytes!);
                }

                if (pdfParts.Count > 0)
                {
                    directPdfBytes = pdfParts.Count == 1
                        ? pdfParts[0]
                        : PdfGenerator.MergePdfsToBytes(pdfParts);
                }
            }

            return directPdfBytes != null;
        }

        private static List<byte[]> ConvertToImages(
            ConvertRequest request,
            ConversionService conversionService,
            double width,
            double height,
            string unit,
            int dpi,
            RendererEngine rendererEngine)
        {
            var imageDataList = new List<byte[]>();

            if (!string.IsNullOrWhiteSpace(request.Zpl))
            {
                return conversionService.Convert(request.Zpl, width, height, unit, dpi, rendererEngine: rendererEngine);
            }

            if (request.ZplArray == null || request.ZplArray.Count == 0)
            {
                return imageDataList;
            }

            foreach (var zpl in request.ZplArray)
            {
                if (string.IsNullOrWhiteSpace(zpl))
                    continue;

                var images = conversionService.Convert(zpl, width, height, unit, dpi, rendererEngine: rendererEngine);
                imageDataList.AddRange(images);
            }

            return imageDataList;
        }
    }
}