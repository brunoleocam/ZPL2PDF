using System;
using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for DaemonManager
    /// </summary>
    public class DaemonManagerTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testPidFile;

        public DaemonManagerTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_DaemonManagerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _testPidFile = Path.Combine(_testDirectory, "test.pid");
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_InitializesCorrectly()
        {
            // Arrange
            var listenFolder = _testDirectory;
            var labelWidth = "100";
            var labelHeight = "200";
            var unit = "mm";
            var printDensity = "203";

            // Act
            var daemonManager = new DaemonManager(listenFolder, labelWidth, labelHeight, unit, printDensity);

            // Assert
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullListenFolder_DoesNotThrow()
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager(null!, "100", "200", "mm", "203");
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithEmptyListenFolder_DoesNotThrow()
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager("", "100", "200", "mm", "203");
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullLabelWidth_DoesNotThrow()
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager(_testDirectory, null!, "200", "mm", "203");
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullLabelHeight_DoesNotThrow()
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager(_testDirectory, "100", null!, "mm", "203");
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullUnit_DoesNotThrow()
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", null!, "203");
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullPrintDensity_DoesNotThrow()
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", null!);
            daemonManager.Should().NotBeNull();
        }

        #endregion

        #region Start Tests

        [Fact]
        public void Start_WhenNotRunning_ReturnsTrue()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            
            // Ensure no daemon is running
            daemonManager.Stop();

            // Act
            var result = daemonManager.Start();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Start_WhenAlreadyRunning_ReturnsFalse()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            daemonManager.Start(); // First start

            // Act
            var result = daemonManager.Start(); // Second start

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Start_WithInvalidListenFolder_CreatesFolderAndReturnsTrue()
        {
            // Arrange
            var invalidFolder = Path.Combine(_testDirectory, "nonexistent", "subfolder");
            var daemonManager = new DaemonManager(invalidFolder, "100", "200", "mm", "203");

            // Ensure no daemon is running
            daemonManager.Stop();

            // Act
            var result = daemonManager.Start();

            // Assert
            result.Should().BeTrue();
            Directory.Exists(invalidFolder).Should().BeTrue();
        }

        #endregion

        #region Stop Tests

        [Fact]
        public void Stop_WhenRunning_ReturnsTrue()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            daemonManager.Start();

            // Act
            var result = daemonManager.Stop();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Stop_WhenNotRunning_ReturnsFalse()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");

            // Act
            var result = daemonManager.Stop();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsRunning Tests

        [Fact]
        public void IsRunning_WhenNotStarted_ReturnsFalse()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");

            // Act
            var result = daemonManager.IsRunning();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsRunning_AfterStart_ReturnsTrue()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            daemonManager.Start();

            // Act
            var result = daemonManager.IsRunning();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsRunning_AfterStop_ReturnsFalse()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            daemonManager.Start();
            daemonManager.Stop();

            // Act
            var result = daemonManager.IsRunning();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetStatus Tests

        [Fact]
        public void IsRunning_WhenNotRunning_ReturnsFalse()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");

            // Act
            var isRunning = daemonManager.IsRunning();

            // Assert
            isRunning.Should().BeFalse();
        }

        [Fact]
        public void IsRunning_WhenRunning_ReturnsTrue()
        {
            // Arrange
            var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            daemonManager.Start();

            // Act
            var isRunning = daemonManager.IsRunning();

            // Assert
            isRunning.Should().BeTrue();
        }

        #endregion

        #region Edge Cases Tests

        [Theory]
        [InlineData("0", "200", "mm", "203")]
        [InlineData("100", "0", "mm", "203")]
        [InlineData("100", "200", "invalid", "203")]
        [InlineData("100", "200", "mm", "0")]
        [InlineData("-100", "200", "mm", "203")]
        [InlineData("100", "-200", "mm", "203")]
        public void Constructor_WithInvalidValues_DoesNotThrow(string width, string height, string unit, string density)
        {
            // Act & Assert - DaemonManager doesn't validate parameters in constructor
            var daemonManager = new DaemonManager(_testDirectory, width, height, unit, density);
            daemonManager.Should().NotBeNull();
        }

        [Fact]
        public void Start_WithVeryLongPath_HandlesCorrectly()
        {
            // Arrange
            var veryLongPath = Path.Combine(_testDirectory, new string('a', 200));
            Directory.CreateDirectory(veryLongPath);
            var daemonManager = new DaemonManager(veryLongPath, "100", "200", "mm", "203");

            // Ensure no daemon is running
            daemonManager.Stop();

            // Act
            var result = daemonManager.Start();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void MultipleInstances_WithSameFolder_HandlesCorrectly()
        {
            // Arrange
            var daemonManager1 = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
            var daemonManager2 = new DaemonManager(_testDirectory, "150", "250", "in", "300");
            
            // Ensure no daemon is running
            daemonManager1.Stop();
            daemonManager2.Stop();

            // Act
            var result1 = daemonManager1.Start();
            var result2 = daemonManager2.Start();

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeFalse(); // Second instance should fail because first is running
        }

        #endregion

        public void Dispose()
        {
            // Stop any running daemon before cleanup
            try
            {
                var daemonManager = new DaemonManager(_testDirectory, "100", "200", "mm", "203");
                daemonManager.Stop();
            }
            catch
            {
                // Ignore errors during cleanup
            }

            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
