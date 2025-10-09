# 🔨 Build Process Guide

Complete guide to building ZPL2PDF for all platforms and deployment scenarios.

---

## 🎯 **Overview**

ZPL2PDF supports building for **8 platforms** with multiple distribution formats:

| Platform | Architecture | Output | Distribution |
|----------|-------------|--------|--------------|
| **Windows** | x64 | `ZPL2PDF.exe` | GitHub Releases |
| **Windows** | x86 | `ZPL2PDF.exe` | GitHub Releases |
| **Windows** | ARM64 | `ZPL2PDF.exe` | GitHub Releases |
| **Linux** | x64 | `ZPL2PDF` | GitHub Releases |
| **Linux** | ARM64 | `ZPL2PDF` | GitHub Releases |
| **Linux** | ARM | `ZPL2PDF` | GitHub Releases |
| **macOS** | x64 | `ZPL2PDF` | GitHub Releases |
| **macOS** | ARM64 | `ZPL2PDF` | GitHub Releases |

---

## 🚀 **Quick Build (All Platforms)**

### **Windows (PowerShell)**
```powershell
# Build all platforms
.\scripts\build-all-platforms.ps1

# Build specific platform
.\scripts\build-all-platforms.ps1 -Platform win-x64
```

### **Linux/macOS (Bash)**
```bash
# Build all platforms
./scripts/build-all-platforms.sh

# Build specific platform
./scripts/build-all-platforms.sh win-x64
```

**Output:** All builds in `bin/Release/` directory

---

## 📋 **Prerequisites**

### **Required Software**

| Software | Version | Purpose |
|----------|---------|---------|
| **.NET SDK** | 9.0+ | Core build tool |
| **Git** | Latest | Source control |
| **PowerShell** | 7.0+ | Windows scripts |
| **Bash** | 4.0+ | Linux/macOS scripts |

### **Platform-Specific Requirements**

#### **Windows**
- Windows 10/11
- PowerShell 7.0+
- .NET SDK 9.0+
- Visual Studio 2022 (optional, for IDE)

#### **Linux**
- Ubuntu 20.04+ / Debian 11+
- .NET SDK 9.0+
- Build tools: `build-essential`
- Git

#### **macOS**
- macOS 11+ (Big Sur)
- .NET SDK 9.0+
- Xcode Command Line Tools
- Git

---

## 🔧 **Detailed Build Process**

### **Step 1: Environment Setup**

```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Verify .NET SDK
dotnet --version
# Expected: 9.0.x

# Restore dependencies
dotnet restore
```

### **Step 2: Build Configuration**

ZPL2PDF uses these build configurations:

| Configuration | Purpose | Output |
|---------------|---------|--------|
| **Debug** | Development | Debug symbols, no optimization |
| **Release** | Production | Optimized, self-contained |

**Default:** Release configuration for all distributions

### **Step 3: Platform-Specific Builds**

#### **Windows Builds**

```powershell
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained true -o bin/Release/win-x64

# Windows x86  
dotnet publish -c Release -r win-x86 --self-contained true -o bin/Release/win-x86

# Windows ARM64
dotnet publish -c Release -r win-arm64 --self-contained true -o bin/Release/win-arm64
```

#### **Linux Builds**

```bash
# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained true -o bin/Release/linux-x64

# Linux ARM64
dotnet publish -c Release -r linux-arm64 --self-contained true -o bin/Release/linux-arm64

# Linux ARM
dotnet publish -c Release -r linux-arm --self-contained true -o bin/Release/linux-arm
```

#### **macOS Builds**

```bash
# macOS x64 (Intel)
dotnet publish -c Release -r osx-x64 --self-contained true -o bin/Release/osx-x64

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained true -o bin/Release/osx-arm64
```

### **Step 4: Build Validation**

After building, validate each platform:

```bash
# Windows
bin/Release/win-x64/ZPL2PDF.exe --help

# Linux
bin/Release/linux-x64/ZPL2PDF --help

# macOS
bin/Release/osx-x64/ZPL2PDF --help
```

---

## 📦 **Package Creation**

### **Archive Creation**

After building, create distribution archives:

```bash
# Windows (PowerShell)
Compress-Archive -Path "bin/Release/win-x64/*" -DestinationPath "ZPL2PDF-v2.0.0-win-x64.zip"

# Linux/macOS
tar -czf ZPL2PDF-v2.0.0-linux-x64.tar.gz -C bin/Release/linux-x64 .
```

### **Checksum Generation**

```bash
# Windows (PowerShell)
Get-FileHash ZPL2PDF-v2.0.0-win-x64.zip -Algorithm SHA256 | Out-File ZPL2PDF-v2.0.0-win-x64.zip.sha256

# Linux/macOS
sha256sum ZPL2PDF-v2.0.0-linux-x64.tar.gz > ZPL2PDF-v2.0.0-linux-x64.tar.gz.sha256
```

---

## 🐳 **Docker Build Process**

### **Build Docker Image**

```bash
# Build multi-architecture image
docker buildx build --platform linux/amd64,linux/arm64 -t brunoleocam/zpl2pdf:latest .

# Build single architecture
docker build -t brunoleocam/zpl2pdf:latest .
```

### **Test Docker Image**

```bash
# Test the image
docker run --rm brunoleocam/zpl2pdf:latest --help

# Test with volume
docker run -v /path/to/zpl:/app/watch -v /path/to/output:/app/output brunoleocam/zpl2pdf:latest run -l /app/watch -o /app/output
```

---

## 📱 **Windows Installer Build**

### **Prerequisites**

- Windows 10/11
- [Inno Setup 6.2+](https://jrsoftware.org/isinfo.php)
- PowerShell 7.0+

### **Build Installer**

```powershell
# Build Windows installer
.\installer\build-installer.ps1

# Output: ZPL2PDF-Setup-2.0.0.exe
```

### **Installer Features**

- ✅ **Multi-language support** (8 languages)
- ✅ **Desktop shortcut** creation
- ✅ **Start menu** integration
- ✅ **File association** (.zpl → ZPL2PDF)
- ✅ **PATH integration** (optional)
- ✅ **Uninstaller** included
- ✅ **Digital signature** support

---

## 📦 **Linux Package Build**

### **Build .deb Package (Debian/Ubuntu)**

```bash
# Using Docker (recommended)
docker run --rm -v ${PWD}:/workspace ubuntu:22.04 bash -c "
  apt update && apt install -y dpkg-dev
  cd /workspace
  ./scripts/build-deb.sh
"

# Output: ZPL2PDF-v2.0.0-linux-amd64.deb
```

### **Build .rpm Package (Fedora/CentOS/RHEL)**

```bash
# Using Docker (recommended)
docker run --rm -v ${PWD}:/workspace fedora:38 bash -c "
  dnf install -y rpm-build
  cd /workspace  
  ./scripts/build-rpm.sh
"

# Output: ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz
```

---

## 🔍 **Build Validation**

### **Automated Tests**

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter Category=Integration

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### **Manual Validation**

#### **Windows Validation**

```powershell
# Test conversion mode
.\bin\Release\win-x64\ZPL2PDF.exe -i test.zpl -o output -n test.pdf -w 4 -h 2 -u in

# Test daemon mode
.\bin\Release\win-x64\ZPL2PDF.exe run -l C:\watch -o C:\output

# Test help
.\bin\Release\win-x64\ZPL2PDF.exe --help
```

#### **Linux Validation**

```bash
# Test conversion mode
./bin/Release/linux-x64/ZPL2PDF -i test.zpl -o output -n test.pdf -w 4 -h 2 -u in

# Test daemon mode
./bin/Release/linux-x64/ZPL2PDF run -l /tmp/watch -o /tmp/output

# Test help
./bin/Release/linux-x64/ZPL2PDF --help
```

#### **macOS Validation**

```bash
# Test conversion mode
./bin/Release/osx-x64/ZPL2PDF -i test.zpl -o output -n test.pdf -w 4 -h 2 -u in

# Test daemon mode
./bin/Release/osx-x64/ZPL2PDF run -l /tmp/watch -o /tmp/output

# Test help
./bin/Release/osx-x64/ZPL2PDF --help
```

---

## 🚨 **Troubleshooting Build Issues**

### **Common Issues**

#### **Issue: "dotnet command not found"**
```bash
# Solution: Install .NET SDK
# Windows: Download from https://dotnet.microsoft.com/download
# Linux: https://docs.microsoft.com/en-us/dotnet/core/install/linux
# macOS: https://docs.microsoft.com/en-us/dotnet/core/install/macos
```

#### **Issue: "Permission denied" (Linux/macOS)**
```bash
# Solution: Make scripts executable
chmod +x scripts/build-all-platforms.sh
chmod +x scripts/build-deb.sh
chmod +x scripts/build-rpm.sh
```

#### **Issue: "Build fails on ARM64"**
```bash
# Solution: Check .NET SDK supports ARM64
dotnet --list-sdks
# Ensure 9.0.x is installed
```

#### **Issue: "Docker build fails"**
```bash
# Solution: Check Docker is running
docker --version
docker ps

# Check Dockerfile syntax
docker build --no-cache .
```

#### **Issue: "Installer build fails"**
```bash
# Solution: Check Inno Setup is installed
# Windows: Install from https://jrsoftware.org/isinfo.php
# Verify ISCC.exe is in PATH
```

---

## 📊 **Build Performance**

### **Build Times (Approximate)**

| Platform | Build Time | Size |
|----------|------------|------|
| **Windows x64** | 30 sec | 45 MB |
| **Windows x86** | 30 sec | 42 MB |
| **Windows ARM64** | 35 sec | 48 MB |
| **Linux x64** | 25 sec | 38 MB |
| **Linux ARM64** | 30 sec | 40 MB |
| **Linux ARM** | 35 sec | 36 MB |
| **macOS x64** | 30 sec | 42 MB |
| **macOS ARM64** | 35 sec | 44 MB |
| **Docker Image** | 2 min | 470 MB |
| **Windows Installer** | 1 min | 35 MB |

### **Optimization Tips**

1. ✅ **Use SSD** for faster I/O
2. ✅ **Increase RAM** for parallel builds
3. ✅ **Use Release config** for production
4. ✅ **Clean build** before release (`dotnet clean`)
5. ✅ **Parallel builds** with scripts

---

## 🔄 **CI/CD Integration**

### **GitHub Actions Build**

The build process is fully automated via GitHub Actions:

```yaml
# .github/workflows/ci.yml
- name: Build All Platforms
  run: |
    dotnet publish -c Release -r win-x64 --self-contained true
    dotnet publish -c Release -r linux-x64 --self-contained true
    # ... all platforms
```

### **Local CI Simulation**

```bash
# Simulate CI locally
.\scripts\build-all-platforms.ps1 -SimulateCI

# This runs:
# 1. Clean build
# 2. Restore dependencies  
# 3. Build all platforms
# 4. Run tests
# 5. Create archives
# 6. Generate checksums
```

---

## 📚 **Build Scripts Reference**

### **Available Scripts**

| Script | Purpose | Platform |
|--------|---------|----------|
| `build-all-platforms.ps1` | Build all platforms | Windows |
| `build-all-platforms.sh` | Build all platforms | Linux/macOS |
| `build-deb.sh` | Build .deb package | Linux |
| `build-rpm.sh` | Build .rpm package | Linux |
| `build-linux-packages.ps1` | Build Linux packages | Windows |
| `release.ps1` | Create release | Windows |
| `release.sh` | Create release | Linux/macOS |

### **Script Usage**

```powershell
# Windows PowerShell
.\scripts\build-all-platforms.ps1 -Help
.\scripts\build-all-platforms.ps1 -Platform win-x64
.\scripts\build-all-platforms.ps1 -Configuration Release
```

```bash
# Linux/macOS Bash
./scripts/build-all-platforms.sh --help
./scripts/build-all-platforms.sh win-x64
./scripts/build-all-platforms.sh --config Release
```

---

## 🎯 **Release Checklist**

Before creating a release:

- [ ] ✅ **Version updated** in all files
- [ ] ✅ **CHANGELOG.md** updated
- [ ] ✅ **All tests pass** (`dotnet test`)
- [ ] ✅ **All platforms build** successfully
- [ ] ✅ **Docker image** builds and runs
- [ ] ✅ **Windows installer** builds and installs
- [ ] ✅ **Linux packages** build successfully
- [ ] ✅ **Documentation** updated
- [ ] ✅ **Git tag** created (`git tag v2.0.0`)
- [ ] ✅ **GitHub Release** created

---

## 🚀 **Next Steps**

1. ✅ **Set up build environment** with prerequisites
2. ✅ **Test local build** process
3. ✅ **Validate all platforms** work correctly
4. ✅ **Create first release** using scripts
5. ✅ **Monitor CI/CD** automation
6. ✅ **Enjoy automated builds!** 🎉

---

**This build process ensures ZPL2PDF works consistently across all platforms!** 🚀
