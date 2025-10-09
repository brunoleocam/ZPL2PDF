# 🐳 Docker Installation Guide

Complete guide for installing and using ZPL2PDF with Docker.

---

## 🎯 **Why Use Docker?**

| Without Docker | With Docker |
|----------------|-------------|
| ❌ Manual .NET installation | ✅ One command install |
| ❌ Platform-specific setup | ✅ Works everywhere |
| ❌ Dependency conflicts | ✅ Isolated environment |
| ❌ Complex configuration | ✅ Simple configuration |

---

## 📋 **Prerequisites**

### **Install Docker**
- **Windows/Mac**: [Docker Desktop](https://www.docker.com/products/docker-desktop)
- **Linux**: 
  ```bash
  sudo apt-get update
  sudo apt-get install docker.io docker-compose
  sudo usermod -aG docker $USER
  ```

### **Verify Installation**
```bash
docker --version
# Output: Docker version 24.0.0 or higher

docker-compose --version
# Output: docker-compose version 2.20.0 or higher
```

---

## 🚀 **Quick Start**

### **1. Pull the Image**
```bash
docker pull brunoleocam/zpl2pdf:latest
```

### **2. Create Folders**
```bash
mkdir watch output
```

### **3. Run Daemon Mode**
```bash
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=en-US \
  brunoleocam/zpl2pdf:latest
```

### **4. Test**
```bash
# Check status
docker exec zpl2pdf /app/ZPL2PDF status

# View logs
docker logs zpl2pdf
```

---

## 🎯 **Installation Methods**

### **Method 1: Docker Compose (Recommended)**

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
    
    restart: unless-stopped
    
    healthcheck:
      test: ["/app/ZPL2PDF", "status"]
      interval: 30s
      timeout: 10s
      retries: 3
```

**Start:**
```bash
docker-compose up -d
```

**Stop:**
```bash
docker-compose down
```

### **Method 2: Docker Run**

**Daemon Mode:**
```bash
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  --restart unless-stopped \
  brunoleocam/zpl2pdf:latest
```

**Single Conversion:**
```bash
docker run --rm \
  -v ./input:/app/input:ro \
  -v ./output:/app/output \
  brunoleocam/zpl2pdf:latest \
  /app/ZPL2PDF -i /app/input/label.txt -o /app/output -n result.pdf
```

### **Method 3: Build from Source**

**Build:**
```bash
git clone https://github.com/brunoleocam/ZPL2PDF.git
cd ZPL2PDF
docker build -t zpl2pdf:local .
```

**Run:**
```bash
docker run -d \
  --name zpl2pdf-local \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  zpl2pdf:local
```

---

## 🌍 **Multi-Language Support**

Set language via environment variable:

```bash
# Portuguese
docker run -e ZPL2PDF_LANGUAGE=pt-BR brunoleocam/zpl2pdf:latest

# Spanish
docker run -e ZPL2PDF_LANGUAGE=es-ES brunoleocam/zpl2pdf:latest

# French
docker run -e ZPL2PDF_LANGUAGE=fr-FR brunoleocam/zpl2pdf:latest

# German
docker run -e ZPL2PDF_LANGUAGE=de-DE brunoleocam/zpl2pdf:latest

# Italian
docker run -e ZPL2PDF_LANGUAGE=it-IT brunoleocam/zpl2pdf:latest

# Japanese
docker run -e ZPL2PDF_LANGUAGE=ja-JP brunoleocam/zpl2pdf:latest

# Chinese
docker run -e ZPL2PDF_LANGUAGE=zh-CN brunoleocam/zpl2pdf:latest
```

**Test Languages:**
```bash
# Test help in different languages
docker run --rm -e ZPL2PDF_LANGUAGE=pt-BR brunoleocam/zpl2pdf:latest /app/ZPL2PDF -help
docker run --rm -e ZPL2PDF_LANGUAGE=es-ES brunoleocam/zpl2pdf:latest /app/ZPL2PDF -help
```

---

## 💡 **Usage Examples**

### **Example 1: Automatic File Processing**
```bash
# Start daemon
docker-compose up -d

# Copy ZPL files to watch folder
cp *.zpl watch/

# PDFs appear in output folder
ls output/
```

### **Example 2: Batch Conversion**
```bash
docker run --rm \
  -v ./zpl-files:/app/watch:ro \
  -v ./pdf-output:/app/output \
  brunoleocam/zpl2pdf:latest \
  /app/ZPL2PDF run -l /app/watch
```

### **Example 3: Multiple Instances**
```bash
# Portuguese instance
docker run -d --name zpl2pdf-pt \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  -v ./watch-pt:/app/watch \
  brunoleocam/zpl2pdf:latest

# Spanish instance
docker run -d --name zpl2pdf-es \
  -e ZPL2PDF_LANGUAGE=es-ES \
  -v ./watch-es:/app/watch \
  brunoleocam/zpl2pdf:latest
```

### **Example 4: Production Deployment**
```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    container_name: zpl2pdf-prod
    
    volumes:
      - /srv/zpl/watch:/app/watch
      - /srv/zpl/output:/app/output
    
    environment:
      - ZPL2PDF_LANGUAGE=en-US
    
    restart: always
    
    deploy:
      resources:
        limits:
          cpus: '2.0'
          memory: 1G
        reservations:
          cpus: '1.0'
          memory: 512M
```

---

## 🧪 **Testing on Different Platforms**

### **Test on Linux (From Windows/Mac)**
```bash
# Build and test
docker build -t zpl2pdf:test .
docker run -it --rm zpl2pdf:test /app/ZPL2PDF -help
```

### **Test Multiple Architectures**
```bash
# Enable multi-platform builds
docker buildx create --use --name multiplatform

# Build for ARM64
docker buildx build \
  --platform linux/arm64 \
  -t zpl2pdf:arm64-test \
  --load \
  .

# Test (uses QEMU if not on ARM)
docker run --rm zpl2pdf:arm64-test /app/ZPL2PDF -help
```

### **Test All Languages**
```bash
# Create test script
cat > test-languages.sh <<'EOF'
#!/bin/bash
languages=("en-US" "pt-BR" "es-ES" "fr-FR" "de-DE" "it-IT" "ja-JP" "zh-CN")

for lang in "${languages[@]}"; do
    echo -n "Testing $lang... "
    docker run --rm \
      -e ZPL2PDF_LANGUAGE=$lang \
      brunoleocam/zpl2pdf:latest \
      /app/ZPL2PDF -help | head -1
done
EOF

chmod +x test-languages.sh
./test-languages.sh
```

---

## 🔧 **Advanced Configuration**

### **Custom Configuration File**
```bash
# Create config
cat > zpl2pdf.json <<EOF
{
  "language": "pt-BR",
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
  brunoleocam/zpl2pdf:latest
```

### **Resource Limits**
```bash
docker run -d \
  --cpus="2.0" \
  --memory="1g" \
  --name zpl2pdf-limited \
  -v ./watch:/app/watch \
  brunoleocam/zpl2pdf:latest
```

### **Network Configuration**
```bash
docker run -d \
  --network host \
  --name zpl2pdf-network \
  -v ./watch:/app/watch \
  brunoleocam/zpl2pdf:latest
```

---

## 🐛 **Troubleshooting**

### **Container Stops Immediately**
```bash
# Check logs
docker logs zpl2pdf

# Run interactively
docker run -it --rm brunoleocam/zpl2pdf:latest /bin/bash
```

### **Files Not Being Converted**
```bash
# Check volume mounts
docker exec zpl2pdf ls -la /app/watch

# Check permissions
docker exec zpl2pdf ls -la /app
```

### **Language Not Working**
```bash
# Check environment variables
docker exec zpl2pdf env | grep ZPL2PDF

# Test language setting
docker exec zpl2pdf /app/ZPL2PDF --show-language
```

### **Performance Issues**
```bash
# Check resource usage
docker stats zpl2pdf

# Increase limits
docker run -d \
  --cpus="2.0" \
  --memory="1g" \
  brunoleocam/zpl2pdf:latest
```

---

## 📊 **Image Information**

| Version | Size | Base | Use Case |
|---------|------|------|----------|
| **latest** | 470MB | Alpine Linux | ✅ **Recommended** - Production |
| **alpine** | 470MB | Alpine Linux | Same as latest |
| **2.0.0** | 470MB | Alpine Linux | Specific version |

### **Available Tags**
- `latest` - Latest stable version
- `2.0.0` - Specific version
- `2.0` - Major.minor version
- `2` - Major version
- `alpine` - Alpine Linux base

---

## 🔗 **Registry Information**

### **Docker Hub**
```bash
docker pull brunoleocam/zpl2pdf:latest
```

### **GitHub Container Registry**
```bash
docker pull ghcr.io/brunoleocam/zpl2pdf:latest
```

### **Both Work Identically**
Both registries contain the same images and stay synchronized.

---

## ✅ **Validation Checklist**

### **Basic Functionality**
- [ ] `docker pull brunoleocam/zpl2pdf:latest` succeeds
- [ ] Container starts without errors
- [ ] `docker exec zpl2pdf /app/ZPL2PDF -help` shows help
- [ ] `docker exec zpl2pdf /app/ZPL2PDF status` works

### **File Processing**
- [ ] Volume mounts work correctly
- [ ] Files in watch folder are converted
- [ ] PDFs appear in output folder
- [ ] File permissions are correct

### **Multi-Language**
- [ ] `ZPL2PDF_LANGUAGE=pt-BR` shows Portuguese
- [ ] `ZPL2PDF_LANGUAGE=es-ES` shows Spanish
- [ ] Fallback to English works

### **Production Ready**
- [ ] Container restarts automatically
- [ ] Health checks pass
- [ ] Resource limits work
- [ ] Logs are accessible

---

## 📚 **Additional Resources**

- **[Main Documentation](../README.md)** - Complete documentation index
- **[Configuration Guide](configuration.md)** - Language and settings
- **[Troubleshooting](../troubleshooting/)** - Common issues and solutions
- **[GitHub Repository](https://github.com/brunoleocam/ZPL2PDF)** - Source code
- **[Docker Hub](https://hub.docker.com/r/brunoleocam/zpl2pdf)** - Docker images

---

## 🚀 **Next Steps**

1. ✅ **Install Docker** (if not already installed)
2. ✅ **Pull image**: `docker pull brunoleocam/zpl2pdf:latest`
3. ✅ **Create folders**: `mkdir watch output`
4. ✅ **Start daemon**: `docker-compose up -d`
5. ✅ **Test**: Copy ZPL files to `watch/` folder
6. ✅ **Monitor**: Check `output/` for generated PDFs

**Happy Converting!** 🎉
