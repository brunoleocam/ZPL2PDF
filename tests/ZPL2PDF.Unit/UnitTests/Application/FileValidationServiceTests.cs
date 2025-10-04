using System;
using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Application.Interfaces;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF.Tests.UnitTests.Application
{
    /// <summary>
    /// Unit tests for FileValidationService
    /// </summary>
    public class FileValidationServiceTests : IDisposable
    {
        private readonly IFileValidationService _fileValidationService;
        private readonly string _testDirectory;

        public FileValidationServiceTests()
        {
            _fileValidationService = new FileValidationService();
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [Fact]
        public void IsValidFile_WithValidTxtFile_ReturnsTrue()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "test content");

            // Act
            var result = _fileValidationService.IsValidFile(testFile);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidFile_WithValidPrnFile_ReturnsTrue()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.prn");
            File.WriteAllText(testFile, "test content");

            // Act
            var result = _fileValidationService.IsValidFile(testFile);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidFile_WithInvalidExtension_ReturnsFalse()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test.doc");
            File.WriteAllText(testFile, "test content");

            // Act
            var result = _fileValidationService.IsValidFile(testFile);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidFile_WithNonExistentFile_ReturnsFalse()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            var result = _fileValidationService.IsValidFile(testFile);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsFileLocked_WithUnlockedFile_ReturnsFalse()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "unlocked.txt");
            File.WriteAllText(testFile, "test content");

            // Act
            var result = _fileValidationService.IsFileLocked(testFile);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsFileLocked_WithNonExistentFile_ReturnsTrue()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act
            var result = _fileValidationService.IsFileLocked(testFile);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidExtension_WithValidExtensions_ReturnsTrue()
        {
            // Arrange
            var txtFile = Path.Combine(_testDirectory, "test.txt");
            var prnFile = Path.Combine(_testDirectory, "test.prn");

            // Act & Assert
            _fileValidationService.IsValidExtension(txtFile).Should().BeTrue();
            _fileValidationService.IsValidExtension(prnFile).Should().BeTrue();
        }

        [Fact]
        public void IsValidExtension_WithInvalidExtensions_ReturnsFalse()
        {
            // Arrange
            var docFile = Path.Combine(_testDirectory, "test.doc");
            var pdfFile = Path.Combine(_testDirectory, "test.pdf");

            // Act & Assert
            _fileValidationService.IsValidExtension(docFile).Should().BeFalse();
            _fileValidationService.IsValidExtension(pdfFile).Should().BeFalse();
        }

        [Fact]
        public void IsValidExtension_WithNullPath_ReturnsFalse()
        {
            // Act
            var result = _fileValidationService.IsValidExtension(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidExtension_WithEmptyPath_ReturnsFalse()
        {
            // Act
            var result = _fileValidationService.IsValidExtension("");

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
