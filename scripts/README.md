# 🛠️ ZPL2PDF Build Scripts

This directory contains automation scripts for building, packaging, and releasing ZPL2PDF across multiple platforms.

> **📦 Para releases completas, use os scripts na pasta [`release/`](../release/README.md)**  
> Os scripts desta pasta são utilitários de build individuais.

---

## 📋 Available Scripts

### 1️⃣ **`build-all-platforms.ps1` / `build-all-platforms.sh`**

**Purpose:** Build ZPL2PDF for all supported platforms (8 platforms total)

**Platforms:**
- Windows: x64, x86, ARM64
- Linux: x64, ARM64, ARM
- macOS: x64 (Intel), ARM64 (Apple Silicon)

**Usage:**

```powershell
# Windows PowerShell
.\scripts\build-all-platforms.ps1

# Skip tests (faster)
.\scripts\build-all-platforms.ps1 -SkipTests

# Custom output directory
.\scripts\build-all-platforms.ps1 -OutputDir "dist"
```

```bash
# Linux/macOS
./scripts/build-all-platforms.sh

# Skip tests
./scripts/build-all-platforms.sh --skip-tests

# Custom output directory
./scripts/build-all-platforms.sh --output dist
```

**Output:**
- Compiled binaries for all platforms in `build/publish/`
- Compressed archives (.zip for Windows, .tar.gz for Linux/macOS)
- SHA256 checksums file (`SHA256SUMS.txt`)

**Features:**
- ✅ Parallel-ready (builds sequentially but designed for parallel execution)
- ✅ Self-contained builds (no .NET runtime required)
- ✅ Single-file executables
- ✅ Automatic archive creation
- ✅ Checksum generation
- ✅ Progress feedback with colored output
- ✅ Error handling and reporting

---

### 2️⃣ **`build-installer.ps1`**

**Purpose:** Build Windows installer using Inno Setup

**Prerequisites:**
- Inno Setup 6 installed
- Windows only
- Project must be built first

**Usage:**

```powershell
# Build installer with default settings
.\scripts\build-installer.ps1

# Build with specific version
.\scripts\build-installer.ps1 -Version "2.1.0"

# Custom Inno Setup path
.\scripts\build-installer.ps1 -InnoSetupPath "C:\InnoSetup\ISCC.exe"

# Show help
.\scripts\build-installer.ps1 -Help
```

**Output:**
- Windows installer (.exe) in `install/output/`
- Multi-language support (EN, PT-BR, ES, FR, DE, IT)
- Automatic environment variable configuration
- Start menu shortcuts
- Uninstaller

**Features:**
- ✅ Multi-language installer
- ✅ Automatic language detection
- ✅ Sets `ZPL2PDF_LANGUAGE` environment variable
- ✅ Creates start menu shortcuts
- ✅ Adds to Windows PATH (optional)
- ✅ Silent install support

---

### 3️⃣ **`winget-submit.ps1`**

**Purpose:** Automate WinGet package submission to microsoft/winget-pkgs

**Prerequisites:**
- GitHub CLI (`gh`) installed and authenticated
- Windows only (PowerShell)
- Installer must be built and published to GitHub Release

**Usage:**

```powershell
# Automatic (downloads installer from release)
.\scripts\winget-submit.ps1 -Version "2.0.0"

# Custom installer path
.\scripts\winget-submit.ps1 -Version "2.0.0" -InstallerPath "installer\ZPL2PDF-Setup-2.0.0.exe"

# Dry run (test without submitting)
.\scripts\winget-submit.ps1 -Version "2.0.0" -DryRun

# Skip validation
.\scripts\winget-submit.ps1 -Version "2.0.0" -SkipValidation
```

**What it does:**
1. ✅ Validates prerequisites (Git, GitHub CLI)
2. ✅ Locates installer and calculates SHA256
3. ✅ Extracts Product Code from Inno Setup script
4. ✅ Updates 4 manifest files (version, installer, locales)
5. ✅ Validates manifests with `winget validate`
6. ✅ Forks/updates brunoleocam/winget-pkgs
7. ✅ Creates new branch: `brunoleocam.ZPL2PDF-2.0.0`
8. ✅ Commits and pushes changes
9. ✅ Creates Pull Request to microsoft/winget-pkgs

**Output:**
- Updated manifests in `manifests/`
- Pull Request to microsoft/winget-pkgs
- Users can install via: `winget install brunoleocam.ZPL2PDF`

**Features:**
- ✅ Automatic SHA256 calculation
- ✅ Product Code extraction
- ✅ Manifest validation
- ✅ Fork management
- ✅ Automated PR creation
- ✅ Detailed progress feedback

---

### 4️⃣ **`full-release.ps1`** ⭐ RECOMENDADO

**Purpose:** Script completo de release que automatiza TODO o processo de deploy

**Prerequisites:**
- .NET SDK 9.0+
- Docker Desktop (para pacotes Linux e imagens Docker)
- GitHub CLI (`gh`) autenticado
- Inno Setup 6 (para instalador Windows)

**Usage:**

```powershell
# Release completo
.\scripts\full-release.ps1 -Version "2.1.0"

# Dry run (testar sem publicar)
.\scripts\full-release.ps1 -Version "2.1.0" -DryRun

# Pular etapas específicas
.\scripts\full-release.ps1 -Version "2.1.0" -SkipTests
.\scripts\full-release.ps1 -Version "2.1.0" -SkipDocker
.\scripts\full-release.ps1 -Version "2.1.0" -SkipWinGet
.\scripts\full-release.ps1 -Version "2.1.0" -SkipGitHubRelease
```

**O que faz (12 etapas):**
1. ✅ Verifica pré-requisitos (git, dotnet, docker, gh)
2. ✅ Atualiza versão em TODOS os arquivos do projeto
3. ✅ Gera builds para 8 plataformas (Windows, Linux, macOS)
4. ✅ Gera pacotes Linux (.deb e .rpm) via Docker
5. ✅ Gera instalador Windows (Inno Setup)
6. ✅ Gera checksums SHA256
7. ✅ Build e push de imagens Docker (Docker Hub + GHCR)
8. ✅ Cria release no GitHub com todos os assets
9. ✅ Atualiza manifests do WinGet
10. ✅ Submete PR para microsoft/winget-pkgs
11. ✅ Commita alterações no repositório
12. ✅ Exibe resumo final

**Output:**
- Builds para todas as plataformas em `build/publish/`
- Instalador Windows
- Pacotes .deb e .rpm
- Imagens Docker publicadas
- Release no GitHub
- PR no WinGet

---

### 5️⃣ **`release.ps1` / `release.sh`**

**Purpose:** Release básico (versão + build + git tag) - Use `full-release.ps1` para deploy completo

**Usage:**

```powershell
# Windows PowerShell - Full release
.\scripts\release.ps1 -Version "2.1.0"

# Pre-release
.\scripts\release.ps1 -Version "2.1.0-rc1" -PreRelease $true

# Dry run (test without pushing)
.\scripts\release.ps1 -Version "2.1.0" -DryRun $true
```

```bash
# Linux/macOS - Full release
./scripts/release.sh -v 2.1.0

# Pre-release
./scripts/release.sh -v 2.1.0-rc1 -p

# Dry run
./scripts/release.sh -v 2.1.0 -d
```

**What it does:**
1. ✅ Validates prerequisites (git, dotnet)
2. ✅ Checks working directory is clean
3. ✅ Updates version in all files:
   - `ZPL2PDF.csproj`
   - `src/Shared/Constants/ApplicationConstants.cs`
   - `winget-manifest.yaml`
   - `rpm/zpl2pdf.spec`
   - `debian/control`
   - `CHANGELOG.md`
4. ✅ Runs tests
5. ✅ Builds all platforms (calls `build-all-platforms.*`)
6. ✅ Builds Windows installer (calls `build-installer.ps1`)
7. ✅ Creates git tag
8. ✅ Pushes to GitHub (unless dry-run)
9. ✅ Provides GitHub release instructions

**Output:**
- All platform builds in `build/publish/`
- Windows installer in `build/publish/`
- Git tag `v2.1.0`
- Checksums in `SHA256SUMS.txt`

---

## 🎯 Typical Workflows

### Quick Build (Development)

Just want to build for your current platform?

```powershell
# Windows
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained true
```

### Build All Platforms

Want to test on multiple platforms?

```powershell
# Windows
.\scripts\build-all-platforms.ps1 -SkipTests
```

```bash
# Linux/macOS
./scripts/build-all-platforms.sh --skip-tests
```

### Create Release (Recomendado) ⭐

Ready to publish a new version? Use os scripts da pasta `release/`:

```powershell
# Windows - Test first with dry-run
.\release\release-main.ps1 -Version "3.1.0" -DryRun

# Windows - Actual release (faz TUDO automaticamente)
.\release\release-main.ps1 -Version "3.1.0"

# Pular etapas se necessário
.\release\release-main.ps1 -Version "3.1.0" -SkipDocker -SkipWinGet

# Executar etapa específica
.\release\07-build-all-platforms.ps1 -Version "3.1.0"
```

**📚 Veja o [Guia Completo de Release](../release/README.md) para mais detalhes.**

### Create Release (Básico)

Se preferir o script básico (sem Docker/WinGet):

```powershell
# Windows - Test first with dry-run
.\scripts\release.ps1 -Version "2.1.0" -DryRun $true

# Windows - Actual release
.\scripts\release.ps1 -Version "2.1.0"
```

```bash
# Linux/macOS - Test first
./scripts/release.sh -v 2.1.0 -d

# Linux/macOS - Actual release
./scripts/release.sh -v 2.1.0
```

---

## 📊 Script Comparison

| Feature | full-release ⭐ | build-all-platforms | build-installer | winget-submit | release |
|---------|----------------|---------------------|-----------------|---------------|---------|
| **Build all platforms** | ✅ | ✅ | ❌ | ❌ | ✅ |
| **Create archives** | ✅ | ✅ | ❌ | ❌ | ✅ |
| **Windows installer** | ✅ | ❌ | ✅ | ❌ | ✅ |
| **Linux packages (.deb/.rpm)** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Docker build & push** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **GitHub Release** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Update versions** | ✅ | ❌ | ❌ | ❌ | ✅ |
| **Run tests** | ✅ (opcional) | ✅ (opcional) | ❌ | ❌ | ✅ |
| **Git operations** | ✅ | ❌ | ❌ | ✅ (fork/PR) | ✅ |
| **Checksums** | ✅ | ✅ | ❌ | ✅ | ✅ |
| **WinGet submission** | ✅ | ❌ | ❌ | ✅ | ❌ |
| **GHCR push** | ✅ | ❌ | ❌ | ❌ | ❌ |

---

## 🔧 Prerequisites

### All Scripts:
- .NET SDK 9.0 or later
- Git (for release scripts)

### Windows Specific:
- PowerShell 7+ (for colored output)
- Inno Setup 6 (for installer creation)
- GitHub CLI (`gh`) (for WinGet submission)

### Linux/macOS Specific:
- Bash 4+
- zip/tar (usually pre-installed)
- sha256sum or shasum (usually pre-installed)

---

## 📝 Output Structure

After running `build-all-platforms.*`:

```
build/publish/
├── win-x64/
│   └── ZPL2PDF.exe
├── win-x86/
│   └── ZPL2PDF.exe
├── linux-x64/
│   └── ZPL2PDF
├── linux-arm64/
│   └── ZPL2PDF
├── osx-x64/
│   └── ZPL2PDF
├── osx-arm64/
│   └── ZPL2PDF
├── ZPL2PDF-v2.0.0-win-x64.zip
├── ZPL2PDF-v2.0.0-win-x86.zip
├── ZPL2PDF-v2.0.0-linux-x64.tar.gz
├── ZPL2PDF-v2.0.0-linux-arm64.tar.gz
├── ZPL2PDF-v2.0.0-osx-x64.tar.gz
├── ZPL2PDF-v2.0.0-osx-arm64.tar.gz
└── SHA256SUMS.txt
```

---

## 🐛 Troubleshooting

### Error: "Inno Setup not found"
**Solution:** Install Inno Setup 6 from https://jrsoftware.org/isinfo.php or specify path with `-InnoSetupPath`

### Error: "tar not available"
**Solution:** Install Git for Windows (includes tar) or use WSL

### Error: "Working directory not clean"
**Solution:** Commit or stash your changes before running release script

### Error: "Tests failed"
**Solution:** Fix failing tests or use `-SkipTests` flag (not recommended for releases)

### PowerShell Execution Policy Error
**Solution:** Run with `-ExecutionPolicy Bypass`:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build-all-platforms.ps1
```

---

## 🚀 CI/CD Integration

These scripts are designed to work with GitHub Actions, Azure Pipelines, and other CI/CD systems.

**Example GitHub Actions usage:**

```yaml
- name: Build All Platforms
  run: pwsh scripts/build-all-platforms.ps1 -SkipTests
  
- name: Upload Artifacts
  uses: actions/upload-artifact@v3
  with:
    name: builds
    path: build/publish/
```

---

## 📚 Additional Resources

- [../README.md](../README.md) - Project documentation
- [../release/README.md](../release/README.md) - **Guia completo de release** ⭐
- [../CONTRIBUTING.md](../CONTRIBUTING.md) - Contribution guidelines
- [../CHANGELOG.md](../CHANGELOG.md) - Version history
- [../docs/development/WINGET_GUIDE.md](../docs/development/WINGET_GUIDE.md) - WinGet packaging guide
- [../docs/guides/LANGUAGE_CONFIGURATION.md](../docs/guides/LANGUAGE_CONFIGURATION.md) - Language system

---

## 🤝 Contributing

To improve these scripts:
1. Test thoroughly on your platform
2. Update this README with any changes
3. Ensure cross-platform compatibility
4. Add error handling for edge cases

---

**Last Updated:** December 2025  
**Version:** 2.0.1
