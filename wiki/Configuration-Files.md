# ⚙️ Configuration Files

Detailed reference for ZPL2PDF configuration files and formats.

## 📄 Main Configuration File

### Location
| Platform | Path |
|----------|------|
| **Windows** | `%APPDATA%\ZPL2PDF\zpl2pdf.json` or executable folder |
| **Linux** | `~/.config/zpl2pdf/zpl2pdf.json` |
| **macOS** | `~/Library/Application Support/ZPL2PDF/zpl2pdf.json` |

### Format (zpl2pdf.json)
```json
{
  "language": "en-US",
  "defaultWatchFolder": "C:\\ZPL2PDF\\Watch",
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
| `language` | string | `"en-US"` | UI language (en-US, pt-BR, es-ES, etc.) |
| `defaultWatchFolder` | string | System default | Default folder for daemon mode |
| `labelWidth` | number | `60` | Default label width (mm) |
| `labelHeight` | number | `120` | Default label height (mm) |
| `unit` | string | `"mm"` | Unit of measurement (mm, cm, in) |
| `dpi` | number | `203` | Print density (150-600) |
| `logLevel` | string | `"Info"` | Logging level (Debug, Info, Warning, Error) |
| `retryDelay` | number | `2000` | Delay between retries (milliseconds) |
| `maxRetries` | number | `3` | Maximum retry attempts |
| `autoDeleteProcessed` | boolean | `false` | Delete files after processing |
| `outputSubfolders` | boolean | `true` | Create date-based subfolders |
| `dateTimeFormat` | string | `"yyyy-MM-dd_HH-mm-ss"` | DateTime format for folders |

---

## 🌐 Environment Variables

### Language Configuration
```bash
# Set default language
export ZPL2PDF_LANGUAGE=pt-BR  # Linux/macOS
set ZPL2PDF_LANGUAGE=pt-BR     # Windows
```

### Folder Configuration
```bash
# Set default watch folder
export ZPL2PDF_WATCH_FOLDER="/var/zpl2pdf/watch"
```

### Logging Configuration
```bash
# Set log level
export ZPL2PDF_LOG_LEVEL=Debug
```

---

## 🐳 Docker Configuration

### Environment Variables
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    environment:
      - ZPL2PDF_LANGUAGE=en-US
      - ZPL2PDF_LOG_LEVEL=Info
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
      - ./config:/app/config
```

### Custom Config File
```bash
# Mount custom configuration
docker run -v $(pwd)/zpl2pdf.json:/app/config/zpl2pdf.json brunoleocam/zpl2pdf:latest
```

---

## 📋 Example Configurations

### Development
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

### Production
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

---

## 🔗 Related Topics

- [[Configuration]] - Configuration guide
- [[Multi-Language Setup]] - Language configuration
- [[Daemon Mode]] - Daemon-specific settings
