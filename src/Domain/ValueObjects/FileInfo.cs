using System;
using System.IO;

namespace ZPL2PDF.Domain.ValueObjects
{
    /// <summary>
    /// Represents file information for processing
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        /// Gets or sets the file path
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file extension
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file size in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the file creation time
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the file last modified time
        /// </summary>
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets whether the file exists
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// Gets or sets whether the file is locked
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the file content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of FileInfo
        /// </summary>
        public FileInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of FileInfo from file path
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public FileInfo(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Extension = Path.GetExtension(filePath);
            Exists = File.Exists(filePath);

            if (Exists)
            {
                var fileInfo = new System.IO.FileInfo(filePath);
                Size = fileInfo.Length;
                CreatedTime = fileInfo.CreationTime;
                LastModifiedTime = fileInfo.LastWriteTime;
            }
        }

        /// <summary>
        /// Creates FileInfo from file path with content
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="content">File content</param>
        /// <returns>FileInfo instance</returns>
        public static FileInfo FromPath(string filePath, string? content = null)
        {
            var fileInfo = new FileInfo(filePath);
            if (!string.IsNullOrEmpty(content))
            {
                fileInfo.Content = content;
            }
            return fileInfo;
        }

        /// <summary>
        /// Creates FileInfo from content only
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="content">File content</param>
        /// <returns>FileInfo instance</returns>
        public static FileInfo FromContent(string fileName, string content)
        {
            return new FileInfo
            {
                FileName = fileName,
                Extension = Path.GetExtension(fileName),
                Content = content,
                Exists = true,
                CreatedTime = DateTime.Now,
                LastModifiedTime = DateTime.Now
            };
        }

        /// <summary>
        /// Validates the file information
        /// </summary>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return false;

            if (string.IsNullOrWhiteSpace(Extension))
                return false;

            if (!IsValidExtension())
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the file extension is valid for ZPL processing
        /// </summary>
        /// <returns>True if valid, False otherwise</returns>
        public bool IsValidExtension()
        {
            var validExtensions = new[] { ".txt", ".prn" };
            return Array.Exists(validExtensions, ext => 
                ext.Equals(Extension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets validation error message
        /// </summary>
        /// <returns>Error message if invalid, empty string if valid</returns>
        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return "File name cannot be null or empty";

            if (string.IsNullOrWhiteSpace(Extension))
                return "File extension cannot be null or empty";

            if (!IsValidExtension())
                return $"Invalid file extension: {Extension}. Valid extensions are: .txt, .prn";

            return string.Empty;
        }

        /// <summary>
        /// Creates a copy of the file info
        /// </summary>
        /// <returns>New instance with same values</returns>
        public FileInfo Clone()
        {
            return new FileInfo
            {
                FilePath = FilePath,
                FileName = FileName,
                Extension = Extension,
                Size = Size,
                CreatedTime = CreatedTime,
                LastModifiedTime = LastModifiedTime,
                Exists = Exists,
                IsLocked = IsLocked,
                Content = Content
            };
        }

        /// <summary>
        /// Returns a string representation of the file info
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"FileInfo: {FileName} ({Size} bytes, {Extension})";
        }
    }
}
