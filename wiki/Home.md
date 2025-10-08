# 🏠 ZPL2PDF Wiki

Welcome to the comprehensive documentation for ZPL2PDF - the powerful ZPL to PDF converter with Clean Architecture and enterprise features.

## 🎯 What is ZPL2PDF?

ZPL2PDF is a cross-platform command-line tool that converts ZPL (Zebra Programming Language) files to high-quality PDF documents. It supports both individual file conversion and automatic folder monitoring for batch processing.

## ✨ Key Features

- 🔄 **Dual Operation Modes**: Conversion and Daemon
- 📏 **Intelligent Dimension Handling**: Automatic ZPL extraction
- 🌍 **Cross-Platform**: Windows, Linux, macOS
- 🏗️ **Clean Architecture**: SOLID principles
- ⚡ **High Performance**: Async processing
- 🏢 **Enterprise-Ready**: PID management and logging
- 🌐 **Multi-Language**: 8 languages supported

## 🚀 Quick Start

### Installation
```bash
# Windows (WinGet)
winget install brunoleocam.ZPL2PDF

# Docker
docker pull brunoleocam/zpl2pdf:latest

# Direct Download
# Visit: https://github.com/brunoleocam/ZPL2PDF/releases
```

### Basic Usage
```bash
# Convert a single file
ZPL2PDF.exe -i label.txt -o output_folder -n my_label.pdf

# Start daemon mode
ZPL2PDF.exe start -l "C:\WatchFolder" -w 7.5 -h 15 -u in

# Check status
ZPL2PDF.exe status
```

## 📚 Documentation Sections

### 🛠️ **User Guides**
- [[Installation Guide]] - Complete installation instructions
- [[Basic Usage]] - Quick start and fundamentals
- [[Conversion Mode]] - Detailed file conversion guide
- [[Daemon Mode]] - Automatic folder monitoring
- [[Configuration]] - Global settings and preferences
- [[Configuration Files]] - Configuration file reference
- [[Multi-Language Setup]] - Language configuration
- [[Performance Optimization]] - Speed and resource optimization

### 🖥️ **Platform Installation**
- [[Windows Installation]] - Windows-specific setup
- [[Linux Installation]] - Linux package management
- [[macOS Installation]] - macOS setup guide

### 🐳 **Deployment**
- [[Docker Deployment]] - Container deployment guide

### 🔧 **Advanced Topics**
- [[Troubleshooting]] - Common issues and solutions

### 🏗️ **Development**
- [[Architecture Overview]] - Clean Architecture principles
- [[Development Setup]] - Development environment
- [[Testing Guide]] - Testing strategies
- [[Contributing Guidelines]] - How to contribute

### 📦 **Build & Release**
- [[Build Process]] - How builds are created
- [[Release Process]] - Release management
- [[Package Formats]] - Supported package types
- [[Distribution Channels]] - Where to get ZPL2PDF

## 🌟 Supported Languages

- 🇺🇸 **English** (en-US)
- 🇧🇷 **Portuguese** (pt-BR)
- 🇪🇸 **Spanish** (es-ES)
- 🇫🇷 **French** (fr-FR)
- 🇩🇪 **German** (de-DE)
- 🇮🇹 **Italian** (it-IT)
- 🇯🇵 **Japanese** (ja-JP)
- 🇨🇳 **Chinese** (zh-CN)

## 🎯 Use Cases

### Individual File Conversion
Perfect for converting single ZPL files to PDF with custom dimensions and quality settings.

### Batch Processing
Ideal for processing multiple files with consistent settings and automatic folder monitoring.

### Enterprise Integration
Suitable for server environments with PID management, logging, and high availability.

### Development Workflow
Great for developers working with ZPL code who need to preview labels as PDFs.

## 🔗 External Resources

- 📁 **GitHub Repository**: https://github.com/brunoleocam/ZPL2PDF
- 🐳 **Docker Hub**: https://hub.docker.com/r/brunoleocam/zpl2pdf
- 📦 **WinGet Package**: `winget install brunoleocam.ZPL2PDF`
- 📋 **Issue Tracker**: https://github.com/brunoleocam/ZPL2PDF/issues
- 💬 **Discussions**: https://github.com/brunoleocam/ZPL2PDF/discussions

## 🤝 Contributing

We welcome contributions! See our [[Contributing Guidelines]] for details on how to:
- Report bugs
- Suggest features
- Submit pull requests
- Improve documentation

## 📄 License

ZPL2PDF is licensed under the MIT License. See [LICENSE](https://github.com/brunoleocam/ZPL2PDF/blob/main/LICENSE) for details.

---

**Need help?** Check our [[Troubleshooting]] guide or open an [issue](https://github.com/brunoleocam/ZPL2PDF/issues) on GitHub!
