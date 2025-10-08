# 🪟 Windows Installation

Complete guide for installing ZPL2PDF on Windows systems.

## 🎯 Overview

ZPL2PDF offers multiple installation methods for Windows:
- ✅ **WinGet** - Recommended, automated installation
- ✅ **Inno Setup Installer** - GUI-based installation
- ✅ **Portable Binary** - No installation required
- ✅ **Docker** - Containerized deployment

---

## 🚀 Method 1: WinGet (Recommended)

### Installation
```powershell
# Install using WinGet
winget install brunoleocam.ZPL2PDF

# Verify installation
ZPL2PDF --help
```

### Update
```powershell
# Update to latest version
winget upgrade brunoleocam.ZPL2PDF

# Update all packages
winget upgrade --all
```

### Uninstall
```powershell
# Uninstall using WinGet
winget uninstall brunoleocam.ZPL2PDF
```

---

## 🎨 Method 2: Inno Setup Installer

### Download and Install
1. Go to [Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Download `ZPL2PDF-Setup-v2.0.0.exe`
3. Run installer **as Administrator**
4. Follow installation wizard

### Installation Options
- **Installation Path**: `C:\Program Files\ZPL2PDF\` (default)
- **Start Menu Shortcuts**: Yes (recommended)
- **Desktop Shortcut**: Optional
- **Add to PATH**: Yes (recommended)
- **Language Selection**: Choose your preferred language

### Features
- ✅ **Multi-language installer** (8 languages)
- ✅ **Automatic PATH configuration**
- ✅ **Start Menu integration**
- ✅ **Desktop shortcuts**
- ✅ **Uninstaller included**

---

## 📦 Method 3: Portable Binary

### Download
```powershell
# Download using PowerShell
Invoke-WebRequest -Uri "https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-win-x64.zip" -OutFile "ZPL2PDF.zip"

# Extract
Expand-Archive -Path "ZPL2PDF.zip" -DestinationPath "C:\ZPL2PDF"
```

### Manual Setup
```powershell
# Add to PATH (current session)
$env:PATH += ";C:\ZPL2PDF"

# Add to PATH (permanent - User)
[Environment]::SetEnvironmentVariable("Path", $env:PATH + ";C:\ZPL2PDF", [EnvironmentVariableTarget]::User)

# Add to PATH (permanent - System, requires admin)
[Environment]::SetEnvironmentVariable("Path", $env:PATH + ";C:\ZPL2PDF", [EnvironmentVariableTarget]::Machine)
```

### Verify Installation
```powershell
# Test executable
C:\ZPL2PDF\ZPL2PDF.exe --help

# Or if added to PATH
ZPL2PDF --help
```

---

## 🐳 Method 4: Docker on Windows

### Prerequisites
- Docker Desktop for Windows
- WSL2 (Windows Subsystem for Linux 2)

### Installation
```powershell
# Pull Docker image
docker pull brunoleocam/zpl2pdf:latest

# Run container
docker run --rm -v ${PWD}:/app/watch -v ${PWD}/output:/app/output brunoleocam/zpl2pdf:latest -i label.txt -o /app/output
```

See [[Docker Deployment]] for detailed Docker instructions.

---

## ⚙️ System Requirements

### Minimum Requirements
- **OS**: Windows 10 (1903+) or Windows 11
- **Architecture**: x64, x86, ARM64
- **RAM**: 512 MB
- **Storage**: 100 MB free space
- **Dependencies**: .NET 9.0 Runtime (included in installers)

### Recommended
- **OS**: Windows 11
- **Architecture**: x64
- **RAM**: 1 GB
- **Storage**: 500 MB free space

---

## 🔧 Post-Installation Configuration

### Set Default Language
```powershell
# Set environment variable (User)
setx ZPL2PDF_LANGUAGE pt-BR

# Set environment variable (System, requires admin)
setx ZPL2PDF_LANGUAGE pt-BR /M

# Verify
echo %ZPL2PDF_LANGUAGE%
```

### Create Configuration File
```powershell
# Navigate to installation directory
cd "C:\Program Files\ZPL2PDF"

# Create configuration file
@"
{
  "language": "en-US",
  "defaultWatchFolder": "C:\\ZPL2PDF\\Watch",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info"
}
"@ | Out-File -FilePath "zpl2pdf.json" -Encoding UTF8
```

### Test Installation
```powershell
# Test with sample ZPL
ZPL2PDF -z "^XA^FO50,50^A0N,50,50^FDHello Windows^FS^XZ" -o C:\Temp -n test.pdf

# Start daemon mode
ZPL2PDF start -l "C:\ZPL2PDF\Watch" -w 7.5 -h 15 -u in

# Check status
ZPL2PDF status

# Stop daemon
ZPL2PDF stop
```

---

## 🔄 Updating ZPL2PDF

### WinGet Update
```powershell
# Check for updates
winget upgrade

# Update ZPL2PDF
winget upgrade brunoleocam.ZPL2PDF
```

### Manual Update
1. Download latest installer or binary
2. Stop daemon if running: `ZPL2PDF stop`
3. Run new installer or replace files
4. Verify new version: `ZPL2PDF --version`

---

## 🗑️ Uninstallation

### WinGet Uninstall
```powershell
winget uninstall brunoleocam.ZPL2PDF
```

### Inno Setup Uninstall
1. Open **Settings** → **Apps** → **Apps & features**
2. Find **ZPL2PDF**
3. Click **Uninstall**

Or use the uninstaller:
```powershell
# Run uninstaller
& "C:\Program Files\ZPL2PDF\unins000.exe"
```

### Manual Uninstall
```powershell
# Stop daemon
ZPL2PDF stop

# Remove from PATH
# Settings → System → About → Advanced system settings → Environment Variables

# Delete installation folder
Remove-Item -Recurse -Force "C:\ZPL2PDF"

# Remove configuration
Remove-Item -Force "$env:APPDATA\ZPL2PDF\zpl2pdf.json"
```

---

## 🏢 Enterprise Deployment

### Group Policy Deployment
```powershell
# Create GPO for software installation
# Use Inno Setup installer with silent mode
ZPL2PDF-Setup-v2.0.0.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /DIR="C:\Program Files\ZPL2PDF"
```

### Chocolatey (Future)
```powershell
# Coming soon
choco install zpl2pdf
```

### SCCM/Intune Deployment
Create deployment package with:
- **Install command**: `ZPL2PDF-Setup-v2.0.0.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART`
- **Uninstall command**: `"C:\Program Files\ZPL2PDF\unins000.exe" /VERYSILENT`
- **Detection method**: Check for `C:\Program Files\ZPL2PDF\ZPL2PDF.exe`

---

## 🐛 Troubleshooting

### Issue: "Windows protected your PC"
```powershell
# Solution: Click "More info" → "Run anyway"
# Or verify digital signature
Get-AuthenticodeSignature "ZPL2PDF-Setup-v2.0.0.exe"
```

### Issue: "Installation requires admin privileges"
```powershell
# Solution: Run installer as administrator
# Right-click installer → Run as administrator
```

### Issue: "PATH not updated"
```powershell
# Solution: Refresh environment variables
$env:PATH = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")

# Or restart PowerShell/CMD
```

### Issue: "DLL not found"
```powershell
# Solution: Install Visual C++ Redistributable
# Download from Microsoft
# Or reinstall using Inno Setup installer
```

---

## 🔗 Related Topics

- [[Installation Guide]] - General installation overview
- [[Configuration]] - Post-installation configuration
- [[Basic Usage]] - Getting started with ZPL2PDF
- [[Troubleshooting]] - Windows-specific issues
