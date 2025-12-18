using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Printing.Windows
{
    /// <summary>
    /// Windows-specific virtual printer installer using port monitor redirection.
    /// Uses the Windows printing subsystem to create a virtual printer that redirects
    /// output to ZPL2PDF for conversion.
    /// </summary>
    public class WindowsPrinterInstaller : IPrinterInstaller
    {
        private const string DefaultPrinterName = "ZPL2PDF";
        private const string PortName = "ZPL2PDF_PORT:";
        private const string DriverName = "Generic / Text Only";

        /// <inheritdoc/>
        public string PlatformName => "Windows";

        /// <inheritdoc/>
        public bool IsInstalled => CheckPrinterExists(DefaultPrinterName);

        /// <inheritdoc/>
        public async Task<bool> InstallAsync(string printerName = DefaultPrinterName)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Error: This installer only works on Windows.");
                return false;
            }

            try
            {
                Console.WriteLine("Installing ZPL2PDF Virtual Printer for Windows...");
                Console.WriteLine();

                // Step 1: Check if running as administrator
                if (!IsAdministrator())
                {
                    Console.WriteLine("Error: Administrator privileges required.");
                    Console.WriteLine("Please run this command as Administrator.");
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

                // Step 3: Create local port for redirection
                Console.WriteLine("Creating printer port...");
                if (!await CreateLocalPortAsync(PortName, exePath))
                {
                    Console.WriteLine("Error: Failed to create printer port.");
                    return false;
                }

                // Step 4: Add the printer
                Console.WriteLine("Adding printer...");
                if (!await AddPrinterAsync(printerName, PortName, DriverName))
                {
                    Console.WriteLine("Error: Failed to add printer.");
                    await DeletePortAsync(PortName);
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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            try
            {
                if (!IsAdministrator())
                {
                    Console.WriteLine("Error: Administrator privileges required.");
                    return false;
                }

                Console.WriteLine("Removing printer...");
                await DeletePrinterAsync(DefaultPrinterName);

                Console.WriteLine("Removing port...");
                await DeletePortAsync(PortName);

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
                status.Message = "Printer is not installed. Run 'ZPL2PDF printer install' to install.";
            }

            return status;
        }

        /// <summary>
        /// Checks if the current process is running with administrator privileges.
        /// </summary>
        private bool IsAdministrator()
        {
            try
            {
                using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
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
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var possiblePath = Path.Combine(programFiles, "ZPL2PDF", "ZPL2PDF.exe");
            if (File.Exists(possiblePath))
            {
                return possiblePath;
            }

            return null;
        }

        /// <summary>
        /// Checks if a printer with the given name exists.
        /// </summary>
        private bool CheckPrinterExists(string printerName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-Command \"Get-Printer -Name '{printerName}' -ErrorAction SilentlyContinue\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                var output = process?.StandardOutput.ReadToEnd();
                process?.WaitForExit();

                return !string.IsNullOrWhiteSpace(output);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a local port that redirects to ZPL2PDF.
        /// </summary>
        private async Task<bool> CreateLocalPortAsync(string portName, string exePath)
        {
            try
            {
                // Use PowerShell to create the port
                // We'll use a file-based approach: write to a temp file, then process it
                var script = $@"
                    $portName = '{portName}'
                    $exePath = '{exePath.Replace("'", "''")}'
                    
                    # Check if port exists
                    $existingPort = Get-PrinterPort -Name $portName -ErrorAction SilentlyContinue
                    if ($existingPort) {{
                        Write-Host 'Port already exists'
                        exit 0
                    }}
                    
                    # Create a local port
                    Add-PrinterPort -Name $portName
                    Write-Host 'Port created successfully'
                ";

                return await RunPowerShellScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating port: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Adds a printer using the specified port and driver.
        /// </summary>
        private async Task<bool> AddPrinterAsync(string printerName, string portName, string driverName)
        {
            try
            {
                var script = $@"
                    $printerName = '{printerName}'
                    $portName = '{portName}'
                    $driverName = '{driverName}'
                    
                    # Check if printer exists
                    $existingPrinter = Get-Printer -Name $printerName -ErrorAction SilentlyContinue
                    if ($existingPrinter) {{
                        Write-Host 'Printer already exists'
                        exit 0
                    }}
                    
                    # Add the printer
                    Add-Printer -Name $printerName -PortName $portName -DriverName $driverName
                    Write-Host 'Printer added successfully'
                ";

                return await RunPowerShellScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding printer: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a printer.
        /// </summary>
        private async Task<bool> DeletePrinterAsync(string printerName)
        {
            try
            {
                var script = $@"
                    $printerName = '{printerName}'
                    Remove-Printer -Name $printerName -ErrorAction SilentlyContinue
                    Write-Host 'Printer removed'
                ";

                return await RunPowerShellScriptAsync(script);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a printer port.
        /// </summary>
        private async Task<bool> DeletePortAsync(string portName)
        {
            try
            {
                var script = $@"
                    $portName = '{portName}'
                    Remove-PrinterPort -Name $portName -ErrorAction SilentlyContinue
                    Write-Host 'Port removed'
                ";

                return await RunPowerShellScriptAsync(script);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Runs a PowerShell script.
        /// </summary>
        private async Task<bool> RunPowerShellScriptAsync(string script)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-ExecutionPolicy Bypass -Command \"{script.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return false;

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(output))
            {
                Console.WriteLine(output.Trim());
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine($"Warning: {error.Trim()}");
            }

            return process.ExitCode == 0;
        }
    }
}

