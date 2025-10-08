# 🤝 Contributing Guidelines

Welcome to ZPL2PDF! We appreciate your interest in contributing to the project.

## 🎯 Ways to Contribute

### 1. Report Bugs 🐛
- Search [existing issues](https://github.com/brunoleocam/ZPL2PDF/issues) first
- Use the bug report template
- Include reproduction steps
- Provide system information

### 2. Suggest Features 💡
- Check [existing feature requests](https://github.com/brunoleocam/ZPL2PDF/issues?q=is%3Aissue+label%3Aenhancement)
- Use the feature request template
- Explain the use case
- Provide examples if possible

### 3. Improve Documentation 📚
- Fix typos and grammatical errors
- Add examples and clarifications
- Translate documentation
- Update outdated information

### 4. Submit Code 💻
- Fix bugs
- Implement features
- Improve performance
- Add tests

---

## 🚀 Getting Started

### Prerequisites
```bash
# Install .NET 9.0 SDK
# Download from https://dotnet.microsoft.com/download/dotnet/9.0

# Install Git
# Download from https://git-scm.com/downloads

# Install Docker (optional)
# Download from https://www.docker.com/products/docker-desktop
```

### Fork and Clone
```bash
# Fork the repository on GitHub
# Click "Fork" button at https://github.com/brunoleocam/ZPL2PDF

# Clone your fork
git clone https://github.com/YOUR_USERNAME/ZPL2PDF.git
cd ZPL2PDF

# Add upstream remote
git remote add upstream https://github.com/brunoleocam/ZPL2PDF.git

# Verify remotes
git remote -v
```

### Build and Test
```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests
dotnet test

# Run application
dotnet run -- --help
```

---

## 📋 Development Workflow

### 1. Create a Branch
```bash
# Update main branch
git checkout main
git pull upstream main

# Create feature branch
git checkout -b feature/your-feature-name

# Or bugfix branch
git checkout -b bugfix/issue-number-description
```

### 2. Make Changes
```bash
# Write code following coding standards
# Add tests for new functionality
# Update documentation if needed

# Check for linting errors
dotnet format
```

### 3. Commit Changes
```bash
# Stage changes
git add .

# Commit with descriptive message
git commit -m "feat: add support for custom output naming

- Add new command-line parameter --output-pattern
- Implement pattern substitution logic
- Add tests for pattern validation
- Update documentation

Closes #123"
```

### Commit Message Guidelines
We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(daemon): add support for multiple watch folders
fix(conversion): handle invalid ZPL gracefully
docs(readme): update installation instructions
test(converter): add unit tests for dimension extraction
```

### 4. Push Changes
```bash
# Push to your fork
git push origin feature/your-feature-name
```

### 5. Create Pull Request
1. Go to your fork on GitHub
2. Click "New Pull Request"
3. Select your feature branch
4. Fill out the PR template
5. Submit for review

---

## 🏗️ Code Standards

### C# Coding Style
```csharp
// Use meaningful names
public class ConversionService
{
    // Use PascalCase for public members
    public void ConvertZplToPdf(string inputPath, string outputPath)
    {
        // Use camelCase for local variables
        var labelDimensions = ExtractDimensions(inputPath);
        
        // Use const for constants
        const int DefaultDpi = 203;
        
        // Add XML documentation for public APIs
        /// <summary>
        /// Extracts label dimensions from ZPL content.
        /// </summary>
        private LabelDimensions ExtractDimensions(string path)
        {
            // Implementation
        }
    }
}
```

### Code Organization
```
src/
├── Application/        # Application services
│   ├── Services/
│   └── Interfaces/
├── Domain/            # Business logic
│   ├── Services/
│   └── ValueObjects/
├── Infrastructure/    # External concerns
│   ├── FileSystem/
│   ├── Configuration/
│   └── Rendering/
├── Presentation/      # CLI interface
└── Shared/           # Common utilities
```

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Subtypes must be substitutable
- **Interface Segregation**: Specific interfaces over general ones
- **Dependency Inversion**: Depend on abstractions, not concretions

---

## 🧪 Testing Guidelines

### Unit Tests
```csharp
[Fact]
public void ConvertZplToPdf_ValidInput_ReturnsSuccess()
{
    // Arrange
    var service = new ConversionService();
    var options = new ConversionOptions
    {
        InputPath = "test.txt",
        OutputPath = "output",
        Width = 7.5,
        Height = 15,
        Unit = "in"
    };
    
    // Act
    var result = service.Convert(options);
    
    // Assert
    Assert.True(result.Success);
    Assert.True(File.Exists("output/test.pdf"));
}
```

### Test Coverage
- Aim for > 80% code coverage
- Test happy paths and edge cases
- Test error handling
- Mock external dependencies

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test
dotnet test --filter "FullyQualifiedName~ConversionService"
```

---

## 📚 Documentation Standards

### Code Documentation
```csharp
/// <summary>
/// Converts ZPL file to PDF document.
/// </summary>
/// <param name="inputPath">Path to input ZPL file.</param>
/// <param name="outputPath">Path for output PDF file.</param>
/// <param name="options">Conversion options.</param>
/// <returns>Conversion result with success status and details.</returns>
/// <exception cref="FileNotFoundException">Thrown when input file not found.</exception>
/// <exception cref="InvalidOperationException">Thrown when conversion fails.</exception>
public ConversionResult ConvertZplToPdf(
    string inputPath,
    string outputPath,
    ConversionOptions options)
{
    // Implementation
}
```

### Wiki Documentation
- Use clear, concise language
- Include code examples
- Add screenshots when helpful
- Link to related pages
- Keep up to date

---

## 🔍 Code Review Process

### What We Look For
- ✅ Code follows project standards
- ✅ Tests are included and pass
- ✅ Documentation is updated
- ✅ No breaking changes (or documented)
- ✅ Performance implications considered
- ✅ Security implications considered

### Review Timeline
- Initial review: Within 2-3 days
- Follow-up reviews: Within 1-2 days
- Merge: After approval and CI passes

### Addressing Feedback
```bash
# Make requested changes
# Commit with descriptive message
git add .
git commit -m "refactor: simplify conversion logic per review"

# Push changes
git push origin feature/your-feature-name

# PR will update automatically
```

---

## 🌍 Translation Contributions

### Adding a New Language
1. Copy `Resources/Messages.en.resx`
2. Rename to `Messages.{culture}.resx` (e.g., `Messages.ru-RU.resx`)
3. Translate all strings
4. Update `LocalizationManager.cs`
5. Test with `--language {culture}`

### Updating Translations
1. Update relevant `.resx` file
2. Test changes
3. Submit PR with translation updates

---

## 📦 Release Process

### Semantic Versioning
We follow [SemVer](https://semver.org/):
- **Major**: Breaking changes (v2.0.0)
- **Minor**: New features (v2.1.0)
- **Patch**: Bug fixes (v2.0.1)

### Release Checklist
- [ ] All tests pass
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] Version bumped in `.csproj`
- [ ] Git tag created
- [ ] GitHub Release created
- [ ] WinGet manifest updated
- [ ] Docker images published

See [[Release Process]] for detailed steps.

---

## 🎯 Priority Areas

We especially welcome contributions in:
- 🌍 **Translations**: Add new languages
- 📚 **Documentation**: Improve guides and examples
- 🧪 **Tests**: Increase coverage
- 🐛 **Bug Fixes**: Address known issues
- ⚡ **Performance**: Optimize critical paths
- 🔧 **Platform Support**: Improve Linux/macOS support

---

## 💬 Communication

### GitHub Discussions
- Questions: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
- Feature Ideas: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
- Show & Tell: [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)

### GitHub Issues
- Bug Reports: [Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- Feature Requests: [Issues](https://github.com/brunoleocam/ZPL2PDF/issues)

### Pull Requests
- Code Contributions: [Pull Requests](https://github.com/brunoleocam/ZPL2PDF/pulls)

---

## 📜 Code of Conduct

### Our Pledge
We are committed to providing a welcoming and inspiring community for all.

### Our Standards
- ✅ Be respectful and inclusive
- ✅ Be collaborative and constructive
- ✅ Focus on what is best for the community
- ✅ Show empathy towards others

### Unacceptable Behavior
- ❌ Harassment or discrimination
- ❌ Trolling or insulting comments
- ❌ Personal or political attacks
- ❌ Publishing others' private information

### Enforcement
Violations can be reported to brunoleocam@gmail.com. All complaints will be reviewed and investigated.

---

## 🏆 Recognition

Contributors are recognized in:
- README.md Contributors section
- GitHub Contributors page
- Release notes (for significant contributions)

---

## 📝 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

## 🔗 Related Topics

- [[Development Setup]] - Setting up development environment
- [[Architecture Overview]] - Understanding the codebase
- [[Testing Guide]] - Writing and running tests
- [[Build Process]] - Building and packaging
