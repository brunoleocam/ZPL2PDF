# 🏗️ Architecture Overview

Complete architectural overview of ZPL2PDF following Clean Architecture principles.

---

## 🎯 **Architectural Vision**

ZPL2PDF follows **Clean Architecture** (Onion Architecture) principles to ensure:

- ✅ **Separation of Concerns** - Clear boundaries between layers
- ✅ **Testability** - Easy to unit test and mock dependencies
- ✅ **Maintainability** - Changes isolated to specific layers
- ✅ **Scalability** - Easy to extend with new features
- ✅ **Independence** - Domain logic independent of frameworks

---

## 📐 **Layered Architecture**

```
┌─────────────────────────────────────────────────────┐
│              Presentation Layer                      │
│  ┌─────────────────────────────────────────────┐   │
│  │  CLI Interface, Argument Processing          │   │
│  │  Mode Handlers (Conversion, Daemon)          │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│             Application Layer                        │
│  ┌─────────────────────────────────────────────┐   │
│  │  Use Cases & Application Services            │   │
│  │  - ConversionService                         │   │
│  │  - DaemonService                             │   │
│  │  - FileProcessingService                     │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│               Domain Layer                           │
│  ┌─────────────────────────────────────────────┐   │
│  │  Business Logic & Domain Services            │   │
│  │  - Value Objects (LabelDimensions, etc)      │   │
│  │  - Domain Interfaces                         │   │
│  │  - Business Rules                            │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│            Infrastructure Layer                      │
│  ┌─────────────────────────────────────────────┐   │
│  │  External Concerns & Implementations         │   │
│  │  - File System (monitoring, validation)      │   │
│  │  - Rendering (ZPL → Image)                   │   │
│  │  - PDF Generation                            │   │
│  │  - Configuration Management                  │   │
│  └─────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

---

## 📁 **Project Structure**

```
ZPL2PDF/
├── src/
│   ├── Presentation/           # CLI & User Interface
│   │   ├── Program.cs          # Entry point
│   │   ├── ArgumentProcessor.cs # Command-line parsing
│   │   ├── ArgumentValidator.cs # Argument validation
│   │   └── Handlers/           # Mode handlers
│   │       ├── ConversionModeHandler.cs
│   │       └── DaemonModeHandler.cs
│   │
│   ├── Application/            # Use Cases & Services
│   │   ├── Services/           # Application services
│   │   │   ├── ConversionService.cs
│   │   │   ├── DaemonService.cs
│   │   │   └── FileProcessingService.cs
│   │   └── Interfaces/         # Service contracts
│   │       ├── IConversionService.cs
│   │       ├── IDaemonService.cs
│   │       └── IFileProcessingService.cs
│   │
│   ├── Domain/                 # Business Logic
│   │   ├── ValueObjects/       # Immutable objects
│   │   │   ├── LabelDimensions.cs
│   │   │   ├── ConversionOptions.cs
│   │   │   ├── ProcessingResult.cs
│   │   │   └── DaemonConfiguration.cs
│   │   └── Services/           # Domain interfaces
│   │       ├── ILabelRenderer.cs
│   │       ├── IPdfGenerator.cs
│   │       ├── IFileValidator.cs
│   │       └── IDimensionExtractor.cs
│   │
│   ├── Infrastructure/         # External Dependencies
│   │   ├── FileSystem/         # File operations
│   │   │   ├── FileSystemWatcher.cs
│   │   │   ├── FileValidator.cs
│   │   │   └── PathService.cs
│   │   ├── Rendering/          # ZPL rendering
│   │   │   ├── LabelRenderer.cs
│   │   │   └── PdfGenerator.cs
│   │   ├── Processing/         # Queue management
│   │   │   └── ProcessingQueue.cs
│   │   └── Configuration/      # Config management
│   │       └── ConfigManager.cs
│   │
│   └── Shared/                 # Common Utilities
│       ├── Localization/       # Multi-language
│       │   ├── LocalizationManager.cs
│       │   └── LanguageConfigManager.cs
│       ├── Constants/          # Configuration
│       │   ├── DefaultSettings.cs
│       │   └── DefaultConfigurations.cs
│       └── Extensions/         # Helper methods
│           └── StringExtensions.cs
│
├── tests/
│   ├── ZPL2PDF.Unit/           # Unit tests
│   ├── ZPL2PDF.Integration/    # Integration tests
│   └── ZPL2PDF.Performance/    # Performance tests
│
├── installer/                   # Windows installer
├── scripts/                     # Build scripts
└── docs/                        # Documentation
```

---

## 🔄 **Data Flow**

### **Conversion Mode Flow**

```
User Input (CLI)
        ↓
ArgumentProcessor
        ↓
ArgumentValidator
        ↓
ConversionModeHandler
        ↓
ConversionService
        ↓
┌──────────────────────────────────┐
│  1. FileValidator                 │
│     - Validate input file         │
│     - Check permissions           │
└──────────────────────────────────┘
        ↓
┌──────────────────────────────────┐
│  2. DimensionExtractor            │
│     - Read ZPL commands           │
│     - Extract ^PW, ^LL            │
│     - Apply priority logic        │
└──────────────────────────────────┘
        ↓
┌──────────────────────────────────┐
│  3. LabelRenderer                 │
│     - Parse ZPL content           │
│     - Render to image             │
│     - Apply dimensions            │
└──────────────────────────────────┘
        ↓
┌──────────────────────────────────┐
│  4. PdfGenerator                  │
│     - Create PDF document         │
│     - Add image to PDF            │
│     - Save to output folder       │
└──────────────────────────────────┘
        ↓
ProcessingResult
        ↓
Console Output (Success/Error)
```

### **Daemon Mode Flow**

```
User Input (CLI: start)
        ↓
ArgumentProcessor
        ↓
DaemonModeHandler
        ↓
DaemonService
        ↓
┌──────────────────────────────────┐
│  1. Create PID File               │
│     - Check if daemon running     │
│     - Write process ID            │
└──────────────────────────────────┘
        ↓
┌──────────────────────────────────┐
│  2. Start FileSystemWatcher       │
│     - Monitor watch folder        │
│     - Detect new files            │
└──────────────────────────────────┘
        ↓
New File Detected
        ↓
┌──────────────────────────────────┐
│  3. ProcessingQueue               │
│     - Add file to queue           │
│     - Check if file is locked     │
│     - Retry mechanism             │
└──────────────────────────────────┘
        ↓
┌──────────────────────────────────┐
│  4. FileProcessingService         │
│     - Read ZPL content            │
│     - Extract dimensions          │
│     - Convert to PDF              │
└──────────────────────────────────┘
        ↓
Continue Monitoring...
```

---

## 🎯 **Design Patterns**

### **Dependency Injection**

```csharp
// Service registration
services.AddScoped<IConversionService, ConversionService>();
services.AddScoped<IDaemonService, DaemonService>();
services.AddScoped<ILabelRenderer, LabelRenderer>();
services.AddScoped<IPdfGenerator, PdfGenerator>();

// Constructor injection
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

### **Factory Pattern**

```csharp
public interface ILabelDimensionsFactory
{
    LabelDimensions Create(
        double? width, 
        double? height, 
        string unit, 
        string zplContent);
}

public class LabelDimensionsFactory : ILabelDimensionsFactory
{
    public LabelDimensions Create(...)
    {
        // Priority logic:
        // 1. Explicit parameters
        // 2. Extract from ZPL
        // 3. Use defaults
    }
}
```

### **Strategy Pattern**

```csharp
public interface IConversionStrategy
{
    Task<ProcessingResult> ConvertAsync(ConversionOptions options);
}

public class FileConversionStrategy : IConversionStrategy { }
public class StringConversionStrategy : IConversionStrategy { }
```

### **Observer Pattern**

```csharp
public interface IFileWatcherObserver
{
    void OnFileCreated(string filePath);
    void OnFileChanged(string filePath);
}

public class FileSystemWatcher : IFileWatcher
{
    private readonly List<IFileWatcherObserver> _observers = new();
    
    public void Subscribe(IFileWatcherObserver observer)
    {
        _observers.Add(observer);
    }
    
    private void NotifyObservers(string filePath)
    {
        foreach (var observer in _observers)
        {
            observer.OnFileCreated(filePath);
        }
    }
}
```

---

## 🔐 **SOLID Principles**

### **Single Responsibility Principle (SRP)**

Each class has **one reason to change**:

- `FileValidator` - Only validates files
- `LabelRenderer` - Only renders ZPL to image
- `PdfGenerator` - Only generates PDFs
- `ConfigManager` - Only manages configuration

### **Open/Closed Principle (OCP)**

Open for extension, closed for modification:

```csharp
// Interface allows new implementations without modifying existing code
public interface ILabelRenderer
{
    Task<Image> RenderAsync(string zplContent, LabelDimensions dimensions);
}

// Can add new renderers without changing existing code
public class SkiaSharpRenderer : ILabelRenderer { }
public class GdiPlusRenderer : ILabelRenderer { }
```

### **Liskov Substitution Principle (LSP)**

Derived classes substitutable for base classes:

```csharp
// All implementations can be used interchangeably
IConversionService service = new ConversionService(...);
// OR
IConversionService service = new BatchConversionService(...);
```

### **Interface Segregation Principle (ISP)**

Specific interfaces instead of general-purpose:

```csharp
// Instead of one large interface:
public interface IFileService
{
    void ValidateFile(string path);
    void WatchFolder(string path);
    void ReadFile(string path);
}

// Use specific interfaces:
public interface IFileValidator { void Validate(string path); }
public interface IFileWatcher { void Watch(string path); }
public interface IFileReader { string Read(string path); }
```

### **Dependency Inversion Principle (DIP)**

Depend on abstractions, not concretions:

```csharp
// High-level module depends on abstraction
public class ConversionService : IConversionService
{
    private readonly ILabelRenderer _renderer;  // Abstraction
    
    public ConversionService(ILabelRenderer renderer)
    {
        _renderer = renderer;  // Not concrete implementation
    }
}
```

---

## 📊 **Component Diagram**

```
┌─────────────────────────────────────────────────────┐
│                 ZPL2PDF Application                  │
│                                                      │
│  ┌────────────────┐          ┌────────────────┐    │
│  │ Conversion     │          │ Daemon         │    │
│  │ Mode Handler   │          │ Mode Handler   │    │
│  └────────┬───────┘          └────────┬───────┘    │
│           │                           │             │
│           ├───────────┬───────────────┤             │
│           │           │               │             │
│  ┌────────▼───────┐  │  ┌────────────▼──────┐     │
│  │ Conversion     │  │  │ File Processing   │     │
│  │ Service        │  │  │ Service           │     │
│  └────────┬───────┘  │  └────────────┬──────┘     │
│           │          │               │             │
│  ┌────────▼──────────▼───────────────▼──────┐     │
│  │        Domain Services Layer              │     │
│  │  ┌──────────┐  ┌──────────┐  ┌────────┐ │     │
│  │  │ Label    │  │ PDF      │  │ File   │ │     │
│  │  │ Renderer │  │Generator │  │Validator│ │     │
│  │  └──────────┘  └──────────┘  └────────┘ │     │
│  └──────────────────────────────────────────┘     │
│                                                      │
│  ┌──────────────────────────────────────────┐     │
│  │     Infrastructure Layer                 │     │
│  │  ┌──────────┐  ┌──────────┐  ┌────────┐ │     │
│  │  │BinaryKits│  │PdfSharp  │  │File    │ │     │
│  │  │  .Zpl    │  │  Core    │  │ System │ │     │
│  │  └──────────┘  └──────────┘  └────────┘ │     │
│  └──────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────┘
```

---

## 🔍 **Key Components**

### **1. ConversionService**

**Responsibility:** Orchestrate ZPL to PDF conversion

**Dependencies:**
- `ILabelRenderer` - Render ZPL to image
- `IPdfGenerator` - Generate PDF from image
- `IFileValidator` - Validate input files

### **2. DaemonService**

**Responsibility:** Manage background daemon process

**Dependencies:**
- `IFileSystemWatcher` - Monitor folder
- `IProcessingQueue` - Queue file processing
- `IFileProcessingService` - Process queued files

### **3. LabelRenderer**

**Responsibility:** Render ZPL content to image

**Dependencies:**
- `BinaryKits.Zpl.Label` - ZPL parsing
- `SkiaSharp` - Cross-platform graphics

### **4. PdfGenerator**

**Responsibility:** Generate PDF from rendered image

**Dependencies:**
- `PdfSharpCore` - PDF creation

---

## 🚀 **Extensibility**

### **Adding New Features**

#### **Example: Add New Output Format (PNG)**

1. **Create Interface:**
```csharp
public interface IImageExporter
{
    Task ExportAsync(Image image, string outputPath);
}
```

2. **Implement Service:**
```csharp
public class PngExporter : IImageExporter
{
    public async Task ExportAsync(Image image, string outputPath)
    {
        // Implementation
    }
}
```

3. **Register Service:**
```csharp
services.AddScoped<IImageExporter, PngExporter>();
```

4. **Use in ConversionService:**
```csharp
public class ConversionService
{
    private readonly ILabelRenderer _renderer;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IImageExporter _imageExporter;  // New dependency
    
    // ...
}
```

---

## 📚 **Related Documentation**

- 🛠️ [Development Environment Setup](setup/development-environment.md)
- 🔨 [Build Process](setup/build-process.md)
- 🧪 [Testing Guide](setup/testing.md)
- 🤝 [Contributing Guide](../../CONTRIBUTING.md)

---

**Clean Architecture ensures ZPL2PDF remains maintainable and extensible for future enhancements!** 🚀
