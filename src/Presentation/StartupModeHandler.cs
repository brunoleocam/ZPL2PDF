using System;
using System.Threading.Tasks;
using ZPL2PDF.Infrastructure.Startup;

namespace ZPL2PDF
{
    /// <summary>
    /// Handles Startup mode operations (enable, disable, status).
    /// </summary>
    public class StartupModeHandler
    {
        /// <summary>
        /// Handles the startup command.
        /// </summary>
        /// <param name="command">Command to execute (enable, disable, status).</param>
        /// <param name="args">Startup arguments.</param>
        /// <returns>Exit code (0 for success, non-zero for error).</returns>
        public async Task<int> HandleCommandAsync(string command, StartupArguments args)
        {
            switch (command.ToLowerInvariant())
            {
                case "enable":
                    return await HandleEnableAsync(args);

                case "disable":
                    return await HandleDisableAsync(args);

                case "status":
                    return HandleStatus();

                default:
                    Console.WriteLine($"Unknown startup command: {command}");
                    ShowUsage();
                    return 1;
            }
        }

        /// <summary>
        /// Handles the enable command.
        /// </summary>
        private async Task<int> HandleEnableAsync(StartupArguments args)
        {
            var manager = StartupManagerFactory.Create();
            if (manager == null)
            {
                Console.WriteLine("Error: Startup management is not supported on this platform.");
                return 1;
            }

            Console.WriteLine($"Enabling startup on {manager.PlatformName}...");
            Console.WriteLine();

            var serviceTypes = ParseServiceTypes(args.ServiceType);
            var success = true;

            foreach (var serviceType in serviceTypes)
            {
                var result = await manager.EnableStartupAsync(serviceType);
                if (!result)
                {
                    success = false;
                }
            }

            return success ? 0 : 1;
        }

        /// <summary>
        /// Handles the disable command.
        /// </summary>
        private async Task<int> HandleDisableAsync(StartupArguments args)
        {
            var manager = StartupManagerFactory.Create();
            if (manager == null)
            {
                Console.WriteLine("Error: Startup management is not supported on this platform.");
                return 1;
            }

            Console.WriteLine($"Disabling startup on {manager.PlatformName}...");
            Console.WriteLine();

            var serviceTypes = ParseServiceTypes(args.ServiceType);
            var success = true;

            foreach (var serviceType in serviceTypes)
            {
                var result = await manager.DisableStartupAsync(serviceType);
                if (!result)
                {
                    success = false;
                }
            }

            return success ? 0 : 1;
        }

        /// <summary>
        /// Handles the status command.
        /// </summary>
        private int HandleStatus()
        {
            var manager = StartupManagerFactory.Create();
            if (manager == null)
            {
                Console.WriteLine("Error: Startup management is not supported on this platform.");
                return 1;
            }

            Console.WriteLine("ZPL2PDF Startup Configuration");
            Console.WriteLine("==============================");
            Console.WriteLine();

            var status = manager.GetStatus();

            Console.WriteLine($"Platform: {manager.PlatformName}");
            Console.WriteLine();
            Console.WriteLine("Startup Services:");
            Console.WriteLine($"  Daemon Mode:      {(status.DaemonEnabled ? "Enabled" : "Disabled")}");
            Console.WriteLine($"  TCP Server:       {(status.TcpServerEnabled ? "Enabled" : "Disabled")}");
            Console.WriteLine($"  Virtual Printer:  {(status.PrinterEnabled ? "Enabled" : "Disabled")}");

            if (!string.IsNullOrEmpty(status.Message))
            {
                Console.WriteLine();
                Console.WriteLine(status.Message);
            }

            return 0;
        }

        /// <summary>
        /// Parses the service type string into an array of service types.
        /// </summary>
        private StartupServiceType[] ParseServiceTypes(string serviceType)
        {
            return serviceType.ToLowerInvariant() switch
            {
                "daemon" => new[] { StartupServiceType.Daemon },
                "tcpserver" => new[] { StartupServiceType.TcpServer },
                "printer" => new[] { StartupServiceType.Printer },
                "all" => new[] { StartupServiceType.Daemon, StartupServiceType.TcpServer, StartupServiceType.Printer },
                _ => new[] { StartupServiceType.Daemon, StartupServiceType.TcpServer, StartupServiceType.Printer }
            };
        }

        /// <summary>
        /// Shows usage information.
        /// </summary>
        private void ShowUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Startup Management Commands:");
            Console.WriteLine("  ZPL2PDF startup enable [service]   - Enable auto-start");
            Console.WriteLine("  ZPL2PDF startup disable [service]  - Disable auto-start");
            Console.WriteLine("  ZPL2PDF startup status             - Show startup status");
            Console.WriteLine();
            Console.WriteLine("Services:");
            Console.WriteLine("  daemon      - Folder monitoring daemon");
            Console.WriteLine("  tcpserver   - TCP virtual printer server");
            Console.WriteLine("  printer     - OS virtual printer");
            Console.WriteLine("  all         - All services (default)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  ZPL2PDF startup enable daemon");
            Console.WriteLine("  ZPL2PDF startup enable all");
            Console.WriteLine("  ZPL2PDF startup disable tcpserver");
        }
    }
}

