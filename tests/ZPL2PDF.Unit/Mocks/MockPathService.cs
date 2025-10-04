using System;
using System.IO;
using ZPL2PDF.Application.Interfaces;

namespace ZPL2PDF.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IPathService for testing
    /// </summary>
    public class MockPathService : IPathService
    {
        private readonly string _basePath;
        private readonly bool _shouldCreateDirectories;

        public MockPathService(string basePath = null, bool shouldCreateDirectories = true)
        {
            _basePath = basePath ?? Path.GetTempPath();
            _shouldCreateDirectories = shouldCreateDirectories;
        }

        public void EnsureDirectoryExists(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return;

            if (_shouldCreateDirectories && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public string GetDefaultListenFolder()
        {
            return Path.Combine(_basePath, "ZPL2PDF Auto Converter");
        }

        public string GetConfigFolder()
        {
            return Path.Combine(_basePath, "ZPL2PDF", "Config");
        }

        public string GetPidFolder()
        {
            return Path.Combine(_basePath, "ZPL2PDF", "Pid");
        }

        public string Combine(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
                return path2;
            if (string.IsNullOrEmpty(path2))
                return path1;

            return Path.Combine(path1, path2);
        }

        public string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            return Path.GetDirectoryName(path) ?? string.Empty;
        }
    }
}
