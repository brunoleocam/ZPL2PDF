# 📋 Changelog

All notable changes to ZPL2PDF will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
