# 📄 Conversion Mode

Detailed guide for converting individual ZPL files to PDF documents.

## 🎯 Overview

Conversion Mode is designed for:
- ✅ **Single file processing**
- ✅ **Batch processing** (multiple files)
- ✅ **String-to-PDF** conversion
- ✅ **Custom dimension** specification
- ✅ **Quality control** per file

---

## 📁 File Input Methods

### Method 1: File Input (`-i`)
```bash
# Basic file conversion
ZPL2PDF.exe -i label.txt -o output_folder

# With custom output name
ZPL2PDF.exe -i label.txt -o output_folder -n my_label.pdf

# Multiple files (batch)
ZPL2PDF.exe -i *.txt -o output_folder
```

### Method 2: String Input (`-z`)
```bash
# Direct ZPL string
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDHello^FS^XZ" -o output_folder

# With custom name
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDHello^FS^XZ" -o output_folder -n hello.pdf
```

---

## 📏 Dimension Configuration

### Automatic Dimension Extraction

ZPL2PDF automatically extracts dimensions from ZPL commands:

```zpl
^XA
^PW400    ← Width: 400 points
^LL600    ← Length: 600 points
^FO50,50^A0N,50,50^FDHello^FS
^XZ
```

**Conversion**: `mm = (points / 203) * 25.4`
- Width: `(400 / 203) * 25.4 = 50.0 mm`
- Length: `(600 / 203) * 25.4 = 75.0 mm`

### Manual Dimension Specification

When ZPL doesn't contain dimensions:

```bash
# Default dimensions (60mm x 120mm)
ZPL2PDF.exe -i label.txt -o output

# Custom dimensions in millimeters
ZPL2PDF.exe -i label.txt -o output -w 100 -h 200 -u mm

# Custom dimensions in inches
ZPL2PDF.exe -i label.txt -o output -w 4 -h 6 -u in

# Custom dimensions in centimeters
ZPL2PDF.exe -i label.txt -o output -w 10 -h 15 -u cm
```

### Dimension Priority

1. **Manual parameters** (`-w`, `-h`, `-u`) - Highest priority
2. **ZPL commands** (`^PW`, `^LL`) - Medium priority  
3. **Default values** (60mm x 120mm) - Fallback

---

## 🎨 Quality Settings

### DPI Configuration
```bash
# Standard DPI (203)
ZPL2PDF.exe -i label.txt -o output -d 203

# High resolution (300 DPI)
ZPL2PDF.exe -i label.txt -o output -d 300

# Low resolution (150 DPI)
ZPL2PDF.exe -i label.txt -o output -d 150
```

**Note**: DPI only affects output quality, not actual dimensions.

---

## 📊 Supported File Types

### Input Files
- ✅ **`.txt`** - Text files containing ZPL
- ✅ **`.prn`** - Print files (ZPL format)
- ✅ **String input** - Direct ZPL content

### Output Files
- ✅ **`.pdf`** - PDF documents (always)

---

## 🔧 Advanced Examples

### Example 1: Batch Processing
```bash
# Convert all .txt files in current directory
for file in *.txt; do
    ZPL2PDF.exe -i "$file" -o ./output -n "${file%.txt}.pdf"
done
```

### Example 2: Quality Control
```bash
# High-quality conversion
ZPL2PDF.exe -i label.txt -o output -w 10 -h 15 -u cm -d 300 -n high_quality.pdf

# Standard quality (faster)
ZPL2PDF.exe -i label.txt -o output -w 10 -h 15 -u cm -d 203 -n standard.pdf
```

### Example 3: String Processing
```bash
# Create PDF from ZPL string
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ" \
  -o ./output -w 7.5 -h 15 -u in -n test_label.pdf
```

### Example 4: Multi-Language Processing
```bash
# Process with Portuguese interface
ZPL2PDF.exe --language pt-BR -i label.txt -o output -n label.pdf

# Process with Spanish interface  
ZPL2PDF.exe --language es-ES -i label.txt -o output -n label.pdf
```

---

## ⚡ Performance Tips

### File Size Optimization
- **Small files** (< 1KB): Use `-d 203` for faster processing
- **Large files** (> 10KB): Use `-d 300` for better quality
- **Batch processing**: Process files sequentially to avoid memory issues

### Memory Management
- **Single file**: Process one at a time
- **Large batches**: Use scripts for sequential processing
- **String input**: Limited by available RAM

---

## ❌ Common Issues

### Issue: "File not found"
```bash
# Wrong path
ZPL2PDF.exe -i ./label.txt -o output  # File doesn't exist

# Solution: Check file path
ls -la ./label.txt  # Linux/macOS
dir .\label.txt     # Windows
```

### Issue: "Invalid dimensions"
```bash
# Zero or negative dimensions
ZPL2PDF.exe -i label.txt -o output -w 0 -h 15

# Solution: Use positive values
ZPL2PDF.exe -i label.txt -o output -w 10 -h 15
```

### Issue: "Output folder not writable"
```bash
# Permission denied
ZPL2PDF.exe -i label.txt -o /root/output

# Solution: Use accessible folder
ZPL2PDF.exe -i label.txt -o ./output
```

---

## 🔗 Related Topics

- [[Daemon Mode]] - Automatic folder monitoring
- [[Configuration]] - Global settings
- [[Troubleshooting]] - Advanced problem solving
- [[Performance Optimization]] - Speed improvements
