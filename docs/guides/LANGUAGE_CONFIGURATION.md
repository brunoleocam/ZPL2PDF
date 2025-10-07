# 🌍 Language System - ZPL2PDF

## 📋 Overview

ZPL2PDF supports **8 languages** with automatic detection and multiple configuration methods.

### Supported Languages:
- 🇺🇸 **English** (en-US) - Default
- 🇧🇷 **Portuguese** (pt-BR)
- 🇪🇸 **Spanish** (es-ES)
- 🇫🇷 **French** (fr-FR)
- 🇩🇪 **German** (de-DE)
- 🇮🇹 **Italian** (it-IT)
- 🇯🇵 **Japanese** (ja-JP)
- 🇨🇳 **Chinese Simplified** (zh-CN)

---

## 🎯 Priority Order

The system uses the following priority order to detect the language:

```
1. --language parameter (temporary, only for that execution)
   ↓
2. ZPL2PDF_LANGUAGE environment variable (persistent)
   ↓
3. zpl2pdf.json configuration file (persistent)
   ↓
4. Operating system automatic detection (default)
```

---

## 🔧 How to Configure

### 1️⃣ **`--language` Parameter (Temporary)**

Use for testing or specific executions:

```bash
# Windows
ZPL2PDF.exe --language es-ES status
ZPL2PDF.exe --language fr-FR -help

# Linux/macOS
./ZPL2PDF --language de-DE status
```

**Advantages:**
- ✅ Quick for testing
- ✅ Doesn't change permanent configuration

**Disadvantages:**
- ❌ Must specify in each command
- ❌ Doesn't persist between executions

---

### 2️⃣ **`ZPL2PDF_LANGUAGE` Environment Variable (Recommended)**

#### Windows:

**PowerShell (temporary - current session only):**
```powershell
$env:ZPL2PDF_LANGUAGE = "es-ES"
```

**PowerShell (permanent - for user):**
```powershell
[System.Environment]::SetEnvironmentVariable("ZPL2PDF_LANGUAGE", "es-ES", "User")
```

**CMD (permanent):**
```cmd
setx ZPL2PDF_LANGUAGE "es-ES"
```

#### Linux/macOS:

**Temporary (current session only):**
```bash
export ZPL2PDF_LANGUAGE="es-ES"
```

**Permanent (add to ~/.bashrc or ~/.zshrc):**
```bash
echo 'export ZPL2PDF_LANGUAGE="es-ES"' >> ~/.bashrc
source ~/.bashrc
```

**Permanent (system-wide - /etc/profile.d/):**
```bash
sudo echo 'export ZPL2PDF_LANGUAGE="es-ES"' > /etc/profile.d/zpl2pdf.sh
sudo chmod +x /etc/profile.d/zpl2pdf.sh
```

**Advantages:**
- ✅ Works on Windows, Linux, and macOS
- ✅ Persistent across executions
- ✅ Easy to configure via scripts
- ✅ Ideal for installers (Inno Setup, .deb, .rpm)
- ✅ Perfect for Docker/Containers
- ✅ Corporate environments (Windows GPO)

**Disadvantages:**
- ⚠️ Requires logout/login or terminal restart (permanent)

---

### 3️⃣ **`zpl2pdf.json` Configuration File**

#### File Location:

| System | Location |
|---------|-------------|
| **Windows** | `C:\Program Files\ZPL2PDF\zpl2pdf.json` |
| **Linux** | `/opt/zpl2pdf/zpl2pdf.json` or `~/.config/zpl2pdf/zpl2pdf.json` |
| **macOS** | `/Applications/ZPL2PDF/zpl2pdf.json` |

#### File Content:

```json
{
  "language": "es-ES",
  "defaultWatchFolder": "C:\\Users\\user\\Documents\\ZPL2PDF Auto Converter",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3
}
```

**Advantages:**
- ✅ Persistent configuration
- ✅ Portable (can copy with executable)
- ✅ Easy to edit (any text editor)
- ✅ Can be versioned (git)

**Disadvantages:**
- ⚠️ May need administrator permissions (Windows/Program Files)
- ⚠️ Less flexible than environment variable for scripts

---

### 4️⃣ **Automatic Detection (Default)**

If none of the above options are configured, the system automatically detects:

#### Windows:
- Uses `CultureInfo.CurrentUICulture`
- Example: Portuguese system → `pt-BR`

#### Linux/macOS:
- Reads `LANG` and `LC_ALL` environment variables
- Example: `LANG=pt_BR.UTF-8` → `pt-BR`

**Advantages:**
- ✅ Works automatically
- ✅ No configuration needed

**Disadvantages:**
- ⚠️ May not detect correctly in some cases
- ⚠️ No manual control

---

## 🚀 Usage with Installers

### Inno Setup (Windows)

You can configure the language automatically during installation:

```pascal
[Code]
procedure CurStepChanged(CurStep: TSetupStep);
var
  LangCode: String;
begin
  if CurStep = ssPostInstall then
  begin
    // Map installer language
    case ActiveLanguage of
      'english': LangCode := 'en-US';
      'portuguese': LangCode := 'pt-BR';
      'spanish': LangCode := 'es-ES';
      'french': LangCode := 'fr-FR';
      'german': LangCode := 'de-DE';
      'italian': LangCode := 'it-IT';
    else
      LangCode := 'en-US';
    end;
    
    // Option 1: Set environment variable (RECOMMENDED)
    RegWriteStringValue(HKCU, 'Environment', 'ZPL2PDF_LANGUAGE', LangCode);
    
    // Option 2: Create configuration file in %APPDATA%
    // (code to create zpl2pdf.json in user's AppData folder)
  end;
end;
```

### .deb Packages (Debian/Ubuntu)

Post-install script:

```bash
#!/bin/bash
# /var/lib/dpkg/info/zpl2pdf.postinst

# Detect system language
SYSTEM_LANG=$(echo $LANG | cut -d. -f1 | tr '_' '-')

# Create global configuration
echo "export ZPL2PDF_LANGUAGE=\"$SYSTEM_LANG\"" > /etc/profile.d/zpl2pdf.sh
chmod +x /etc/profile.d/zpl2pdf.sh
```

### .rpm Packages (Red Hat/CentOS)

```spec
%post
# Detect system language
SYSTEM_LANG=$(echo $LANG | cut -d. -f1 | tr '_' '-')

# Create global configuration
echo "export ZPL2PDF_LANGUAGE=\"$SYSTEM_LANG\"" > /etc/profile.d/zpl2pdf.sh
chmod +x /etc/profile.d/zpl2pdf.sh
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0

# Set container language
ENV ZPL2PDF_LANGUAGE=pt-BR

COPY --from=build /app/publish /app
WORKDIR /app

ENTRYPOINT ["/app/ZPL2PDF"]
```

**Docker Compose Example:**

```yaml
version: '3.8'
services:
  zpl2pdf:
    image: zpl2pdf:latest
    environment:
      - ZPL2PDF_LANGUAGE=es-ES  # Set to Spanish
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
```

---

## 📊 Detailed Comparison

| Method | Cross-Platform | Persistent | Priority | Installer | Scripts | Docker |
|--------|----------------|-------------|----------|-----------|---------|--------|
| **`--language`** | ✅ | ❌ | 1 (Highest) | ❌ | ✅ | ✅ |
| **Env Variable** | ✅ | ✅ | 2 | ✅ | ✅ | ✅ |
| **Config File** | ✅ | ✅ | 3 | ✅ | ⚠️ | ⚠️ |
| **Auto-detect** | ✅ | ✅ | 4 (Lowest) | ✅ | ✅ | ✅ |

---

## 🎯 Recommendations

### For End Users:
- ✅ **Environment variable** via installer (Inno Setup)
- ✅ Easy to change through Control Panel

### For Developers:
- ✅ **`--language` parameter** for quick testing
- ✅ **Environment variable** for development

### For Enterprises:
- ✅ **Environment variable** via GPO (Windows)
- ✅ **Environment variable** via scripts (Linux)

### For Containers:
- ✅ **Environment variable** in Dockerfile
- ✅ Easy to configure via `docker-compose.yml`

---

## 🧪 Test Examples

### Test 1: Parameter Priority
```bash
# Even with environment variable set, parameter takes priority
$env:ZPL2PDF_LANGUAGE = "pt-BR"
ZPL2PDF.exe --language es-ES status
# Result: Message in Spanish
```

### Test 2: Environment Variable
```bash
$env:ZPL2PDF_LANGUAGE = "fr-FR"
ZPL2PDF.exe status
# Result: Message in French
```

### Test 3: Configuration File
```json
// zpl2pdf.json
{
  "language": "de-DE"
}
```
```bash
ZPL2PDF.exe status
# Result: Message in German (if no environment variable is set)
```

### Test 4: Automatic Detection
```bash
# Without any configuration, uses system language
ZPL2PDF.exe status
# Result: Message in Windows/Linux system language
```

---

## ❓ FAQ

### **Q: How to permanently change language on Windows?**
**A:** Use the command `setx ZPL2PDF_LANGUAGE "es-ES"` in CMD or configure through Control Panel.

### **Q: Does environment variable work on Linux?**
**A:** Yes! Use `export ZPL2PDF_LANGUAGE="pt-BR"` and add to `~/.bashrc` to make it permanent.

### **Q: Does configuration file have priority over environment variable?**
**A:** No. Environment variable has **higher priority** than configuration file.

### **Q: Can I use different languages in different commands?**
**A:** Yes! Use the `--language` parameter for temporary override:
```bash
ZPL2PDF.exe --language en-US status
ZPL2PDF.exe --language es-ES -help
```

### **Q: How should the installer configure the language?**
**A:** We recommend setting the **user environment variable** during installation, based on the language chosen by the user in the installer.

### **Q: What happens if I set an invalid language code?**
**A:** The system will fall back to the next priority level. If all fail, it defaults to English (en-US).

### **Q: Can I add new languages?**
**A:** Yes! Add a new `Messages.{culture}.resx` file in the `Resources/` folder with translations, and the language will be automatically supported.

---

## 🐛 Troubleshooting

### Problem: Language doesn't change even after setting environment variable
**Solution:** 
- Windows: Close and reopen terminal/command prompt
- Linux: Run `source ~/.bashrc` or open a new terminal

### Problem: Configuration file is not read
**Solution:**
- Verify that `zpl2pdf.json` file is in the same folder as the executable
- Verify that JSON is valid (no syntax errors)
- Check if there's an environment variable set (has higher priority)

### Problem: Strange characters (???) in output
**Solution:**
- On Windows: Use PowerShell or terminal with UTF-8 support
- Configure terminal encoding: `[Console]::OutputEncoding = [System.Text.Encoding]::UTF8`
- On Linux: Ensure locale is properly configured: `sudo dpkg-reconfigure locales`

### Problem: Japanese/Chinese characters don't display correctly
**Solution:**
- Install proper fonts that support CJK characters
- Windows: Usually works by default
- Linux: Install `fonts-noto-cjk` or similar package

---

## 🔧 Advanced Configuration

### Multiple Users on Same System

Each user can have their own language preference:

```bash
# User 1 (Portuguese)
User1> setx ZPL2PDF_LANGUAGE "pt-BR"

# User 2 (Spanish)
User2> setx ZPL2PDF_LANGUAGE "es-ES"
```

### CI/CD Pipelines

Set language for automated builds:

```yaml
# GitHub Actions
- name: Test in Spanish
  env:
    ZPL2PDF_LANGUAGE: es-ES
  run: ./ZPL2PDF status
```

### Kubernetes Deployments

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: zpl2pdf
spec:
  template:
    spec:
      containers:
      - name: zpl2pdf
        image: zpl2pdf:latest
        env:
        - name: ZPL2PDF_LANGUAGE
          value: "fr-FR"
```

---

## 📝 Adding New Translations

To add a new language:

1. **Create resource file:**
   - Copy `Resources/Messages.en.resx`
   - Rename to `Messages.{culture}.resx` (e.g., `Messages.nl-NL.resx` for Dutch)

2. **Translate strings:**
   - Open the new `.resx` file
   - Translate all `<value>` tags
   - Keep parameter placeholders (e.g., `{0}`, `{1}`)

3. **Add to supported cultures:**
   - Edit `src/Shared/Localization/LocalizationManager.cs`
   - Add culture code to `IsCultureSupported` method

4. **Test:**
   ```bash
   ZPL2PDF.exe --language nl-NL status
   ```

---

## 🌐 Localization Best Practices

### For Translators:

1. **Preserve formatting:**
   - Keep `{0}`, `{1}` parameter placeholders in the same position
   - Example: `"File processed: {0}"` → `"Archivo procesado: {0}"`

2. **Keep command names:**
   - Don't translate: `start`, `stop`, `status`, `run`
   - These are command keywords

3. **Preserve special characters:**
   - Keep: `\n`, `\r`, `\t` (line breaks, tabs)
   - Example: `"Line 1\nLine 2"` → `"Línea 1\nLínea 2"`

4. **Test your translations:**
   ```bash
   ZPL2PDF.exe --language YOUR-CULTURE -help
   ```

---

## 📚 Additional Resources

- [README.md](../README.md) - General documentation
- [README.pt.md](../README.pt.md) - Portuguese documentation
- [CONTRIBUTING.md](../CONTRIBUTING.md) - Contribution guide
- [Resources/](../Resources/) - Translation files (.resx)

---

## 🤝 Contributing Translations

We welcome translations to new languages! To contribute:

1. Fork the repository
2. Create a new `Messages.{culture}.resx` file
3. Translate all strings
4. Test thoroughly
5. Submit a Pull Request

See [CONTRIBUTING.md](../CONTRIBUTING.md) for detailed guidelines.

---

## 📞 Support

If you have questions or issues with language configuration:

- 📖 Check this documentation first
- 🐛 Open an issue on GitHub
- 💬 Join our community discussions

---

**Last Updated:** January 2025  
**Version:** 2.0.0