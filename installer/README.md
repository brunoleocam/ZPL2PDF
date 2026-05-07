# 📦 ZPL2PDF - Windows Installer (Inno Setup)

Professional Windows installer for ZPL2PDF with multi-language support.

---

## 🎯 **Features**

The installer provides:

- ✅ **Multi-language interface** (8 languages)
- ✅ **Automatic file association** (.zpl and .imp files)
- ✅ **PATH integration** (optional)
- ✅ **Language configuration** during installation
- ✅ **Default folders creation** (watch/output)
- ✅ **Start Menu shortcuts**
- ✅ **Desktop icon** (optional)
- ✅ **Clean uninstallation**

---

## 📋 **Requirements**

### 1. Inno Setup Compiler

Download and install from: https://jrsoftware.org/isinfo.php

**Minimum version:** 6.2.0 or higher

**Installation:**
```powershell
# Option 1: Download installer
# Visit: https://jrsoftware.org/isdl.php

# Option 2: Using winget
winget install JRSoftware.InnoSetup

# Option 3: Using Chocolatey
choco install innosetup
```

### 2. Build Artifacts

Build all platforms first:

```powershell
.\scripts\build-all-platforms.ps1
```

This creates the required files in `build/publish/`.

---

## 🚀 **How to Build the Installer**

### **Option 1: GUI (Easiest)**

1. Open **Inno Setup Compiler**
2. Click "**File**" → "**Open**"
3. Select `installer/ZPL2PDF-Setup.iss`
4. Click "**Build**" → "**Compile**" (or press `F9`)
5. Wait ~30 seconds
6. Installer will be created: `installer/ZPL2PDF-Setup-3.0.0.exe`

### **Option 2: Command Line**

```powershell
# Using Inno Setup compiler
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\ZPL2PDF-Setup.iss

# Output: installer/ZPL2PDF-Setup-3.0.0.exe
```

### **Option 3: Automated Script**

```powershell
# Use the build script
.\installer\build-installer.ps1

# Or with custom version
.\installer\build-installer.ps1 -Version "3.0.0"
```

### **Optional: Clean old installer artifacts**

```powershell
# Remove old installer artifacts and keep only one version
.\scripts\release\cleanup-installer-output.ps1 -KeepVersion "3.1.1"

# Preview only (no deletion)
.\scripts\release\cleanup-installer-output.ps1 -KeepVersion "3.1.1" -WhatIf
```

---

## 📦 **What Gets Installed**

### **Files:**

```
C:\Program Files\ZPL2PDF\
├── ZPL2PDF.exe              (Main executable)
├── zpl2pdf.json.example     (Configuration template)
├── ZPL2PDF.ico              (Application icon)
├── docs\
│   ├── README.md
│   ├── README.pt.md
│   ├── LICENSE
│   ├── CHANGELOG.md
│   └── CONTRIBUTING.md
└── samples\
    ├── example.txt
    ├── test-10x15.txt
    └── ...
```

### **User Folders:**

```
%USERPROFILE%\Documents\ZPL2PDF Auto Converter\
├── watch\                   (Drop ZPL files here)
└── output\                  (PDFs generated here)
```

### **Registry:**

```
HKEY_CURRENT_USER\Software\ZPL2PDF\
├── InstallPath = "C:\Program Files\ZPL2PDF"
├── Version = "3.0.0"
└── WatchFolder = "...\Documents\ZPL2PDF Auto Converter\watch"

HKEY_CURRENT_USER\Environment\
└── ZPL2PDF_LANGUAGE = "pt-BR"  (if configured)
```

### **Start Menu:**

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

---

## 🌍 **Multi-Language Support**

### **Installer Languages:**

During installation, user can choose:

1. English
2. Português (Brasil)
3. Español
4. Français
5. Deutsch
6. Italiano
7. 日本語 (Japanese)

### **Application Language:**

The installer offers a **language configuration page** where user selects the ZPL2PDF interface language:

- English (en-US)
- Português Brasil (pt-BR)
- Español (es-ES)
- Français (fr-FR)
- Deutsch (de-DE)
- Italiano (it-IT)
- 日本語 (ja-JP)
- 中文 (zh-CN)

This sets the `ZPL2PDF_LANGUAGE` environment variable permanently.

---

## ⚙️ **Installation Options**

### **Standard Installation:**
- ✅ Install application
- ✅ Create shortcuts
- ✅ Create default folders
- ✅ Register .zpl and .imp file associations

### **Optional Tasks:**

| Task | Description | Default |
|------|-------------|---------|
| **Desktop Icon** | Create icon on desktop | ❌ Unchecked |
| **Quick Launch** | Add to Quick Launch (Windows 7) | ❌ Unchecked |
| **Add to PATH** | Add to system PATH variable | ❌ Unchecked |
| **Set Language** | Configure application language | ✅ Checked |

---

## 🔧 **Customization**

### **Change Version:**

Edit `ZPL2PDF-Setup.iss`:

```pascal
#define MyAppVersion "3.0.0"  // Change here
```

### **Add More Files:**

```pascal
[Files]
Source: "path\to\file"; DestDir: "{app}"; Flags: ignoreversion
```

### **Add More Shortcuts:**

```pascal
[Icons]
Name: "{group}\MyShortcut"; Filename: "{app}\{#MyAppExeName}"; Parameters: "run"
```

### **Add Custom Actions:**

```pascal
[Run]
Filename: "{app}\setup-helper.exe"; Description: "Configure application"; Flags: postinstall
```

---

## 🐛 **Troubleshooting**

### **Error: "Cannot find file"**

**Solution:** Build all platforms first:
```powershell
.\scripts\build-all-platforms.ps1
```

### **Error: "ISCC.exe not found"**

**Solution:** Install Inno Setup or update the path in build script.

### **Installer doesn't show multi-language**

**Solution:** Make sure all `.isl` files are installed with Inno Setup (default installation includes them).

### **Language not persisting**

**Solution:** Check that `ZPL2PDF_LANGUAGE` environment variable is set:
```powershell
[Environment]::GetEnvironmentVariable("ZPL2PDF_LANGUAGE", "User")
```

---

## 📊 **Installer Statistics**

| Metric | Value |
|--------|-------|
| **Installer Size** | ~50 MB (compressed) |
| **Installed Size** | ~120 MB |
| **Supported Languages** | 8 (installer) |
| **Compression** | LZMA2 (maximum) |
| **Architectures** | x64 (Windows 10/11) |

---

## 🎯 **Next Steps**

After building the installer:

1. ✅ Test installation on clean Windows VM
2. ✅ Test all language options
3. ✅ Test file association (.zpl files)
4. ✅ Test daemon mode start/stop
5. ✅ Test uninstallation
6. ✅ Sign the installer (optional, for code signing)
7. ✅ Upload to GitHub Releases

---

## 📚 **Additional Resources**

- [Inno Setup Documentation](https://jrsoftware.org/ishelp/)
- [Inno Setup Examples](https://jrsoftware.org/ishelp/topic_samples.htm)
- [Code Signing Guide](https://jrsoftware.org/ishelp/topic_setup_signtool.htm)

---

## ✅ **Validation Checklist**

- [ ] Inno Setup installed
- [ ] Build artifacts created
- [ ] Installer compiles without errors
- [ ] Installation succeeds
- [ ] Application launches correctly
- [ ] Language selection works
- [ ] File association works (.zpl and .imp files)
- [ ] Start Menu shortcuts created
- [ ] Daemon start/stop works
- [ ] Uninstallation succeeds
- [ ] Environment variable set correctly

---

**Ready to build?** Open `ZPL2PDF-Setup.iss` in Inno Setup and press `F9`! 🚀
