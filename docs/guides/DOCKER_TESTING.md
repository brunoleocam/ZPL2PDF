# 🧪 ZPL2PDF - Testing with Docker on All Operating Systems

This guide shows **exactly** how to test ZPL2PDF on **all operating systems** using Docker, without needing multiple physical machines.

---

## 🎯 Why Test with Docker?

| Without Docker | With Docker |
|----------------|-------------|
| ❌ Need 5+ different PCs | ✅ Test everything on 1 PC |
| ❌ Complex OS installations | ✅ One command per OS |
| ❌ Hard to reproduce bugs | ✅ Exact same environment |
| ❌ Slow testing cycle | ✅ Fast parallel testing |

---

## 📋 Prerequisites

```bash
# Install Docker Desktop
# Windows/Mac: https://www.docker.com/products/docker-desktop
# Linux: 
sudo apt-get update
sudo apt-get install docker.io docker-compose
sudo usermod -aG docker $USER
```

**Verify installation:**
```bash
docker --version
# Output: Docker version 24.0.0 or higher

docker-compose --version
# Output: docker-compose version 2.20.0 or higher
```

---

## 🐧 Test 1: Ubuntu Linux (Debian-based)

### Why Test on Ubuntu?
- ✅ Most popular Linux distribution
- ✅ Used by most Docker images
- ✅ Debian-based (.deb packages)

### Build and Test:

```bash
# 1. Build ZPL2PDF for Linux
docker build -t zpl2pdf:ubuntu-test .

# 2. Run interactive test
docker run -it --rm \
  --name zpl2pdf-ubuntu-test \
  -v ${PWD}/docs/Sample:/app/test:ro \
  -v ${PWD}/test-output:/app/output \
  zpl2pdf:ubuntu-test \
  /bin/bash

# Inside container, run tests:
# ----------------------------
cd /app

# Test 1: Help command
./ZPL2PDF -help

# Test 2: Status check
./ZPL2PDF status

# Test 3: Convert sample file
./ZPL2PDF -i /app/test/example.txt -o /app/output -n ubuntu-test.pdf -w 10 -h 5 -u cm

# Test 4: Check daemon
./ZPL2PDF run -l /app/test &
sleep 5
./ZPL2PDF status

# Exit container
exit
```

### Expected Output:
```
✅ Help should display in English (or configured language)
✅ Status should show "Daemon is not running"
✅ PDF should be created in test-output/
✅ Daemon should start successfully
```

---

## 🔴 Test 2: CentOS/RHEL (Red Hat-based)

### Why Test on CentOS?
- ✅ Popular in enterprise environments
- ✅ RPM-based package manager
- ✅ Different from Debian

### Create Test Dockerfile:

```bash
# Create Dockerfile.centos
cat > Dockerfile.centos <<'EOF'
FROM quay.io/centos/centos:stream9

# Install .NET runtime dependencies
RUN dnf install -y \
    libgdiplus \
    glibc \
    libicu \
    && dnf clean all

# Copy pre-built Linux executable
WORKDIR /app
COPY build/publish/linux-x64/ZPL2PDF .
RUN chmod +x ZPL2PDF

# Create directories
RUN mkdir -p /app/watch /app/output

CMD ["/app/ZPL2PDF", "run", "-l", "/app/watch"]
EOF
```

### Build and Test:

```bash
# 1. Build CentOS version
docker build -f Dockerfile.centos -t zpl2pdf:centos-test .

# 2. Test basic commands
docker run -it --rm zpl2pdf:centos-test /app/ZPL2PDF -help

# 3. Test conversion
docker run --rm \
  -v ${PWD}/docs/Sample:/app/test:ro \
  -v ${PWD}/test-output:/app/output \
  zpl2pdf:centos-test \
  /app/ZPL2PDF -i /app/test/example.txt -o /app/output -n centos-test.pdf
```

---

## 🏔️ Test 3: Alpine Linux (Minimal)

### Why Test on Alpine?
- ✅ Ultra-lightweight (~5MB base)
- ✅ Different C library (musl vs glibc)
- ✅ Popular for Docker production

### Create Test Dockerfile:

```bash
# Create Dockerfile.alpine
cat > Dockerfile.alpine <<'EOF'
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine

# Install required packages
RUN apk add --no-cache \
    libgdiplus \
    libintl \
    icu-libs \
    ttf-dejavu

# Copy pre-built Linux executable
WORKDIR /app
COPY build/publish/linux-x64/ZPL2PDF .
RUN chmod +x ZPL2PDF

# Create directories
RUN mkdir -p /app/watch /app/output

CMD ["/app/ZPL2PDF", "run", "-l", "/app/watch"]
EOF
```

### Build and Test:

```bash
# 1. Build Alpine version
docker build -f Dockerfile.alpine -t zpl2pdf:alpine-test .

# 2. Test and compare image sizes
docker images | grep zpl2pdf

# Expected output:
# zpl2pdf   ubuntu-test   200MB
# zpl2pdf   centos-test   180MB
# zpl2pdf   alpine-test   150MB  ← Smallest!

# 3. Run tests
docker run -it --rm zpl2pdf:alpine-test /app/ZPL2PDF -help
```

---

## 🪟 Test 4: Windows Containers

> **⚠️ Important:** Windows containers **ONLY** run on Windows hosts with Docker Desktop

### Create Test Dockerfile:

```powershell
# Create Dockerfile.windows
@"
FROM mcr.microsoft.com/dotnet/runtime:9.0-windowsservercore-ltsc2022

WORKDIR /app
COPY build/publish/win-x64/ZPL2PDF.exe .

RUN mkdir C:\app\watch C:\app\output

CMD ["ZPL2PDF.exe", "run", "-l", "C:\\app\\watch"]
"@ | Out-File -Encoding ASCII Dockerfile.windows
```

### Build and Test (Windows Only):

```powershell
# 1. Switch Docker to Windows containers
# (Right-click Docker icon → "Switch to Windows containers")

# 2. Build Windows version
docker build -f Dockerfile.windows -t zpl2pdf:windows-test .

# 3. Test
docker run -it --rm zpl2pdf:windows-test ZPL2PDF.exe -help

# 4. Switch back to Linux containers when done
```

---

## 🍎 Test 5: macOS (ARM64 - Apple Silicon)

### Why Test on ARM?
- ✅ New Apple Silicon Macs
- ✅ Raspberry Pi 4
- ✅ AWS Graviton servers

### Test with QEMU (Cross-Platform):

```bash
# 1. Enable multi-platform builds
docker buildx create --use --name multiplatform

# 2. Build for ARM64
docker buildx build \
  --platform linux/arm64 \
  -t zpl2pdf:arm64-test \
  --load \
  .

# 3. Test (automatically uses QEMU if not on ARM)
docker run --rm zpl2pdf:arm64-test /app/ZPL2PDF -help
```

---

## 🚀 Test 6: All Platforms at Once (Automated)

### Create Test Script:

```bash
# Create test-all-platforms.sh
cat > test-all-platforms.sh <<'EOF'
#!/bin/bash
set -e

echo "==================================="
echo "ZPL2PDF - Multi-Platform Tests"
echo "==================================="

# Test Ubuntu
echo "[1/5] Testing Ubuntu..."
docker build -t zpl2pdf:ubuntu-test .
docker run --rm zpl2pdf:ubuntu-test /app/ZPL2PDF -help > /dev/null && echo "✅ Ubuntu OK" || echo "❌ Ubuntu FAILED"

# Test Alpine
echo "[2/5] Testing Alpine..."
docker build -f Dockerfile.alpine -t zpl2pdf:alpine-test .
docker run --rm zpl2pdf:alpine-test /app/ZPL2PDF -help > /dev/null && echo "✅ Alpine OK" || echo "❌ Alpine FAILED"

# Test CentOS
echo "[3/5] Testing CentOS..."
docker build -f Dockerfile.centos -t zpl2pdf:centos-test .
docker run --rm zpl2pdf:centos-test /app/ZPL2PDF -help > /dev/null && echo "✅ CentOS OK" || echo "❌ CentOS FAILED"

# Test Conversion
echo "[4/5] Testing Conversion..."
docker run --rm \
  -v ${PWD}/docs/Sample:/test:ro \
  -v ${PWD}/test-output:/output \
  zpl2pdf:ubuntu-test \
  /app/ZPL2PDF -i /test/example.txt -o /output -n test.pdf
[ -f test-output/test.pdf ] && echo "✅ Conversion OK" || echo "❌ Conversion FAILED"

# Test Daemon
echo "[5/5] Testing Daemon..."
docker run -d --name zpl2pdf-daemon-test \
  -v ${PWD}/watch:/app/watch \
  zpl2pdf:ubuntu-test
sleep 5
docker exec zpl2pdf-daemon-test /app/ZPL2PDF status > /dev/null && echo "✅ Daemon OK" || echo "❌ Daemon FAILED"
docker stop zpl2pdf-daemon-test
docker rm zpl2pdf-daemon-test

echo "==================================="
echo "All tests completed!"
echo "==================================="
EOF

chmod +x test-all-platforms.sh
```

### Run All Tests:

```bash
# Run comprehensive tests
./test-all-platforms.sh
```

---

## 🌍 Test 7: Multi-Language Testing

### Test All Languages:

```bash
# Create language test script
cat > test-languages.sh <<'EOF'
#!/bin/bash

languages=("en-US" "pt-BR" "es-ES" "fr-FR" "de-DE" "it-IT" "ja-JP" "zh-CN")

echo "Testing all languages..."
for lang in "${languages[@]}"; do
    echo -n "Testing $lang... "
    docker run --rm \
      -e ZPL2PDF_LANGUAGE=$lang \
      zpl2pdf:ubuntu-test \
      /app/ZPL2PDF -help | head -1
done
EOF

chmod +x test-languages.sh
./test-languages.sh
```

### Expected Output:
```
Testing en-US... ZPL2PDF - ZPL to PDF Converter
Testing pt-BR... ZPL2PDF - Conversor ZPL para PDF
Testing es-ES... ZPL2PDF - Convertidor de ZPL a PDF
Testing fr-FR... ZPL2PDF - Convertisseur ZPL vers PDF
...
```

---

## 📊 Test Results Comparison

### Create Results Table:

```bash
# Test and compare
cat > compare-platforms.sh <<'EOF'
#!/bin/bash

echo "Platform | Image Size | Startup Time | Memory Usage"
echo "---------|------------|--------------|-------------"

# Ubuntu
size=$(docker images zpl2pdf:ubuntu-test --format "{{.Size}}")
echo "Ubuntu   | $size | Testing... | Testing..."

# Alpine
size=$(docker images zpl2pdf:alpine-test --format "{{.Size}}")
echo "Alpine   | $size | Testing... | Testing..."

# CentOS
size=$(docker images zpl2pdf:centos-test --format "{{.Size}}")
echo "CentOS   | $size | Testing... | Testing..."
EOF

chmod +x compare-platforms.sh
./compare-platforms.sh
```

### Expected Results:
```
Platform | Image Size | Startup Time | Memory Usage
---------|------------|--------------|-------------
Ubuntu   | ~200MB     | ~2s          | ~120MB
Alpine   | ~150MB     | ~1.5s        | ~100MB
CentOS   | ~180MB     | ~2.5s        | ~130MB
```

---

## 🐛 Common Issues and Solutions

### Issue 1: "libgdiplus not found"

```bash
# Solution: Install in Dockerfile
RUN apt-get install -y libgdiplus  # Debian/Ubuntu
RUN dnf install -y libgdiplus      # CentOS/RHEL
RUN apk add --no-cache libgdiplus  # Alpine
```

### Issue 2: Permission denied

```bash
# Solution: Fix permissions
docker exec <container> chmod +x /app/ZPL2PDF
```

### Issue 3: Files not visible

```bash
# Solution: Check volume mounts
docker inspect <container> | grep Mounts -A 10
```

### Issue 4: Wrong architecture

```bash
# Solution: Build for correct platform
docker buildx build --platform linux/amd64 ...
```

---

## ✅ Testing Checklist

Use this checklist for comprehensive testing:

### Basic Functionality:
- [ ] `-help` shows help in correct language
- [ ] `status` reports daemon state
- [ ] `--show-language` displays current language
- [ ] `--set-language` changes language permanently

### Conversion Mode:
- [ ] Convert .txt file to PDF
- [ ] Convert .prn file to PDF
- [ ] Convert with custom dimensions
- [ ] Convert with different units (mm, cm, in)

### Daemon Mode:
- [ ] `start` starts daemon in background
- [ ] `stop` stops running daemon
- [ ] `run` runs in foreground
- [ ] Auto-converts files in watch folder

### Docker-Specific:
- [ ] Container builds successfully
- [ ] Health check passes
- [ ] Volume mounts work correctly
- [ ] Environment variables applied
- [ ] Multi-language support works
- [ ] Container restarts automatically

### Cross-Platform:
- [ ] Works on Ubuntu
- [ ] Works on Alpine
- [ ] Works on CentOS
- [ ] Works on Windows containers (if available)
- [ ] Works on ARM64 (if available)

---

## 🚀 CI/CD Integration

### GitHub Actions Example:

```yaml
name: Docker Multi-Platform Tests

on: [push, pull_request]

jobs:
  test-platforms:
    runs-on: ubuntu-latest
    strategy:
      matrix:
        platform:
          - ubuntu
          - alpine
          - centos
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Build ${{ matrix.platform }}
        run: docker build -f Dockerfile.${{ matrix.platform }} -t zpl2pdf:${{ matrix.platform }}-test .
      
      - name: Test ${{ matrix.platform }}
        run: docker run --rm zpl2pdf:${{ matrix.platform }}-test /app/ZPL2PDF -help
```

---

## 📚 Next Steps

1. ✅ **Run Basic Test**: `docker build -t zpl2pdf:test . && docker run --rm zpl2pdf:test /app/ZPL2PDF -help`
2. ✅ **Test Conversion**: Use sample files from `docs/Sample/`
3. ✅ **Test Daemon**: Run `docker-compose up` and copy files to `watch/`
4. ✅ **Test All Platforms**: Run `./test-all-platforms.sh`
5. ✅ **Test Languages**: Run `./test-languages.sh`

**Happy Testing!** 🎉
