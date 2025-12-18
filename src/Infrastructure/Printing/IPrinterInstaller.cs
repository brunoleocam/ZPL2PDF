using System.Threading.Tasks;

namespace ZPL2PDF.Infrastructure.Printing
{
    /// <summary>
    /// Interface for platform-specific virtual printer installation.
    /// </summary>
    public interface IPrinterInstaller
    {
        /// <summary>
        /// Gets the name of the platform (Windows, Linux, macOS).
        /// </summary>
        string PlatformName { get; }

        /// <summary>
        /// Gets whether the virtual printer is currently installed.
        /// </summary>
        bool IsInstalled { get; }

        /// <summary>
        /// Installs the virtual printer on the system.
        /// </summary>
        /// <param name="printerName">Name for the virtual printer.</param>
        /// <returns>True if installation was successful.</returns>
        Task<bool> InstallAsync(string printerName = "ZPL2PDF");

        /// <summary>
        /// Uninstalls the virtual printer from the system.
        /// </summary>
        /// <returns>True if uninstallation was successful.</returns>
        Task<bool> UninstallAsync();

        /// <summary>
        /// Gets the status of the virtual printer.
        /// </summary>
        /// <returns>Status information.</returns>
        PrinterStatus GetStatus();
    }

    /// <summary>
    /// Status information for the virtual printer.
    /// </summary>
    public class PrinterStatus
    {
        /// <summary>
        /// Whether the printer is installed.
        /// </summary>
        public bool IsInstalled { get; set; }

        /// <summary>
        /// Name of the installed printer.
        /// </summary>
        public string? PrinterName { get; set; }

        /// <summary>
        /// Whether the printer service is running.
        /// </summary>
        public bool IsServiceRunning { get; set; }

        /// <summary>
        /// Output directory for PDFs.
        /// </summary>
        public string? OutputDirectory { get; set; }

        /// <summary>
        /// Additional status message.
        /// </summary>
        public string? Message { get; set; }
    }
}

