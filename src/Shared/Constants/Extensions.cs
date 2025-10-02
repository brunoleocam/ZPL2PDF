using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZPL2PDF.Shared.Constants
{
    /// <summary>
    /// Useful extension methods for the application
    /// </summary>
    public static class Extensions
    {
        #region String Extensions
        /// <summary>
        /// Checks if a string is null, empty, or whitespace
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>True if null, empty, or whitespace</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Checks if a string is not null, empty, or whitespace
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>True if not null, empty, or whitespace</returns>
        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Safely converts string to double with default value
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="defaultValue">Default value if conversion fails</param>
        /// <returns>Converted double or default value</returns>
        public static double ToDoubleOrDefault(this string value, double defaultValue = 0.0)
        {
            if (double.TryParse(value, out double result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Safely converts string to int with default value
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="defaultValue">Default value if conversion fails</param>
        /// <returns>Converted int or default value</returns>
        public static int ToIntOrDefault(this string value, int defaultValue = 0)
        {
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Checks if a string is a valid unit
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>True if valid unit</returns>
        public static bool IsValidUnit(this string value)
        {
            return RegexPatterns.IsValidUnit(value);
        }

        /// <summary>
        /// Checks if a string is a valid log level
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>True if valid log level</returns>
        public static bool IsValidLogLevel(this string value)
        {
            return RegexPatterns.IsValidLogLevel(value);
        }

        /// <summary>
        /// Checks if a string is a valid file extension
        /// </summary>
        /// <param name="value">String to check</param>
        /// <returns>True if valid file extension</returns>
        public static bool IsValidFileExtension(this string value)
        {
            return RegexPatterns.IsValidFileExtension(value);
        }

        /// <summary>
        /// Truncates a string to specified length
        /// </summary>
        /// <param name="value">String to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="suffix">Suffix to add if truncated</param>
        /// <returns>Truncated string</returns>
        public static string Truncate(this string? value, int maxLength, string suffix = "...")
        {
            if (value == null || value.Length <= maxLength)
                return value ?? string.Empty;

            return value.Substring(0, maxLength - suffix.Length) + suffix;
        }

        /// <summary>
        /// Ensures a string ends with a specific character
        /// </summary>
        /// <param name="value">String to check</param>
        /// <param name="suffix">Suffix to ensure</param>
        /// <returns>String with ensured suffix</returns>
        public static string EnsureEndsWith(this string value, string suffix)
        {
            if (value == null)
                return suffix;

            if (value.EndsWith(suffix))
                return value;

            return value + suffix;
        }

        /// <summary>
        /// Ensures a string starts with a specific character
        /// </summary>
        /// <param name="value">String to check</param>
        /// <param name="prefix">Prefix to ensure</param>
        /// <returns>String with ensured prefix</returns>
        public static string EnsureStartsWith(this string value, string prefix)
        {
            if (value == null)
                return prefix;

            if (value.StartsWith(prefix))
                return value;

            return prefix + value;
        }
        #endregion

        #region Double Extensions
        /// <summary>
        /// Checks if a double value is valid for dimensions
        /// </summary>
        /// <param name="value">Double value to check</param>
        /// <returns>True if valid dimension</returns>
        public static bool IsValidDimension(this double value)
        {
            return RegexPatterns.IsValidDimension(value);
        }

        /// <summary>
        /// Clamps a double value between min and max
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Rounds a double to specified decimal places
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="decimalPlaces">Number of decimal places</param>
        /// <returns>Rounded value</returns>
        public static double RoundTo(this double value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces);
        }

        /// <summary>
        /// Checks if a double value is approximately equal to another
        /// </summary>
        /// <param name="value">First value</param>
        /// <param name="other">Second value</param>
        /// <param name="tolerance">Tolerance for comparison</param>
        /// <returns>True if approximately equal</returns>
        public static bool IsApproximatelyEqual(this double value, double other, double tolerance = 0.0001)
        {
            return Math.Abs(value - other) < tolerance;
        }
        #endregion

        #region Int Extensions
        /// <summary>
        /// Checks if an int value is valid for DPI
        /// </summary>
        /// <param name="value">Int value to check</param>
        /// <returns>True if valid DPI</returns>
        public static bool IsValidDpi(this int value)
        {
            return RegexPatterns.IsValidDpi(value);
        }

        /// <summary>
        /// Clamps an int value between min and max
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Checks if an int value is within range
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>True if within range</returns>
        public static bool IsInRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        #endregion

        #region File System Extensions
        /// <summary>
        /// Checks if a file path has a valid extension
        /// </summary>
        /// <param name="filePath">File path to check</param>
        /// <returns>True if valid extension</returns>
        public static bool HasValidExtension(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var extension = Path.GetExtension(filePath);
            return extension.IsValidFileExtension();
        }

        /// <summary>
        /// Gets file size in human readable format
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Human readable file size</returns>
        public static string GetHumanReadableSize(this string filePath)
        {
            if (!File.Exists(filePath))
                return "File not found";

            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length.GetHumanReadableSize();
        }

        /// <summary>
        /// Gets file size in human readable format
        /// </summary>
        /// <param name="bytes">File size in bytes</param>
        /// <returns>Human readable file size</returns>
        public static string GetHumanReadableSize(this long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Ensures directory exists
        /// </summary>
        /// <param name="directoryPath">Directory path</param>
        public static void EnsureDirectoryExists(this string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// Checks if a path is a valid directory
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if valid directory</returns>
        public static bool IsValidDirectory(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                var directoryInfo = new DirectoryInfo(path);
                return directoryInfo.Exists;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a path is a valid file
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>True if valid file</returns>
        public static bool IsValidFile(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                var fileInfo = new FileInfo(path);
                return fileInfo.Exists;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Collection Extensions
        /// <summary>
        /// Checks if a collection is null or empty
        /// </summary>
        /// <typeparam name="T">Type of collection elements</typeparam>
        /// <param name="collection">Collection to check</param>
        /// <returns>True if null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// Checks if a collection is not null and not empty
        /// </summary>
        /// <typeparam name="T">Type of collection elements</typeparam>
        /// <param name="collection">Collection to check</param>
        /// <returns>True if not null and not empty</returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Any();
        }

        /// <summary>
        /// Safely gets the first element or default
        /// </summary>
        /// <typeparam name="T">Type of collection elements</typeparam>
        /// <param name="collection">Collection to get element from</param>
        /// <param name="defaultValue">Default value if empty</param>
        /// <returns>First element or default</returns>
        public static T FirstOrDefault<T>(this IEnumerable<T> collection, T defaultValue)
        {
            if (collection == null)
                return defaultValue;

            var enumerator = collection.GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current;

            return defaultValue;
        }
        #endregion

        #region DateTime Extensions
        /// <summary>
        /// Gets a human readable time difference
        /// </summary>
        /// <param name="dateTime">DateTime to compare</param>
        /// <returns>Human readable time difference</returns>
        public static string GetTimeAgo(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalDays > 1)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalHours > 1)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalMinutes > 1)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalSeconds > 1)
                return $"{(int)timeSpan.TotalSeconds} seconds ago";

            return "just now";
        }

        /// <summary>
        /// Checks if a DateTime is today
        /// </summary>
        /// <param name="dateTime">DateTime to check</param>
        /// <returns>True if today</returns>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// Checks if a DateTime is yesterday
        /// </summary>
        /// <param name="dateTime">DateTime to check</param>
        /// <returns>True if yesterday</returns>
        public static bool IsYesterday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today.AddDays(-1);
        }
        #endregion
    }
}
