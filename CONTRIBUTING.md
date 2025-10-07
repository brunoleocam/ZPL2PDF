# 🤝 Contributing to ZPL2PDF

Thank you for your interest in contributing to ZPL2PDF! This document provides guidelines and information for contributors.

---

## 🚀 Getting Started

### Prerequisites
- ✅ .NET 9.0 SDK or later
- ✅ Git
- ✅ Visual Studio 2022, VS Code, or Rider (recommended)
- ✅ Docker (for cross-platform testing)
- ✅ Inno Setup (for Windows installer, optional)

### Development Setup
```bash
# 1. Fork and clone the repository
git clone https://github.com/YOUR_USERNAME/ZPL2PDF.git
cd ZPL2PDF

# 2. Restore dependencies
dotnet restore

# 3. Build the solution
dotnet build

# 4. Run tests
dotnet test

# 5. Build for your platform
dotnet publish -c Release -r win-x64 --self-contained true

# 6. Test locally
.\bin\Release\net9.0\win-x64\publish\ZPL2PDF.exe -help
```

## 🏗️ Project Structure

```
ZPL2PDF/
├── src/                    # Source code
│   ├── Application/        # Use cases and services
│   ├── Domain/            # Business entities and rules
│   ├── Infrastructure/    # External concerns
│   ├── Presentation/      # CLI and user interface
│   └── Shared/           # Common utilities
├── tests/                 # Test projects
│   ├── ZPL2PDF.Unit/     # Unit tests
│   └── ZPL2PDF.Integration/ # Integration tests
├── docs/                  # Documentation
└── build/                 # Build scripts
```

## 🧪 Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test tests/ZPL2PDF.Unit/

# Run integration tests only
dotnet test tests/ZPL2PDF.Integration/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Guidelines
- Write tests for all new functionality
- Maintain test coverage above 90%
- Use descriptive test names
- Follow AAA pattern (Arrange, Act, Assert)
- Mock external dependencies

## 📝 Code Style

### C# Guidelines
- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Use `var` when type is obvious
- Prefer `string.IsNullOrWhiteSpace()` over `string.IsNullOrEmpty()`

### Architecture Guidelines
- Follow Clean Architecture principles
- Use dependency injection
- Keep classes focused on single responsibility
- Use interfaces for abstractions
- Implement proper error handling

## 🔧 Development Workflow

### 1. Create a Feature Branch
```bash
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes
- Write code following the style guidelines
- Add tests for new functionality
- Update documentation if needed

### 3. Test Your Changes
```bash
# Run all tests
dotnet test

# Build for all platforms
dotnet build -c Release

# Test cross-platform (if applicable)
docker run --rm -v ${PWD}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet test
```

### 4. Commit Your Changes
```bash
git add .
git commit -m "feat: add new feature description"
```

### 5. Push and Create Pull Request
```bash
git push origin feature/your-feature-name
```

## 📋 Pull Request Guidelines

### Before Submitting
- [ ] Code follows style guidelines
- [ ] All tests pass
- [ ] New functionality has tests
- [ ] Documentation is updated
- [ ] No breaking changes (or clearly documented)

### Pull Request Template
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] No breaking changes
```

## 🐛 Bug Reports

### Before Reporting
1. Check existing issues
2. Ensure you're using the latest version
3. Try to reproduce the issue

### Bug Report Template
```markdown
**Describe the bug**
A clear description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Run command '...'
2. See error

**Expected behavior**
What you expected to happen.

**Environment**
- OS: [e.g. Windows 11, Ubuntu 22.04]
- Version: [e.g. 2.0.0]
- .NET Version: [e.g. 9.0]

**Additional context**
Any other context about the problem.
```

## ✨ Feature Requests

### Before Requesting
1. Check existing feature requests
2. Consider if it fits the project scope
3. Provide clear use case

### Feature Request Template
```markdown
**Is your feature request related to a problem?**
A clear description of what the problem is.

**Describe the solution you'd like**
A clear description of what you want to happen.

**Describe alternatives you've considered**
A clear description of any alternative solutions.

**Additional context**
Add any other context or screenshots about the feature request.
```

## 🏷️ Release Process

### Version Numbering
We follow [Semantic Versioning](https://semver.org/):
- **MAJOR** (3.0.0): Breaking changes
- **MINOR** (2.1.0): New features (backward compatible)
- **PATCH** (2.0.1): Bug fixes (backward compatible)

### Release Checklist
- [ ] All tests pass (`dotnet test`)
- [ ] Documentation updated
- [ ] Version numbers updated in:
  - [ ] `ZPL2PDF.csproj`
  - [ ] `installer/ZPL2PDF-Setup.iss`
  - [ ] `CHANGELOG.md`
- [ ] Changelog updated with changes
- [ ] Cross-platform builds tested
- [ ] Create Git tag (`git tag -a v2.1.0 -m "Release v2.1.0"`)
- [ ] Push tag (`git push origin v2.1.0`)
- [ ] Create GitHub Release (automatic CI/CD takes over)

### Automated Release Process

When you create a GitHub Release, our CI/CD automatically:
1. ✅ Runs all tests on all platforms
2. ✅ Builds for 8 platforms (Windows, Linux, macOS)
3. ✅ Builds and publishes Docker images
4. ✅ Builds Windows installer (Inno Setup)
5. ✅ Uploads all artifacts to GitHub Release
6. ✅ Creates WinGet package update PR

See [CI/CD Workflow](docs/development/CI_CD_WORKFLOW.md) for details.

## 🌍 Cross-Platform Development

### Supported Platforms
- Windows (x64, x86)
- Linux (x64, ARM64, ARM)
- macOS (x64, ARM64)

### Testing on Different Platforms
```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained true

# macOS
dotnet publish -c Release -r osx-x64 --self-contained true
```

### Docker Testing
```bash
# Test on Linux
docker run --rm -v ${PWD}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 dotnet test

# Test on Alpine
docker run --rm -v ${PWD}:/app -w /app mcr.microsoft.com/dotnet/sdk:9.0-alpine dotnet test
```

## 📚 Documentation

### Code Documentation
- ✅ Use XML documentation for public APIs (`///`)
- ✅ Include examples for complex methods
- ✅ Document all parameters and return values
- ✅ Add remarks for important notes

### User Documentation

Documentation is organized in `docs/`:

- **User Guides** (`docs/guides/`): For end-users
- **Development** (`docs/development/`): For contributors
- **Translations** (`docs/i18n/`): Multi-language READMEs

**When to update:**
- ✅ Update README.md for user-facing changes
- ✅ Add examples for new features
- ✅ Update command-line help text (src/Presentation/HelpDisplay.cs)
- ✅ Document configuration options in zpl2pdf.json.example
- ✅ Update localization resources (Resources/Messages.*.resx)

### Documentation Standards
- ✅ Use clear, concise language
- ✅ Include code examples
- ✅ Add screenshots for visual features
- ✅ Keep multi-language docs in sync

## 🤝 Community Guidelines

### Code of Conduct
- Be respectful and inclusive
- Focus on constructive feedback
- Help others learn and grow
- Follow the golden rule

### Communication
- Use clear, concise language
- Provide context for questions
- Be patient with newcomers
- Celebrate contributions

## 🆘 Getting Help

### Resources
- [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
- [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- [Documentation](https://github.com/brunoleocam/ZPL2PDF/wiki)

### Questions
- Check existing discussions first
- Provide clear problem description
- Include relevant code snippets
- Share error messages and logs

## 📄 License

By contributing to ZPL2PDF, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to ZPL2PDF! 🎉
