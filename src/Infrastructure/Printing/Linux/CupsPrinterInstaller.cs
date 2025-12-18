using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Printing.Linux
{
    /// <summary>
    /// Linux-specific virtual printer installer using CUPS backend.
    /// Creates a CUPS printer that redirects output to ZPL2PDF for conversion.
    /// </summary>
    public class CupsPrinterInstaller : IPrinterInstaller
    {
        private const string DefaultPrinterName = "ZPL2PDF";
        private const string BackendName = "zpl2pdf";
        private const string CupsBackendDir = "/usr/lib/cups/backend";
        private const string CupsBackendDirAlt = "/usr/libexec/cups/backend";

        /// <inheritdoc/>
        public string PlatformName => "Linux (CUPS)";

        /// <inheritdoc/>
        public bool IsInstalled => CheckPrinterExists(DefaultPrinterName);

        /// <inheritdoc/>
        public async Task<bool> InstallAsync(string printerName = DefaultPrinterName)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine("Error: This installer only works on Linux.");
                return false;
            }

            try
            {
                Console.WriteLine("Installing ZPL2PDF Virtual Printer for Linux (CUPS)...");
                Console.WriteLine();

                // Step 1: Check if CUPS is installed
                if (!await IsCupsInstalledAsync())
                {
                    Console.WriteLine("Error: CUPS is not installed.");
                    Console.WriteLine("Install CUPS with: sudo apt install cups");
                    return false;
                }

                // Step 2: Check if running as root
                if (!IsRoot())
                {
                    Console.WriteLine("Error: Root privileges required.");
                    Console.WriteLine("Please run this command with sudo.");
                    return false;
                }

                // Step 3: Get ZPL2PDF executable path
                var exePath = GetZpl2PdfExePath();
                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("Error: Could not find ZPL2PDF executable.");
                    return false;
                }

                Console.WriteLine($"ZPL2PDF Path: {exePath}");

                // Step 4: Install CUPS backend
                Console.WriteLine("Installing CUPS backend...");
                if (!await InstallCupsBackendAsync(exePath))
                {
                    Console.WriteLine("Error: Failed to install CUPS backend.");
                    return false;
                }

                // Step 5: Restart CUPS
                Console.WriteLine("Restarting CUPS service...");
                await RestartCupsAsync();

                // Step 6: Add printer
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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
                var backendPath = GetCupsBackendPath();
                if (!string.IsNullOrEmpty(backendPath))
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
        /// Checks if CUPS is installed.
        /// </summary>
        private async Task<bool> IsCupsInstalledAsync()
        {
            var (exitCode, _) = await RunCommandAsync("which", "lpadmin");
            return exitCode == 0;
        }

        /// <summary>
        /// Checks if the current process is running as root.
        /// </summary>
        private bool IsRoot()
        {
            try
            {
                // Check using id command
                var (exitCode, output) = RunCommandAsync("id", "-u").Result;
                return exitCode == 0 && output.Trim() == "0";
            }
            catch
            {
                // Fallback: check username
                return Environment.UserName == "root";
            }
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
                "/usr/bin/zpl2pdf",
                "/usr/local/bin/zpl2pdf",
                "/opt/zpl2pdf/zpl2pdf"
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
        /// Gets the CUPS backend directory.
        /// </summary>
        private string GetCupsBackendDir()
        {
            if (Directory.Exists(CupsBackendDir))
                return CupsBackendDir;
            if (Directory.Exists(CupsBackendDirAlt))
                return CupsBackendDirAlt;
            return CupsBackendDir;
        }

        /// <summary>
        /// Gets the path to the CUPS backend.
        /// </summary>
        private string? GetCupsBackendPath()
        {
            var backendDir = GetCupsBackendDir();
            var backendPath = Path.Combine(backendDir, BackendName);
            return File.Exists(backendPath) ? backendPath : null;
        }

        /// <summary>
        /// Installs the CUPS backend script.
        /// </summary>
        private async Task<bool> InstallCupsBackendAsync(string exePath)
        {
            try
            {
                var backendDir = GetCupsBackendDir();
                var backendPath = Path.Combine(backendDir, BackendName);

                // Create backend script
                var backendScript = $@"#!/bin/bash
# ZPL2PDF CUPS Backend
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
                await RunCommandAsync("chown", $"root:root {backendPath}");

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
            // Try systemd first
            var (exitCode, _) = await RunCommandAsync("systemctl", "restart cups");
            if (exitCode != 0)
            {
                // Try service command
                await RunCommandAsync("service", "cups restart");
            }
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

