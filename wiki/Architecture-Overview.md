# 🏗️ Architecture Overview

Understanding ZPL2PDF's layered architecture and design principles.

## 🎯 Design Philosophy

ZPL2PDF is organized in layers inspired by **Clean Architecture**:
- ✅ **Separation of Concerns** - Clear layer boundaries
- ✅ **Abstractions in Domain/Application** - Interfaces for renderers, PDF generation and validation
- ✅ **Testability** - Services are covered by unit and integration tests
- ✅ **Cross-Platform** - Works on Windows, Linux, macOS

> Note: services are currently wired by direct instantiation (e.g. `new ConversionService()`), not by a DI container. The interfaces exist mainly for testing and future extension.

---

## 📁 Project Structure

```
src/
├── Application/           # Use cases & application logic
│   ├── Services/
│   │   ├── ConversionService.cs
│   │   ├── FileValidationService.cs
│   │   ├── PathService.cs
│   │   └── UnitConversionService.cs
│   └── Interfaces/
│       ├── IConversionService.cs
│       ├── IDaemonService.cs
│       ├── IFileProcessingService.cs
│       ├── IFileValidationService.cs
│       ├── IPathService.cs
│       └── IUnitConversionService.cs
│
├── Domain/                # Contracts & value objects
│   ├── Services/          # Domain interfaces
│   │   ├── ILabelRenderer.cs
│   │   ├── IPdfGenerator.cs
│   │   ├── IFileValidator.cs
│   │   └── IDimensionExtractor.cs
│   └── ValueObjects/
│       ├── ConversionOptions.cs
│       ├── DaemonConfiguration.cs
│       ├── FileInfo.cs
│       └── ProcessingResult.cs
│
├── Infrastructure/        # I/O and external dependencies
│   ├── Configuration/
│   │   └── ConfigManager.cs
│   ├── FileSystem/
│   │   └── FolderMonitor.cs
│   ├── Processing/
│   │   └── ProcessingQueue.cs
│   ├── Rendering/
│   │   ├── LabelRenderer.cs      # Offline (BinaryKits + SkiaSharp)
│   │   ├── LabelaryRenderer.cs   # Labelary online API
│   │   ├── AutoRenderer.cs       # Labelary first, offline fallback
│   │   └── PdfGenerator.cs       # PNG pages -> PDF (PDFsharp)
│   ├── TcpServer/
│   │   └── TcpPrinterServer.cs   # Virtual network printer
│   ├── DaemonManager.cs
│   ├── PidManager.cs
│   └── ProcessManager.cs
│
├── Presentation/          # CLI, REST API and mode handlers
│   ├── Api/Models/        # ConvertRequest / ConvertResponse (REST API)
│   ├── Program.cs         # Entry point + REST API (Minimal API)
│   ├── ModeDetector.cs
│   ├── ArgumentParser.cs
│   ├── ArgumentProcessor.cs
│   ├── ArgumentValidator.cs
│   ├── ConversionModeHandler.cs
│   ├── DaemonModeHandler.cs
│   ├── TcpServerModeHandler.cs
│   └── HelpDisplay.cs
│
└── Shared/                # Common utilities
    ├── Constants/         # Includes RendererEngine enum
    ├── Localization/
    ├── DefaultSettings.cs
    ├── LabelFileReader.cs       # ZPL preprocessing & label splitting
    └── ZplDimensionExtractor.cs
```

---

## 🚦 Operation Modes

`Program.cs` picks the mode; `ModeDetector` decides between the CLI modes.

| Mode | How to start | Handler |
|------|--------------|---------|
| **Conversion** | `ZPL2PDF -i file.zpl -o out/` or `-z "<zpl>"` | `ConversionModeHandler` |
| **Daemon** (watch folder) | `ZPL2PDF start` / `stop` / `status` / `run` (no args = start) | `DaemonModeHandler` |
| **TCP server** (virtual printer) | `ZPL2PDF server start\|stop\|status -o out/` (default port `9101`) | `TcpServerModeHandler` → `TcpPrinterServer` |
| **REST API** | `ZPL2PDF --api` (alias `--web`), `--host`, `--port` (default `0.0.0.0:5000`) | `Program.StartApiMode` |
| **Help** | `-help` or unclear arguments | `HelpDisplay` |

REST API endpoints: `POST /api/convert` (JSON body `zpl` or `zplArray`, `format` = `pdf`\|`png`, `renderer`, `width`, `height`, `unit`, `dpi`) and `GET /api/health`. See [API guide](../docs/guides/API_GUIDE.md).

---

## 🔄 Data Flow

### Conversion Mode
```
User Input
    ↓
ArgumentProcessor → ArgumentValidator
    ↓
ConversionModeHandler
    ↓
ConversionService
    ├─ LabelFileReader.PreprocessZpl / SplitLabels
    ├─ ZplDimensionExtractor (^PW / ^LL)
    ├─ TryConvertPdfDirectWithLabelary (labelary / auto)  → PDF bytes
    └─ ILabelRenderer (offline / labelary / auto)         → PNG pages
            ↓
        PdfGenerator
    ↓
Output PDF (file or --stdout)
```

### Daemon Mode
```
DaemonModeHandler → DaemonManager
    ↓
FolderMonitor (watch files)
    ↓
ProcessingQueue → FileValidationService
    ↓
ILabelRenderer → PdfGenerator
    ↓
Output PDFs
```

### TCP Server and REST API
Both reuse `ConversionService` (and therefore the same renderers and the Labelary PDF shortcut) and write/return PDF or PNG output.

---

## 🖨️ Renderers

The renderer is chosen with `--renderer offline|labelary|auto` (CLI) or the `renderer` field (REST API). The enum is `RendererEngine` in `Shared/Constants`.

| Engine | Class | Behavior |
|--------|-------|----------|
| `offline` (default) | `LabelRenderer` | BinaryKits.Zpl.Viewer + SkiaSharp; no network. Supports custom fonts (`--fonts-dir`, `--font`). |
| `labelary` | `LabelaryRenderer` | Labelary online API. Can return a PDF directly (`RenderPdf`) or PNG per label. Errors are thrown. |
| `auto` | `AutoRenderer` | Tries Labelary per label and falls back to the offline renderer on failure. |

Labelary only accepts 6, 8, 12 or 24 dpmm; `LabelaryRenderer` maps the requested DPI to the closest one.

---

## 🧩 Core Components

### Application Layer
- `ConversionService`: single conversion pipeline shared by CLI, REST API, TCP server and daemon (preprocess → split → dimensions → render, plus the direct Labelary PDF path).
- `FileValidationService`: file extension/content checks.
- `UnitConversionService`: unit conversions (mm, cm, in).
- `PathService`: path management.

### Domain Layer
- Contracts: `ILabelRenderer`, `IPdfGenerator`, `IFileValidator`, `IDimensionExtractor`.
- Value objects: `ConversionOptions`, `ProcessingResult`, `FileInfo`, `DaemonConfiguration`.

### Infrastructure Layer
- Renderers and `PdfGenerator` (see above), `FolderMonitor`, `ProcessingQueue`, `ConfigManager`, `DaemonManager`/`PidManager`/`ProcessManager`, `TcpPrinterServer`.

### Presentation Layer
- Argument parsing/validation, mode detection and handlers, help, REST API endpoints, multi-language messages.

### Shared
- `LabelFileReader`: ZPL preprocessing (`^FN` removal, `^B0` → `^BO` workaround for the offline viewer, `^FH` hex decoding) and label splitting with `~DGR`/`^IDR` graphic state.
- `ZplDimensionExtractor`: dimension priority is explicit arguments > ZPL (`^PW`/`^LL`) > defaults.
- `Localization`: resource-based messages in `Resources/Messages.*.resx` (8 languages).

---

## 🌍 Cross-Platform Support

Use cross-platform path APIs and avoid OS-specific code unless guarded:

```csharp
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { /* ... */ }
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { /* ... */ }
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { /* ... */ }

var defaultFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "ZPL2PDF Auto Converter");
```

---

## 🔐 Security Considerations

- Validate all user inputs and file extensions
- Sanitize file paths and prevent path traversal
- Regexes use timeouts to avoid ReDoS on large inputs
- Prefer read-only mounts for watch folders in Docker
- The REST API allows any CORS origin and binds to `0.0.0.0` by default; restrict `--host` / network exposure in production

---

## 🔗 External Dependencies

### NuGet Packages
- `BinaryKits.Zpl.Label` / `BinaryKits.Zpl.Viewer` - offline ZPL parsing and drawing (SkiaSharp-based)
- `PDFsharp` - PDF generation
- ASP.NET Core shared framework (`Microsoft.AspNetCore.App`) - REST API

### External APIs
- **Labelary API** (`api.labelary.com`) - ZPL rendering service used by the `labelary` and `auto` engines (requires internet; the offline engine needs none).

---

## 🔗 Related Topics

- [[Development Setup]] - Setting up development environment
- [[Contributing Guidelines]] - How to contribute
- [[Testing Guide]] - Testing strategies
- [[Build Process]] - Building the project
