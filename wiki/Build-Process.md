# 🔨 Build Process

Complete guide for building ZPL2PDF for all platforms.

## 🎯 Build Targets

ZPL2PDF supports 8 platform targets:
- **Windows**: x64, x86, ARM64
- **Linux**: x64, ARM64, ARM
- **macOS**: x64 (Intel), ARM64 (Apple Silicon)

---

## 🚀 Quick Build Commands

### Build for Current Platform
```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release
```

### Build for Specific Platform
```bash
# Windows x64
dotnet publish -c Release -r win-x64 --self-contained true

# Linux x64
dotnet publish -c Release -r linux-x64 --self-contained true

# macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained true
```

---

## 📦 Build All Platforms

### Windows (PowerShell)
```powershell
.\scripts\build-all-platforms.ps1
```

### Linux/macOS (Bash)
```bash
chmod +x scripts/build-all-platforms.sh
./scripts/build-all-platforms.sh
```

### Output Structure
```
build/publish/
├── ZPL2PDF-v2.0.0-win-x64.zip
├── ZPL2PDF-v2.0.0-win-x86.zip
├── ZPL2PDF-v2.0.0-win-arm64.zip
├── ZPL2PDF-v2.0.0-linux-x64.tar.gz
├── ZPL2PDF-v2.0.0-linux-arm64.tar.gz
├── ZPL2PDF-v2.0.0-linux-arm.tar.gz
├── ZPL2PDF-v2.0.0-osx-x64.tar.gz
├── ZPL2PDF-v2.0.0-osx-arm64.tar.gz
└── SHA256SUMS.txt
```

---

## 🏗️ Build Configuration

### Project Settings (ZPL2PDF.csproj)
```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <OutputType>Exe</OutputType>
  <RuntimeIdentifiers>win-x64;win-x86;win-arm64;linux-x64;linux-arm64;linux-arm;osx-x64;osx-arm64</RuntimeIdentifiers>
  <PublishSingleFile>true</PublishSingleFile>
  <PublishTrimmed>true</PublishTrimmed>
  <SelfContained>true</SelfContained>
</PropertyGroup>
```

### Build Optimizations
- **Trimming**: Reduces binary size
- **ReadyToRun**: Improves startup time
- **Single File**: Packages all dependencies

---

## 🐳 Docker Build

```bash
# Build multi-arch image
docker buildx build --platform linux/amd64,linux/arm64 -t brunoleocam/zpl2pdf:latest .

# Build for specific platform
docker build -t zpl2pdf:latest .

# Build and push
docker buildx build --platform linux/amd64,linux/arm64 -t brunoleocam/zpl2pdf:latest --push .
```

---

## 🪟 Windows Installer

### Build Installer (Inno Setup)
```powershell
# Build application
dotnet publish -c Release -r win-x64 --self-contained true

# Compile installer
.\scripts\build-installer.ps1
```

### Output
- `ZPL2PDF-Setup-v2.0.0.exe` (35.46 MB)

---

## 📋 Build Checklist

- [ ] Update version in `.csproj`
- [ ] Update `CHANGELOG.md`
- [ ] Run all tests: `dotnet test`
- [ ] Build all platforms
- [ ] Generate checksums
- [ ] Build Docker images
- [ ] Build Windows installer
- [ ] Verify all artifacts

---

## 🔗 Related Topics

- [[Release Process]] - Creating releases
- [[Development Setup]] - Development environment
- [[Package Formats]] - Package types
- [[Distribution Channels]] - Where to publish
