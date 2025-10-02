using System;
using System.IO;
using ZPL2PDF.Application.Interfaces;

namespace ZPL2PDF.Application.Services
{
    /// <summary>
    /// Service responsible for path management
    /// </summary>
    public class PathService : IPathService
    {
        /// <summary>
        /// Ensures directory exists, creating it if necessary
        /// </summary>
        public void EnsureDirectoryExists(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty", nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// Gets the default monitoring folder based on OS
        /// </summary>
        public string GetDefaultListenFolder()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsPath, "ZPL2PDF Auto Converter");
        }

        /// <summary>
        /// Gets the configuration folder based on OS
        /// </summary>
        public string GetConfigFolder()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %APPDATA%\ZPL2PDF
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appDataPath, "ZPL2PDF");
            }
            else
            {
                // Linux: ~/.config/zpl2pdf
                var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return Path.Combine(homePath, ".config", "zpl2pdf");
            }
        }

        /// <summary>
        /// Gets the PID folder based on OS
        /// </summary>
        public string GetPidFolder()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %TEMP%
                return Path.GetTempPath();
            }
            else
            {
                // Linux: /var/run
                return "/var/run";
            }
        }

        /// <summary>
        /// Combines paths safely
        /// </summary>
        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        /// <summary>
        /// Gets directory name from path
        /// </summary>
        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path) ?? string.Empty;
        }
    }
}
