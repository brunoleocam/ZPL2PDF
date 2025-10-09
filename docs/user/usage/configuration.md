# ⚙️ Configuration Guide

Complete guide to configuring ZPL2PDF for optimal performance and customization.

---

## 🎯 **Overview**

ZPL2PDF can be configured through:
- 📄 **Configuration File** (`zpl2pdf.json`)
- 🌍 **Environment Variables**
- 📝 **Command-Line Arguments** (highest priority)

### **Priority Order**

```
Command-Line Arguments (highest)
        ↓
Environment Variables
        ↓
Configuration File
        ↓
Default Values (lowest)
```

---

## 📄 **Configuration File**

### **Location**

| OS | Default Location |
|----|-----------------|
| **Windows** | `C:\Program Files\ZPL2PDF\zpl2pdf.json` |
| **Linux** | `/etc/zpl2pdf/zpl2pdf.json` or `~/.config/zpl2pdf/zpl2pdf.json` |
| **macOS** | `/usr/local/etc/zpl2pdf/zpl2pdf.json` or `~/Library/Application Support/zpl2pdf/zpl2pdf.json` |
| **Docker** | `/app/config/zpl2pdf.json` |

### **Complete Configuration Example**

Create a file named `zpl2pdf.json`:

```json
{
  "language": "en-US",
  "defaultWatchFolder": "C:\\Users\\user\\Documents\\ZPL2PDF Auto Converter",
  "defaultOutputFolder": "C:\\Users\\user\\Documents\\ZPL2PDF Output",
  "labelWidth": 100,
  "labelHeight": 150,
  "unit": "mm",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3,
  "enableAutoStart": false,
  "fileWatcherInterval": 1000,
  "processingQueueSize": 10,
  "deleteSourceAfterConversion": false,
  "createSubfolders": true,
  "outputFileNamePattern": "{original}_{timestamp}.pdf"
}
```

### **Configuration Options**

#### **Language Configuration**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `language` | string | System default | Interface language (`en-US`, `pt-BR`, `es-ES`, etc.) |

**Supported Languages:**
- `en-US` - English (United States)
- `pt-BR` - Português (Brasil)
- `es-ES` - Español (España)
- `fr-FR` - Français (France)
- `de-DE` - Deutsch (Deutschland)
- `it-IT` - Italiano (Italia)
- `ja-JP` - 日本語 (日本)
- `zh-CN` - 中文 (中国)

**Example:**
```json
{
  "language": "pt-BR"
}
```

#### **Folder Configuration**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `defaultWatchFolder` | string | `Documents/ZPL2PDF Auto Converter` | Default folder to monitor in daemon mode |
| `defaultOutputFolder` | string | `Documents/ZPL2PDF Output` | Default output folder for PDFs |
| `createSubfolders` | boolean | `true` | Create subfolders by date/time |

**Example:**
```json
{
  "defaultWatchFolder": "C:\\Labels\\Incoming",
  "defaultOutputFolder": "C:\\Labels\\PDF",
  "createSubfolders": true
}
```

#### **Label Dimensions**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `labelWidth` | number | `100` | Default label width |
| `labelHeight` | number | `150` | Default label height |
| `unit` | string | `mm` | Unit of measurement (`mm`, `cm`, `in`) |
| `dpi` | number | `203` | Print density (203, 300, 600) |

**Example:**
```json
{
  "labelWidth": 4,
  "labelHeight": 6,
  "unit": "in",
  "dpi": 300
}
```

#### **Performance Configuration**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `retryDelay` | number | `2000` | Delay between retry attempts (ms) |
| `maxRetries` | number | `3` | Maximum number of retries for locked files |
| `fileWatcherInterval` | number | `1000` | File watcher polling interval (ms) |
| `processingQueueSize` | number | `10` | Maximum concurrent conversions |

**Example:**
```json
{
  "retryDelay": 3000,
  "maxRetries": 5,
  "fileWatcherInterval": 500,
  "processingQueueSize": 20
}
```

#### **Logging Configuration**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `logLevel` | string | `Info` | Logging level (`Debug`, `Info`, `Warning`, `Error`) |

**Example:**
```json
{
  "logLevel": "Debug"
}
```

#### **Daemon Mode Configuration**

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `enableAutoStart` | boolean | `false` | Start daemon automatically on system boot |
| `deleteSourceAfterConversion` | boolean | `false` | Delete ZPL file after successful conversion |
| `outputFileNamePattern` | string | `{original}.pdf` | Output filename pattern |

**Filename Patterns:**
- `{original}` - Original filename without extension
- `{timestamp}` - Current timestamp (yyyyMMddHHmmss)
- `{date}` - Current date (yyyyMMdd)
- `{time}` - Current time (HHmmss)
- `{guid}` - Unique GUID

**Example:**
```json
{
  "enableAutoStart": true,
  "deleteSourceAfterConversion": false,
  "outputFileNamePattern": "{original}_{timestamp}.pdf"
}
```

---

## 🌍 **Environment Variables**

### **Available Variables**

| Variable | Description | Example |
|----------|-------------|---------|
| `ZPL2PDF_LANGUAGE` | Application language | `pt-BR` |
| `ZPL2PDF_LOG_LEVEL` | Logging level | `Debug` |
| `ZPL2PDF_WATCH_FOLDER` | Default watch folder | `/app/watch` |
| `ZPL2PDF_OUTPUT_FOLDER` | Default output folder | `/app/output` |
| `ZPL2PDF_DPI` | Print density | `300` |

### **Setting Environment Variables**

#### **Windows (PowerShell)**
```powershell
# Temporary (current session)
$env:ZPL2PDF_LANGUAGE = "pt-BR"
$env:ZPL2PDF_LOG_LEVEL = "Debug"

# Permanent (system-wide)
[System.Environment]::SetEnvironmentVariable("ZPL2PDF_LANGUAGE", "pt-BR", "User")
```

#### **Linux/macOS (Bash)**
```bash
# Temporary (current session)
export ZPL2PDF_LANGUAGE=pt-BR
export ZPL2PDF_LOG_LEVEL=Debug

# Permanent (add to ~/.bashrc or ~/.zshrc)
echo 'export ZPL2PDF_LANGUAGE=pt-BR' >> ~/.bashrc
source ~/.bashrc
```

#### **Docker**
```bash
docker run -d \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  -e ZPL2PDF_LOG_LEVEL=Debug \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  brunoleocam/zpl2pdf:latest
```

**Docker Compose:**
```yaml
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR
      - ZPL2PDF_LOG_LEVEL=Debug
      - ZPL2PDF_DPI=300
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
```

---

## 📝 **Command-Line Arguments**

Command-line arguments **override** all other configurations.

### **Global Arguments**

```bash
ZPL2PDF --language pt-BR --log-level Debug [command]
```

| Argument | Description | Example |
|----------|-------------|---------|
| `--language <code>` | Set language temporarily | `--language pt-BR` |
| `--set-language <code>` | Set language permanently | `--set-language en-US` |
| `--reset-language` | Reset to system default | `--reset-language` |
| `--show-language` | Show current language | `--show-language` |
| `--log-level <level>` | Set logging level | `--log-level Debug` |

### **Conversion Mode Arguments**

```bash
ZPL2PDF -i input.txt -o output/ -w 10 -h 15 -u cm -d 300
```

| Argument | Description | Example |
|----------|-------------|---------|
| `-i <file>` | Input ZPL file | `-i label.txt` |
| `-z <content>` | ZPL content as string | `-z "^XA...^XZ"` |
| `-o <folder>` | Output folder | `-o C:\Output` |
| `-n <name>` | Output filename | `-n result.pdf` |
| `-w <width>` | Label width | `-w 10` |
| `-h <height>` | Label height | `-h 15` |
| `-u <unit>` | Unit (mm, cm, in) | `-u cm` |
| `-d <dpi>` | Print density | `-d 300` |

### **Daemon Mode Arguments**

```bash
ZPL2PDF start -l "C:\Watch" -o "C:\Output" -w 10 -h 15 -u cm
```

| Argument | Description | Example |
|----------|-------------|---------|
| `-l <folder>` | Watch folder | `-l C:\Watch` |
| `-o <folder>` | Output folder | `-o C:\Output` |
| `-w <width>` | Fixed width | `-w 10` |
| `-h <height>` | Fixed height | `-h 15` |
| `-u <unit>` | Unit | `-u cm` |
| `-d <dpi>` | Print density | `-d 300` |

---

## 🎯 **Configuration Examples**

### **Example 1: Basic Setup**

**Configuration File (`zpl2pdf.json`):**
```json
{
  "language": "en-US",
  "labelWidth": 100,
  "labelHeight": 150,
  "unit": "mm",
  "dpi": 203
}
```

### **Example 2: High-Volume Production**

**Configuration File:**
```json
{
  "language": "en-US",
  "processingQueueSize": 50,
  "retryDelay": 1000,
  "maxRetries": 5,
  "fileWatcherInterval": 500,
  "deleteSourceAfterConversion": true,
  "outputFileNamePattern": "{original}_{timestamp}.pdf"
}
```

### **Example 3: Multi-Language Environment**

**Docker Compose:**
```yaml
version: '3.8'

services:
  zpl2pdf-en:
    image: brunoleocam/zpl2pdf:latest
    container_name: zpl2pdf-english
    environment:
      - ZPL2PDF_LANGUAGE=en-US
    volumes:
      - ./watch-en:/app/watch
      - ./output-en:/app/output

  zpl2pdf-pt:
    image: brunoleocam/zpl2pdf:latest
    container_name: zpl2pdf-portuguese
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR
    volumes:
      - ./watch-pt:/app/watch
      - ./output-pt:/app/output
```

### **Example 4: Development Environment**

**Configuration File:**
```json
{
  "language": "en-US",
  "logLevel": "Debug",
  "retryDelay": 5000,
  "maxRetries": 10,
  "deleteSourceAfterConversion": false,
  "createSubfolders": true
}
```

---

## ⚡ **Performance Optimization**

### **High-Volume Processing**

For processing hundreds of files per minute:

```json
{
  "processingQueueSize": 50,
  "fileWatcherInterval": 200,
  "retryDelay": 500,
  "maxRetries": 2
}
```

### **Memory Optimization**

For low-memory environments:

```json
{
  "processingQueueSize": 5,
  "fileWatcherInterval": 2000
}
```

### **Network Storage**

For network-mounted folders:

```json
{
  "retryDelay": 5000,
  "maxRetries": 10,
  "fileWatcherInterval": 3000
}
```

---

## 🐛 **Troubleshooting Configuration**

### **Configuration Not Loading**

1. **Check file location**: Ensure `zpl2pdf.json` is in the correct directory
2. **Validate JSON syntax**: Use a JSON validator
3. **Check permissions**: Ensure read access to the file
4. **Check logs**: Run with `--log-level Debug`

### **Environment Variables Not Working**

```bash
# Verify variable is set
# Windows
echo $env:ZPL2PDF_LANGUAGE

# Linux/macOS
echo $ZPL2PDF_LANGUAGE

# Docker
docker exec zpl2pdf env | grep ZPL2PDF
```

### **Priority Issues**

Remember the priority order:
1. **Command-line arguments** (highest)
2. **Environment variables**
3. **Configuration file**
4. **Default values** (lowest)

---

## 📚 **Related Documentation**

- 🚀 [Quick Start Guide](quick-start.md)
- 🔄 [Conversion Mode Guide](conversion-mode.md)
- 🔄 [Daemon Mode Guide](daemon-mode.md)
- 🐛 [Troubleshooting Guide](../troubleshooting/common-issues.md)
- 🐳 [Docker Guide](../installation/docker.md)

---

**Proper configuration ensures optimal ZPL2PDF performance for your specific use case!** 🚀
