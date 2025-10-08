# 📦 Linux Package Distribution Guide

How `.deb` and `.rpm` packages are created and distributed for ZPL2PDF.

## 🎯 Distribution Strategy

Instead of maintaining official repositories (PPA for Debian/Ubuntu or COPR for Fedora/CentOS), we provide pre-built `.deb` and `.rpm` packages directly in GitHub Releases.

### Advantages:
- ✅ **Simpler maintenance** - No repository infrastructure needed
- ✅ **Faster releases** - Direct upload to GitHub
- ✅ **Universal compatibility** - Works on all distros
- ✅ **Easy installation** - Users download and install with one command

---

## 🔨 Building Packages

### Prerequisites
- **Docker** installed and running
- **PowerShell** (Windows) or **Bash** (Linux/macOS)

### Build All Packages (Recommended)
```powershell
# Windows (PowerShell)
.\scripts\build-linux-packages.ps1

# Outputs:
# - build/publish/ZPL2PDF-v2.0.0-linux-amd64.deb (37.67 MB)
# - build/publish/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz (48.93 MB)
```

### Build Individual Packages
```powershell
# Build only .deb
.\scripts\build-linux-packages.ps1 -DebOnly

# Build only .rpm tarball
.\scripts\build-linux-packages.ps1 -RpmOnly
```

### What Gets Built
- **`.deb` package**: Ready for `dpkg -i` installation on Debian/Ubuntu
- **`.rpm.tar.gz` tarball**: Contains RPM structure, ready to be extracted to system

---

## 📤 Publishing to GitHub Releases

### 1. Build Packages
```powershell
# Build all packages using Docker
.\scripts\build-linux-packages.ps1
```

### 2. Upload to Release
When creating a GitHub Release:
1. Go to [Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Click "Draft a new release"
3. Upload packages from `build/publish/`:
   - `ZPL2PDF-v2.0.0-linux-amd64.deb` (37.67 MB)
   - `ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz` (48.93 MB)

---

## 👥 User Installation

### Debian/Ubuntu
```bash
# Download from GitHub Releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# Install
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# Fix dependencies (if needed)
sudo apt-get install -f

# Verify
zpl2pdf --help
```

### Fedora/CentOS/RHEL
```bash
# Download from GitHub Releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# Extract to system
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# Make executable
sudo chmod +x /usr/bin/ZPL2PDF

# Create symbolic link (optional, for lowercase command)
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# Verify
zpl2pdf --help
```

---

## 🧪 Testing Packages

### Test .deb
```bash
# Check package contents
dpkg-deb -c ZPL2PDF-v2.0.0-linux-amd64.deb

# Check package info
dpkg-deb -I ZPL2PDF-v2.0.0-linux-amd64.deb

# Install locally for testing
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb
```

### Test .rpm tarball
```bash
# Check tarball contents
tar -tzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# Extract to temporary location for testing
mkdir -p /tmp/rpm-test
tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /tmp/rpm-test

# Check binary
/tmp/rpm-test/usr/bin/ZPL2PDF --help

# Install to system
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /
```

---

## 📋 Package Contents

### Files Installed:
- `/usr/bin/ZPL2PDF` - Main executable
- `/usr/share/doc/zpl2pdf/README.md` - Documentation
- `/usr/share/doc/zpl2pdf/LICENSE` - License
- `/usr/share/doc/zpl2pdf/CHANGELOG.md` - Changelog
- `/usr/share/man/man1/zpl2pdf.1.gz` - Man page
- `/var/zpl2pdf/watch/` - Default watch folder
- `/var/zpl2pdf/output/` - Default output folder

### Dependencies:
- **Debian/Ubuntu**: `libgdiplus`, `libc6-dev`
- **Fedora/CentOS**: `libgdiplus`, `glibc-devel`

---

## 🔄 Uninstallation

### Debian/Ubuntu
```bash
sudo apt-get remove zpl2pdf
# or
sudo dpkg -r zpl2pdf
```

### Fedora/CentOS/RHEL
```bash
sudo dnf remove zpl2pdf
# or
sudo rpm -e zpl2pdf
```

---

## 📝 Future Enhancements

### Official Repositories (Future)
If demand increases, we may consider:

**Debian/Ubuntu PPA:**
```bash
sudo add-apt-repository ppa:brunoleocam/zpl2pdf
sudo apt update
sudo apt install zpl2pdf
```

**Fedora/CentOS COPR:**
```bash
sudo dnf copr enable brunoleocam/zpl2pdf
sudo dnf install zpl2pdf
```

**Arch Linux AUR:**
```bash
yay -S zpl2pdf
# or
paru -S zpl2pdf
```

---

## 🔗 Related Documentation

- [[Linux Installation]] - Installation guide for Linux
- [[Package Formats]] - Package format details
- [[Distribution Channels]] - All distribution methods
- [[Build Process]] - Complete build instructions
