# ❓ Troubleshooting

Comprehensive guide for diagnosing and resolving common ZPL2PDF issues.

## 🎯 Quick Diagnostics

### Check Installation
```bash
# Verify ZPL2PDF is installed
ZPL2PDF --help

# Check version
ZPL2PDF --version

# Test basic functionality
ZPL2PDF -z "^XA^FO50,50^A0N,50,50^FDTest^FS^XZ" -o ./test -n test.pdf
```

### Check System Resources
```bash
# Windows - Check available memory
wmic OS get TotalVisibleMemorySize,FreePhysicalMemory /value

# Linux - Check available memory
free -h

# Check disk space
df -h  # Linux/macOS
dir    # Windows
```

---

## 📁 File-Related Issues

### Issue: "File not found"
```bash
# Problem: Input file doesn't exist
ZPL2PDF.exe -i non_existent.txt -o output

# Solution: Check file path and existence
ls -la input.txt        # Linux/macOS
dir input.txt           # Windows

# Use absolute path if needed
ZPL2PDF.exe -i "C:\full\path\to\file.txt" -o output
```

### Issue: "Permission denied"
```bash
# Problem: Cannot read input file or write output
ZPL2PDF.exe -i protected_file.txt -o /root/output

# Solution: Check file permissions
ls -la input.txt        # Linux/macOS
icacls input.txt        # Windows

# Fix permissions
chmod 644 input.txt     # Linux/macOS
icacls input.txt /grant Everyone:F  # Windows
```

### Issue: "Invalid file format"
```bash
# Problem: File is not valid ZPL
ZPL2PDF.exe -i document.pdf -o output

# Solution: Verify file content
head -5 input.txt       # Linux/macOS
type input.txt          # Windows

# Check for ZPL commands
grep -E "\^[A-Z]" input.txt  # Linux/macOS
findstr "\^[A-Z]" input.txt  # Windows
```

---

## 🔄 Daemon Mode Issues

### Issue: "Daemon already running"
```bash
# Problem: Cannot start daemon, already running
ZPL2PDF.exe start

# Solution: Check and stop existing daemon
ZPL2PDF.exe status
ZPL2PDF.exe stop

# Force stop if unresponsive
ZPL2PDF.exe stop
# Or kill process manually
tasklist | findstr ZPL2PDF  # Windows
ps aux | grep ZPL2PDF       # Linux/macOS
```

### Issue: "Cannot create PID file"
```bash
# Problem: Permission denied creating PID file
ZPL2PDF.exe start

# Solution: Check temp directory permissions
# Windows: Check %TEMP% directory
# Linux: Check /var/run or /tmp directory

# Run with appropriate permissions
sudo ZPL2PDF start       # Linux
# Or run as administrator (Windows)
```

### Issue: "Folder monitoring not working"
```bash
# Problem: Daemon not detecting new files
ZPL2PDF.exe start -l "./watch_folder"

# Solution: Check folder permissions and existence
ls -la ./watch_folder    # Linux/macOS
dir .\watch_folder       # Windows

# Verify folder is writable
touch ./watch_folder/test.txt  # Linux/macOS
echo test > .\watch_folder\test.txt  # Windows

# Check daemon status
ZPL2PDF.exe status
```

---

## 📏 Dimension and Quality Issues

### Issue: "Invalid dimensions"
```bash
# Problem: Zero or negative dimensions
ZPL2PDF.exe -i label.txt -o output -w 0 -h 15

# Solution: Use positive values
ZPL2PDF.exe -i label.txt -o output -w 10 -h 15

# Check ZPL dimension commands
grep -E "\^(PW|LL)" input.txt  # Linux/macOS
findstr "\^(PW|LL)" input.txt  # Windows
```

### Issue: "Poor PDF quality"
```bash
# Problem: Blurry or low-quality PDF output
ZPL2PDF.exe -i label.txt -o output

# Solution: Increase DPI
ZPL2PDF.exe -i label.txt -o output -d 300

# Use appropriate dimensions
ZPL2PDF.exe -i label.txt -o output -w 10 -h 15 -u cm -d 300
```

### Issue: "PDF too large or small"
```bash
# Problem: PDF dimensions don't match expectations
ZPL2PDF.exe -i label.txt -o output

# Solution: Specify correct dimensions
ZPL2PDF.exe -i label.txt -o output -w 7.5 -h 15 -u in

# Check unit conversion
# mm = (points / 203) * 25.4
```

---

## 🌍 Multi-Language Issues

### Issue: "Language not changing"
```bash
# Problem: Interface still in English despite setting language
ZPL2PDF.exe --language pt-BR --help

# Solution: Check language code format
ZPL2PDF.exe --show-language

# Use correct language codes
ZPL2PDF.exe --language pt-BR  # Portuguese
ZPL2PDF.exe --language es-ES  # Spanish
ZPL2PDF.exe --language fr-FR  # French
```

### Issue: "Invalid language code"
```bash
# Problem: Language code not recognized
ZPL2PDF.exe --language portuguese

# Solution: Use standard language codes
ZPL2PDF.exe --language pt-BR

# Check supported languages
ZPL2PDF.exe --show-language
```

### Issue: "Language resets after restart"
```bash
# Problem: Language setting doesn't persist
ZPL2PDF.exe --language-definitive pt-BR
# Restart application
ZPL2PDF.exe --show-language  # Back to English

# Solution: Set environment variable
export ZPL2PDF_LANGUAGE=pt-BR  # Linux/macOS
set ZPL2PDF_LANGUAGE=pt-BR     # Windows
```

---

## 🐳 Docker Issues

### Issue: "Container exits immediately"
```bash
# Problem: Docker container starts and stops
docker run brunoleocam/zpl2pdf:latest

# Solution: Check container logs
docker logs <container_id>

# Run with proper command
docker run brunoleocam/zpl2pdf:latest --help
```

### Issue: "Volume mount not working"
```bash
# Problem: Files not accessible in container
docker run -v $(pwd):/app/watch brunoleocam/zpl2pdf:latest

# Solution: Check volume permissions
docker run -v $(pwd):/app/watch brunoleocam/zpl2pdf:latest ls -la /app/watch

# Fix host permissions
chmod -R 755 ./watch_folder
```

### Issue: "Permission denied in container"
```bash
# Problem: Cannot write to mounted volume
docker run -v $(pwd):/app/output brunoleocam/zpl2pdf:latest

# Solution: Run with proper user
docker run --user 1000:1000 -v $(pwd):/app/output brunoleocam/zpl2pdf:latest

# Or fix host permissions
chown -R 1000:1000 ./output_folder
```

---

## 🔧 Performance Issues

### Issue: "Slow processing"
```bash
# Problem: Files take too long to process
ZPL2PDF.exe -i large_file.txt -o output

# Solution: Check file size and complexity
wc -c input.txt         # Linux/macOS
dir input.txt           # Windows

# Use lower DPI for faster processing
ZPL2PDF.exe -i input.txt -o output -d 203

# Process smaller batches
split -l 100 large_file.txt  # Linux/macOS
```

### Issue: "High memory usage"
```bash
# Problem: ZPL2PDF uses too much memory
# Check memory usage
tasklist /FI "IMAGENAME eq ZPL2PDF.exe"  # Windows
ps aux | grep ZPL2PDF                    # Linux/macOS

# Solution: Process files individually
for file in *.txt; do
    ZPL2PDF.exe -i "$file" -o output
done
```

### Issue: "Disk space issues"
```bash
# Problem: Output folder full
ZPL2PDF.exe -i input.txt -o /full/folder

# Solution: Check available space
df -h /output/folder    # Linux/macOS
dir /output/folder      # Windows

# Clean up old files or use different output folder
ZPL2PDF.exe -i input.txt -o /new/output/folder
```

---

## 🔍 Debugging Techniques

### Enable Debug Logging
```bash
# Set debug log level
export ZPL2PDF_LOG_LEVEL=Debug  # Linux/macOS
set ZPL2PDF_LOG_LEVEL=Debug     # Windows

# Run with debug output
ZPL2PDF.exe -i input.txt -o output
```

### Check Log Files
```bash
# Windows log location
type %TEMP%\zpl2pdf.log

# Linux log location
cat /tmp/zpl2pdf.log

# Follow logs in real-time
tail -f /tmp/zpl2pdf.log  # Linux/macOS
```

### Test with Minimal ZPL
```bash
# Test with simple ZPL
ZPL2PDF.exe -z "^XA^FO50,50^A0N,50,50^FDTest^FS^XZ" -o test -n test.pdf

# Test with dimensions
ZPL2PDF.exe -z "^XA^PW400^LL600^FO50,50^A0N,50,50^FDTest^FS^XZ" -o test -n test.pdf
```

---

## 🚨 Error Messages Reference

### Common Error Messages

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "File not found" | Input file doesn't exist | Check file path and existence |
| "Permission denied" | Insufficient file permissions | Fix file/folder permissions |
| "Invalid dimensions" | Zero/negative dimensions | Use positive dimension values |
| "Daemon already running" | Another instance is active | Stop existing daemon first |
| "Cannot create PID file" | Temp directory not writable | Fix temp directory permissions |
| "Invalid language code" | Unsupported language code | Use supported language codes |
| "Folder not found" | Watch folder doesn't exist | Create folder or use existing path |

---

## 📞 Getting Help

### Self-Help Resources
1. **Check this troubleshooting guide**
2. **Review [[Basic Usage]] documentation**
3. **Check [[Configuration]] settings**
4. **Verify system requirements**

### Community Support
- **GitHub Issues**: https://github.com/brunoleocam/ZPL2PDF/issues
- **GitHub Discussions**: https://github.com/brunoleocam/ZPL2PDF/discussions

### Reporting Issues
When reporting issues, include:
1. **ZPL2PDF version**: `ZPL2PDF --version`
2. **Operating system**: Windows/Linux/macOS version
3. **Command used**: Exact command that failed
4. **Error message**: Complete error output
5. **Log files**: Relevant log entries
6. **Sample files**: Minimal ZPL file that reproduces issue

---

## 🔗 Related Topics

- [[Basic Usage]] - Basic troubleshooting steps
- [[Configuration]] - Configuration-related issues
- [[Performance Optimization]] - Performance troubleshooting
- [[Docker Deployment]] - Docker-specific issues
