# 🏗️ Architecture Overview

Understanding ZPL2PDF's Clean Architecture and design principles.

## 🎯 Design Philosophy

ZPL2PDF follows **Clean Architecture** principles:
- ✅ **Separation of Concerns** - Clear layer boundaries
- ✅ **Dependency Inversion** - Depend on abstractions
- ✅ **SOLID Principles** - Maintainable, extensible code
- ✅ **Testability** - Easy to unit test
- ✅ **Cross-Platform** - Works on Windows, Linux, macOS

---

## 📁 Project Structure

```
src/
├── Application/           # Use Cases & Application Logic
│   ├── Services/
│   │   ├── ConversionService.cs
│   │   ├── FileValidationService.cs
│   │   ├── PathService.cs
│   │   └── UnitConversionService.cs
│   └── Interfaces/
│       ├── IConversionService.cs
│       ├── IDaemonService.cs
│       └── IFileProcessingService.cs
│
├── Domain/               # Business Logic & Rules
│   ├── Services/        # Domain Interfaces
│   │   ├── ILabelRenderer.cs
│   │   ├── IPdfGenerator.cs
│   │   ├── IFileValidator.cs
│   │   └── IDimensionExtractor.cs
│   └── ValueObjects/    # Immutable Value Objects
│       ├── ConversionOptions.cs
│       ├── LabelDimensions.cs
│       ├── FileInfo.cs
│       ├── ProcessingResult.cs
│       └── DaemonConfiguration.cs
│
├── Infrastructure/       # External Dependencies
│   ├── FileSystem/
│   │   ├── FolderMonitor.cs
│   │   └── FileValidator.cs
│   ├── Configuration/
│   │   └── ConfigManager.cs
│   ├── Processing/
│   │   └── ProcessingQueue.cs
│   ├── Rendering/
│   │   ├── LabelRenderer.cs
│   │   └── PdfGenerator.cs
│   ├── DaemonManager.cs
│   └── PidManager.cs
│
├── Presentation/         # User Interface (CLI)
│   ├── Program.cs
│   ├── ArgumentProcessor.cs
│   ├── ArgumentValidator.cs
│   ├── ConversionModeHandler.cs
│   ├── DaemonModeHandler.cs
│   └── HelpDisplay.cs
│
└── Shared/              # Common Utilities
    ├── Constants/
    ├── Localization/
    ├── DefaultSettings.cs
    └── ZplDimensionExtractor.cs
```

---

## 🔄 Data Flow

### Conversion Mode Flow
```
User Input
    ↓
ArgumentProcessor → ArgumentValidator
    ↓
ConversionModeHandler
    ↓
ConversionService
    ↓
LabelRenderer → PdfGenerator
    ↓
Output PDF
```

### Daemon Mode Flow
```
User Start Command
    ↓
DaemonModeHandler → DaemonManager
    ↓
FolderMonitor (watch files)
    ↓
ProcessingQueue (queue files)
    ↓
FileValidationService
    ↓
ConversionService
    ↓
LabelRenderer → PdfGenerator
    ↓
Output PDFs
```

---

## 🧩 Core Components

### 1. Application Layer
**Purpose**: Orchestrate use cases and business workflows

**Key Services**:
- `ConversionService`: Centralized conversion logic
- `FileValidationService`: File validation and checking
- `UnitConversionService`: Unit conversions (mm, cm, in)
- `PathService`: Path management and validation

### 2. Domain Layer
**Purpose**: Business logic and domain rules

**Key Concepts**:
- `LabelDimensions`: Width, height, unit, DPI
- `ConversionOptions`: User-specified options
- `ProcessingResult`: Conversion outcomes
- Domain interfaces (no implementations)

### 3. Infrastructure Layer
**Purpose**: External dependencies and I/O

**Key Components**:
- `LabelRenderer`: ZPL rendering using Labelary API
- `PdfGenerator`: PDF document generation
- `FolderMonitor`: File system watching
- `ConfigManager`: Configuration management

### 4. Presentation Layer
**Purpose**: User interaction (CLI)

**Key Features**:
- Argument parsing and validation
- Mode detection (conversion vs daemon)
- Help display and error messages
- Multi-language support

---

## 🔑 Key Design Patterns

### Dependency Injection
```csharp
public class ConversionService : IConversionService
{
    private readonly ILabelRenderer _renderer;
    private readonly IPdfGenerator _pdfGenerator;
    
    public ConversionService(
        ILabelRenderer renderer,
        IPdfGenerator pdfGenerator)
    {
        _renderer = renderer;
        _pdfGenerator = pdfGenerator;
    }
}
```

### Value Objects
```csharp
public record LabelDimensions(
    double Width,
    double Height,
    string Unit,
    int Dpi)
{
    public double WidthInMm => ConvertToMm(Width, Unit);
    public double HeightInMm => ConvertToMm(Height, Unit);
}
```

### Strategy Pattern
```csharp
// Different conversion strategies
public interface IConversionStrategy
{
    ProcessingResult Convert(ConversionOptions options);
}

public class FileConversionStrategy : IConversionStrategy { }
public class StringConversionStrategy : IConversionStrategy { }
```

---

## 🌍 Cross-Platform Support

### Platform Detection
```csharp
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    // Windows-specific code
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    // Linux-specific code
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    // macOS-specific code
}
```

### Path Handling
```csharp
// Use cross-platform path APIs
var defaultFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "ZPL2PDF Auto Converter"
);
```

---

## 🔐 Security Considerations

### Input Validation
- Validate all user inputs
- Sanitize file paths
- Check file permissions
- Validate ZPL content

### File System Security
- Use read-only mounts when possible
- Validate file extensions
- Check file size limits
- Prevent path traversal attacks

---

## ⚡ Performance Optimizations

### Async Processing
```csharp
public async Task<ProcessingResult> ConvertAsync(
    ConversionOptions options,
    CancellationToken cancellationToken)
{
    // Async file I/O
    var zplContent = await File.ReadAllTextAsync(
        options.InputPath,
        cancellationToken
    );
    
    // Async HTTP requests
    var renderedImage = await _renderer.RenderAsync(
        zplContent,
        options.Dimensions
    );
    
    return result;
}
```

### Caching
- Cache dimension extraction results
- Cache compiled regex patterns
- Cache configuration settings

---

## 🔗 External Dependencies

### NuGet Packages
- `PdfSharp` - PDF generation
- `System.Drawing.Common` - Image processing
- `Microsoft.Extensions.Configuration` - Configuration
- `Microsoft.Extensions.Logging` - Logging

### External APIs
- **Labelary API** - ZPL rendering service
  - Converts ZPL to PNG
  - Free tier available
  - Fallback to local rendering

---

## 🔗 Related Topics

- [[Development Setup]] - Setting up development environment
- [[Contributing Guidelines]] - How to contribute
- [[Testing Guide]] - Testing strategies
- [[Build Process]] - Building the project
