using System;
using System.IO;
using System.Threading.Tasks;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF.Infrastructure.TcpServer
{
    /// <summary>
    /// Manages the TCP Server lifecycle, including start/stop/status operations.
    /// Uses a separate PID file from Daemon mode to allow independent operation.
    /// </summary>
    public class TcpServerManager : IDisposable
    {
        private TcpPrinterServer? _server;
        private readonly string _pidFilePath;
        private const string PidFileName = "zpl2pdf-tcpserver.pid";

        /// <summary>
        /// Gets whether the TCP server is currently running.
        /// </summary>
        public bool IsRunning => _server?.IsRunning ?? false;

        /// <summary>
        /// Initializes a new instance of the TcpServerManager.
        /// </summary>
        public TcpServerManager()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var zpl2pdfPath = Path.Combine(appDataPath, "ZPL2PDF");
            Directory.CreateDirectory(zpl2pdfPath);
            _pidFilePath = Path.Combine(zpl2pdfPath, PidFileName);
        }

        /// <summary>
        /// Starts the TCP server.
        /// </summary>
        /// <param name="port">Port to listen on.</param>
        /// <param name="outputDirectory">Directory for output PDFs.</param>
        /// <param name="rendererMode">Renderer mode to use.</param>
        /// <param name="widthMm">Default label width in mm.</param>
        /// <param name="heightMm">Default label height in mm.</param>
        /// <param name="dpi">Default DPI.</param>
        /// <param name="background">Whether to run in background mode.</param>
        /// <returns>True if started successfully, false otherwise.</returns>
        public async Task<bool> StartAsync(
            int port = TcpPrinterServer.DefaultPort,
            string? outputDirectory = null,
            RendererMode rendererMode = RendererMode.Offline,
            double widthMm = 100,
            double heightMm = 150,
            int dpi = 203,
            bool background = true)
        {
            // Check if already running
            if (IsServerRunning())
            {
                Console.WriteLine("TCP Server is already running.");
                return false;
            }

            try
            {
                _server = new TcpPrinterServer(
                    port,
                    outputDirectory,
                    rendererMode,
                    widthMm,
                    heightMm,
                    dpi);

                // Subscribe to events
                _server.ConnectionReceived += OnConnectionReceived;
                _server.LabelProcessed += OnLabelProcessed;
                _server.ErrorOccurred += OnErrorOccurred;

                if (background)
                {
                    _server.StartBackground();
                    WritePidFile();
                    return true;
                }
                else
                {
                    WritePidFile();
                    await _server.StartAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start TCP Server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stops the TCP server.
        /// </summary>
        /// <returns>True if stopped successfully, false otherwise.</returns>
        public async Task<bool> StopAsync()
        {
            if (_server == null || !_server.IsRunning)
            {
                // Check if running in another process
                if (File.Exists(_pidFilePath))
                {
                    Console.WriteLine("TCP Server appears to be running in another process.");
                    Console.WriteLine("Removing PID file...");
                    DeletePidFile();
                    return true;
                }

                Console.WriteLine("TCP Server is not running.");
                return false;
            }

            try
            {
                await _server.StopAsync();
                DeletePidFile();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping TCP Server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the status of the TCP server.
        /// </summary>
        /// <returns>Server status information.</returns>
        public TcpServerStatus GetStatus()
        {
            if (_server != null)
            {
                return _server.GetStatus();
            }

            // Check if running in another process
            if (File.Exists(_pidFilePath))
            {
                try
                {
                    var pidContent = File.ReadAllText(_pidFilePath);
                    var parts = pidContent.Split('|');
                    
                    return new TcpServerStatus
                    {
                        IsRunning = true,
                        Port = parts.Length > 0 ? int.Parse(parts[0]) : TcpPrinterServer.DefaultPort,
                        OutputDirectory = parts.Length > 1 ? parts[1] : "Unknown",
                        RendererName = "Unknown (external process)",
                        TotalConnections = -1
                    };
                }
                catch
                {
                    return new TcpServerStatus { IsRunning = true };
                }
            }

            return new TcpServerStatus { IsRunning = false };
        }

        /// <summary>
        /// Checks if the server is running (either in this process or another).
        /// </summary>
        /// <returns>True if running, false otherwise.</returns>
        public bool IsServerRunning()
        {
            if (_server?.IsRunning == true)
                return true;

            return File.Exists(_pidFilePath);
        }

        /// <summary>
        /// Writes the PID file.
        /// </summary>
        private void WritePidFile()
        {
            if (_server == null) return;

            try
            {
                var content = $"{_server.Port}|{_server.OutputDirectory}|{Environment.ProcessId}";
                File.WriteAllText(_pidFilePath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not write PID file: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the PID file.
        /// </summary>
        private void DeletePidFile()
        {
            try
            {
                if (File.Exists(_pidFilePath))
                {
                    File.Delete(_pidFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not delete PID file: {ex.Message}");
            }
        }

        /// <summary>
        /// Event handler for connection received.
        /// </summary>
        private void OnConnectionReceived(object? sender, TcpConnectionEventArgs e)
        {
            // Can be used for logging or notifications
        }

        /// <summary>
        /// Event handler for label processed.
        /// </summary>
        private void OnLabelProcessed(object? sender, TcpLabelProcessedEventArgs e)
        {
            // Can be used for logging or notifications
        }

        /// <summary>
        /// Event handler for errors.
        /// </summary>
        private void OnErrorOccurred(object? sender, TcpErrorEventArgs e)
        {
            // Can be used for logging or notifications
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_server != null)
            {
                _server.ConnectionReceived -= OnConnectionReceived;
                _server.LabelProcessed -= OnLabelProcessed;
                _server.ErrorOccurred -= OnErrorOccurred;
                _server.Dispose();
            }
            DeletePidFile();
        }
    }
}

