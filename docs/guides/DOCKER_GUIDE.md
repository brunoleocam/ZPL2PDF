# 🐳 ZPL2PDF - Docker Complete Guide

Complete guide for using ZPL2PDF with Docker, including testing on all operating systems.

---

## 📋 Table of Contents

1. [Quick Start](#-quick-start)
2. [Understanding Docker Files](#-understanding-docker-files)
3. [Testing on All Operating Systems](#-testing-on-all-operating-systems)
4. [Usage Examples](#-usage-examples)
5. [Multi-Language Support](#-multi-language-support)
6. [Troubleshooting](#-troubleshooting)

---

## 🚀 Quick Start

### Prerequisites

```bash
# Install Docker Desktop
# Windows/Mac: https://www.docker.com/products/docker-desktop
# Linux: sudo apt-get install docker.io docker-compose
```

### 1. Build the Image

```bash
# Build ZPL2PDF Docker image
docker build -t zpl2pdf:2.0.0 .
```

### 2. Run Daemon Mode

```bash
# Create watch and output folders
mkdir watch output

# Run in daemon mode
docker run -d \
  --name zpl2pdf-daemon \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  zpl2pdf:2.0.0
```

### 3. Check Status

```bash
# View logs
docker logs zpl2pdf-daemon

# Check health
docker exec zpl2pdf-daemon /app/ZPL2PDF status
```

---

## 📁 Understanding Docker Files

### **Dockerfile** = Build Recipe

Defines **HOW** to build the ZPL2PDF container:

```dockerfile
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
COPY . .
RUN dotnet publish --runtime linux-x64

# Stage 2: Create minimal runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0
COPY --from=build /app/publish/ZPL2PDF .
CMD ["/app/ZPL2PDF", "run", "-l", "/app/watch"]
```

**Benefits:**
- ✅ Multi-stage build (smaller image ~200MB vs 1GB)
- ✅ Self-contained executable (no .NET runtime needed)
- ✅ Non-root user for security
- ✅ Health checks built-in

### **docker-compose.yml** = Usage Configuration

Defines **HOW TO USE** the container:

```yaml
services:
  zpl2pdf:
    build: .
    volumes:
      - ./watch:/app/watch  # Your folder -> Container folder
      - ./output:/app/output
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR  # Set language
    command: run -l /app/watch
    restart: unless-stopped
```

**Benefits:**
- ✅ Easy configuration management
- ✅ Multiple services in one file
- ✅ One command to start everything
- ✅ Environment variables

### **.dockerignore** = Optimization

Excludes unnecessary files from Docker build:

```
bin/
obj/
*.md
tests/
```

**Benefits:**
- ✅ Faster builds (less files to copy)
- ✅ Smaller images
- ✅ Better security (no secrets)

---

## 🧪 Testing on All Operating Systems

### Why Test with Docker?

**WITHOUT Docker:**
- ❌ Need separate PC for each OS
- ❌ Complex setup for each platform
- ❌ Hard to reproduce issues

**WITH Docker:**
- ✅ Test Linux on Windows!
- ✅ Test different distributions
- ✅ Reproducible environment
- ✅ Same as production

### 🐧 **Option 1: Test on Linux (Ubuntu)**

```bash
# Build for Linux
docker build -t zpl2pdf:linux-test .

# Run interactive shell
docker run -it --rm \
  -v ./docs/Sample:/app/test \
  zpl2pdf:linux-test \
  /bin/bash

# Inside container, test commands:
/app/ZPL2PDF -help
/app/ZPL2PDF -i /app/test/example.txt -o /tmp -n test.pdf
/app/ZPL2PDF status
```

### 🍎 **Option 2: Test on Alpine Linux (Lightweight)**

Create `Dockerfile.alpine`:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS runtime

# Install required packages
RUN apk add --no-cache \
    libgdiplus \
    libintl \
    icu-libs

WORKDIR /app
COPY build/publish/linux-x64/ZPL2PDF .
RUN chmod +x /app/ZPL2PDF

CMD ["/app/ZPL2PDF", "run", "-l", "/app/watch"]
```

```bash
# Build Alpine version
docker build -f Dockerfile.alpine -t zpl2pdf:alpine .

# Test
docker run -it --rm zpl2pdf:alpine /app/ZPL2PDF -help
```

### 🔴 **Option 3: Test on CentOS/RHEL**

Create `Dockerfile.centos`:

```dockerfile
FROM centos:8

# Install .NET runtime dependencies
RUN dnf install -y \
    libgdiplus \
    glibc \
    libicu

WORKDIR /app
COPY build/publish/linux-x64/ZPL2PDF .
RUN chmod +x /app/ZPL2PDF

CMD ["/app/ZPL2PDF", "run", "-l", "/app/watch"]
```

```bash
# Build CentOS version
docker build -f Dockerfile.centos -t zpl2pdf:centos .

# Test
docker run -it --rm zpl2pdf:centos /app/ZPL2PDF -help
```

### 🪟 **Option 4: Test Windows Containers (Windows Only)**

> **Note:** Windows containers only run on Windows hosts

Create `Dockerfile.windows`:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0-windowsservercore-ltsc2022

WORKDIR /app
COPY build/publish/win-x64/ZPL2PDF.exe .

CMD ["ZPL2PDF.exe", "run", "-l", "C:\\app\\watch"]
```

```powershell
# Build Windows container
docker build -f Dockerfile.windows -t zpl2pdf:windows .

# Test
docker run -it --rm zpl2pdf:windows ZPL2PDF.exe -help
```

### 🧪 **Option 5: Automated Testing with Docker Compose**

```bash
# Run tests in container
docker-compose --profile test up

# Run tests for specific language
docker run --rm \
  -v ./tests:/src/tests \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  zpl2pdf:2.0.0 \
  dotnet test
```

---

## 💡 Usage Examples

### Example 1: Daemon Mode (Auto-Convert)

```bash
# Using docker-compose (EASIEST)
docker-compose up -d

# Using docker run
docker run -d \
  --name zpl2pdf-daemon \
  -v C:/ZPL:/app/watch \
  -v C:/PDF:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  --restart unless-stopped \
  zpl2pdf:2.0.0
```

### Example 2: Convert Single File

```bash
# Copy file and convert
docker run --rm \
  -v ./input:/app/input:ro \
  -v ./output:/app/output \
  zpl2pdf:2.0.0 \
  /app/ZPL2PDF -i /app/input/label.txt -o /app/output -n result.pdf -w 10 -h 5 -u cm
```

### Example 3: Batch Conversion

```bash
# Convert all files in a folder
docker run --rm \
  -v ./zpl-files:/app/watch:ro \
  -v ./pdf-output:/app/output \
  zpl2pdf:2.0.0 \
  /app/ZPL2PDF run -l /app/watch
```

### Example 4: Multi-Instance (Different Languages)

```bash
# Start English instance
docker run -d \
  --name zpl2pdf-en \
  -v ./watch-en:/app/watch \
  -e ZPL2PDF_LANGUAGE=en-US \
  zpl2pdf:2.0.0

# Start Portuguese instance
docker run -d \
  --name zpl2pdf-pt \
  -v ./watch-pt:/app/watch \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  zpl2pdf:2.0.0

# Start Spanish instance
docker run -d \
  --name zpl2pdf-es \
  -v ./watch-es:/app/watch \
  -e ZPL2PDF_LANGUAGE=es-ES \
  zpl2pdf:2.0.0
```

### Example 5: Production Deployment

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  zpl2pdf:
    image: zpl2pdf:2.0.0
    container_name: zpl2pdf-prod
    
    volumes:
      - /srv/zpl/watch:/app/watch
      - /srv/zpl/output:/app/output
      - /srv/zpl/config:/app/config
    
    environment:
      - ZPL2PDF_LANGUAGE=en-US
    
    command: run -l /app/watch
    
    restart: always
    
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 1G
        reservations:
          cpus: '1.0'
          memory: 512M
    
    healthcheck:
      test: ["/app/ZPL2PDF", "status"]
      interval: 30s
      timeout: 10s
      retries: 3
    
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

```bash
# Deploy to production
docker-compose -f docker-compose.prod.yml up -d
```

---

## 🌍 Multi-Language Support

### Supported Languages

| Language | Code | Environment Variable |
|----------|------|---------------------|
| English | en-US | `ZPL2PDF_LANGUAGE=en-US` |
| Portuguese | pt-BR | `ZPL2PDF_LANGUAGE=pt-BR` |
| Spanish | es-ES | `ZPL2PDF_LANGUAGE=es-ES` |
| French | fr-FR | `ZPL2PDF_LANGUAGE=fr-FR` |
| German | de-DE | `ZPL2PDF_LANGUAGE=de-DE` |
| Italian | it-IT | `ZPL2PDF_LANGUAGE=it-IT` |
| Japanese | ja-JP | `ZPL2PDF_LANGUAGE=ja-JP` |
| Chinese | zh-CN | `ZPL2PDF_LANGUAGE=zh-CN` |

### Set Language

**Option 1: Environment Variable (Recommended)**

```bash
docker run -d \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  zpl2pdf:2.0.0
```

**Option 2: Docker Compose**

```yaml
environment:
  - ZPL2PDF_LANGUAGE=es-ES
```

**Option 3: At Build Time**

```dockerfile
ENV ZPL2PDF_LANGUAGE=fr-FR
```

### Test All Languages

```bash
# Test English
docker run --rm -e ZPL2PDF_LANGUAGE=en-US zpl2pdf:2.0.0 /app/ZPL2PDF -help

# Test Portuguese
docker run --rm -e ZPL2PDF_LANGUAGE=pt-BR zpl2pdf:2.0.0 /app/ZPL2PDF -help

# Test Spanish
docker run --rm -e ZPL2PDF_LANGUAGE=es-ES zpl2pdf:2.0.0 /app/ZPL2PDF -help
```

---

## 🔧 Advanced Usage

### Custom Configuration

```bash
# Create custom config
cat > zpl2pdf.json <<EOF
{
  "language": "pt-BR",
  "defaultListenFolder": "/app/watch",
  "labelWidth": 10,
  "labelHeight": 5,
  "unit": "cm",
  "dpi": 203
}
EOF

# Mount config
docker run -d \
  -v ./zpl2pdf.json:/app/zpl2pdf.json \
  -v ./watch:/app/watch \
  zpl2pdf:2.0.0
```

### Build Custom Image

```bash
# Build with custom base image
docker build \
  --build-arg BASE_IMAGE=mcr.microsoft.com/dotnet/runtime:9.0-alpine \
  -t zpl2pdf:custom \
  .
```

### Debug Mode

```bash
# Run with debug logging
 # Adjust log level using `logLevel` from the mounted `zpl2pdf.json`
docker run -it --rm \
  -v ./zpl2pdf.json:/app/zpl2pdf.json \
  -v ./watch:/app/watch \
  zpl2pdf:2.0.0 \
  /app/ZPL2PDF run -l /app/watch
```

---

## 🐛 Troubleshooting

### Issue: Container stops immediately

```bash
# Check logs
docker logs zpl2pdf-daemon

# Run interactively to see errors
docker run -it --rm zpl2pdf:2.0.0 /bin/bash
```

### Issue: Files not being converted

```bash
# Check if folder is mounted correctly
docker exec zpl2pdf-daemon ls -la /app/watch

# Check permissions
docker exec zpl2pdf-daemon ls -la /app/watch
```

### Issue: "libgdiplus not found"

```bash
# Rebuild with correct runtime
docker build --no-cache -t zpl2pdf:2.0.0 .
```

### Issue: Language not working

```bash
# Check environment variables
docker exec zpl2pdf-daemon env | grep ZPL2PDF

# Test language setting
docker exec zpl2pdf-daemon /app/ZPL2PDF --show-language
```

### Issue: Performance problems

```bash
# Check resource usage
docker stats zpl2pdf-daemon

# Increase limits
docker run -d \
  --cpus="2.0" \
  --memory="1g" \
  zpl2pdf:2.0.0
```

---

## 📊 Comparison: Native vs Docker

| Feature | Native Windows/Linux | Docker |
|---------|---------------------|--------|
| **Installation** | Manual .NET install | One command |
| **Dependencies** | Manual libgdiplus install | Auto-installed |
| **Portability** | OS-specific | Works anywhere |
| **Updates** | Manual download | `docker pull` |
| **Isolation** | Shared system | Isolated |
| **Testing** | Need separate PCs | Test all OS on one PC |
| **Deployment** | Complex scripts | `docker-compose up` |
| **Rollback** | Backup/restore | Switch image tag |

---

## 🎯 Best Practices

### 1. Use Multi-Stage Builds
✅ Smaller images (200MB vs 1GB)
✅ Faster deployments

### 2. Use Specific Tags
```bash
# ❌ BAD: Latest tag can change
docker pull zpl2pdf:latest

# ✅ GOOD: Specific version
docker pull zpl2pdf:2.0.0
```

### 3. Set Resource Limits
```yaml
deploy:
  resources:
    limits:
      cpus: '1.0'
      memory: 512M
```

### 4. Use Health Checks
```yaml
healthcheck:
  test: ["/app/ZPL2PDF", "status"]
  interval: 30s
```

### 5. Use Volumes for Data
```bash
# ❌ BAD: Data lost when container stops
docker run zpl2pdf:2.0.0

# ✅ GOOD: Data persists
docker run -v ./data:/app/watch zpl2pdf:2.0.0
```

---

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [ZPL2PDF GitHub](https://github.com/brunoleocam/ZPL2PDF)

---

## ❓ FAQ

**Q: Can I test Linux on Windows?**
A: ✅ Yes! Docker Desktop for Windows can run Linux containers.

**Q: Can I run multiple instances?**
A: ✅ Yes! Just use different container names and ports.

**Q: Does it work on Raspberry Pi?**
A: ✅ Yes! Use `linux-arm64` or `linux-arm` builds.

**Q: Can I use it without Docker?**
A: ✅ Yes! Use the native builds from `build/publish/`.

**Q: How do I update?**
A: Run `docker pull zpl2pdf:latest` and restart containers.

---

## 🚀 Next Steps

1. ✅ Build Docker image: `docker build -t zpl2pdf:2.0.0 .`
2. ✅ Test locally: `docker-compose up`
3. ✅ Test on Linux: See "Testing on All OS" section
4. ✅ Deploy to production: `docker-compose -f docker-compose.prod.yml up -d`
5. ✅ Monitor: `docker logs -f zpl2pdf-daemon`

**Happy Converting!** 🎉
