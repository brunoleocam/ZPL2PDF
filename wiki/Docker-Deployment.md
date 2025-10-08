# 🐳 Docker Deployment

Complete guide for deploying ZPL2PDF using Docker containers.

## 🎯 Overview

Docker deployment offers:
- ✅ **Consistent environments** across platforms
- ✅ **Easy scaling** and orchestration
- ✅ **Isolated execution** with security
- ✅ **Simple deployment** with minimal dependencies
- ✅ **Production-ready** container images

---

## 🚀 Quick Start

### Pull and Run
```bash
# Pull latest image
docker pull brunoleocam/zpl2pdf:latest

# Run basic conversion
docker run --rm -v $(pwd):/app/watch -v $(pwd)/output:/app/output \
  brunoleocam/zpl2pdf:latest -i label.txt -o /app/output -n label.pdf

# Run daemon mode
docker run -d --name zpl2pdf-daemon \
  -v /path/to/watch:/app/watch \
  -v /path/to/output:/app/output \
  brunoleocam/zpl2pdf:latest start -l /app/watch
```

---

## 📦 Available Images

### Image Tags
```bash
# Latest stable release
docker pull brunoleocam/zpl2pdf:latest

# Specific version
docker pull brunoleocam/zpl2pdf:2.0.0

# Alpine variant (smaller)
docker pull brunoleocam/zpl2pdf:alpine

# Development builds
docker pull brunoleocam/zpl2pdf:dev
```

### Image Sizes
- **Standard**: ~650MB (full .NET runtime)
- **Alpine**: ~470MB (optimized Alpine Linux)
- **Multi-arch**: Supports x64, ARM64, ARM

---

## 🔧 Configuration

### Environment Variables
```bash
# Set language
docker run -e ZPL2PDF_LANGUAGE=pt-BR brunoleocam/zpl2pdf:latest

# Set log level
docker run -e ZPL2PDF_LOG_LEVEL=Debug brunoleocam/zpl2pdf:latest

# Set default folder
docker run -e ZPL2PDF_WATCH_FOLDER=/app/watch brunoleocam/zpl2pdf:latest
```

### Volume Mounts
```bash
# Watch folder for daemon mode
-v /host/watch:/app/watch

# Output folder for PDFs
-v /host/output:/app/output

# Configuration file
-v /host/config:/app/config

# Logs (optional)
-v /host/logs:/app/logs
```

---

## 🏗️ Docker Compose

### Basic Setup
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    environment:
      - ZPL2PDF_LANGUAGE=en-US
    command: start -l /app/watch -w 7.5 -h 15 -u in
    restart: unless-stopped
```

### Production Setup
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:2.0.0
    container_name: zpl2pdf-daemon
    volumes:
      - /var/zpl2pdf/watch:/app/watch:ro
      - /var/zpl2pdf/output:/app/output
      - /var/zpl2pdf/config:/app/config
      - /var/zpl2pdf/logs:/app/logs
    environment:
      - ZPL2PDF_LANGUAGE=pt-BR
      - ZPL2PDF_LOG_LEVEL=Info
    command: start -l /app/watch -w 100 -h 200 -u mm -d 300
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "ZPL2PDF", "status"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

### Multi-Instance Setup
```yaml
version: '3.8'
services:
  zpl2pdf-small:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch-small:/app/watch
      - ./output-small:/app/output
    command: start -l /app/watch -w 5 -h 10 -u cm -d 203
    restart: unless-stopped
    
  zpl2pdf-large:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch-large:/app/watch
      - ./output-large:/app/output
    command: start -l /app/watch -w 10 -h 15 -u cm -d 300
    restart: unless-stopped
```

---

## 🔄 Operation Modes

### Conversion Mode
```bash
# Single file conversion
docker run --rm \
  -v $(pwd)/input:/app/input \
  -v $(pwd)/output:/app/output \
  brunoleocam/zpl2pdf:latest \
  -i /app/input/label.txt -o /app/output -n label.pdf

# String conversion
docker run --rm \
  -v $(pwd)/output:/app/output \
  brunoleocam/zpl2pdf:latest \
  -z "^XA^FO50,50^A0N,50,50^FDHello^FS^XZ" -o /app/output -n hello.pdf
```

### Daemon Mode
```bash
# Start daemon
docker run -d --name zpl2pdf \
  -v /host/watch:/app/watch \
  -v /host/output:/app/output \
  brunoleocam/zpl2pdf:latest start -l /app/watch

# Check status
docker exec zpl2pdf ZPL2PDF status

# Stop daemon
docker stop zpl2pdf
```

---

## 🏢 Production Deployment

### Security Considerations
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    user: "1000:1000"  # Non-root user
    volumes:
      - ./watch:/app/watch:ro  # Read-only watch folder
      - ./output:/app/output
    environment:
      - ZPL2PDF_LANGUAGE=en-US
    command: start -l /app/watch
    restart: unless-stopped
    security_opt:
      - no-new-privileges:true
    cap_drop:
      - ALL
    cap_add:
      - CHOWN
      - SETUID
      - SETGID
```

### Resource Limits
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    command: start -l /app/watch
    restart: unless-stopped
    deploy:
      resources:
        limits:
          memory: 512M
          cpus: '0.5'
        reservations:
          memory: 256M
          cpus: '0.25'
```

### Logging Configuration
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    command: start -l /app/watch
    restart: unless-stopped
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"
```

---

## 🔍 Monitoring and Debugging

### Container Status
```bash
# Check running containers
docker ps

# Check container logs
docker logs zpl2pdf-daemon

# Follow logs in real-time
docker logs -f zpl2pdf-daemon

# Check resource usage
docker stats zpl2pdf-daemon
```

### Health Checks
```bash
# Built-in health check
docker exec zpl2pdf-daemon ZPL2PDF status

# Custom health check script
docker exec zpl2pdf-daemon sh -c "ZPL2PDF status | grep -q 'running'"
```

### Debugging
```bash
# Interactive shell
docker exec -it zpl2pdf-daemon sh

# Check mounted volumes
docker exec zpl2pdf-daemon ls -la /app/

# Check environment variables
docker exec zpl2pdf-daemon env | grep ZPL2PDF
```

---

## 🚀 Scaling and Orchestration

### Docker Swarm
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:latest
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    command: start -l /app/watch
    restart: unless-stopped
    deploy:
      replicas: 2
      placement:
        constraints:
          - node.role == worker
      resources:
        limits:
          memory: 512M
          cpus: '0.5'
```

### Kubernetes
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: zpl2pdf
spec:
  replicas: 2
  selector:
    matchLabels:
      app: zpl2pdf
  template:
    metadata:
      labels:
        app: zpl2pdf
    spec:
      containers:
      - name: zpl2pdf
        image: brunoleocam/zpl2pdf:latest
        command: ["ZPL2PDF", "start", "-l", "/app/watch"]
        volumeMounts:
        - name: watch-volume
          mountPath: /app/watch
        - name: output-volume
          mountPath: /app/output
        env:
        - name: ZPL2PDF_LANGUAGE
          value: "en-US"
        resources:
          limits:
            memory: "512Mi"
            cpu: "500m"
          requests:
            memory: "256Mi"
            cpu: "250m"
      volumes:
      - name: watch-volume
        persistentVolumeClaim:
          claimName: zpl2pdf-watch-pvc
      - name: output-volume
        persistentVolumeClaim:
          claimName: zpl2pdf-output-pvc
```

---

## 🔧 Custom Images

### Build Custom Image
```dockerfile
FROM brunoleocam/zpl2pdf:latest

# Add custom configuration
COPY custom-config.json /app/config/zpl2pdf.json

# Add custom scripts
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

# Set custom entrypoint
ENTRYPOINT ["/app/entrypoint.sh"]
```

### Custom Entrypoint Script
```bash
#!/bin/sh
# entrypoint.sh

# Load custom configuration
if [ -f "/app/config/zpl2pdf.json" ]; then
    echo "Loading custom configuration..."
fi

# Set default language if not specified
if [ -z "$ZPL2PDF_LANGUAGE" ]; then
    export ZPL2PDF_LANGUAGE=en-US
fi

# Start ZPL2PDF with custom parameters
exec ZPL2PDF "$@"
```

---

## 🐛 Troubleshooting

### Common Issues

#### Issue: "Volume mount not working"
```bash
# Check volume permissions
docker exec zpl2pdf ls -la /app/

# Fix permissions
docker run --rm -v $(pwd):/app/workdir alpine chown -R 1000:1000 /app/workdir
```

#### Issue: "Container exits immediately"
```bash
# Check container logs
docker logs zpl2pdf

# Run with interactive mode for debugging
docker run -it brunoleocam/zpl2pdf:latest sh
```

#### Issue: "Permission denied"
```bash
# Run with proper user
docker run --user 1000:1000 brunoleocam/zpl2pdf:latest

# Or fix host permissions
chmod -R 755 ./watch ./output
```

### Performance Issues
```bash
# Monitor resource usage
docker stats

# Check disk I/O
docker exec zpl2pdf iostat -x 1

# Check memory usage
docker exec zpl2pdf free -h
```

---

## 🔗 Related Topics

- [[Installation Guide]] - Docker installation options
- [[Daemon Mode]] - Daemon mode configuration
- [[Configuration]] - Environment variables and settings
- [[Troubleshooting]] - Docker-specific issues
