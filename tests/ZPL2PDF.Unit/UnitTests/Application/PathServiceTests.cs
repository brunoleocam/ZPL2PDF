using System;
using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF.Tests.UnitTests.Application
{
    /// <summary>
    /// Unit tests for PathService
    /// </summary>
    public class PathServiceTests : IDisposable
    {
        private readonly PathService _pathService;
        private readonly string _testDirectory;

        public PathServiceTests()
        {
            _pathService = new PathService();
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_Tests", Guid.NewGuid().ToString());
        }

        #region EnsureDirectoryExists Tests

        [Fact]
        public void EnsureDirectoryExists_WithValidPath_CreatesDirectory()
        {
            // Arrange
            var directoryPath = Path.Combine(_testDirectory, "new_directory");

            // Act
            _pathService.EnsureDirectoryExists(directoryPath);

            // Assert
            Directory.Exists(directoryPath).Should().BeTrue();
        }

        [Fact]
        public void EnsureDirectoryExists_WithExistingDirectory_DoesNothing()
        {
            // Arrange
            var directoryPath = Path.Combine(_testDirectory, "existing_directory");
            Directory.CreateDirectory(directoryPath);

            // Act
            _pathService.EnsureDirectoryExists(directoryPath);

            // Assert
            Directory.Exists(directoryPath).Should().BeTrue();
        }

        [Fact]
        public void EnsureDirectoryExists_WithNullPath_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _pathService.EnsureDirectoryExists(null));
            
            exception.ParamName.Should().Be("directoryPath");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public void EnsureDirectoryExists_WithEmptyPath_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _pathService.EnsureDirectoryExists(""));
            
            exception.ParamName.Should().Be("directoryPath");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        [Fact]
        public void EnsureDirectoryExists_WithWhitespacePath_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                _pathService.EnsureDirectoryExists("   "));
            
            exception.ParamName.Should().Be("directoryPath");
            exception.Message.Should().Contain("cannot be null or empty");
        }

        #endregion

        #region GetDefaultListenFolder Tests

        [Fact]
        public void GetDefaultListenFolder_ReturnsDocumentsPath()
        {
            // Act
            var result = _pathService.GetDefaultListenFolder();

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("Documents");
            result.Should().Contain("ZPL2PDF Auto Converter");
        }

        [Fact]
        public void GetDefaultListenFolder_ContainsZPL2PDFFolder()
        {
            // Act
            var result = _pathService.GetDefaultListenFolder();

            // Assert
            result.Should().EndWith("ZPL2PDF Auto Converter");
        }

        #endregion

        #region GetConfigFolder Tests

        [Fact]
        public void GetConfigFolder_OnWindows_ReturnsAppDataPath()
        {
            // Act
            var result = _pathService.GetConfigFolder();

            // Assert
            result.Should().NotBeNullOrEmpty();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                result.Should().Contain("AppData");
                result.Should().Contain("ZPL2PDF");
            }
        }

        [Fact]
        public void GetConfigFolder_OnLinux_ReturnsConfigPath()
        {
            // Act
            var result = _pathService.GetConfigFolder();

            // Assert
            result.Should().NotBeNullOrEmpty();
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                result.Should().Contain(".config");
                result.Should().Contain("zpl2pdf");
            }
        }

        #endregion

        #region GetPidFolder Tests

        [Fact]
        public void GetPidFolder_OnWindows_ReturnsTempPath()
        {
            // Act
            var result = _pathService.GetPidFolder();

            // Assert
            result.Should().NotBeNullOrEmpty();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                result.Should().Be(Path.GetTempPath());
            }
        }

        [Fact]
        public void GetPidFolder_OnLinux_ReturnsVarRunPath()
        {
            // Act
            var result = _pathService.GetPidFolder();

            // Assert
            result.Should().NotBeNullOrEmpty();
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                result.Should().Be("/var/run");
            }
        }

        #endregion

        #region Combine Tests

        [Fact]
        public void Combine_WithValidPaths_ReturnsCombinedPath()
        {
            // Arrange
            var path1 = "C:\\test";
            var path2 = "subfolder";

            // Act
            var result = _pathService.Combine(path1, path2);

            // Assert
            result.Should().Be(Path.Combine(path1, path2));
        }

        [Fact]
        public void Combine_WithMultiplePaths_ReturnsCombinedPath()
        {
            // Arrange
            var path1 = "C:\\test";
            var path2 = "subfolder";
            var expected = Path.Combine(path1, path2);

            // Act
            var result = _pathService.Combine(path1, path2);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Combine_WithEmptyPaths_ReturnsEmptyPath()
        {
            // Act
            var result = _pathService.Combine("", "");

            // Assert
            result.Should().Be(Path.Combine("", ""));
        }

        #endregion

        #region GetDirectoryName Tests

        [Fact]
        public void GetDirectoryName_WithValidPath_ReturnsDirectoryName()
        {
            // Arrange
            var filePath = "C:\\test\\subfolder\\file.txt";
            var expected = "C:\\test\\subfolder";

            // Act
            var result = _pathService.GetDirectoryName(filePath);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetDirectoryName_WithRootPath_ReturnsEmpty()
        {
            // Arrange
            var filePath = "C:\\file.txt";

            // Act
            var result = _pathService.GetDirectoryName(filePath);

            // Assert
            result.Should().Be("C:\\");
        }

        [Fact]
        public void GetDirectoryName_WithRelativePath_ReturnsDirectoryName()
        {
            // Arrange
            var filePath = "test\\subfolder\\file.txt";
            var expected = "test\\subfolder";

            // Act
            var result = _pathService.GetDirectoryName(filePath);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetDirectoryName_WithNullPath_ReturnsEmpty()
        {
            // Act
            var result = _pathService.GetDirectoryName(null);

            // Assert
            result.Should().Be(string.Empty);
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
