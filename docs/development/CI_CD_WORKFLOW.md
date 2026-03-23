# 🔄 CI/CD Workflow - Complete Automation Flow

## 🎯 **Overview**

This document describes the **complete automated workflow** for ZPL2PDF, from development to publication.

---

## 📊 **Complete Flow Diagram**

```
Developer commits code
        ↓
    GitHub Push
        ↓
    ┌─────────────────────────────────────┐
    │   GitHub Actions CI/CD              │
    │   (Triggered automatically)         │
    └─────────────────────────────────────┘
        ↓
    ┌─────────────────────────────────────┐
    │   Step 1: Run Tests                 │
    │   - Unit tests (all platforms)      │
    │   - Integration tests               │
    │   - Code coverage                   │
    └─────────────────────────────────────┘
        ↓
    Tests Pass? ─── NO ──→ ❌ Stop (notify developer)
        ↓
       YES
        ↓
    ┌─────────────────────────────────────┐
    │   Step 2: Build All Platforms       │
    │   - Windows (x64, x86, ARM64)       │
    │   - Linux (x64, ARM64, ARM)         │
    │   - macOS (x64, ARM64)              │
    │   - Create archives (.zip/.tar.gz)  │
    │   - Generate checksums              │
    └─────────────────────────────────────┘
        ↓
    ┌─────────────────────────────────────┐
    │   Step 3: Build Docker Images       │
    │   - Alpine Linux (multi-arch)       │
    │   - Tag: latest, version, alpine    │
    └─────────────────────────────────────┘
        ↓
    ┌─────────────────────────────────────┐
    │   Step 4: Build Windows Installer   │
    │   - Inno Setup compilation          │
    │   - Multi-language support          │
    │   - Generate checksum               │
    └─────────────────────────────────────┘
        ↓
    Is this a Release Tag? ─── NO ──→ ✅ End (artifacts saved)
        ↓
       YES (v3.1.0, v3.1.0, etc.)
        ↓
    ┌─────────────────────────────────────┐
    │   Step 5: Publish Docker Images     │
    │   - Push to GitHub Container        │
    │     Registry (ghcr.io)              │
    │   - Push to Docker Hub              │
    │   - Multi-architecture support      │
    └─────────────────────────────────────┘
        ↓
    ┌─────────────────────────────────────┐
    │   Step 6: Create GitHub Release     │
    │   - Upload all platform builds      │
    │   - Upload Windows installer        │
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
    ┌─────────────────────────────────────┐
    │   Step 8: Update Package Repos      │
    │   - Debian/Ubuntu (.deb)            │
    │   - Red Hat/CentOS (.rpm)           │
    │   - Homebrew (macOS/Linux)          │
    └─────────────────────────────────────┘
        ↓
    ✅ Complete! Users can download/install
```

---

## 🚀 **Workflow Details**

### **Trigger 1: Push to Main Branch**

```yaml
on:
  push:
    branches: [main]
```

**What happens:**
1. ✅ Run all tests
2. ✅ Build all platforms
3. ✅ Save artifacts (for manual download)
4. ❌ Does NOT publish (development build)

**Purpose:** Continuous Integration - ensure code quality

---

### **Trigger 2: Create Release (v3.1.0, v3.1.0, etc.)**

```yaml
on:
  release:
    types: [published]
```

**What happens:**
1. ✅ Run all tests
2. ✅ Build all platforms
3. ✅ Build Docker images
4. ✅ Build Windows installer
5. ✅ **Publish Docker** → ghcr.io + Docker Hub
6. ✅ **Create GitHub Release** → Upload all builds
7. ✅ **Create WinGet PR** → Automated submission
8. ✅ **Update package repos** → .deb, .rpm, Homebrew

**Purpose:** Continuous Deployment - full automated release

---

### **Trigger 3: Manual Workflow Dispatch**

```yaml
on:
  workflow_dispatch:
```

**What happens:**
- Can be triggered manually from GitHub Actions UI
- Useful for testing or custom builds
- Follows same steps as push/release

---

## 📋 **GitHub Actions Workflows**

### **Current Workflows:**

| Workflow | File | Trigger | Purpose |
|----------|------|---------|---------|
| **Docker Publish** | `.github/workflows/docker-publish.yml` | ✅ Release | Build & publish Docker images |
| **WinGet Publish** | `.github/workflows/winget-publish.yml` | ✅ Release | Submit to microsoft/winget-pkgs |
| **CI Tests** | `.github/workflows/ci.yml` | ⏳ To create | Run tests on every push |
| **Release Build** | `.github/workflows/release.yml` | ⏳ To create | Build all platforms on release |

---

## 🔧 **Step-by-Step Process**

### **Step 1: Developer Workflow**

```bash
# 1. Developer makes changes
git checkout -b feature/new-feature
# ... code changes ...

# 2. Run tests locally
dotnet test

# 3. Commit and push
git add .
git commit -m "feat: add new feature"
git push origin feature/new-feature

# 4. Create Pull Request on GitHub
# → GitHub Actions runs tests automatically
# → Code review
# → Merge to main
```

---

### **Step 2: Automated Testing (On Every Push)**

```yaml
# .github/workflows/ci.yml (TO CREATE)
name: CI Tests

on:
  push:
    branches: [main, develop]
  pull_request:

jobs:
  test:
    strategy:
      matrix:
        os: [windows-latest, ubuntu-latest, macos-latest]
    
    runs-on: ${{ matrix.os }}
    
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal
```

**Result:** ✅ or ❌ Prevents broken code from being merged

---

### **Step 3: Create Release**

```bash
# 1. Update version in files
# - ZPL2PDF.csproj
# - installer/ZPL2PDF-Setup.iss
# - CHANGELOG.md

# 2. Commit version bump
git add .
git commit -m "chore: bump version to 2.1.0"
git push

# 3. Create Git tag
git tag -a v3.1.0 -m "Release version 3.1.0"
git push origin v3.1.0

# 4. Create GitHub Release
# - Go to: https://github.com/brunoleocam/ZPL2PDF/releases/new
# - Select tag: v3.1.0
# - Write release notes
# - Click "Publish release"

# 5. GitHub Actions automatically:
#    ✅ Runs all tests
#    ✅ Builds all platforms
#    ✅ Builds Docker images
#    ✅ Publishes to Docker Hub + GitHub Registry
#    ✅ Uploads artifacts to GitHub Release
#    ✅ Creates WinGet PR
```

---

### **Step 4: Automated Docker Publishing**

When you create a release, Docker workflow automatically:

```yaml
# .github/workflows/docker-publish.yml (ALREADY EXISTS)

1. Log in to GitHub Container Registry (ghcr.io)
   - Uses: secrets.GITHUB_TOKEN (automatic)

2. Log in to Docker Hub
   - Uses: secrets.DOCKERHUB_USERNAME
   - Uses: secrets.DOCKERHUB_TOKEN

3. Build multi-architecture image
   - Platforms: linux/amd64, linux/arm64
   - Base: Alpine Linux (optimized)

4. Tag image with multiple tags:
   - latest
   - 2.1.0 (version)
   - 2.1 (major.minor)
   - 2 (major)
   - alpine (base image)

5. Push to both registries simultaneously:
   - ghcr.io/brunoleocam/zpl2pdf:2.1.0
   - brunoleocam/zpl2pdf:2.1.0
```

**Result:** Users can `docker pull brunoleocam/zpl2pdf:latest`

---

### **Step 5: Automated Windows Installer**

```yaml
# .github/workflows/release.yml (TO CREATE)

1. Build all Windows platforms
   - win-x64, win-x86, win-arm64

2. Extract win-x64 for installer

3. Compile Inno Setup script
   - Uses: installer/ZPL2PDF-Setup.iss
   - Output: ZPL2PDF-Setup-2.1.0.exe

4. Calculate SHA256 checksum

5. Upload to GitHub Release
   - ZPL2PDF-Setup-2.1.0.exe
   - ZPL2PDF-Setup-2.1.0.exe.sha256
```

---

### **Step 6: Automated WinGet Update**

```yaml
# .github/workflows/winget-publish.yml (CREATED)

1. Download installer from GitHub Release
   - URL: github.com/brunoleocam/ZPL2PDF/releases/download/v3.1.0/ZPL2PDF-Setup-3.0.3.exe

2. Calculate SHA256 hash automatically

3. Update manifest files (4 files):
   - brunoleocam.ZPL2PDF.yaml
   - brunoleocam.ZPL2PDF.installer.yaml
   - brunoleocam.ZPL2PDF.locale.en-US.yaml
   - brunoleocam.ZPL2PDF.locale.pt-BR.yaml

4. Fork/update brunoleocam/winget-pkgs

5. Create branch: brunoleocam.ZPL2PDF-2.1.0

6. Copy manifests to: manifests/b/brunoleocam/ZPL2PDF/2.1.0/

7. Create Pull Request to microsoft/winget-pkgs
   - Title: "brunoleocam.ZPL2PDF version 3.1.0"
   - Body: Auto-generated with release notes
```

**Result:** WinGet package updated after PR approval (~1-7 days)

---

## 🎯 **Manual vs Automated**

### **What You Do Manually:**

| Action | Frequency | Time |
|--------|-----------|------|
| Write code | Daily | - |
| Run local tests | Per feature | 1 min |
| Create Git tag | Per release | 30 sec |
| Create GitHub Release | Per release | 2 min |
| **TOTAL PER RELEASE** | - | **~3 minutes** |

### **What GitHub Actions Does Automatically:**

| Action | Time | Status |
|--------|------|--------|
| Run tests (all platforms) | 5 min | ✅ Auto |
| Build 8 platforms | 15 min | ✅ Auto |
| Build Docker images | 5 min | ✅ Auto |
| Publish Docker (2 registries) | 10 min | ✅ Auto |
| Build Windows installer | 2 min | ✅ Auto |
| Upload to GitHub Release | 5 min | ✅ Auto |
| Create WinGet PR | 2 min | ✅ Auto |
| **TOTAL** | **~45 minutes** | **✅ AUTOMATED** |

**You save:** ~40 minutes per release! ✅

---

## 📝 **Versioning Strategy**

### **Semantic Versioning (SemVer):**

```
MAJOR.MINOR.PATCH
  2  .  1  .  3

Major: Breaking changes (2.0.0 → 3.0.0)
Minor: New features (2.0.0 → 2.1.0)
Patch: Bug fixes (2.1.0 → 2.1.1)
```

### **When to Release:**

| Change Type | Version Bump | Example |
|-------------|--------------|---------|
| **Bug fix** | Patch | 2.0.0 → 2.0.1 |
| **New feature** | Minor | 2.0.1 → 2.1.0 |
| **Breaking change** | Major | 2.1.0 → 3.0.0 |

---

## 🔐 **Required Secrets**

Configure these in GitHub Settings → Secrets:

| Secret | Purpose | How to get |
|--------|---------|------------|
| `DOCKERHUB_USERNAME` | Docker Hub login | Your Docker Hub username |
| `DOCKERHUB_TOKEN` | Docker Hub password | https://hub.docker.com/settings/security |
| `GITHUB_TOKEN` | GitHub API | Automatic (provided by GitHub) |

---

## ✅ **Testing the Workflow**

### **Test Before Release:**

```bash
# 1. Create test tag (doesn't trigger release)
git tag -a test-v3.1.0 -m "Test release"
git push origin test-v3.1.0

# 2. Manually trigger workflow
# Go to: https://github.com/brunoleocam/ZPL2PDF/actions
# Select: "Docker Build and Publish"
# Click: "Run workflow"
```

### **Validate After Release:**

```bash
# 1. Check Docker Hub
docker pull brunoleocam/zpl2pdf:2.0.0

# 2. Check GitHub Container Registry  
docker pull ghcr.io/brunoleocam/zpl2pdf:2.0.0

# 3. Check GitHub Release
# https://github.com/brunoleocam/ZPL2PDF/releases

# 4. Check WinGet PR
# https://github.com/microsoft/winget-pkgs/pulls?q=is:pr+ZPL2PDF
```

---

## 🐛 **Troubleshooting**

### **Tests Fail:**
- Check logs in GitHub Actions
- Fix issues locally
- Push fix and retry

### **Docker Build Fails:**
- Check Dockerfile syntax
- Verify secrets are configured
- Check Docker Hub quota

### **Installer Build Fails:**
- Verify build artifacts exist
- Check Inno Setup script
- Validate file paths

### **WinGet PR Fails:**
- Check manifest syntax
- Verify SHA256 matches
- Ensure version format is correct

---

## 📚 **Files Involved in CI/CD**

### **Configuration Files:**

```
.github/workflows/
├── docker-publish.yml     ✅ EXISTS (Docker automation)
├── winget-publish.yml     ✅ EXISTS (WinGet automation)
├── ci.yml                 ⏳ TO CREATE (Test automation)
└── release.yml            ⏳ TO CREATE (Build automation)

scripts/
├── build-all-platforms.ps1  ✅ EXISTS
├── build-all-platforms.sh   ✅ EXISTS
├── release.ps1              ✅ EXISTS
└── release.sh               ✅ EXISTS

installer/
├── ZPL2PDF-Setup.iss        ✅ EXISTS
└── build-installer.ps1      ✅ EXISTS

Dockerfile                   ✅ EXISTS (Alpine optimized)
docker-compose.yml           ✅ EXISTS
winget-manifest.yaml         ⏳ TO VALIDATE
```

---

## 🎯 **Next Steps to Complete Automation**

1. ✅ **Docker Publish** - Already configured
2. ✅ **WinGet Publish** - Already configured
3. ⏳ **CI Tests** - Create `.github/workflows/ci.yml`
4. ⏳ **Release Build** - Create `.github/workflows/release.yml`

---

## 🌍 **Distribution Channels**

After automated workflow, ZPL2PDF is available on:

| Channel | URL | Install Command |
|---------|-----|-----------------|
| **GitHub Releases** | [Releases](https://github.com/brunoleocam/ZPL2PDF/releases) | Manual download |
| **GitHub Container** | ghcr.io | `docker pull ghcr.io/brunoleocam/zpl2pdf` |
| **Docker Hub** | [Docker Hub](https://hub.docker.com/r/brunoleocam/zpl2pdf) | `docker pull brunoleocam/zpl2pdf` |
| **WinGet** | Microsoft Store | `winget install brunoleocam.ZPL2PDF` |
| **APT** (future) | PPA | `apt install zpl2pdf` |
| **YUM** (future) | RPM repo | `yum install zpl2pdf` |
| **Homebrew** (future) | Brew tap | `brew install zpl2pdf` |

---

## ✅ **Benefits of This Workflow**

1. ✅ **Consistency** - Same build process every time
2. ✅ **Quality** - Tests before every release
3. ✅ **Speed** - 45 minutes automated vs hours manual
4. ✅ **Reliability** - No human error
5. ✅ **Multi-platform** - All platforms built simultaneously
6. ✅ **Documentation** - Release notes automated
7. ✅ **Distribution** - Multiple channels updated

---

**This is the industry-standard CI/CD approach used by professional projects!** 🚀
