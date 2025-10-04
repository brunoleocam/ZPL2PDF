using System;

namespace ZPL2PDF.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for file system watching service
    /// </summary>
    public interface IFileSystemWatcher : IDisposable
    {
        /// <summary>
        /// Event fired when a file is created
        /// </summary>
        event EventHandler<FileCreatedEventArgs> FileCreated;

        /// <summary>
        /// Gets or sets the path to watch
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the filter for files to watch
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// Gets or sets whether to include subdirectories
        /// </summary>
        bool IncludeSubdirectories { get; set; }

        /// <summary>
        /// Gets or sets whether the watcher is enabled
        /// </summary>
        bool EnableRaisingEvents { get; set; }

        /// <summary>
        /// Starts watching the specified path
        /// </summary>
        void StartWatching();

        /// <summary>
        /// Stops watching
        /// </summary>
        void StopWatching();
    }

    /// <summary>
    /// Event arguments for file created events
    /// </summary>
    public class FileCreatedEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
    }
}
