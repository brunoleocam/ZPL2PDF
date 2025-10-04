using System.Threading.Tasks;

namespace ZPL2PDF.Application.Interfaces
{
    /// <summary>
    /// Interface for daemon service
    /// </summary>
    public interface IDaemonService
    {
        /// <summary>
        /// Starts the daemon
        /// </summary>
        /// <param name="listenFolder">Folder to monitor</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        /// <returns>True if started successfully, False otherwise</returns>
        bool Start(string listenFolder, double width, double height, string unit, int dpi);

        /// <summary>
        /// Stops the daemon
        /// </summary>
        /// <returns>True if stopped successfully, False otherwise</returns>
        bool Stop();

        /// <summary>
        /// Gets the daemon status
        /// </summary>
        /// <returns>True if running, False otherwise</returns>
        bool Status();

        /// <summary>
        /// Checks if the daemon is running
        /// </summary>
        /// <returns>True if running, False otherwise</returns>
        bool IsRunning();

        /// <summary>
        /// Runs daemon mode in the current process
        /// </summary>
        /// <param name="listenFolder">Folder to monitor</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        /// <returns>Task representing the daemon operation</returns>
        Task RunAsync(string listenFolder, double width, double height, string unit, int dpi);
    }
}
