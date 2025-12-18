using System;
using System.Threading.Tasks;
using ZPL2PDF.Infrastructure.TcpServer;
using ZPL2PDF.Infrastructure.Rendering;

namespace ZPL2PDF
{
    /// <summary>
    /// Handles TCP Server mode operations (start, stop, status).
    /// </summary>
    public class TcpServerModeHandler
    {
        private readonly TcpServerManager _serverManager;

        /// <summary>
        /// Initializes a new instance of the TcpServerModeHandler.
        /// </summary>
        public TcpServerModeHandler()
        {
            _serverManager = new TcpServerManager();
        }

        /// <summary>
        /// Handles the TCP server command.
        /// </summary>
        /// <param name="command">Command to execute (start, stop, status).</param>
        /// <param name="args">TCP server arguments.</param>
        /// <returns>Exit code (0 for success, non-zero for error).</returns>
        public async Task<int> HandleCommandAsync(string command, TcpServerArguments args)
        {
            switch (command.ToLowerInvariant())
            {
                case "start":
                    return await HandleStartAsync(args);

                case "stop":
                    return await HandleStopAsync();

                case "status":
                    return HandleStatus();

                default:
                    Console.WriteLine($"Unknown TCP server command: {command}");
                    Console.WriteLine("Available commands: start, stop, status");
                    return 1;
            }
        }

        /// <summary>
        /// Handles the start command.
        /// </summary>
        private async Task<int> HandleStartAsync(TcpServerArguments args)
        {
            Console.WriteLine("Starting TCP Server...");
            Console.WriteLine($"  Port: {args.Port}");
            Console.WriteLine($"  Output: {args.OutputDirectory ?? "default"}");
            Console.WriteLine($"  Renderer: {args.RendererMode}");
            Console.WriteLine($"  Dimensions: {args.WidthMm}mm x {args.HeightMm}mm @ {args.Dpi} DPI");
            Console.WriteLine();

            var success = await _serverManager.StartAsync(
                args.Port,
                args.OutputDirectory,
                args.RendererMode,
                args.WidthMm,
                args.HeightMm,
                args.Dpi,
                args.Background);

            if (success)
            {
                if (args.Background)
                {
                    Console.WriteLine("TCP Server started in background.");
                    Console.WriteLine("Use 'ZPL2PDF server stop' to stop the server.");
                    return 0;
                }
                else
                {
                    // Server is running in foreground, wait for Ctrl+C
                    Console.WriteLine("Press Ctrl+C to stop the server...");
                    Console.CancelKeyPress += async (s, e) =>
                    {
                        e.Cancel = true;
                        await _serverManager.StopAsync();
                    };

                    // Keep running until stopped
                    while (_serverManager.IsRunning)
                    {
                        await Task.Delay(1000);
                    }
                    return 0;
                }
            }

            return 1;
        }

        /// <summary>
        /// Handles the stop command.
        /// </summary>
        private async Task<int> HandleStopAsync()
        {
            Console.WriteLine("Stopping TCP Server...");

            var success = await _serverManager.StopAsync();
            return success ? 0 : 1;
        }

        /// <summary>
        /// Handles the status command.
        /// </summary>
        private int HandleStatus()
        {
            var status = _serverManager.GetStatus();

            Console.WriteLine("TCP Server Status:");
            Console.WriteLine($"  Running: {(status.IsRunning ? "Yes" : "No")}");

            if (status.IsRunning)
            {
                Console.WriteLine($"  Port: {status.Port}");
                Console.WriteLine($"  Output Directory: {status.OutputDirectory}");
                Console.WriteLine($"  Renderer: {status.RendererName}");
                if (status.TotalConnections >= 0)
                {
                    Console.WriteLine($"  Total Connections: {status.TotalConnections}");
                }
            }

            return 0;
        }
    }

    /// <summary>
    /// Arguments for TCP server operations.
    /// </summary>
    public class TcpServerArguments
    {
        /// <summary>
        /// TCP port to listen on.
        /// </summary>
        public int Port { get; set; } = TcpPrinterServer.DefaultPort;

        /// <summary>
        /// Output directory for generated PDFs.
        /// </summary>
        public string? OutputDirectory { get; set; }

        /// <summary>
        /// Renderer mode to use.
        /// </summary>
        public RendererMode RendererMode { get; set; } = RendererMode.Offline;

        /// <summary>
        /// Default label width in mm.
        /// </summary>
        public double WidthMm { get; set; } = 100;

        /// <summary>
        /// Default label height in mm.
        /// </summary>
        public double HeightMm { get; set; } = 150;

        /// <summary>
        /// Default DPI.
        /// </summary>
        public int Dpi { get; set; } = 203;

        /// <summary>
        /// Whether to run in background mode.
        /// </summary>
        public bool Background { get; set; } = true;
    }
}

