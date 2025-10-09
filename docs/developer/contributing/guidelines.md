# 🤝 Detailed Contributing Guidelines

Comprehensive guide for contributing to ZPL2PDF.

---

## 🎯 **Overview**

This document provides detailed guidelines for contributing to ZPL2PDF beyond the basic workflow covered in [CONTRIBUTING.md](../../../CONTRIBUTING.md).

---

## 📋 **Code Standards**

### **Coding Conventions**

#### **Naming Conventions**

```csharp
// Classes: PascalCase
public class ConversionService { }

// Interfaces: PascalCase with I prefix
public interface IConversionService { }

// Methods: PascalCase
public async Task ConvertAsync() { }

// Private fields: camelCase with _ prefix
private readonly ILabelRenderer _renderer;

// Local variables: camelCase
var labelDimensions = new LabelDimensions();

// Constants: PascalCase
public const int DefaultDpi = 203;

// Properties: PascalCase
public string OutputPath { get; set; }
```

#### **File Organization**

```csharp
// 1. Using statements
using System;
using System.Threading.Tasks;

// 2. Namespace
namespace ZPL2PDF.Application.Services
{
    // 3. Class documentation
    /// <summary>
    /// Service for converting ZPL to PDF
    /// </summary>
    public class ConversionService : IConversionService
    {
        // 4. Private fields
        private readonly ILabelRenderer _renderer;
        
        // 5. Constructor
        public ConversionService(ILabelRenderer renderer)
        {
            _renderer = renderer;
        }
        
        // 6. Public methods
        public async Task<ProcessingResult> ConvertAsync(...)
        {
            // Implementation
        }
        
        // 7. Private methods
        private void ValidateInput(...)
        {
            // Implementation
        }
    }
}
```

### **Code Style**

#### **Formatting**

```csharp
// Use braces for all control structures
if (condition)
{
    DoSomething();
}

// Place opening brace on new line
public void Method()
{
    // Code
}

// Use var for local variables when type is obvious
var service = new ConversionService();
var result = await service.ConvertAsync();

// Explicit type when not obvious
IConversionService service = GetService();
```

#### **Comments**

```csharp
/// <summary>
/// Converts ZPL content to PDF
/// </summary>
/// <param name="zplContent">ZPL content to convert</param>
/// <param name="options">Conversion options</param>
/// <returns>Processing result with success/error information</returns>
public async Task<ProcessingResult> ConvertAsync(
    string zplContent, 
    ConversionOptions options)
{
    // Extract dimensions from ZPL or use provided values
    var dimensions = ExtractDimensions(zplContent, options);
    
    // Render ZPL to image
    var image = await _renderer.RenderAsync(zplContent, dimensions);
    
    // TODO: Add image optimization
    
    return ProcessingResult.Success();
}
```

---

## 🏗️ **Architecture Guidelines**

### **Layer Responsibilities**

#### **Presentation Layer**

- ✅ **DO**: Handle CLI arguments and user input
- ✅ **DO**: Validate command-line parameters
- ✅ **DO**: Format output for console display
- ❌ **DON'T**: Contain business logic
- ❌ **DON'T**: Access infrastructure directly

#### **Application Layer**

- ✅ **DO**: Orchestrate use cases
- ✅ **DO**: Coordinate between domain and infrastructure
- ✅ **DO**: Handle application-level validation
- ❌ **DON'T**: Contain presentation logic
- ❌ **DON'T**: Implement infrastructure details

#### **Domain Layer**

- ✅ **DO**: Define business rules and logic
- ✅ **DO**: Create value objects and entities
- ✅ **DO**: Define domain interfaces
- ❌ **DON'T**: Depend on other layers
- ❌ **DON'T**: Reference infrastructure libraries

#### **Infrastructure Layer**

- ✅ **DO**: Implement domain interfaces
- ✅ **DO**: Handle external dependencies
- ✅ **DO**: Manage file I/O and system resources
- ❌ **DON'T**: Contain business logic
- ❌ **DON'T**: Be referenced by domain layer

---

## 🧪 **Testing Guidelines**

### **Test Structure**

```csharp
[TestFixture]
public class ConversionServiceTests
{
    private Mock<ILabelRenderer> _rendererMock;
    private Mock<IPdfGenerator> _pdfGeneratorMock;
    private ConversionService _service;
    
    [SetUp]
    public void SetUp()
    {
        _rendererMock = new Mock<ILabelRenderer>();
        _pdfGeneratorMock = new Mock<IPdfGenerator>();
        _service = new ConversionService(
            _rendererMock.Object, 
            _pdfGeneratorMock.Object);
    }
    
    [Test]
    public async Task ConvertAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var zplContent = "^XA^FO50,50^A0N,30,30^FDTest^FS^XZ";
        var options = new ConversionOptions { ... };
        
        _rendererMock
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<LabelDimensions>()))
            .ReturnsAsync(new Bitmap(100, 100));
        
        // Act
        var result = await _service.ConvertAsync(zplContent, options);
        
        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
        _rendererMock.Verify(r => r.RenderAsync(zplContent, It.IsAny<LabelDimensions>()), Times.Once);
    }
}
```

### **Test Coverage Requirements**

| Layer | Minimum Coverage | Target Coverage |
|-------|-----------------|-----------------|
| **Domain** | 95% | 100% |
| **Application** | 90% | 95% |
| **Infrastructure** | 80% | 85% |
| **Presentation** | 85% | 90% |
| **Overall** | 85% | 90% |

### **Test Naming Convention**

```
MethodName_StateUnderTest_ExpectedBehavior

Examples:
- ConvertAsync_ValidInput_ReturnsSuccess
- ConvertAsync_InvalidZpl_ThrowsException
- ExtractDimensions_NoPwCommand_UsesDefaultWidth
```

---

## 📝 **Documentation Guidelines**

### **Code Documentation**

```csharp
/// <summary>
/// Extracts label dimensions from ZPL content or uses provided values
/// </summary>
/// <param name="zplContent">ZPL content to analyze</param>
/// <param name="providedWidth">Width provided by user (optional)</param>
/// <param name="providedHeight">Height provided by user (optional)</param>
/// <param name="unit">Unit of measurement (mm, cm, in)</param>
/// <returns>
/// LabelDimensions object with extracted or provided dimensions.
/// Falls back to default values (100mm × 150mm) if neither provided nor extractable.
/// </returns>
/// <exception cref="ArgumentNullException">Thrown when zplContent is null</exception>
/// <exception cref="ArgumentException">Thrown when unit is invalid</exception>
/// <example>
/// <code>
/// var dimensions = ExtractDimensions("^XA^PW800^LL1200^XZ", null, null, "mm");
/// // Returns: Width = 99.8mm, Height = 149.8mm
/// </code>
/// </example>
public LabelDimensions ExtractDimensions(
    string zplContent, 
    double? providedWidth, 
    double? providedHeight, 
    string unit)
{
    // Implementation
}
```

### **User-Facing Documentation**

When adding new features, update:

1. **README.md** - Add to feature list
2. **User Guide** - Add usage instructions
3. **Configuration Guide** - Add new configuration options
4. **Changelog** - Document changes

---

## 🔄 **Pull Request Guidelines**

### **PR Title Format**

Use conventional commits format:

```
type(scope): description

Examples:
feat(daemon): add support for custom file extensions
fix(rendering): resolve memory leak in image processing
docs(readme): update installation instructions
refactor(services): extract dimension calculation logic
test(integration): add tests for daemon mode
```

### **PR Description Template**

```markdown
## 📝 Description
Brief description of changes and motivation

## 🔄 Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

## ✅ Testing
- [ ] Unit tests pass locally
- [ ] Integration tests pass locally
- [ ] Manual testing completed
- [ ] All platforms tested (Windows, Linux, macOS)

## 📋 Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No breaking changes (or documented if intentional)
- [ ] Tests added/updated for changes
- [ ] All tests pass
- [ ] CHANGELOG.md updated

## 📸 Screenshots (if applicable)
Add screenshots for UI changes

## 🔗 Related Issues
Closes #123
Related to #456
```

### **Review Process**

1. **Self-Review**: Review your own PR first
2. **Automated Checks**: Ensure CI/CD passes
3. **Code Review**: Wait for maintainer review
4. **Address Feedback**: Respond to review comments
5. **Approval**: Get required approvals
6. **Merge**: Maintainer merges PR

---

## 🚨 **Common Pitfalls**

### **What to Avoid**

❌ **DON'T**: Mix multiple unrelated changes in one PR
```
# Bad: One PR with multiple unrelated changes
- Add Docker support
- Fix memory leak
- Update documentation
- Refactor services

# Good: Separate PRs for each change
PR #1: feat(docker): add Docker support
PR #2: fix(memory): resolve memory leak in rendering
PR #3: docs: update installation guide
PR #4: refactor(services): extract common logic
```

❌ **DON'T**: Commit directly to `main`
```bash
# This is blocked by branch protection
git commit -m "fix"
git push origin main  # ❌ BLOCKED!

# Always use feature branches
git checkout -b fix/memory-leak
git commit -m "fix(memory): resolve leak in rendering"
git push origin fix/memory-leak
# Create PR on GitHub
```

❌ **DON'T**: Ignore linting errors
```bash
# Fix all linting errors before committing
dotnet format
dotnet build
```

❌ **DON'T**: Skip tests
```bash
# Always run tests before committing
dotnet test
```

---

## 🎯 **Best Practices**

### **Code Quality**

✅ **DO**: Follow SOLID principles
✅ **DO**: Write testable code
✅ **DO**: Use dependency injection
✅ **DO**: Handle errors gracefully
✅ **DO**: Log important events
✅ **DO**: Validate inputs
✅ **DO**: Clean up resources

### **Performance**

✅ **DO**: Use async/await for I/O operations
✅ **DO**: Dispose of resources properly
✅ **DO**: Avoid blocking calls
✅ **DO**: Optimize for common cases
✅ **DO**: Profile performance-critical code

### **Security**

✅ **DO**: Validate all external inputs
✅ **DO**: Use parameterized queries (if applicable)
✅ **DO**: Avoid hardcoding secrets
✅ **DO**: Follow security best practices
✅ **DO**: Report security issues privately

---

## 📊 **Metrics**

### **Code Quality Metrics**

| Metric | Target | Tool |
|--------|--------|------|
| **Code Coverage** | > 85% | dotnet test --collect:"XPlat Code Coverage" |
| **Code Complexity** | < 10 per method | Visual Studio Code Metrics |
| **Maintainability Index** | > 80 | Visual Studio Code Metrics |
| **Lines per Method** | < 50 | Manual review |

---

## 🌍 **Internationalization**

### **Adding New Language**

1. **Create Resource File**
```
Resources/Messages.{culture}.resx
Example: Resources/Messages.fr-FR.resx
```

2. **Translate All Strings**
```xml
<data name="CONVERSION_SUCCESS" xml:space="preserve">
  <value>Conversion réussie</value>
</data>
```

3. **Update LocalizationManager**
```csharp
public static readonly string[] SupportedLanguages = 
{
    "en-US", "pt-BR", "es-ES", "fr-FR", "de-DE", 
    "it-IT", "ja-JP", "zh-CN", "fr-FR"  // Add new language
};
```

4. **Test All UI Messages**
```bash
ZPL2PDF --set-language fr-FR
ZPL2PDF --help
```

---

## 📚 **Related Documentation**

- 🛠️ [Development Environment Setup](../setup/development-environment.md)
- 🏗️ [Architecture Overview](../architecture.md)
- 🔨 [Build Process](../setup/build-process.md)
- 🧪 [Testing Guide](../setup/testing.md)
- 🌿 [Git Workflow](../workflows/git-workflow.md)
- 🚀 [Release Process](../workflows/releases.md)

---

**Following these guidelines ensures high-quality, maintainable contributions to ZPL2PDF!** 🚀
