namespace ZPL2PDF.Application.Interfaces
{
    /// <summary>
    /// Interface for file validation service
    /// </summary>
    public interface IFileValidationService
    {
        /// <summary>
        /// Checks if the file is valid for processing
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>True if valid, False otherwise</returns>
        bool IsValidFile(string filePath);

        /// <summary>
        /// Checks if the file is being used by another process
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
    }
}
