using System;
using System.IO;
using ZPL2PDF.Application.Interfaces;

namespace ZPL2PDF.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IFileValidationService for testing
    /// </summary>
    public class MockFileValidationService : IFileValidationService
    {
        private readonly bool _isValidFile;
        private readonly bool _isFileLocked;

        public MockFileValidationService(bool isValidFile = true, bool isFileLocked = false)
        {
            _isValidFile = isValidFile;
            _isFileLocked = isFileLocked;
        }

        public bool IsValidFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            // Mock validation logic
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return _isValidFile && (extension == ".txt" || extension == ".prn");
        }

        public bool IsFileLocked(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            // Mock file locking logic
            return _isFileLocked;
        }

        public bool IsValidExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension == ".txt" || extension == ".prn";
        }

        public string GetValidationError(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "File path is null or empty";

            if (!IsValidExtension(filePath))
                return "Invalid file extension. Only .txt and .prn files are supported";

            if (IsFileLocked(filePath))
                return "File is locked and cannot be accessed";

            if (!File.Exists(filePath))
                return "File does not exist";

            return string.Empty;
        }
    }
}
