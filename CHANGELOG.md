# 📋 Changelog

All notable changes to ZPL2PDF will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [3.0.0] - 2025-12-18

### 🎉 Major Release - Labelary Integration & TCP Server

This is a **major release** with new rendering engines, TCP server mode, and custom font support.

### ✨ Added

#### Multiple Rendering Engines
- **Labelary API Integration**: High-fidelity ZPL rendering via Labelary API with direct PDF output
- **Renderer Selection**: `--renderer` parameter with options: `offline` (BinaryKits), `labelary` (API), `auto` (fallback)
- **Direct PDF Generation**: Labelary mode generates vector PDFs directly from API (smaller, higher quality)
- **Batch Processing**: Automatic batching for 50+ labels with PDF merging
- **Smart Fallback**: Auto mode tries Labelary first, falls back to BinaryKits if offline

#### TCP Server Mode (Virtual Printer)
- **Virtual Zebra Printer**: Acts as a TCP printer on configurable port (default: 9101)
- **Server Commands**: `server start`, `server stop`, `server status`
- **Foreground Mode**: `--foreground` option for debugging
- **Independent Operation**: Runs independently from daemon mode (both can run simultaneously)
- **Custom Output**: Configure output directory with `-o` parameter

#### Custom Font Support
- **Font Directory**: `--fonts-dir` to load all fonts from a directory
- **Individual Font Mapping**: `--font "A=path/to/font.ttf"` for specific font IDs
- **Multiple Mappings**: Support for multiple `--font` parameters
- **TrueType/OpenType**: Support for `.ttf` and `.otf` font files

#### Extended File Support
- **New Extensions**: Added support for `.zpl` and `.imp` file extensions
- **Consistent Handling**: All extensions (`.txt`, `.prn`, `.zpl`, `.imp`) treated equally

#### Custom Output File Naming
- **ZPL Comment Naming**: `^FX FileName: MyLabel` sets output filename
- **Forced Naming**: `^FX !FileName: MyLabel` overrides `-n` parameter
- **Priority System**: `^FX !FileName:` > `-n` parameter > `^FX FileName:` > input filename

### 🔧 Improved

- **Help System**: Updated `-help` with all new options in 8 languages
- **Documentation**: New `LABELARY_API.md` and `ROADMAP.md` documentation
- **Test Coverage**: Comprehensive manual test suite with 20 test cases
- **Error Messages**: Better error handling for API failures and network issues

### 📦 Technical Details

- **Files Added**:
  - `src/Infrastructure/Rendering/LabelaryRenderer.cs` - Labelary API integration
  - `src/Infrastructure/Rendering/RendererFactory.cs` - Renderer selection logic
  - `src/Infrastructure/Rendering/FallbackRenderer.cs` - Auto mode with fallback
  - `src/Infrastructure/TcpServer/TcpPrinterServer.cs` - TCP server implementation
  - `src/Infrastructure/TcpServer/TcpServerManager.cs` - Server lifecycle management
  - `src/Presentation/TcpServerModeHandler.cs` - TCP server command handler
  - `docs/LABELARY_API.md` - Labelary API documentation
  - `docs/ROADMAP.md` - Future development roadmap
  - `docs/MANUAL_TESTES_V3.md` - Manual test guide

- **Files Modified**:
  - `src/Shared/LabelFileReader.cs` - Extended file extensions, forced filename support
  - `src/Application/Services/ConversionService.cs` - Renderer integration
  - `src/Presentation/HelpDisplay.cs` - New help sections
  - `src/Shared/Localization/ResourceKeys.cs` - New localization keys
  - All `Resources/Messages.*.resx` files - New translations

### 🔄 Changed

- **PDF Generation**: Labelary mode generates smaller, vector-based PDFs
- **Rendering Pipeline**: Modular renderer architecture with factory pattern
- **Help Display**: Reorganized with TCP Server and Advanced Options sections

---

## [2.0.1] - 2025-12-16

### 🐛 Fixed

#### Docker/Linux CLI Mode Issue
- **Missing Fonts in Alpine Linux**: Fixed "Value cannot be null (Parameter 'asset')" error when running in CLI mode on Docker/Amazon Linux
- **Root Cause**: SkiaSharp/BinaryKits.Zpl requires fonts for text rendering, which were missing in the Alpine base image
- **Solution**: Added `fontconfig`, `ttf-dejavu`, `ttf-liberation`, and `font-noto` packages to Dockerfile
- **Impact**: ZPL2PDF now works correctly in both daemon and CLI modes on all Linux environments

#### ZPL ^FN (Field Number) Tag Issue
- **^FN Not Rendering with ^FD**: Fixed issue where ZPL lines containing `^FN` tags were not appearing in generated PDFs
- **Root Cause**: BinaryKits.Zpl.Viewer doesn't fully support field templates when `^FN` is followed by `^FD`
- **Solution**: Implemented ZPL preprocessing to remove `^FN<number>` tags when followed by `^FD`, preserving the field data for direct rendering
- **Example**: `^FO90,12^A0N,20,20^FN6^FDHello World^FS` → `^FO90,12^A0N,20,20^FDHello World^FS`

### 🔧 Improved

- **Code Quality**: Added ReDoS protection timeout to regex operations
- **Performance**: Pre-compiled regex patterns for ZPL preprocessing
- **Validation**: Improved input validation consistency (`IsNullOrWhiteSpace` vs `IsNullOrEmpty`)
- **Documentation**: Enhanced Dockerfile comments explaining font packages and their purpose

### 📦 Technical Details

- **Files Modified**:
  - `Dockerfile`: Added font packages for SkiaSharp text rendering support
  - `src/Shared/LabelFileReader.cs`: Added `PreprocessZpl` method with compiled regex
  - `src/Application/Services/ConversionService.cs`: Integrated ZPL preprocessing in conversion pipeline

---

## [2.0.0] - 2025-01-07

### 🎉 Major Release - Complete Rewrite

This is a **major release** with complete architecture refactoring and numerous new features.

### ✨ Added

#### Multi-Language Support
- **8 Languages**: English, Portuguese, Spanish, French, German, Italian, Japanese, Chinese
- **Automatic Detection**: Detects system language automatically
- **Permanent Configuration**: `--set-language` command for persistent settings
- **Environment Variable**: `ZPL2PDF_LANGUAGE` support
- **Configuration File**: Language setting in `zpl2pdf.json`
- **Localization System**: Complete `.resx` resource files with `LocalizationManager`

#### Daemon Mode (Auto-Conversion)
- **Automatic Folder Monitoring**: Watches folder and converts files automatically
- **Background Process**: Daemon runs in background with PID management
- **Daemon Commands**: `start`, `stop`, `status`, `run`
- **Custom Watch Folders**: Configure any folder to monitor
- **Fixed Dimensions**: Optional fixed dimensions for all files
- **Dynamic Extraction**: Extract dimensions from each ZPL file automatically

#### Cross-Platform Support
- **Windows**: x64, x86, ARM64
- **Linux**: x64, ARM64, ARM (Ubuntu, Debian, CentOS, Alpine)
- **macOS**: Intel (x64), Apple Silicon (ARM64)
- **Self-Contained**: No .NET installation required
- **Optimized Builds**: Platform-specific optimizations

#### Docker Support
- **Alpine Linux Base**: Ultra-lightweight (470MB)
- **Multi-Architecture**: linux/amd64, linux/arm64
- **Docker Compose**: Ready-to-use configurations
- **Health Checks**: Built-in container health monitoring
- **GitHub Container Registry**: `ghcr.io/brunoleocam/zpl2pdf`
- **Docker Hub**: `brunoleocam/zpl2pdf`

#### Professional Windows Installer
- **Inno Setup**: Professional installer with multi-language UI
- **Language Selection**: Choose app language during installation
- **File Association**: `.zpl` files open with ZPL2PDF
- **PATH Integration**: Optional system PATH addition
- **Smart Shortcuts**: Start/Stop daemon from Start Menu
- **Clean Uninstallation**: Preserves user data optionally

#### Build & Distribution Automation
- **Build Scripts**: `build-all-platforms.ps1` and `.sh`
- **8 Platform Builds**: Single command builds all platforms
- **Automated Checksums**: SHA256 for all builds
- **Release Automation**: `release.ps1` and `.sh` scripts
- **GitHub Actions**: Automated Docker publishing on releases

#### Architecture & Code Quality
- **Clean Architecture**: Separation of concerns (Application, Domain, Infrastructure, Presentation)
- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Value Objects**: Immutable domain objects (`ConversionOptions`, `LabelDimensions`, `DaemonConfiguration`)
- **Service Layer**: Centralized business logic (`ConversionService`, `FileValidationService`, `UnitConversionService`)
- **Dependency Injection**: Interfaces for all services
- **90%+ Test Coverage**: Comprehensive unit and integration tests

#### Documentation
- **Complete Restructure**: Organized docs into user guides, development guides, and translations
- **Multi-Language Docs**: README available in 8 languages
- **Docker Documentation**: Comprehensive Docker usage and publishing guides
- **Build Documentation**: Detailed build scripts documentation
- **CI/CD Documentation**: Complete automation workflow documentation

### 🔄 Changed
- **Architecture**: Migrated from monolithic to Clean Architecture (4-layer separation)
- **Performance**: Improved conversion speed by 300% with async processing
- **Memory Usage**: Reduced memory footprint by 50% with optimized rendering
- **Error Handling**: Enhanced error messages with localization support
- **Code Quality**: Improved maintainability and testability (90%+ coverage)
- **Build Process**: Automated cross-platform builds with single command
- **Docker Image**: Optimized from 674MB to 470MB (Alpine Linux)
- **Installation Path**: Changed from AppData to Program Files (Windows)
- **Installer Size**: Reduced to 35.44 MB with LZMA2 compression

### 🐛 Fixed
- **File Locking Issues**: Implemented robust retry mechanisms for files in use
- **Memory Leaks**: Fixed memory leaks in long-running daemon processes
- **Cross-Platform Compatibility**: Resolved platform-specific path issues
- **Dimension Calculation**: Improved accuracy of ZPL dimension extraction
- **Error Recovery**: Better error handling with graceful degradation
- **Encoding Issues**: Fixed multi-language character encoding
- **Docker Permissions**: Fixed file permission issues in containers

### 🔐 Security
- **Dependency Updates**: Updated all dependencies to latest secure versions
- **Non-Root Docker**: Containers run as non-root user `zpl2pdf`
- **Input Validation**: Enhanced validation to prevent malicious ZPL code
- **Environment Variables**: Secure handling of sensitive configuration
- **File Permissions**: Proper permissions for daemon-created folders

### 🗑️ Removed
- **Legacy Code**: Removed old monolithic architecture
- **Hardcoded Strings**: Replaced with localization resources
- **Duplicate Code**: Eliminated 200+ lines of code duplication
- **Temporary Files**: Removed dependency on temporary file creation

## [1.0.0] - 2023-12-01

### Added
- **Basic Conversion**: Convert ZPL files to PDF
- **Command Line Interface**: Basic CLI with essential parameters
- **Windows Support**: Native Windows support
- **Basic Error Handling**: Simple error handling and logging
- **Documentation**: Basic README and usage instructions

### Features
- Single file conversion
- Basic dimension support
- PDF generation
- Command line interface
- Windows executable

---

## Migration Guide

### From v1.0 to v2.0

#### Breaking Changes
- **Command Line Arguments**: Some argument names have changed
- **Configuration**: New configuration file format
- **Architecture**: Internal architecture completely refactored

#### Migration Steps
1. **Update Command Line Usage**:
   ```bash
   # Old (v1.0)
   ZPL2PDF.exe -input file.txt -output folder
   
   # New (v2.0)
   ZPL2PDF.exe -i file.txt -o folder
   ```

2. **Update Configuration**:
   - Create new `zpl2pdf.json` configuration file
   - Migrate any custom settings to new format

3. **Update Scripts**:
   - Update any automation scripts to use new argument names
   - Test daemon mode functionality

#### New Features to Explore
- **Daemon Mode**: Set up automatic folder monitoring
- **Docker**: Use containerized deployment
- **Configuration**: Leverage JSON configuration files
- **Cross-Platform**: Deploy on Linux and macOS

## Support

For migration assistance or questions about new features, please:
- Check the [Documentation](https://github.com/brunoleocam/ZPL2PDF/wiki)
- Open an [Issue](https://github.com/brunoleocam/ZPL2PDF/issues)
- Join [Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
