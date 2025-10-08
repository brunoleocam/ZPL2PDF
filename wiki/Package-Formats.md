# 📦 Package Formats

Overview of ZPL2PDF package formats and distribution methods.

## 🎯 Available Formats

### Windows
- **WinGet Package** - Microsoft package manager
- **Inno Setup Installer** - GUI installer (.exe)
- **Portable Binary** - Standalone executable (.zip)
- **Chocolatey** - Community package manager (coming soon)

### Linux
- **Debian Package** - .deb for Ubuntu/Debian (coming soon)
- **RPM Package** - .rpm for CentOS/Fedora (coming soon)
- **Portable Binary** - Standalone executable (.tar.gz)
- **Docker Image** - Container deployment

### macOS
- **Homebrew Formula** - Package manager (coming soon)
- **Portable Binary** - Standalone executable (.tar.gz)
- **Docker Image** - Container deployment

---

## 🪟 Windows Packages

### WinGet Manifest
```yaml
PackageIdentifier: brunoleocam.ZPL2PDF
PackageVersion: 2.0.0
Publisher: Bruno Campos
PackageName: ZPL2PDF
License: MIT
ShortDescription: ZPL to PDF Converter
```

Installation:
```powershell
winget install brunoleocam.ZPL2PDF
```

### Inno Setup Installer
- **Size**: ~35 MB
- **Features**: GUI wizard, PATH integration, Start Menu shortcuts
- **Languages**: 8 languages supported
- **Admin**: Requires administrator privileges

---

## 🐧 Linux Packages

### Debian Package (.deb)
```bash
# Structure
zpl2pdf_2.0.0_amd64.deb
├── DEBIAN/
│   ├── control
│   ├── postinst
│   └── prerm
└── usr/
    ├── bin/ZPL2PDF
    └── share/
        ├── doc/zpl2pdf/
        └── man/man1/zpl2pdf.1.gz
```

Installation:
```bash
sudo dpkg -i zpl2pdf_2.0.0_amd64.deb
sudo apt-get install -f  # Fix dependencies
```

### RPM Package (.rpm)
```bash
# Structure
zpl2pdf-2.0.0-1.x86_64.rpm
├── usr/bin/ZPL2PDF
├── usr/share/doc/zpl2pdf/
└── usr/share/man/man1/zpl2pdf.1.gz
```

Installation:
```bash
sudo rpm -ivh zpl2pdf-2.0.0-1.x86_64.rpm
# Or
sudo dnf install zpl2pdf-2.0.0-1.x86_64.rpm
```

---

## 🐳 Docker Images

### Image Variants
```bash
# Latest stable
brunoleocam/zpl2pdf:latest

# Specific version
brunoleocam/zpl2pdf:2.0.0

# Alpine (smaller)
brunoleocam/zpl2pdf:alpine

# Development
brunoleocam/zpl2pdf:dev
```

### Multi-Architecture Support
- **linux/amd64** - Standard 64-bit
- **linux/arm64** - ARM 64-bit (Raspberry Pi, etc.)
- **linux/arm/v7** - ARM 32-bit

---

## 📊 Package Comparison

| Format | Size | Install Time | Update Method | Platform |
|--------|------|-------------|---------------|----------|
| **WinGet** | 48 MB | < 30s | `winget upgrade` | Windows |
| **Inno Setup** | 35 MB | < 60s | Manual | Windows |
| **.deb** | 45 MB | < 15s | `apt upgrade` | Debian/Ubuntu |
| **.rpm** | 45 MB | < 15s | `dnf upgrade` | Fedora/CentOS |
| **Docker** | 470 MB | < 2min | `docker pull` | All |
| **Portable** | 48 MB | Instant | Manual | All |

---

## 🔗 Related Topics

- [[Installation Guide]] - How to install
- [[Distribution Channels]] - Where to get packages
- [[Build Process]] - How packages are built
