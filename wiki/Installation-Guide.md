# 📦 Installation Guide

ZPL2PDF supports multiple installation methods across different platforms.

## 🪟 Windows

### Option 1: WinGet (Recommended)
```bash
winget install brunoleocam.ZPL2PDF
```

### Option 2: Direct Download
1. Go to [Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Download `ZPL2PDF-v2.0.0-win-x64.zip`
3. Extract to desired folder
4. Add to PATH (optional)

### Option 3: Inno Setup Installer
1. Download `ZPL2PDF-Setup-v2.0.0.exe`
2. Run installer with admin privileges
3. Follow installation wizard

## 🐧 Linux

### Option 1: Docker (Recommended)
```bash
# Pull image
docker pull brunoleocam/zpl2pdf:latest

# Run container
docker run -v /path/to/watch:/app/watch -v /path/to/output:/app/output brunoleocam/zpl2pdf:latest
```

### Option 2: Binary Download
```bash
# Download and extract
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Make executable
chmod +x ZPL2PDF

# Install dependencies
sudo apt-get install libgdiplus libc6-dev  # Ubuntu/Debian
sudo yum install libgdiplus glibc-devel    # CentOS/RHEL
```

### Option 3: Package Managers (Coming Soon)
```bash
# Ubuntu/Debian (PPA)
sudo add-apt-repository ppa:brunoleocam/zpl2pdf
sudo apt update && sudo apt install zpl2pdf

# Fedora/CentOS (COPR)
sudo dnf copr enable brunoleocam/zpl2pdf
sudo dnf install zpl2pdf
```

## 🍎 macOS

### Option 1: Binary Download
```bash
# Download and extract
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-arm64.tar.gz -o ZPL2PDF.tar.gz
tar -xzf ZPL2PDF.tar.gz

# Make executable
chmod +x ZPL2PDF
```

### Option 2: Homebrew (Coming Soon)
```bash
brew install brunoleocam/zpl2pdf/zpl2pdf
```

## 🔧 System Requirements

### Minimum Requirements
- **OS**: Windows 10+, Ubuntu 18.04+, macOS 10.15+
- **RAM**: 512 MB
- **Storage**: 100 MB free space
- **Architecture**: x64, ARM64, ARM

### Dependencies
- **Linux**: libgdiplus, glibc
- **Docker**: Docker 20.10+

## ✅ Verification

Test your installation:
```bash
ZPL2PDF --help
```

Expected output:
```
ZPL2PDF - Conversor ZPL para PDF
Usage: ZPL2PDF.exe [opções]
...
```

## 🔗 Next Steps

- [[Basic Usage]] - Learn how to use ZPL2PDF
- [[Configuration]] - Configure settings
- [[Docker Deployment]] - Advanced Docker setup
