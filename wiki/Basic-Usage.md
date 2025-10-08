# 🚀 Basic Usage

Learn the fundamentals of using ZPL2PDF for converting ZPL files to PDF.

## 🎯 Two Operation Modes

ZPL2PDF operates in two distinct modes:

### 📄 Conversion Mode
Convert individual ZPL files or strings to PDF documents.

### 🔄 Daemon Mode  
Monitor a folder and automatically convert ZPL files as they appear.

---

## 📄 Conversion Mode

### Basic File Conversion
```bash
# Convert a single file
ZPL2PDF.exe -i label.txt -o output_folder -n my_label.pdf
```

### String Conversion
```bash
# Convert ZPL string directly
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ" -o output_folder -n hello.pdf
```

### With Custom Dimensions
```bash
# Specify dimensions manually
ZPL2PDF.exe -i label.txt -o output_folder -w 7.5 -h 15 -u in -d 203
```

### Parameters Explained

| Parameter | Description | Example |
|-----------|-------------|---------|
| `-i, --input` | Input ZPL file (.txt or .prn) | `-i label.txt` |
| `-z, --zpl` | ZPL content as string | `-z "^XA...^XZ"` |
| `-o, --output` | Output folder for PDF | `-o ./output` |
| `-n, --name` | Output PDF filename | `-n my_label.pdf` |
| `-w, --width` | Label width | `-w 7.5` |
| `-h, --height` | Label height | `-h 15` |
| `-u, --unit` | Unit (mm, cm, in) | `-u in` |
| `-d, --dpi` | Print density (203, 300) | `-d 203` |

---

## 🔄 Daemon Mode

### Start Daemon
```bash
# Start with default folder
ZPL2PDF.exe start

# Start with custom folder and dimensions
ZPL2PDF.exe start -l "C:\WatchFolder" -w 7.5 -h 15 -u in
```

### Check Status
```bash
ZPL2PDF.exe status
```

### Stop Daemon
```bash
ZPL2PDF.exe stop
```

### Daemon Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| `-l, --listen` | Folder to monitor | `-l "C:\WatchFolder"` |
| `-w, --width` | Fixed width for all files | `-w 7.5` |
| `-h, --height` | Fixed height for all files | `-h 15` |
| `-u, --unit` | Unit of measurement | `-u in` |
| `-d, --dpi` | Print density | `-d 203` |

---

## 🌍 Multi-Language Support

ZPL2PDF supports 8 languages:

### Set Language Temporarily
```bash
# Use Spanish for this command
ZPL2PDF.exe --language es-ES status

# Use Portuguese
ZPL2PDF.exe --language pt-BR --help
```

### Set Language Permanently
```bash
# Set default language
ZPL2PDF.exe --language-definitive es-ES

# Reset to system default
ZPL2PDF.exe --language-standard

# Check current language
ZPL2PDF.exe --show-language
```

### Supported Languages
- 🇺🇸 English (en-US)
- 🇧🇷 Portuguese (pt-BR)  
- 🇪🇸 Spanish (es-ES)
- 🇫🇷 French (fr-FR)
- 🇩🇪 German (de-DE)
- 🇮🇹 Italian (it-IT)
- 🇯🇵 Japanese (ja-JP)
- 🇨🇳 Chinese (zh-CN)

---

## 📊 Dimension Handling

### Automatic Extraction
ZPL2PDF automatically extracts dimensions from ZPL commands:
- `^PW<width>` - Label width in points
- `^LL<length>` - Label length in points

### Manual Specification
When ZPL doesn't contain dimensions:
```bash
# Use default dimensions (60mm x 120mm)
ZPL2PDF.exe -i label.txt -o output

# Specify custom dimensions
ZPL2PDF.exe -i label.txt -o output -w 100 -h 200 -u mm
```

### Unit Conversion
- **Points to mm**: `mm = (points / 203) * 25.4`
- **DPI**: Fixed at 203 DPI (Zebra standard)

---

## 💡 Examples

### Example 1: Simple Label
```bash
ZPL2PDF.exe -i simple_label.txt -o ./output -n simple.pdf
```

### Example 2: Custom Dimensions
```bash
ZPL2PDF.exe -i label.txt -o ./output -w 10 -h 5 -u cm -d 300
```

### Example 3: Daemon with Fixed Dimensions
```bash
ZPL2PDF.exe start -l "./watch_folder" -w 7.5 -h 15 -u in
```

### Example 4: Multi-Language
```bash
ZPL2PDF.exe --language pt-BR -i label.txt -o ./output
```

---

## 🔗 Next Steps

- [[Conversion Mode]] - Detailed conversion guide
- [[Daemon Mode]] - Advanced daemon configuration  
- [[Configuration]] - Global settings
- [[Multi-Language Setup]] - Language configuration
- [[Troubleshooting]] - Common issues and solutions
