namespace ZPL2PDF.Application.Interfaces
{
    /// <summary>
    /// Interface for path management service
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
    }
}
