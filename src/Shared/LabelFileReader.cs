using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for reading the input file and splitting the ZPL labels.
    /// </summary>
    public static class LabelFileReader {
        /// <summary>
        /// Pre-compiled regex for better performance (compiled once, used many times).
        /// Pattern: ^FN followed by digits, optional whitespace, then lookahead for ^FD.
        /// Includes timeout protection against ReDoS attacks on large inputs.
        /// </summary>
        private static readonly Regex FieldNumberRegex = new Regex(
            @"\^FN\d+\s*(?=\^FD)", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromSeconds(5)
        );

        /// <summary>
        /// Pre-compiled regex to extract file name from ZPL comment.
        /// Pattern: ^FX followed by FileName: and captures the name until next ^ or newline.
        /// Supports syntax: ^FX FileName: my-label-name
        /// </summary>
        private static readonly Regex FileNameRegex = new Regex(
            @"\^FX\s+FileName:\s*(.+?)(?:\^|\r|\n|$)", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromSeconds(5)
        );

        /// <summary>
        /// Pre-compiled regex to extract FORCED file name from ZPL comment.
        /// Pattern: ^FX followed by !FileName: (with !) and captures the name until next ^ or newline.
        /// The ! prefix indicates this name should override even the -n parameter.
        /// Supports syntax: ^FX !FileName: my-forced-label-name
        /// </summary>
        private static readonly Regex ForcedFileNameRegex = new Regex(
            @"\^FX\s+!FileName:\s*(.+?)(?:\^|\r|\n|$)", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromSeconds(5)
        );

        /// <summary>
        /// Reads the file and returns its content.
        /// </summary>
        /// <param name="filePath">Path to the input file.</param>
        /// <returns>Content of the file.</returns>
        public static string ReadFile(string filePath) {
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Splits the content into a list of ZPL labels based on the delimiters "^XA" and "^XZ".
        /// If there is any content before the first "^XA", this is considered as graphic elements
        /// and will be prepended to each label.
        /// </summary>
        /// <param name="content">Content of the file.</param>
        /// <returns>List of ZPL labels.</returns>
        public static List<string> SplitLabels(string content) {
            // Extracts all content before the first "^XA" as graphic elements
            string graphicElements = string.Empty;
            int firstXAPos = content.IndexOf("^XA", StringComparison.OrdinalIgnoreCase);
            if (firstXAPos > 0) {
                graphicElements = content.Substring(0, firstXAPos).Trim();
                content = content.Substring(firstXAPos);
            }

            var labels = new List<string>();
            var tokens = content.Split(new string[] { "^XA" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens) {
                int posEnd = token.IndexOf("^XZ", StringComparison.OrdinalIgnoreCase);
                if (posEnd > -1) {
                    // Rebuilds the label with the initial delimiter that was removed by Split
                    string label = "^XA" + token.Substring(0, posEnd + 3);
                    // If there are graphic elements, add them at the beginning of the label
                    if (!string.IsNullOrEmpty(graphicElements)) {
                        label = graphicElements + Environment.NewLine + label;
                    }
                    labels.Add(label);
                }
            }
            return labels;
        }

        /// <summary>
        /// Preprocesses ZPL content to handle unsupported or problematic commands.
        /// Currently handles:
        /// - ^FN (Field Number): Removes ^FN tags when followed by ^FD, as BinaryKits.Zpl.Viewer
        ///   doesn't fully support field templates. The ^FD content is preserved for direct rendering.
        /// </summary>
        /// <param name="content">Raw ZPL content.</param>
        /// <returns>Preprocessed ZPL content ready for rendering.</returns>
        /// <example>
        /// Input:  ^FO90,12^A0N,20,20^FN6^FDHello World^FS
        /// Output: ^FO90,12^A0N,20,20^FDHello World^FS
        /// </example>
        public static string PreprocessZpl(string content) {
            if (string.IsNullOrWhiteSpace(content)) {
                return content ?? string.Empty;
            }

            // Remove ^FN<number> when followed by ^FD
            // Uses pre-compiled regex with timeout for safety and performance
            return FieldNumberRegex.Replace(content, string.Empty);
        }

        /// <summary>
        /// Extracts a custom file name from ZPL content using the ^FX FileName: comment syntax.
        /// This allows users to specify the output PDF file name directly in the ZPL code.
        /// </summary>
        /// <param name="content">ZPL content that may contain a FileName comment.</param>
        /// <returns>
        /// The extracted file name (without extension) if found and valid, or null if not found.
        /// Invalid file name characters are replaced with underscores.
        /// </returns>
        /// <example>
        /// Input ZPL:
        ///   ^FX FileName: USPS-Shipping-Label
        ///   ^XA...^XZ
        /// 
        /// Returns: "USPS-Shipping-Label"
        /// </example>
        public static string? ExtractFileName(string content) {
            if (string.IsNullOrWhiteSpace(content)) {
                return null;
            }

            var match = FileNameRegex.Match(content);
            if (match.Success) {
                var fileName = match.Groups[1].Value.Trim();
                
                // Return null if file name is empty after trim
                if (string.IsNullOrWhiteSpace(fileName)) {
                    return null;
                }

                // Sanitize file name by replacing invalid characters with underscores
                foreach (var c in Path.GetInvalidFileNameChars()) {
                    fileName = fileName.Replace(c, '_');
                }

                return fileName;
            }

            return null;
        }

        /// <summary>
        /// Extracts a FORCED file name from ZPL content using the ^FX !FileName: comment syntax.
        /// The ! prefix indicates this name has MAXIMUM priority and overrides even the -n parameter.
        /// Use this when the ZPL file MUST generate a specific output name regardless of user input.
        /// </summary>
        /// <param name="content">ZPL content that may contain a forced FileName comment.</param>
        /// <returns>
        /// The extracted file name (without extension) if found and valid, or null if not found.
        /// Invalid file name characters are replaced with underscores.
        /// </returns>
        /// <example>
        /// Input ZPL:
        ///   ^FX !FileName: MANDATORY-Label-Name
        ///   ^XA...^XZ
        /// 
        /// Returns: "MANDATORY-Label-Name"
        /// This name will be used even if -n parameter is provided.
        /// </example>
        public static string? ExtractForcedFileName(string content) {
            if (string.IsNullOrWhiteSpace(content)) {
                return null;
            }

            var match = ForcedFileNameRegex.Match(content);
            if (match.Success) {
                var fileName = match.Groups[1].Value.Trim();
                
                // Return null if file name is empty after trim
                if (string.IsNullOrWhiteSpace(fileName)) {
                    return null;
                }

                // Sanitize file name by replacing invalid characters with underscores
                foreach (var c in Path.GetInvalidFileNameChars()) {
                    fileName = fileName.Replace(c, '_');
                }

                return fileName;
            }

            return null;
        }
    }
}