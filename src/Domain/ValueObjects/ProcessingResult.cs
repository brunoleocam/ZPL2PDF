using System;
using System.Collections.Generic;

namespace ZPL2PDF.Domain.ValueObjects
{
    /// <summary>
    /// Represents the result of a processing operation
    /// </summary>
    public class ProcessingResult
    {
        /// <summary>
        /// Gets or sets whether the processing was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the error message if processing failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the output file path
        /// </summary>
        public string OutputFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of images processed
        /// </summary>
        public int ImagesProcessed { get; set; }

        /// <summary>
        /// Gets or sets the processing duration
        /// </summary>
        public TimeSpan ProcessingDuration { get; set; }

        /// <summary>
        /// Gets or sets the processing start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the processing end time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets additional metadata
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of ProcessingResult
        /// </summary>
        public ProcessingResult()
        {
        }

        /// <summary>
        /// Creates a successful processing result
        /// </summary>
        /// <param name="outputFilePath">Output file path</param>
        /// <param name="imagesProcessed">Number of images processed</param>
        /// <param name="startTime">Processing start time</param>
        /// <param name="endTime">Processing end time</param>
        /// <returns>Successful ProcessingResult</returns>
        public static ProcessingResult Success(string outputFilePath, int imagesProcessed, DateTime startTime, DateTime endTime)
        {
            return new ProcessingResult
            {
                IsSuccess = true,
                OutputFilePath = outputFilePath,
                ImagesProcessed = imagesProcessed,
                StartTime = startTime,
                EndTime = endTime,
                ProcessingDuration = endTime - startTime
            };
        }

        /// <summary>
        /// Creates a failed processing result
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="startTime">Processing start time</param>
        /// <param name="endTime">Processing end time</param>
        /// <returns>Failed ProcessingResult</returns>
        public static ProcessingResult Failure(string errorMessage, DateTime startTime, DateTime endTime)
        {
            return new ProcessingResult
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                StartTime = startTime,
                EndTime = endTime,
                ProcessingDuration = endTime - startTime
            };
        }

        /// <summary>
        /// Creates a failed processing result with exception
        /// </summary>
        /// <param name="exception">Exception that occurred</param>
        /// <param name="startTime">Processing start time</param>
        /// <param name="endTime">Processing end time</param>
        /// <returns>Failed ProcessingResult</returns>
        public static ProcessingResult Failure(Exception exception, DateTime startTime, DateTime endTime)
        {
            return new ProcessingResult
            {
                IsSuccess = false,
                ErrorMessage = exception?.Message ?? "Unknown error occurred",
                StartTime = startTime,
                EndTime = endTime,
                ProcessingDuration = endTime - startTime
            };
        }

        /// <summary>
        /// Adds metadata to the result
        /// </summary>
        /// <param name="key">Metadata key</param>
        /// <param name="value">Metadata value</param>
        public void AddMetadata(string key, object value)
        {
            Metadata[key] = value;
        }

        /// <summary>
        /// Gets metadata value
        /// </summary>
        /// <param name="key">Metadata key</param>
        /// <returns>Metadata value or null if not found</returns>
        public object? GetMetadata(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
                
            return Metadata.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Gets metadata value as string
        /// </summary>
        /// <param name="key">Metadata key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Metadata value as string</returns>
        public string GetMetadataString(string key, string defaultValue = "")
        {
            var value = GetMetadata(key);
            return value?.ToString() ?? defaultValue;
        }

        /// <summary>
        /// Gets metadata value as integer
        /// </summary>
        /// <param name="key">Metadata key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Metadata value as integer</returns>
        public int GetMetadataInt(string key, int defaultValue = 0)
        {
            var value = GetMetadata(key);
            if (value is int intValue)
                return intValue;
            if (int.TryParse(value?.ToString(), out var parsedValue))
                return parsedValue;
            return defaultValue;
        }

        /// <summary>
        /// Creates a copy of the processing result
        /// </summary>
        /// <returns>New instance with same values</returns>
        public ProcessingResult Clone()
        {
            return new ProcessingResult
            {
                IsSuccess = IsSuccess,
                ErrorMessage = ErrorMessage,
                OutputFilePath = OutputFilePath,
                ImagesProcessed = ImagesProcessed,
                ProcessingDuration = ProcessingDuration,
                StartTime = StartTime,
                EndTime = EndTime,
                Metadata = new Dictionary<string, object>(Metadata)
            };
        }

        /// <summary>
        /// Returns a string representation of the processing result
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            if (IsSuccess)
            {
                return $"ProcessingResult: Success - {ImagesProcessed} images processed in {ProcessingDuration.TotalMilliseconds:F0}ms";
            }
            else
            {
                return $"ProcessingResult: Failed - {ErrorMessage}";
            }
        }
    }
}
