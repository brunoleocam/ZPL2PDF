# 🚀 Release Process

Step-by-step guide for creating and publishing ZPL2PDF releases.

## 🎯 Release Types

### Semantic Versioning
- **Major** (v2.0.0): Breaking changes
- **Minor** (v2.1.0): New features, backwards compatible
- **Patch** (v2.0.1): Bug fixes

---

## 📋 Pre-Release Checklist

- [ ] All features merged to `main`
- [ ] All tests passing
- [ ] Code coverage > 80%
- [ ] Documentation updated
- [ ] `CHANGELOG.md` updated
- [ ] Version bumped in `.csproj`
- [ ] No open critical bugs

---

## 🔧 Release Steps

### 1. Update Version
```xml
<!-- ZPL2PDF.csproj -->
<PropertyGroup>
  <Version>2.0.0</Version>
  <AssemblyVersion>2.0.0.0</AssemblyVersion>
  <FileVersion>2.0.0.0</FileVersion>
</PropertyGroup>
```

### 2. Update Changelog
```markdown
## [2.0.0] - 2024-01-15

### Added
- Multi-language support (8 languages)
- Docker deployment
- Clean Architecture refactoring

### Fixed
- Dimension extraction bugs
- Memory leaks in daemon mode

### Changed
- Improved performance by 30%
```

### 3. Build All Platforms
```bash
# Windows
.\scripts\build-all-platforms.ps1

# Linux/macOS
./scripts/build-all-platforms.sh
```

### 4. Create Git Tag
```bash
# Create annotated tag
git tag -a v2.0.0 -m "Release v2.0.0"

# Push tag to GitHub
git push origin v2.0.0
```

### 5. Create GitHub Release
1. Go to [GitHub Releases](https://github.com/brunoleocam/ZPL2PDF/releases)
2. Click "Draft a new release"
3. Select tag `v2.0.0`
4. Title: `ZPL2PDF v2.0.0`
5. Description: Copy from `CHANGELOG.md`
6. Upload artifacts from `build/publish/`
7. Click "Publish release"

### 6. Publish Docker Images
```bash
# Login to registries
docker login
docker login ghcr.io

# Build and push multi-arch
docker buildx build --platform linux/amd64,linux/arm64 \
  -t brunoleocam/zpl2pdf:2.0.0 \
  -t brunoleocam/zpl2pdf:latest \
  -t ghcr.io/brunoleocam/zpl2pdf:2.0.0 \
  -t ghcr.io/brunoleocam/zpl2pdf:latest \
  --push .
```

### 7. Submit WinGet PR
```powershell
# Update manifests
.\scripts\winget-submit.ps1 -Version "2.0.0" -NoConfirm
```

### 8. Announce Release
- Twitter/X
- LinkedIn
- Reddit (r/dotnet, r/selfhosted)
- GitHub Discussions

---

## 🔄 Post-Release Tasks

- [ ] Monitor for critical bugs
- [ ] Update documentation site
- [ ] Respond to user feedback
- [ ] Plan next release

---

## 🔗 Related Topics

- [[Build Process]] - Building artifacts
- [[Package Formats]] - Package types
- [[Distribution Channels]] - Publishing locations
