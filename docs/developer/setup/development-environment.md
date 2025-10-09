# 🛠️ Development Environment Setup

Complete guide to setting up a development environment for ZPL2PDF.

---

## 🎯 **Overview**

This guide covers setting up a complete development environment for ZPL2PDF on Windows, Linux, and macOS.

### **What You'll Get**

- ✅ **Full development environment** with all tools
- ✅ **Cross-platform testing** capabilities
- ✅ **Docker integration** for containerized development
- ✅ **CI/CD simulation** locally
- ✅ **Debugging and profiling** tools

---

## 🖥️ **Windows Development Setup**

### **Prerequisites**

| Software | Version | Purpose |
|----------|---------|---------|
| **Windows 10/11** | Latest | Operating system |
| **.NET SDK** | 9.0+ | Core development platform |
| **Visual Studio 2022** | Latest | IDE (recommended) |
| **Git** | Latest | Version control |
| **Docker Desktop** | Latest | Container development |
| **PowerShell** | 7.0+ | Scripting and automation |

### **Step 1: Install .NET SDK**

```powershell
# Download from Microsoft
# https://dotnet.microsoft.com/download/dotnet/9.0

# Verify installation
dotnet --version
# Expected: 9.0.x

# Install additional workloads (optional)
dotnet workload install wasm-tools
```

### **Step 2: Install Visual Studio 2022**

1. **Download**: https://visualstudio.microsoft.com/vs/
2. **Select workloads**:
   - ✅ **.NET desktop development**
   - ✅ **ASP.NET and web development**
   - ✅ **Azure development**
3. **Individual components**:
   - ✅ **Git for Windows**
   - ✅ **GitHub extension for Visual Studio**
   - ✅ **IntelliCode**

### **Step 3: Install Docker Desktop**

```powershell
# Download from Docker
# https://www.docker.com/products/docker-desktop

# Verify installation
docker --version
docker-compose --version

# Test Docker
docker run hello-world
```

### **Step 4: Install Additional Tools**

```powershell
# Install PowerShell 7
winget install Microsoft.PowerShell

# Install Git (if not installed with VS)
winget install Git.Git

# Install Node.js (for some build tools)
winget install OpenJS.NodeJS

# Install Python (for some scripts)
winget install Python.Python.3.11
```

### **Step 5: Clone and Setup Project**

```powershell
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests
dotnet test

# Verify setup
.\scripts\build-all-platforms.ps1 -Platform win-x64
```

---

## 🐧 **Linux Development Setup**

### **Ubuntu/Debian Setup**

#### **Prerequisites Installation**

```bash
# Update package list
sudo apt update

# Install .NET SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-9.0

# Install Git
sudo apt install -y git

# Install Docker
sudo apt install -y docker.io docker-compose
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker $USER

# Install build tools
sudo apt install -y build-essential curl wget

# Install additional tools
sudo apt install -y nodejs npm python3 python3-pip
```

#### **Verify Installation**

```bash
# Check versions
dotnet --version
git --version
docker --version
docker-compose --version

# Test Docker (logout/login required for group changes)
docker run hello-world
```

### **Fedora/CentOS Setup**

```bash
# Install .NET SDK
sudo dnf install -y dotnet-sdk-9.0

# Install Git
sudo dnf install -y git

# Install Docker
sudo dnf install -y docker docker-compose
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker $USER

# Install build tools
sudo dnf groupinstall -y "Development Tools"
sudo dnf install -y curl wget nodejs npm python3 python3-pip
```

### **Arch Linux Setup**

```bash
# Install .NET SDK
sudo pacman -S dotnet-sdk

# Install Git
sudo pacman -S git

# Install Docker
sudo pacman -S docker docker-compose
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker $USER

# Install build tools
sudo pacman -S base-devel curl wget nodejs npm python python-pip
```

### **Project Setup**

```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests
dotnet test

# Verify setup
./scripts/build-all-platforms.sh linux-x64
```

---

## 🍎 **macOS Development Setup**

### **Prerequisites Installation**

```bash
# Install Homebrew (if not installed)
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install .NET SDK
brew install --cask dotnet-sdk

# Install Git
brew install git

# Install Docker Desktop
brew install --cask docker

# Install build tools
brew install curl wget node python3

# Install Xcode Command Line Tools (if not installed)
xcode-select --install
```

### **Verify Installation**

```bash
# Check versions
dotnet --version
git --version
docker --version

# Test Docker
docker run hello-world
```

### **Project Setup**

```bash
# Clone repository
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run tests
dotnet test

# Verify setup
./scripts/build-all-platforms.sh osx-x64
```

---

## 🐳 **Docker Development Setup**

### **Development with Docker**

#### **Option 1: Development Container**

```bash
# Create development container
docker run -it --rm \
  -v $(pwd):/workspace \
  -w /workspace \
  mcr.microsoft.com/dotnet/sdk:9.0 \
  bash

# Inside container
dotnet restore
dotnet build
dotnet test
```

#### **Option 2: Docker Compose Development**

```yaml
# docker-compose.dev.yml
version: '3.8'
services:
  zpl2pdf-dev:
    image: mcr.microsoft.com/dotnet/sdk:9.0
    volumes:
      - .:/workspace
    working_dir: /workspace
    command: bash
    stdin_open: true
    tty: true
```

```bash
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# Enter container
docker-compose -f docker-compose.dev.yml exec zpl2pdf-dev bash

# Inside container
dotnet restore
dotnet build
dotnet test
```

---

## 🔧 **IDE Setup**

### **Visual Studio 2022 (Windows)**

#### **Recommended Extensions**

| Extension | Purpose | Install |
|-----------|---------|---------|
| **GitHub Extension** | Git integration | Built-in |
| **IntelliCode** | AI-assisted coding | Built-in |
| **SonarLint** | Code quality | Extension Manager |
| **GitLens** | Enhanced Git | Extension Manager |
| **CodeMaid** | Code cleanup | Extension Manager |

#### **Project Configuration**

1. **Open solution**: `ZPL2PDF.sln`
2. **Set startup project**: `ZPL2PDF` (Presentation layer)
3. **Configure debugging**: Set command line arguments
4. **Set breakpoints**: In key methods
5. **Run tests**: Test Explorer window

### **VS Code (Cross-Platform)**

#### **Required Extensions**

```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.vscode-dotnet-runtime",
    "ms-vscode.vscode-json",
    "redhat.vscode-yaml",
    "ms-vscode.powershell",
    "eamodio.gitlens",
    "sonarsource.sonarlint-vscode"
  ]
}
```

#### **VS Code Settings**

```json
{
  "dotnet.defaultSolution": "ZPL2PDF.sln",
  "files.exclude": {
    "**/bin": true,
    "**/obj": true,
    "**/.vs": true
  },
  "editor.formatOnSave": true,
  "editor.codeActionsOnSave": {
    "source.organizeImports": true
  }
}
```

### **JetBrains Rider (Cross-Platform)**

#### **Setup Steps**

1. **Install Rider**: https://www.jetbrains.com/rider/
2. **Open solution**: `ZPL2PDF.sln`
3. **Configure Git**: VCS → Git
4. **Set up debugging**: Run/Debug configurations
5. **Install plugins**: Git, Docker, SonarLint

---

## 🧪 **Testing Setup**

### **Unit Testing**

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/ZPL2PDF.Unit/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter ClassName=ConversionServiceTests

# Run with detailed output
dotnet test --verbosity normal
```

### **Integration Testing**

```bash
# Run integration tests
dotnet test tests/ZPL2PDF.Integration/

# Run with test settings
dotnet test tests/ZPL2PDF.Integration/ --settings integration.runsettings

# Run specific category
dotnet test --filter TestCategory=Conversion
```

### **Performance Testing**

```bash
# Run performance tests
dotnet test tests/ZPL2PDF.Performance/

# Run with profiling
dotnet test --collect:"XPlat Code Coverage" --collect:"XPlat Memory"
```

---

## 🔍 **Debugging Setup**

### **Visual Studio Debugging**

#### **Configuration**

1. **Set startup project**: Right-click `ZPL2PDF` → Set as Startup Project
2. **Configure arguments**: Project Properties → Debug → Command line arguments
3. **Set working directory**: Project Properties → Debug → Working directory
4. **Set breakpoints**: Click left margin or F9

#### **Debug Arguments**

```
# Conversion mode
-i "C:\temp\test.zpl" -o "C:\temp\output" -n "test.pdf" -w 4 -h 2 -u in

# Daemon mode
run -l "C:\temp\watch" -o "C:\temp\output"

# Help
--help
```

### **VS Code Debugging**

#### **Launch Configuration**

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch ZPL2PDF",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/ZPL2PDF.dll",
      "args": ["-i", "test.zpl", "-o", "output", "-n", "test.pdf", "-w", "4", "-h", "2", "-u", "in"],
      "cwd": "${workspaceFolder}",
      "console": "internalConsole"
    }
  ]
}
```

### **Command Line Debugging**

```bash
# Debug with dotnet
dotnet run -- -i test.zpl -o output -n test.pdf -w 4 -h 2 -u in

# Debug with specific configuration
dotnet run --configuration Debug -- -i test.zpl -o output -n test.pdf -w 4 -h 2 -u in
```

---

## 📊 **Profiling and Performance**

### **Memory Profiling**

```bash
# Run with memory profiling
dotnet run --configuration Release -- -i large.zpl -o output -n test.pdf -w 4 -h 2 -u in

# Monitor memory usage
# Use Task Manager (Windows) or htop (Linux/macOS)
```

### **Performance Profiling**

```bash
# Run performance tests
dotnet test tests/ZPL2PDF.Performance/ --logger "console;verbosity=detailed"

# Profile specific methods
dotnet run --configuration Release -- -i test.zpl -o output -n test.pdf -w 4 -h 2 -u in
```

---

## 🔧 **Build and Release Tools**

### **Local Build Testing**

```bash
# Build all platforms
# Windows
.\scripts\build-all-platforms.ps1

# Linux/macOS
./scripts/build-all-platforms.sh

# Build specific platform
.\scripts\build-all-platforms.ps1 -Platform win-x64
./scripts/build-all-platforms.sh linux-x64
```

### **Docker Build Testing**

```bash
# Build Docker image
docker build -t zpl2pdf:dev .

# Test Docker image
docker run --rm zpl2pdf:dev --help

# Test with volumes
docker run --rm -v $(pwd)/test:/app/test -v $(pwd)/output:/app/output zpl2pdf:dev -i /app/test/sample.zpl -o /app/output -n test.pdf -w 4 -h 2 -u in
```

### **Package Build Testing**

```bash
# Build Windows installer
.\installer\build-installer.ps1

# Build Linux packages
.\scripts\build-linux-packages.ps1

# Test packages
# Windows: Run installer
# Linux: Install .deb package
```

---

## 🚨 **Troubleshooting**

### **Common Issues**

#### **Issue: "dotnet command not found"**

**Windows:**
```powershell
# Solution: Add to PATH
# Add: C:\Program Files\dotnet\
# Restart PowerShell/Command Prompt
```

**Linux/macOS:**
```bash
# Solution: Source profile
echo 'export PATH=$PATH:/usr/share/dotnet' >> ~/.bashrc
source ~/.bashrc
```

#### **Issue: "Docker permission denied"**

**Linux:**
```bash
# Solution: Add user to docker group
sudo usermod -aG docker $USER
# Logout and login again
```

#### **Issue: "Git authentication failed"**

```bash
# Solution: Configure Git credentials
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# For HTTPS
git config --global credential.helper store

# For SSH
ssh-keygen -t ed25519 -C "your.email@example.com"
# Add public key to GitHub
```

#### **Issue: "Build fails on ARM64"**

```bash
# Solution: Check .NET SDK supports ARM64
dotnet --list-sdks
# Ensure 9.0.x is installed

# For Docker ARM64 on x64 host
docker run --platform linux/arm64 -v $(pwd):/workspace mcr.microsoft.com/dotnet/sdk:9.0
```

---

## 📋 **Development Checklist**

### **Initial Setup**

- [ ] ✅ **Operating system** requirements met
- [ ] ✅ **.NET SDK 9.0+** installed and working
- [ ] ✅ **Git** configured with credentials
- [ ] ✅ **Docker** installed and running
- [ ] ✅ **IDE** configured with extensions
- [ ] ✅ **Project** cloned and built successfully
- [ ] ✅ **Tests** run without errors

### **Daily Development**

- [ ] ✅ **Pull latest changes** (`git pull origin main`)
- [ ] ✅ **Run tests** before committing (`dotnet test`)
- [ ] ✅ **Build project** (`dotnet build`)
- [ ] ✅ **Follow commit conventions**
- [ ] ✅ **Create feature branches**
- [ ] ✅ **Test changes** locally

### **Before Release**

- [ ] ✅ **All tests pass** on all platforms
- [ ] ✅ **Code coverage** meets requirements
- [ ] ✅ **Documentation** updated
- [ ] ✅ **Version numbers** updated
- [ ] ✅ **CHANGELOG.md** updated
- [ ] ✅ **Release process** validated

---

## 🎯 **Next Steps**

1. ✅ **Complete environment setup** following this guide
2. ✅ **Clone and build** ZPL2PDF project
3. ✅ **Run tests** to verify setup
4. ✅ **Configure IDE** with recommended settings
5. ✅ **Create your first feature branch**
6. ✅ **Start contributing** to ZPL2PDF!

---

**A well-configured development environment is essential for productive ZPL2PDF development!** 🚀
