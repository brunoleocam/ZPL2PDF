# 🚀 Release Process Guide

Complete guide to creating and managing releases for ZPL2PDF.

---

## 🎯 **Release Strategy Overview**

ZPL2PDF uses **Semantic Versioning (SemVer)** with automated release process:

```
Version Format: MAJOR.MINOR.PATCH
Example:        2.1.3

Major (2): Breaking changes
Minor (1): New features (backward compatible)  
Patch (3): Bug fixes (backward compatible)
```

### **Release Types**

| Type | Version Bump | Example | Frequency |
|------|-------------|---------|-----------|
| **Patch** | 2.1.0 → 2.1.1 | Bug fixes | As needed |
| **Minor** | 2.1.0 → 2.2.0 | New features | Monthly |
| **Major** | 2.1.0 → 3.0.0 | Breaking changes | Quarterly |

---

## 🔄 **Automated Release Process**

### **Complete Automation Flow**

```
Developer creates Git tag
        ↓
GitHub Actions triggered
        ↓
┌─────────────────────────────────────┐
│   Step 1: Run All Tests             │
│   - Unit tests (all platforms)      │
│   - Integration tests               │
│   - Code coverage validation        │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│   Step 2: Build All Platforms       │
│   - Windows (x64, x86, ARM64)       │
│   - Linux (x64, ARM64, ARM)         │
│   - macOS (x64, ARM64)              │
│   - Create archives (.zip/.tar.gz)  │
│   - Generate SHA256 checksums       │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│   Step 3: Build Docker Images       │
│   - Alpine Linux (multi-arch)       │
│   - Tag: latest, version, alpine    │
│   - Push to GitHub Container        │
│     Registry + Docker Hub           │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│   Step 4: Build Windows Installer   │
│   - Inno Setup compilation          │
│   - Multi-language support          │
│   - Generate installer checksum     │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│   Step 5: Build Linux Packages      │
│   - .deb package (Debian/Ubuntu)    │
│   - .rpm tarball (Fedora/CentOS)    │
│   - Generate package checksums      │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│   Step 6: Create GitHub Release     │
│   - Upload all platform builds      │
│   - Upload Windows installer        │
│   - Upload Linux packages           │
│   - Upload checksums                │
│   - Generate release notes          │
└─────────────────────────────────────┘
        ↓
┌─────────────────────────────────────┐
│   Step 7: Create WinGet PR          │
│   - Update winget manifest          │
│   - Create PR to microsoft/winget   │
│   - Automated submission            │
└─────────────────────────────────────┘
        ↓
✅ Release Complete! Available on all platforms
```

---

## 📋 **Manual Release Process**

### **Step 1: Pre-Release Preparation**

#### **Update Version Numbers**

Update version in these files:

```bash
# 1. Update project file
# File: ZPL2PDF.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <Version>2.1.0</Version>  <!-- Update this -->
    <AssemblyVersion>2.1.0.0</AssemblyVersion>
    <FileVersion>2.1.0.0</FileVersion>
  </PropertyGroup>
</Project>

# 2. Update Windows installer
# File: installer/ZPL2PDF-Setup.iss
#define AppVersion "2.1.0"  // Update this

# 3. Update Linux packages
# File: debian/control
Version: 2.1.0  // Update this

# File: rpm/zpl2pdf.spec
Version: 2.1.0  // Update this
```

#### **Update Documentation**

```bash
# Update CHANGELOG.md
## [2.1.0] - 2025-01-07

### Added
- New Docker multi-architecture support
- Enhanced error handling

### Fixed  
- Memory leak in daemon mode
- File locking issues

### Changed
- Improved build performance

# Update README.md (if needed)
# Update version badges
# Update installation instructions
```

#### **Commit Version Bump**

```bash
git add .
git commit -m "chore: bump version to 2.1.0"
git push origin main
```

### **Step 2: Create Git Tag**

```bash
# Create annotated tag
git tag -a v2.1.0 -m "Release version 2.1.0"

# Push tag to trigger automation
git push origin v2.1.0
```

### **Step 3: Create GitHub Release**

1. **Go to GitHub Releases**: https://github.com/brunoleocam/ZPL2PDF/releases
2. **Click "Create a new release"**
3. **Select tag**: `v2.1.0`
4. **Release title**: `ZPL2PDF v2.1.0`
5. **Description**: Copy from CHANGELOG.md
6. **Click "Publish release"**

### **Step 4: Monitor Automation**

Watch GitHub Actions workflows:

1. **CI Tests**: https://github.com/brunoleocam/ZPL2PDF/actions/workflows/ci.yml
2. **Docker Build**: https://github.com/brunoleocam/ZPL2PDF/actions/workflows/docker-publish.yml
3. **WinGet PR**: https://github.com/brunoleocam/ZPL2PDF/actions/workflows/winget-publish.yml
4. **Linux Packages**: https://github.com/brunoleocam/ZPL2PDF/actions/workflows/build-linux-packages.yml

### **Step 5: Validate Release**

#### **Check GitHub Release**

```bash
# Verify all files uploaded
https://github.com/brunoleocam/ZPL2PDF/releases/tag/v2.1.0

# Expected files:
# - ZPL2PDF-v2.1.0-win-x64.zip
# - ZPL2PDF-v2.1.0-win-x86.zip  
# - ZPL2PDF-v2.1.0-win-arm64.zip
# - ZPL2PDF-v2.1.0-linux-x64.tar.gz
# - ZPL2PDF-v2.1.0-linux-arm64.tar.gz
# - ZPL2PDF-v2.1.0-linux-arm.tar.gz
# - ZPL2PDF-v2.1.0-osx-x64.tar.gz
# - ZPL2PDF-v2.1.0-osx-arm64.tar.gz
# - ZPL2PDF-Setup-2.1.0.exe
# - ZPL2PDF-v2.1.0-linux-amd64.deb
# - ZPL2PDF-v2.1.0-linux-x64-rpm.tar.gz
# - All corresponding .sha256 files
```

#### **Check Docker Images**

```bash
# GitHub Container Registry
docker pull ghcr.io/brunoleocam/zpl2pdf:2.1.0
docker pull ghcr.io/brunoleocam/zpl2pdf:latest

# Docker Hub
docker pull brunoleocam/zpl2pdf:2.1.0
docker pull brunoleocam/zpl2pdf:latest

# Test image
docker run --rm ghcr.io/brunoleocam/zpl2pdf:2.1.0 --help
```

#### **Check WinGet PR**

```bash
# Verify PR created
https://github.com/microsoft/winget-pkgs/pulls?q=is:pr+ZPL2PDF

# Expected PR title: "brunoleocam.ZPL2PDF version 2.1.0"
# Expected status: Open, awaiting review
```

---

## 📦 **Release Artifacts**

### **Platform Builds**

| Platform | File | Size | Purpose |
|----------|------|------|---------|
| **Windows x64** | `ZPL2PDF-v2.1.0-win-x64.zip` | ~45 MB | Windows 64-bit |
| **Windows x86** | `ZPL2PDF-v2.1.0-win-x86.zip` | ~42 MB | Windows 32-bit |
| **Windows ARM64** | `ZPL2PDF-v2.1.0-win-arm64.zip` | ~48 MB | Windows ARM |
| **Linux x64** | `ZPL2PDF-v2.1.0-linux-x64.tar.gz` | ~38 MB | Linux 64-bit |
| **Linux ARM64** | `ZPL2PDF-v2.1.0-linux-arm64.tar.gz` | ~40 MB | Linux ARM64 |
| **Linux ARM** | `ZPL2PDF-v2.1.0-linux-arm.tar.gz` | ~36 MB | Linux ARM |
| **macOS x64** | `ZPL2PDF-v2.1.0-osx-x64.tar.gz` | ~42 MB | macOS Intel |
| **macOS ARM64** | `ZPL2PDF-v2.1.0-osx-arm64.tar.gz` | ~44 MB | macOS Apple Silicon |

### **Installation Packages**

| Package | File | Size | Purpose |
|---------|------|------|---------|
| **Windows Installer** | `ZPL2PDF-Setup-2.1.0.exe` | ~35 MB | Windows installation |
| **Debian Package** | `ZPL2PDF-v2.1.0-linux-amd64.deb` | ~38 MB | Ubuntu/Debian |
| **RPM Tarball** | `ZPL2PDF-v2.1.0-linux-x64-rpm.tar.gz` | ~38 MB | Fedora/CentOS |

### **Docker Images**

| Registry | Tag | Size | Purpose |
|----------|-----|------|---------|
| **GitHub Container** | `ghcr.io/brunoleocam/zpl2pdf:2.1.0` | ~470 MB | Production |
| **GitHub Container** | `ghcr.io/brunoleocam/zpl2pdf:latest` | ~470 MB | Latest stable |
| **Docker Hub** | `brunoleocam/zpl2pdf:2.1.0` | ~470 MB | Production |
| **Docker Hub** | `brunoleocam/zpl2pdf:latest` | ~470 MB | Latest stable |

---

## 🔄 **Release Timeline**

### **Typical Release Timeline**

| Time | Action | Status |
|------|--------|--------|
| **T-1 day** | Prepare release (version bump, docs) | Manual |
| **T-0** | Create Git tag | Manual |
| **T+0** | GitHub Actions triggered | Automated |
| **T+5 min** | Tests complete | Automated |
| **T+20 min** | All builds complete | Automated |
| **T+25 min** | Docker images published | Automated |
| **T+30 min** | GitHub Release created | Automated |
| **T+35 min** | WinGet PR created | Automated |
| **T+1-7 days** | WinGet PR approved | Manual (Microsoft) |

### **Total Automation Time: ~35 minutes**

---

## 🚨 **Rollback Procedure**

### **If Release Has Issues**

#### **Option 1: Create Hotfix Release**

```bash
# Create hotfix branch
git checkout main
git pull origin main
git checkout -b hotfix/critical-fix

# Make minimal fix
git add .
git commit -m "fix: resolve critical issue in v2.1.0"

# Push and create PR
git push origin hotfix/critical-fix

# After merge, create hotfix release
git tag -a v2.1.1 -m "Hotfix release v2.1.1"
git push origin v2.1.1
```

#### **Option 2: Unpublish Release**

```bash
# 1. Delete GitHub Release
# Go to: https://github.com/brunoleocam/ZPL2PDF/releases
# Click on release → "Edit" → "Delete release"

# 2. Delete Git tag
git tag -d v2.1.0
git push origin :refs/tags/v2.1.0

# 3. Revert commits
git revert <commit-hash>
git push origin main

# 4. Close WinGet PR (if created)
# Go to PR and close with explanation
```

---

## 📊 **Release Metrics**

### **Release Success Criteria**

- [ ] ✅ **All tests pass** (100% success rate)
- [ ] ✅ **All platforms build** successfully
- [ ] ✅ **Docker images** published to both registries
- [ ] ✅ **GitHub Release** created with all artifacts
- [ ] ✅ **WinGet PR** created automatically
- [ ] ✅ **Linux packages** built and uploaded
- [ ] ✅ **Installation** works on all platforms
- [ ] ✅ **No critical bugs** reported within 24h

### **Performance Metrics**

| Metric | Target | Current |
|--------|--------|---------|
| **Build Time** | < 30 min | ~25 min ✅ |
| **Test Coverage** | > 85% | 90%+ ✅ |
| **Artifact Size** | < 50 MB | ~45 MB ✅ |
| **Docker Image Size** | < 500 MB | 470 MB ✅ |
| **WinGet Approval** | < 7 days | ~3 days ✅ |

---

## 🔍 **Release Validation**

### **Automated Validation**

GitHub Actions automatically validates:

- ✅ **Code compilation** on all platforms
- ✅ **Unit tests** pass (100% success rate)
- ✅ **Integration tests** pass
- ✅ **Code coverage** meets requirements (>85%)
- ✅ **Security scans** pass
- ✅ **Build artifacts** are created
- ✅ **Checksums** are generated correctly

### **Manual Validation Checklist**

#### **Pre-Release**

- [ ] ✅ **Version numbers** updated in all files
- [ ] ✅ **CHANGELOG.md** updated with changes
- [ ] ✅ **Documentation** updated if needed
- [ ] ✅ **Breaking changes** documented
- [ ] ✅ **All tests pass** locally

#### **Post-Release**

- [ ] ✅ **GitHub Release** accessible
- [ ] ✅ **All artifacts** downloadable
- [ ] ✅ **Docker images** pullable
- [ ] ✅ **WinGet PR** created
- [ ] ✅ **Installation** works on Windows
- [ ] ✅ **Installation** works on Linux
- [ ] ✅ **Installation** works on macOS
- [ ] ✅ **Docker** runs correctly

---

## 🛠️ **Release Tools**

### **Automated Scripts**

| Script | Purpose | Usage |
|--------|---------|-------|
| `scripts/release.ps1` | Windows release automation | `.\scripts\release.ps1 -Version 2.1.0` |
| `scripts/release.sh` | Linux/macOS release automation | `./scripts/release.sh 2.1.0` |
| `scripts/build-all-platforms.ps1` | Build all platforms | `.\scripts\build-all-platforms.ps1` |
| `scripts/build-all-platforms.sh` | Build all platforms | `./scripts/build-all-platforms.sh` |

### **Manual Commands**

```bash
# Build specific platform
dotnet publish -c Release -r win-x64 --self-contained true

# Create archive
Compress-Archive -Path "bin/Release/win-x64/*" -DestinationPath "ZPL2PDF-v2.1.0-win-x64.zip"

# Generate checksum
Get-FileHash ZPL2PDF-v2.1.0-win-x64.zip -Algorithm SHA256

# Build Docker image
docker build -t brunoleocam/zpl2pdf:2.1.0 .

# Build Windows installer
.\installer\build-installer.ps1
```

---

## 📚 **Release Best Practices**

### **Version Management**

1. ✅ **Follow SemVer** strictly
2. ✅ **Update all version files** consistently
3. ✅ **Document breaking changes** clearly
4. ✅ **Use conventional commits** for automation
5. ✅ **Tag releases** immediately after merge

### **Quality Assurance**

1. ✅ **Test on all platforms** before release
2. ✅ **Validate installation** process
3. ✅ **Check Docker images** functionality
4. ✅ **Verify documentation** accuracy
5. ✅ **Monitor for issues** post-release

### **Communication**

1. ✅ **Update CHANGELOG.md** with all changes
2. ✅ **Write clear release notes** for users
3. ✅ **Announce breaking changes** prominently
4. ✅ **Provide migration guides** when needed
5. ✅ **Respond to user feedback** promptly

---

## 🎯 **Next Steps**

1. ✅ **Understand release process** and automation
2. ✅ **Prepare first release** following guidelines
3. ✅ **Test release process** with test version
4. ✅ **Monitor automation** and fix any issues
5. ✅ **Establish release cadence** (monthly/quarterly)

---

**Automated releases ensure consistent, reliable delivery to users across all platforms!** 🚀
