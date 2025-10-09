# 🚀 Quick Start Guide

Get ZPL2PDF up and running in 5 minutes!

---

## 🎯 **What You'll Learn**

- ✅ Install ZPL2PDF on your platform
- ✅ Convert your first ZPL file to PDF
- ✅ Set up automatic file processing
- ✅ Configure your preferred language

---

## 📦 **Step 1: Installation**

### **Windows**
```powershell
# Option 1: WinGet (Recommended)
winget install brunoleocam.ZPL2PDF

# Option 2: Download installer
# Go to: https://github.com/brunoleocam/ZPL2PDF/releases
# Download: ZPL2PDF-Setup-2.0.0.exe
```

### **Linux**
```bash
# Debian/Ubuntu
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# Fedora/CentOS/RHEL
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /
sudo chmod +x /usr/bin/ZPL2PDF
```

### **macOS**
```bash
# Homebrew (Recommended)
brew tap brunoleocam/zpl2pdf
brew install zpl2pdf

# Manual
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-x64.tar.gz
tar -xzf ZPL2PDF-v2.0.0-osx-x64.tar.gz
sudo mv ZPL2PDF /usr/local/bin/
```

### **Docker (Any Platform)**
```bash
# Pull and run
docker pull brunoleocam/zpl2pdf:latest
mkdir watch output
docker run -d --name zpl2pdf -v ./watch:/app/watch -v ./output:/app/output brunoleocam/zpl2pdf:latest
```

---

## 🧪 **Step 2: Test Installation**

```bash
# Check if installed correctly
ZPL2PDF -help

# Expected output: Help text in your language
```

---

## 📄 **Step 3: Convert Your First File**

### **Create a Test ZPL File**
```bash
# Create test.zpl
cat > test.zpl <<EOF
^XA
^FO50,50^A0N,50,50^FDHello World^FS
^FO50,120^A0N,30,30^FDZPL2PDF Test^FS
^FO50,180^A0N,20,20^FDQuick Start Guide^FS
^XZ
EOF
```

### **Convert to PDF**
```bash
# Basic conversion
ZPL2PDF -i test.zpl -o . -n my-first-label.pdf

# With custom dimensions
ZPL2PDF -i test.zpl -o . -n my-first-label.pdf -w 10 -h 5 -u cm

# Expected result: my-first-label.pdf created
```

### **Verify PDF**
```bash
# Check if PDF was created
ls -la *.pdf

# Open PDF to verify content
# Windows: start my-first-label.pdf
# Linux: xdg-open my-first-label.pdf
# macOS: open my-first-label.pdf
```

---

## 🔄 **Step 4: Automatic Processing (Daemon Mode)**

### **Start Daemon**
```bash
# Start monitoring mode
ZPL2PDF start

# Check status
ZPL2PDF status

# Expected output: "Daemon is running"
```

### **Test Automatic Conversion**
```bash
# Copy ZPL file to watch folder
cp test.zpl ~/Documents/ZPL2PDF\ Auto\ Converter/watch/

# Wait a few seconds, then check output folder
ls ~/Documents/ZPL2PDF\ Auto\ Converter/output/

# Expected result: PDF file automatically created
```

### **Stop Daemon**
```bash
# Stop monitoring
ZPL2PDF stop

# Check status
ZPL2PDF status

# Expected output: "Daemon is not running"
```

---

## 🌍 **Step 5: Configure Language**

### **Set Language (Temporary)**
```bash
# Windows PowerShell
$env:ZPL2PDF_LANGUAGE = "pt-BR"
ZPL2PDF -help

# Linux/macOS
export ZPL2PDF_LANGUAGE="es-ES"
ZPL2PDF -help
```

### **Set Language (Permanent)**
```bash
# Windows
setx ZPL2PDF_LANGUAGE "pt-BR"

# Linux/macOS
echo 'export ZPL2PDF_LANGUAGE="pt-BR"' >> ~/.bashrc
source ~/.bashrc
```

### **Available Languages**
- 🇺🇸 English (en-US)
- 🇧🇷 Portuguese (pt-BR)
- 🇪🇸 Spanish (es-ES)
- 🇫🇷 French (fr-FR)
- 🇩🇪 German (de-DE)
- 🇮🇹 Italian (it-IT)
- 🇯🇵 Japanese (ja-JP)
- 🇨🇳 Chinese (zh-CN)

---

## 🎯 **Common Use Cases**

### **1. Single File Conversion**
```bash
# Convert one file
ZPL2PDF -i label.zpl -o output/ -n converted.pdf -w 10 -h 5 -u cm
```

### **2. Batch Conversion**
```bash
# Start daemon
ZPL2PDF start

# Copy multiple files to watch folder
cp *.zpl ~/Documents/ZPL2PDF\ Auto\ Converter/watch/

# All files automatically converted to PDFs
```

### **3. Custom Dimensions**
```bash
# Small label (2x1 inches)
ZPL2PDF -i small.zpl -o . -n small.pdf -w 2 -h 1 -u in

# Large label (10x15 cm)
ZPL2PDF -i large.zpl -o . -n large.pdf -w 10 -h 15 -u cm
```

### **4. Different Units**
```bash
# Millimeters
ZPL2PDF -i label.zpl -o . -n mm.pdf -w 100 -h 50 -u mm

# Centimeters
ZPL2PDF -i label.zpl -o . -n cm.pdf -w 10 -h 5 -u cm

# Inches
ZPL2PDF -i label.zpl -o . -n in.pdf -w 4 -h 2 -u in
```

---

## 🔧 **Configuration Options**

### **Command Line Parameters**
| Parameter | Description | Example |
|-----------|-------------|---------|
| `-i` | Input ZPL file | `-i label.zpl` |
| `-o` | Output directory | `-o ./output/` |
| `-n` | Output filename | `-n my-label.pdf` |
| `-w` | Label width | `-w 10` |
| `-h` | Label height | `-h 5` |
| `-u` | Unit (mm/cm/in) | `-u cm` |

### **Daemon Commands**
| Command | Description |
|---------|-------------|
| `ZPL2PDF start` | Start daemon mode |
| `ZPL2PDF stop` | Stop daemon mode |
| `ZPL2PDF status` | Check daemon status |
| `ZPL2PDF run` | Run in foreground |

### **Configuration Commands**
| Command | Description |
|---------|-------------|
| `ZPL2PDF --show-language` | Show current language |
| `ZPL2PDF --set-language pt-BR` | Set language |
| `ZPL2PDF --version` | Show version |

---

## 🐛 **Troubleshooting**

### **Issue: "Command not found"**
**Solution:**
- **Windows**: Restart command prompt
- **Linux/macOS**: Add to PATH or use full path
- **Docker**: Use `docker exec zpl2pdf /app/ZPL2PDF`

### **Issue: "Permission denied"**
**Solution:**
- **Linux/macOS**: `chmod +x ZPL2PDF`
- **Windows**: Run as administrator

### **Issue: "PDF not created"**
**Solution:**
- Check input file exists and is readable
- Verify output directory exists and is writable
- Check ZPL file format is correct

### **Issue: "Daemon won't start"**
**Solution:**
- Check if already running: `ZPL2PDF status`
- Stop existing daemon: `ZPL2PDF stop`
- Check watch folder permissions

---

## 📚 **Next Steps**

### **Learn More**
- 📖 **[Conversion Mode Guide](conversion-mode.md)** - Detailed conversion options
- 🔄 **[Daemon Mode Guide](daemon-mode.md)** - Automatic file processing
- ⚙️ **[Configuration Guide](configuration.md)** - Advanced settings

### **Advanced Features**
- 🌍 **Multi-language support** - 8 languages available
- 🐳 **Docker deployment** - Container-based installation
- 📦 **Package management** - WinGet, Homebrew, APT, YUM

### **Get Help**
- 🐛 **[Troubleshooting](../troubleshooting/)** - Common issues and solutions
- 💬 **[GitHub Discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)** - Community support
- 📧 **Contact maintainer** - Direct support

---

## ✅ **Quick Start Checklist**

- [ ] ✅ **Install ZPL2PDF** on your platform
- [ ] ✅ **Test installation** with `ZPL2PDF -help`
- [ ] ✅ **Create test ZPL file** with sample content
- [ ] ✅ **Convert to PDF** successfully
- [ ] ✅ **Start daemon mode** and test automatic conversion
- [ ] ✅ **Configure language** (if desired)
- [ ] ✅ **Verify output** PDF files are correct

---

## 🎉 **Congratulations!**

You've successfully:
- ✅ Installed ZPL2PDF
- ✅ Converted your first ZPL file to PDF
- ✅ Set up automatic file processing
- ✅ Configured your preferred language

**You're now ready to use ZPL2PDF for all your ZPL to PDF conversion needs!**

---

## 🚀 **Pro Tips**

1. **Use daemon mode** for continuous file processing
2. **Set language permanently** for consistent experience
3. **Use appropriate dimensions** for your label sizes
4. **Check output folder** regularly for converted files
5. **Use Docker** for isolated, portable deployments

**Happy Converting!** 🎉
