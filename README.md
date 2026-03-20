# ZPL2PDF - ZPL to PDF Converter

[![Version](https://img.shields.io/badge/version-3.1.0-blue.svg)](https://github.com/brunoleocam/ZPL2PDF/releases)
![GitHub all releases](https://img.shields.io/github/downloads/brunoleocam/ZPL2PDF/total)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)](https://github.com/brunoleocam/ZPL2PDF)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Docker Hub](https://img.shields.io/badge/docker-Alpine%20470MB-blue.svg)](https://hub.docker.com/r/brunoleocam/zpl2pdf)
[![WinGet Package](https://img.shields.io/badge/winget-brunoleocam.ZPL2PDF-blue)](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)

**[English](#)** | **[Português-BR](docs/i18n/README.pt-BR.md)** | **[Español](docs/i18n/README.es-ES.md)** | **[Français](docs/i18n/README.fr-FR.md)** | **[Deutsch](docs/i18n/README.de-DE.md)** | **[Italiano](docs/i18n/README.it-IT.md)** | **[日本語](docs/i18n/README.ja-JP.md)** | **[中文](docs/i18n/README.zh-CN.md)**

A powerful, cross-platform command-line tool that converts ZPL (Zebra Programming Language) files to high-quality PDF documents. Perfect for label printing workflows, automated document generation, and enterprise label management systems.

![ZPL2PDF Demo](docs/Image/example_converted.png)

---

## 🔜 **CLI & packaging (unreleased / main branch)**

These items are documented in [CHANGELOG.md](CHANGELOG.md) under **Unreleased**:

- **`--stdout`**: write the PDF to standard output (binary). `-o` is not required; no status text is written to stdout.
- **Default output name (`-n`)**: if omitted, uses the input file base name (e.g. `label.txt` → `label.pdf`); with `-z`, a timestamped `ZPL2PDF_*.pdf` name is used.
- **Dimensions**: `-u` is required whenever `-w` / `-h` are set, including for `mm`.
- **Linux package scripts**: `scripts/build-deb.sh` and `scripts/build-rpm.sh` read the version from `ZPL2PDF.csproj`.
- **Rendering stack**: PDFsharp 6.x and updated BinaryKits packages; Aztec `^B0` is normalized for offline rendering (`^BO`).

---

## 🚀 **What's New in v3.1.0**

### 🐛 Bug Fixes
- **Fixed Issue #45**: Duplicate or blank labels when `^XA` appears inside `~DGR:` base64 payload — `^XA` is now treated as label start only at line start or after `^XZ`.

### ✨ New Features
- **Issue #48 – TCP Server**: Virtual Zebra printer mode is now implemented. Use `ZPL2PDF server start --port 9101 -o output/`, `server stop`, and `server status`.
- **REST API (PR #47)**: Run `ZPL2PDF --api --host localhost --port 5000` for `POST /api/convert` (ZPL to PDF or PNG) and `GET /api/health`.

---

## 🚀 **What's New in v3.1.0**

### 🐛 Bug Fixes
- **Fixed Issue #39**: Sequential graphic processing for multiple graphics with same name
  - ZPL files with multiple `~DGR` graphics now process correctly
  - Each label uses the correct graphic based on sequential state
  - `^IDR` cleanup commands no longer generate blank pages
  - Resolves issue where all labels were identical in Shopee shipping label files

### 🔧 Improvements
- Added input validation in public methods
- Improved exception handling
- Performance optimizations with compiled regex
- Code cleanup and removal of unused methods

---

## 🚀 **What's New in v3.1.0**

### 🎉 Major New Features
- 🎨 **Labelary API Integration** - High-fidelity ZPL rendering with vector PDF output
- 🖨️ **TCP Server Mode** - Virtual Zebra printer on TCP port (default: 9101)
- 🔤 **Custom Fonts** - Load TrueType/OpenType fonts with `--fonts-dir` and `--font`
- 📁 **Extended File Support** - Added `.zpl` and `.imp` file extensions
- 📝 **Custom Naming** - Set output filename via `^FX FileName:` in ZPL

### 🔧 Rendering Options
```bash
--renderer offline    # BinaryKits (default, works offline)
--renderer labelary   # Labelary API (high-fidelity, requires internet)
--renderer auto       # Try Labelary, fallback to BinaryKits
```

### 🖨️ TCP Server (Virtual Printer)
```bash
ZPL2PDF server start --port 9101 -o output/
ZPL2PDF server status
ZPL2PDF server stop
```

### v2.x Features (Still Available)
- 🌍 **Multi-language Support** - 8 languages (EN, PT, ES, FR, DE, IT, JA, ZH)
- 🔄 **Daemon Mode** - Automatic folder monitoring and batch conversion
- 🏗️ **Clean Architecture** - Completely refactored with SOLID principles
- 🌍 **Cross-Platform** - Native support for Windows, Linux, and macOS
- 📐 **Smart Dimensions** - Automatic ZPL dimension extraction (`^PW`, `^LL`)
- ⚡ **High Performance** - Async processing with retry mechanisms
- 🐳 **Docker Support** - Alpine Linux optimized (470MB)
- 📦 **Professional Installer** - Windows installer with multi-language setup

---

## ✨ **Key Features**

### 🎯 **Three Operation Modes**

#### **Conversion Mode** - Convert individual files
```bash
ZPL2PDF -i label.txt -o output/ -n mylabel.pdf
```

#### **Daemon Mode** - Auto-monitor folders
```bash
ZPL2PDF start -l "C:\Labels"
```

#### **TCP Server Mode** - Virtual printer
```bash
ZPL2PDF server start --port 9101 -o output/
```

### 📐 **Intelligent Dimension Handling**

- ✅ Extract dimensions from ZPL commands (`^PW`, `^LL`)
- ✅ Support for multiple units (mm, cm, inches, points)
- ✅ Automatic fallback to sensible defaults
- ✅ Priority-based dimension resolution

### 🌍 **Multi-Language Interface**

Set your preferred language:
```bash
# Temporary (current session)
ZPL2PDF --language pt-BR status

# Permanent (all sessions)
ZPL2PDF --set-language pt-BR

# Check configuration
ZPL2PDF --show-language
```

**Supported Languages:**
- 🇺🇸 English (en-US)
- 🇧🇷 Português (pt-BR)
- 🇪🇸 Español (es-ES)
- 🇫🇷 Français (fr-FR)
- 🇩🇪 Deutsch (de-DE)
- 🇮🇹 Italiano (it-IT)
- 🇯🇵 日本語 (ja-JP)
- 🇨🇳 中文 (zh-CN)

---

## 📦 **Installation**

### **Windows**

#### Option 1: WinGet (Recommended)
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Option 2: Installer
1. Download [ZPL2PDF-Setup.exe](https://github.com/brunoleocam/ZPL2PDF/releases/latest)
2. Run installer
3. Choose your language during installation
4. Done! ✅

### **Linux**

#### Ubuntu/Debian (.deb package)
```bash
# Download .deb package from releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-linux-amd64.deb

# Install package
sudo dpkg -i ZPL2PDF-v3.1.0-linux-amd64.deb

# Fix dependencies if needed
sudo apt-get install -f

# Verify installation
zpl2pdf --help
```

#### Fedora/CentOS/RHEL (.tar.gz)
```bash
# Download tarball from releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-linux-x64-rpm.tar.gz

# Extract to system
sudo tar -xzf ZPL2PDF-v3.1.0-linux-x64-rpm.tar.gz -C /

# Make executable
sudo chmod +x /usr/bin/ZPL2PDF

# Create symbolic link
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# Verify installation
zpl2pdf --help
```

#### Docker (All Linux distributions)
```bash
docker pull brunoleocam/zpl2pdf:latest
docker run -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

### **macOS**

#### Intel Macs
```bash
# Download
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-osx-x64.tar.gz -o zpl2pdf.tar.gz

# Extract and run
tar -xzf zpl2pdf.tar.gz
./ZPL2PDF -help
```

#### Apple Silicon (M1/M2/M3)
```bash
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-v3.1.0-osx-arm64.tar.gz -o zpl2pdf.tar.gz
tar -xzf zpl2pdf.tar.gz
./ZPL2PDF -help
```

---

## 🚀 **Quick Start**

### **Convert a Single File**
```bash
ZPL2PDF -i label.txt -o output_folder -n my_label.pdf
```

### **Convert with Custom Dimensions**
```bash
ZPL2PDF -i label.txt -o output_folder -w 10 -h 5 -u cm
```

### **Convert ZPL String Directly**
```bash
ZPL2PDF -z "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ" -o output_folder
```

### **Start Daemon Mode (Auto-Conversion)**
```bash
# Start with default settings
ZPL2PDF start

# Start with custom folder
ZPL2PDF start -l "C:\Labels" -w 7.5 -h 15 -u in

# Check status
ZPL2PDF status

# Stop daemon
ZPL2PDF stop
```

---

## 📖 **Usage Guide**

### **Conversion Mode Parameters**

```bash
ZPL2PDF -i <input_file> (-o <output_folder> | --stdout) [options]
ZPL2PDF -z <zpl_content> (-o <output_folder> | --stdout) [options]
```

| Parameter | Description | Example |
|-----------|-------------|---------|
| `-i <file>` | Input ZPL file (.txt, .prn, .zpl, .imp) | `-i label.zpl` |
| `-z <content>` | ZPL content as string | `-z "^XA...^XZ"` |
| `-o <folder>` | Output folder for PDF (required unless `--stdout`) | `-o C:\Output` |
| `--stdout` | Write PDF bytes to stdout only (no file) | `--stdout` |
| `-n <name>` | Output PDF filename (optional; default: input basename or timestamp for `-z`) | `-n result.pdf` |
| `-w <width>` | Label width | `-w 10` |
| `-h <height>` | Label height | `-h 5` |
| `-u <unit>` | Unit (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Print density (default: 203) | `-d 300` |
| `--renderer` | Rendering engine (offline/labelary/auto) | `--renderer labelary` |
| `--fonts-dir` | Custom fonts directory | `--fonts-dir C:\Fonts` |
| `--font` | Map specific font | `--font "A=arial.ttf"` |

**Custom fonts (offline renderer):** ZPL font IDs (`^A0N`, `^AAN`, `^ABN`, …) are mapped to TTF/OTF files. Use `--font "A=arial.ttf"` to map font A; use multiple `--font` for B, 0, etc. If you set `--fonts-dir ./fonts`, relative paths in `--font` are resolved under that directory (e.g. `--font "A=arial.ttf"` → `./fonts/arial.ttf`). Example:

```bash
ZPL2PDF -i zpl_teste.txt -o output -n label1.pdf -d 600 -w 10.0 -h 8.0 -u cm --fonts-dir ./fonts --font "A=arial.ttf" --font "B=another.ttf"
```

### **Daemon Mode Commands**

```bash
ZPL2PDF start [options]    # Start daemon in background
ZPL2PDF stop               # Stop daemon
ZPL2PDF status             # Check daemon status
ZPL2PDF run [options]      # Run daemon in foreground (testing)
```

| Option | Description | Default |
|--------|-------------|---------|
| `-l <folder>` | Folder to monitor | `Documents/ZPL2PDF Auto Converter` |
| `-w <width>` | Fixed width for all conversions | Extract from ZPL |
| `-h <height>` | Fixed height for all conversions | Extract from ZPL |
| `-u <unit>` | Unit of measurement | `mm` |
| `-d <dpi>` | Print density | `203` |

### **TCP Server Commands**

```bash
ZPL2PDF server start [options]    # Start TCP server (virtual printer)
ZPL2PDF server stop               # Stop TCP server
ZPL2PDF server status             # Check TCP server status
```

| Option | Description | Default |
|--------|-------------|---------|
| `--port <port>` | TCP port to listen on | `9101` |
| `-o <folder>` | Output folder for PDFs | `Documents/ZPL2PDF TCP Output` |
| `--foreground` | Run in foreground (not background) | Background |
| `--renderer` | Rendering engine | `offline` |

### **Language Commands**

```bash
--language <code>           # Temporary language override
--set-language <code>       # Set language permanently
--reset-language            # Reset to system default
--show-language             # Show current configuration
```

---

## 🎨 **Rendering Engines**

### **Offline (BinaryKits)** - Default
```bash
ZPL2PDF -i label.txt -o output/ --renderer offline
```
- ✅ Works without internet
- ✅ Fast processing
- ⚠️ Some ZPL commands may render differently

### **Labelary (API)** - High Fidelity
```bash
ZPL2PDF -i label.txt -o output/ --renderer labelary
```
- ✅ Exact Zebra printer emulation
- ✅ Vector PDF output (smaller files)
- ✅ Automatic batching for 50+ labels
- ⚠️ Requires internet connection

### **Auto (Fallback)**
```bash
ZPL2PDF -i label.txt -o output/ --renderer auto
```
- ✅ Tries Labelary first
- ✅ Falls back to BinaryKits if offline

---

## 🐳 **Docker Usage**

### **Quick Start with Docker**

```bash
# Pull image
docker pull brunoleocam/zpl2pdf:latest

# Run daemon mode
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  brunoleocam/zpl2pdf:latest
```

### **Docker Compose**

Create `docker-compose.yml`:

```yaml
version: '3.8'

services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    container_name: zpl2pdf-daemon
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR
    restart: unless-stopped
```

Run:
```bash
docker-compose up -d
```

📘 **Full Docker Guide:** [docs/DOCKER_GUIDE.md](docs/DOCKER_GUIDE.md)

---

## 🔧 **Configuration**

### **Configuration File (`zpl2pdf.json`)**

Create a `zpl2pdf.json` file in the application directory:

```json
{
  "language": "en-US",
  "defaultWatchFolder": "C:\\Users\\user\\Documents\\ZPL2PDF Auto Converter",
  "labelWidth": 10,
  "labelHeight": 5,
  "unit": "cm",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3
}
```

See [zpl2pdf.json.example](zpl2pdf.json.example) for full configuration options.

### **Environment Variables**

| Variable | Description | Example |
|----------|-------------|---------|
| `ZPL2PDF_LANGUAGE` | Application language | `pt-BR` |
| `ZPL2PDF_LOG_LEVEL` | Logging level | `Debug` |

📘 **Language Configuration Guide:** [docs/LANGUAGE_CONFIGURATION.md](docs/LANGUAGE_CONFIGURATION.md)

---

## 📐 **ZPL Support**

### **Supported ZPL Commands**

- ✅ `^XA` / `^XZ` - Label start/end
- ✅ `^PW<width>` - Print width in points
- ✅ `^LL<length>` - Label length in points
- ✅ All standard ZPL text, graphics, and barcode commands

### **Dimension Extraction**

ZPL2PDF automatically extracts dimensions:

```zpl
^XA
^PW800        ← Width: 800 points
^LL1200       ← Height: 1200 points
^FO50,50^A0N,50,50^FDHello^FS
^XZ
```

**Conversion:** `mm = (points / 203) * 25.4`

### **Priority Logic**

1. ⭐ **Explicit Parameters** (`-w`, `-h`) - Highest priority
2. ⭐⭐ **ZPL Commands** (`^PW`, `^LL`) - If no parameters
3. ⭐⭐⭐ **Default Values** (100mm × 150mm) - Fallback

---

## 🏗️ **Architecture**

ZPL2PDF follows **Clean Architecture** principles:

```
src/
├── Application/          # Use Cases & Services
│   ├── Services/         # Business logic
│   └── Interfaces/       # Service contracts
├── Domain/              # Business entities & rules
│   ├── ValueObjects/    # Immutable data objects
│   └── Services/        # Domain interfaces
├── Infrastructure/      # External concerns
│   ├── FileSystem/      # File operations
│   ├── Rendering/       # PDF generation
│   └── Processing/      # Queue management
├── Presentation/        # CLI & user interface
│   └── Handlers/        # Mode handlers
└── Shared/             # Common utilities
    ├── Localization/   # Multi-language
    └── Constants/      # Configuration
```

---

## 🧪 **Testing**

### **Run Tests**

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/ZPL2PDF.Unit/

# Integration tests
dotnet test tests/ZPL2PDF.Integration/

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### **Test Coverage**
- ✅ Unit Tests: 90%+ coverage
- ✅ Integration Tests: End-to-end workflows
- ✅ Cross-Platform: Windows, Linux, macOS

---

## 📚 **Documentation**

### **User Guides**
- 📖 [Complete Documentation](docs/README.md) - Full user manual
- 🌍 [Multi-language Configuration](docs/LANGUAGE_CONFIGURATION.md)
- 🐳 [Docker Usage Guide](docs/DOCKER_GUIDE.md)
- 📦 [Inno Setup Guide](docs/INNO_SETUP_GUIDE.md)

### **Developer Guides**
- 🛠️ [Contributing Guide](CONTRIBUTING.md)
- 📋 [Changelog](CHANGELOG.md)
- 🏗️ [Architecture Overview](docs/ARCHITECTURE.md)
- 🔄 [CI/CD Workflow](docs/CI_CD_WORKFLOW.md)

### **Build & Deployment**
- 🔨 [Build Scripts](scripts/README.md)
- 🐳 [Docker Publishing](docs/DOCKER_PUBLISH_GUIDE.md)
- 📦 [Windows Installer](installer/README.md)

---

## 💡 **Use Cases**

### **1. ERP Integration**
```csharp
// C# example
Process.Start("ZPL2PDF.exe", "-i label.txt -o output/ -w 10 -h 5 -u cm");
```

### **2. Batch Processing**
```bash
# Process all ZPL files in a folder
for file in *.txt; do
    ZPL2PDF -i "$file" -o output/
done
```

### **3. Automated Workflow**
```bash
# Start daemon on system startup
ZPL2PDF start -l "C:\Labels\Incoming"
```

### **4. Docker Deployment**
```bash
# Deploy to server
docker run -d \
  -v /srv/labels:/app/watch \
  -v /srv/pdfs:/app/output \
  --restart always \
  brunoleocam/zpl2pdf:latest
```

---

## 📊 **Performance**

### **Benchmarks**

| Metric | Value |
|--------|-------|
| **Single Label** | ~50ms |
| **Batch Processing** | 100+ labels/minute |
| **Memory Usage** | <50MB typical |
| **PDF File Size** | ~100KB per label |
| **Startup Time** | <1 second |

### **Optimization Features**
- ✅ Async processing with configurable concurrency
- ✅ Retry mechanisms for locked files
- ✅ Memory-efficient image processing
- ✅ Optimized PDF generation with compression

---

## 🛠️ **Development**

### **Prerequisites**

- .NET 9.0 SDK or later
- Git
- Visual Studio 2022 or VS Code
- Docker (for cross-platform testing)

### **Build from Source**

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

# Build for your platform
dotnet publish -c Release -r win-x64 --self-contained true

# Build all platforms
.\scripts\build-all-platforms.ps1  # Windows
./scripts/build-all-platforms.sh   # Linux/macOS
```

### **Project Structure**

```
ZPL2PDF/
├── src/                    # Source code (Clean Architecture)
├── tests/                  # Unit & integration tests
├── docs/                   # Documentation
│   ├── i18n/              # Translated documentation
│   ├── Image/             # Screenshots & icons
│   └── Sample/            # Sample ZPL files
├── installer/              # Windows installer (Inno Setup)
├── scripts/                # Build & release scripts
├── .github/workflows/      # GitHub Actions CI/CD
├── docker-compose.yml      # Docker orchestration
└── Dockerfile              # Docker image definition
```

---

## 🐛 **Troubleshooting**

### **Common Issues**

| Issue | Solution |
|-------|----------|
| **File Locked Error** | Wait for the process writing the file to complete |
| **Invalid ZPL Content** | Ensure file contains valid ZPL commands (`^XA...^XZ`) |
| **Permission Denied** | Run with appropriate permissions or check folder access |
| **Docker: libgdiplus not found** | Use official image: `brunoleocam/zpl2pdf:alpine` |

### **Debug Mode**

```bash
# Enable verbose logging
ZPL2PDF -i label.txt -o output/ --log-level Debug
```

### **Get Help**

- 📖 [Wiki](https://github.com/brunoleocam/ZPL2PDF/wiki)
- 🐛 [Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- 💬 [Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)

---

## 🤝 **Contributing**

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

### **Quick Start**

```bash
# 1. Fork and clone
git clone https://github.com/YOUR_USERNAME/ZPL2PDF.git

# 2. Create feature branch
git checkout -b feature/amazing-feature

# 3. Make changes and test
dotnet test

# 4. Commit and push
git commit -m "feat: add amazing feature"
git push origin feature/amazing-feature

# 5. Create Pull Request
```

---

## 📄 **License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 **Acknowledgments**

Built with amazing open-source libraries:

- [BinaryKits.Zpl](https://github.com/BinaryKits/BinaryKits.Zpl) - ZPL parsing and rendering
- [PdfSharpCore](https://github.com/empira/PdfSharpCore) - PDF generation
- [SkiaSharp](https://github.com/mono/SkiaSharp) - Cross-platform graphics

---

## 📞 **Support**

- 📖 **Documentation**: [Full documentation](docs/)
- 🐛 **Bug Reports**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- 💬 **Questions**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
- 📧 **Email**: [Contact](mailto:brunoleocam@gmail.com)

---

## 💝 **Support the Project**

If ZPL2PDF helps you, consider supporting its development:

- ☕ [Buy Me a Coffee](https://buymeacoffee.com/brunoleocam)
- 🎗️ [Patreon](https://patreon.com/brunoleocam)
- 💖 [GitHub Sponsors](https://github.com/sponsors/brunoleocam)

Your support helps maintain and improve ZPL2PDF for everyone!

---

## 🌟 **Star History**

If ZPL2PDF helps you, please ⭐ star the repository!

---

## 👥 **Contributors**

Thanks to all contributors who have helped make ZPL2PDF better!

<a href="https://github.com/brunoleocam/ZPL2PDF/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=brunoleocam/ZPL2PDF&max=30" alt="Contributors" />
</a>

_Image may be cached; [see full list on GitHub](https://github.com/brunoleocam/ZPL2PDF/graphs/contributors)._

---

**ZPL2PDF** - Convert ZPL labels to PDF easily and efficiently.