using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF.Infrastructure.TcpServer
{
    /// <summary>
    /// TCP Server that acts as a virtual Zebra printer.
    /// Listens for ZPL data on a configurable port and converts to PDF.
    /// This mode is independent of the Daemon mode and can run simultaneously.
    /// </summary>
    public class TcpPrinterServer : IDisposable
    {
        private TcpListener? _listener;
        private CancellationTokenSource? _cts;
        private Task? _listenerTask;
        private readonly int _port;
        private readonly string _outputDirectory;
        private readonly ILabelRenderer _renderer;
        private readonly double _defaultWidthMm;
        private readonly double _defaultHeightMm;
        private readonly int _defaultDpi;
        private bool _isRunning;
        private int _connectionCount;

        /// <summary>
        /// Default TCP port for ZPL printers (9101 to avoid conflict with 9100).
        /// </summary>
        public const int DefaultPort = 9101;

        /// <summary>
        /// Event fired when a connection is received.
        /// </summary>
        public event EventHandler<TcpConnectionEventArgs>? ConnectionReceived;

        /// <summary>
        /// Event fired when a label is processed.
        /// </summary>
        public event EventHandler<TcpLabelProcessedEventArgs>? LabelProcessed;

        /// <summary>
        /// Event fired when an error occurs.
        /// </summary>
        public event EventHandler<TcpErrorEventArgs>? ErrorOccurred;

        /// <summary>
        /// Gets whether the server is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Gets the port the server is listening on.
        /// </summary>
        public int Port => _port;

        /// <summary>
        /// Gets the output directory for generated PDFs.
        /// </summary>
        public string OutputDirectory => _outputDirectory;

        /// <summary>
        /// Gets the total number of connections handled.
        /// </summary>
        public int TotalConnections => _connectionCount;

        /// <summary>
        /// Initializes a new instance of the TcpPrinterServer.
        /// </summary>
        /// <param name="port">TCP port to listen on (default: 9101).</param>
        /// <param name="outputDirectory">Directory to save generated PDFs.</param>
        /// <param name="rendererMode">Renderer mode to use (offline, labelary, auto).</param>
        /// <param name="defaultWidthMm">Default label width in mm.</param>
        /// <param name="defaultHeightMm">Default label height in mm.</param>
        /// <param name="defaultDpi">Default print density in DPI.</param>
        public TcpPrinterServer(
            int port = DefaultPort,
            string? outputDirectory = null,
            RendererMode rendererMode = RendererMode.Offline,
            double defaultWidthMm = 100,
            double defaultHeightMm = 150,
            int defaultDpi = 203)
        {
            _port = port;
            _outputDirectory = outputDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ZPL2PDF TCP Output");
            _renderer = RendererFactory.Create(rendererMode);
            _defaultWidthMm = defaultWidthMm;
            _defaultHeightMm = defaultHeightMm;
            _defaultDpi = defaultDpi;
            _isRunning = false;
            _connectionCount = 0;
        }

        /// <summary>
        /// Starts the TCP server.
        /// </summary>
        /// <returns>Task representing the async operation.</returns>
        public async Task StartAsync()
        {
            if (_isRunning)
            {
                Console.WriteLine("TCP Server is already running.");
                return;
            }

            // Ensure output directory exists
            Directory.CreateDirectory(_outputDirectory);

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _port);

            try
            {
                _listener.Start();
                _isRunning = true;

                Console.WriteLine($"TCP Server started on port {_port}");
                Console.WriteLine($"Output directory: {_outputDirectory}");
                Console.WriteLine($"Renderer: {_renderer.Name}");
                Console.WriteLine("Waiting for connections...");

                _listenerTask = AcceptConnectionsAsync(_cts.Token);
                await _listenerTask;
            }
            catch (SocketException ex)
            {
                _isRunning = false;
                throw new InvalidOperationException($"Failed to start TCP server on port {_port}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Starts the TCP server in background mode (non-blocking).
        /// </summary>
        public void StartBackground()
        {
            if (_isRunning)
            {
                Console.WriteLine("TCP Server is already running.");
                return;
            }

            // Ensure output directory exists
            Directory.CreateDirectory(_outputDirectory);

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _port);

            try
            {
                _listener.Start();
                _isRunning = true;

                Console.WriteLine($"TCP Server started on port {_port}");
                Console.WriteLine($"Output directory: {_outputDirectory}");
                Console.WriteLine($"Renderer: {_renderer.Name}");

                // Start accepting connections in background
                _listenerTask = Task.Run(() => AcceptConnectionsAsync(_cts.Token));
            }
            catch (SocketException ex)
            {
                _isRunning = false;
                throw new InvalidOperationException($"Failed to start TCP server on port {_port}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Stops the TCP server.
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                Console.WriteLine("TCP Server is not running.");
                return;
            }

            Console.WriteLine("Stopping TCP Server...");

            _cts?.Cancel();
            _listener?.Stop();
            _isRunning = false;

            if (_listenerTask != null)
            {
                try
                {
                    await _listenerTask;
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancelling
                }
            }

            Console.WriteLine("TCP Server stopped.");
        }

        /// <summary>
        /// Accepts incoming TCP connections.
        /// </summary>
        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _listener != null)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Interlocked.Increment(ref _connectionCount);

                    var remoteEndpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
                    Console.WriteLine($"Connection #{_connectionCount} from {remoteEndpoint}");

                    ConnectionReceived?.Invoke(this, new TcpConnectionEventArgs(remoteEndpoint, _connectionCount));

                    // Handle connection in background (fire and forget)
                    _ = HandleClientAsync(client, _connectionCount, cancellationToken);
                }
                catch (ObjectDisposedException)
                {
                    // Listener was stopped
                    break;
                }
                catch (SocketException) when (cancellationToken.IsCancellationRequested)
                {
                    // Expected when stopping
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting connection: {ex.Message}");
                    ErrorOccurred?.Invoke(this, new TcpErrorEventArgs(ex.Message, null));
                }
            }
        }

        /// <summary>
        /// Handles an individual client connection.
        /// </summary>
        private async Task HandleClientAsync(TcpClient client, int connectionId, CancellationToken cancellationToken)
        {
            var remoteEndpoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";

            try
            {
                using (client)
                using (var stream = client.GetStream())
                {
                    // Set read timeout
                    stream.ReadTimeout = 30000; // 30 seconds

                    // Read all data from the stream
                    var zplData = await ReadAllDataAsync(stream, cancellationToken);

                    if (string.IsNullOrWhiteSpace(zplData))
                    {
                        Console.WriteLine($"Connection #{connectionId}: No data received");
                        return;
                    }

                    Console.WriteLine($"Connection #{connectionId}: Received {zplData.Length} bytes");

                    // Process the ZPL data
                    await ProcessZplDataAsync(zplData, connectionId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection #{connectionId} error: {ex.Message}");
                ErrorOccurred?.Invoke(this, new TcpErrorEventArgs(ex.Message, remoteEndpoint));
            }
        }

        /// <summary>
        /// Reads all data from the network stream.
        /// </summary>
        private async Task<string> ReadAllDataAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            var buffer = new byte[8192];
            var dataBuilder = new MemoryStream();
            int bytesRead;

            // Read until no more data or timeout
            while (stream.DataAvailable || dataBuilder.Length == 0)
            {
                try
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0)
                        break;

                    dataBuilder.Write(buffer, 0, bytesRead);

                    // Small delay to allow more data to arrive
                    if (!stream.DataAvailable)
                        await Task.Delay(100, cancellationToken);
                }
                catch (IOException)
                {
                    // Timeout or connection closed
                    break;
                }
            }

            return Encoding.UTF8.GetString(dataBuilder.ToArray());
        }

        /// <summary>
        /// Processes ZPL data and generates PDF.
        /// </summary>
        private async Task ProcessZplDataAsync(string zplData, int connectionId)
        {
            try
            {
                // Preprocess ZPL (remove ^FN tags, etc.)
                var processedZpl = LabelFileReader.PreprocessZpl(zplData);

                // Split into labels
                var labels = LabelFileReader.SplitLabels(processedZpl);

                if (labels.Count == 0)
                {
                    Console.WriteLine($"Connection #{connectionId}: No valid ZPL labels found");
                    return;
                }

                Console.WriteLine($"Connection #{connectionId}: Found {labels.Count} label(s)");

                // Try to extract custom file name from ZPL
                var customFileName = LabelFileReader.ExtractFileName(zplData);
                string outputFileName;

                if (!string.IsNullOrEmpty(customFileName))
                {
                    outputFileName = $"{customFileName}.pdf";
                    Console.WriteLine($"Connection #{connectionId}: Using custom file name: {outputFileName}");
                }
                else
                {
                    outputFileName = $"label_{DateTime.Now:yyyyMMdd_HHmmss}_{connectionId}.pdf";
                }

                var outputPath = Path.Combine(_outputDirectory, outputFileName);

                // Render labels to images
                var images = await _renderer.RenderLabelsAsync(labels, _defaultWidthMm, _defaultHeightMm, _defaultDpi);

                if (images.Count == 0)
                {
                    Console.WriteLine($"Connection #{connectionId}: No images generated");
                    return;
                }

                // Generate PDF
                PdfGenerator.GeneratePdf(images, outputPath);

                Console.WriteLine($"Connection #{connectionId}: PDF generated: {outputPath}");

                LabelProcessed?.Invoke(this, new TcpLabelProcessedEventArgs(
                    outputPath,
                    labels.Count,
                    connectionId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection #{connectionId}: Processing error: {ex.Message}");
                ErrorOccurred?.Invoke(this, new TcpErrorEventArgs(ex.Message, null));
            }
        }

        /// <summary>
        /// Gets the server status.
        /// </summary>
        /// <returns>Server status information.</returns>
        public TcpServerStatus GetStatus()
        {
            return new TcpServerStatus
            {
                IsRunning = _isRunning,
                Port = _port,
                OutputDirectory = _outputDirectory,
                RendererName = _renderer.Name,
                TotalConnections = _connectionCount
            };
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        public void Dispose()
        {
            _cts?.Cancel();
            _listener?.Stop();
            _cts?.Dispose();
            _isRunning = false;
        }
    }

    /// <summary>
    /// TCP Server status information.
    /// </summary>
    public class TcpServerStatus
    {
        public bool IsRunning { get; set; }
        public int Port { get; set; }
        public string OutputDirectory { get; set; } = string.Empty;
        public string RendererName { get; set; } = string.Empty;
        public int TotalConnections { get; set; }
    }

    /// <summary>
    /// Event args for TCP connection events.
    /// </summary>
    public class TcpConnectionEventArgs : EventArgs
    {
        public string RemoteEndpoint { get; }
        public int ConnectionId { get; }

        public TcpConnectionEventArgs(string remoteEndpoint, int connectionId)
        {
            RemoteEndpoint = remoteEndpoint;
            ConnectionId = connectionId;
        }
    }

    /// <summary>
    /// Event args for label processed events.
    /// </summary>
    public class TcpLabelProcessedEventArgs : EventArgs
    {
        public string OutputPath { get; }
        public int LabelCount { get; }
        public int ConnectionId { get; }

        public TcpLabelProcessedEventArgs(string outputPath, int labelCount, int connectionId)
        {
            OutputPath = outputPath;
            LabelCount = labelCount;
            ConnectionId = connectionId;
        }
    }

    /// <summary>
    /// Event args for TCP error events.
    /// </summary>
    public class TcpErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; }
        public string? RemoteEndpoint { get; }

        public TcpErrorEventArgs(string errorMessage, string? remoteEndpoint)
        {
            ErrorMessage = errorMessage;
            RemoteEndpoint = remoteEndpoint;
        }
    }
}

