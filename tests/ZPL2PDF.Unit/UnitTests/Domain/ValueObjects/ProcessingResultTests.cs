using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Domain.ValueObjects;

namespace ZPL2PDF.Tests.UnitTests.Domain.ValueObjects
{
    /// <summary>
    /// Unit tests for ProcessingResult
    /// </summary>
    public class ProcessingResultTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_Default_InitializesWithDefaultValues()
        {
            // Act
            var result = new ProcessingResult();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(string.Empty);
            result.OutputFilePath.Should().Be(string.Empty);
            result.ImagesProcessed.Should().Be(0);
            result.ProcessingDuration.Should().Be(TimeSpan.Zero);
            result.StartTime.Should().Be(default(DateTime));
            result.EndTime.Should().Be(default(DateTime));
            result.Metadata.Should().NotBeNull();
            result.Metadata.Should().BeEmpty();
        }

        #endregion

        #region Success Factory Method Tests

        [Fact]
        public void Success_WithValidParameters_ReturnsSuccessfulResult()
        {
            // Arrange
            var outputFilePath = "C:\\Output\\test.pdf";
            var imagesProcessed = 3;
            var startTime = DateTime.Now.AddMinutes(-1);
            var endTime = DateTime.Now;

            // Act
            var result = ProcessingResult.Success(outputFilePath, imagesProcessed, startTime, endTime);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.OutputFilePath.Should().Be(outputFilePath);
            result.ImagesProcessed.Should().Be(imagesProcessed);
            result.StartTime.Should().Be(startTime);
            result.EndTime.Should().Be(endTime);
            result.ProcessingDuration.Should().Be(endTime - startTime);
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void Success_WithZeroImages_ReturnsSuccessfulResult()
        {
            // Arrange
            var outputFilePath = "C:\\Output\\test.pdf";
            var imagesProcessed = 0;
            var startTime = DateTime.Now.AddMinutes(-1);
            var endTime = DateTime.Now;

            // Act
            var result = ProcessingResult.Success(outputFilePath, imagesProcessed, startTime, endTime);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ImagesProcessed.Should().Be(0);
        }

        [Fact]
        public void Success_WithSameStartAndEndTime_ReturnsZeroDuration()
        {
            // Arrange
            var outputFilePath = "C:\\Output\\test.pdf";
            var imagesProcessed = 1;
            var startTime = DateTime.Now;
            var endTime = startTime;

            // Act
            var result = ProcessingResult.Success(outputFilePath, imagesProcessed, startTime, endTime);

            // Assert
            result.ProcessingDuration.Should().Be(TimeSpan.Zero);
        }

        #endregion

        #region Failure Factory Method Tests

        [Fact]
        public void Failure_WithErrorMessage_ReturnsFailedResult()
        {
            // Arrange
            var errorMessage = "File not found";
            var startTime = DateTime.Now.AddMinutes(-1);
            var endTime = DateTime.Now;

            // Act
            var result = ProcessingResult.Failure(errorMessage, startTime, endTime);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(errorMessage);
            result.StartTime.Should().Be(startTime);
            result.EndTime.Should().Be(endTime);
            result.ProcessingDuration.Should().Be(endTime - startTime);
            result.OutputFilePath.Should().BeEmpty();
            result.ImagesProcessed.Should().Be(0);
        }

        [Fact]
        public void Failure_WithEmptyErrorMessage_ReturnsFailedResult()
        {
            // Arrange
            var errorMessage = "";
            var startTime = DateTime.Now.AddMinutes(-1);
            var endTime = DateTime.Now;

            // Act
            var result = ProcessingResult.Failure(errorMessage, startTime, endTime);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void Failure_WithException_ReturnsFailedResult()
        {
            // Arrange
            var exception = new ArgumentException("Invalid parameter");
            var startTime = DateTime.Now.AddMinutes(-1);
            var endTime = DateTime.Now;

            // Act
            var result = ProcessingResult.Failure(exception, startTime, endTime);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(exception.Message);
            result.StartTime.Should().Be(startTime);
            result.EndTime.Should().Be(endTime);
            result.ProcessingDuration.Should().Be(endTime - startTime);
        }

        [Fact]
        public void Failure_WithNullException_ReturnsFailedResult()
        {
            // Arrange
            Exception? exception = null;
            var startTime = DateTime.Now.AddMinutes(-1);
            var endTime = DateTime.Now;

            // Act
            var result = ProcessingResult.Failure(exception!, startTime, endTime);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Unknown error occurred");
        }

        #endregion

        #region AddMetadata Tests

        [Fact]
        public void AddMetadata_WithValidKeyValue_AddsToMetadata()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var value = 1024;

            // Act
            result.AddMetadata(key, value);

            // Assert
            result.Metadata.Should().ContainKey(key);
            result.Metadata[key].Should().Be(value);
        }

        [Fact]
        public void AddMetadata_WithExistingKey_UpdatesValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var initialValue = 1024;
            var updatedValue = 2048;

            // Act
            result.AddMetadata(key, initialValue);
            result.AddMetadata(key, updatedValue);

            // Assert
            result.Metadata.Should().ContainKey(key);
            result.Metadata[key].Should().Be(updatedValue);
            result.Metadata.Should().HaveCount(1);
        }

        [Fact]
        public void AddMetadata_WithNullKey_ThrowsException()
        {
            // Arrange
            var result = new ProcessingResult();
            string? key = null;
            var value = 1024;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => result.AddMetadata(key!, value));
            exception.ParamName.Should().Be("key");
        }

        [Fact]
        public void AddMetadata_WithEmptyKey_AddsToMetadata()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "";
            var value = 1024;

            // Act
            result.AddMetadata(key, value);

            // Assert
            result.Metadata.Should().ContainKey(key);
            result.Metadata[key].Should().Be(value);
        }

        #endregion

        #region GetMetadata Tests

        [Fact]
        public void GetMetadata_WithExistingKey_ReturnsValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var value = 1024;
            result.AddMetadata(key, value);

            // Act
            var retrievedValue = result.GetMetadata(key);

            // Assert
            retrievedValue.Should().Be(value);
        }

        [Fact]
        public void GetMetadata_WithNonExistentKey_ReturnsNull()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "nonExistent";

            // Act
            var retrievedValue = result.GetMetadata(key);

            // Assert
            retrievedValue.Should().BeNull();
        }

        [Fact]
        public void GetMetadata_WithNullKey_ReturnsNull()
        {
            // Arrange
            var result = new ProcessingResult();

            // Act
            var retrievedValue = result.GetMetadata(null!);

            // Assert
            retrievedValue.Should().BeNull();
        }

        #endregion

        #region GetMetadataString Tests

        [Fact]
        public void GetMetadataString_WithExistingKey_ReturnsStringValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileName";
            var value = "test.pdf";
            result.AddMetadata(key, value);

            // Act
            var retrievedValue = result.GetMetadataString(key);

            // Assert
            retrievedValue.Should().Be(value);
        }

        [Fact]
        public void GetMetadataString_WithNonExistentKey_ReturnsDefaultValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "nonExistent";
            var defaultValue = "default.pdf";

            // Act
            var retrievedValue = result.GetMetadataString(key, defaultValue);

            // Assert
            retrievedValue.Should().Be(defaultValue);
        }

        [Fact]
        public void GetMetadataString_WithNonExistentKeyAndNoDefault_ReturnsEmptyString()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "nonExistent";

            // Act
            var retrievedValue = result.GetMetadataString(key);

            // Assert
            retrievedValue.Should().BeEmpty();
        }

        [Fact]
        public void GetMetadataString_WithIntValue_ReturnsStringRepresentation()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var value = 1024;
            result.AddMetadata(key, value);

            // Act
            var retrievedValue = result.GetMetadataString(key);

            // Assert
            retrievedValue.Should().Be("1024");
        }

        #endregion

        #region GetMetadataInt Tests

        [Fact]
        public void GetMetadataInt_WithIntValue_ReturnsIntValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var value = 1024;
            result.AddMetadata(key, value);

            // Act
            var retrievedValue = result.GetMetadataInt(key);

            // Assert
            retrievedValue.Should().Be(value);
        }

        [Fact]
        public void GetMetadataInt_WithStringValue_ReturnsParsedInt()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var value = "2048";
            result.AddMetadata(key, value);

            // Act
            var retrievedValue = result.GetMetadataInt(key);

            // Assert
            retrievedValue.Should().Be(2048);
        }

        [Fact]
        public void GetMetadataInt_WithNonExistentKey_ReturnsDefaultValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "nonExistent";
            var defaultValue = 999;

            // Act
            var retrievedValue = result.GetMetadataInt(key, defaultValue);

            // Assert
            retrievedValue.Should().Be(defaultValue);
        }

        [Fact]
        public void GetMetadataInt_WithNonExistentKeyAndNoDefault_ReturnsZero()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "nonExistent";

            // Act
            var retrievedValue = result.GetMetadataInt(key);

            // Assert
            retrievedValue.Should().Be(0);
        }

        [Fact]
        public void GetMetadataInt_WithInvalidStringValue_ReturnsDefaultValue()
        {
            // Arrange
            var result = new ProcessingResult();
            var key = "fileSize";
            var value = "invalid";
            result.AddMetadata(key, value);

            // Act
            var retrievedValue = result.GetMetadataInt(key, 999);

            // Assert
            retrievedValue.Should().Be(999);
        }

        #endregion

        #region Clone Tests

        [Fact]
        public void Clone_WithValidResult_ReturnsExactCopy()
        {
            // Arrange
            var original = new ProcessingResult
            {
                IsSuccess = true,
                ErrorMessage = "Test error",
                OutputFilePath = "C:\\test.pdf",
                ImagesProcessed = 5,
                ProcessingDuration = TimeSpan.FromSeconds(10),
                StartTime = DateTime.Now.AddMinutes(-10),
                EndTime = DateTime.Now
            };
            original.AddMetadata("key1", "value1");
            original.AddMetadata("key2", 123);

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.IsSuccess.Should().Be(original.IsSuccess);
            clone.ErrorMessage.Should().Be(original.ErrorMessage);
            clone.OutputFilePath.Should().Be(original.OutputFilePath);
            clone.ImagesProcessed.Should().Be(original.ImagesProcessed);
            clone.ProcessingDuration.Should().Be(original.ProcessingDuration);
            clone.StartTime.Should().Be(original.StartTime);
            clone.EndTime.Should().Be(original.EndTime);
            clone.Metadata.Should().NotBeSameAs(original.Metadata);
            clone.Metadata.Should().BeEquivalentTo(original.Metadata);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_WithSuccessResult_ReturnsSuccessString()
        {
            // Arrange
            var result = new ProcessingResult
            {
                IsSuccess = true,
                ImagesProcessed = 3,
                ProcessingDuration = TimeSpan.FromMilliseconds(1500)
            };

            // Act
            var resultString = result.ToString();

            // Assert
            resultString.Should().Contain("ProcessingResult");
            resultString.Should().Contain("Success");
            resultString.Should().Contain("3 images processed");
            resultString.Should().Contain("1500ms");
        }

        [Fact]
        public void ToString_WithFailureResult_ReturnsFailureString()
        {
            // Arrange
            var result = new ProcessingResult
            {
                IsSuccess = false,
                ErrorMessage = "File not found"
            };

            // Act
            var resultString = result.ToString();

            // Assert
            resultString.Should().Contain("ProcessingResult");
            resultString.Should().Contain("Failed");
            resultString.Should().Contain("File not found");
        }

        [Fact]
        public void ToString_WithZeroImages_ReturnsZeroImagesString()
        {
            // Arrange
            var result = new ProcessingResult
            {
                IsSuccess = true,
                ImagesProcessed = 0,
                ProcessingDuration = TimeSpan.FromMilliseconds(500)
            };

            // Act
            var resultString = result.ToString();

            // Assert
            resultString.Should().Contain("0 images processed");
        }

        #endregion

        #region Property Setting Tests

        [Fact]
        public void SetProperties_WithValidValues_UpdatesCorrectly()
        {
            // Arrange
            var result = new ProcessingResult();
            var startTime = DateTime.Now.AddMinutes(-5);
            var endTime = DateTime.Now;

            // Act
            result.IsSuccess = true;
            result.ErrorMessage = "Test error";
            result.OutputFilePath = "C:\\test.pdf";
            result.ImagesProcessed = 10;
            result.ProcessingDuration = TimeSpan.FromMinutes(5);
            result.StartTime = startTime;
            result.EndTime = endTime;

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().Be("Test error");
            result.OutputFilePath.Should().Be("C:\\test.pdf");
            result.ImagesProcessed.Should().Be(10);
            result.ProcessingDuration.Should().Be(TimeSpan.FromMinutes(5));
            result.StartTime.Should().Be(startTime);
            result.EndTime.Should().Be(endTime);
        }

        #endregion
    }
}
