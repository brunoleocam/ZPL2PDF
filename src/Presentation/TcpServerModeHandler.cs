using System;
using System.IO;
using System.Threading;

namespace ZPL2PDF
{
    /// <summary>
    /// Handles TCP Server mode: start, stop, status.
    /// </summary>
    public class TcpServerModeHandler
    {
        private const string TcpPidFileName = "zpl2pdf-tcp.pid";

        /// <summary>
        /// Handles the server subcommand (start, stop, status).
        /// </summary>
        /// <param name="args">Processed arguments with ServerCommand, ServerPort, ServerOutputFolder, ServerForeground.</param>
        public void HandleServer(ArgumentProcessor args)
        {
            switch (args.ServerCommand?.ToLowerInvariant() ?? "start")
            {
                case "start":
                    HandleStart(args);
                    break;
                case "stop":
                    HandleStop();
                    break;
                case "status":
                    HandleStatus();
                    break;
                default:
                    Console.WriteLine("Error: Unknown server command. Use start, stop, or status.");
                    Environment.Exit(1);
                    break;
            }
        }

        private void HandleStart(ArgumentProcessor args)
        {
            if (string.IsNullOrWhiteSpace(args.ServerOutputFolder))
            {
                Console.WriteLine("Error: Output folder (-o) is required for server start.");
                Environment.Exit(1);
            }

            var pidManager = new PidManager(TcpPidFileName);
            if (pidManager.PidFileExists() && IsServerProcessRunning(pidManager))
            {
                var pid = pidManager.GetPidFromFile();
                Console.WriteLine($"TCP server is already running (PID: {pid}). Use 'ZPL2PDF server stop' to stop it.");
                return;
            }

            if (args.ServerForeground)
            {
                RunServerInForeground(args.ServerPort, args.ServerOutputFolder);
                return;
            }

            // Background: spawn process with --foreground so the child runs the server
            var processManager = new ProcessManager();
            var serverArgs = BuildServerArguments(args.ServerPort, args.ServerOutputFolder, foreground: true);
            var startedPid = processManager.StartBackgroundProcess(serverArgs);
            if (startedPid <= 0)
            {
                Console.WriteLine("Error: Failed to start TCP server process.");
                Environment.Exit(1);
            }
            if (!pidManager.SavePidToFile(startedPid))
            {
                Console.WriteLine("Warning: Server started but could not save PID file.");
            }
            Console.WriteLine($"TCP server started in background (PID: {startedPid}). Listening on port {args.ServerPort}, output: {args.ServerOutputFolder}");
            Console.WriteLine("Use 'ZPL2PDF server stop' to stop.");
        }

        private void HandleStop()
        {
            var pidManager = new PidManager(TcpPidFileName);
            if (!pidManager.PidFileExists())
            {
                Console.WriteLine("TCP server is not running (no PID file).");
                return;
            }
            var pid = pidManager.GetPidFromFile();
            if (pid <= 0)
            {
                Console.WriteLine("TCP server is not running (invalid PID file).");
                pidManager.RemovePidFile();
                return;
            }
            var processManager = new ProcessManager();
            if (processManager.KillProcess(pid))
            {
                pidManager.RemovePidFile();
                Console.WriteLine("TCP server stopped.");
            }
            else
            {
                Console.WriteLine("TCP server process was not running; PID file removed.");
                pidManager.RemovePidFile();
            }
        }

        private void HandleStatus()
        {
            var pidManager = new PidManager(TcpPidFileName);
            if (!pidManager.PidFileExists())
            {
                Console.WriteLine("TCP server is not running.");
                return;
            }
            var pid = pidManager.GetPidFromFile();
            if (pid <= 0)
            {
                Console.WriteLine("TCP server is not running (invalid PID file).");
                return;
            }
            if (IsServerProcessRunning(pidManager))
            {
                Console.WriteLine($"TCP server is running (PID: {pid}).");
            }
            else
            {
                Console.WriteLine("TCP server is not running (process not found). PID file may be stale.");
            }
        }

        private static bool IsServerProcessRunning(PidManager pidManager)
        {
            var pid = pidManager.GetPidFromFile();
            if (pid <= 0) return false;
            try
            {
                using (var process = System.Diagnostics.Process.GetProcessById(pid))
                {
                    return !process.HasExited;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static void RunServerInForeground(int port, string outputFolder)
        {
            var server = new TcpPrinterServer(port, outputFolder);
            Console.WriteLine($"TCP server listening on port {port}. Output folder: {outputFolder}");
            Console.WriteLine("Press Ctrl+C to stop.");
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                server.Stop(); // Unblock AcceptTcpClient so Run can exit
            };
            try
            {
                server.Run(cts.Token);
            }
            finally
            {
                server.Stop();
            }
        }

        private static string BuildServerArguments(int port, string outputFolder, bool foreground)
        {
            var output = outputFolder.Contains(" ") ? $"\"{outputFolder}\"" : outputFolder;
            var args = $"server start --port {port} -o {output}";
            if (foreground)
                args += " --foreground";
            return args;
        }
    }
}
