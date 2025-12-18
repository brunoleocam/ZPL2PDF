using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Startup.Linux
{
    /// <summary>
    /// Linux-specific startup manager using systemd user services.
    /// </summary>
    public class LinuxStartupManager : IStartupManager
    {
        private const string SystemdUserDir = ".config/systemd/user";

        /// <inheritdoc/>
        public string PlatformName => "Linux (systemd)";

        /// <inheritdoc/>
        public async Task<bool> EnableStartupAsync(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            try
            {
                var serviceName = GetServiceName(serviceType);
                var exePath = GetZpl2PdfExePath();
                
                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("Error: Could not find ZPL2PDF executable.");
                    return false;
                }

                // Create systemd user directory
                var userDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), SystemdUserDir);
                Directory.CreateDirectory(userDir);

                // Create service file
                var servicePath = Path.Combine(userDir, $"{serviceName}.service");
                var serviceContent = CreateServiceFile(serviceType, exePath);
                await File.WriteAllTextAsync(servicePath, serviceContent);

                // Reload systemd and enable service
                await RunCommandAsync("systemctl", "--user daemon-reload");
                var (exitCode, _) = await RunCommandAsync("systemctl", $"--user enable {serviceName}");

                if (exitCode == 0)
                {
                    Console.WriteLine($"Enabled {serviceType} startup.");
                    Console.WriteLine($"Service file: {servicePath}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling startup: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DisableStartupAsync(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            try
            {
                var serviceName = GetServiceName(serviceType);

                // Disable and stop service
                await RunCommandAsync("systemctl", $"--user disable {serviceName}");
                await RunCommandAsync("systemctl", $"--user stop {serviceName}");

                // Remove service file
                var userDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), SystemdUserDir);
                var servicePath = Path.Combine(userDir, $"{serviceName}.service");
                
                if (File.Exists(servicePath))
                {
                    File.Delete(servicePath);
                }

                await RunCommandAsync("systemctl", "--user daemon-reload");

                Console.WriteLine($"Disabled {serviceType} startup.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling startup: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool IsStartupEnabled(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            try
            {
                var serviceName = GetServiceName(serviceType);
                var (exitCode, output) = RunCommandAsync("systemctl", $"--user is-enabled {serviceName}").Result;
                return exitCode == 0 && output.Trim() == "enabled";
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public StartupStatus GetStatus()
        {
            return new StartupStatus
            {
                DaemonEnabled = IsStartupEnabled(StartupServiceType.Daemon),
                TcpServerEnabled = IsStartupEnabled(StartupServiceType.TcpServer),
                PrinterEnabled = IsStartupEnabled(StartupServiceType.Printer),
                Message = "Startup entries are stored as systemd user services."
            };
        }

        /// <summary>
        /// Gets the systemd service name for the service type.
        /// </summary>
        private string GetServiceName(StartupServiceType serviceType)
        {
            return serviceType switch
            {
                StartupServiceType.Daemon => "zpl2pdf-daemon",
                StartupServiceType.TcpServer => "zpl2pdf-tcpserver",
                StartupServiceType.Printer => "zpl2pdf-printer",
                _ => throw new ArgumentException($"Unknown service type: {serviceType}")
            };
        }

        /// <summary>
        /// Creates the systemd service file content.
        /// </summary>
        private string CreateServiceFile(StartupServiceType serviceType, string exePath)
        {
            var (description, execStart) = serviceType switch
            {
                StartupServiceType.Daemon => ("ZPL2PDF Daemon - Automatic folder monitoring", $"{exePath} run"),
                StartupServiceType.TcpServer => ("ZPL2PDF TCP Server - Virtual printer on TCP", $"{exePath} server start --foreground"),
                StartupServiceType.Printer => ("ZPL2PDF Virtual Printer Service", $"{exePath} printer start"),
                _ => throw new ArgumentException($"Unknown service type: {serviceType}")
            };

            return $@"[Unit]
Description={description}
After=network.target

[Service]
Type=simple
ExecStart={execStart}
Restart=on-failure
RestartSec=5

[Install]
WantedBy=default.target
";
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

