# 🍎 macOS Installation

Complete guide for installing ZPL2PDF on macOS systems.

## 🎯 Overview

ZPL2PDF supports multiple installation methods for macOS:
- ✅ **Homebrew** - Recommended (coming soon)
- ✅ **Binary Download** - Direct executable
- ✅ **Docker** - Containerized deployment
- ✅ **Build from Source** - For developers

---

## 🍺 Method 1: Homebrew (Coming Soon)

### Installation
```bash
# Add tap (coming soon)
brew tap brunoleocam/zpl2pdf

# Install
brew install zpl2pdf

# Verify installation
ZPL2PDF --help
```

### Update
```bash
# Update Homebrew
brew update

# Upgrade ZPL2PDF
brew upgrade zpl2pdf
```

### Uninstall
```bash
# Uninstall
brew uninstall zpl2pdf
```

---

## 📦 Method 2: Binary Download

### Intel Macs (x64)
```bash
# Download latest release
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-x64.tar.gz -o ZPL2PDF.tar.gz

# Extract
tar -xzf ZPL2PDF.tar.gz

# Move to system directory
sudo mv ZPL2PDF /usr/local/bin/

# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Verify installation
ZPL2PDF --help
```

### Apple Silicon (ARM64)
```bash
# Download latest release
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-arm64.tar.gz -o ZPL2PDF.tar.gz

# Extract
tar -xzf ZPL2PDF.tar.gz

# Move to system directory
sudo mv ZPL2PDF /usr/local/bin/

# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Verify installation
ZPL2PDF --help
```

### Remove Quarantine Attribute
```bash
# macOS may block unsigned binaries
sudo xattr -r -d com.apple.quarantine /usr/local/bin/ZPL2PDF

# Or allow in System Preferences
# System Preferences → Security & Privacy → Allow
```

---

## 🐳 Method 3: Docker

### Installation
```bash
# Install Docker Desktop for Mac
# Download from https://www.docker.com/products/docker-desktop

# Pull ZPL2PDF image
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
  -v ~/Documents/ZPL2PDF/watch:/app/watch \
  -v ~/Documents/ZPL2PDF/output:/app/output \
  brunoleocam/zpl2pdf:latest \
  start -l /app/watch -w 7.5 -h 15 -u in
```

See [[Docker Deployment]] for detailed instructions.

---

## 🔨 Method 4: Build from Source

### Prerequisites
```bash
# Install Homebrew (if not installed)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install .NET SDK
brew install --cask dotnet-sdk

# Install Git
brew install git
```

### Clone and Build
```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Build for macOS ARM64 (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained true

# Or build for macOS x64 (Intel)
dotnet publish -c Release -r osx-x64 --self-contained true

# Install
sudo cp bin/Release/net9.0/osx-arm64/publish/ZPL2PDF /usr/local/bin/
sudo chmod +x /usr/local/bin/ZPL2PDF

# Verify
ZPL2PDF --help
```

---

## ⚙️ System Requirements

### Minimum Requirements
- **OS**: macOS 10.15 Catalina or later
- **Architecture**: Intel (x64) or Apple Silicon (ARM64)
- **RAM**: 512 MB
- **Storage**: 100 MB free space
- **Dependencies**: Included in binary

### Recommended
- **OS**: macOS 13 Ventura or later
- **Architecture**: Apple Silicon (ARM64)
- **RAM**: 1 GB
- **Storage**: 500 MB free space

---

## 🔧 Post-Installation Configuration

### Set Default Language
```bash
# Add to ~/.zshrc (default on macOS Catalina+)
echo 'export ZPL2PDF_LANGUAGE=en-US' >> ~/.zshrc
source ~/.zshrc

# Or add to ~/.bash_profile (older macOS)
echo 'export ZPL2PDF_LANGUAGE=en-US' >> ~/.bash_profile
source ~/.bash_profile

# Verify
echo $ZPL2PDF_LANGUAGE
```

### Create Configuration File
```bash
# Create config directory
mkdir -p ~/Library/Application\ Support/ZPL2PDF

# Create configuration file
cat > ~/Library/Application\ Support/ZPL2PDF/zpl2pdf.json << 'EOF'
{
  "language": "en-US",
  "defaultWatchFolder": "~/Documents/ZPL2PDF Auto Converter",
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
ZPL2PDF -z '^XA^FO50,50^A0N,50,50^FDHello macOS^FS^XZ' -o ~/Desktop -n test.pdf

# Start daemon mode
ZPL2PDF start -l "~/Documents/ZPL2PDF/Watch" -w 7.5 -h 15 -u in

# Check status
ZPL2PDF status

# Stop daemon
ZPL2PDF stop
```

---

## 🔄 Updating ZPL2PDF

### Homebrew Update
```bash
# Update Homebrew
brew update

# Upgrade ZPL2PDF
brew upgrade zpl2pdf
```

### Manual Update
```bash
# Stop daemon if running
ZPL2PDF stop

# Download latest version
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-arm64.tar.gz -o ZPL2PDF.tar.gz

# Extract and replace
tar -xzf ZPL2PDF.tar.gz
sudo mv -f ZPL2PDF /usr/local/bin/
sudo chmod +x /usr/local/bin/ZPL2PDF

# Remove quarantine
sudo xattr -r -d com.apple.quarantine /usr/local/bin/ZPL2PDF

# Verify new version
ZPL2PDF --version
```

---

## 🗑️ Uninstallation

### Homebrew Uninstall
```bash
brew uninstall zpl2pdf
```

### Manual Uninstall
```bash
# Stop daemon
ZPL2PDF stop

# Remove executable
sudo rm /usr/local/bin/ZPL2PDF

# Remove configuration
rm -rf ~/Library/Application\ Support/ZPL2PDF

# Remove data directories
rm -rf ~/Documents/ZPL2PDF
```

### Docker Uninstall
```bash
# Stop and remove containers
docker stop zpl2pdf
docker rm zpl2pdf

# Remove image
docker rmi brunoleocam/zpl2pdf:latest
```

---

## 🏢 LaunchAgent Setup (Daemon)

### Create LaunchAgent File
```bash
# Create LaunchAgents directory
mkdir -p ~/Library/LaunchAgents

# Create plist file
nano ~/Library/LaunchAgents/com.brunoleocam.zpl2pdf.plist
```

### LaunchAgent Configuration
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>Label</key>
    <string>com.brunoleocam.zpl2pdf</string>
    
    <key>ProgramArguments</key>
    <array>
        <string>/usr/local/bin/ZPL2PDF</string>
        <string>start</string>
        <string>-l</string>
        <string>~/Documents/ZPL2PDF/Watch</string>
        <string>-w</string>
        <string>7.5</string>
        <string>-h</string>
        <string>15</string>
        <string>-u</string>
        <string>in</string>
    </array>
    
    <key>RunAtLoad</key>
    <true/>
    
    <key>KeepAlive</key>
    <true/>
    
    <key>StandardOutPath</key>
    <string>/tmp/zpl2pdf.log</string>
    
    <key>StandardErrorPath</key>
    <string>/tmp/zpl2pdf-error.log</string>
    
    <key>EnvironmentVariables</key>
    <dict>
        <key>ZPL2PDF_LANGUAGE</key>
        <string>en-US</string>
    </dict>
</dict>
</plist>
```

### Load and Manage LaunchAgent
```bash
# Load agent
launchctl load ~/Library/LaunchAgents/com.brunoleocam.zpl2pdf.plist

# Unload agent
launchctl unload ~/Library/LaunchAgents/com.brunoleocam.zpl2pdf.plist

# Check status
launchctl list | grep zpl2pdf

# View logs
tail -f /tmp/zpl2pdf.log
```

---

## 🐛 Troubleshooting

### Issue: "Cannot be opened because it is from an unidentified developer"
```bash
# Solution: Remove quarantine attribute
sudo xattr -r -d com.apple.quarantine /usr/local/bin/ZPL2PDF

# Or allow in System Preferences
# System Preferences → Security & Privacy → General → Allow
```

### Issue: "Permission denied"
```bash
# Make executable
sudo chmod +x /usr/local/bin/ZPL2PDF

# Check permissions
ls -l /usr/local/bin/ZPL2PDF

# Fix ownership
sudo chown root:wheel /usr/local/bin/ZPL2PDF
```

### Issue: "Command not found"
```bash
# Check if /usr/local/bin is in PATH
echo $PATH

# Add to PATH (zsh)
echo 'export PATH="/usr/local/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc

# Add to PATH (bash)
echo 'export PATH="/usr/local/bin:$PATH"' >> ~/.bash_profile
source ~/.bash_profile
```

### Issue: "Apple Silicon compatibility"
```bash
# Check architecture
uname -m  # Should show "arm64" for Apple Silicon

# Download ARM64 version
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-arm64.tar.gz -o ZPL2PDF.tar.gz

# For Intel Macs, use x64 version
curl -L https://github.com/brunoleocam/ZPL2PDF/releases/download/v2.0.0/ZPL2PDF-v2.0.0-osx-x64.tar.gz -o ZPL2PDF.tar.gz
```

---

## 🔗 Related Topics

- [[Installation Guide]] - General installation overview
- [[Configuration]] - Post-installation configuration
- [[Docker Deployment]] - Container deployment
- [[Troubleshooting]] - macOS-specific issues
