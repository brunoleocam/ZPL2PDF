using System;
using System.IO;
using ZPL2PDF.Shared.Constants;
using ZPL2PDF.Shared.Localization;

namespace ZPL2PDF
{
    /// <summary>
    /// Manages the ZPL2PDF daemon, including start/stop/status and PID control
    /// </summary>
    public class DaemonManager
    {
        private readonly PidManager _pidManager;
        private readonly ProcessManager _processManager;
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
            
            _pidManager = new PidManager();
            _processManager = new ProcessManager();
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
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DAEMON_ALREADY_RUNNING));
                    var currentPid = _pidManager.GetPidFromFile();
                    Console.WriteLine($"   PID: {currentPid}");
                    
                    // Read actual daemon configuration
                    var daemonInfo = GetDaemonInfoFromFile();
                    if (daemonInfo != null)
                    {
                        Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.MONITORING_FOLDER, daemonInfo.ListenFolder)}");
                        Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.DIMENSIONS_INFO, daemonInfo.LabelWidth, daemonInfo.LabelHeight, daemonInfo.Unit)}");
                        var dpi1 = int.TryParse(daemonInfo.PrintDensity, out int parsedDpi1) ? parsedDpi1 : ApplicationConstants.DEFAULT_DPI;
                        Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.PRINT_DENSITY_INFO, ApplicationConstants.ConvertDpiToDpmm(dpi1), dpi1)}");
                        Console.WriteLine($"   Started at: {daemonInfo.StartTime}");
                    }
                    else
                    {
                        // Fallback to old configuration
                        Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.MONITORING_FOLDER, _listenFolder)}");
                        Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.DIMENSIONS_INFO, _labelWidth, _labelHeight, _unit)}");
                        var dpi2 = int.TryParse(_printDensity, out int parsedDpi2) ? parsedDpi2 : ApplicationConstants.DEFAULT_DPI;
                        Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.PRINT_DENSITY_INFO, ApplicationConstants.ConvertDpiToDpmm(dpi2), dpi2)}");
                    }
                    return false;
                }

                // Create monitoring folder if it doesn't exist
                if (!Directory.Exists(_listenFolder))
                {
                    Directory.CreateDirectory(_listenFolder);
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.FOLDER_CREATED, _listenFolder));
                }

                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.STARTING_DAEMON));
                Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.MONITORING_FOLDER, _listenFolder)}");
                Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.DIMENSIONS_INFO, _labelWidth, _labelHeight, _unit)}");
                var dpi3 = int.TryParse(_printDensity, out int parsedDpi3) ? parsedDpi3 : ApplicationConstants.DEFAULT_DPI;
                Console.WriteLine($"   {LocalizationManager.GetString(ResourceKeys.PRINT_DENSITY_INFO, ApplicationConstants.ConvertDpiToDpmm(dpi3), dpi3)}");

                // Create background process
                var arguments = BuildDaemonArguments();
                var pid = _processManager.StartBackgroundProcess(arguments);
                
                if (pid > 0)
                {
                    // Save PID to file
                    if (_pidManager.SavePidToFile(pid))
                    {
                        // Save daemon info to file
                        SaveDaemonInfoToFile(pid);
                        Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DAEMON_STARTED_SUCCESS, pid));
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DAEMON_STARTED_BUT_FAILED_PID));
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.FAILED_TO_START_DAEMON));
                    return false;
                }
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
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DAEMON_NOT_RUNNING));
                    return false;
                }

                var pid = _pidManager.GetPidFromFile();
                if (pid > 0)
                {
                    if (_processManager.KillProcess(pid))
                    {
                        // Remove PID file
                        _pidManager.RemovePidFile();
                        Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DAEMON_STOPPED_SUCCESS));
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(LocalizationManager.GetString(ResourceKeys.ERROR_STOPPING_DAEMON));
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.INVALID_PID_IN_FILE));
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
        /// Gets the daemon status
        /// </summary>
        /// <returns>True if running, False otherwise</returns>
        public bool Status()
        {
            try
            {
                if (!IsRunning())
                {
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.DAEMON_NOT_RUNNING));
                    return false;
                }

                var pid = _pidManager.GetPidFromFile();
                Console.WriteLine($"Daemon is running! PID: {pid}");
                
                // Read actual daemon configuration
                var daemonInfo = GetDaemonInfoFromFile();
                if (daemonInfo != null)
                {
                    Console.WriteLine($"   Monitored folder: {daemonInfo.ListenFolder}");
                    Console.WriteLine($"   Dimensions: {daemonInfo.LabelWidth} x {daemonInfo.LabelHeight} {daemonInfo.Unit}");
                    var dpi5 = int.TryParse(daemonInfo.PrintDensity, out int parsedDpi5) ? parsedDpi5 : ApplicationConstants.DEFAULT_DPI;
                    Console.WriteLine($"   Print Density: {ApplicationConstants.ConvertDpiToDpmm(dpi5):F1} dpmm ({dpi5} dpi)");
                    Console.WriteLine($"   Started at: {daemonInfo.StartTime}");
                }
                else
                {
                    // Fallback to old configuration
                    Console.WriteLine($"   Monitored folder: {_listenFolder}");
                    Console.WriteLine($"   Dimensions: {_labelWidth} x {_labelHeight} {_unit}");
                    var dpi4 = int.TryParse(_printDensity, out int parsedDpi4) ? parsedDpi4 : ApplicationConstants.DEFAULT_DPI;
                    Console.WriteLine($"   Print Density: {ApplicationConstants.ConvertDpiToDpmm(dpi4):F1} dpmm ({dpi4} dpi)");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking daemon status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the daemon is running
        /// </summary>
        /// <returns>True if running, False otherwise</returns>
        public bool IsRunning()
        {
            try
            {
                if (!_pidManager.PidFileExists())
                {
                    return false;
                }

                var pid = _pidManager.GetPidFromFile();
                if (pid <= 0)
                {
                    return false;
                }

                return _processManager.IsProcessRunning(pid);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Builds daemon arguments string
        /// </summary>
        /// <returns>Arguments string</returns>
        private string BuildDaemonArguments()
        {
            var arguments = $"run -l \"{_listenFolder}\"";
            
            // Only add dimension parameters if not 0 (extract from ZPL)
            if (double.TryParse(_labelWidth, out double width) && width > 0)
                arguments += $" -w {_labelWidth}";
            if (double.TryParse(_labelHeight, out double height) && height > 0)
                arguments += $" -h {_labelHeight}";
            if (!string.IsNullOrEmpty(_unit))
                arguments += $" -u {_unit}";
            if (int.TryParse(_printDensity, out int dpi) && dpi > 0)
                arguments += $" -d {_printDensity}";
            
            return arguments;
        }

        /// <summary>
        /// Saves daemon info to file
        /// </summary>
        /// <param name="pid">Process ID</param>
        private void SaveDaemonInfoToFile(int pid)
        {
            try
            {
                var daemonInfo = new DaemonInfo
                {
                    ListenFolder = _listenFolder,
                    LabelWidth = _labelWidth,
                    LabelHeight = _labelHeight,
                    Unit = _unit,
                    PrintDensity = _printDensity,
                    StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProcessId = pid
                };

                // Get the directory where the PID file is stored
                var pidFilePath = _pidManager.GetType().GetField("_pidFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_pidManager)?.ToString();
                var infoFilePath = Path.Combine(Path.GetDirectoryName(pidFilePath ?? ""), "zpl2pdf.info");
                
                // Save daemon info to file in a simple format
                var lines = new[]
                {
                    $"ListenFolder={_listenFolder}",
                    $"LabelWidth={_labelWidth}",
                    $"LabelHeight={_labelHeight}",
                    $"Unit={_unit}",
                    $"PrintDensity={_printDensity}",
                    $"StartTime={daemonInfo.StartTime}",
                    $"ProcessId={pid}"
                };
                
                File.WriteAllLines(infoFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not save daemon info: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets daemon info from file
        /// </summary>
        /// <returns>Daemon info or null if not found</returns>
        private DaemonInfo GetDaemonInfoFromFile()
        {
            try
            {
                // Get the directory where the PID file is stored
                var pidFilePath = _pidManager.GetType().GetField("_pidFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_pidManager)?.ToString();
                var infoFilePath = Path.Combine(Path.GetDirectoryName(pidFilePath ?? ""), "zpl2pdf.info");
                
                if (!File.Exists(infoFilePath))
                {
                    return null;
                }
                
                var lines = File.ReadAllLines(infoFilePath);
                var daemonInfo = new DaemonInfo();
                
                foreach (var line in lines)
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        
                        switch (key)
                        {
                            case "ListenFolder":
                                daemonInfo.ListenFolder = value;
                                break;
                            case "LabelWidth":
                                daemonInfo.LabelWidth = value;
                                break;
                            case "LabelHeight":
                                daemonInfo.LabelHeight = value;
                                break;
                            case "Unit":
                                daemonInfo.Unit = value;
                                break;
                            case "PrintDensity":
                                daemonInfo.PrintDensity = value;
                                break;
                            case "StartTime":
                                daemonInfo.StartTime = value;
                                break;
                            case "ProcessId":
                                if (int.TryParse(value, out int pid))
                                {
                                    daemonInfo.ProcessId = pid;
                                }
                                break;
                        }
                    }
                }
                
                return daemonInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}