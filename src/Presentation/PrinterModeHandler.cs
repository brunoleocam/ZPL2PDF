using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ZPL2PDF.Infrastructure.Printing;
using ZPL2PDF.Infrastructure.Printing.Windows;
using ZPL2PDF.Infrastructure.Printing.Linux;
using ZPL2PDF.Infrastructure.Printing.MacOS;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF
{
    /// <summary>
    /// Handles Virtual Printer mode operations (install, uninstall, start, stop, status, process).
    /// </summary>
    public class PrinterModeHandler
    {
        /// <summary>
        /// Handles the virtual printer command.
        /// </summary>
        /// <param name="command">Command to execute (install, uninstall, start, stop, status, process).</param>
        /// <param name="args">Printer arguments.</param>
        /// <returns>Exit code (0 for success, non-zero for error).</returns>
        public async Task<int> HandleCommandAsync(string command, PrinterArguments args)
        {
            switch (command.ToLowerInvariant())
            {
                case "install":
                    return await HandleInstallAsync(args);

                case "uninstall":
                    return await HandleUninstallAsync();

                case "start":
                    return HandleStart(args);

                case "stop":
                    return HandleStop();

                case "status":
                    return HandleStatus();

                case "process":
                    return await HandleProcessAsync(args);

                default:
                    Console.WriteLine($"Unknown printer command: {command}");
                    ShowUsage();
                    return 1;
            }
        }

        /// <summary>
        /// Handles the install command.
        /// </summary>
        private async Task<int> HandleInstallAsync(PrinterArguments args)
        {
            Console.WriteLine("Installing ZPL2PDF Virtual Printer...");
            Console.WriteLine();

            var installer = GetPlatformInstaller();
            if (installer == null)
            {
                Console.WriteLine("Error: Virtual printer installation is not supported on this platform.");
                return 1;
            }

            if (installer.IsInstalled)
            {
                Console.WriteLine($"Virtual printer '{args.PrinterName}' is already installed.");
                return 0;
            }

            Console.WriteLine($"Platform: {installer.PlatformName}");
            Console.WriteLine($"Printer Name: {args.PrinterName}");
            Console.WriteLine();

            var success = await installer.InstallAsync(args.PrinterName);

            if (success)
            {
                Console.WriteLine();
                Console.WriteLine("Virtual printer installed successfully!");
                Console.WriteLine();
                Console.WriteLine("You can now select 'ZPL2PDF' as a printer in any application.");
                Console.WriteLine("When you print to this printer, ZPL commands will be converted to PDF.");
                return 0;
            }

            Console.WriteLine();
            Console.WriteLine("Error: Failed to install virtual printer.");
            Console.WriteLine("Make sure you have administrator/root privileges.");
            return 1;
        }

        /// <summary>
        /// Handles the uninstall command.
        /// </summary>
        private async Task<int> HandleUninstallAsync()
        {
            Console.WriteLine("Uninstalling ZPL2PDF Virtual Printer...");
            Console.WriteLine();

            var installer = GetPlatformInstaller();
            if (installer == null)
            {
                Console.WriteLine("Error: Virtual printer is not supported on this platform.");
                return 1;
            }

            if (!installer.IsInstalled)
            {
                Console.WriteLine("Virtual printer is not installed.");
                return 0;
            }

            var success = await installer.UninstallAsync();

            if (success)
            {
                Console.WriteLine("Virtual printer uninstalled successfully.");
                return 0;
            }

            Console.WriteLine("Error: Failed to uninstall virtual printer.");
            return 1;
        }

        /// <summary>
        /// Handles the start command (placeholder for future service mode).
        /// </summary>
        private int HandleStart(PrinterArguments args)
        {
            Console.WriteLine("Note: The virtual printer doesn't require a separate service to run.");
            Console.WriteLine("Once installed, it processes print jobs automatically.");
            Console.WriteLine();
            Console.WriteLine("Use 'ZPL2PDF printer status' to check if the printer is installed.");
            return 0;
        }

        /// <summary>
        /// Handles the stop command (placeholder for future service mode).
        /// </summary>
        private int HandleStop()
        {
            Console.WriteLine("Note: The virtual printer doesn't have a separate service to stop.");
            Console.WriteLine("Use 'ZPL2PDF printer uninstall' to remove the printer.");
            return 0;
        }

        /// <summary>
        /// Handles the status command.
        /// </summary>
        private int HandleStatus()
        {
            Console.WriteLine("ZPL2PDF Virtual Printer Status");
            Console.WriteLine("==============================");
            Console.WriteLine();

            var installer = GetPlatformInstaller();
            if (installer == null)
            {
                Console.WriteLine("Platform: Not Supported");
                Console.WriteLine("Virtual printer is not available on this platform.");
                return 1;
            }

            var status = installer.GetStatus();

            Console.WriteLine($"Platform: {installer.PlatformName}");
            Console.WriteLine($"Installed: {(status.IsInstalled ? "Yes" : "No")}");

            if (status.IsInstalled)
            {
                Console.WriteLine($"Printer Name: {status.PrinterName ?? "ZPL2PDF"}");
                Console.WriteLine($"Output Directory: {status.OutputDirectory ?? VirtualPrinterService.DefaultOutputDirectory}");
            }

            if (!string.IsNullOrEmpty(status.Message))
            {
                Console.WriteLine();
                Console.WriteLine(status.Message);
            }

            return 0;
        }

        /// <summary>
        /// Handles the process command - processes a print job from stdin.
        /// This is called internally by the port monitor/CUPS backend.
        /// </summary>
        private async Task<int> HandleProcessAsync(PrinterArguments args)
        {
            var service = new VirtualPrinterService(
                args.RendererMode,
                args.WidthMm,
                args.HeightMm,
                args.Dpi,
                args.OutputDirectory,
                args.OpenPdfAfterGeneration);

            return await service.ProcessPrintJobFromStdinAsync();
        }

        /// <summary>
        /// Gets the platform-specific printer installer.
        /// </summary>
        private IPrinterInstaller? GetPlatformInstaller()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsPrinterInstaller();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new CupsPrinterInstaller();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MacPrinterInstaller();
            }

            return null;
        }

        /// <summary>
        /// Shows usage information.
        /// </summary>
        private void ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Virtual Printer Commands:");
            Console.WriteLine("  ZPL2PDF printer install     - Install the virtual printer");
            Console.WriteLine("  ZPL2PDF printer uninstall   - Remove the virtual printer");
            Console.WriteLine("  ZPL2PDF printer status      - Check printer status");
            Console.WriteLine("  ZPL2PDF printer process     - Process print job (internal use)");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --printer-name <name>   Custom printer name (default: ZPL2PDF)");
            Console.WriteLine("  -o, --output <dir>      Output directory for PDFs");
            Console.WriteLine("  --renderer <mode>       Renderer: offline, labelary, auto");
            Console.WriteLine("  --no-open               Don't open PDF after generation");
        }
    }
}

