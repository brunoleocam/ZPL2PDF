using System;
using System.Collections.Generic;
using System.IO;

namespace ETQ2PDF {
    /// <summary>
    /// Responsável por ler o arquivo de entrada e separar as etiquetas ZPL.
    /// </summary>
    public static class LabelFileReader {
        /// <summary>
        /// Lê o arquivo e retorna o conteúdo.
        /// </summary>
        public static string ReadFile(string filePath) {
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
            }
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Separa o conteúdo em uma lista de etiquetas ZPL baseadas nos delimitadores "^XA" e "^XZ".
        /// </summary>
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