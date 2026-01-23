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

                // Route to appropriate mode
                if (argumentProcessor.Mode == OperationMode.Daemon)
                {
                    var daemonHandler = new DaemonModeHandler();
                    await daemonHandler.HandleDaemon(argumentProcessor);
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
            // Parse port from arguments (--port 5000)
            int port = 5000;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port" && i + 1 < args.Length && int.TryParse(args[i + 1], out int parsedPort))
                {
                    port = parsedPort;
                    break;
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

                    // Convert ZPL(s)
                    var conversionService = new ConversionService();
                    var imageDataList = new List<byte[]>();

                    if (!string.IsNullOrWhiteSpace(request.Zpl))
                    {
                        // Single ZPL string (can contain multiple labels)
                        imageDataList = conversionService.Convert(request.Zpl, width, height, unit, dpi);
                    }
                    else if (request.ZplArray != null && request.ZplArray.Count > 0)
                    {
                        // Multiple ZPL strings - convert each and combine
                        foreach (var zpl in request.ZplArray)
                        {
                            if (!string.IsNullOrWhiteSpace(zpl))
                            {
                                var images = conversionService.Convert(zpl, width, height, unit, dpi);
                                imageDataList.AddRange(images);
                            }
                        }
                    }

                    if (imageDataList == null || imageDataList.Count == 0)
                    {
                        var errorResponse = new ConvertResponse
                        {
                            Success = false,
                            Message = "No images generated from ZPL content"
                        };
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsJsonAsync(errorResponse);
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
                        // Generate PDF from images
                        var pdfBytes = PdfGenerator.GeneratePdfToBytes(imageDataList);
                        response.Pdf = Convert.ToBase64String(pdfBytes);
                    }
                    else // png
                    {
                        // Convert images to base64
                        if (imageDataList.Count == 1)
                        {
                            // Single image
                            response.Image = Convert.ToBase64String(imageDataList[0]);
                        }
                        else
                        {
                            // Multiple images
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

            Console.WriteLine($"ZPL2PDF API is starting on port {port}");
            Console.WriteLine($"API endpoint: http://localhost:{port}/api/convert");
            Console.WriteLine($"Health check: http://localhost:{port}/api/health");
            Console.WriteLine("Press Ctrl+C to stop the API server");

            await app.RunAsync($"http://localhost:{port}");
        }
    }
}