# 🔄 Daemon Mode

Comprehensive guide for automatic folder monitoring and batch processing.

## 🎯 Overview

Daemon Mode is designed for:
- ✅ **Automatic file processing** - Convert files as they appear
- ✅ **Continuous operation** - Run in background
- ✅ **Batch processing** - Handle multiple files
- ✅ **Server deployment** - Enterprise environments
- ✅ **Production workflows** - Automated label generation

---

## 🚀 Getting Started

### Start Daemon
```bash
# Basic start (default folder)
ZPL2PDF.exe start

# Custom folder and dimensions
ZPL2PDF.exe start -l "C:\WatchFolder" -w 7.5 -h 15 -u in

# High resolution processing
ZPL2PDF.exe start -l "./watch" -w 100 -h 200 -u mm -d 300
```

### Check Status
```bash
# Check if daemon is running
ZPL2PDF.exe status

# Expected output:
# Daemon is running (PID: 1234)
# Monitoring folder: C:\WatchFolder
# Processing files: 0
```

### Stop Daemon
```bash
# Graceful shutdown
ZPL2PDF.exe stop

# Expected output:
# Stopping daemon...
# Daemon stopped successfully
```

---

## 📁 Folder Configuration

### Default Folders

| Platform | Default Watch Folder |
|----------|---------------------|
| **Windows** | `%USERPROFILE%\Documents\ZPL2PDF Auto Converter` |
| **Linux** | `$HOME/Documents/ZPL2PDF Auto Converter` |
| **macOS** | `$HOME/Documents/ZPL2PDF Auto Converter` |

### Custom Folder Setup
```bash
# Create custom watch folder
mkdir "C:\MyLabelProcessing"

# Start daemon with custom folder
ZPL2PDF.exe start -l "C:\MyLabelProcessing" -w 7.5 -h 15 -u in
```

### Folder Structure
```
WatchFolder/
├── input/          ← Place ZPL files here
├── processing/     ← Files being processed
├── output/         ← Generated PDFs
└── error/          ← Failed conversions
```

---

## ⚙️ Configuration Options

### Dimension Settings
```bash
# Fixed dimensions for all files
ZPL2PDF.exe start -l "./watch" -w 10 -h 15 -u cm

# High resolution processing
ZPL2PDF.exe start -l "./watch" -w 7.5 -h 15 -u in -d 300

# Mixed units (width in cm, height in mm)
ZPL2PDF.exe start -l "./watch" -w 10 -h 150 -u cm  # Height converted to cm
```

### Processing Options
```bash
# Standard processing
ZPL2PDF.exe start -l "./watch"

# High quality processing
ZPL2PDF.exe start -l "./watch" -d 300

# Fast processing (lower quality)
ZPL2PDF.exe start -l "./watch" -d 150
```

---

## 🔄 File Processing Workflow

### 1. File Detection
- Daemon monitors the watch folder
- Detects new `.txt` and `.prn` files
- Validates file format and accessibility

### 2. Queue Processing
- Files are added to processing queue
- Sequential processing to avoid conflicts
- Retry mechanism for locked files

### 3. Conversion Process
- Extract dimensions from ZPL (if available)
- Apply fixed dimensions (if specified)
- Generate PDF with specified quality
- Save to output folder

### 4. Cleanup
- Remove processed files (optional)
- Log processing results
- Handle errors gracefully

---

## 📊 Monitoring and Logging

### Real-time Monitoring
```bash
# Check daemon status
ZPL2PDF.exe status

# Detailed status information:
# - Process ID (PID)
# - Watch folder path
# - Files in queue
# - Processing statistics
# - Last activity time
```

### Log Files
```
%TEMP%/zpl2pdf.log          # Windows
/tmp/zpl2pdf.log            # Linux/macOS
```

### Log Levels
- **Info**: General processing information
- **Warning**: Non-critical issues
- **Error**: Processing failures
- **Debug**: Detailed debugging information

---

## 🏢 Enterprise Features

### PID Management
```bash
# Check if daemon is running by PID
ZPL2PDF.exe status

# Output: Daemon is running (PID: 1234)
# Use PID to monitor process externally
```

### Process Monitoring
```bash
# Windows - Check if process exists
tasklist /FI "PID eq 1234"

# Linux - Check if process exists  
ps -p 1234

# Kill daemon if unresponsive
ZPL2PDF.exe stop  # Graceful shutdown
# or
kill 1234         # Force kill (Linux)
taskkill /PID 1234 /F  # Force kill (Windows)
```

### Service Integration
```bash
# Run as Windows Service (using NSSM)
nssm install ZPL2PDF "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" start -l "C:\WatchFolder"

# Run as systemd service (Linux)
sudo systemctl enable zpl2pdf-daemon
sudo systemctl start zpl2pdf-daemon
```

---

## 🔧 Advanced Configuration

### Environment Variables
```bash
# Set default language
export ZPL2PDF_LANGUAGE=pt-BR

# Set default watch folder
export ZPL2PDF_WATCH_FOLDER="/var/zpl2pdf/watch"

# Set log level
export ZPL2PDF_LOG_LEVEL=Debug
```

### Configuration File
Create `zpl2pdf.json`:
```json
{
  "defaultWatchFolder": "C:\\WatchFolder",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info",
  "retryDelay": 2000,
  "maxRetries": 3
}
```

---

## 🚨 Error Handling

### Common Issues

#### Issue: "Folder not found"
```bash
# Solution: Create folder first
mkdir "C:\WatchFolder"
ZPL2PDF.exe start -l "C:\WatchFolder"
```

#### Issue: "Permission denied"
```bash
# Solution: Run with appropriate permissions
sudo ZPL2PDF start -l "/var/watch"  # Linux
# or run as administrator (Windows)
```

#### Issue: "Daemon already running"
```bash
# Solution: Stop existing daemon first
ZPL2PDF.exe stop
ZPL2PDF.exe start
```

### Recovery Procedures
```bash
# Check daemon status
ZPL2PDF.exe status

# If unresponsive, force stop
ZPL2PDF.exe stop

# Check for orphaned processes
ps aux | grep ZPL2PDF  # Linux
tasklist | findstr ZPL2PDF  # Windows

# Clean restart
ZPL2PDF.exe start
```

---

## 📈 Performance Optimization

### Resource Management
```bash
# Monitor resource usage
top -p $(pgrep ZPL2PDF)  # Linux
tasklist /FI "IMAGENAME eq ZPL2PDF.exe"  # Windows
```

### Batch Processing
```bash
# Process large batches efficiently
ZPL2PDF.exe start -l "./batch_folder" -d 203  # Standard quality for speed
```

### Memory Optimization
```bash
# Restart daemon periodically for long-running processes
# Create cron job (Linux) or scheduled task (Windows)
0 2 * * * ZPL2PDF stop && ZPL2PDF start
```

---

## 🔗 Integration Examples

### Windows Batch Script
```batch
@echo off
echo Starting ZPL2PDF Daemon...
ZPL2PDF.exe start -l "C:\WatchFolder" -w 7.5 -h 15 -u in
echo Daemon started. Press any key to stop.
pause
ZPL2PDF.exe stop
```

### Linux Shell Script
```bash
#!/bin/bash
echo "Starting ZPL2PDF Daemon..."
ZPL2PDF start -l "/var/watch" -w 100 -h 200 -u mm
echo "Daemon started. Press Ctrl+C to stop."
trap 'ZPL2PDF stop; exit' INT
wait
```

### Docker Compose
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    command: start -l /app/watch -w 7.5 -h 15 -u in
    restart: unless-stopped
```

---

## 🔗 Related Topics

- [[Configuration]] - Global settings and preferences
- [[Performance Optimization]] - Speed and resource optimization
- [[Troubleshooting]] - Common issues and solutions
- [[Docker Deployment]] - Container-based deployment
