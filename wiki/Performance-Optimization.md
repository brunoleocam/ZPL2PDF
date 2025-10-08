# ⚡ Performance Optimization

Complete guide for optimizing ZPL2PDF performance and resource usage.

## 🎯 Overview

Optimize ZPL2PDF for:
- ✅ **Processing speed** - Faster conversions
- ✅ **Memory usage** - Reduced RAM consumption
- ✅ **Disk I/O** - Efficient file operations
- ✅ **CPU utilization** - Better resource management
- ✅ **Scalability** - Handle large volumes

---

## 🚀 Quick Wins

### 1. Reduce DPI for Faster Processing
```bash
# Standard quality (fastest)
ZPL2PDF -i label.txt -o output -d 203

# Low quality (even faster, for testing)
ZPL2PDF -i label.txt -o output -d 150

# Only use high DPI when necessary
ZPL2PDF -i label.txt -o output -d 300  # Slower
```

### 2. Use Appropriate Dimensions
```bash
# Let ZPL2PDF extract dimensions (faster)
ZPL2PDF -i label.txt -o output

# Only specify when necessary
ZPL2PDF -i label.txt -o output -w 7.5 -h 15 -u in
```

### 3. Process Files Sequentially
```bash
# Avoid parallel processing (prevents memory spikes)
for file in *.txt; do
    ZPL2PDF -i "$file" -o output
done
```

---

## 💾 Memory Optimization

### Configuration Settings
```json
{
  "processing": {
    "batchSize": 10,
    "parallelProcessing": false,
    "memoryLimit": "512MB",
    "timeoutSeconds": 300
  }
}
```

### Best Practices
```bash
# Process large files individually
ZPL2PDF -i large_file.txt -o output

# Split large files before processing
split -l 100 large_file.txt chunk_

# Clear processed files
rm processed/*.txt  # Manual cleanup
```

### Monitor Memory Usage
```bash
# Linux - Monitor memory
top -p $(pgrep ZPL2PDF)
htop -p $(pgrep ZPL2PDF)

# macOS - Monitor memory
top -pid $(pgrep ZPL2PDF)

# Windows - Monitor memory
tasklist /FI "IMAGENAME eq ZPL2PDF.exe"
```

---

## 🔧 CPU Optimization

### Single vs. Parallel Processing
```bash
# Single-threaded (more stable)
ZPL2PDF start -l ./watch

# Process files one at a time
for file in *.txt; do
    ZPL2PDF -i "$file" -o output &
    wait  # Wait for completion before next
done
```

### CPU Affinity (Linux)
```bash
# Run on specific CPU cores
taskset -c 0-3 ZPL2PDF start -l ./watch

# Or use systemd service
[Service]
CPUAffinity=0-3
```

### Resource Limits
```bash
# Limit CPU usage (Linux)
cpulimit -e ZPL2PDF -l 50  # 50% CPU

# Or use nice
nice -n 10 ZPL2PDF start -l ./watch

# Docker resource limits
docker run --cpus="0.5" --memory="512m" brunoleocam/zpl2pdf:latest
```

---

## 💽 Disk I/O Optimization

### Fast Storage
```bash
# Use SSD for watch and output folders
ZPL2PDF start -l /mnt/ssd/watch -w 7.5 -h 15 -u in

# Or use tmpfs (RAM disk) for temporary files
sudo mount -t tmpfs -o size=512M tmpfs /tmp/zpl2pdf
ZPL2PDF start -l /tmp/zpl2pdf/watch
```

### Reduce File Operations
```json
{
  "autoDeleteProcessed": true,   // Delete after processing
  "outputSubfolders": false,     // Disable subfolders
  "retryDelay": 5000,            // Longer retry delay
  "maxRetries": 2                // Fewer retries
}
```

### Batch Processing
```bash
# Process files in batches
find ./watch -name "*.txt" | head -n 10 | while read file; do
    ZPL2PDF -i "$file" -o output
done

# Wait between batches
sleep 5
```

---

## 🐳 Docker Optimization

### Optimized Image
```bash
# Use Alpine variant (smaller, faster)
docker pull brunoleocam/zpl2pdf:alpine

# Multi-stage build (custom)
# See Dockerfile in repository
```

### Resource Limits
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:alpine
    volumes:
      - ./watch:/app/watch
      - ./output:/app/output
    command: start -l /app/watch -w 7.5 -h 15 -u in
    deploy:
      resources:
        limits:
          memory: 512M
          cpus: '0.5'
        reservations:
          memory: 256M
          cpus: '0.25'
```

### Volume Performance
```yaml
version: '3.8'
services:
  zpl2pdf:
    image: brunoleocam/zpl2pdf:alpine
    volumes:
      # Use cached/delegated for better performance
      - ./watch:/app/watch:cached
      - ./output:/app/output:delegated
    command: start -l /app/watch
```

---

## 📊 Monitoring and Profiling

### Performance Metrics
```bash
# Measure conversion time
time ZPL2PDF -i label.txt -o output -n label.pdf

# Monitor processing rate
watch -n 1 'ls -l output/ | wc -l'

# Track resource usage
htop -p $(pgrep ZPL2PDF)
```

### Logging for Performance
```json
{
  "logLevel": "Info",  // Use Info (not Debug) in production
  "logToFile": false,  // Disable file logging for speed
  "logToConsole": true
}
```

### Profiling
```bash
# Linux - Profile with perf
perf record -g ZPL2PDF -i label.txt -o output
perf report

# macOS - Profile with Instruments
instruments -t "Time Profiler" ZPL2PDF -i label.txt -o output

# Windows - Profile with PerfView
PerfView.exe collect ZPL2PDF.exe -i label.txt -o output
```

---

## 🔄 Daemon Mode Optimization

### Optimized Configuration
```json
{
  "retryDelay": 5000,        // Longer delay = fewer checks
  "maxRetries": 2,           // Fewer retries = faster failure
  "autoDeleteProcessed": true, // Clean up automatically
  "batchSize": 10,           // Process in small batches
  "watchInterval": 1000      // Check folder every 1 second
}
```

### Watch Folder Best Practices
```bash
# Use dedicated SSD partition
ZPL2PDF start -l /mnt/ssd/watch

# Limit folder size
# Use cron to clean old files
0 2 * * * find /var/zpl2pdf/watch -mtime +7 -delete

# Separate input/output folders
ZPL2PDF start -l /var/watch -o /var/output
```

### Periodic Restart
```bash
# Restart daemon daily to free memory
# Add to crontab
0 2 * * * ZPL2PDF stop && sleep 5 && ZPL2PDF start -l /var/watch

# Or use systemd timer (Linux)
[Service]
Restart=always
RestartSec=86400  # 24 hours
```

---

## 📈 Scaling Strategies

### Horizontal Scaling
```yaml
# Multiple instances for different label sizes
version: '3.8'
services:
  zpl2pdf-small:
    image: brunoleocam/zpl2pdf:alpine
    volumes:
      - ./watch-small:/app/watch
      - ./output:/app/output
    command: start -l /app/watch -w 5 -h 10 -u cm
    
  zpl2pdf-medium:
    image: brunoleocam/zpl2pdf:alpine
    volumes:
      - ./watch-medium:/app/watch
      - ./output:/app/output
    command: start -l /app/watch -w 10 -h 15 -u cm
    
  zpl2pdf-large:
    image: brunoleocam/zpl2pdf:alpine
    volumes:
      - ./watch-large:/app/watch
      - ./output:/app/output
    command: start -l /app/watch -w 15 -h 20 -u cm
```

### Load Balancing
```bash
# Use multiple watch folders
mkdir -p watch/{1,2,3,4}

# Start multiple daemons
ZPL2PDF start -l watch/1 -w 7.5 -h 15 -u in &
ZPL2PDF start -l watch/2 -w 7.5 -h 15 -u in &
ZPL2PDF start -l watch/3 -w 7.5 -h 15 -u in &
ZPL2PDF start -l watch/4 -w 7.5 -h 15 -u in &

# Distribute files across folders
# Use custom script or load balancer
```

---

## 🧪 Benchmarking

### Test Scenarios
```bash
# Scenario 1: Single file conversion
time ZPL2PDF -i label.txt -o output -d 203

# Scenario 2: Batch conversion (100 files)
time for i in {1..100}; do
    ZPL2PDF -i "label_${i}.txt" -o output
done

# Scenario 3: Different DPI settings
time ZPL2PDF -i label.txt -o output -d 150  # Low
time ZPL2PDF -i label.txt -o output -d 203  # Standard
time ZPL2PDF -i label.txt -o output -d 300  # High
```

### Performance Comparison
| DPI | Processing Time | File Size | Quality |
|-----|----------------|-----------|---------|
| **150** | 0.5s | 50KB | Low |
| **203** | 1.0s | 80KB | Standard |
| **300** | 2.5s | 150KB | High |
| **600** | 6.0s | 400KB | Premium |

---

## 💡 Best Practices Summary

### Do ✅
- Use standard DPI (203) for most cases
- Process files sequentially
- Monitor resource usage
- Clean up processed files
- Use SSD for watch folders
- Restart daemon periodically
- Set appropriate resource limits

### Don't ❌
- Don't use high DPI unnecessarily
- Don't process large batches in parallel
- Don't keep unlimited log files
- Don't use network drives for watch folders
- Don't run without resource limits
- Don't ignore memory warnings

---

## 🔗 Related Topics

- [[Configuration]] - Performance-related settings
- [[Daemon Mode]] - Daemon optimization
- [[Docker Deployment]] - Container optimization
- [[Troubleshooting]] - Performance issues
