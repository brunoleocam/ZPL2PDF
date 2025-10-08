# 🐧 Linux Installation

Complete guide for installing ZPL2PDF on Linux distributions.

## 🎯 Overview

ZPL2PDF supports multiple installation methods for Linux:
- ✅ **Docker** - Recommended, works on all distributions
- ✅ **Package Managers** - .deb (Debian/Ubuntu) and .tar.gz (Fedora/CentOS/RHEL)
- ✅ **Binary Download** - Direct executable
- ✅ **Build from Source** - For developers

---

## 🐳 Method 1: Docker (Recommended)

### Installation
```bash
# Pull Docker image
docker pull brunoleocam/zpl2pdf:latest

# Verify installation
docker run --rm brunoleocam/zpl2pdf:latest --help
```

### Usage
```bash
# Run conversion
docker run --rm \
  -v $(pwd):/app/watch \
  -v $(pwd)/output:/app/output \
  brunoleocam/zpl2pdf:latest \
  -i /app/watch/label.txt -o /app/output -n label.pdf

# Run daemon mode
docker run -d --name zpl2pdf \
  -v /var/zpl2pdf/watch:/app/watch \
  -v /var/zpl2pdf/output:/app/output \
  brunoleocam/zpl2pdf:latest \
  start -l /app/watch -w 7.5 -h 15 -u in
```

See [[Docker Deployment]] for detailed instructions.

---

## 📦 Method 2: Binary Download

### Ubuntu/Debian
```bash
# Download latest release
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Extract
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Move to system directory
sudo mv ZPL2PDF /usr/local/bin/

# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Install dependencies
sudo apt-get update
sudo apt-get install -y libgdiplus libc6-dev ca-certificates

# Verify installation
ZPL2PDF --help
```

### CentOS/RHEL/Fedora
```bash
# Download latest release
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Extract
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Move to system directory
sudo mv ZPL2PDF /usr/local/bin/

# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Install dependencies
sudo yum install -y libgdiplus glibc-devel ca-certificates

# Verify installation
ZPL2PDF --help
```

### Arch Linux
```bash
# Download latest release
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Extract
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Move to system directory
sudo mv ZPL2PDF /usr/local/bin/

# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Install dependencies
sudo pacman -S libgdiplus glibc ca-certificates

# Verify installation
ZPL2PDF --help
```

---

## 📦 Method 3: Package Managers

### Ubuntu/Debian (.deb package)
```bash
# Download .deb package from GitHub Releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-amd64.deb

# Install package
sudo dpkg -i ZPL2PDF-v2.0.0-linux-amd64.deb

# Fix dependencies if needed
sudo apt-get install -f

# Verify installation
zpl2pdf --help
```

### Fedora/CentOS/RHEL (.tar.gz)
```bash
# Download tarball from GitHub Releases
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz

# Extract to system
sudo tar -xzf ZPL2PDF-v2.0.0-linux-x64-rpm.tar.gz -C /

# Make executable
sudo chmod +x /usr/bin/ZPL2PDF

# Create symbolic link
sudo ln -s /usr/bin/ZPL2PDF /usr/bin/zpl2pdf

# Verify installation
zpl2pdf --help
```

### Arch Linux (AUR - Coming Soon)
```bash
# Using yay (coming soon)
yay -S zpl2pdf

# Or using paru
paru -S zpl2pdf
```

---

## 🔨 Method 4: Build from Source

### Prerequisites
```bash
# Install .NET SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
sudo ./dotnet-install.sh --channel 9.0

# Install dependencies
sudo apt-get install -y libgdiplus libc6-dev git  # Ubuntu/Debian
sudo yum install -y libgdiplus glibc-devel git     # CentOS/RHEL
```

### Clone and Build
```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Build for Linux x64
dotnet publish -c Release -r linux-x64 --self-contained true

# Install
sudo cp bin/Release/net9.0/linux-x64/publish/ZPL2PDF /usr/local/bin/
sudo chmod +x /usr/local/bin/ZPL2PDF

# Verify
ZPL2PDF --help
```

---

## ⚙️ System Requirements

### Minimum Requirements
- **OS**: Ubuntu 18.04+, Debian 10+, CentOS 8+, Fedora 33+
- **Architecture**: x64, ARM64, ARM
- **RAM**: 512 MB
- **Storage**: 100 MB free space
- **Dependencies**: libgdiplus, glibc

### Recommended
- **OS**: Ubuntu 22.04 LTS or Debian 12
- **Architecture**: x64
- **RAM**: 1 GB
- **Storage**: 500 MB free space

---

## 🔧 Post-Installation Configuration

### Set Default Language
```bash
# Add to ~/.bashrc or ~/.zshrc
echo 'export ZPL2PDF_LANGUAGE=pt-BR' >> ~/.bashrc
source ~/.bashrc

# Verify
echo $ZPL2PDF_LANGUAGE
```

### Create Configuration File
```bash
# Create config directory
mkdir -p ~/.config/zpl2pdf

# Create configuration file
cat > ~/.config/zpl2pdf/zpl2pdf.json << 'EOF'
{
  "language": "en-US",
  "defaultWatchFolder": "/var/zpl2pdf/watch",
  "labelWidth": 7.5,
  "labelHeight": 15,
  "unit": "in",
  "dpi": 203,
  "logLevel": "Info"
}
EOF
```

### Test Installation
```bash
# Test with sample ZPL
ZPL2PDF -z '^XA^FO50,50^A0N,50,50^FDHello Linux^FS^XZ' -o /tmp -n test.pdf

# Start daemon mode
ZPL2PDF start -l "/var/zpl2pdf/watch" -w 7.5 -h 15 -u in

# Check status
ZPL2PDF status

# Stop daemon
ZPL2PDF stop
```

---

## 🔄 Updating ZPL2PDF

### Docker Update
```bash
# Pull latest image
docker pull brunoleocam/zpl2pdf:latest

# Restart containers
docker-compose down
docker-compose up -d
```

### Binary Update
```bash
# Stop daemon if running
ZPL2PDF stop

# Download latest version
wget https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-linux-x64.tar.gz

# Extract and replace
tar -xzf ZPL2PDF-v2.0.0-linux-x64.tar.gz
sudo mv -f ZPL2PDF /usr/local/bin/

# Verify new version
ZPL2PDF --version
```

---

## 🗑️ Uninstallation

### Remove Binary
```bash
# Stop daemon
ZPL2PDF stop

# Remove executable
sudo rm /usr/local/bin/ZPL2PDF

# Remove configuration
rm -rf ~/.config/zpl2pdf

# Remove data directories
sudo rm -rf /var/zpl2pdf
```

### Remove Docker
```bash
# Stop and remove containers
docker stop zpl2pdf
docker rm zpl2pdf

# Remove image
docker rmi brunoleocam/zpl2pdf:latest
```

---

## 🏢 systemd Service Setup

### Create Service File
```bash
# Create service file
sudo nano /etc/systemd/system/zpl2pdf.service
```

### Service Configuration
```ini
[Unit]
Description=ZPL2PDF Daemon
After=network.target

[Service]
Type=simple
User=zpl2pdf
WorkingDirectory=/opt/zpl2pdf
ExecStart=/usr/local/bin/ZPL2PDF start -l /var/zpl2pdf/watch -w 7.5 -h 15 -u in
ExecStop=/usr/local/bin/ZPL2PDF stop
Restart=always
RestartSec=10
Environment=ZPL2PDF_LANGUAGE=en-US
Environment=ZPL2PDF_LOG_LEVEL=Info

[Install]
WantedBy=multi-user.target
```

### Enable and Start Service
```bash
# Create user
sudo useradd -r -s /bin/false zpl2pdf

# Create directories
sudo mkdir -p /var/zpl2pdf/{watch,output}
sudo chown -R zpl2pdf:zpl2pdf /var/zpl2pdf

# Reload systemd
sudo systemctl daemon-reload

# Enable service
sudo systemctl enable zpl2pdf

# Start service
sudo systemctl start zpl2pdf

# Check status
sudo systemctl status zpl2pdf

# View logs
sudo journalctl -u zpl2pdf -f
```

---

## 🐛 Troubleshooting

### Issue: "libgdiplus not found"
```bash
# Ubuntu/Debian
sudo apt-get install libgdiplus

# CentOS/RHEL
sudo yum install libgdiplus

# Arch Linux
sudo pacman -S libgdiplus
```

### Issue: "Permission denied"
```bash
# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Check permissions
ls -l /usr/local/bin/ZPL2PDF

# Fix ownership
sudo chown root:root /usr/local/bin/ZPL2PDF
```

### Issue: "Cannot create PID file"
```bash
# Create directory
sudo mkdir -p /var/run/zpl2pdf
sudo chown $USER:$USER /var/run/zpl2pdf

# Or run with sudo
sudo ZPL2PDF start
```

### Issue: "Segmentation fault"
```bash
# Install missing dependencies
sudo apt-get install libc6-dev ca-certificates  # Ubuntu/Debian
sudo yum install glibc-devel ca-certificates    # CentOS/RHEL

# Check architecture compatibility
file /usr/local/bin/ZPL2PDF
uname -m
```

---

## 🔗 Related Topics

- [[Installation Guide]] - General installation overview
- [[Configuration]] - Post-installation configuration
- [[Docker Deployment]] - Container deployment
- [[Troubleshooting]] - Linux-specific issues
