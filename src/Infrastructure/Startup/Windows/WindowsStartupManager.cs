using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ZPL2PDF.Infrastructure.Startup.Windows
{
    /// <summary>
    /// Windows-specific startup manager using Registry.
    /// </summary>
    public class WindowsStartupManager : IStartupManager
    {
        private const string RegistryRunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string DaemonKeyName = "ZPL2PDF_Daemon";
        private const string TcpServerKeyName = "ZPL2PDF_TcpServer";
        private const string PrinterKeyName = "ZPL2PDF_Printer";

        /// <inheritdoc/>
        public string PlatformName => "Windows";

        /// <inheritdoc/>
        public Task<bool> EnableStartupAsync(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Task.FromResult(false);
            }

            try
            {
                var exePath = GetZpl2PdfExePath();
                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("Error: Could not find ZPL2PDF executable.");
                    return Task.FromResult(false);
                }

                var (keyName, arguments) = GetRegistryInfo(serviceType);
                var command = $"\"{exePath}\" {arguments}";

                using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true);
                if (key == null)
                {
                    Console.WriteLine("Error: Could not open registry key.");
                    return Task.FromResult(false);
                }

                key.SetValue(keyName, command);
                Console.WriteLine($"Enabled {serviceType} startup: {command}");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enabling startup: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc/>
        public Task<bool> DisableStartupAsync(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Task.FromResult(false);
            }

            try
            {
                var (keyName, _) = GetRegistryInfo(serviceType);

                using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey, writable: true);
                if (key == null)
                {
                    return Task.FromResult(true); // Key doesn't exist, nothing to disable
                }

                key.DeleteValue(keyName, throwOnMissingValue: false);
                Console.WriteLine($"Disabled {serviceType} startup.");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disabling startup: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc/>
        public bool IsStartupEnabled(StartupServiceType serviceType)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            try
            {
                var (keyName, _) = GetRegistryInfo(serviceType);

                using var key = Registry.CurrentUser.OpenSubKey(RegistryRunKey);
                if (key == null)
                {
                    return false;
                }

                var value = key.GetValue(keyName);
                return value != null;
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
                Message = "Startup entries are stored in the Windows Registry (HKCU\\...\\Run)."
            };
        }

        /// <summary>
        /// Gets the registry key name and command arguments for the service type.
        /// </summary>
        private (string keyName, string arguments) GetRegistryInfo(StartupServiceType serviceType)
        {
            return serviceType switch
            {
                StartupServiceType.Daemon => (DaemonKeyName, "start"),
                StartupServiceType.TcpServer => (TcpServerKeyName, "server start"),
                StartupServiceType.Printer => (PrinterKeyName, "printer start"),
                _ => throw new ArgumentException($"Unknown service type: {serviceType}")
            };
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
    }
}

