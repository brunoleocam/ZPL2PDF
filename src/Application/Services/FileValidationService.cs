using System;
using System.IO;
using ZPL2PDF.Application.Interfaces;

namespace ZPL2PDF.Application.Services
{
    /// <summary>
    /// Service responsible for file validation
    /// </summary>
    public class FileValidationService : IFileValidationService
    {
        /// <summary>
        /// Valid file extensions for ZPL processing
        /// </summary>
        private static readonly string[] ValidExtensions = { ".txt", ".prn", ".zpl", ".imp" };

        /// <summary>
        /// Checks if the file is valid for processing
        /// </summary>
        public bool IsValidFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            if (!File.Exists(filePath))
                return false;

            return IsValidExtension(filePath);
        }

        /// <summary>
        /// Checks if the file is being used by another process
        /// </summary>
        public bool IsFileLocked(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Validates file extension
        /// </summary>
        public bool IsValidExtension(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return Array.Exists(ValidExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }
    }
}
