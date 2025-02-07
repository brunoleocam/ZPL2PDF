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
        /// </summary>
        /// <param name="content">Content of the file.</param>
        /// <returns>List of ZPL labels.</returns>
        public static List<string> SplitLabels(string content) {
            var labels = new List<string>();
            var tokens = content.Split(new string[] { "^XA" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens) {
                int posEnd = token.IndexOf("^XZ", StringComparison.OrdinalIgnoreCase);
                if (posEnd > -1) {
                    string label = "^XA" + token.Substring(0, posEnd + 3);
                    labels.Add(label);
                }
            }
            return labels;
        }
    }
}