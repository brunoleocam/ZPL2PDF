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
        private readonly string _processFileName;
        private readonly string _processArgumentsPrefix;

        /// <summary>
        /// Initializes a new instance of the ProcessManager
        /// </summary>
        public ProcessManager()
        {
            (_processFileName, _processArgumentsPrefix) = GetProcessStartInfo();
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
                //Console.WriteLine($"DEBUG - Starting background process:");
                //Console.WriteLine($"  Executable: {_executablePath}");
                //Console.WriteLine($"  Arguments: {arguments}");
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = _processFileName,
                    // When using dotnet + dll, we need to prefix arguments with the dll path.
                    Arguments = string.IsNullOrWhiteSpace(_processArgumentsPrefix)
                        ? arguments
                        : $"{_processArgumentsPrefix} {arguments}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    // Avoid stdout/stderr pipe deadlocks when daemon runs in background.
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                var process = Process.Start(startInfo);
                if (process != null)
                {
                    //Console.WriteLine($"DEBUG - Background process started with PID: {process.Id}");
                    return process.Id;
                }
                else
                {
                    //Console.WriteLine("DEBUG - Failed to start process - Process.Start returned null");
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
        /// Resolves how to start the ZPL2PDF process.
        /// Prefer `dotnet ZPL2PDF.dll <args>` when running from test output.
        /// </summary>
        /// <returns>Tuple with process start file and argument prefix</returns>
        private static (string FileName, string ArgumentsPrefix) GetProcessStartInfo()
        {
            var appBaseDirectory = AppContext.BaseDirectory;

            // Prefer the real built executable from project output.
            // In `dotnet test`, AppContext.BaseDirectory usually points to the test output folder,
            // which may not contain the runnable daemon binary (missing hostpolicy bits, etc).
            var projectRoot = TryGetProjectRoot(appBaseDirectory);
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                var candidates = new[]
                {
                    // Windows (CI/dev)
                    Path.Combine(projectRoot, "bin", "Release", "net9.0", "win-x64", "ZPL2PDF.exe"),
                    Path.Combine(projectRoot, "bin", "Debug", "net9.0", "win-x64", "ZPL2PDF.exe"),

                    // Framework-dependent fallback (if ever produced)
                    Path.Combine(projectRoot, "bin", "Release", "net9.0", "ZPL2PDF.exe"),
                    Path.Combine(projectRoot, "bin", "Debug", "net9.0", "ZPL2PDF.exe")
                };

                foreach (var candidate in candidates)
                {
                    if (File.Exists(candidate))
                    {
                        return (candidate, string.Empty);
                    }
                }
            }

            // Fallback: start ZPL2PDF.exe next to the app, if present.
            var processPath = Environment.ProcessPath;
            if (!string.IsNullOrWhiteSpace(processPath))
            {
                var fileName = Path.GetFileName(processPath);
                if (!string.IsNullOrWhiteSpace(fileName) &&
                    fileName.StartsWith("ZPL2PDF", StringComparison.OrdinalIgnoreCase) &&
                    File.Exists(processPath))
                {
                    return (processPath, string.Empty);
                }
            }

            var executablePath = Path.Combine(appBaseDirectory, "ZPL2PDF.exe");
            if (File.Exists(executablePath))
            {
                return (executablePath, string.Empty);
            }

            // Last resort (rare): try running the dll via dotnet.
            var dllPath = Path.Combine(appBaseDirectory, "ZPL2PDF.dll");
            if (File.Exists(dllPath))
            {
                return ("dotnet", $"\"{dllPath}\"");
            }

            return (executablePath, string.Empty);
        }

        private static string? TryGetProjectRoot(string startDirectory)
        {
            try
            {
                var dir = new DirectoryInfo(startDirectory);
                for (int i = 0; i < 12 && dir != null; i++)
                {
                    var candidate = Path.Combine(dir.FullName, "ZPL2PDF.csproj");
                    if (File.Exists(candidate))
                    {
                        return dir.FullName;
                    }

                    dir = dir.Parent;
                }
            }
            catch
            {
                // Ignore and fall back.
            }

            return null;
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
