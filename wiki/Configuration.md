# ⚙️ Configuration

Complete guide for configuring ZPL2PDF settings and preferences.

## 🎯 Overview

ZPL2PDF can be configured through:
- ✅ **Configuration files** - Persistent settings
- ✅ **Environment variables** - System-wide defaults
- ✅ **Command-line parameters** - Per-execution settings
- ✅ **Multi-language support** - Interface localization

---

## 📄 Configuration File

### File Locations
| Platform | Default Location |
|----------|-----------------|
| **Windows** | Same folder as executable |
| **Linux** | `~/.config/zpl2pdf/zpl2pdf.json` |
| **macOS** | `~/Library/Application Support/ZPL2PDF/zpl2pdf.json` |

### Configuration Format
Create `zpl2pdf.json`:
```json
{
  "language": "pt-BR",
  "defaultWatchFolder": "C:\\WatchFolder",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3,
  "autoDeleteProcessed": false,
  "outputSubfolders": true,
  "dateTimeFormat": "yyyy-MM-dd_HH-mm-ss"
}
```

### Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `language` | string | `"en-US"` | Default interface language |
| `defaultWatchFolder` | string | System default | Default folder for daemon mode |
| `labelWidth` | number | `60` | Default label width in mm |
| `labelHeight` | number | `120` | Default label height in mm |
| `unit` | string | `"mm"` | Default unit (mm, cm, in) |
| `dpi` | number | `203` | Default print density |
| `logLevel` | string | `"Info"` | Logging level (Debug, Info, Warning, Error) |
| `retryDelay` | number | `2000` | Delay between retries (ms) |
| `maxRetries` | number | `3` | Maximum retry attempts |
| `autoDeleteProcessed` | boolean | `false` | Delete files after processing |
| `outputSubfolders` | boolean | `true` | Create date-based subfolders |
| `dateTimeFormat` | string | `"yyyy-MM-dd_HH-mm-ss"` | DateTime format for subfolders |

---

## 🌐 Environment Variables

### Language Configuration
```bash
# Set default language
export ZPL2PDF_LANGUAGE=pt-BR  # Linux/macOS
set ZPL2PDF_LANGUAGE=pt-BR     # Windows

# Supported languages
export ZPL2PDF_LANGUAGE=en-US  # English
export ZPL2PDF_LANGUAGE=pt-BR  # Portuguese
export ZPL2PDF_LANGUAGE=es-ES  # Spanish
export ZPL2PDF_LANGUAGE=fr-FR  # French
export ZPL2PDF_LANGUAGE=de-DE  # German
export ZPL2PDF_LANGUAGE=it-IT  # Italian
export ZPL2PDF_LANGUAGE=ja-JP  # Japanese
export ZPL2PDF_LANGUAGE=zh-CN  # Chinese
```

### Logging Configuration
```bash
# Set log level
export ZPL2PDF_LOG_LEVEL=Debug  # Linux/macOS
set ZPL2PDF_LOG_LEVEL=Debug     # Windows

# Available levels
export ZPL2PDF_LOG_LEVEL=Debug   # Detailed debugging
export ZPL2PDF_LOG_LEVEL=Info    # General information
export ZPL2PDF_LOG_LEVEL=Warning # Warnings only
export ZPL2PDF_LOG_LEVEL=Error   # Errors only
```

### Folder Configuration
```bash
# Set default watch folder
export ZPL2PDF_WATCH_FOLDER="/var/zpl2pdf/watch"  # Linux/macOS
set ZPL2PDF_WATCH_FOLDER=C:\WatchFolder           # Windows

# Set default output folder
export ZPL2PDF_OUTPUT_FOLDER="/var/zpl2pdf/output"  # Linux/macOS
set ZPL2PDF_OUTPUT_FOLDER=C:\OutputFolder           # Windows
```

### Performance Configuration
```bash
# Set retry delay (milliseconds)
export ZPL2PDF_RETRY_DELAY=5000  # Linux/macOS
set ZPL2PDF_RETRY_DELAY=5000     # Windows

# Set maximum retries
export ZPL2PDF_MAX_RETRIES=5     # Linux/macOS
set ZPL2PDF_MAX_RETRIES=5        # Windows
```

---

## 🎨 Default Settings

### Dimension Defaults
```json
{
  "labelWidth": 60,    // mm
  "labelHeight": 120,  // mm
  "unit": "mm",
  "dpi": 203
}
```

### Unit Conversion Factors
| Unit | Factor | Example |
|------|--------|---------|
| **mm** | 1.0 | 60mm = 60mm |
| **cm** | 10.0 | 6cm = 60mm |
| **in** | 25.4 | 2.36in = 60mm |

### Quality Settings
| DPI | Quality | Use Case |
|-----|---------|----------|
| **150** | Low | Fast processing, draft quality |
| **203** | Standard | Default, balanced quality/speed |
| **300** | High | High quality, slower processing |
| **600** | Premium | Maximum quality, slow processing |

---

## 🔧 Advanced Configuration

### Custom Output Naming
```json
{
  "outputNaming": {
    "prefix": "LABEL_",
    "suffix": "_CONVERTED",
    "useTimestamp": true,
    "timestampFormat": "yyyyMMdd_HHmmss"
  }
}
```

### Folder Structure Configuration
```json
{
  "folderStructure": {
    "createDateSubfolders": true,
    "dateFormat": "yyyy-MM-dd",
    "createTypeSubfolders": true,
    "typeMapping": {
      "txt": "TextFiles",
      "prn": "PrintFiles"
    }
  }
}
```

### Processing Configuration
```json
{
  "processing": {
    "batchSize": 10,
    "parallelProcessing": false,
    "memoryLimit": "512MB",
    "timeoutSeconds": 300
  }
}
```

---

## 🏢 Enterprise Configuration

### Server Configuration
```json
{
  "server": {
    "host": "0.0.0.0",
    "port": 8080,
    "apiKey": "your-api-key",
    "ssl": {
      "enabled": true,
      "certificate": "/path/to/cert.pem",
      "privateKey": "/path/to/key.pem"
    }
  }
}
```

### Database Configuration
```json
{
  "database": {
    "provider": "sqlite",
    "connectionString": "Data Source=zpl2pdf.db",
    "logging": {
      "enabled": true,
      "retentionDays": 30
    }
  }
}
```

### Monitoring Configuration
```json
{
  "monitoring": {
    "metrics": {
      "enabled": true,
      "interval": 60,
      "endpoint": "http://localhost:9090/metrics"
    },
    "healthChecks": {
      "enabled": true,
      "interval": 30,
      "timeout": 10
    }
  }
}
```

---

## 🔄 Configuration Priority

Configuration is applied in the following order (highest to lowest priority):

1. **Command-line parameters** - Override all other settings
2. **Environment variables** - System-wide defaults
3. **Configuration file** - Persistent settings
4. **Built-in defaults** - Fallback values

### Example Priority
```bash
# 1. Command-line (highest priority)
ZPL2PDF.exe -w 10 -h 15 -u cm -d 300

# 2. Environment variable
export ZPL2PDF_LANGUAGE=pt-BR

# 3. Configuration file
{
  "language": "en-US",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203
}

# 4. Built-in defaults (lowest priority)
# language: "en-US", width: 60mm, height: 120mm, unit: "mm", dpi: 203
```

---

## 📊 Configuration Validation

### Validate Configuration File
```bash
# Check configuration syntax
ZPL2PDF.exe --validate-config

# Expected output:
# Configuration file is valid
# or
# Configuration error: Invalid value for 'dpi': must be between 150 and 600
```

### Configuration Test
```bash
# Test configuration without processing files
ZPL2PDF.exe --test-config

# Expected output:
# Testing configuration...
# Language: Portuguese (pt-BR)
# Default dimensions: 7.5in x 15in
# DPI: 203
# Watch folder: C:\WatchFolder
# Configuration test passed
```

---

## 🔍 Configuration Examples

### Development Environment
```json
{
  "language": "en-US",
  "logLevel": "Debug",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "retryDelay": 1000,
  "maxRetries": 1
}
```

### Production Environment
```json
{
  "language": "pt-BR",
  "logLevel": "Info",
  "defaultWatchFolder": "/var/zpl2pdf/watch",
  "labelWidth": 100,
  "labelHeight": 200,
  "unit": "mm",
  "dpi": 300,
  "retryDelay": 5000,
  "maxRetries": 5,
  "autoDeleteProcessed": true,
  "outputSubfolders": true
}
```

### Docker Environment
```json
{
  "language": "en-US",
  "logLevel": "Info",
  "defaultWatchFolder": "/app/watch",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "retryDelay": 2000,
  "maxRetries": 3
}
```

---

## 🔧 Configuration Management

### Backup Configuration
```bash
# Backup current configuration
cp zpl2pdf.json zpl2pdf.json.backup  # Linux/macOS
copy zpl2pdf.json zpl2pdf.json.backup  # Windows
```

### Reset Configuration
```bash
# Reset to defaults
rm zpl2pdf.json  # Linux/macOS
del zpl2pdf.json  # Windows

# Or restore from backup
cp zpl2pdf.json.backup zpl2pdf.json  # Linux/macOS
copy zpl2pdf.json.backup zpl2pdf.json  # Windows
```

### Configuration Migration
```bash
# Export configuration
ZPL2PDF.exe --export-config > config_backup.json

# Import configuration
ZPL2PDF.exe --import-config config_backup.json
```

---

## 🔗 Related Topics

- [[Multi-Language Setup]] - Language configuration details
- [[Basic Usage]] - Command-line parameter reference
- [[Daemon Mode]] - Daemon-specific configuration
- [[Troubleshooting]] - Configuration-related issues
