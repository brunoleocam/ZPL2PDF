# 🌍 Multi-Language Setup

Complete guide for configuring and using ZPL2PDF in multiple languages.

## 🎯 Overview

ZPL2PDF supports **8 languages** with automatic detection and manual configuration options:

- 🇺🇸 **English** (en-US) - Default
- 🇧🇷 **Portuguese** (pt-BR)
- 🇪🇸 **Spanish** (es-ES)
- 🇫🇷 **French** (fr-FR)
- 🇩🇪 **German** (de-DE)
- 🇮🇹 **Italian** (it-IT)
- 🇯🇵 **Japanese** (ja-JP)
- 🇨🇳 **Chinese** (zh-CN)

---

## 🔍 Language Detection Priority

ZPL2PDF detects language in the following order:

1. **Command-line parameter** (`--language`) - Highest priority
2. **Environment variable** (`ZPL2PDF_LANGUAGE`) - Medium priority
3. **System language** - Automatic detection
4. **English (en-US)** - Fallback

---

## ⚡ Quick Commands

### Temporary Language Change
```bash
# Use Spanish for this command only
ZPL2PDF.exe --language es-ES status

# Use Portuguese for help
ZPL2PDF.exe --language pt-BR --help

# Use French for conversion
ZPL2PDF.exe --language fr-FR -i label.txt -o output
```

### Permanent Language Change
```bash
# Set default language to Spanish
ZPL2PDF.exe --language-definitive es-ES

# Set default language to Portuguese
ZPL2PDF.exe --language-definitive pt-BR

# Reset to system default
ZPL2PDF.exe --language-standard
```

### Check Current Language
```bash
# Show current language configuration
ZPL2PDF.exe --show-language

# Expected output:
# Current language: Spanish (es-ES)
# System language: Portuguese (pt-BR)
# Environment variable: Not set
# Language priority order: Command-line → Environment → System → English
```

---

## 🌐 Environment Variable Setup

### Windows
```cmd
# Set for current session
set ZPL2PDF_LANGUAGE=es-ES

# Set permanently (User)
setx ZPL2PDF_LANGUAGE es-ES

# Set permanently (System)
setx ZPL2PDF_LANGUAGE es-ES /M

# Verify setting
echo %ZPL2PDF_LANGUAGE%
```

### Linux/macOS
```bash
# Set for current session
export ZPL2PDF_LANGUAGE=es-ES

# Set permanently (add to ~/.bashrc or ~/.zshrc)
echo 'export ZPL2PDF_LANGUAGE=es-ES' >> ~/.bashrc
source ~/.bashrc

# Verify setting
echo $ZPL2PDF_LANGUAGE
```

### Docker
```bash
# Run container with specific language
docker run -e ZPL2PDF_LANGUAGE=pt-BR brunoleocam/zpl2pdf:latest

# Docker Compose
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    environment:
      - ZPL2PDF_LANGUAGE=es-ES
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
```

---

## 🔧 Configuration File

Create or modify `zpl2pdf.json`:

```json
{
  "language": "pt-BR",
  "defaultWatchFolder": "C:\\WatchFolder",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info"
}
```

### Configuration File Locations
- **Windows**: Same folder as executable
- **Linux**: `~/.config/zpl2pdf/zpl2pdf.json`
- **macOS**: `~/Library/Application Support/ZPL2PDF/zpl2pdf.json`

---

## 📋 Language Codes Reference

| Language | Code | Native Name |
|----------|------|-------------|
| English | `en-US` | English |
| Portuguese | `pt-BR` | Português |
| Spanish | `es-ES` | Español |
| French | `fr-FR` | Français |
| German | `de-DE` | Deutsch |
| Italian | `it-IT` | Italiano |
| Japanese | `ja-JP` | 日本語 |
| Chinese | `zh-CN` | 中文 |

---

## 🎨 Usage Examples

### Example 1: Portuguese Interface
```bash
# Set Portuguese as default
ZPL2PDF.exe --language-definitive pt-BR

# Now all commands use Portuguese
ZPL2PDF.exe status
# Output: Verificando status do daemon...
#         O daemon não está em execução!

ZPL2PDF.exe --help
# Output: ZPL2PDF - Conversor ZPL para PDF
#         Uso: ZPL2PDF.exe [opções]
```

### Example 2: Spanish Conversion
```bash
# Convert with Spanish interface
ZPL2PDF.exe --language es-ES -i label.txt -o output -n etiqueta.pdf

# Output: Procesando archivo: label.txt
#         Dimensiones detectadas: 100mm x 200mm
#         Generando PDF: etiqueta.pdf
#         Conversión completada exitosamente
```

### Example 3: French Daemon
```bash
# Start daemon with French interface
ZPL2PDF.exe --language fr-FR start -l "./watch" -w 7.5 -h 15 -u in

# Output: Démarrage du mode démon...
#         Dossier surveillé: ./watch
#         Dimensions: 7.5in x 15in
#         Le démon a démarré avec succès
```

### Example 4: Japanese Configuration
```bash
# Set Japanese as default
ZPL2PDF.exe --language-definitive ja-JP

# Check configuration
ZPL2PDF.exe --show-language
# Output: 現在の言語: 日本語 (ja-JP)
#         システム言語: 英語 (en-US)
#         環境変数: 設定されていません
```

---

## 🔄 Language Switching Workflow

### Development Workflow
```bash
# Test in multiple languages
ZPL2PDF.exe --language en-US --help
ZPL2PDF.exe --language pt-BR --help
ZPL2PDF.exe --language es-ES --help

# Compare outputs for consistency
```

### Production Deployment
```bash
# Set language via environment variable
export ZPL2PDF_LANGUAGE=pt-BR

# Start daemon (uses environment language)
ZPL2PDF.exe start -l "./watch"
```

### User Experience
```bash
# User can override system language
ZPL2PDF.exe --language es-ES -i label.txt -o output

# Or set permanent preference
ZPL2PDF.exe --language-definitive es-ES
```

---

## 🐛 Troubleshooting

### Issue: "Invalid language code"
```bash
# Wrong: Invalid code
ZPL2PDF.exe --language spanish

# Correct: Use proper code
ZPL2PDF.exe --language es-ES
```

### Issue: "Language not supported"
```bash
# Check supported languages
ZPL2PDF.exe --show-language

# Use supported code
ZPL2PDF.exe --language pt-BR
```

### Issue: "Language not changing"
```bash
# Check current configuration
ZPL2PDF.exe --show-language

# Force language change
ZPL2PDF.exe --language-definitive es-ES

# Restart application
ZPL2PDF.exe stop
ZPL2PDF.exe start
```

---

## 📊 Language-Specific Features

### Character Encoding
- **Latin languages** (EN, PT, ES, FR, DE, IT): UTF-8
- **Japanese** (JA): UTF-8 with proper rendering
- **Chinese** (ZH): UTF-8 with proper rendering

### Date/Time Formats
- **English**: MM/DD/YYYY
- **Portuguese/Spanish**: DD/MM/YYYY
- **German**: DD.MM.YYYY
- **French**: DD/MM/YYYY
- **Japanese**: YYYY年MM月DD日
- **Chinese**: YYYY年MM月DD日

### Number Formats
- **English/German**: 1,234.56
- **Portuguese/Spanish/French**: 1.234,56
- **Japanese/Chinese**: 1,234.56

---

## 🔗 Integration Examples

### Windows Service
```batch
# Install service with Portuguese language
nssm install ZPL2PDF "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" start -l "C:\WatchFolder"
nssm set ZPL2PDF AppEnvironmentExtra ZPL2PDF_LANGUAGE=pt-BR
```

### Linux systemd Service
```ini
# /etc/systemd/system/zpl2pdf.service
[Unit]
Description=ZPL2PDF Daemon
After=network.target

[Service]
Type=simple
User=zpl2pdf
WorkingDirectory=/opt/zpl2pdf
ExecStart=/opt/zpl2pdf/ZPL2PDF start -l /var/zpl2pdf/watch
Environment=ZPL2PDF_LANGUAGE=es-ES
Restart=always

[Install]
WantedBy=multi-user.target
```

### Docker Compose Multi-Language
```yaml
version: '3.8'
services:
  zpl2pdf-en:
    image: brunoleocam/zpl2pdf:latest
    environment:
      - ZPL2PDF_LANGUAGE=en-US
    volumes:
      - ./watch-en:/app/watch
      - ./output-en:/app/output
    
  zpl2pdf-pt:
    image: brunoleocam/zpl2pdf:latest
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR
    volumes:
      - ./watch-pt:/app/watch
      - ./output-pt:/app/output
```

---

## 🔗 Related Topics

- [[Configuration]] - Global settings and preferences
- [[Basic Usage]] - Language commands overview
- [[Installation Guide]] - Multi-language installation
- [[Troubleshooting]] - Language-related issues
