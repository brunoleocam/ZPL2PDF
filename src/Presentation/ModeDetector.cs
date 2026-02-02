using System;
using System.Linq;

namespace ZPL2PDF
{
    /// <summary>
    /// Responsible for detecting the operation mode from command line arguments
    /// </summary>
    public class ModeDetector
    {
        /// <summary>
        /// Detects the operation mode based on command line arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Detected operation mode</returns>
        public OperationMode DetectMode(string[] args)
        {
            if (args.Length == 0)
            {
                // No arguments = daemon start mode (default behavior)
                return OperationMode.Daemon;
            }

            if (args[0].Equals("-help", StringComparison.OrdinalIgnoreCase))
            {
                return OperationMode.Help;
            }

            string firstArg = args[0].ToLowerInvariant();

            // Check for TCP Server mode (server start | stop | status)
            if (firstArg == "server")
            {
                return OperationMode.Server;
            }
            
            // Check for explicit daemon commands
            if (firstArg == "start" || firstArg == "stop" || firstArg == "status" || firstArg == "run")
            {
                return OperationMode.Daemon;
            }

            // Check for conversion mode indicators
            if (firstArg == "-i" || firstArg == "-z" || firstArg == "-o")
            {
                return OperationMode.Conversion;
            }

            // Check for invalid daemon usage
            if (firstArg == "-l")
            {
                // -l sem start deve mostrar help
                return OperationMode.Help;
            }

            // Check for implicit mode detection
            bool hasConversionArgs = args.Any(arg => 
                arg.Equals("-i", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-z", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-o", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-n", StringComparison.OrdinalIgnoreCase));
            
            bool hasDaemonArgs = args.Any(arg => 
                arg.Equals("-w", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-u", StringComparison.OrdinalIgnoreCase) ||
                arg.Equals("-d", StringComparison.OrdinalIgnoreCase));
            
            if (hasConversionArgs)
            {
                return OperationMode.Conversion;
            }
            else if (hasDaemonArgs)
            {
                return OperationMode.Daemon;
            }

            // Default to help if unclear
            return OperationMode.Help;
        }

        /// <summary>
        /// Extracts the daemon command from arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Daemon command or empty string</returns>
        public string ExtractDaemonCommand(string[] args)
        {
            if (args.Length == 0)
            {
                return "start";
            }

            string firstArg = args[0].ToLowerInvariant();
            
            if (firstArg == "start" || firstArg == "stop" || firstArg == "status" || firstArg == "run")
            {
                return firstArg;
            }

            return string.Empty;
        }

        /// <summary>
        /// Extracts the TCP server subcommand from arguments (start, stop, status).
        /// </summary>
        /// <param name="args">Command line arguments (first element should be "server")</param>
        /// <returns>Server command: "start", "stop", "status", or "start" as default</returns>
        public string ExtractServerCommand(string[] args)
        {
            if (args.Length < 2 || !args[0].Equals("server", StringComparison.OrdinalIgnoreCase))
            {
                return "start";
            }

            string sub = args[1].ToLowerInvariant();
            if (sub == "start" || sub == "stop" || sub == "status")
            {
                return sub;
            }

            return "start";
        }
    }
}
