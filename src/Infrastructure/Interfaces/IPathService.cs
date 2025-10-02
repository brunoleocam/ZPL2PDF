namespace ZPL2PDF.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for path management service in infrastructure layer
    /// </summary>
    public interface IPathService
    {
        /// <summary>
        /// Ensures directory exists, creating it if necessary
        /// </summary>
        /// <param name="directoryPath">Path to the directory</param>
        void EnsureDirectoryExists(string directoryPath);

        /// <summary>
        /// Gets the default monitoring folder based on OS
        /// </summary>
        /// <returns>Default folder path</returns>
        string GetDefaultListenFolder();

        /// <summary>
        /// Gets the configuration folder based on OS
        /// </summary>
        /// <returns>Configuration folder path</returns>
        string GetConfigFolder();

        /// <summary>
        /// Gets the PID folder based on OS
        /// </summary>
        /// <returns>PID folder path</returns>
        string GetPidFolder();

        /// <summary>
        /// Combines paths safely
        /// </summary>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <returns>Combined path</returns>
        string Combine(string path1, string path2);

        /// <summary>
        /// Gets directory name from path
        /// </summary>
        /// <param name="path">File or directory path</param>
        /// <returns>Directory name</returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// Checks if a path exists
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if exists, False otherwise</returns>
        bool Exists(string path);

        /// <summary>
        /// Gets the file name from a path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>File name</returns>
        string GetFileName(string path);

        /// <summary>
        /// Gets the file extension from a path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>File extension</returns>
        string GetExtension(string path);
    }
}
