# 📦 WinGet Manifests

This folder contains the **Windows Package Manager (WinGet)** manifest files for ZPL2PDF.

---

## 📁 **Files**

| File | Purpose |
|------|---------|
| `brunoleocam.ZPL2PDF.yaml` | Version manifest (links all manifests together) |
| `brunoleocam.ZPL2PDF.installer.yaml` | Installer details (URL, SHA256, switches) |
| `brunoleocam.ZPL2PDF.locale.en-US.yaml` | English locale (default) |
| `brunoleocam.ZPL2PDF.locale.pt-BR.yaml` | Portuguese locale (optional) |

---

## 🚀 **Usage**

### **Validate Manifests**

```powershell
winget validate .\manifests\
```

### **Test Installation (Local)**

```powershell
winget install --manifest .\manifests\
```

### **Submit to WinGet (Automated)**

```powershell
.\scripts\winget-submit.ps1 -Version 2.0.0
```

---

## 📝 **Updating for New Version**

### **Manual Update**

1. Update `PackageVersion` in all files
2. Update `InstallerUrl` with new release URL
3. Calculate new SHA256:
   ```powershell
   Get-FileHash -Path "installer\ZPL2PDF-Setup-2.0.0.exe" -Algorithm SHA256
   ```
4. Update `InstallerSha256` in installer manifest
5. Update `ReleaseDate` to current date
6. Validate: `winget validate .\manifests\`

### **Automated Update**

The script `scripts\winget-submit.ps1` automatically:
- ✅ Calculates SHA256
- ✅ Updates all manifest files
- ✅ Validates manifests
- ✅ Creates PR to microsoft/winget-pkgs

---

## 🔗 **Links**

- **WinGet Repository**: https://github.com/microsoft/winget-pkgs
- **Documentation**: [WINGET_GUIDE.md](../docs/development/WINGET_GUIDE.md)
- **Manifest Schema**: https://aka.ms/winget-manifest.version.1.6.0.schema.json

---

## ⚠️ **Important Notes**

1. **Never commit with placeholder SHA256** - Always calculate actual hash
2. **Installer must be publicly accessible** - Ensure release is published
3. **Version must match exactly** - Tag, installer filename, and manifests
4. **Validate before submitting** - Use `winget validate` locally

---

**These manifests are automatically submitted to microsoft/winget-pkgs on each release!** 🚀
