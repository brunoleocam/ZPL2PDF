using System;
using System.Collections.Generic;
using System.IO;

namespace ZPL2PDF {
    /// <summary>
    /// Responsible for reading the input file and splitting the ZPL labels.
    /// </summary>
    public static class LabelFileReader {
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
    }
}