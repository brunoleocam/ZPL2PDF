using System;
using System.Threading.Tasks;
using ZPL2PDF.Shared.Localization;

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
    }
}