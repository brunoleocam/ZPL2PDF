using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Startup
{
    /// <summary>
    /// Startup service types that can be configured to run at system startup.
    /// </summary>
    public enum StartupServiceType
    {
        /// <summary>
        /// Daemon mode - monitors folder and converts automatically.
        /// </summary>
        Daemon,

        /// <summary>
        /// TCP Server mode - acts as virtual printer on TCP port.
        /// </summary>
        TcpServer,

        /// <summary>
        /// Virtual Printer - OS-level virtual printer.
        /// </summary>
        Printer
    }

    /// <summary>
    /// Interface for platform-specific startup management.
    /// </summary>
    public interface IStartupManager
    {
        /// <summary>
        /// Gets the name of the platform.
        /// </summary>
        string PlatformName { get; }

        /// <summary>
        /// Enables startup for the specified service.
        /// </summary>
        /// <param name="serviceType">Service type to enable.</param>
        /// <returns>True if successful.</returns>
        Task<bool> EnableStartupAsync(StartupServiceType serviceType);

        /// <summary>
        /// Disables startup for the specified service.
        /// </summary>
        /// <param name="serviceType">Service type to disable.</param>
        /// <returns>True if successful.</returns>
        Task<bool> DisableStartupAsync(StartupServiceType serviceType);

        /// <summary>
        /// Checks if startup is enabled for the specified service.
        /// </summary>
        /// <param name="serviceType">Service type to check.</param>
        /// <returns>True if enabled.</returns>
        bool IsStartupEnabled(StartupServiceType serviceType);

        /// <summary>
        /// Gets the startup status for all services.
        /// </summary>
        /// <returns>Status information.</returns>
        StartupStatus GetStatus();
    }

    /// <summary>
    /// Status information for startup configuration.
    /// </summary>
    public class StartupStatus
    {
        /// <summary>
        /// Whether daemon startup is enabled.
        /// </summary>
        public bool DaemonEnabled { get; set; }

        /// <summary>
        /// Whether TCP server startup is enabled.
        /// </summary>
        public bool TcpServerEnabled { get; set; }

        /// <summary>
        /// Whether virtual printer startup is enabled.
        /// </summary>
        public bool PrinterEnabled { get; set; }

        /// <summary>
        /// Additional status message.
        /// </summary>
        public string? Message { get; set; }
    }
}

