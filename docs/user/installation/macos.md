# 🍎 macOS Installation Guide

Complete guide for installing ZPL2PDF on macOS.

---

## 🎯 **Installation Methods**

| Method | Ease | Features | Recommended For |
|--------|------|----------|-----------------|
| **Homebrew** | ⭐⭐⭐⭐⭐ | Package manager, auto-updates | Most users |
| **Manual Binary** | ⭐⭐⭐ | Direct executable | Advanced users |
| **Docker** | ⭐⭐⭐⭐ | Containerized | Development |

---

## 🍺 **Method 1: Homebrew (Recommended)**

### **Install Homebrew (if not installed)**
```bash
# Install Homebrew
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Add to PATH (if needed)
echo 'eval "$(/opt/homebrew/bin/brew shellenv)"' >> ~/.zprofile
source ~/.zprofile
```

### **Install ZPL2PDF**
```bash
# Add tap (repository)
brew tap brunoleocam/zpl2pdf

# Install ZPL2PDF
brew install zpl2pdf

# Verify installation
zpl2pdf --help
```

### **Update**
```bash
# Update to latest version
brew upgrade zpl2pdf
```

### **Uninstall**
```bash
# Remove completely
brew uninstall zpl2pdf
brew untap brunoleocam/zpl2pdf
```

### **Advantages**
- ✅ **Automatic updates** via `brew upgrade`
- ✅ **Dependency management** (libgdiplus, etc.)
- ✅ **Clean uninstallation**
- ✅ **Integration** with Homebrew ecosystem

---

## 📁 **Method 2: Manual Binary Installation**

### **Download and Install**
```bash
# Download for Intel Mac
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-x64.tar.gz

# Download for Apple Silicon (M1/M2/M3)
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-arm64.tar.gz

# Extract
tar -xzf ZPL2PDF-v2.0.0-osx-x64.tar.gz  # Intel
# or
tar -xzf ZPL2PDF-v2.0.0-osx-arm64.tar.gz  # Apple Silicon

# Make executable
chmod +x ZPL2PDF

# Move to system location
sudo mv ZPL2PDF /usr/local/bin/

# Create symbolic link (optional)
sudo ln -s /usr/local/bin/ZPL2PDF /usr/local/bin/zpl2pdf
```

### **Create User Directories**
```bash
# Create default folders
mkdir -p ~/Documents/ZPL2PDF\ Auto\ Converter/watch
mkdir -p ~/Documents/ZPL2PDF\ Auto\ Converter/output
```

### **Add to PATH (if needed)**
```bash
# Add to user PATH
echo 'export PATH="$PATH:/usr/local/bin"' >> ~/.zprofile
source ~/.zprofile
```

### **Advantages**
- ✅ **No package manager** required
- ✅ **Direct control** over installation
- ✅ **Works offline** after download

---

## 🐳 **Method 3: Docker Installation**

### **Install Docker Desktop**
1. Download [Docker Desktop for Mac](https://www.docker.com/products/docker-desktop)
2. Install and start Docker Desktop
3. Verify installation:
   ```bash
   docker --version
   ```

### **Run ZPL2PDF in Docker**
```bash
# Pull image
docker pull brunoleocam/zpl2pdf:latest

# Create directories
mkdir watch output

# Run daemon mode
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  brunoleocam/zpl2pdf:latest
```

### **Docker Compose**
```yaml
# docker-compose.yml
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
    
    restart: unless-stopped
```

```bash
# Start with docker-compose
docker-compose up -d
```

### **Advantages**
- ✅ **Isolated environment**
- ✅ **No dependencies** on host system
- ✅ **Consistent behavior** across platforms

---

## 🔧 **Dependencies**

### **Required Dependencies**

#### **For Native Installation**
```bash
# Install libgdiplus (if not already installed)
brew install libgdiplus

# Or install via MacPorts
sudo port install libgdiplus
```

#### **For Docker Installation**
No additional dependencies needed - everything is included in the container.

### **Optional Dependencies**
```bash
# For better font support
brew install font-noto-cjk

# For additional utilities
brew install wget curl
```

---

## 🌍 **Language Configuration**

### **Environment Variable (Recommended)**
```bash
# Set language permanently
echo 'export ZPL2PDF_LANGUAGE="pt-BR"' >> ~/.zprofile
source ~/.zprofile

# Set language temporarily
export ZPL2PDF_LANGUAGE="es-ES"

# Verify language
zpl2pdf --show-language
```

### **Configuration File**
```bash
# Create config directory
mkdir -p ~/.config/zpl2pdf

# Create config file
cat > ~/.config/zpl2pdf/zpl2pdf.json <<EOF
{
  "language": "pt-BR",
  "defaultWatchFolder": "/Users/user/Documents/ZPL2PDF Auto Converter",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203
}
EOF
```

### **System-wide Configuration**
```bash
# Create global config
sudo mkdir -p /etc/zpl2pdf
sudo cat > /etc/zpl2pdf/zpl2pdf.json <<EOF
{
  "language": "en-US",
  "defaultWatchFolder": "/var/zpl2pdf/watch",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203
}
EOF
```

---

## 🧪 **Testing Installation**

### **Basic Tests**
```bash
# Test 1: Help command
zpl2pdf -help

# Test 2: Status check
zpl2pdf status

# Test 3: Language display
zpl2pdf --show-language

# Test 4: Version info
zpl2pdf --version
```

### **Conversion Test**
```bash
# Create test file
cat > test.zpl <<EOF
^XA
^FO50,50^A0N,50,50^FDTest Label^FS
^XZ
EOF

# Convert to PDF
zpl2pdf -i test.zpl -o . -n test.pdf -w 10 -h 5 -u cm

# Verify PDF was created
ls -la test.pdf
```

### **Daemon Test**
```bash
# Start daemon
zpl2pdf start

# Check status
zpl2pdf status

# Test file processing
echo "^XA^FO50,50^A0N,50,50^FDTest^FS^XZ" > ~/Documents/ZPL2PDF\ Auto\ Converter/watch/test.zpl
sleep 5
ls ~/Documents/ZPL2PDF\ Auto\ Converter/output/

# Stop daemon
zpl2pdf stop
```

---

## 📊 **macOS Version Support**

### **Supported Versions**
| macOS Version | Support | Notes |
|---------------|---------|-------|
| **macOS 14 (Sonoma)** | ✅ Full | Recommended |
| **macOS 13 (Ventura)** | ✅ Full | Full support |
| **macOS 12 (Monterey)** | ✅ Full | Full support |
| **macOS 11 (Big Sur)** | ✅ Limited | May work |
| **macOS 10.15 (Catalina)** | ⚠️ Limited | End of life |

### **Architecture Support**
| Architecture | Status | Notes |
|-------------|--------|-------|
| **Apple Silicon (ARM64)** | ✅ Full | M1, M2, M3 Macs |
| **Intel x64** | ✅ Full | Intel Macs |

---

## 🐛 **Troubleshooting**

### **Issue: "libgdiplus not found"**
**Solution:**
```bash
# Install via Homebrew
brew install libgdiplus

# Or install via MacPorts
sudo port install libgdiplus

# Check installation
brew list libgdiplus
```

### **Issue: "Permission denied"**
**Solution:**
```bash
# Make executable
chmod +x ZPL2PDF

# Check permissions
ls -la ZPL2PDF
```

### **Issue: "Command not found"**
**Solution:**
```bash
# Check if in PATH
which zpl2pdf

# Add to PATH
echo 'export PATH="$PATH:/usr/local/bin"' >> ~/.zprofile
source ~/.zprofile
```

### **Issue: "Gatekeeper blocked"**
**Solution:**
```bash
# Allow execution (one-time)
sudo xattr -rd com.apple.quarantine ZPL2PDF

# Or run with explicit permission
sudo spctl --add ZPL2PDF
```

### **Issue: Language not working**
**Solution:**
```bash
# Check environment variable
echo $ZPL2PDF_LANGUAGE

# Set language
export ZPL2PDF_LANGUAGE="pt-BR"

# Check locale support
locale -a | grep pt
```

### **Issue: Font rendering problems**
**Solution:**
```bash
# Install fonts
brew install font-noto-cjk

# Check font cache
fc-cache -fv
```

### **Issue: Docker not working**
**Solution:**
```bash
# Check Docker Desktop is running
docker info

# Restart Docker Desktop
# Or check Docker Desktop settings
```

---

## 🔄 **Updates**

### **Homebrew Installation**
```bash
# Update Homebrew
brew update

# Update ZPL2PDF
brew upgrade zpl2pdf
```

### **Manual Installation**
```bash
# Download new version
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.1.0/ZPL2PDF-v2.1.0-osx-x64.tar.gz

# Backup current version
sudo mv /usr/local/bin/ZPL2PDF /usr/local/bin/ZPL2PDF.backup

# Install new version
tar -xzf ZPL2PDF-v2.1.0-osx-x64.tar.gz
sudo mv ZPL2PDF /usr/local/bin/
```

### **Docker Installation**
```bash
# Pull latest image
docker pull brunoleocam/zpl2pdf:latest

# Restart container
docker-compose down
docker-compose up -d
```

---

## 🗑️ **Uninstallation**

### **Homebrew Installation**
```bash
# Remove ZPL2PDF
brew uninstall zpl2pdf

# Remove tap
brew untap brunoleocam/zpl2pdf
```

### **Manual Installation**
```bash
# Remove executable
sudo rm /usr/local/bin/ZPL2PDF

# Remove symbolic link
sudo rm /usr/local/bin/zpl2pdf

# Remove user directories (optional)
rm -rf ~/Documents/ZPL2PDF\ Auto\ Converter/
```

### **Docker Installation**
```bash
# Stop and remove container
docker-compose down
docker rm zpl2pdf

# Remove image (optional)
docker rmi brunoleocam/zpl2pdf:latest
```

---

## 📊 **System Requirements**

### **Minimum Requirements**
- ✅ **macOS 11.0+** (Big Sur or later)
- ✅ **Apple Silicon** (M1/M2/M3) or **Intel x64**
- ✅ **4 GB RAM** (recommended)
- ✅ **100 MB** disk space
- ✅ **libgdiplus** library

### **Recommended Requirements**
- ✅ **macOS 13.0+** (Ventura or later)
- ✅ **8 GB RAM**
- ✅ **1 GB** disk space
- ✅ **Modern CPU** (Apple Silicon preferred)

---

## 🔐 **Security Considerations**

### **Gatekeeper**
macOS may block execution of unsigned binaries:
```bash
# Allow execution (one-time)
sudo xattr -rd com.apple.quarantine ZPL2PDF

# Or run with explicit permission
sudo spctl --add ZPL2PDF
```

### **Notarization**
For production use, consider code signing and notarization:
- Code signing certificate
- Notarization with Apple
- Distribution via App Store or Developer ID

---

## 📚 **Additional Resources**

- **[Usage Guide](../usage/)** - How to use ZPL2PDF
- **[Configuration Guide](../usage/configuration.md)** - Advanced configuration
- **[Docker Installation](docker.md)** - Container installation
- **[Troubleshooting](../troubleshooting/)** - Common issues and solutions
- **[GitHub Repository](https://github.com/brunoleocam/ZPL2PDF)** - Source code

---

## ✅ **Installation Checklist**

### **Pre-Installation**
- [ ] macOS 11.0+ (Big Sur or later)
- [ ] Admin privileges (for system-wide install)
- [ ] Internet connection for downloads
- [ ] 100 MB free disk space

### **Installation**
- [ ] Choose installation method (Homebrew recommended)
- [ ] Install dependencies (libgdiplus)
- [ ] Complete installation without errors
- [ ] Verify executable is accessible

### **Post-Installation**
- [ ] Run `zpl2pdf -help` successfully
- [ ] Test language configuration
- [ ] Test basic conversion
- [ ] Test daemon mode
- [ ] Verify default folders created

---

## 🚀 **Next Steps**

1. ✅ **Choose installation method** (Homebrew recommended)
2. ✅ **Install dependencies** (libgdiplus)
3. ✅ **Install ZPL2PDF**
4. ✅ **Test installation** with basic commands
5. ✅ **Configure language** (if desired)
6. ✅ **Read [Usage Guide](../usage/)** to learn how to use ZPL2PDF
7. ✅ **Try [Quick Start Guide](../usage/quick-start.md)** for immediate results

**Welcome to ZPL2PDF on macOS!** 🍎🎉
