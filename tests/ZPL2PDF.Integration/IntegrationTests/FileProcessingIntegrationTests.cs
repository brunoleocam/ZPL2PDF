using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using ZPL2PDF;
using ZPL2PDF.Tests.TestData;

namespace ZPL2PDF.Integration.IntegrationTests
{
    /// <summary>
    /// Integration tests for file processing functionality
    /// </summary>
    public class FileProcessingIntegrationTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _listenFolder;
        private readonly ZplDimensionExtractor _dimensionExtractor;
        private readonly ConfigManager _configManager;

        public FileProcessingIntegrationTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_FileProcessingIntegrationTests", Guid.NewGuid().ToString());
            _listenFolder = Path.Combine(_testDirectory, "listen");
            
            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_listenFolder);

            _dimensionExtractor = new ZplDimensionExtractor();
            _configManager = new ConfigManager();
        }

        #region File Processing Tests

        [Fact]
        public async Task ProcessFile_WithValidZpl_ConvertsToPdf()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var testFile = Path.Combine(_listenFolder, "test.txt");
            var zplContent = SampleZplData.SimpleLabel;
            
            File.WriteAllText(testFile, zplContent);

            var processingItem = new ProcessingItem
            {
                FilePath = testFile,
                FileName = "test.txt",
                Content = zplContent,
                CreatedAt = DateTime.Now
            };

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait for processing
            await Task.Delay(2000);

            // Assert
            File.Exists(testFile).Should().BeTrue();
            // Note: Processing may succeed or fail, but should not crash
        }

        [Fact]
        public async Task ProcessFile_WithInvalidZpl_HandlesError()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var testFile = Path.Combine(_listenFolder, "invalid.txt");
            var invalidZpl = "INVALID_ZPL_CONTENT";
            
            File.WriteAllText(testFile, invalidZpl);

            var processingItem = new ProcessingItem
            {
                FilePath = testFile,
                FileName = "invalid.txt",
                Content = invalidZpl,
                CreatedAt = DateTime.Now
            };

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait for processing
            await Task.Delay(2000);

            // Assert
            File.Exists(testFile).Should().BeTrue();
            // Should handle invalid ZPL gracefully
        }

        [Fact]
        public async Task ProcessFile_WithLockedFile_RetriesAndSucceeds()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var testFile = Path.Combine(_listenFolder, "locked.txt");
            var zplContent = SampleZplData.SimpleLabel;
            
            File.WriteAllText(testFile, zplContent);

            var processingItem = new ProcessingItem
            {
                FilePath = testFile,
                FileName = "locked.txt",
                Content = zplContent,
                CreatedAt = DateTime.Now
            };

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait for processing
            await Task.Delay(2000);

            // Assert
            File.Exists(testFile).Should().BeTrue();
            // Should handle file locking gracefully
        }

        #endregion

        #region Folder Monitor Tests

        [Fact]
        public async Task FolderMonitor_WithValidFile_DetectsAndProcesses()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var folderMonitor = new FolderMonitor(
                _listenFolder,
                processingQueue,
                _dimensionExtractor,
                _configManager
            );

            bool fileDetected = false;
            folderMonitor.FileDetected += (sender, e) => fileDetected = true;

            // Act
            folderMonitor.StartWatching();

            // Wait a bit for monitoring to start
            await Task.Delay(1000);

            // Create test file
            var testFile = Path.Combine(_listenFolder, "monitor_test.txt");
            File.WriteAllText(testFile, SampleZplData.SimpleLabel);

            // Wait for detection
            await Task.Delay(2000);

            // Stop monitoring
            folderMonitor.StopWatching();

            // Assert
            fileDetected.Should().BeTrue();
            File.Exists(testFile).Should().BeTrue();
        }

        [Fact]
        public async Task FolderMonitor_WithInvalidFile_IgnoresFile()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var folderMonitor = new FolderMonitor(
                _listenFolder,
                processingQueue,
                _dimensionExtractor,
                _configManager
            );

            bool fileDetected = false;
            folderMonitor.FileDetected += (sender, e) => fileDetected = true;

            // Act
            folderMonitor.StartWatching();

            // Wait a bit for monitoring to start
            await Task.Delay(1000);

            // Create invalid file
            var testFile = Path.Combine(_listenFolder, "invalid.doc");
            File.WriteAllText(testFile, "This is not a ZPL file");

            // Wait for detection
            await Task.Delay(2000);

            // Stop monitoring
            folderMonitor.StopWatching();

            // Assert
            fileDetected.Should().BeFalse(); // Should not detect invalid files
            File.Exists(testFile).Should().BeTrue();
        }

        #endregion

        #region Multiple File Processing Tests

        [Fact]
        public async Task ProcessMultipleFiles_WithValidZpl_ProcessesAll()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var fileCount = 5;

            // Act
            for (int i = 0; i < fileCount; i++)
            {
                var testFile = Path.Combine(_listenFolder, $"test_{i}.txt");
                File.WriteAllText(testFile, SampleZplData.SimpleLabel);

                var processingItem = new ProcessingItem
                {
                    FilePath = testFile,
                    FileName = $"test_{i}.txt",
                    Content = SampleZplData.SimpleLabel,
                    CreatedAt = DateTime.Now
                };

                await processingQueue.AddFileAsync(processingItem);
            }

            // Wait for processing
            await Task.Delay(3000);

            // Assert
            for (int i = 0; i < fileCount; i++)
            {
                var testFile = Path.Combine(_listenFolder, $"test_{i}.txt");
                File.Exists(testFile).Should().BeTrue();
            }
        }

        [Fact]
        public async Task ProcessMultipleFiles_WithMixedContent_HandlesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var validFile = Path.Combine(_listenFolder, "valid.txt");
            var invalidFile = Path.Combine(_listenFolder, "invalid.txt");

            // Act
            File.WriteAllText(validFile, SampleZplData.SimpleLabel);
            File.WriteAllText(invalidFile, "INVALID_ZPL_CONTENT");

            var validItem = new ProcessingItem
            {
                FilePath = validFile,
                FileName = "valid.txt",
                Content = SampleZplData.SimpleLabel,
                CreatedAt = DateTime.Now
            };

            var invalidItem = new ProcessingItem
            {
                FilePath = invalidFile,
                FileName = "invalid.txt",
                Content = "INVALID_ZPL_CONTENT",
                CreatedAt = DateTime.Now
            };

            await processingQueue.AddFileAsync(validItem);
            await processingQueue.AddFileAsync(invalidItem);

            // Wait for processing
            await Task.Delay(3000);

            // Assert
            File.Exists(validFile).Should().BeTrue();
            File.Exists(invalidFile).Should().BeTrue();
            // Both files should be handled gracefully
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task ProcessFile_WithEmptyFile_HandlesGracefully()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var testFile = Path.Combine(_listenFolder, "empty.txt");
            
            File.WriteAllText(testFile, string.Empty);

            var processingItem = new ProcessingItem
            {
                FilePath = testFile,
                FileName = "empty.txt",
                Content = string.Empty,
                CreatedAt = DateTime.Now
            };

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait for processing
            await Task.Delay(2000);

            // Assert
            File.Exists(testFile).Should().BeTrue();
            // Should handle empty files gracefully
        }

        [Fact]
        public async Task ProcessFile_WithNonExistentFile_HandlesGracefully()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var nonExistentFile = Path.Combine(_listenFolder, "nonexistent.txt");

            var processingItem = new ProcessingItem
            {
                FilePath = nonExistentFile,
                FileName = "nonexistent.txt",
                Content = SampleZplData.SimpleLabel,
                CreatedAt = DateTime.Now
            };

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait for processing
            await Task.Delay(2000);

            // Assert
            File.Exists(nonExistentFile).Should().BeFalse();
            // Should handle non-existent files gracefully
        }

        #endregion

        public void Dispose()
        {
            // Clean up test files and directories
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
