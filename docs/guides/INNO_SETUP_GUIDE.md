# 📦 Inno Setup - Complete Guide

Complete guide for creating and customizing the ZPL2PDF Windows installer.

---

## 🎯 **What is Inno Setup?**

**Inno Setup** is a **FREE** professional installer creator for Windows applications.

### **Why Use Inno Setup?**

| Without Installer | With Inno Setup |
|-------------------|-----------------|
| ❌ Extract .zip manually | ✅ One-click installation |
| ❌ Copy files manually | ✅ Automatic file placement |
| ❌ Create shortcuts manually | ✅ Auto-creates shortcuts |
| ❌ Configure manually | ✅ Guided configuration |
| ❌ No uninstaller | ✅ Clean uninstallation |

**Popular apps using Inno Setup:**
- Visual Studio Code
- Git for Windows
- Node.js
- Python (Windows installer)

---

## 📋 **What ZPL2PDF Installer Does**

### **Installation Process:**

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
   Optional: Start Daemon
```

### **What Gets Configured:**

1. ✅ **Files installed** in `C:\Program Files\ZPL2PDF\`
2. ✅ **User folders created** in `Documents\ZPL2PDF Auto Converter\`
3. ✅ **Shortcuts created** in Start Menu
4. ✅ **File association** (.zpl files open with ZPL2PDF)
5. ✅ **Environment variable** (ZPL2PDF_LANGUAGE)
6. ✅ **Registry keys** (install path, version, settings)
7. ✅ **PATH variable** (optional)

---

## 🚀 **Quick Start**

### **Prerequisites:**

```powershell
# 1. Install Inno Setup
winget install JRSoftware.InnoSetup

# 2. Build ZPL2PDF for Windows
.\scripts\build-all-platforms.ps1
```

### **Build Installer:**

```powershell
# Option 1: Automated script
.\installer\build-installer.ps1

# Option 2: Manual compilation
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\ZPL2PDF-Setup.iss

# Output: installer\ZPL2PDF-Setup-2.0.0.exe (~50 MB)
```

### **Test Installation:**

```powershell
# Run the installer
.\installer\ZPL2PDF-Setup-2.0.0.exe

# Or silent installation
.\installer\ZPL2PDF-Setup-2.0.0.exe /SILENT /LANG=english

# Or very silent (no UI at all)
.\installer\ZPL2PDF-Setup-2.0.0.exe /VERYSILENT /LANG=brazilianportuguese
```

---

## 🌍 **Multi-Language Installation**

### **Installer Interface Languages:**

User can choose installer language at start:

```
English
Português (Brasil)
Español
Français
Deutsch
Italiano
日本語
```

### **Application Language Configuration:**

During installation, a **custom page** appears:

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

**Result:** `ZPL2PDF_LANGUAGE` environment variable is set permanently.

---

## 🔧 **Advanced Features**

### **1. File Association**

Double-clicking any `.zpl` file will:
- ✅ Open ZPL2PDF automatically
- ✅ Convert to PDF
- ✅ Save to default output folder

**Registry entries:**
```
HKEY_CLASSES_ROOT\.zpl
  (Default) = "ZPLFile"

HKEY_CLASSES_ROOT\ZPLFile\shell\open\command
  (Default) = "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" "-i" "%1"
```

### **2. PATH Integration**

If user selects "Add to PATH":
- ✅ Can run `ZPL2PDF` from any folder
- ✅ Can use in scripts and batch files

**Example:**
```cmd
# Before: Need full path
C:\Program Files\ZPL2PDF\ZPL2PDF.exe -help

# After: Just the name
ZPL2PDF -help
```

### **3. Start Menu Integration**

Creates organized shortcuts:

```
Start Menu\Programs\ZPL2PDF\
├── ZPL2PDF                    (Shows help)
├── ZPL2PDF (Start Daemon)     (Starts monitoring)
├── ZPL2PDF (Stop Daemon)      (Stops monitoring)
├── Documentation              (Opens README)
├── Sample Files               (Opens samples folder)
├── Watch Folder               (Opens monitoring folder)
├── Output Folder              (Opens PDF output folder)
└── Uninstall ZPL2PDF          (Uninstall program)
```

### **4. Default Folders**

Automatically creates:

```
%USERPROFILE%\Documents\ZPL2PDF Auto Converter\
├── watch\      (Drop ZPL files here for auto-conversion)
└── output\     (Converted PDFs appear here)
```

---

## 🔐 **Code Signing (Optional)**

### **Why Sign?**

- ✅ Windows SmartScreen won't warn users
- ✅ Shows publisher name
- ✅ Professional appearance
- ✅ Prevents tampering

### **How to Sign:**

1. Obtain a code signing certificate
2. Install certificate on your PC
3. Uncomment in `ZPL2PDF-Setup.iss`:

```pascal
[Setup]
SignTool=signtool sign /f "path\to\cert.pfx" /p password /t http://timestamp.digicert.com $f
SignedUninstaller=yes
```

4. Rebuild installer

**Certificate providers:**
- DigiCert
- GlobalSign
- Sectigo
- SSL.com

**Cost:** ~$200-500/year

---

## 📊 **Comparison: Manual vs Installer**

| Aspect | Manual Installation | Inno Setup Installer |
|--------|---------------------|---------------------|
| **User Experience** | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| **Installation Time** | 5-10 minutes | 30 seconds |
| **Configuration** | Manual | Guided |
| **Shortcuts** | Manual | Automatic |
| **File Association** | Manual registry | Automatic |
| **Uninstallation** | Manual deletion | One-click clean |
| **Professional Look** | ❌ | ✅ |
| **Multi-language** | Manual config | Guided selection |

---

## 🎯 **Testing the Installer**

### **Test Checklist:**

```powershell
# 1. Install
.\installer\ZPL2PDF-Setup-2.0.0.exe

# 2. Verify installation
Test-Path "C:\Program Files\ZPL2PDF\ZPL2PDF.exe"

# 3. Test command
& "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" -help

# 4. Test daemon start
& "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" start

# 5. Check status
& "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" status

# 6. Test file association
# Double-click a .zpl file in Explorer

# 7. Check environment variable
[Environment]::GetEnvironmentVariable("ZPL2PDF_LANGUAGE", "User")

# 8. Uninstall
# Control Panel → Programs → Uninstall ZPL2PDF
```

---

## 🐛 **Troubleshooting**

### **Issue: Compilation fails**

```
Error: Cannot find source file
```

**Solution:**
```powershell
# Build artifacts first
.\scripts\build-all-platforms.ps1

# Check if files exist
dir build\publish\ZPL2PDF-v2.0.0-win-x64.zip
```

### **Issue: "ISCC.exe not found"**

**Solution:**
```powershell
# Install Inno Setup
winget install JRSoftware.InnoSetup

# Or use custom path
.\installer\build-installer.ps1 -InnoSetupPath "D:\Tools\Inno Setup 6\ISCC.exe"
```

### **Issue: Language not persisting**

**Solution:**
- Restart application after installation
- Check environment variable is set
- Make sure "Configure language" task was selected during install

---

## 📚 **Customization Examples**

### **Add Custom Components:**

```pascal
[Components]
Name: "main"; Description: "Main Application"; Types: full compact custom; Flags: fixed
Name: "docs"; Description: "Documentation"; Types: full
Name: "samples"; Description: "Sample Files"; Types: full
```

### **Add Pre/Post Install Scripts:**

```pascal
[Run]
Filename: "{app}\post-install.bat"; Description: "Configure application"; Flags: runhidden postinstall

[UninstallRun]
Filename: "{app}\pre-uninstall.bat"; Flags: runhidden; RunOnceId: "Cleanup"
```

### **Custom Installation Messages:**

```pascal
[CustomMessages]
english.WelcomeLabel1=Welcome to ZPL2PDF Setup
english.FinishedLabel=Installation Complete!

brazilianportuguese.WelcomeLabel1=Bem-vindo ao Instalador do ZPL2PDF
brazilianportuguese.FinishedLabel=Instalação Concluída!
```

---

## ✅ **Validation Complete**

After building and testing:

- [x] ✅ **Inno Setup script created** (installer/ZPL2PDF-Setup.iss)
- [x] ✅ **Build script created** (installer/build-installer.ps1)
- [x] ✅ **Documentation created** (installer/README.md)
- [ ] ⏳ **Compile and test** installer
- [ ] ⏳ **Sign installer** (optional)
- [ ] ⏳ **Upload to GitHub Releases**

---

## 🚀 **Next Steps**

1. **Install Inno Setup** (if not already): https://jrsoftware.org/isinfo.php
2. **Build artifacts**: `.\scripts\build-all-platforms.ps1`
3. **Compile installer**: `.\installer\build-installer.ps1`
4. **Test installation** on a clean Windows VM
5. **Upload to GitHub Releases**

**Happy Installing!** 🎉
