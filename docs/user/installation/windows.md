# 🪟 Windows Installation Guide

Complete guide for installing ZPL2PDF on Windows.

---

## 🎯 **Installation Methods**

| Method | Ease | Features | Recommended For |
|--------|------|----------|-----------------|
| **WinGet** | ⭐⭐⭐⭐⭐ | Auto-updates, simple | Most users |
| **Inno Setup** | ⭐⭐⭐⭐ | Full installer, shortcuts | Traditional users |
| **Manual** | ⭐⭐ | Direct executable | Advanced users |

---

## 🚀 **Method 1: WinGet (Recommended)**

### **Quick Install**
```powershell
# Install latest version
winget install brunoleocam.ZPL2PDF

# Verify installation
ZPL2PDF -help
```

### **Update**
```powershell
# Update to latest version
winget upgrade brunoleocam.ZPL2PDF
```

### **Uninstall**
```powershell
# Remove completely
winget uninstall brunoleocam.ZPL2PDF
```

### **Advantages**
- ✅ **Automatic updates** via Windows Update
- ✅ **One command** installation
- ✅ **Integrated** with Windows Package Manager
- ✅ **Clean uninstall**

---

## 📦 **Method 2: Inno Setup Installer**

### **Download**
1. Go to [Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Download `ZPL2PDF-Setup-2.0.0.exe` (~50 MB)
3. Run the installer

### **Installation Process**
```
1. Welcome Screen (Multi-language)
   ↓
2. License Agreement
   ↓
3. Select Installation Folder
   ↓
4. Select Language (for ZPL2PDF interface)
   ↓
5. Select Tasks:
   - [ ] Create desktop icon
   - [ ] Add to PATH
   - [x] Configure language
   ↓
6. Install Files
   ↓
7. Create Shortcuts & Registry
   ↓
8. Optional: Launch ZPL2PDF
```

### **What Gets Installed**
```
C:\Program Files\ZPL2PDF\
├── ZPL2PDF.exe              (Main executable)
├── zpl2pdf.json.example     (Configuration template)
├── ZPL2PDF.ico              (Application icon)
├── docs\                    (Documentation)
└── samples\                 (Sample files)

%USERPROFILE%\Documents\ZPL2PDF Auto Converter\
├── watch\                   (Drop ZPL files here)
└── output\                  (PDFs generated here)
```

### **Start Menu Shortcuts**
```
Start Menu\Programs\ZPL2PDF\
├── ZPL2PDF                          (Launch help)
├── ZPL2PDF (Start Daemon)           (Start monitoring)
├── ZPL2PDF (Stop Daemon)            (Stop monitoring)
├── Documentation
├── Sample Files
├── Watch Folder
├── Output Folder
└── Uninstall ZPL2PDF
```

### **File Association**
Double-clicking `.zpl` files will:
- ✅ Open ZPL2PDF automatically
- ✅ Convert to PDF
- ✅ Save to default output folder

### **Advantages**
- ✅ **Professional installer** with multi-language support
- ✅ **Automatic shortcuts** and file associations
- ✅ **PATH integration** (optional)
- ✅ **Language configuration** during installation
- ✅ **Clean uninstallation**

---

## 📁 **Method 3: Manual Installation**

### **Download**
1. Go to [Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Download `ZPL2PDF-v2.0.0-win-x64.zip`
3. Extract to desired folder (e.g., `C:\ZPL2PDF\`)

### **Setup**
```powershell
# Extract to Program Files (optional)
Expand-Archive -Path "ZPL2PDF-v2.0.0-win-x64.zip" -DestinationPath "C:\Program Files\ZPL2PDF\" -Force

# Add to PATH (optional)
$env:PATH += ";C:\Program Files\ZPL2PDF"
[Environment]::SetEnvironmentVariable("PATH", $env:PATH, "User")
```

### **Create Folders**
```powershell
# Create default folders
New-Item -ItemType Directory -Path "$env:USERPROFILE\Documents\ZPL2PDF Auto Converter\watch" -Force
New-Item -ItemType Directory -Path "$env:USERPROFILE\Documents\ZPL2PDF Auto Converter\output" -Force
```

### **Advantages**
- ✅ **No installer** required
- ✅ **Portable** (can run from USB)
- ✅ **Full control** over installation location

---

## 🌍 **Language Configuration**

### **During Installation (Inno Setup)**
The installer offers a language selection page:
```
┌─────────────────────────────────────────┐
│  Language Configuration                 │
├─────────────────────────────────────────┤
│  Choose the application language:       │
│                                          │
│  ( ) English (en-US)                     │
│  (•) Português Brasil (pt-BR)            │
│  ( ) Español (es-ES)                     │
│  ( ) Français (fr-FR)                    │
│  ( ) Deutsch (de-DE)                     │
│  ( ) Italiano (it-IT)                    │
│  ( ) 日本語 (ja-JP)                       │
│  ( ) 中文 (zh-CN)                         │
│                                          │
│  This sets the ZPL2PDF_LANGUAGE          │
│  environment variable.                   │
└─────────────────────────────────────────┘
```

### **After Installation**
```powershell
# Set language permanently
setx ZPL2PDF_LANGUAGE "pt-BR"

# Set language temporarily (current session)
$env:ZPL2PDF_LANGUAGE = "es-ES"

# Verify language
ZPL2PDF --show-language
```

### **Supported Languages**
- 🇺🇸 English (en-US)
- 🇧🇷 Portuguese (pt-BR)
- 🇪🇸 Spanish (es-ES)
- 🇫🇷 French (fr-FR)
- 🇩🇪 German (de-DE)
- 🇮🇹 Italian (it-IT)
- 🇯🇵 Japanese (ja-JP)
- 🇨🇳 Chinese (zh-CN)

---

## 🧪 **Testing Installation**

### **Basic Tests**
```powershell
# Test 1: Help command
ZPL2PDF -help

# Test 2: Status check
ZPL2PDF status

# Test 3: Language display
ZPL2PDF --show-language

# Test 4: Version info
ZPL2PDF --version
```

### **Conversion Test**
```powershell
# Create test file
@"
^XA
^FO50,50^A0N,50,50^FDTest Label^FS
^XZ
"@ | Out-File -FilePath "test.zpl" -Encoding ASCII

# Convert to PDF
ZPL2PDF -i test.zpl -o . -n test.pdf -w 10 -h 5 -u cm

# Verify PDF was created
Test-Path test.pdf
```

### **Daemon Test**
```powershell
# Start daemon
ZPL2PDF start

# Check status
ZPL2PDF status

# Stop daemon
ZPL2PDF stop
```

---

## 🔧 **Configuration**

### **Environment Variables**
```powershell
# Set language
setx ZPL2PDF_LANGUAGE "pt-BR"

# Set log level
setx ZPL2PDF_LOG_LEVEL "Debug"
```

### **Configuration File**
Create `zpl2pdf.json`:
```json
{
  "language": "pt-BR",
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

### **File Locations**
| File Type | Location |
|-----------|----------|
| **Executable** | `C:\Program Files\ZPL2PDF\ZPL2PDF.exe` |
| **Config** | `C:\Program Files\ZPL2PDF\zpl2pdf.json` |
| **Watch Folder** | `%USERPROFILE%\Documents\ZPL2PDF Auto Converter\watch` |
| **Output Folder** | `%USERPROFILE%\Documents\ZPL2PDF Auto Converter\output` |

---

## 🐛 **Troubleshooting**

### **Issue: "ZPL2PDF is not recognized"**
**Solution:**
```powershell
# Check if in PATH
Get-Command ZPL2PDF -ErrorAction SilentlyContinue

# Add to PATH manually
$env:PATH += ";C:\Program Files\ZPL2PDF"
```

### **Issue: "Access denied" errors**
**Solution:**
- Run PowerShell as Administrator
- Check file permissions
- Ensure antivirus isn't blocking

### **Issue: Language not changing**
**Solution:**
```powershell
# Check environment variable
[Environment]::GetEnvironmentVariable("ZPL2PDF_LANGUAGE", "User")

# Restart terminal/command prompt
# Or restart application
```

### **Issue: Daemon won't start**
**Solution:**
```powershell
# Check if already running
ZPL2PDF status

# Stop any existing daemon
ZPL2PDF stop

# Check logs
ZPL2PDF start
```

### **Issue: Files not converting**
**Solution:**
```powershell
# Check watch folder exists
Test-Path "$env:USERPROFILE\Documents\ZPL2PDF Auto Converter\watch"

# Check file permissions
Get-Acl "$env:USERPROFILE\Documents\ZPL2PDF Auto Converter\watch"
```

---

## 📊 **System Requirements**

### **Minimum Requirements**
- ✅ **Windows 10** (version 1903 or later)
- ✅ **Windows 11** (any version)
- ✅ **4 GB RAM** (recommended)
- ✅ **100 MB** disk space
- ✅ **.NET 9.0 Runtime** (included in installer)

### **Supported Architectures**
- ✅ **x64** (64-bit) - Recommended
- ✅ **x86** (32-bit) - Limited support
- ✅ **ARM64** - Windows on ARM

### **Windows Versions**
| Version | Support | Notes |
|---------|---------|-------|
| **Windows 11** | ✅ Full | Recommended |
| **Windows 10** | ✅ Full | Version 1903+ |
| **Windows 8.1** | ⚠️ Limited | May work |
| **Windows 7** | ❌ No | End of life |

---

## 🔄 **Updates**

### **WinGet Users**
```powershell
# Check for updates
winget upgrade --query brunoleocam.ZPL2PDF

# Update to latest
winget upgrade brunoleocam.ZPL2PDF
```

### **Inno Setup Users**
1. Download new installer from [Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Run installer (will upgrade existing installation)
3. No need to uninstall first

### **Manual Installation Users**
1. Download new version
2. Replace executable
3. Keep configuration files

---

## 🗑️ **Uninstallation**

### **WinGet**
```powershell
winget uninstall brunoleocam.ZPL2PDF
```

### **Inno Setup**
1. **Control Panel** → **Programs** → **Uninstall ZPL2PDF**
2. Or use **Start Menu** → **ZPL2PDF** → **Uninstall ZPL2PDF**

### **Manual**
```powershell
# Remove executable
Remove-Item "C:\Program Files\ZPL2PDF" -Recurse -Force

# Remove from PATH (if added)
# Edit environment variables manually
```

---

## 📚 **Additional Resources**

- **[Usage Guide](../usage/)** - How to use ZPL2PDF
- **[Configuration Guide](../usage/configuration.md)** - Advanced configuration
- **[Troubleshooting](../troubleshooting/)** - Common issues and solutions
- **[GitHub Repository](https://github.com/brunoleocam/ZPL2PDF)** - Source code
- **[WinGet Package](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/brunoleocam/ZPL2PDF)** - Package information

---

## ✅ **Installation Checklist**

### **Pre-Installation**
- [ ] Windows 10 version 1903+ or Windows 11
- [ ] Administrator privileges (for system-wide install)
- [ ] Antivirus temporarily disabled (if needed)

### **Installation**
- [ ] Choose installation method (WinGet recommended)
- [ ] Complete installation without errors
- [ ] Verify executable is accessible

### **Post-Installation**
- [ ] Run `ZPL2PDF -help` successfully
- [ ] Test language configuration
- [ ] Test basic conversion
- [ ] Test daemon mode
- [ ] Verify file associations (.zpl files)

---

## 🚀 **Next Steps**

1. ✅ **Choose installation method** (WinGet recommended)
2. ✅ **Install ZPL2PDF**
3. ✅ **Test installation** with basic commands
4. ✅ **Configure language** (if desired)
5. ✅ **Read [Usage Guide](../usage/)** to learn how to use ZPL2PDF
6. ✅ **Try [Quick Start Guide](../usage/quick-start.md)** for immediate results

**Welcome to ZPL2PDF!** 🎉
