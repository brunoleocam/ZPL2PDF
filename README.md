# ZPL2PDF - ZPL to PDF Converter

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

A powerful, cross-platform command-line tool that converts ZPL (Zebra Programming Language) files to high-quality PDF documents. Perfect for label printing workflows, automated document generation, and enterprise label management systems.

## 🚀 What's New in v2.0

- **🔄 Daemon Mode**: Automatic folder monitoring and batch conversion
- **🏗️ Clean Architecture**: Completely refactored with SOLID principles
- **🌍 Cross-Platform**: Native support for Windows, Linux, and macOS
- **📐 Smart Dimensions**: Automatic ZPL dimension extraction (`^PW`, `^LL`)
- **⚡ High Performance**: Async processing with retry mechanisms
- **🔧 Enterprise Ready**: PID management, configuration files, and logging

## ✨ Key Features

### 🎯 **Dual Operation Modes**
- **Conversion Mode**: Convert individual files or ZPL strings
- **Daemon Mode**: Monitor folders and auto-convert files as they arrive

### 📐 **Intelligent Dimension Handling**
- Extract dimensions directly from ZPL commands (`^PW`, `^LL`)
- Support for multiple units (mm, cm, inches, points)
- Automatic fallback to sensible defaults
- Priority-based dimension resolution

### 🏗️ **Enterprise Architecture**
- Clean Architecture with separated concerns
- Dependency injection and SOLID principles
- Comprehensive error handling and logging
- Retry mechanisms for file locking scenarios

### 🌍 **Cross-Platform Support**
- Windows (x64, x86)
- Linux (x64, ARM64, ARM)
- macOS (x64, ARM64)
- Self-contained executables

## 📦 Installation

### Windows (Winget)
```bash
winget install ZPL2PDF
```

### Linux (Coming Soon)
```bash
# Ubuntu/Debian
sudo apt install zpl2pdf

# CentOS/RHEL
sudo yum install zpl2pdf
```

### Manual Installation
Download the latest release for your platform from the [Releases](https://github.com/brunoleocam/ZPL2PDF/releases) page.

## 🚀 Quick Start

### Basic Conversion
```bash
# Convert a single file
ZPL2PDF.exe -i label.txt -o output_folder -n my_label.pdf

# Convert with custom dimensions
ZPL2PDF.exe -i label.txt -o output_folder -w 10 -h 5 -u cm

# Convert ZPL string directly
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ" -o output_folder
```

### Daemon Mode (Auto-Conversion)
```bash
# Start daemon with default settings
ZPL2PDF.exe start

# Start with custom folder and dimensions
ZPL2PDF.exe start -l "C:\Labels" -w 7.5 -h 15 -u in

# Check daemon status
ZPL2PDF.exe status

# Stop daemon
ZPL2PDF.exe stop
```

## 📖 Usage Guide

### Conversion Mode

Convert individual ZPL files or strings to PDF:

```bash
ZPL2PDF.exe -i <input_file> -o <output_folder> [options]
ZPL2PDF.exe -z <zpl_content> -o <output_folder> [options]
```

**Parameters:**
- `-i <file>`: Input ZPL file (.txt or .prn)
- `-z <content>`: ZPL content as string
- `-o <folder>`: Output folder for PDF
- `-n <name>`: Output PDF filename (optional)
- `-w <width>`: Label width
- `-h <height>`: Label height
- `-u <unit>`: Unit (mm, cm, in)
- `-d <dpi>`: Print density (203, 300, etc.)

### Daemon Mode

Monitor folders and automatically convert files:

```bash
ZPL2PDF.exe start [options]    # Start daemon
ZPL2PDF.exe stop               # Stop daemon
ZPL2PDF.exe status             # Check status
```

**Daemon Options:**
- `-l <folder>`: Folder to monitor (default: Documents/ZPL2PDF Auto Converter)
- `-w <width>`: Fixed width for all conversions
- `-h <height>`: Fixed height for all conversions
- `-u <unit>`: Unit of measurement
- `-d <dpi>`: Print density

## 🏗️ Architecture

ZPL2PDF follows Clean Architecture principles with clear separation of concerns:

```
src/
├── Application/          # Use Cases & Services
│   ├── Services/         # Business logic services
│   └── Interfaces/       # Service contracts
├── Domain/              # Business entities & rules
│   ├── ValueObjects/    # Immutable data objects
│   └── Services/        # Domain interfaces
├── Infrastructure/      # External concerns
│   ├── FileSystem/      # File operations
│   ├── Rendering/       # PDF generation
│   └── Processing/      # Queue management
└── Presentation/        # CLI & user interface
    ├── Program.cs       # Entry point
    └── Handlers/        # Mode handlers
```

## 🔧 Configuration

### Configuration File (`zpl2pdf.json`)
```json
{
  "defaultWatchFolder": "C:\\Users\\user\\Documents\\ZPL2PDF Auto Converter",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3
}
```

### Environment Variables
- `ZPL2PDF_LANGUAGE`: Set application language
- `ZPL2PDF_LOG_LEVEL`: Set logging level
- `ZPL2PDF_CONFIG_PATH`: Custom config file path

## 📐 ZPL Support

### Supported Commands
- `^XA` / `^XZ`: Label start/end
- `^PW<width>`: Print width in points
- `^LL<length>`: Label length in points
- All standard ZPL text, graphics, and barcode commands

### Dimension Extraction
The tool automatically extracts dimensions from ZPL commands:
- `^PW<width>` → Label width
- `^LL<length>` → Label height
- Converts points to millimeters: `mm = (points / 203) * 25.4`

### Priority Logic
1. **ZPL Commands**: Extract from `^PW` and `^LL`
2. **Explicit Parameters**: Use `-w` and `-h` values
3. **Default Values**: Fallback to 100mm × 150mm

## 🐳 Docker Support

### Run with Docker
```bash
# Build image
docker build -t zpl2pdf .

# Run daemon mode
docker run -d -v /path/to/labels:/app/watch zpl2pdf start

# Run conversion
docker run -v /path/to/input:/app/input -v /path/to/output:/app/output zpl2pdf -i /app/input/label.txt -o /app/output
```

### Docker Compose
```yaml
version: '3.8'
services:
  zpl2pdf:
    build: .
    volumes:
      - ./labels:/app/watch
      - ./output:/app/output
    command: start -l /app/watch -o /app/output
```

## 🧪 Testing

### Run Tests
```bash
# Unit tests
dotnet test tests/ZPL2PDF.Unit/

# Integration tests
dotnet test tests/ZPL2PDF.Integration/

# All tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Coverage
- **Unit Tests**: 90%+ coverage target
- **Integration Tests**: End-to-end workflows
- **Cross-Platform**: Windows, Linux, macOS

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup
```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test
```

### Pull Request Process
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📊 Performance

### Benchmarks
- **Single Label**: ~50ms conversion time
- **Batch Processing**: 100+ labels/minute
- **Memory Usage**: <50MB typical
- **File Size**: ~100KB per label PDF

### Optimization Features
- Async processing with configurable concurrency
- Retry mechanisms for locked files
- Memory-efficient image processing
- Optimized PDF generation

## 🐛 Troubleshooting

### Common Issues

**File Locked Error**
```
Error: File in use, waiting: label.txt
```
- **Solution**: The file is being written to. Wait for the process to complete.

**Invalid ZPL Content**
```
Error: No ZPL labels found in file
```
- **Solution**: Ensure the file contains valid ZPL commands (`^XA...^XZ`).

**Permission Denied**
```
Error: Access to the path is denied
```
- **Solution**: Run with appropriate permissions or check folder access.

### Debug Mode
```bash
# Enable verbose logging
ZPL2PDF.exe -i label.txt -o output --log-level Debug
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl) - ZPL parsing and rendering
- [PdfSharpCore](https://github.com/empira/PdfSharpCore) - PDF generation
- [SkiaSharp](https://github.com/mono/SkiaSharp) - Cross-platform graphics

## 📞 Support

- **Documentation**: [Wiki](https://github.com/brunoleocam/ZPL2PDF/wiki)
- **Issues**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- **Discussions**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)

---

**Made with ❤️ for the ZPL community**
