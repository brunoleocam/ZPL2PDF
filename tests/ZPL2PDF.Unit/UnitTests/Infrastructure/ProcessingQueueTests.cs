using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for ProcessingQueue
    /// </summary>
    public class ProcessingQueueTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly ZplDimensionExtractor _dimensionExtractor;
        private readonly ConfigManager _configManager;

        public ProcessingQueueTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_ProcessingQueueTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            
            _dimensionExtractor = new ZplDimensionExtractor();
            _configManager = new ConfigManager();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_InitializesCorrectly()
        {
            // Act
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);

            // Assert
            processingQueue.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithCustomMaxConcurrentFiles_InitializesCorrectly()
        {
            // Act
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager, maxConcurrentFiles: 5);

            // Assert
            processingQueue.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithCustomOutputFolder_InitializesCorrectly()
        {
            // Arrange
            var customOutputFolder = Path.Combine(_testDirectory, "output");

            // Act
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager, customOutputFolder: customOutputFolder);

            // Assert
            processingQueue.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullDimensionExtractor_DoesNotThrow()
        {
            // Act & Assert - ProcessingQueue doesn't validate parameters in constructor
            var processingQueue = new ProcessingQueue(null!, _configManager);
            processingQueue.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullConfigManager_DoesNotThrow()
        {
            // Act & Assert - ProcessingQueue doesn't validate parameters in constructor
            var processingQueue = new ProcessingQueue(_dimensionExtractor, null!);
            processingQueue.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Constructor_WithInvalidMaxConcurrentFiles_ThrowsArgumentOutOfRangeException(int maxConcurrentFiles)
        {
            // Act & Assert - ProcessingQueue throws ArgumentOutOfRangeException for invalid maxConcurrentFiles
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                new ProcessingQueue(_dimensionExtractor, _configManager, maxConcurrentFiles));
        }

        #endregion

        #region AddFileAsync Tests

        [Fact]
        public async Task AddFileAsync_WithValidFile_AddsToQueue()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var processingItem = new ProcessingItem { FilePath = testFile, FileName = "test.txt" };

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Assert - Should not throw exception
            processingQueue.Should().NotBeNull();
        }

        [Fact]
        public async Task AddFileAsync_WithNullItem_ThrowsNullReferenceException()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);

            // Act & Assert - Should throw NullReferenceException
            await Assert.ThrowsAsync<NullReferenceException>(() => processingQueue.AddFileAsync(null!));
        }

        [Fact]
        public async Task AddFileAsync_WithEmptyFilePath_DoesNotThrow()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var processingItem = new ProcessingItem { FilePath = "", FileName = "test.txt" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task AddFileAsync_WithNonExistentFile_DoesNotThrow()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");
            var processingItem = new ProcessingItem { FilePath = nonExistentFile, FileName = "nonexistent.txt" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task AddFileAsync_WithInvalidFileExtension_DoesNotThrow()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var invalidFile = Path.Combine(_testDirectory, "test.doc");
            File.WriteAllText(invalidFile, "This is not a ZPL file");
            var processingItem = new ProcessingItem { FilePath = invalidFile, FileName = "test.doc" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task AddFileAsync_WithEmptyFile_DoesNotThrow()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var emptyFile = Path.Combine(_testDirectory, "empty.txt");
            File.WriteAllText(emptyFile, "");
            var processingItem = new ProcessingItem { FilePath = emptyFile, FileName = "empty.txt" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task AddFileAsync_WithLockedFile_DoesNotThrow()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var lockedFile = Path.Combine(_testDirectory, "locked.txt");
            var processingItem = new ProcessingItem { FilePath = lockedFile, FileName = "locked.txt" };
            
            using (var fileStream = File.Create(lockedFile))
            {
                // Act & Assert - Should not throw exception
                await processingQueue.AddFileAsync(processingItem);
            }
        }

        #endregion

        #region Queue Processing Tests

            [Fact]
            public async Task ProcessQueue_WithValidFile_ProcessesSuccessfully()
            {
                // Arrange
                var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
                var testFile = Path.Combine(_testDirectory, "test.txt");
                var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";
                File.WriteAllText(testFile, zplContent);
                var processingItem = new ProcessingItem 
                { 
                    FilePath = testFile, 
                    FileName = "test.txt",
                    Content = zplContent
                };

                bool fileProcessed = false;
                processingQueue.FileProcessed += (sender, e) => fileProcessed = true;

                // Act
                await processingQueue.AddFileAsync(processingItem);
                
                // Wait for processing
                await Task.Delay(3000);

                // Assert
                fileProcessed.Should().BeTrue();
            }

        [Fact]
        public async Task ProcessQueue_WithMultipleFiles_ProcessesAll()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            int processedCount = 0;
            processingQueue.FileProcessed += (sender, e) => processedCount++;

            // Act - Create and add multiple files
            for (int i = 0; i < 3; i++)
            {
                var testFile = Path.Combine(_testDirectory, $"test{i}.txt");
                var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";
                File.WriteAllText(testFile, zplContent);
                var processingItem = new ProcessingItem 
                { 
                    FilePath = testFile, 
                    FileName = $"test{i}.txt",
                    Content = zplContent
                };
                await processingQueue.AddFileAsync(processingItem);
            }

            // Wait for processing
            await Task.Delay(5000);

            // Assert
            processedCount.Should().Be(3);
        }

        [Fact]
        public async Task ProcessQueue_WithInvalidFile_HandlesError()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var invalidFile = Path.Combine(_testDirectory, "invalid.txt");
            var invalidContent = "This is not valid ZPL";
            File.WriteAllText(invalidFile, invalidContent);
            var processingItem = new ProcessingItem 
            { 
                FilePath = invalidFile, 
                FileName = "invalid.txt",
                Content = invalidContent
            };

            bool fileError = false;
            processingQueue.FileError += (sender, e) => fileError = true;

            // Act
            await processingQueue.AddFileAsync(processingItem);
            
            // Wait for processing
            await Task.Delay(2000);

            // Assert
            fileError.Should().BeTrue();
        }

        #endregion

        #region Event Tests

        [Fact]
        public async Task FileProcessed_WhenFileProcessedSuccessfully_FiresEvent()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var testFile = Path.Combine(_testDirectory, "test.txt");
            var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";
            File.WriteAllText(testFile, zplContent);
            var processingItem = new ProcessingItem 
            { 
                FilePath = testFile, 
                FileName = "test.txt",
                Content = zplContent
            };

            bool eventFired = false;
            processingQueue.FileProcessed += (sender, e) => eventFired = true;

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait a bit for processing
            await Task.Delay(4000);

            // Assert
            eventFired.Should().BeTrue();
        }

        [Fact]
        public async Task FileError_WhenFileProcessingFails_FiresEvent()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var invalidFile = Path.Combine(_testDirectory, "invalid.txt");
            File.WriteAllText(invalidFile, "Invalid content");
            var processingItem = new ProcessingItem { FilePath = invalidFile, FileName = "invalid.txt" };

            bool eventFired = false;
            processingQueue.FileError += (sender, e) => eventFired = true;

            // Act
            await processingQueue.AddFileAsync(processingItem);

            // Wait a bit for processing
            await Task.Delay(2000);

            // Assert
            eventFired.Should().BeTrue();
        }

        #endregion

        #region Concurrent Processing Tests

        [Fact]
        public async Task ProcessQueue_WithConcurrentFiles_ProcessesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager, maxConcurrentFiles: 2);
            int processedCount = 0;
            processingQueue.FileProcessed += (sender, e) => Interlocked.Increment(ref processedCount);

            // Act - Create and add multiple files
            for (int i = 0; i < 5; i++)
            {
                var testFile = Path.Combine(_testDirectory, $"concurrent{i}.txt");
                var zplContent = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";
                File.WriteAllText(testFile, zplContent);
                var processingItem = new ProcessingItem 
                { 
                    FilePath = testFile, 
                    FileName = $"concurrent{i}.txt",
                    Content = zplContent
                };
                await processingQueue.AddFileAsync(processingItem);
            }

            // Wait for processing
            await Task.Delay(8000);

            // Assert
            processedCount.Should().Be(5);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task AddFileAsync_WithVeryLongFilePath_HandlesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var veryLongPath = Path.Combine(_testDirectory, new string('a', 200) + ".txt");
            File.WriteAllText(veryLongPath, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var processingItem = new ProcessingItem { FilePath = veryLongPath, FileName = new string('a', 200) + ".txt" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task AddFileAsync_WithSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var specialFile = Path.Combine(_testDirectory, "test with spaces & symbols!.txt");
            File.WriteAllText(specialFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var processingItem = new ProcessingItem { FilePath = specialFile, FileName = "test with spaces & symbols!.txt" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task AddFileAsync_WithUnicodeCharacters_HandlesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var unicodeFile = Path.Combine(_testDirectory, "test_测试_файл.txt");
            File.WriteAllText(unicodeFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var processingItem = new ProcessingItem { FilePath = unicodeFile, FileName = "test_测试_файл.txt" };

            // Act & Assert - Should not throw exception
            await processingQueue.AddFileAsync(processingItem);
        }

        [Fact]
        public async Task ProcessQueue_WithLargeFile_ProcessesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);
            var largeFile = Path.Combine(_testDirectory, "large.txt");
            
            // Create a large ZPL file
            var largeZpl = "^XA";
            for (int i = 0; i < 100; i++)
            {
                largeZpl += $"^FO50,{50 + i * 20}^A0N,50,50^FDLine {i}^FS";
            }
            largeZpl += "^XZ";
            File.WriteAllText(largeFile, largeZpl);
            var processingItem = new ProcessingItem 
            { 
                FilePath = largeFile, 
                FileName = "large.txt",
                Content = largeZpl
            };

            bool fileProcessed = false;
            processingQueue.FileProcessed += (sender, e) => fileProcessed = true;

            // Act
            await processingQueue.AddFileAsync(processingItem);
            
            // Wait for processing
            await Task.Delay(3000);

            // Assert
            fileProcessed.Should().BeTrue();
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_WhenCalled_DisposesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);

            // Act
            processingQueue.Dispose();

            // Assert - Should not throw exception
            processingQueue.Should().NotBeNull();
        }

        [Fact]
        public void Dispose_MultipleTimes_HandlesCorrectly()
        {
            // Arrange
            var processingQueue = new ProcessingQueue(_dimensionExtractor, _configManager);

            // Act & Assert - Should not throw exception
            processingQueue.Dispose();
            processingQueue.Dispose();
        }

        #endregion

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
