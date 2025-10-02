using System;
using System.Diagnostics;
using System.IO;

namespace ZPL2PDF
{
    /// <summary>
    /// Manages process operations for daemon
    /// </summary>
    public class ProcessManager
    {
        private readonly string _executablePath;

        /// <summary>
        /// Initializes a new instance of the ProcessManager
        /// </summary>
        public ProcessManager()
        {
            _executablePath = GetExecutablePath();
        }

        /// <summary>
        /// Starts a background process with the specified arguments
        /// </summary>
        /// <param name="arguments">Command line arguments</param>
        /// <returns>Process ID if started successfully, 0 otherwise</returns>
        public int StartBackgroundProcess(string arguments)
        {
            try
            {
                Console.WriteLine($"DEBUG - Starting background process:");
                Console.WriteLine($"  Executable: {_executablePath}");
                Console.WriteLine($"  Arguments: {arguments}");
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = _executablePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(startInfo);
                if (process != null)
                {
                    Console.WriteLine($"DEBUG - Background process started with PID: {process.Id}");
                    return process.Id;
                }
                else
                {
                    Console.WriteLine("DEBUG - Failed to start process - Process.Start returned null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting background process: {ex.Message}");
            }

            return 0;
        }

        /// <summary>
        /// Kills a process by its ID
        /// </summary>
        /// <param name="pid">Process ID to kill</param>
        /// <returns>True if killed successfully, False otherwise</returns>
        public bool KillProcess(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                process.Kill();
                process.WaitForExit(5000); // Wait up to 5 seconds
                return true;
            }
            catch (ArgumentException)
            {
                // Process no longer exists
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error killing process {pid}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a process is running by its ID
        /// </summary>
        /// <param name="pid">Process ID to check</param>
        /// <returns>True if running, False otherwise</returns>
        public bool IsProcessRunning(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                return !process.HasExited;
            }
            catch (ArgumentException)
            {
                // Process doesn't exist
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets process information by its ID
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns>Process information or null if not found</returns>
        public ProcessInfo GetProcessInfo(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                return new ProcessInfo
                {
                    Id = process.Id,
                    ProcessName = process.ProcessName,
                    StartTime = process.StartTime,
                    HasExited = process.HasExited
                };
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the executable path
        /// </summary>
        /// <returns>Executable path</returns>
        private string GetExecutablePath()
        {
            var executablePath = Environment.ProcessPath ?? 
                Path.Combine(AppContext.BaseDirectory, 
                    Environment.OSVersion.Platform == PlatformID.Win32NT ? "ZPL2PDF.exe" : "ZPL2PDF");
            
            //Console.WriteLine($"DEBUG - Executable path: {executablePath}");
            //Console.WriteLine($"DEBUG - File exists: {File.Exists(executablePath)}");
            //Console.WriteLine($"DEBUG - Environment.ProcessPath: {Environment.ProcessPath}");
            //Console.WriteLine($"DEBUG - AppContext.BaseDirectory: {AppContext.BaseDirectory}");
            
            return executablePath;
        }
    }

    /// <summary>
    /// Represents process information
    /// </summary>
    public class ProcessInfo
    {
        public int Id { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public bool HasExited { get; set; }
    }
}
