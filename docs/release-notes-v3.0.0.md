# 🎉 ZPL2PDF v3.0.0 - Release Notes

**Release Date:** December 19, 2025  
**Release Type:** Major Release

---

## 📥 Downloads

### Windows
| Platform | File | Size |
|----------|------|------|
| **Installer** | `ZPL2PDF-Setup-3.0.0.exe` | ~35 MB |
| Windows x64 | `ZPL2PDF-v3.0.0-win-x64.zip` | ~49 MB |
| Windows x86 | `ZPL2PDF-v3.0.0-win-x86.zip` | ~44 MB |
| Windows ARM64 | `ZPL2PDF-v3.0.0-win-arm64.zip` | ~46 MB |

### Linux
| Platform | File | Size |
|----------|------|------|
| Linux x64 | `ZPL2PDF-v3.0.0-linux-x64.tar.gz` | ~49 MB |
| Linux x64 (DEB) | `ZPL2PDF-v3.0.0-linux-amd64.deb` | ~34 MB |
| Linux x64 (RPM) | `ZPL2PDF-v3.0.0-linux-x64-rpm.tar.gz` | ~49 MB |
| Linux ARM64 | `ZPL2PDF-v3.0.0-linux-arm64.tar.gz` | ~45 MB |
| Linux ARM | `ZPL2PDF-v3.0.0-linux-arm.tar.gz` | ~48 MB |

### macOS
| Platform | File | Size |
|----------|------|------|
| macOS Intel | `ZPL2PDF-v3.0.0-osx-x64.tar.gz` | ~52 MB |
| macOS Apple Silicon | `ZPL2PDF-v3.0.0-osx-arm64.tar.gz` | ~48 MB |

---

## 🐳 Docker

```bash
# Docker Hub
docker pull brunoleocam/zpl2pdf:3.0.0

# GitHub Container Registry
docker pull ghcr.io/brunoleocam/zpl2pdf:3.0.0

# Latest version
docker pull brunoleocam/zpl2pdf:latest
```

---

## ✨ What's New in v3.0.0

### 🎨 Labelary API Integration - High-Fidelity Rendering

ZPL2PDF now supports **Labelary API** for high-fidelity ZPL rendering with exact Zebra printer emulation!

**Features:**
- ✅ **Vector PDF Output**: Smaller, higher quality PDFs with vector graphics
- ✅ **Exact Emulation**: Perfect rendering matching actual Zebra printers
- ✅ **Direct PDF Generation**: PDFs generated directly from API (no intermediate images)
- ✅ **Batch Processing**: Automatic batching for 50+ labels with PDF merging
- ✅ **Smart Fallback**: Auto mode tries Labelary first, falls back to BinaryKits if offline

**Usage:**
```bash
# Use Labelary API (requires internet)
ZPL2PDF -i label.zpl -o output/ --renderer labelary

# Use offline BinaryKits (default, no internet needed)
ZPL2PDF -i label.zpl -o output/ --renderer offline

# Auto mode: try Labelary, fallback to BinaryKits
ZPL2PDF -i label.zpl -o output/ --renderer auto
```

### 🖨️ TCP Server Mode - Virtual Zebra Printer

ZPL2PDF can now act as a **virtual Zebra printer** on a TCP port, perfect for integration with applications that send ZPL directly to printers!

**Features:**
- ✅ **Virtual Printer**: Acts as TCP printer on configurable port (default: 9101)
- ✅ **Background Operation**: Runs independently in background
- ✅ **Independent from Daemon**: Can run simultaneously with daemon mode
- ✅ **Custom Output**: Configure output directory for generated PDFs
- ✅ **Foreground Mode**: Debug mode for troubleshooting

**Usage:**
```bash
# Start TCP server on default port 9101
ZPL2PDF server start -o output/

# Start on custom port
ZPL2PDF server start --port 9100 -o output/

# Check server status
ZPL2PDF server status

# Stop server
ZPL2PDF server stop

# Run in foreground for debugging
ZPL2PDF server start --foreground -o output/
```

**Integration Example:**
```csharp
// Send ZPL to virtual printer
var client = new TcpClient("localhost", 9101);
var stream = client.GetStream();
stream.Write(Encoding.ASCII.GetBytes(zplContent));
// PDF is automatically generated in output folder
```

### 🔤 Custom Font Support

Load your own TrueType/OpenType fonts for custom label designs!

**Features:**
- ✅ **Font Directory**: Load all fonts from a directory
- ✅ **Individual Mapping**: Map specific font IDs to font files
- ✅ **Multiple Fonts**: Support for multiple font mappings
- ✅ **TrueType/OpenType**: Support for `.ttf` and `.otf` files

**Usage:**
```bash
# Load all fonts from directory
ZPL2PDF -i label.zpl -o output/ --fonts-dir C:\Fonts

# Map specific font ID
ZPL2PDF -i label.zpl -o output/ --font "A=arial.ttf" --font "B=times.ttf"

# Combine both
ZPL2PDF -i label.zpl -o output/ --fonts-dir C:\Fonts --font "A=custom.ttf"
```

### 📁 Extended File Support

ZPL2PDF now supports additional file extensions commonly used for ZPL files.

**New Extensions:**
- ✅ `.zpl` - Standard ZPL file extension
- ✅ `.imp` - Common extension for ZPL files
- ✅ All previous extensions still supported: `.txt`, `.prn`

**Usage:**
```bash
# All these now work
ZPL2PDF -i label.zpl -o output/
ZPL2PDF -i label.imp -o output/
ZPL2PDF -i label.txt -o output/
ZPL2PDF -i label.prn -o output/
```

### 📝 Custom Output File Naming

Control output PDF filenames directly from ZPL code using special comments!

**Features:**
- ✅ **ZPL Comment Naming**: Use `^FX FileName: MyLabel` in ZPL
- ✅ **Forced Naming**: Use `^FX !FileName: MyLabel` to override `-n` parameter
- ✅ **Priority System**: Clear priority order for filename resolution

**Priority Order:**
1. `^FX !FileName:` (forced) - Highest priority
2. `-n` parameter (command line)
3. `^FX FileName:` (suggestion)
4. Input filename - Lowest priority

**Usage:**
```zpl
^XA
^FX FileName: ShippingLabel
^FO50,50^A0N,50,50^FDHello World^FS
^XZ
```

```bash
# Output will be: ShippingLabel.pdf
ZPL2PDF -i label.zpl -o output/

# Forced naming (overrides -n parameter)
^FX !FileName: CustomName
```

---

## 🔧 Improvements

### Help System
- ✅ Updated `-help` with all new options in 8 languages
- ✅ Reorganized help sections: TCP Server and Advanced Options
- ✅ Better documentation of rendering engines

### Documentation
- ✅ New `LABELARY_API.md` - Complete Labelary API integration guide
- ✅ New `ROADMAP.md` - Future development roadmap
- ✅ New `MANUAL_TESTES_V3.md` - Comprehensive manual test suite with 20 test cases

### Error Handling
- ✅ Better error messages for API failures
- ✅ Improved network error handling
- ✅ Graceful fallback when Labelary API is unavailable

---

## 🔄 Changes

### PDF Generation
- **Labelary Mode**: Generates smaller, vector-based PDFs with higher quality
- **Offline Mode**: Maintains same quality as v2.x (BinaryKits rendering)

### Rendering Pipeline
- **Modular Architecture**: New factory pattern for renderer selection
- **Extensible Design**: Easy to add new rendering engines in the future

### Help Display
- **Reorganized Sections**: TCP Server and Advanced Options clearly separated
- **Better Organization**: More intuitive help structure

---

## 📦 Technical Details

### New Files
- `src/Infrastructure/Rendering/LabelaryRenderer.cs` - Labelary API integration
- `src/Infrastructure/Rendering/RendererFactory.cs` - Renderer selection logic
- `src/Infrastructure/Rendering/FallbackRenderer.cs` - Auto mode with fallback
- `src/Infrastructure/TcpServer/TcpPrinterServer.cs` - TCP server implementation
- `src/Infrastructure/TcpServer/TcpServerManager.cs` - Server lifecycle management
- `src/Presentation/TcpServerModeHandler.cs` - TCP server command handler
- `docs/LABELARY_API.md` - Labelary API documentation
- `docs/ROADMAP.md` - Future development roadmap
- `docs/MANUAL_TESTES_V3.md` - Manual test guide

### Modified Files
- `src/Shared/LabelFileReader.cs` - Extended file extensions, forced filename support
- `src/Application/Services/ConversionService.cs` - Renderer integration
- `src/Presentation/HelpDisplay.cs` - New help sections
- `src/Shared/Localization/ResourceKeys.cs` - New localization keys
- All `Resources/Messages.*.resx` files - New translations for all 8 languages

---

## 🔄 Migration from v2.0.1 to v3.0.0

### Compatibility
- ✅ **Fully Compatible**: Version 3.0.0 maintains full compatibility with v2.0.1
- ✅ **No Breaking Changes**: All existing commands and configurations continue to work
- ✅ **Backward Compatible**: Default behavior unchanged (uses BinaryKits offline renderer)

### New Features (Optional)
- **Labelary API**: Opt-in feature, requires internet connection
- **TCP Server**: New mode, doesn't affect existing workflows
- **Custom Fonts**: Optional enhancement, not required

### Recommendations
1. **Test New Features**: Try Labelary API for better quality rendering
2. **Explore TCP Server**: Consider using virtual printer mode for integrations
3. **Update Scripts**: No changes needed, but you can add `--renderer` parameter if desired

---

## 📚 Installation

### Windows

#### Via Installer (Recommended)
```powershell
# Download and run the installer
ZPL2PDF-Setup-3.0.0.exe
```

#### Via WinGet
```powershell
winget install brunoleocam.ZPL2PDF
```

#### Via ZIP
```powershell
# Extract the ZIP file
Expand-Archive ZPL2PDF-v3.0.0-win-x64.zip -DestinationPath C:\ZPL2PDF
```

### Linux

#### Debian/Ubuntu (DEB)
```bash
sudo dpkg -i ZPL2PDF-v3.0.0-linux-amd64.deb
sudo apt-get install -f  # Install dependencies if needed
```

#### Fedora/CentOS (RPM Tarball)
```bash
tar -xzf ZPL2PDF-v3.0.0-linux-x64-rpm.tar.gz
# Manually install files to /usr/bin and /usr/share
```

#### Generic Tarball
```bash
tar -xzf ZPL2PDF-v3.0.0-linux-x64.tar.gz
sudo cp ZPL2PDF /usr/local/bin/
```

### macOS

```bash
# Extract the tarball
tar -xzf ZPL2PDF-v3.0.0-osx-arm64.tar.gz

# Move to /usr/local/bin
sudo mv ZPL2PDF /usr/local/bin/
```

### Docker

```bash
# Pull the image
docker pull brunoleocam/zpl2pdf:3.0.0

# Run with Labelary API support
docker run --rm -v $(pwd):/data brunoleocam/zpl2pdf:3.0.0 -i input.zpl -o output/ --renderer labelary
```

---

## 🎯 Use Cases

### 1. High-Fidelity Label Rendering
```bash
# Use Labelary API for exact printer emulation
ZPL2PDF -i label.zpl -o output/ --renderer labelary
```

### 2. Virtual Printer Integration
```bash
# Start TCP server for application integration
ZPL2PDF server start --port 9101 -o output/

# Applications can send ZPL directly to localhost:9101
```

### 3. Custom Font Labels
```bash
# Use custom fonts for branded labels
ZPL2PDF -i label.zpl -o output/ --fonts-dir ./fonts --font "A=brand.ttf"
```

### 4. Batch Processing with Auto Fallback
```bash
# Try Labelary, automatically fallback if offline
ZPL2PDF -i label.zpl -o output/ --renderer auto
```

---

## 📊 Release Statistics

- **11 Supported Platforms**: Windows, Linux, macOS on multiple architectures
- **3 Rendering Engines**: Offline (BinaryKits), Labelary API, Auto (fallback)
- **4 Package Formats**: ZIP, TAR.GZ, DEB, RPM
- **1 Professional Installer**: Windows Inno Setup
- **2 Docker Registries**: Docker Hub and GitHub Container Registry
- **100% Self-Contained**: No external dependencies required
- **8 Languages**: Full localization support

---

## 🔐 Security

- ✅ All dependencies updated to secure versions
- ✅ Builds verified with SHA256 checksums
- ✅ Optimized and secure Docker images
- ✅ Safe API communication with Labelary

---

## 🙏 Acknowledgments

We thank all contributors, testers, and users who helped make this release possible!

Special thanks to:
- **Labelary** for providing the excellent ZPL rendering API
- All beta testers who provided valuable feedback
- The open-source community for continuous support

---

## 📞 Support

- **Documentation**: [GitHub Wiki](https://github.com/brunoleocam/ZPL2PDF/wiki)
- **Issues**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- **Discussions**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
- **Labelary API Docs**: [docs/LABELARY_API.md](docs/LABELARY_API.md)

---

## 🔗 Useful Links

- **Repository**: https://github.com/brunoleocam/ZPL2PDF
- **Releases**: https://github.com/brunoleocam/ZPL2PDF/releases
- **Docker Hub**: https://hub.docker.com/r/brunoleocam/zpl2pdf
- **GHCR**: https://github.com/brunoleocam/ZPL2PDF/pkgs/container/zpl2pdf
- **WinGet**: `winget install brunoleocam.ZPL2PDF`
- **Labelary API**: https://labelary.com/

---

## 📝 Full Changelog

To see all detailed changes, check the [CHANGELOG.md](CHANGELOG.md) or visit:
**Full Changelog**: https://github.com/brunoleocam/ZPL2PDF/compare/v2.0.1...v3.0.0

---

**Developed by Bruno Campos**

