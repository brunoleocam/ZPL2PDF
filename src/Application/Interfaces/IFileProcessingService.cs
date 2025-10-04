using System.Threading.Tasks;

namespace ZPL2PDF.Application.Interfaces
{
    /// <summary>
    /// Interface for file processing service
    /// </summary>
    public interface IFileProcessingService
    {
        /// <summary>
        /// Processes a file for conversion
        /// </summary>
        /// <param name="filePath">Path to the file to process</param>
        /// <param name="width">Label width</param>
        /// <param name="height">Label height</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="dpi">Print density</param>
        /// <returns>True if processed successfully, False otherwise</returns>
        Task<bool> ProcessFileAsync(string filePath, double width, double height, string unit, int dpi);

        /// <summary>
        /// Adds a file to the processing queue
        /// </summary>
        /// <param name="filePath">Path to the file to add</param>
        void AddFile(string filePath);

        /// <summary>
        /// Starts the processing queue
        /// </summary>
        void StartProcessing();

        /// <summary>
        /// Stops the processing queue
        /// </summary>
        void StopProcessing();

        /// <summary>
        /// Gets the current queue status
        /// </summary>
        /// <returns>Queue status information</returns>
        QueueStatus GetQueueStatus();
    }

    /// <summary>
    /// Represents queue status information
    /// </summary>
    public class QueueStatus
    {
        public bool IsRunning { get; set; }
        public int PendingFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int FailedFiles { get; set; }
    }
}
