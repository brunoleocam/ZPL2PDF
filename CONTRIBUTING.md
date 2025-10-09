# 🤝 Contributing to ZPL2PDF

Thank you for your interest in contributing to ZPL2PDF! This guide will help you understand how to contribute effectively and follow our development practices.

## 📋 Table of Contents

- [🚀 Quick Start](#-quick-start)
- [🔒 Branch Protection](#-branch-protection)
- [🛠️ Environment Setup](#️-environment-setup)
- [📝 Conventions](#-conventions)
- [🔄 Contribution Workflow](#-contribution-workflow)
- [🧪 Testing](#-testing)
- [📚 Documentation](#-documentation)
- [🚨 Special Cases](#-special-cases)
- [❓ FAQ](#-faq)

## 🚀 Quick Start

### 1. Fork the Repository
```bash
# Fork the repository on GitHub
# Then clone your fork
git clone https://github.com/YOUR_USERNAME/ZPL2PDF.git
cd ZPL2PDF
```

### 2. Setup Remote
```bash
# Add the original repository as upstream
git remote add upstream https://github.com/brunoleocam/ZPL2PDF.git

# Verify remotes
git remote -v
```

### 3. Create Feature Branch
```bash
# Update main with latest changes
git checkout main
git pull upstream main

# Create new branch
git checkout -b feature/your-feature
```

## 🔒 Branch Protection

⚠️ **IMPORTANT**: The `main` branch is protected! You **CANNOT** push directly to it.

### ✅ Required Workflow:
1. **Create branch** from `main`
2. **Make commits** following convention
3. **Push branch** to your fork
4. **Create Pull Request** in the original repository
5. **Wait for CI/CD** to pass (5 status checks)
6. **Wait for approval** from maintainer
7. **Merge** after approval

## 🛠️ Environment Setup

### Prerequisites
- **.NET 9.0 SDK**
- **Git** (2.30+)
- **Visual Studio 2022** or **VS Code** (recommended)
- **Docker** (for Linux testing)

### Configuration
```bash
# 1. Restore dependencies
dotnet restore

# 2. Build solution
dotnet build

# 3. Run tests
dotnet test

# 4. Run application
dotnet run --project src/ZPL2PDF.csproj --help
```

### Recommended VS Code Extensions
- **C# Dev Kit**
- **GitLens**
- **GitHub Pull Requests**
- **Git Graph**

## 📝 Conventions

### Commits (Conventional Commits)
```bash
# Format: type(scope): description

# Allowed types:
feat: add new feature
fix: fix bug
docs: update documentation
style: formatting (spaces, etc)
refactor: refactor code
test: add/modify tests
chore: maintenance tasks
perf: performance improvement
ci: CI/CD changes
build: build system changes

# Examples:
git commit -m "feat(daemon): add custom configuration support"
git commit -m "fix(conversion): fix unit conversion error"
git commit -m "docs(readme): update installation instructions"
git commit -m "test(integration): add tests for daemon mode"
```

### Branch Naming
```bash
# Patterns:
feature/feature-name
fix/bug-description
hotfix/urgent-fix
docs/documentation-type
refactor/refactored-area
test/test-type
chore/maintenance-task

# Examples:
feature/docker-compose
fix/memory-leak-conversion
hotfix/critical-security-issue
docs/api-reference
refactor/clean-architecture
test/unit-tests-coverage
```

### Code
- **Language**: Code in English, comments in English
- **Formatting**: Follow .NET standards
- **Naming**: PascalCase for classes, camelCase for variables
- **Documentation**: XMLDoc for public APIs

## 🔄 Contribution Workflow

### 1. Planning
- [ ] Check existing [Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- [ ] Create issue if needed (discuss before coding)
- [ ] Define scope and approach

### 2. Development
```bash
# Create branch
git checkout -b feature/new-feature

# Make incremental changes
git add .
git commit -m "feat: implement feature X"

# Regular push for backup
git push origin feature/new-feature
```

### 3. Local Testing
```bash
# Run all tests
dotnet test

# Specific test
dotnet test tests/ZPL2PDF.Unit/UnitTests/Application/ConversionServiceTests.cs

# Release build
dotnet build --configuration Release

# Manual application test
dotnet run --project src/ZPL2PDF.csproj --help
```

### 4. Pull Request
```bash
# Ensure you're up to date
git checkout main
git pull upstream main
git checkout feature/new-feature
git rebase main

# Final push
git push origin feature/new-feature
```

#### PR Template:
```markdown
## 📝 Description
Brief description of what was implemented/fixed.

## 🔗 Related Issue
Closes #123

## 🧪 Tests
- [ ] Unit tests passing
- [ ] Integration tests passing
- [ ] Manual testing done
- [ ] Tested on Windows/Linux

## 📋 Checklist
- [ ] Code follows project conventions
- [ ] Documentation updated
- [ ] No breaking changes (or documented)
- [ ] Commits follow conventional commits
- [ ] Branch updated with main

## 📸 Screenshots (if applicable)
Add screenshots if UI changes.

## 🚀 How to Test
1. Testing instructions
2. Step by step
3. Expected result
```

### 5. Code Review
- **Respond** to reviewer comments
- **Make requested** adjustments
- **Keep discussion** constructive
- **Accept suggestions** when appropriate

### 6. Merge
- **Only maintainer** can merge
- **CI/CD must pass** (5 status checks)
- **At least 1 approval** required

## 🧪 Testing

### Test Structure
```
tests/
├── ZPL2PDF.Unit/           # Unit tests
│   ├── UnitTests/
│   │   ├── Application/    # Service tests
│   │   ├── Domain/         # Value object tests
│   │   ├── Infrastructure/ # Infrastructure tests
│   │   └── Presentation/   # CLI tests
│   └── TestData/           # Test data
└── ZPL2PDF.Integration/    # Integration tests
    ├── IntegrationTests/
    └── TestData/
```

### Run Tests
```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/ZPL2PDF.Unit/

# Integration tests only
dotnet test tests/ZPL2PDF.Integration/

# With coverage
dotnet test --collect:"XPlat Code Coverage"

# Verbose
dotnet test --verbosity normal
```

### Write Tests
```csharp
[Test]
public void ConversionService_ValidInput_ReturnsExpectedResult()
{
    // Arrange
    var service = new ConversionService();
    var input = "test data";
    
    // Act
    var result = service.Convert(input);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.IsSuccess, Is.True);
}
```

## 📚 Documentation

### Documentation Types
- **README**: Overview and quick start
- **Wiki**: Detailed documentation
- **Comments**: Inline code
- **XMLDoc**: Public APIs
- **CHANGELOG**: Changes per version

### Update Documentation
```bash
# Main documentation
README.md
CONTRIBUTING.md
SECURITY.md

# Wiki (copy from wiki/ to GitHub Wiki)
wiki/
├── Home.md
├── Installation-Guide.md
├── Basic-Usage.md
└── ...

# Code comments
/// <summary>
/// Converts ZPL content to PDF format
/// </summary>
/// <param name="zplContent">ZPL content to convert</param>
/// <returns>Conversion result</returns>
public ConversionResult Convert(string zplContent)
```

## 🚨 Special Cases

### Urgent Hotfix
```bash
# For critical production bugs
git checkout -b hotfix/critical-bug-fix
# Make minimal fix
git commit -m "fix: fix critical conversion bug"
git push origin hotfix/critical-bug-fix
# Create PR immediately with "urgent" label
```

### Breaking Changes
```bash
# Document breaking changes in PR
git commit -m "feat!: change conversion API (breaking change)

BREAKING CHANGE: Convert() method now returns Task<Result>
instead of Result directly."
```

### Rollback
```bash
# Revert specific commit
git checkout -b hotfix/rollback-commit-xyz
git revert <commit-hash>
git commit -m "revert: undo problematic changes"
```

## ❓ FAQ

### Q: Can I contribute even as a beginner?
**A**: Yes! Start with:
- Fixing typos in documentation
- Improving tests
- Issues with "good first issue" label

### Q: How do I know if my contribution is welcome?
**A**: Always create an issue first to discuss:
- Bugs: Use bug report template
- Features: Use feature request template
- Improvements: Discuss before implementing

### Q: How long does it take for my PR to be reviewed?
**A**: Usually 2-5 business days. For hotfixes, it can be faster.

### Q: Can I contribute translations?
**A**: Yes! See `Resources/Messages.*.resx` to add new languages.

### Q: How to report security issues?
**A**: Use the process in `SECURITY.md` - **DO NOT** create public issue.

### Q: Can I suggest architectural changes?
**A**: Yes! Create issue for discussion before implementing.

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- **Discussions**: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
- **Wiki**: [Complete Documentation](https://github.com/brunoleocam/ZPL2PDF/wiki)
- **Security**: See `SECURITY.md` for vulnerability reporting

## 🎉 Thank You!

Your contribution is very important to ZPL2PDF. Together, we can make this tool even better for the community!

---

**Last updated**: December 2024  
**Version**: 2.0.0