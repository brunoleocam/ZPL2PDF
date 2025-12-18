using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Printing.MacOS
{
    /// <summary>
    /// macOS-specific virtual printer installer using CUPS backend.
    /// Creates a CUPS printer that redirects output to ZPL2PDF for conversion.
    /// </summary>
    public class MacPrinterInstaller : IPrinterInstaller
    {
        private const string DefaultPrinterName = "ZPL2PDF";
        private const string BackendName = "zpl2pdf";
        private const string CupsBackendDir = "/usr/libexec/cups/backend";

        /// <inheritdoc/>
        public string PlatformName => "macOS (CUPS)";

        /// <inheritdoc/>
        public bool IsInstalled => CheckPrinterExists(DefaultPrinterName);

        /// <inheritdoc/>
        public async Task<bool> InstallAsync(string printerName = DefaultPrinterName)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("Error: This installer only works on macOS.");
                return false;
            }

            try
            {
                Console.WriteLine("Installing ZPL2PDF Virtual Printer for macOS (CUPS)...");
                Console.WriteLine();

                // Step 1: Check if running as root
                if (!IsRoot())
                {
                    Console.WriteLine("Error: Root privileges required.");
                    Console.WriteLine("Please run this command with sudo.");
                    return false;
                }

                // Step 2: Get ZPL2PDF executable path
                var exePath = GetZpl2PdfExePath();
                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("Error: Could not find ZPL2PDF executable.");
                    return false;
                }

                Console.WriteLine($"ZPL2PDF Path: {exePath}");

                // Step 3: Install CUPS backend
                Console.WriteLine("Installing CUPS backend...");
                if (!await InstallCupsBackendAsync(exePath))
                {
                    Console.WriteLine("Error: Failed to install CUPS backend.");
                    return false;
                }

                // Step 4: Restart CUPS
                Console.WriteLine("Restarting CUPS service...");
                await RestartCupsAsync();

                // Step 5: Add printer
                Console.WriteLine("Adding printer...");
                if (!await AddPrinterAsync(printerName))
                {
                    Console.WriteLine("Error: Failed to add printer.");
                    return false;
                }

                Console.WriteLine();
                Console.WriteLine($"Printer '{printerName}' installed successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during installation: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UninstallAsync()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return false;
            }

            try
            {
                if (!IsRoot())
                {
                    Console.WriteLine("Error: Root privileges required.");
                    return false;
                }

                Console.WriteLine("Removing printer...");
                await RunCommandAsync("lpadmin", $"-x {DefaultPrinterName}");

                Console.WriteLine("Removing CUPS backend...");
                var backendPath = Path.Combine(CupsBackendDir, BackendName);
                if (File.Exists(backendPath))
                {
                    File.Delete(backendPath);
                }

                await RestartCupsAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during uninstallation: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public PrinterStatus GetStatus()
        {
            var status = new PrinterStatus
            {
                IsInstalled = CheckPrinterExists(DefaultPrinterName),
                PrinterName = DefaultPrinterName,
                OutputDirectory = VirtualPrinterService.DefaultOutputDirectory
            };

            if (status.IsInstalled)
            {
                status.Message = "Printer is ready. Select 'ZPL2PDF' when printing to convert ZPL to PDF.";
            }
            else
            {
                status.Message = "Printer is not installed. Run 'sudo zpl2pdf printer install' to install.";
            }

            return status;
        }

        /// <summary>
        /// Checks if the current process is running as root.
        /// </summary>
        private bool IsRoot()
        {
            var (exitCode, output) = RunCommandAsync("id", "-u").Result;
            return exitCode == 0 && output.Trim() == "0";
        }

        /// <summary>
        /// Gets the path to the ZPL2PDF executable.
        /// </summary>
        private string? GetZpl2PdfExePath()
        {
            // First try the current executable location
            var currentExe = Process.GetCurrentProcess().MainModule?.FileName;
            if (!string.IsNullOrEmpty(currentExe) && File.Exists(currentExe))
            {
                return currentExe;
            }

            // Try common installation paths
            var possiblePaths = new[]
            {
                "/usr/local/bin/zpl2pdf",
                "/opt/homebrew/bin/zpl2pdf",
                "/Applications/ZPL2PDF.app/Contents/MacOS/ZPL2PDF"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        /// <summary>
        /// Installs the CUPS backend script.
        /// </summary>
        private async Task<bool> InstallCupsBackendAsync(string exePath)
        {
            try
            {
                var backendPath = Path.Combine(CupsBackendDir, BackendName);

                // Create backend script
                var backendScript = $@"#!/bin/bash
# ZPL2PDF CUPS Backend for macOS
# This script is called by CUPS when printing to the ZPL2PDF virtual printer

case ""$1"" in
    # Discovery mode - return device info
    """")
        echo ""direct {BackendName}:/ \""ZPL2PDF Virtual Printer\"" \""ZPL2PDF - Convert ZPL to PDF\""""
        exit 0
        ;;
esac

# Process print job - read from stdin and convert
exec ""{exePath}"" printer process
";

                await File.WriteAllTextAsync(backendPath, backendScript);

                // Make executable
                await RunCommandAsync("chmod", $"755 {backendPath}");

                // Set ownership to root
                await RunCommandAsync("chown", $"root:wheel {backendPath}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error installing backend: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Restarts the CUPS service.
        /// </summary>
        private async Task RestartCupsAsync()
        {
            // macOS uses launchctl
            await RunCommandAsync("launchctl", "stop org.cups.cupsd");
            await Task.Delay(500);
            await RunCommandAsync("launchctl", "start org.cups.cupsd");
        }

        /// <summary>
        /// Adds a printer using lpadmin.
        /// </summary>
        private async Task<bool> AddPrinterAsync(string printerName)
        {
            var (exitCode, _) = await RunCommandAsync("lpadmin",
                $"-p {printerName} -E -v {BackendName}:/ -m raw -D \"ZPL2PDF Virtual Printer\" -L \"Converts ZPL to PDF\"");

            return exitCode == 0;
        }

        /// <summary>
        /// Checks if a printer with the given name exists.
        /// </summary>
        private bool CheckPrinterExists(string printerName)
        {
            try
            {
                var (exitCode, output) = RunCommandAsync("lpstat", $"-p {printerName}").Result;
                return exitCode == 0 && !string.IsNullOrWhiteSpace(output);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Runs a command and returns the exit code and output.
        /// </summary>
        private async Task<(int exitCode, string output)> RunCommandAsync(string command, string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null)
                    return (-1, string.Empty);

                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                return (process.ExitCode, output);
            }
            catch (Exception ex)
            {
                return (-1, ex.Message);
            }
        }
    }
}

