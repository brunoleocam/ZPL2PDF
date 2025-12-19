# 🚀 Complete Release Guide - ZPL2PDF

This guide documents the complete step-by-step process to generate a full ZPL2PDF release.

---

## 📋 Overview

The release process is divided into **16 steps** that can be executed individually or through the main script. Each step has its own script to allow manual execution in case of failure.

---

## 🎯 Prerequisites

Before starting the release process, make sure you have:

### Required Tools
- ✅ .NET SDK 9.0 or higher
- ✅ Git installed and configured
- ✅ PowerShell 7+ (Windows) or Bash (Linux/macOS)
- ✅ Docker Desktop installed and running (for Linux packages and Docker images)
- ✅ Inno Setup 6 installed (for Windows installer)
- ✅ GitHub CLI (`gh`) installed and authenticated

### Accounts and Authentication
- ✅ GitHub account with repository access
- ✅ Docker Hub account (to publish images)
- ✅ GitHub CLI authenticated: `gh auth login`
- ✅ Docker Hub authenticated: `docker login`
- ✅ GitHub token with `write:packages` scope (for GHCR)

### Quick Verification
```powershell
# Verify tools
dotnet --version
git --version
docker --version
gh --version

# Verify authentication
gh auth status
docker info
```

---

## 📂 Script Structure

```
release/
├── README.md                    # This file
├── release-main.ps1             # Main script (orchestrates all)
├── _checkpoint-utils.ps1        # Checkpoint utilities
├── show-checkpoint.ps1          # Show checkpoint status
│
├── 01-update-docs.ps1            # Update documentation
├── 02-update-resources.ps1       # Update Resources Messages
├── 03-update-i18n.ps1            # Update i18n files
├── 04-update-resource-keys.ps1   # Update Localization.ResourceKeys
├── 05-update-changelog.ps1       # Update CHANGELOG.md
├── 06-update-dockerfile.ps1      # Update Dockerfile
├── 07-build-all-platforms.ps1    # Build all platforms
├── 08-build-installer.ps1        # Build Windows installer
├── 09-build-linux-packages.ps1   # Build .deb and .rpm packages
├── 10-update-manifests.ps1       # Update WinGet manifests
├── 11-build-docker-images.ps1    # Build Docker images
├── 12-create-version-branch.ps1  # Create version branch and PR
├── 13-create-github-release.ps1  # Create/update GitHub release
├── 14-publish-dockerhub.ps1      # Publish to Docker Hub
├── 15-publish-ghcr.ps1           # Publish to GitHub Package Registry
└── 16-submit-winget-pr.ps1       # Update WinGet fork and create PR
```

---

## 🚀 Quick Usage

### Run Complete Release

```powershell
# Run all steps in sequence
.\release\release-main.ps1 -Version "3.1.0"

# Run with dry-run (test without publishing)
.\release\release-main.ps1 -Version "3.1.0" -DryRun

# Skip specific steps
.\release\release-main.ps1 -Version "3.1.0" -SkipDocker -SkipWinGet

# Resume interrupted release (skips already completed steps)
.\release\release-main.ps1 -Version "3.1.0" -Resume

# Continue from a specific step
.\release\release-main.ps1 -Version "3.1.0" -StartFromStep 8
```

### Run Individual Step

```powershell
# Run only a specific step
.\release\01-update-docs.ps1 -Version "3.1.0"
.\release\07-build-all-platforms.ps1 -Version "3.1.0"
```

### Check Checkpoint Status

```powershell
# View current checkpoint status
.\release\show-checkpoint.ps1 -Version "3.1.0"
```

---

## 💾 Checkpoint System

The release system uses a checkpoint file to track progress and allow resuming from where it stopped.

### Checkpoint File

When you start a release, a `.release-checkpoint-{Version}.json` file is created at the project root:

```json
{
  "Version": "3.1.0",
  "StartedAt": "2025-12-20 10:30:00",
  "LastUpdated": "2025-12-20 11:45:00",
  "CompletedSteps": [1, 2, 3, 4, 5, 6, 7],
  "FailedSteps": [],
  "SkippedSteps": [9, 11],
  "Data": {}
}
```

### Features

- ✅ **Automatic tracking**: Each step saves its status upon completion
- ✅ **Resume release**: Use `-Resume` to skip already completed steps
- ✅ **Failure history**: Failed steps are recorded
- ✅ **Persistent data**: Important information is saved in the checkpoint

### Usage Examples

```powershell
# Start release (creates checkpoint automatically)
.\release\release-main.ps1 -Version "3.1.0"

# If something fails, resume from where it stopped
.\release\release-main.ps1 -Version "3.1.0" -Resume

# Continue from a specific step
.\release\release-main.ps1 -Version "3.1.0" -StartFromStep 8

# View checkpoint status (via utility script)
# Status is shown automatically when using -Resume
```

### Checkpoint Location

The checkpoint file is saved at the project root:
```
.release-checkpoint-3.1.0.json
```

### Clear Checkpoint

To clear the checkpoint and start from scratch:

```powershell
# Checkpoint is kept after release for history
# To clear manually, delete the file:
Remove-Item .release-checkpoint-3.1.0.json
```

---

## 📝 Detailed Steps

### Step 1: Update Documentation
**Script:** `01-update-docs.ps1`

**What it does:**
- Updates version in `docs/README.md`
- Updates version in other relevant documentation files
- Updates version references in guides

**Modified files:**
- `docs/README.md`
- Other `.md` files that reference version

**Execution:**
```powershell
.\release\01-update-docs.ps1 -Version "3.1.0"
```

---

### Step 2: Update Resources Messages
**Script:** `02-update-resources.ps1`

**What it does:**
- Updates version in all `Resources/Messages.*.resx` files
- Maintains consistency across all languages

**Modified files:**
- `Resources/Messages.en.resx`
- `Resources/Messages.pt-BR.resx`
- `Resources/Messages.es.resx`
- `Resources/Messages.fr.resx`
- `Resources/Messages.de.resx`
- `Resources/Messages.it.resx`
- `Resources/Messages.ja.resx`
- `Resources/Messages.zh.resx`

**Execution:**
```powershell
.\release\02-update-resources.ps1 -Version "3.1.0"
```

---

### Step 3: Update i18n Files
**Script:** `03-update-i18n.ps1`

**What it does:**
- Updates version in internationalization files
- Updates language documentation

**Modified files:**
- Files in `docs/i18n/`

**Execution:**
```powershell
.\release\03-update-i18n.ps1 -Version "3.1.0"
```

---

### Step 4: Update Localization.ResourceKeys
**Script:** `04-update-resource-keys.ps1`

**What it does:**
- Checks and updates `src/Shared/Localization/ResourceKeys.cs` if necessary
- Ensures all keys are synchronized

**Modified files:**
- `src/Shared/Localization/ResourceKeys.cs`

**Execution:**
```powershell
.\release\04-update-resource-keys.ps1 -Version "3.1.0"
```

---

### Step 5: Update CHANGELOG
**Script:** `05-update-changelog.ps1`

**What it does:**
- Updates `CHANGELOG.md` with the new version
- Adds release date
- Moves items from `[Unreleased]` to the new version

**Modified files:**
- `CHANGELOG.md`

**Execution:**
```powershell
.\release\05-update-changelog.ps1 -Version "3.1.0"
```

---

### Step 6: Update Dockerfile
**Script:** `06-update-dockerfile.ps1`

**What it does:**
- Updates version in `Dockerfile` (LABEL version)
- Updates version references if any

**Modified files:**
- `Dockerfile`

**Execution:**
```powershell
.\release\06-update-dockerfile.ps1 -Version "3.1.0"
```

---

### Step 7: Build All Platforms
**Script:** `07-build-all-platforms.ps1`

**What it does:**
- Builds for 8 platforms:
  - Windows: x64, x86, ARM64
  - Linux: x64, ARM64, ARM
  - macOS: x64, ARM64
- Creates compressed files (.zip for Windows, .tar.gz for Linux/macOS)
- Generates SHA256 checksums

**Generated files:**
- `Assets/ZPL2PDF-v{Version}-win-x64.zip`
- `Assets/ZPL2PDF-v{Version}-win-x86.zip`
- `Assets/ZPL2PDF-v{Version}-win-arm64.zip`
- `Assets/ZPL2PDF-v{Version}-linux-x64.tar.gz`
- `Assets/ZPL2PDF-v{Version}-linux-arm64.tar.gz`
- `Assets/ZPL2PDF-v{Version}-linux-arm.tar.gz`
- `Assets/ZPL2PDF-v{Version}-osx-x64.tar.gz`
- `Assets/ZPL2PDF-v{Version}-osx-arm64.tar.gz`
- `Assets/SHA256SUMS.txt`

**Execution:**
```powershell
.\release\07-build-all-platforms.ps1 -Version "3.1.0" -SkipTests
```

---

### Step 8: Build Windows Installer
**Script:** `08-build-installer.ps1`

**What it does:**
- Compiles installer using Inno Setup
- Generates `ZPL2PDF-Setup-{Version}.exe`
- Supports multiple languages

**Generated files:**
- `installer/Output/ZPL2PDF-Setup-{Version}.exe` (also copied to `Assets/`)
- `Assets/ZPL2PDF-Setup-{Version}.exe`

**Execution:**
```powershell
.\release\08-build-installer.ps1 -Version "3.1.0"
```

---

### Step 9: Build Linux Packages (.deb and .rpm)
**Script:** `09-build-linux-packages.ps1`

**What it does:**
- Generates `.deb` package (Debian/Ubuntu)
- Generates `.rpm` package (Red Hat/CentOS)
- Uses Docker to ensure compatibility

**Generated files:**
- `Assets/ZPL2PDF-v{Version}-linux-amd64.deb`
- `Assets/ZPL2PDF-v{Version}-linux-x64-rpm.tar.gz`

**Execution:**
```powershell
.\release\09-build-linux-packages.ps1 -Version "3.1.0"
```

---

### Step 10: Update WinGet Manifests
**Script:** `10-update-manifests.ps1`

**What it does:**
- Updates version in all WinGet manifests
- Calculates installer SHA256
- Updates URLs and dates

**Modified files:**
- `manifests/brunoleocam.ZPL2PDF.yaml`
- `manifests/brunoleocam.ZPL2PDF.installer.yaml`
- `manifests/brunoleocam.ZPL2PDF.locale.*.yaml`

**Execution:**
```powershell
.\release\10-update-manifests.ps1 -Version "3.1.0"
```

---

### Step 11: Build Docker Images
**Script:** `11-build-docker-images.ps1`

**What it does:**
- Builds Docker image (Alpine Linux)
- Creates tags: `latest`, `{version}`, `{major}.{minor}`, `{major}`, `alpine`
- Prepares for publication

**Generated files:**
- Local Docker images (not published yet)

**Execution:**
```powershell
.\release\11-build-docker-images.ps1 -Version "3.1.0"
```

---

### Step 12: Create Version Branch and PR
**Script:** `12-create-version-branch.ps1`

**What it does:**
- Creates branch `release/v{Version}`
- Commits all changes
- Creates Pull Request to `main`
- **Does NOT auto-merge** (requires manual approval)

**Execution:**
```powershell
.\release\12-create-version-branch.ps1 -Version "3.1.0"
```

---

### Step 13: Create/Update GitHub Release
**Script:** `13-create-github-release.ps1`

**What it does:**
- Creates Git tag `v{Version}`
- Creates GitHub release
- Generates source code archives (ZIP and TAR.GZ)
- Uploads all generated files from `Assets/`:
  - Builds for all platforms
  - Windows installer
  - Linux packages (.deb and .rpm)
  - Source code archives
  - Checksums (SHA256SUMS.txt)

**Prerequisites:**
- Git tag must exist or will be created
- GitHub CLI authenticated

**Execution:**
```powershell
.\release\13-create-github-release.ps1 -Version "3.1.0"
```

---

### Step 14: Publish to Docker Hub
**Script:** `14-publish-dockerhub.ps1`

**What it does:**
- Pushes Docker images to Docker Hub
- Publishes tags: `latest`, `{version}`, `{major}.{minor}`, `{major}`, `alpine`

**Prerequisites:**
- Docker Hub authenticated: `docker login`
- Docker images already built (Step 11)

**Execution:**
```powershell
.\release\14-publish-dockerhub.ps1 -Version "3.1.0"
```

---

### Step 15: Publish to GitHub Package Registry
**Script:** `15-publish-ghcr.ps1`

**What it does:**
- Pushes Docker images to GHCR (ghcr.io)
- Publishes tags: `latest`, `{version}`, `{major}.{minor}`, `{major}`, `alpine`

**Prerequisites:**
- GitHub CLI authenticated with `write:packages` scope
- Docker images already built (Step 11)

**Execution:**
```powershell
.\release\15-publish-ghcr.ps1 -Version "3.1.0"
```

---

### Step 16: Submit WinGet PR
**Script:** `16-submit-winget-pr.ps1`

**What it does:**
- Updates fork of `microsoft/winget-pkgs` repository
- Creates branch with new version
- Copies updated manifests
- Creates Pull Request to `microsoft/winget-pkgs`

**Prerequisites:**
- GitHub CLI authenticated
- Fork of `microsoft/winget-pkgs` exists
- Manifests updated (Step 10)
- GitHub release created (Step 13)

**Execution:**
```powershell
.\release\16-submit-winget-pr.ps1 -Version "3.1.0"
```

---

## 🔄 Complete Release Flow

### Recommended Order

1. **Preparation** (Steps 1-6)
   - Update documentation
   - Update resources and i18n
   - Update CHANGELOG
   - Update Dockerfile

2. **Build** (Steps 7-9)
   - Build all platforms
   - Build Windows installer
   - Build Linux packages

3. **Publication Preparation** (Steps 10-11)
   - Update manifests
   - Build Docker images

4. **Git and GitHub** (Steps 12-13)
   - Create version branch and PR
   - Create GitHub release

5. **Publication** (Steps 14-16)
   - Publish to Docker Hub
   - Publish to GHCR
   - Submit WinGet PR

### Run Everything

```powershell
# Run complete release
.\release\release-main.ps1 -Version "3.1.0"
```

---

## 🛠️ Common Parameters

All scripts accept the following parameters:

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `-Version` | string | Version to release (required) | - |
| `-DryRun` | switch | Runs without making permanent changes | $false |
| `-SkipTests` | switch | Skips test execution | $false |
| `-SkipDocker` | switch | Skips Docker-related steps | $false |
| `-SkipWinGet` | switch | Skips WinGet submission | $false |
| `-SkipGitHubRelease` | switch | Skips GitHub release creation | $false |
| `-Resume` | switch | Resumes release skipping already completed steps | $false |
| `-StartFromStep` | int | Starts from a specific step (1-16) | 1 |

---

## 🐛 Troubleshooting

### Error: "Tool not found"
**Solution:** Install the missing tool or use `-Skip*` parameters to skip steps

### Error: "Not authenticated"
**Solution:**
- GitHub CLI: `gh auth login`
- Docker Hub: `docker login`

### Error: "Build failed"
**Solution:**
- Check if code compiles: `dotnet build`
- Run tests: `dotnet test`
- Check error logs
- Use `-Resume` to resume from where it stopped

### Error: "Docker is not running"
**Solution:** Start Docker Desktop

### Error: "Release already exists"
**Solution:**
- Use a different version
- Or delete the existing release on GitHub

### Interrupted Release
**Solution:**
```powershell
# Check checkpoint status
# (shown automatically when using -Resume)

# Resume from where it stopped
.\release\release-main.ps1 -Version "3.1.0" -Resume

# Or continue from a specific step
.\release\release-main.ps1 -Version "3.1.0" -StartFromStep 8
```

### Corrupted Checkpoint
**Solution:**
```powershell
# Delete checkpoint and start from scratch
Remove-Item .release-checkpoint-3.1.0.json
.\release\release-main.ps1 -Version "3.1.0"
```

---

## 📊 Release Checklist

Before starting, verify:

- [ ] Version defined (e.g., 3.1.0)
- [ ] CHANGELOG.md updated with changes
- [ ] Code tested and working
- [ ] All tools installed
- [ ] All accounts authenticated
- [ ] Docker Desktop running
- [ ] `main` branch updated
- [ ] No uncommitted changes

During execution:

- [ ] Steps 1-6 completed (preparation)
- [ ] Builds generated correctly (Step 7)
- [ ] Installer generated (Step 8)
- [ ] Linux packages generated (Step 9)
- [ ] Manifests updated (Step 10)
- [ ] Docker images built (Step 11)
- [ ] PR created on GitHub (Step 12)
- [ ] Release created on GitHub (Step 13)
- [ ] Docker Hub published (Step 14)
- [ ] GHCR published (Step 15)
- [ ] WinGet PR submitted (Step 16)

After release:

- [ ] PR approved and merged to `main`
- [ ] Release available on GitHub
- [ ] Docker images available
- [ ] WinGet PR approved (may take days)
- [ ] Announce release

---

## 📚 Related Documentation

- [Git Workflow Guide](../docs/development/GIT_WORKFLOW_GUIDE.md)
- [CI/CD Guide](../docs/development/CI_CD_WORKFLOW.md)
- [WinGet Guide](../docs/development/WINGET_GUIDE.md)
- [Docker Guide](../docs/development/DOCKER_PUBLISH_GUIDE.md)
- [Installer Guide](../docs/guides/INNO_SETUP_GUIDE.md)
- [Linux Packages Guide](../docs/guides/LINUX_PACKAGES.md)

---

## ✅ Best Practices

1. **Always test with `-DryRun` first**
2. **Run steps individually if there's a failure**
3. **Verify each step before proceeding**
4. **Keep logs of each release**
5. **Document issues found**
6. **Use semantic versioning (SemVer)**

---

**Last updated:** December 2025  
**Guide version:** 1.0.0
