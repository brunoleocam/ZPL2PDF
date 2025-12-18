using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Startup.MacOS
{
    /// <summary>
    /// macOS-specific startup manager using launchd.
    /// </summary>
    public class MacStartupManager : IStartupManager
    {
        private const string LaunchAgentsDir = "Library/LaunchAgents";

        /// <inheritdoc/>
        public string PlatformName => "macOS (launchd)";

        /// <inheritdoc/>
        public async Task<bool> EnableStartupAsync(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return false;
            }

            try
            {
                var plistName = GetPlistName(serviceType);
                var exePath = GetZpl2PdfExePath();
                
                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("Error: Could not find ZPL2PDF executable.");
                    return false;
                }

                // Create LaunchAgents directory
                var agentsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), LaunchAgentsDir);
                Directory.CreateDirectory(agentsDir);

                // Create plist file
                var plistPath = Path.Combine(agentsDir, $"{plistName}.plist");
                var plistContent = CreatePlistFile(serviceType, exePath, plistName);
                await File.WriteAllTextAsync(plistPath, plistContent);

                // Load the launch agent
                var (exitCode, _) = await RunCommandAsync("launchctl", $"load {plistPath}");

                if (exitCode == 0)
                {
                    Console.WriteLine($"Enabled {serviceType} startup.");
                    Console.WriteLine($"Plist file: {plistPath}");
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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return false;
            }

            try
            {
                var plistName = GetPlistName(serviceType);
                var agentsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), LaunchAgentsDir);
                var plistPath = Path.Combine(agentsDir, $"{plistName}.plist");

                // Unload the launch agent
                if (File.Exists(plistPath))
                {
                    await RunCommandAsync("launchctl", $"unload {plistPath}");
                    File.Delete(plistPath);
                }

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
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return false;
            }

            try
            {
                var plistName = GetPlistName(serviceType);
                var agentsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), LaunchAgentsDir);
                var plistPath = Path.Combine(agentsDir, $"{plistName}.plist");
                
                return File.Exists(plistPath);
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
                Message = "Startup entries are stored as launchd agents in ~/Library/LaunchAgents."
            };
        }

        /// <summary>
        /// Gets the plist name for the service type.
        /// </summary>
        private string GetPlistName(StartupServiceType serviceType)
        {
            return serviceType switch
            {
                StartupServiceType.Daemon => "com.zpl2pdf.daemon",
                StartupServiceType.TcpServer => "com.zpl2pdf.tcpserver",
                StartupServiceType.Printer => "com.zpl2pdf.printer",
                _ => throw new ArgumentException($"Unknown service type: {serviceType}")
            };
        }

        /// <summary>
        /// Creates the launchd plist file content.
        /// </summary>
        private string CreatePlistFile(StartupServiceType serviceType, string exePath, string plistName)
        {
            var arguments = serviceType switch
            {
                StartupServiceType.Daemon => new[] { exePath, "run" },
                StartupServiceType.TcpServer => new[] { exePath, "server", "start", "--foreground" },
                StartupServiceType.Printer => new[] { exePath, "printer", "start" },
                _ => throw new ArgumentException($"Unknown service type: {serviceType}")
            };

            var argumentsXml = string.Join("\n        ", Array.ConvertAll(arguments, a => $"<string>{a}</string>"));

            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
<dict>
    <key>Label</key>
    <string>{plistName}</string>
    <key>ProgramArguments</key>
    <array>
        {argumentsXml}
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
    <key>StandardOutPath</key>
    <string>/tmp/{plistName}.log</string>
    <key>StandardErrorPath</key>
    <string>/tmp/{plistName}.error.log</string>
</dict>
</plist>
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

