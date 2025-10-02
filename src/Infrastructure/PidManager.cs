using System;
using System.IO;

namespace ZPL2PDF
{
    /// <summary>
    /// Manages PID file operations for daemon processes
    /// </summary>
    public class PidManager
    {
        private readonly string _pidFilePath;

        /// <summary>
        /// Initializes a new instance of the PidManager
        /// </summary>
        public PidManager()
        {
            _pidFilePath = GetPidFilePath();
        }

        /// <summary>
        /// Gets the PID from the PID file
        /// </summary>
        /// <returns>PID value or 0 if not found</returns>
        public int GetPidFromFile()
        {
            try
            {
                if (File.Exists(_pidFilePath))
                {
                    var content = File.ReadAllText(_pidFilePath).Trim();
                    if (int.TryParse(content, out int pid))
                    {
                        return pid;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading PID file: {ex.Message}");
            }

            return 0;
        }

        /// <summary>
        /// Saves the PID to the PID file
        /// </summary>
        /// <param name="pid">Process ID to save</param>
        /// <returns>True if saved successfully, False otherwise</returns>
        public bool SavePidToFile(int pid)
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(_pidFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_pidFilePath, pid.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving PID file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes the PID file
        /// </summary>
        /// <returns>True if removed successfully, False otherwise</returns>
        public bool RemovePidFile()
        {
            try
            {
                if (File.Exists(_pidFilePath))
                {
                    File.Delete(_pidFilePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing PID file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the PID file exists
        /// </summary>
        /// <returns>True if exists, False otherwise</returns>
        public bool PidFileExists()
        {
            return File.Exists(_pidFilePath);
        }

        /// <summary>
        /// Gets the PID file path based on the operating system
        /// </summary>
        /// <returns>PID file path</returns>
        private string GetPidFilePath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %TEMP%\zpl2pdf.pid
                var tempPath = Path.GetTempPath();
                return Path.Combine(tempPath, "zpl2pdf.pid");
            }
            else
            {
                // Linux: /var/run/zpl2pdf.pid
                return "/var/run/zpl2pdf.pid";
            }
        }
    }
}
