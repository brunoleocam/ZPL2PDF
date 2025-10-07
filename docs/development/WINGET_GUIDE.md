# 📦 WinGet Packaging Guide

## 🎯 **Overview**

This guide explains how to publish ZPL2PDF to the **Windows Package Manager** (WinGet) community repository.

---

## 📋 **What is WinGet?**

[WinGet](https://github.com/microsoft/winget-pkgs) is Microsoft's official package manager for Windows 10/11.

**Benefits:**
- ✅ Users can install with: `winget install brunoleocam.ZPL2PDF`
- ✅ Automatic updates via Windows Update integration
- ✅ Part of Windows built-in tools
- ✅ Centralized package management

---

## 🏗️ **WinGet Manifest Structure**

WinGet uses **3 YAML files** per package version:

```
manifests/b/brunoleocam/ZPL2PDF/2.0.0/
├── brunoleocam.ZPL2PDF.yaml                  # Version manifest
├── brunoleocam.ZPL2PDF.installer.yaml        # Installer details
└── brunoleocam.ZPL2PDF.locale.en-US.yaml     # English locale
└── brunoleocam.ZPL2PDF.locale.pt-BR.yaml     # Portuguese locale (optional)
```

### **1. Version Manifest** (`brunoleocam.ZPL2PDF.yaml`)

```yaml
PackageIdentifier: brunoleocam.ZPL2PDF
PackageVersion: 2.0.0
DefaultLocale: en-US
ManifestType: version
ManifestVersion: 1.6.0
```

### **2. Installer Manifest** (`brunoleocam.ZPL2PDF.installer.yaml`)

```yaml
PackageIdentifier: brunoleocam.ZPL2PDF
PackageVersion: 2.0.0
InstallerType: inno
Scope: machine
Installers:
- Architecture: x64
  InstallerUrl: https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-Setup-2.0.0.exe
  InstallerSha256: <SHA256_HASH>
```

### **3. Locale Manifest** (`brunoleocam.ZPL2PDF.locale.en-US.yaml`)

```yaml
PackageIdentifier: brunoleocam.ZPL2PDF
PackageVersion: 2.0.0
PackageLocale: en-US
Publisher: Bruno Campos
PackageName: ZPL2PDF
ShortDescription: Convert ZPL files to PDF
Description: |
  Full package description...
Tags:
- zpl
- pdf
- converter
```

---

## 🚀 **Automated Submission (Recommended)**

### **Method 1: GitHub Actions (Automatic on Release)**

When you create a GitHub Release, the workflow automatically:

1. ✅ Downloads the installer from the release
2. ✅ Calculates SHA256 hash
3. ✅ Updates manifest files
4. ✅ Creates PR to microsoft/winget-pkgs
5. ✅ Monitors PR status

**Workflow file:** `.github/workflows/winget-publish.yml`

**How to trigger:**
```bash
# Create a release on GitHub
# The workflow runs automatically!
```

---

### **Method 2: Manual Script**

Run the automated PowerShell script:

```powershell
# Prerequisites
gh auth login  # Authenticate with GitHub CLI

# Submit to WinGet
.\scripts\winget-submit.ps1 -Version 2.0.0

# Dry run (test without submitting)
.\scripts\winget-submit.ps1 -Version 2.0.0 -DryRun

# Skip validation
.\scripts\winget-submit.ps1 -Version 2.0.0 -SkipValidation

# Custom installer path
.\scripts\winget-submit.ps1 -Version 2.0.0 -InstallerPath "C:\path\to\installer.exe"
```

**What the script does:**
1. ✅ Validates prerequisites (Git, GitHub CLI)
2. ✅ Locates installer and calculates SHA256
3. ✅ Updates manifest files with correct hashes
4. ✅ Validates manifests with `winget validate`
5. ✅ Forks/updates your winget-pkgs fork
6. ✅ Creates a new branch
7. ✅ Commits and pushes changes
8. ✅ Creates Pull Request to microsoft/winget-pkgs

---

## 🔧 **Manual Submission (Advanced)**

If you prefer full manual control:

### **Step 1: Prepare Manifests**

```bash
# Update manifests in manifests/ folder
# Edit: brunoleocam.ZPL2PDF.installer.yaml
# - Update PackageVersion
# - Update InstallerUrl
# - Update InstallerSha256 (calculate with: Get-FileHash -Algorithm SHA256)
```

### **Step 2: Validate Locally**

```powershell
winget validate .\manifests\
```

### **Step 3: Fork WinGet Repository**

```bash
# Fork on GitHub: https://github.com/microsoft/winget-pkgs
# Or via CLI:
gh repo fork microsoft/winget-pkgs --clone=false
```

### **Step 4: Clone and Create Branch**

```bash
git clone https://github.com/brunoleocam/winget-pkgs.git
cd winget-pkgs

# Sync with upstream
git remote add upstream https://github.com/microsoft/winget-pkgs.git
git fetch upstream
git merge upstream/master

# Create branch
git checkout -b brunoleocam.ZPL2PDF-2.0.0
```

### **Step 5: Copy Manifests**

```bash
# Create directory structure
# Pattern: manifests/<first-letter>/<publisher>/<package>/<version>/
mkdir -p manifests/b/brunoleocam/ZPL2PDF/2.0.0

# Copy manifests
cp manifests/*.yaml manifests/b/brunoleocam/ZPL2PDF/2.0.0/
```

### **Step 6: Commit and Push**

```bash
git add .
git commit -m "New version: brunoleocam.ZPL2PDF version 2.0.0"
git push origin brunoleocam.ZPL2PDF-2.0.0
```

### **Step 7: Create Pull Request**

```bash
gh pr create --repo microsoft/winget-pkgs \
  --title "brunoleocam.ZPL2PDF version 2.0.0" \
  --body "Automated submission for brunoleocam.ZPL2PDF version 2.0.0"
```

---

## ✅ **Validation Process**

### **Local Validation**

```powershell
# Install WinGet (if not already installed)
# Included in Windows 11 by default

# Validate manifests
winget validate .\manifests\

# Test installation (from local manifests)
winget install --manifest .\manifests\
```

### **CI Validation**

After submitting PR, Microsoft's CI automatically:

1. ✅ Validates YAML syntax
2. ✅ Checks manifest schema compliance
3. ✅ Downloads installer
4. ✅ Verifies SHA256 hash
5. ✅ Tests installation
6. ✅ Scans for malware
7. ✅ Verifies uninstallation

**Common validation errors:**
- ❌ Invalid SHA256 hash
- ❌ Installer URL not accessible
- ❌ Invalid YAML syntax
- ❌ Missing required fields
- ❌ Version already exists

---

## 🔑 **Key Information**

### **Package Details**

| Field | Value |
|-------|-------|
| **Package Identifier** | `brunoleocam.ZPL2PDF` |
| **Publisher** | `brunoleocam` |
| **Package Name** | `ZPL2PDF` |
| **Moniker** | `zpl2pdf` |
| **Repository** | https://github.com/microsoft/winget-pkgs |

### **Installer Details**

| Field | Value |
|-------|-------|
| **Type** | Inno Setup (`inno`) |
| **Scope** | Machine (all users) |
| **Architecture** | x64 |
| **Silent Install** | `/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-` |
| **Upgrade Behavior** | `install` |

### **URLs**

| Resource | URL |
|----------|-----|
| **Package Page** | https://github.com/brunoleocam/ZPL2PDF |
| **Releases** | https://github.com/brunoleocam/ZPL2PDF/releases |
| **Issues** | https://github.com/brunoleocam/ZPL2PDF/issues |
| **License** | https://github.com/brunoleocam/ZPL2PDF/blob/main/LICENSE |
| **WinGet Repo** | https://github.com/microsoft/winget-pkgs |
| **Your Fork** | https://github.com/brunoleocam/winget-pkgs |

---

## 📝 **PR Review Process**

### **Timeline**

| Stage | Duration | What Happens |
|-------|----------|--------------|
| **Automated Validation** | 5-10 min | CI runs all validation checks |
| **Moderator Review** | 1-7 days | Human review of package |
| **Merge** | Instant | PR merged to master |
| **Availability** | ~1 hour | Package available via `winget install` |

### **What Moderators Check**

- ✅ Package name is appropriate
- ✅ Publisher is legitimate
- ✅ Installer is from official source
- ✅ License information is correct
- ✅ No malware or suspicious behavior
- ✅ Manifest follows best practices

### **Common Feedback**

| Issue | Solution |
|-------|----------|
| **Update description** | More detailed package description |
| **Add tags** | Include relevant search tags |
| **Fix SHA256** | Recalculate installer hash |
| **Update URL** | Use canonical GitHub release URL |

---

## 🐛 **Troubleshooting**

### **Issue: "Invalid SHA256"**

```powershell
# Recalculate SHA256
$hash = (Get-FileHash -Path "installer\ZPL2PDF-Setup-2.0.0.exe" -Algorithm SHA256).Hash
Write-Host $hash
```

### **Issue: "Installer not found"**

```bash
# Verify release exists
curl -I https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-Setup-2.0.0.exe

# Check release page
https://github.com/brunoleocam/ZPL2PDF/releases/tag/v2.0.0
```

### **Issue: "Validation failed"**

```powershell
# Run local validation
winget validate .\manifests\

# Check YAML syntax
https://www.yamllint.com/
```

### **Issue: "PR was closed without merge"**

**Possible reasons:**
- ❌ Validation failed
- ❌ Duplicate submission
- ❌ Non-compliant package
- ❌ Policy violation

**Solution:**
1. Read closing comment carefully
2. Fix the issue
3. Create a new PR

---

## 📚 **Resources**

### **Official Documentation**

- [WinGet Documentation](https://docs.microsoft.com/en-us/windows/package-manager/)
- [Manifest Schema](https://github.com/microsoft/winget-pkgs/tree/master/doc/manifest)
- [Contributing Guide](https://github.com/microsoft/winget-pkgs/blob/master/CONTRIBUTING.md)
- [Authoring Manifests](https://docs.microsoft.com/en-us/windows/package-manager/package/manifest)

### **Tools**

- [WinGet CLI](https://github.com/microsoft/winget-cli)
- [WinGet Create](https://github.com/microsoft/winget-create)
- [GitHub CLI](https://cli.github.com/)
- [YAML Validator](https://www.yamllint.com/)

### **Community**

- [WinGet Community](https://github.com/microsoft/winget-pkgs/discussions)
- [Discord Server](https://discord.gg/winget)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/winget)

---

## ✅ **Checklist for v2.0.0 Submission**

- [ ] Installer built and tested
- [ ] SHA256 hash calculated
- [ ] Manifests updated (all 3-4 files)
- [ ] Local validation passed (`winget validate`)
- [ ] Installer uploaded to GitHub Release
- [ ] Release is public (not draft)
- [ ] Fork of winget-pkgs updated
- [ ] PR created to microsoft/winget-pkgs
- [ ] CI validation passed (green checks)
- [ ] Moderator approved
- [ ] PR merged
- [ ] Package available (`winget search brunoleocam.ZPL2PDF`)

---

## 🎯 **Quick Commands**

```powershell
# Install prerequisites
winget install GitHub.cli

# Authenticate
gh auth login

# Submit to WinGet (automated)
.\scripts\winget-submit.ps1 -Version 2.0.0

# Validate manifests
winget validate .\manifests\

# Test installation from local manifests
winget install --manifest .\manifests\

# Search for package (after merge)
winget search zpl2pdf

# Install package (after merge)
winget install brunoleocam.ZPL2PDF

# Show package info
winget show brunoleocam.ZPL2PDF

# Upgrade package
winget upgrade brunoleocam.ZPL2PDF
```

---

**This process ensures ZPL2PDF is available to millions of Windows users!** 🚀
