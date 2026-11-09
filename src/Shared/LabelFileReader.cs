using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// Pre-compiled regex for detecting cleanup commands (^IDR:filename^FS).
        /// Includes timeout protection against ReDoS attacks on large inputs.
        /// </summary>
        private static readonly Regex CleanupCommandRegex = new Regex(
            @"^\^IDR:[^\^]+\^FS\s*$",
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
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// Splits the content into a list of ZPL labels based on the delimiters "^XA" and "^XZ".
        /// Processes the file sequentially, maintaining graphic state in memory (like a printer would).
        /// Handles ~DGR (define graphic), ^IDR (delete graphic), and ^XA...^XZ (labels) commands.
        /// </summary>
        /// <param name="content">Content of the file.</param>
        /// <returns>List of ZPL labels with appropriate graphics prepended.</returns>
        /// <exception cref="ArgumentNullException">Thrown when content is null.</exception>
        public static List<string> SplitLabels(string content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }

            var labels = new List<string>();
            var graphicsInMemory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            // Process content sequentially to maintain graphic state
            int currentPos = 0;
            int maxIterations = content.Length; // Protection against infinite loops
            int iterationCount = 0;
            
            while (currentPos < content.Length && iterationCount < maxIterations) {
                iterationCount++;
                
                // Look for ~DGR (define graphic) commands
                int dgrPos = content.IndexOf("~DGR:", currentPos, StringComparison.OrdinalIgnoreCase);
                // Look for ^XA (start label) commands
                int xaPos = content.IndexOf("^XA", currentPos, StringComparison.OrdinalIgnoreCase);
                
                // Determine which comes first
                int nextPos = -1;
                bool isDgr = false;
                
                if (dgrPos >= 0 && xaPos >= 0) {
                    if (dgrPos < xaPos) {
                        nextPos = dgrPos;
                        isDgr = true;
                    } else {
                        nextPos = xaPos;
                        isDgr = false;
                    }
                } else if (dgrPos >= 0) {
                    nextPos = dgrPos;
                    isDgr = true;
                } else if (xaPos >= 0) {
                    nextPos = xaPos;
                    isDgr = false;
                } else {
                    // No more commands found
                    break;
                }

                // ^XA inside ~DGR payload (e.g. base64) must not start a label; only ^XA at line start or content start is valid
                if (!isDgr && !IsValidLabelStart(content, nextPos)) {
                    currentPos = nextPos + 3;
                    continue;
                }
                
                // Protection: Ensure we always advance
                if (nextPos < currentPos) {
                    break; // Safety check to prevent infinite loop
                }
                
                if (isDgr) {
                    // Process ~DGR command: ~DGR:filename,size,bytes,:Z64:data:checksum
                    // Prefer end after checksum so that ^XA on the same line (after ~DGR) is not swallowed
                    int endByChecksum = -1;
                    int z64Idx = content.IndexOf(":Z64:", nextPos, StringComparison.OrdinalIgnoreCase);
                    if (z64Idx >= 0) {
                        int colonAfterBase64 = content.IndexOf(':', z64Idx + 5);
                        if (colonAfterBase64 >= 0) {
                            int checksumEnd = colonAfterBase64 + 1;
                            while (checksumEnd < content.Length && IsHexDigit(content[checksumEnd])) checksumEnd++;
                            endByChecksum = checksumEnd;
                        }
                    }
                    int endByNewline = content.IndexOf('\n', nextPos);
                    if (endByNewline < 0) endByNewline = content.IndexOf('\r', nextPos);

                    int endOfLine;
                    bool usedChecksumEnd = false;
                    if (endByChecksum >= 0) {
                        endOfLine = endByChecksum;
                        usedChecksumEnd = true;
                    } else if (endByNewline >= 0) {
                        endOfLine = endByNewline;
                    } else {
                        endOfLine = content.Length;
                    }

                    // Extract the complete ~DGR line (include newline only when we ended at newline)
                    int lineLength = endOfLine - nextPos;
                    if (!usedChecksumEnd && endOfLine < content.Length && (content[endOfLine] == '\n' || content[endOfLine] == '\r')) {
                        if (endOfLine + 1 < content.Length && content[endOfLine] == '\r' && content[endOfLine + 1] == '\n') {
                            lineLength += 2;
                        } else {
                            lineLength += 1;
                        }
                    }
                    
                    string dgrLine = content.Substring(nextPos, lineLength).TrimEnd('\r', '\n');
                    
                    // Extract filename from ~DGR:filename,...
                    int colonPos = dgrLine.IndexOf(':');
                    int commaPos = dgrLine.IndexOf(',', colonPos + 1);
                    if (colonPos >= 0 && commaPos > colonPos) {
                        string filename = dgrLine.Substring(colonPos + 1, commaPos - colonPos - 1);
                        // Store the graphic definition in memory (replaces if already exists)
                        graphicsInMemory[filename] = dgrLine;
                    }
                    currentPos = nextPos + lineLength;
                } else {
                    // Process ^XA...^XZ label
                    int xzPos = content.IndexOf("^XZ", nextPos, StringComparison.OrdinalIgnoreCase);
                    if (xzPos < 0) {
                        // No closing ^XZ found, skip this ^XA
                        currentPos = nextPos + 3;
                        continue;
                    }
                    
                    string label = content.Substring(nextPos, xzPos - nextPos + 3);
                    string labelContent = label.Replace("^XA", "").Replace("^XZ", "").Trim();
                    
                    // Check if this is a cleanup command (^IDR)
                    if (IsCleanupCommandOnly(labelContent)) {
                        // Extract filename from ^IDR:filename^FS
                        var idrMatch = Regex.Match(labelContent, @"\^IDR:([^\^]+)\^FS", RegexOptions.IgnoreCase);
                        if (idrMatch.Success) {
                            string filename = idrMatch.Groups[1].Value;
                            // Remove graphic from memory
                            graphicsInMemory.Remove(filename);
                        }
                        // Skip cleanup commands - they don't generate pages
                        currentPos = xzPos + 3;
                        continue;
                    }
                    
                    // This is a real label - prepend current graphics in memory
                    string labelWithGraphics = BuildLabelWithGraphics(label, graphicsInMemory);
                    labels.Add(labelWithGraphics);
                    
                    currentPos = xzPos + 3;
                }
            }
            
            return labels;
        }
        
        private static bool IsHexDigit(char c) =>
            (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');

        /// <summary>
        /// Returns true if the position is a valid start of a ZPL label (^XA).
        /// We reject ^XA only when it appears inside the base64 segment of a ~DGR line
        /// (between ":Z64:" and the next ":"), so we don't treat "^XA" inside ~DGR base64 as a label start.
        /// ^XA that appears after the ~DGR payload on the same line (e.g. after :checksum) is valid.
        /// </summary>
        private static bool IsValidLabelStart(string content, int xaPosition) {
            if (xaPosition <= 0) return true;
            // Find start of current line (last newline before xaPosition)
            int lineStart = xaPosition - 1;
            while (lineStart >= 0 && content[lineStart] != '\n' && content[lineStart] != '\r') {
                lineStart--;
            }
            lineStart++; // first char of this line
            // Only consider ~DGR lines
            if (lineStart + 5 > content.Length ||
                string.Compare(content, lineStart, "~DGR:", 0, 5, StringComparison.OrdinalIgnoreCase) != 0) {
                return true;
            }
            // Find the base64 segment: ~DGR:name,size,bytes,:Z64:data:checksum — ^XA is false only inside "data"
            int z64Index = content.IndexOf(":Z64:", lineStart, StringComparison.OrdinalIgnoreCase);
            if (z64Index < 0 || z64Index >= xaPosition) {
                // No :Z64: on this line before ^XA, or ^XA is before :Z64: — treat as valid (e.g. malformed or no base64)
                return true;
            }
            int base64Start = z64Index + 5; // after ":Z64:"
            int base64End = content.IndexOf(':', base64Start);
            if (base64End < 0) base64End = content.Length;
            // ^XA is invalid only when inside the base64 segment
            if (xaPosition >= base64Start && xaPosition < base64End) return false;
            return true;
        }

        /// <summary>
        /// Builds a complete label by prepending all graphics currently in memory.
        /// </summary>
        /// <param name="label">The label content (^XA...^XZ).</param>
        /// <param name="graphicsInMemory">Dictionary of graphics currently in memory.</param>
        /// <returns>Complete label with graphics prepended.</returns>
        private static string BuildLabelWithGraphics(string label, Dictionary<string, string> graphicsInMemory) {
            if (graphicsInMemory.Count == 0) {
                return label;
            }
            
            var graphicsText = string.Join(Environment.NewLine, graphicsInMemory.Values);
            return graphicsText + Environment.NewLine + label;
        }

        /// <summary>
        /// Determines if a label content is only a cleanup command (e.g., ^IDR:filename^FS).
        /// These commands should not be rendered as separate pages.
        /// </summary>
        /// <param name="labelContent">Label content without ^XA and ^XZ delimiters.</param>
        /// <returns>True if the label is only a cleanup command, false otherwise.</returns>
        private static bool IsCleanupCommandOnly(string labelContent) {
            if (string.IsNullOrWhiteSpace(labelContent)) {
                return true; // Empty labels are considered cleanup
            }

            // Remove whitespace for comparison
            string trimmed = labelContent.Trim();
            
            // Check if it's only an ^IDR command (delete graphic)
            // Pattern: ^IDR:filename^FS (with optional whitespace and newlines)
            if (CleanupCommandRegex.IsMatch(trimmed)) {
                return true;
            }

            // Check if it contains only minimal commands that don't render content
            // These are typically just ^FS (field separator) or similar
            string withoutFS = trimmed.Replace("^FS", "").Replace("\r", "").Replace("\n", "").Trim();
            if (string.IsNullOrWhiteSpace(withoutFS)) {
                return true;
            }

            // Check if it's a very short label with only cleanup commands
            // Labels with actual content usually have more than just ^IDR or ^FS
            if (trimmed.Length < 20 && (trimmed.Contains("^IDR", StringComparison.OrdinalIgnoreCase) || 
                                         trimmed.Replace("^FS", "").Trim().Length == 0)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Preprocesses ZPL content to handle unsupported or problematic commands.
        /// Currently handles:
        /// - ^FN (Field Number): Removes ^FN tags when followed by ^FD, as BinaryKits.Zpl.Viewer
        ///   doesn't fully support field templates. The ^FD content is preserved for direct rendering.
        /// - ^FH hex in ^FD: Decodes d+XX sequences (UTF-8 bytes, d = optional ^FH indicator, default '_') into Unicode
        ///   so the viewer renders accented characters (ã, ç, ú, etc.) correctly instead of Ã£, Ã§, Ãº.
        /// </summary>
        /// <param name="content">Raw ZPL content.</param>
        /// <returns>Preprocessed ZPL content ready for rendering.</returns>
        public static string PreprocessZpl(string content) {
            if (string.IsNullOrWhiteSpace(content)) {
                return content ?? string.Empty;
            }

            // Remove ^FN<number> when followed by ^FD
            string result = FieldNumberRegex.Replace(content, string.Empty);

            // Work around typo in BinaryKits.Zpl.Viewer: it recognizes ^BO, but not ^B0.
            result = Regex.Replace(result, @"\^B0", "^BO", RegexOptions.IgnoreCase);

            // Decode ^FH hex sequences (d+XX per ZPL) in field data as UTF-8 so ã, ç, ú render correctly
            result = DecodeFhHexInFieldData(result);

            return result;
        }

        /// <summary>
        /// Replaces dXX hex sequences (from ^FH) in ^FD...^FS blocks with the corresponding UTF-8 character,
        /// where d is the hexadecimal indicator from ^FH (default '_' per ZPL).
        /// Prevents BinaryKits from interpreting UTF-8 bytes as Latin-1 (e.g. Ã£ instead of ã).
        /// </summary>
        private static string DecodeFhHexInFieldData(string zpl) {
            if (string.IsNullOrEmpty(zpl)) return zpl ?? string.Empty;

            var sb = new StringBuilder(zpl.Length);
            int i = 0;

            // Only decode dXX when the next field is explicitly marked with ^FH (d = '_' by default or the char after ^FH).
            bool decodeHexForNextField = false;
            char fhHexIndicator = '_';

            // Barcode payload fields should NOT be UTF-8 decoded, otherwise content gets truncated/corrupted.
            // This is set when we encounter a ^B* instruction and consumed when we process the next ^FD...^FS.
            bool barcodeFieldPending = false;

            while (i < zpl.Length) {
                // ^FH / ^FHd: next ^FD may use d + two hex digits per byte (ZPL default d = '_').
                if (i + 3 <= zpl.Length &&
                    zpl[i] == '^' &&
                    (zpl[i + 1] == 'F' || zpl[i + 1] == 'f') &&
                    (zpl[i + 2] == 'H' || zpl[i + 2] == 'h')) {
                    fhHexIndicator = '_';
                    int consumed = 3;
                    if (i + 3 < zpl.Length && zpl[i + 3] != '^') {
                        fhHexIndicator = zpl[i + 3];
                        consumed = 4;
                    }

                    sb.Append(zpl, i, consumed);
                    decodeHexForNextField = true;
                    i += consumed;
                    continue;
                }

                // ^FD...^FS: field data block
                if (i + 3 <= zpl.Length &&
                    zpl[i] == '^' &&
                    (zpl[i + 1] == 'F' || zpl[i + 1] == 'f') &&
                    (zpl[i + 2] == 'D' || zpl[i + 2] == 'd')) {
                    int fieldDataStart = i + 3;
                    int fsIndex = zpl.IndexOf("^FS", fieldDataStart, StringComparison.OrdinalIgnoreCase);
                    if (fsIndex < 0) {
                        // Malformed label; copy remainder as-is.
                        sb.Append(zpl.Substring(i));
                        break;
                    }

                    string fieldData = zpl.Substring(fieldDataStart, fsIndex - fieldDataStart);

                    bool shouldDecode = decodeHexForNextField && !barcodeFieldPending;
                    string decoded = shouldDecode ? DecodeFhHexSequence(fieldData, fhHexIndicator) : fieldData;

                    sb.Append(zpl, i, 3);
                    sb.Append(decoded);
                    sb.Append(zpl, fsIndex, 3);

                    i = fsIndex + 3;
                    decodeHexForNextField = false;
                    fhHexIndicator = '_';
                    barcodeFieldPending = false;
                    continue;
                }

                // ^B*: barcodes. We must avoid UTF-8 decoding the payload that comes in the next ^FD...^FS.
                if (i + 2 < zpl.Length &&
                    zpl[i] == '^' &&
                    (zpl[i + 1] == 'B' || zpl[i + 1] == 'b')) {
                    // Skip ^BY (barcode parameter/mode), but still treat most ^B* payload producers as barcodes.
                    char barcodeType = zpl[i + 2];
                    if (barcodeType != 'Y' && barcodeType != 'y') {
                        barcodeFieldPending = true;
                    }

                    sb.Append(zpl, i, 2); // Copy "^B" verbatim
                    i += 2;
                    continue;
                }

                // Default: copy char
                sb.Append(zpl[i]);
                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Decodes a string containing dXX (hex byte) sequences into UTF-8 text, where d is the ^FH indicator (default '_').
        /// Consecutive dXX are treated as one UTF-8 byte sequence (e.g. _C3_A3 or \C3\A3 with indicator '\' -> ã).
        /// </summary>
        private static string DecodeFhHexSequence(string fieldData, char hexIndicator) {
            var sb = new StringBuilder(fieldData.Length);
            int i = 0;
            while (i < fieldData.Length) {
                if (i + 3 <= fieldData.Length && fieldData[i] == hexIndicator && IsHexDigit(fieldData[i + 1]) && IsHexDigit(fieldData[i + 2])) {
                    var bytes = new List<byte>();
                    while (i + 3 <= fieldData.Length && fieldData[i] == hexIndicator && IsHexDigit(fieldData[i + 1]) && IsHexDigit(fieldData[i + 2])) {
                        bytes.Add((byte)Convert.ToInt32(fieldData.Substring(i + 1, 2), 16));
                        i += 3;
                    }
                    if (bytes.Count > 0) {
                        try {
                            sb.Append(Encoding.UTF8.GetString(bytes.ToArray()));
                        } catch {
                            foreach (byte b in bytes)
                                sb.Append((char)b);
                        }
                    }
                } else {
                    sb.Append(fieldData[i]);
                    i++;
                }
            }
            return sb.ToString();
        }
    }
}