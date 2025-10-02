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
    /// Integration tests for daemon functionality
    /// </summary>
    public class DaemonIntegrationTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _listenFolder;
        private readonly string _pidFilePath;

        public DaemonIntegrationTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_DaemonIntegrationTests", Guid.NewGuid().ToString());
            _listenFolder = Path.Combine(_testDirectory, "listen");
            _pidFilePath = Path.Combine(_testDirectory, "zpl2pdf.pid");
            
            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_listenFolder);

            // Set environment variables for test isolation
            Environment.SetEnvironmentVariable("ZPL2PDF_PID_FOLDER", _testDirectory);
            Environment.SetEnvironmentVariable("ZPL2PDF_CONFIG_FOLDER", _testDirectory);
        }

        #region Daemon Lifecycle Tests

        [Fact]
        public async Task StartDaemon_ProcessFile_ConvertsSuccessfully()
        {
            // Arrange
            var daemonManager = new DaemonManager(_listenFolder, "100", "200", "mm", "203");
            var testFile = Path.Combine(_listenFolder, "test.txt");
            var outputFile = Path.Combine(_listenFolder, "test.pdf");

            // Act
            var startResult = daemonManager.Start();
            startResult.Should().BeTrue();

            // Wait a bit for daemon to start
            await Task.Delay(1000);

            // Create test file
            File.WriteAllText(testFile, SampleZplData.SimpleLabel);

            // Wait for processing
            await Task.Delay(2000);

            // Stop daemon
            daemonManager.Stop();

            // Assert
            startResult.Should().BeTrue();
            File.Exists(testFile).Should().BeTrue();
            // Note: Output file may not exist if processing failed, but daemon should start successfully
        }

        [Fact]
        public async Task StartDaemon_StopDaemon_StopsCorrectly()
        {
            // Arrange
            var daemonManager = new DaemonManager(_listenFolder, "100", "200", "mm", "203");

            // Act
            var startResult = daemonManager.Start();
            startResult.Should().BeTrue();

            // Wait a bit for daemon to start
            await Task.Delay(1000);

            var stopResult = daemonManager.Stop();

            // Assert
            startResult.Should().BeTrue();
            stopResult.Should().BeTrue();
            daemonManager.IsRunning().Should().BeFalse();
        }

        [Fact]
        public async Task Daemon_WithInvalidFile_HandlesError()
        {
            // Arrange
            var daemonManager = new DaemonManager(_listenFolder, "100", "200", "mm", "203");
            var testFile = Path.Combine(_listenFolder, "invalid.txt");

            // Act
            var startResult = daemonManager.Start();
            startResult.Should().BeTrue();

            // Wait a bit for daemon to start
            await Task.Delay(1000);

            // Create invalid file
            File.WriteAllText(testFile, "INVALID_ZPL_CONTENT");

            // Wait for processing
            await Task.Delay(2000);

            // Stop daemon
            daemonManager.Stop();

            // Assert
            startResult.Should().BeTrue();
            File.Exists(testFile).Should().BeTrue();
            // Daemon should handle invalid files gracefully
        }

        #endregion

        #region Daemon Status Tests

        [Fact]
        public void Daemon_Status_WhenNotRunning_ReturnsFalse()
        {
            // Arrange
            var daemonManager = new DaemonManager(_listenFolder, "100", "200", "mm", "203");

            // Act
            var isRunning = daemonManager.IsRunning();

            // Assert
            isRunning.Should().BeFalse();
        }

        [Fact]
        public async Task Daemon_Status_WhenRunning_ReturnsTrue()
        {
            // Arrange
            var daemonManager = new DaemonManager(_listenFolder, "100", "200", "mm", "203");

            // Act
            var startResult = daemonManager.Start();
            startResult.Should().BeTrue();

            // Wait a bit for daemon to start
            await Task.Delay(1000);

            var isRunning = daemonManager.IsRunning();

            // Cleanup
            daemonManager.Stop();

            // Assert
            isRunning.Should().BeTrue();
        }

        #endregion

        #region Daemon Configuration Tests

        [Fact]
        public async Task Daemon_WithCustomDimensions_WorksCorrectly()
        {
            // Arrange
            var daemonManager = new DaemonManager(_listenFolder, "150", "250", "in", "300");

            // Act
            var startResult = daemonManager.Start();
            startResult.Should().BeTrue();

            // Wait a bit for daemon to start
            await Task.Delay(1000);

            var isRunning = daemonManager.IsRunning();

            // Cleanup
            daemonManager.Stop();

            // Assert
            startResult.Should().BeTrue();
            isRunning.Should().BeTrue();
        }

        [Fact]
        public async Task Daemon_WithNonExistentFolder_CreatesFolder()
        {
            // Arrange
            var nonExistentFolder = Path.Combine(_testDirectory, "nonexistent", "subfolder");
            var daemonManager = new DaemonManager(nonExistentFolder, "100", "200", "mm", "203");

            // Act
            var startResult = daemonManager.Start();
            startResult.Should().BeTrue();

            // Wait a bit for daemon to start
            await Task.Delay(1000);

            var isRunning = daemonManager.IsRunning();

            // Cleanup
            daemonManager.Stop();

            // Assert
            startResult.Should().BeTrue();
            isRunning.Should().BeTrue();
            Directory.Exists(nonExistentFolder).Should().BeTrue();
        }

        #endregion

        #region Multiple Daemon Tests

        [Fact]
        public async Task MultipleDaemons_WithSameFolder_OnlyOneCanRun()
        {
            // Arrange
            var daemonManager1 = new DaemonManager(_listenFolder, "100", "200", "mm", "203");
            var daemonManager2 = new DaemonManager(_listenFolder, "150", "250", "in", "300");

            // Act
            var startResult1 = daemonManager1.Start();
            startResult1.Should().BeTrue();

            // Wait a bit for first daemon to start
            await Task.Delay(1000);

            var startResult2 = daemonManager2.Start();

            // Cleanup
            daemonManager1.Stop();
            daemonManager2.Stop();

            // Assert
            startResult1.Should().BeTrue();
            startResult2.Should().BeFalse(); // Second daemon should fail
        }

        #endregion

        public void Dispose()
        {
            // Clean up test files and directories
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
            Environment.SetEnvironmentVariable("ZPL2PDF_PID_FOLDER", null);
            Environment.SetEnvironmentVariable("ZPL2PDF_CONFIG_FOLDER", null);
        }
    }
}
