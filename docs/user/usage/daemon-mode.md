# 🔄 Daemon Mode Guide

Complete guide to using ZPL2PDF in daemon mode for automatic folder monitoring and batch conversion.

---

## 🎯 **Overview**

**Daemon Mode** allows ZPL2PDF to run continuously in the background, automatically monitoring a folder for new ZPL files and converting them to PDF. This is ideal for:

- ✅ **Automated workflows**
- ✅ **Integration with label printing systems**
- ✅ **Unattended batch processing**
- ✅ **Production environments**

---

## 🚀 **Quick Start**

### **Start Daemon with Default Settings**

```bash
ZPL2PDF start
```

This monitors: `Documents/ZPL2PDF Auto Converter`

### **Start with Custom Folder**

```bash
ZPL2PDF start -l "C:\Labels" -o "C:\PDFs"
```

### **Check Status**

```bash
ZPL2PDF status
```

### **Stop Daemon**

```bash
ZPL2PDF stop
```

---

## 📋 **Commands**

| Command | Description | Usage |
|---------|-------------|-------|
| `start` | Start daemon in background | `ZPL2PDF start [options]` |
| `stop` | Stop daemon | `ZPL2PDF stop` |
| `status` | Check daemon status | `ZPL2PDF status` |
| `run` | Run daemon in foreground (testing) | `ZPL2PDF run [options]` |

---

## 🔧 **Options**

| Option | Description | Default | Example |
|--------|-------------|---------|---------|
| `-l <folder>` | Folder to monitor | `Documents/ZPL2PDF Auto Converter` | `-l C:\Watch` |
| `-o <folder>` | Output folder for PDFs | Same as watch folder | `-o C:\Output` |
| `-w <width>` | Fixed width for all conversions | Extract from ZPL | `-w 10` |
| `-h <height>` | Fixed height for all conversions | Extract from ZPL | `-h 15` |
| `-u <unit>` | Unit of measurement | `mm` | `-u cm` |
| `-d <dpi>` | Print density | `203` | `-d 300` |

---

## 🌍 **Default Folders**

### **Watch Folder Locations**

| OS | Default Watch Folder |
|----|---------------------|
| **Windows** | `C:\Users\<username>\Documents\ZPL2PDF Auto Converter` |
| **Linux** | `/home/<username>/Documents/ZPL2PDF Auto Converter` |
| **macOS** | `/Users/<username>/Documents/ZPL2PDF Auto Converter` |

**The folder is automatically created if it doesn't exist.**

---

## 📐 **Dimension Handling**

### **Option 1: Fixed Dimensions (Recommended)**

Use the same dimensions for **all** files:

```bash
ZPL2PDF start -l "C:\Labels" -w 10 -h 15 -u cm
```

**Use when:**
- All labels have the same size
- Consistent output is required
- Performance is critical

### **Option 2: Extract from ZPL (Default)**

Let ZPL2PDF read dimensions from each ZPL file (`^PW`, `^LL`):

```bash
ZPL2PDF start -l "C:\Labels"
```

**Use when:**
- Labels have different sizes
- ZPL files contain dimension commands
- Flexibility is needed

### **Priority Logic**

1. **⭐ Parameters** (`-w`, `-h`) - If provided, use for ALL files
2. **⭐⭐ ZPL Commands** (`^PW`, `^LL`) - If no parameters, extract per file
3. **⭐⭐⭐ Default** (100mm × 150mm) - Fallback if neither available

---

## 🎯 **Usage Examples**

### **Example 1: Basic Daemon**

Start monitoring default folder:

```bash
ZPL2PDF start
```

**What happens:**
1. Creates `Documents/ZPL2PDF Auto Converter` folder
2. Monitors for new `.txt` and `.prn` files
3. Converts each file to PDF
4. Saves PDF in the same folder

### **Example 2: Production Environment**

Monitor specific folders with fixed dimensions:

```bash
ZPL2PDF start -l "C:\Production\Labels" -o "C:\Production\PDFs" -w 4 -h 6 -u in
```

**What happens:**
1. Monitors `C:\Production\Labels`
2. Converts all files with 4" × 6" dimensions
3. Saves PDFs to `C:\Production\PDFs`

### **Example 3: High-Resolution Conversion**

Convert with 300 DPI:

```bash
ZPL2PDF start -l "C:\Labels" -d 300
```

### **Example 4: Multi-Language Setup**

Start daemon with Portuguese interface:

```bash
ZPL2PDF --language pt-BR start -l "C:\Etiquetas"
```

### **Example 5: Testing in Foreground**

Run daemon in foreground (see logs in console):

```bash
ZPL2PDF run -l "C:\Test\Labels" -o "C:\Test\PDFs"
```

**Press `Ctrl+C` to stop.**

---

## 🔄 **Workflow**

### **Complete Workflow**

```
1. ZPL file is created/copied to watch folder
        ↓
2. Daemon detects new file
        ↓
3. Wait for file to be fully written (not locked)
        ↓
4. Read ZPL content
        ↓
5. Extract dimensions (if not using fixed parameters)
        ↓
6. Convert ZPL to PDF
        ↓
7. Save PDF to output folder
        ↓
8. (Optional) Delete source ZPL file
        ↓
9. Continue monitoring
```

### **File Processing**

- ✅ **Supported extensions:** `.txt`, `.prn`
- ✅ **Automatic retry:** If file is locked, retry after delay
- ✅ **Error handling:** Logs errors but continues monitoring
- ✅ **Concurrent processing:** Multiple files processed in parallel

---

## ⚙️ **Configuration**

### **Using Configuration File**

Create `zpl2pdf.json` in the application directory:

```json
{
  "defaultWatchFolder": "C:\\Labels\\Incoming",
  "defaultOutputFolder": "C:\\Labels\\PDFs",
  "labelWidth": 10,
  "labelHeight": 15,
  "unit": "cm",
  "dpi": 203,
  "retryDelay": 2000,
  "maxRetries": 3,
  "processingQueueSize": 10,
  "deleteSourceAfterConversion": false
}
```

Then start daemon without parameters:

```bash
ZPL2PDF start
```

**See:** [Configuration Guide](configuration.md) for all options.

### **Using Environment Variables**

```bash
# Windows (PowerShell)
$env:ZPL2PDF_WATCH_FOLDER = "C:\Labels"
$env:ZPL2PDF_OUTPUT_FOLDER = "C:\PDFs"
ZPL2PDF start

# Linux/macOS
export ZPL2PDF_WATCH_FOLDER=/home/user/labels
export ZPL2PDF_OUTPUT_FOLDER=/home/user/pdfs
ZPL2PDF start
```

---

## 🐳 **Docker Daemon**

### **Run Daemon in Docker**

```bash
docker run -d \
  --name zpl2pdf-daemon \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  --restart unless-stopped \
  brunoleocam/zpl2pdf:latest
```

### **Docker Compose**

Create `docker-compose.yml`:

```yaml
version: '3.8'

services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    container_name: zpl2pdf-daemon
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    environment:
      - ZPL2PDF_LANGUAGE=en-US
      - ZPL2PDF_LOG_LEVEL=Info
    restart: unless-stopped
```

Start:
```bash
docker-compose up -d
```

Check logs:
```bash
docker-compose logs -f zpl2pdf
```

**See:** [Docker Guide](../installation/docker.md) for more details.

---

## 🖥️ **Platform-Specific Setup**

### **Windows: Run as Service**

#### **Option 1: NSSM (Recommended)**

```powershell
# Download NSSM: https://nssm.cc/download

# Install service
nssm install ZPL2PDF "C:\Program Files\ZPL2PDF\ZPL2PDF.exe" start -l "C:\Labels"

# Start service
nssm start ZPL2PDF

# Check status
nssm status ZPL2PDF

# Stop service
nssm stop ZPL2PDF
```

#### **Option 2: Task Scheduler**

1. Open **Task Scheduler**
2. **Create Task** → Name: "ZPL2PDF Daemon"
3. **Triggers** → New → "At startup"
4. **Actions** → New → Start program: `C:\Program Files\ZPL2PDF\ZPL2PDF.exe`
5. **Arguments:** `start -l "C:\Labels"`
6. **OK** to save

### **Linux: systemd Service**

Create `/etc/systemd/system/zpl2pdf.service`:

```ini
[Unit]
Description=ZPL2PDF Daemon
After=network.target

[Service]
Type=simple
User=zpl2pdf
ExecStart=/usr/bin/ZPL2PDF start -l /var/lib/zpl2pdf/watch -o /var/lib/zpl2pdf/output
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

Enable and start:

```bash
sudo systemctl daemon-reload
sudo systemctl enable zpl2pdf
sudo systemctl start zpl2pdf

# Check status
sudo systemctl status zpl2pdf

# View logs
sudo journalctl -u zpl2pdf -f
```

### **macOS: launchd Service**

Create `~/Library/LaunchAgents/com.zpl2pdf.daemon.plist`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.zpl2pdf.daemon</string>
    <key>ProgramArguments</key>
    <array>
        <string>/usr/local/bin/ZPL2PDF</string>
        <string>start</string>
        <string>-l</string>
        <string>/Users/username/Labels</string>
    </array>
    <key>RunAtLoad</key>
    <true/>
    <key>KeepAlive</key>
    <true/>
</dict>
</plist>
```

Load and start:

```bash
launchctl load ~/Library/LaunchAgents/com.zpl2pdf.daemon.plist

# Check status
launchctl list | grep zpl2pdf

# Unload
launchctl unload ~/Library/LaunchAgents/com.zpl2pdf.daemon.plist
```

---

## 🐛 **Troubleshooting**

### **Issue: Daemon Not Starting**

**Check PID file:**

```bash
# Windows
dir $env:TEMP\zpl2pdf.pid

# Linux/macOS
ls -la /var/run/zpl2pdf.pid
```

**Solution:**
```bash
# Remove stale PID file and restart
ZPL2PDF stop
# Manually delete PID file if needed
ZPL2PDF start
```

### **Issue: Files Not Processing**

**Check:**
1. ✅ Daemon is running: `ZPL2PDF status`
2. ✅ Files have correct extension (`.txt`, `.prn`)
3. ✅ Files contain valid ZPL (`^XA...^XZ`)
4. ✅ Watch folder path is correct
5. ✅ Permissions are correct

**Enable debug logging:**
```bash
ZPL2PDF stop
ZPL2PDF --log-level Debug run -l "C:\Labels"
```

### **Issue: High CPU Usage**

**Reduce polling frequency:**

Edit `zpl2pdf.json`:
```json
{
  "fileWatcherInterval": 5000
}
```

### **Issue: Files Locked**

**Increase retry settings:**

Edit `zpl2pdf.json`:
```json
{
  "retryDelay": 5000,
  "maxRetries": 10
}
```

---

## 📊 **Monitoring**

### **Check Daemon Status**

```bash
ZPL2PDF status
```

**Output:**
```
Daemon Status: Running
PID: 12345
Watch Folder: C:\Labels
Output Folder: C:\PDFs
Files Processed: 1234
Uptime: 2 days, 5 hours
```

### **View Logs**

**Windows:**
```powershell
Get-Content "$env:PROGRAMDATA\ZPL2PDF\logs\daemon.log" -Tail 50 -Wait
```

**Linux/macOS:**
```bash
tail -f /var/log/zpl2pdf/daemon.log
```

**Docker:**
```bash
docker logs -f zpl2pdf-daemon
```

---

## 📚 **Related Documentation**

- 🚀 [Quick Start Guide](quick-start.md)
- 🔄 [Conversion Mode Guide](conversion-mode.md)
- ⚙️ [Configuration Guide](configuration.md)
- 🐛 [Troubleshooting Guide](../troubleshooting/common-issues.md)
- 🐳 [Docker Guide](../installation/docker.md)

---

**Daemon mode provides hands-free, automated ZPL to PDF conversion for production environments!** 🚀
