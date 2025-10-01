using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ZPL2PDF
{
    /// <summary>
    /// Manages the ZPL2PDF daemon, including start/stop/status and PID control
    /// </summary>
    public class DaemonManager
    {
        private readonly string _pidFilePath;
        private readonly string _executablePath;
        private readonly string _listenFolder;
        private readonly string _labelWidth;
        private readonly string _labelHeight;
        private readonly string _unit;
        private readonly string _printDensity;

        /// <summary>
        /// DaemonManager constructor
        /// </summary>
        /// <param name="listenFolder">Folder to monitor</param>
        /// <param name="labelWidth">Label width</param>
        /// <param name="labelHeight">Label height</param>
        /// <param name="unit">Unit of measurement (mm, in, pt)</param>
        /// <param name="printDensity">Print density (DPI)</param>
        public DaemonManager(string listenFolder, string labelWidth, string labelHeight, string unit, string printDensity)
        {
            _listenFolder = listenFolder;
            _labelWidth = labelWidth;
            _labelHeight = labelHeight;
            _unit = unit;
            _printDensity = printDensity;
            
            // Determine executable path (use current executable)
            _executablePath = Environment.ProcessPath ?? 
                Path.Combine(AppContext.BaseDirectory, 
                    Environment.OSVersion.Platform == PlatformID.Win32NT ? "ZPL2PDF.exe" : "ZPL2PDF");
            
            // Determine PID file location based on OS
            _pidFilePath = GetPidFilePath();
        }

        /// <summary>
        /// Starts the daemon
        /// </summary>
        /// <returns>True if started successfully, False otherwise</returns>
        public bool Start()
        {
            try
            {
                // Check if already running
                if (IsRunning())
                {
                    Console.WriteLine("Daemon is already running!");
                    var pid = GetPidFromFile();
                    Console.WriteLine($"   PID: {pid}");
                    
                    // Read actual daemon configuration
                    var daemonInfo = GetDaemonInfoFromFile();
                    if (daemonInfo != null)
                    {
                        Console.WriteLine($"   Monitored folder: {daemonInfo.ListenFolder}");
                        Console.WriteLine($"   Dimensions: {daemonInfo.LabelWidth} x {daemonInfo.LabelHeight} {daemonInfo.Unit}");
                        Console.WriteLine($"   DPI: {daemonInfo.PrintDensity}");
                        Console.WriteLine($"   Started at: {daemonInfo.StartTime}");
                    }
                    else
                    {
                        // Fallback to old configuration
                        Console.WriteLine($"   Monitored folder: {_listenFolder}");
                        Console.WriteLine($"   Dimensions: {_labelWidth} x {_labelHeight} {_unit}");
                        Console.WriteLine($"   DPI: {_printDensity}");
                    }
                    return false;
                }

                // Create monitoring folder if it doesn't exist
                if (!Directory.Exists(_listenFolder))
                {
                    Directory.CreateDirectory(_listenFolder);
                    Console.WriteLine($"Folder created: {_listenFolder}");
                }

                Console.WriteLine("Starting daemon in background...");
                Console.WriteLine($"   Monitored folder: {_listenFolder}");
                Console.WriteLine($"   Dimensions: {_labelWidth} x {_labelHeight} {_unit}");
                Console.WriteLine($"   DPI: {_printDensity}");

                // Create background process
                var arguments = $"run -l \"{_listenFolder}\"";
                
                // Only add dimension parameters if not 0 (extract from ZPL)
                if (double.TryParse(_labelWidth, out double width) && width > 0)
                    arguments += $" -w {_labelWidth}";
                if (double.TryParse(_labelHeight, out double height) && height > 0)
                    arguments += $" -h {_labelHeight}";
                if (!string.IsNullOrEmpty(_unit))
                    arguments += $" -u {_unit}";
                if (int.TryParse(_printDensity, out int density) && density > 0)
                    arguments += $" -d {_printDensity}";
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = _executablePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                var process = Process.Start(startInfo);
                if (process == null)
                {
                    Console.WriteLine("Error starting daemon process!");
                    return false;
                }

                // Wait for daemon to create PID file
                var pidFilePath = GetPidFilePath();
                var maxWaitTime = 30; // 30 seconds
                var waitTime = 0;
                
                while (waitTime < maxWaitTime)
                {
                    if (File.Exists(pidFilePath))
                    {
                        Thread.Sleep(1000); // Wait 1 more second to ensure PID was saved
                        break;
                    }
                    Thread.Sleep(1000);
                    waitTime++;
                }

                if (!File.Exists(pidFilePath))
                {
                    Console.WriteLine("Daemon did not create PID file after initialization!");
                    return false;
                }

                Console.WriteLine("Daemon started successfully!");
                Console.WriteLine($"   PID: {GetPidFromFile()}");
                Console.WriteLine($"   Monitored folder: {_listenFolder}");
                Console.WriteLine($"   Dimensions: {_labelWidth} x {_labelHeight} {_unit}");
                Console.WriteLine($"   DPI: {_printDensity}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting daemon: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stops the daemon
        /// </summary>
        /// <returns>True if stopped successfully, False otherwise</returns>
        public bool Stop()
        {
            try
            {
                if (!IsRunning())
                {
                    Console.WriteLine("Daemon is not running!");
                    return false;
                }

                var pid = GetPidFromFile();
                if (pid > 0)
                {
                    try
                    {
                        var process = Process.GetProcessById(pid);
                        process.Kill();
                        process.WaitForExit(5000); // Wait up to 5 seconds
                        
                        // Remove PID file
                        RemovePidFile();
                        
                        Console.WriteLine("Daemon stopped successfully!");
                        return true;
                    }
                    catch (ArgumentException)
                    {
                        // Process no longer exists
                        RemovePidFile();
                        Console.WriteLine("Daemon was already stopped (PID file removed)");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error stopping daemon: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid PID in file!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping daemon: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks daemon status
        /// </summary>
        /// <returns>True if running, False otherwise</returns>
        public bool Status()
        {
            try
            {
                if (IsRunning())
                {
                    var pid = GetPidFromFile();
                    Console.WriteLine("Daemon is running!");
                    Console.WriteLine($"   PID: {pid}");
                    
                    // Read configuration from PID file
                    var daemonInfo = GetDaemonInfoFromFile();
                    if (daemonInfo != null)
                    {
                        Console.WriteLine($"   Monitored folder: {daemonInfo.ListenFolder}");
                        Console.WriteLine($"   Dimensions: {daemonInfo.LabelWidth} x {daemonInfo.LabelHeight} {daemonInfo.Unit}");
                        Console.WriteLine($"   DPI: {daemonInfo.PrintDensity}");
                        Console.WriteLine($"   Started at: {daemonInfo.StartTime}");
                    }
                    else
                    {
                        // Fallback to old configuration
                        Console.WriteLine($"   Monitored folder: {_listenFolder}");
                        Console.WriteLine($"   Dimensions: {_labelWidth} x {_labelHeight} {_unit}");
                        Console.WriteLine($"   DPI: {_printDensity}");
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("Daemon is not running!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if daemon is running
        /// </summary>
        /// <returns>True if running, False otherwise</returns>
        public bool IsRunning()
        {
            try
            {
                if (!File.Exists(_pidFilePath))
                    return false;

                var pid = GetPidFromFile();
                if (pid <= 0)
                    return false;

                // Check if process exists
                try
                {
                    var process = Process.GetProcessById(pid);
                    return !process.HasExited;
                }
                catch (ArgumentException)
                {
                    // Process no longer exists, remove PID file
                    RemovePidFile();
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets PID from file
        /// </summary>
        /// <returns>PID or -1 if invalid</returns>
        private int GetPidFromFile()
        {
            try
            {
                if (File.Exists(_pidFilePath))
                {
                    var content = File.ReadAllText(_pidFilePath).Trim();
                    
                    // Try to read as JSON first (new format)
                    try
                    {
                        var daemonInfo = System.Text.Json.JsonSerializer.Deserialize<DaemonInfo>(content);
                        return daemonInfo?.Pid ?? -1;
                    }
                    catch
                    {
                        // If it fails, try to read as simple PID (old format)
                        if (int.TryParse(content, out int pid))
                            return pid;
                    }
                }
            }
            catch
            {
                // Ignore reading errors
            }
            return -1;
        }


        /// <summary>
        /// Removes the PID file
        /// </summary>
        private void RemovePidFile()
        {
            try
            {
                if (File.Exists(_pidFilePath))
                    File.Delete(_pidFilePath);
            }
            catch
            {
                // Ignore removal errors
            }
        }

        /// <summary>
        /// Determines PID file path based on OS
        /// </summary>
        /// <returns>PID file path</returns>
        private string GetPidFilePath()
        {
            var fileName = "zpl2pdf-daemon.pid";
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows: %TEMP%
                var tempPath = Path.GetTempPath();
                return Path.Combine(tempPath, fileName);
            }
            else
            {
                // Linux: /var/run
                return Path.Combine("/var/run", fileName);
            }
        }

        /// <summary>
        /// Builds arguments for daemon process
        /// </summary>
        /// <returns>Arguments string</returns>
        private string BuildDaemonArguments()
        {
            var args = new System.Text.StringBuilder();
            args.Append("daemon-run"); // Internal command for daemon mode
            args.Append($" -l \"{_listenFolder}\"");
            args.Append($" -w {_labelWidth}");
            args.Append($" -h {_labelHeight}");
            args.Append($" -u {_unit}");
            args.Append($" -d {_printDensity}");
            
            return args.ToString();
        }

        /// <summary>
        /// Reads daemon information from PID file
        /// </summary>
        /// <returns>Daemon information or null if unable to read</returns>
        private DaemonInfo? GetDaemonInfoFromFile()
        {
            try
            {
                if (!File.Exists(_pidFilePath))
                {
                    return null;
                }

                var json = File.ReadAllText(_pidFilePath);
                
                // Try to deserialize as JSON (new format)
                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<DaemonInfo>(json);
                }
                catch
                {
                    // If it fails, try to read as simple PID (old format)
                    if (int.TryParse(json.Trim(), out int pid))
                    {
                        return new DaemonInfo
                        {
                            Pid = pid,
                            ListenFolder = _listenFolder,
                            LabelWidth = double.TryParse(_labelWidth, out double width) ? width : 0,
                            LabelHeight = double.TryParse(_labelHeight, out double height) ? height : 0,
                            Unit = _unit,
                            PrintDensity = int.TryParse(_printDensity, out int density) ? density : 203,
                            StartTime = "Unknown"
                        };
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Daemon information saved in PID file
    /// </summary>
    public class DaemonInfo
    {
        public int Pid { get; set; }
        public string ListenFolder { get; set; } = "";
        public double LabelWidth { get; set; }
        public double LabelHeight { get; set; }
        public string Unit { get; set; } = "";
        public int PrintDensity { get; set; }
        public string StartTime { get; set; } = "";
    }
}
