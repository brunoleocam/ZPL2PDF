namespace ZPL2PDF.Domain.Services
{
    /// <summary>
    /// Interface for file validation service
    /// </summary>
    public interface IFileValidator
    {
        /// <summary>
        /// Validates if a file is valid for processing
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>True if valid, False otherwise</returns>
        bool IsValidFile(string filePath);

        /// <summary>
        /// Validates if a file is locked by another process
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>True if locked, False otherwise</returns>
        bool IsFileLocked(string filePath);

        /// <summary>
        /// Validates file extension
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>True if extension is valid, False otherwise</returns>
        bool IsValidExtension(string filePath);

        /// <summary>
        /// Gets the list of valid file extensions
        /// </summary>
        /// <returns>Array of valid extensions</returns>
        string[] GetValidExtensions();
    }
}
