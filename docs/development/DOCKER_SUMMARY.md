# 🐳 ZPL2PDF Docker Guide

## 🎯 **Overview**

ZPL2PDF provides Docker support for easy deployment and cross-platform testing. The Docker image is optimized using Alpine Linux for minimal size and maximum efficiency.

---

## 📦 **Docker Image Details**

### **Base Image**
- **Platform**: Alpine Linux 3.19
- **Size**: ~470MB (optimized)
- **Multi-architecture**: linux/amd64, linux/arm64
- **Security**: Non-root user execution

### **Features**
- ✅ Multi-language support (8 languages)
- ✅ Daemon mode for automatic file processing
- ✅ Health checks and auto-restart
- ✅ Volume mount points for input/output
- ✅ Environment variable configuration

---

## 🚀 **Quick Start**

### **Option 1: Docker Compose (Recommended)**

```bash
# Start daemon mode
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

### **Option 2: Docker Run**

```bash
# Create directories
mkdir watch output

# Run daemon
docker run -d \
  --name zpl2pdf \
  -v ./watch:/app/watch \
  -v ./output:/app/output \
  -e ZPL2PDF_LANGUAGE=pt-BR \
  brunoleocam/zpl2pdf:latest
```

### **Option 3: Single File Conversion**

```bash
docker run --rm \
  -v ./input:/app/input:ro \
  -v ./output:/app/output \
  brunoleocam/zpl2pdf:latest \
  /app/ZPL2PDF -i /app/input/label.txt -o /app/output -n result.pdf
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

# English (default)
docker run -e ZPL2PDF_LANGUAGE=en-US brunoleocam/zpl2pdf:latest
```

---

## 🧪 **Testing on Different Platforms**

### **Test on Linux (Without Having Linux)**

```bash
# Build image
docker build -t zpl2pdf:test .

# Interactive shell
docker run -it --rm zpl2pdf:test /bin/bash

# Inside container:
/app/ZPL2PDF -help
/app/ZPL2PDF status
```

### **Test Multiple Languages**

```bash
# Test help in different languages
docker run --rm -e ZPL2PDF_LANGUAGE=pt-BR zpl2pdf:test /app/ZPL2PDF -help
docker run --rm -e ZPL2PDF_LANGUAGE=es-ES zpl2pdf:test /app/ZPL2PDF -help
docker run --rm -e ZPL2PDF_LANGUAGE=fr-FR zpl2pdf:test /app/ZPL2PDF -help
```

---

## 🎯 **Use Cases**

### **Developer Testing**
```bash
# Quick build and test
docker build -t zpl2pdf:dev .
docker run -it --rm zpl2pdf:dev /app/ZPL2PDF -help
```

### **Production Deployment**
```bash
# Production with docker-compose
docker-compose -f docker-compose.prod.yml up -d

# Monitor resources
docker stats zpl2pdf-daemon
```

### **Multiple Instances**
```bash
# Portuguese instance
docker run -d --name zpl2pdf-pt -e ZPL2PDF_LANGUAGE=pt-BR -v ./watch-pt:/app/watch brunoleocam/zpl2pdf:latest

# Spanish instance
docker run -d --name zpl2pdf-es -e ZPL2PDF_LANGUAGE=es-ES -v ./watch-es:/app/watch brunoleocam/zpl2pdf:latest

# English instance
docker run -d --name zpl2pdf-en -e ZPL2PDF_LANGUAGE=en-US -v ./watch-en:/app/watch brunoleocam/zpl2pdf:latest
```

---

## 📊 **Image Comparison**

| Version | Size | Base | Use Case |
|---------|------|------|----------|
| **Alpine** | 470MB | Alpine Linux | ✅ **Recommended** - Production |
| **Ubuntu** | 579MB | Ubuntu | Alternative |
| **Original** | 674MB | Debian | Legacy |

---

## 🔧 **Advanced Configuration**

### **Persistent Configuration**

```bash
# Create configuration file
cat > zpl2pdf.json <<EOF
{
  "language": "pt-BR",
  "labelWidth": 10,
  "labelHeight": 5,
  "unit": "cm",
  "dpi": 203
}
EOF

# Mount configuration
docker run -d \
  -v ./zpl2pdf.json:/app/zpl2pdf.json \
  -v ./watch:/app/watch \
  brunoleocam/zpl2pdf:latest
```

### **Health Check**

```yaml
healthcheck:
  test: ["/app/ZPL2PDF", "status"]
  interval: 30s
  timeout: 10s
  retries: 3
```

### **Resource Limits**

```yaml
deploy:
  resources:
    limits:
      cpus: '1.0'
      memory: 512M
```

---

## 🐛 **Troubleshooting**

### **Container Stops Immediately**
```bash
# Check logs
docker logs zpl2pdf-daemon

# Interactive debug
docker run -it --rm brunoleocam/zpl2pdf:latest /bin/bash
```

### **Files Not Appearing**
```bash
# Check volumes
docker exec zpl2pdf-daemon ls -la /app/watch

# Check permissions
docker exec zpl2pdf-daemon ls -la /app
```

### **Wrong Language**
```bash
# Check environment variables
docker exec zpl2pdf-daemon env | grep ZPL2PDF

# Check language configuration
docker exec zpl2pdf-daemon /app/ZPL2PDF --show-language
```

---

## 📚 **Documentation Links**

- **Main Repository**: [github.com/brunoleocam/ZPL2PDF](https://github.com/brunoleocam/ZPL2PDF)
- **Docker Hub**: [hub.docker.com/r/brunoleocam/zpl2pdf](https://hub.docker.com/r/brunoleocam/zpl2pdf)
- **GitHub Container Registry**: [ghcr.io/brunoleocam/zpl2pdf](https://ghcr.io/brunoleocam/zpl2pdf)
- **Full Documentation**: [README.md](../../README.md)

---

## ✅ **Validation Checklist**

### **Build Validation**
- [ ] `docker build -t zpl2pdf:test .` succeeds
- [ ] Image size ~470MB (not 1GB+)
- [ ] Multi-stage build working

### **Execution Validation**
- [ ] `docker run --rm zpl2pdf:test /app/ZPL2PDF -help` shows help
- [ ] `docker run --rm zpl2pdf:test /app/ZPL2PDF status` works
- [ ] Health check passes

### **Volume Validation**
- [ ] `watch` folder is mounted correctly
- [ ] `output` folder receives PDFs
- [ ] Permissions are correct

### **Multi-language Validation**
- [ ] `ZPL2PDF_LANGUAGE=pt-BR` shows Portuguese messages
- [ ] `ZPL2PDF_LANGUAGE=es-ES` shows Spanish messages
- [ ] Fallback to English works

### **Daemon Validation**
- [ ] `docker-compose up -d` starts daemon
- [ ] Files in `watch` folder are converted
- [ ] PDFs appear in `output` folder
- [ ] Container restarts automatically

---

**ZPL2PDF Docker** - Deploy and test ZPL to PDF conversion anywhere! 🚀