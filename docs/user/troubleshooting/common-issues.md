# 🐛 Troubleshooting Guide

Common issues and solutions for ZPL2PDF.

---

## 🎯 **Quick Diagnostics**

### **Check ZPL2PDF Version**

```bash
ZPL2PDF --version
```

### **Enable Debug Logging**

```bash
ZPL2PDF --log-level Debug [command]
```

### **Test Basic Functionality**

```bash
ZPL2PDF -z "^XA^FO50,50^A0N,30,30^FDTest^FS^XZ" -o C:\Temp -n test.pdf
```

---

## 📁 **File and Folder Issues**

### **Issue: File Not Found**

**Error:** `Input file not found: label.txt`

**Causes:**
- File path is incorrect
- File doesn't exist
- Wrong working directory

**Solutions:**
```bash
# Use absolute path
ZPL2PDF -i C:\Full\Path\To\label.txt -o C:\Output

# Check file exists (PowerShell)
Test-Path C:\Path\To\label.txt

# Check file exists (Linux/macOS)
ls -la /path/to/label.txt
```

### **Issue: Permission Denied**

**Error:** `Access denied to output folder: C:\Output`

**Causes:**
- Insufficient permissions
- Folder is read-only
- Folder doesn't exist

**Solutions:**
```bash
# Windows: Run as administrator
# Right-click Command Prompt → "Run as administrator"

# Linux/macOS: Check permissions
ls -la /path/to/output
chmod 755 /path/to/output

# Create output folder
mkdir C:\Output  # Windows
mkdir -p /path/to/output  # Linux/macOS
```

### **Issue: File Locked**

**Error:** `File is locked by another process: label.txt`

**Causes:**
- File is being written by another application
- Antivirus scanning the file
- Network file system delay

**Solutions:**
```bash
# Wait for file to be fully written
# Daemon mode automatically retries

# Increase retry settings (zpl2pdf.json)
{
  "retryDelay": 5000,
  "maxRetries": 10
}

# Check which process has the file locked (Windows)
handle.exe label.txt  # Sysinternals Handle tool

# Check which process has the file locked (Linux)
lsof /path/to/label.txt
```

---

## 📄 **ZPL Content Issues**

### **Issue: Invalid ZPL Content**

**Error:** `Invalid ZPL content: Missing ^XA or ^XZ`

**Causes:**
- ZPL doesn't start with `^XA`
- ZPL doesn't end with `^XZ`
- File contains non-ZPL content

**Solutions:**
```bash
# Check file content
cat label.txt  # Linux/macOS
Get-Content label.txt  # PowerShell

# Valid ZPL structure:
^XA
^FO50,50^A0N,30,30^FDHello^FS
^XZ

# Test with minimal ZPL
ZPL2PDF -z "^XA^FO50,50^A0N,30,30^FDTest^FS^XZ" -o output/
```

### **Issue: Encoding Problems**

**Error:** `Unable to read file: Invalid encoding`

**Causes:**
- File is UTF-16 instead of UTF-8
- File contains BOM (Byte Order Mark)
- Wrong character encoding

**Solutions:**
```bash
# Convert to UTF-8 without BOM (PowerShell)
Get-Content label.txt | Set-Content -Encoding UTF8 label_fixed.txt

# Convert to UTF-8 (Linux)
iconv -f UTF-16 -t UTF-8 label.txt > label_fixed.txt

# Remove BOM (PowerShell)
$content = Get-Content label.txt -Raw
[IO.File]::WriteAllText("label_fixed.txt", $content, (New-Object System.Text.UTF8Encoding $false))
```

---

## 📐 **Dimension Issues**

### **Issue: Invalid Dimensions**

**Error:** `Invalid width: -1` or `Invalid height: 0`

**Causes:**
- Negative or zero dimensions provided
- ZPL file doesn't contain `^PW` or `^LL`
- Incorrect unit specified

**Solutions:**
```bash
# Provide valid dimensions explicitly
ZPL2PDF -i label.txt -o output/ -w 100 -h 150 -u mm

# Check for ^PW and ^LL in ZPL file
grep "^PW\|^LL" label.txt

# Use default fallback dimensions
ZPL2PDF -i label.txt -o output/  # Uses 100mm × 150mm default
```

### **Issue: PDF Size Incorrect**

**Problem:** Generated PDF doesn't match expected label size

**Causes:**
- Wrong DPI value
- Unit conversion error
- ZPL commands override parameters

**Solutions:**
```bash
# Force specific dimensions (override ZPL)
ZPL2PDF -i label.txt -o output/ -w 4 -h 6 -u in

# Check DPI setting
ZPL2PDF -i label.txt -o output/ -d 203  # Standard Zebra printer DPI

# For 300 DPI printers
ZPL2PDF -i label.txt -o output/ -d 300
```

---

## 🔄 **Daemon Mode Issues**

### **Issue: Daemon Not Starting**

**Error:** `Failed to start daemon` or `Daemon already running`

**Causes:**
- Daemon is already running
- PID file exists from previous crash
- Port conflict (if using network features)

**Solutions:**
```bash
# Check daemon status
ZPL2PDF status

# Stop existing daemon
ZPL2PDF stop

# Remove stale PID file (Windows)
del %TEMP%\zpl2pdf.pid

# Remove stale PID file (Linux/macOS)
sudo rm /var/run/zpl2pdf.pid

# Restart daemon
ZPL2PDF start -l C:\Labels
```

### **Issue: Files Not Being Processed**

**Problem:** Daemon is running but files aren't converted

**Causes:**
- Wrong file extension
- File doesn't contain valid ZPL
- Watch folder path is incorrect
- Insufficient permissions

**Solutions:**
```bash
# Check daemon status and settings
ZPL2PDF status

# Stop and run in foreground to see logs
ZPL2PDF stop
ZPL2PDF run -l C:\Labels  # See console output

# Enable debug logging
ZPL2PDF --log-level Debug run -l C:\Labels

# Verify watch folder exists
dir C:\Labels  # Windows
ls -la /path/to/labels  # Linux/macOS

# Ensure files have correct extension (.txt or .prn)
ren C:\Labels\label.zpl label.txt  # Windows
mv label.zpl label.txt  # Linux/macOS
```

### **Issue: High CPU Usage**

**Problem:** Daemon uses too much CPU

**Causes:**
- File watcher polling too frequently
- Too many files being processed
- Antivirus scanning

**Solutions:**
```json
// Edit zpl2pdf.json
{
  "fileWatcherInterval": 5000,  // Increase from 1000ms to 5000ms
  "processingQueueSize": 5      // Decrease from 10 to 5
}
```

---

## 🐳 **Docker Issues**

### **Issue: libgdiplus Not Found**

**Error:** `Unable to load shared library 'libgdiplus'`

**Causes:**
- Using wrong Docker image
- Missing system dependencies

**Solutions:**
```bash
# Use official Alpine image
docker pull brunoleocam/zpl2pdf:alpine

# Or pull latest
docker pull brunoleocam/zpl2pdf:latest

# If building custom image, ensure libgdiplus is installed
RUN apk add --no-cache libgdiplus
```

### **Issue: Permission Denied in Docker**

**Error:** `Permission denied: /app/output`

**Causes:**
- Volume mounted with wrong permissions
- Container user doesn't have write access

**Solutions:**
```bash
# Run with user permissions (Linux)
docker run -d \
  --user $(id -u):$(id -g) \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  brunoleocam/zpl2pdf:latest

# Fix folder permissions
chmod -R 777 ./output  # Linux/macOS

# Windows: Right-click folder → Properties → Security → Edit permissions
```

### **Issue: Container Stops Immediately**

**Problem:** Docker container exits right after starting

**Causes:**
- Invalid command
- Configuration error
- Missing environment variables

**Solutions:**
```bash
# Check container logs
docker logs zpl2pdf-daemon

# Run in foreground to see errors
docker run --rm \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  brunoleocam/zpl2pdf:latest

# Verify command syntax
docker run --rm brunoleocam/zpl2pdf:latest --help
```

---

## 🌍 **Language and Localization Issues**

### **Issue: Wrong Language Displayed**

**Problem:** Interface shows incorrect language

**Causes:**
- System locale not detected
- Language configuration override

**Solutions:**
```bash
# Set language temporarily
ZPL2PDF --language en-US status

# Set language permanently
ZPL2PDF --set-language pt-BR

# Reset to system default
ZPL2PDF --reset-language

# Check current language
ZPL2PDF --show-language

# Docker: Set via environment variable
docker run -e ZPL2PDF_LANGUAGE=en-US brunoleocam/zpl2pdf:latest
```

---

## 🖥️ **Platform-Specific Issues**

### **Windows Issues**

#### **Issue: "Windows protected your PC" Warning**

**Problem:** SmartScreen blocks ZPL2PDF installer

**Solution:**
1. Click **"More info"**
2. Click **"Run anyway"**
3. Or download from trusted source: [GitHub Releases](https://github.com/brunoleocam/ZPL2PDF/releases)

#### **Issue: PATH Not Updated**

**Problem:** `ZPL2PDF` command not found after installation

**Solutions:**
```powershell
# Check if ZPL2PDF is in PATH
$env:PATH -split ';' | Select-String ZPL2PDF

# Add to PATH manually
$env:PATH += ";C:\Program Files\ZPL2PDF"

# Or reinstall using WinGet (automatically adds to PATH)
winget install brunoleocam.ZPL2PDF
```

### **Linux Issues**

#### **Issue: .NET Runtime Not Found**

**Error:** `The framework 'Microsoft.NETCore.App', version '9.0.0' was not found`

**Solution:**
```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-runtime-9.0

# Fedora/CentOS
sudo dnf install -y dotnet-runtime-9.0

# Arch Linux
sudo pacman -S dotnet-runtime
```

#### **Issue: Permission Denied to Execute**

**Error:** `bash: ./ZPL2PDF: Permission denied`

**Solution:**
```bash
# Make executable
chmod +x ZPL2PDF

# Or install via package manager
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb
```

### **macOS Issues**

#### **Issue: "ZPL2PDF cannot be opened because the developer cannot be verified"**

**Problem:** macOS Gatekeeper blocks execution

**Solutions:**
```bash
# Option 1: Right-click and open
# Right-click ZPL2PDF → Open → Click "Open" in dialog

# Option 2: Remove quarantine attribute
xattr -d com.apple.quarantine ZPL2PDF

# Option 3: System Settings
# Go to: System Settings → Privacy & Security → Click "Open Anyway"
```

---

## 📊 **Performance Issues**

### **Issue: Slow Conversion**

**Problem:** Conversions take longer than expected

**Causes:**
- Large ZPL files
- Network storage
- Insufficient CPU/RAM

**Solutions:**
```bash
# Use SSD for better I/O performance
# Increase processing queue size (zpl2pdf.json)
{
  "processingQueueSize": 20
}

# Use explicit dimensions to skip ZPL parsing
ZPL2PDF -i label.txt -o output/ -w 10 -h 15 -u cm

# Close other applications to free resources
```

### **Issue: High Memory Usage**

**Problem:** ZPL2PDF uses too much RAM

**Causes:**
- Processing many files simultaneously
- Very large ZPL files

**Solutions:**
```json
// Edit zpl2pdf.json
{
  "processingQueueSize": 5  // Reduce concurrent processing
}
```

---

## 🔍 **Diagnostic Commands**

### **System Information**

```bash
# Check .NET version
dotnet --version

# Check ZPL2PDF version
ZPL2PDF --version

# Check system resources (Windows)
systeminfo

# Check system resources (Linux)
free -h
df -h
```

### **Log Analysis**

```bash
# Windows: View logs
Get-Content "$env:PROGRAMDATA\ZPL2PDF\logs\latest.log"

# Linux/macOS: View logs
cat /var/log/zpl2pdf/latest.log

# Docker: View logs
docker logs -f zpl2pdf-daemon
```

---

## 📞 **Getting Help**

If you're still experiencing issues:

1. **📖 Check Documentation**
   - [Quick Start Guide](../usage/quick-start.md)
   - [Configuration Guide](../usage/configuration.md)
   - [Docker Guide](../installation/docker.md)

2. **🐛 Report an Issue**
   - [GitHub Issues](https://github.com/brunoleocam/ZPL2PDF/issues)
   - Provide: OS, ZPL2PDF version, error message, logs

3. **💬 Ask the Community**
   - [GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
   - Search existing topics first

4. **📧 Contact Support**
   - Email: brunoleocam@gmail.com
   - Include: Version, OS, detailed description

---

**Most issues can be resolved by checking logs and following these troubleshooting steps!** 🚀
