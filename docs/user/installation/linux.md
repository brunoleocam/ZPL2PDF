# 🐧 Linux Installation Guide

Complete guide for installing ZPL2PDF on Linux distributions.

---

## 🎯 **Installation Methods**

| Method | Ease | Features | Recommended For |
|--------|------|----------|-----------------|
| **Package (.deb/.rpm)** | ⭐⭐⭐⭐⭐ | Native package manager | Most users |
| **Manual Binary** | ⭐⭐⭐ | Direct executable | Advanced users |
| **Docker** | ⭐⭐⭐⭐ | Containerized | Cloud/container users |

---

## 📦 **Method 1: Package Installation (Recommended)**

### **Debian/Ubuntu (.deb package)**

#### **Download and Install**
```bash
# Download from GitHub Releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# Install package
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# Fix dependencies (if needed)
sudo apt-get install -f

# Verify installation
zpl2pdf --help
```

#### **Package Contents**
```
/usr/bin/ZPL2PDF                    # Main executable
/usr/share/doc/zpl2pdf/README.md    # Documentation
/usr/share/doc/zpl2pdf/LICENSE      # License
/usr/share/doc/zpl2pdf/CHANGELOG.md # Changelog
/usr/share/man/man1/zpl2pdf.1.gz    # Man page
/var/zpl2pdf/watch/                 # Default watch folder
/var/zpl2pdf/output/                # Default output folder
```

### **Fedora/CentOS/RHEL (.rpm tarball)**

#### **Download and Install**
```bash
# Download from GitHub Releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# Extract to system
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# Make executable
sudo chmod +x /usr/bin/ZPL2PDF

# Create symbolic link (optional, for lowercase command)
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# Verify installation
zpl2pdf --help
```

#### **Package Contents**
```
/usr/bin/ZPL2PDF                    # Main executable
/usr/share/doc/zpl2pdf/README.md    # Documentation
/usr/share/doc/zpl2pdf/LICENSE      # License
/usr/share/doc/zpl2pdf/CHANGELOG.md # Changelog
/usr/share/man/man1/zpl2pdf.1.gz    # Man page
/var/zpl2pdf/watch/                 # Default watch folder
/var/zpl2pdf/output/                # Default output folder
```

### **Advantages**
- ✅ **Native package** integration
- ✅ **Automatic dependencies** resolution
- ✅ **Clean uninstallation**
- ✅ **Man pages** included
- ✅ **Standard file locations**

---

## 📁 **Method 2: Manual Binary Installation**

### **Download and Extract**
```bash
# Download binary
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Extract
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Make executable
chmod +x ZPL2PDF

# Move to system location (optional)
sudo mv ZPL2PDF /usr/local/bin/
```

### **Create User Directories**
```bash
# Create default folders
mkdir -p ~/Documents/ZPL2PDF\ Auto\ Converter/watch
mkdir -p ~/Documents/ZPL2PDF\ Auto\ Converter/output

# Or use system-wide directories
sudo mkdir -p /var/zpl2pdf/watch
sudo mkdir -p /var/zpl2pdf/output
sudo chown $USER:$USER /var/zpl2pdf/watch
sudo chown $USER:$USER /var/zpl2pdf/output
```

### **Add to PATH (Optional)**
```bash
# Add to user PATH
echo 'export PATH="$PATH:/usr/local/bin"' >> ~/.bashrc
source ~/.bashrc

# Or add to system PATH
sudo ln -s /usr/local/bin/ZPL2PDF /usr/bin/zpl2pdf
```

### **Advantages**
- ✅ **No package manager** required
- ✅ **Portable** installation
- ✅ **Custom location** support
- ✅ **Full control** over installation

---

## 🐳 **Method 3: Docker Installation**

### **Quick Start**
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
- ✅ **Easy updates**
- ✅ **Consistent behavior** across distributions

---

## 🔧 **Dependencies**

### **Required Dependencies**
```bash
# Debian/Ubuntu
sudo apt-get update
sudo apt-get install libgdiplus libc6-dev

# Fedora/CentOS/RHEL
sudo dnf install libgdiplus glibc-devel

# Arch Linux
sudo pacman -S libgdiplus glibc

# Alpine Linux
sudo apk add libgdiplus glibc-dev
```

### **Optional Dependencies**
```bash
# For better font support
sudo apt-get install fonts-noto-cjk  # Debian/Ubuntu
sudo dnf install google-noto-cjk-fonts  # Fedora/CentOS
```

---

## 🌍 **Language Configuration**

### **Environment Variable (Recommended)**
```bash
# Set language permanently
echo 'export ZPL2PDF_LANGUAGE="pt-BR"' >> ~/.bashrc
source ~/.bashrc

# Set language temporarily
export ZPL2PDF_LANGUAGE="es-ES"

# Verify language
ZPL2PDF --show-language
```

### **Configuration File**
```bash
# Create config file
cat > ~/.config/zpl2pdf/zpl2pdf.json <<EOF
{
  "language": "pt-BR",
  "defaultWatchFolder": "/home/user/Documents/ZPL2PDF Auto Converter",
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
ZPL2PDF -help

# Test 2: Status check
ZPL2PDF status

# Test 3: Language display
ZPL2PDF --show-language

# Test 4: Version info
ZPL2PDF --version
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
ZPL2PDF -i test.zpl -o . -n test.pdf -w 10 -h 5 -u cm

# Verify PDF was created
ls -la test.pdf
```

### **Daemon Test**
```bash
# Start daemon
ZPL2PDF start

# Check status
ZPL2PDF status

# Test file processing
echo "^XA^FO50,50^A0N,50,50^FDTest^FS^XZ" > ~/Documents/ZPL2PDF\ Auto\ Converter/watch/test.zpl
sleep 5
ls ~/Documents/ZPL2PDF\ Auto\ Converter/output/

# Stop daemon
ZPL2PDF stop
```

---

## 📊 **Supported Distributions**

### **Fully Tested**
| Distribution | Version | Package | Status |
|-------------|---------|---------|--------|
| **Ubuntu** | 20.04, 22.04, 24.04 | .deb | ✅ |
| **Debian** | 11, 12 | .deb | ✅ |
| **Fedora** | 37, 38, 39 | .rpm | ✅ |
| **CentOS** | 8, 9 | .rpm | ✅ |
| **RHEL** | 8, 9 | .rpm | ✅ |

### **Should Work**
| Distribution | Package | Notes |
|-------------|---------|-------|
| **Arch Linux** | Manual | Use manual installation |
| **openSUSE** | .rpm | Use RPM tarball |
| **Alpine Linux** | Manual | Use manual installation |
| **Amazon Linux** | .rpm | Use RPM tarball |

---

## 🐛 **Troubleshooting**

### **Issue: "libgdiplus not found"**
**Solution:**
```bash
# Install libgdiplus
sudo apt-get install libgdiplus  # Debian/Ubuntu
sudo dnf install libgdiplus      # Fedora/CentOS
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
which ZPL2PDF

# Add to PATH
echo 'export PATH="$PATH:/path/to/zpl2pdf"' >> ~/.bashrc
source ~/.bashrc
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
sudo apt-get install fonts-noto-cjk  # Debian/Ubuntu
sudo dnf install google-noto-cjk-fonts  # Fedora/CentOS

# Check font cache
fc-cache -fv
```

### **Issue: Daemon won't start**
**Solution:**
```bash
# Check if already running
ps aux | grep ZPL2PDF

# Kill existing process
pkill ZPL2PDF

# Check permissions on watch folder
ls -la ~/Documents/ZPL2PDF\ Auto\ Converter/watch/
```

---

## 🔄 **Updates**

### **Package Installation**
```bash
# Download new version
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.1.0/ZPL2PDF-v2.1.0-linux-amd64.deb

# Install over existing version
sudo dpkg -i ZPL2PDF-v2.1.0-linux-amd64.deb
```

### **Manual Installation**
```bash
# Download new version
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.1.0/ZPL2PDF-v2.1.0-linux-x64.tar.gz

# Backup current version
sudo mv /usr/local/bin/ZPL2PDF /usr/local/bin/ZPL2PDF.backup

# Install new version
tar -xzf ZPL2PDF-v2.1.0-linux-x64.tar.gz
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

### **Package Installation**
```bash
# Debian/Ubuntu
sudo apt-get remove zpl2pdf

# Fedora/CentOS/RHEL
sudo dnf remove zpl2pdf
```

### **Manual Installation**
```bash
# Remove executable
sudo rm /usr/local/bin/ZPL2PDF

# Remove symbolic link
sudo rm /usr/bin/zpl2pdf

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
- ✅ **Linux kernel 4.15+** (most modern distributions)
- ✅ **glibc 2.28+** (Ubuntu 18.04+, Debian 10+)
- ✅ **512 MB RAM** (minimum)
- ✅ **100 MB** disk space
- ✅ **libgdiplus** library

### **Recommended Requirements**
- ✅ **2 GB RAM**
- ✅ **1 GB** disk space
- ✅ **Modern CPU** (x64 or ARM64)

### **Architecture Support**
| Architecture | Status | Notes |
|-------------|--------|-------|
| **x64 (amd64)** | ✅ Full | Recommended |
| **ARM64** | ✅ Full | Raspberry Pi 4+ |
| **ARM** | ✅ Limited | Basic support |

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
- [ ] Linux distribution with kernel 4.15+
- [ ] Root/sudo access
- [ ] Internet connection for downloads
- [ ] 100 MB free disk space

### **Installation**
- [ ] Choose installation method (package recommended)
- [ ] Install dependencies (libgdiplus)
- [ ] Complete installation without errors
- [ ] Verify executable is accessible

### **Post-Installation**
- [ ] Run `ZPL2PDF -help` successfully
- [ ] Test language configuration
- [ ] Test basic conversion
- [ ] Test daemon mode
- [ ] Verify default folders created

---

## 🚀 **Next Steps**

1. ✅ **Choose installation method** (package recommended)
2. ✅ **Install dependencies** (libgdiplus)
3. ✅ **Install ZPL2PDF**
4. ✅ **Test installation** with basic commands
5. ✅ **Configure language** (if desired)
6. ✅ **Read [Usage Guide](../usage/)** to learn how to use ZPL2PDF
7. ✅ **Try [Quick Start Guide](../usage/quick-start.md)** for immediate results

**Welcome to ZPL2PDF on Linux!** 🐧🎉
