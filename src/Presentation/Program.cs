using System;
using System.Threading.Tasks;

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
                Console.Error.WriteLine($"Error during execution: {ex.Message}");
            }
        }
    }
}