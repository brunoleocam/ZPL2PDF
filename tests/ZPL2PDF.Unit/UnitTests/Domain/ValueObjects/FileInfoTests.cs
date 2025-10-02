using System;
using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Domain.ValueObjects;
using FileInfo = ZPL2PDF.Domain.ValueObjects.FileInfo;

namespace ZPL2PDF.Tests.UnitTests.Domain.ValueObjects
{
    /// <summary>
    /// Unit tests for FileInfo
    /// </summary>
    public class FileInfoTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFile;

        public FileInfoTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _testFile = Path.Combine(_testDirectory, "test.txt");
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_Default_InitializesWithDefaultValues()
        {
            // Act
            var fileInfo = new FileInfo();

            // Assert
            fileInfo.FilePath.Should().Be(string.Empty);
            fileInfo.FileName.Should().Be(string.Empty);
            fileInfo.Extension.Should().Be(string.Empty);
            fileInfo.Size.Should().Be(0);
            fileInfo.CreatedTime.Should().Be(default(DateTime));
            fileInfo.LastModifiedTime.Should().Be(default(DateTime));
            fileInfo.Exists.Should().BeFalse();
            fileInfo.IsLocked.Should().BeFalse();
            fileInfo.Content.Should().Be(string.Empty);
        }

        [Fact]
        public void Constructor_WithValidFilePath_SetsPropertiesCorrectly()
        {
            // Arrange
            var content = "Test ZPL content";
            File.WriteAllText(_testFile, content);

            // Act
            var fileInfo = new FileInfo(_testFile);

            // Assert
            fileInfo.FilePath.Should().Be(_testFile);
            fileInfo.FileName.Should().Be("test.txt");
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Exists.Should().BeTrue();
            fileInfo.Size.Should().BeGreaterThan(0);
            fileInfo.CreatedTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
            fileInfo.LastModifiedTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void Constructor_WithNonExistentFilePath_SetsExistsToFalse()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            var fileInfo = new FileInfo(nonExistentFile);

            // Assert
            fileInfo.FilePath.Should().Be(nonExistentFile);
            fileInfo.FileName.Should().Be("nonexistent.txt");
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Exists.Should().BeFalse();
            fileInfo.Size.Should().Be(0);
        }

        #endregion

        #region FromPath Tests

        [Fact]
        public void FromPath_WithValidPathAndContent_ReturnsFileInfoWithContent()
        {
            // Arrange
            var content = "ZPL content for testing";
            File.WriteAllText(_testFile, content);

            // Act
            var fileInfo = FileInfo.FromPath(_testFile, content);

            // Assert
            fileInfo.FilePath.Should().Be(_testFile);
            fileInfo.FileName.Should().Be("test.txt");
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Exists.Should().BeTrue();
            fileInfo.Content.Should().Be(content);
        }

        [Fact]
        public void FromPath_WithValidPathAndNullContent_ReturnsFileInfoWithoutContent()
        {
            // Arrange
            var content = "ZPL content for testing";
            File.WriteAllText(_testFile, content);

            // Act
            var fileInfo = FileInfo.FromPath(_testFile, null);

            // Assert
            fileInfo.FilePath.Should().Be(_testFile);
            fileInfo.FileName.Should().Be("test.txt");
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Exists.Should().BeTrue();
            fileInfo.Content.Should().Be(string.Empty);
        }

        [Fact]
        public void FromPath_WithValidPathAndEmptyContent_ReturnsFileInfoWithoutContent()
        {
            // Arrange
            var content = "ZPL content for testing";
            File.WriteAllText(_testFile, content);

            // Act
            var fileInfo = FileInfo.FromPath(_testFile, "");

            // Assert
            fileInfo.FilePath.Should().Be(_testFile);
            fileInfo.FileName.Should().Be("test.txt");
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Exists.Should().BeTrue();
            fileInfo.Content.Should().Be(string.Empty);
        }

        #endregion

        #region FromContent Tests

        [Fact]
        public void FromContent_WithValidFileNameAndContent_ReturnsFileInfo()
        {
            // Arrange
            var fileName = "test.prn";
            var content = "ZPL content for testing";

            // Act
            var fileInfo = FileInfo.FromContent(fileName, content);

            // Assert
            fileInfo.FileName.Should().Be(fileName);
            fileInfo.Extension.Should().Be(".prn");
            fileInfo.Content.Should().Be(content);
            fileInfo.Exists.Should().BeTrue();
            fileInfo.CreatedTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
            fileInfo.LastModifiedTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void FromContent_WithTxtExtension_ReturnsFileInfoWithTxtExtension()
        {
            // Arrange
            var fileName = "test.txt";
            var content = "ZPL content";

            // Act
            var fileInfo = FileInfo.FromContent(fileName, content);

            // Assert
            fileInfo.FileName.Should().Be(fileName);
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Content.Should().Be(content);
        }

        #endregion

        #region IsValid Tests

        [Fact]
        public void IsValid_WithValidTxtFile_ReturnsTrue()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test.txt",
                Extension = ".txt"
            };

            // Act
            var result = fileInfo.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithValidPrnFile_ReturnsTrue()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test.prn",
                Extension = ".prn"
            };

            // Act
            var result = fileInfo.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithEmptyFileName_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "",
                Extension = ".txt"
            };

            // Act
            var result = fileInfo.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithNullFileName_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = null,
                Extension = ".txt"
            };

            // Act
            var result = fileInfo.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithEmptyExtension_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test",
                Extension = ""
            };

            // Act
            var result = fileInfo.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithInvalidExtension_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test.doc",
                Extension = ".doc"
            };

            // Act
            var result = fileInfo.IsValid();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region IsValidExtension Tests

        [Fact]
        public void IsValidExtension_WithTxtExtension_ReturnsTrue()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = ".txt" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidExtension_WithPrnExtension_ReturnsTrue()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = ".prn" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidExtension_WithTxtExtensionUpperCase_ReturnsTrue()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = ".TXT" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidExtension_WithPrnExtensionUpperCase_ReturnsTrue()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = ".PRN" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidExtension_WithDocExtension_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = ".doc" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidExtension_WithPdfExtension_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = ".pdf" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidExtension_WithEmptyExtension_ReturnsFalse()
        {
            // Arrange
            var fileInfo = new FileInfo { Extension = "" };

            // Act
            var result = fileInfo.IsValidExtension();

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetValidationError Tests

        [Fact]
        public void GetValidationError_WithValidFile_ReturnsEmptyString()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test.txt",
                Extension = ".txt"
            };

            // Act
            var result = fileInfo.GetValidationError();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetValidationError_WithEmptyFileName_ReturnsFileNameError()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "",
                Extension = ".txt"
            };

            // Act
            var result = fileInfo.GetValidationError();

            // Assert
            result.Should().Be("File name cannot be null or empty");
        }

        [Fact]
        public void GetValidationError_WithEmptyExtension_ReturnsExtensionError()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test",
                Extension = ""
            };

            // Act
            var result = fileInfo.GetValidationError();

            // Assert
            result.Should().Be("File extension cannot be null or empty");
        }

        [Fact]
        public void GetValidationError_WithInvalidExtension_ReturnsExtensionError()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test.doc",
                Extension = ".doc"
            };

            // Act
            var result = fileInfo.GetValidationError();

            // Assert
            result.Should().Be("Invalid file extension: .doc. Valid extensions are: .txt, .prn");
        }

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_WithValidFileInfo_ReturnsExactCopy()
        {
            // Arrange
            var original = new FileInfo
            {
                FilePath = "C:\\test\\file.txt",
                FileName = "file.txt",
                Extension = ".txt",
                Size = 1024,
                CreatedTime = DateTime.Now.AddDays(-1),
                LastModifiedTime = DateTime.Now,
                Exists = true,
                IsLocked = false,
                Content = "Test content"
            };

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.FilePath.Should().Be(original.FilePath);
            clone.FileName.Should().Be(original.FileName);
            clone.Extension.Should().Be(original.Extension);
            clone.Size.Should().Be(original.Size);
            clone.CreatedTime.Should().Be(original.CreatedTime);
            clone.LastModifiedTime.Should().Be(original.LastModifiedTime);
            clone.Exists.Should().Be(original.Exists);
            clone.IsLocked.Should().Be(original.IsLocked);
            clone.Content.Should().Be(original.Content);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_WithValidFileInfo_ReturnsFormattedString()
        {
            // Arrange
            var fileInfo = new FileInfo
            {
                FileName = "test.txt",
                Size = 1024,
                Extension = ".txt"
            };

            // Act
            var result = fileInfo.ToString();

            // Assert
            result.Should().Contain("FileInfo");
            result.Should().Contain("test.txt");
            result.Should().Contain("1024");
            result.Should().Contain(".txt");
        }

        #endregion

        #region Property Setting Tests

        [Fact]
        public void SetProperties_WithValidValues_UpdatesCorrectly()
        {
            // Arrange
            var fileInfo = new FileInfo();

            // Act
            fileInfo.FilePath = "C:\\test\\file.txt";
            fileInfo.FileName = "file.txt";
            fileInfo.Extension = ".txt";
            fileInfo.Size = 2048;
            fileInfo.CreatedTime = DateTime.Now.AddDays(-2);
            fileInfo.LastModifiedTime = DateTime.Now.AddDays(-1);
            fileInfo.Exists = true;
            fileInfo.IsLocked = true;
            fileInfo.Content = "Updated content";

            // Assert
            fileInfo.FilePath.Should().Be("C:\\test\\file.txt");
            fileInfo.FileName.Should().Be("file.txt");
            fileInfo.Extension.Should().Be(".txt");
            fileInfo.Size.Should().Be(2048);
            fileInfo.CreatedTime.Should().BeCloseTo(DateTime.Now.AddDays(-2), TimeSpan.FromMinutes(1));
            fileInfo.LastModifiedTime.Should().BeCloseTo(DateTime.Now.AddDays(-1), TimeSpan.FromMinutes(1));
            fileInfo.Exists.Should().BeTrue();
            fileInfo.IsLocked.Should().BeTrue();
            fileInfo.Content.Should().Be("Updated content");
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
