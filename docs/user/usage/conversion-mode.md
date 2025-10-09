# 🔄 Conversion Mode Guide

Complete guide to using ZPL2PDF in conversion mode for individual file conversions.

---

## 🎯 **Overview**

**Conversion Mode** allows you to convert individual ZPL files or ZPL strings directly to PDF documents. This is ideal for:

- ✅ **One-time conversions**
- ✅ **Batch processing scripts**
- ✅ **Integration with other applications**
- ✅ **Testing and validation**

---

## 🚀 **Basic Usage**

### **Convert a File**

```bash
ZPL2PDF -i label.txt -o output_folder -n mylabel.pdf
```

### **Convert with Custom Dimensions**

```bash
ZPL2PDF -i label.txt -o output_folder -w 10 -h 5 -u cm
```

### **Convert ZPL String Directly**

```bash
ZPL2PDF -z "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ" -o output_folder
```

---

## 📋 **Command Syntax**

### **File Input**

```bash
ZPL2PDF -i <input_file> -o <output_folder> [options]
```

### **String Input**

```bash
ZPL2PDF -z <zpl_content> -o <output_folder> [options]
```

---

## 🔧 **Parameters**

### **Required Parameters**

| Parameter | Description | Example |
|-----------|-------------|---------|
| `-i <file>` | Input ZPL file (.txt or .prn) | `-i label.txt` |
| `-z <content>` | ZPL content as string | `-z "^XA...^XZ"` |
| `-o <folder>` | Output folder for PDF | `-o C:\Output` |

**Note:** Use **either** `-i` (file) **or** `-z` (string), not both.

### **Optional Parameters**

| Parameter | Description | Default | Example |
|-----------|-------------|---------|---------|
| `-n <name>` | Output PDF filename | Input filename | `-n result.pdf` |
| `-w <width>` | Label width | Extract from ZPL or 100mm | `-w 10` |
| `-h <height>` | Label height | Extract from ZPL or 150mm | `-h 5` |
| `-u <unit>` | Unit (mm, cm, in) | `mm` | `-u cm` |
| `-d <dpi>` | Print density | `203` | `-d 300` |

---

## 📐 **Dimension Handling**

### **Priority Order**

1. **⭐ Explicit Parameters** (`-w`, `-h`) - **Highest priority**
2. **⭐⭐ ZPL Commands** (`^PW`, `^LL`) - If no parameters provided
3. **⭐⭐⭐ Default Values** (100mm × 150mm) - Fallback

### **Example: Extract from ZPL**

If your ZPL file contains:

```zpl
^XA
^PW800        ← Width: 800 points
^LL1200       ← Height: 1200 points
^FO50,50^A0N,50,50^FDHello^FS
^XZ
```

ZPL2PDF automatically calculates:
- **Width:** 800 points = 99.8mm (at 203 DPI)
- **Height:** 1200 points = 149.8mm (at 203 DPI)

**Conversion Formula:** `mm = (points / 203) * 25.4`

### **Example: Override with Parameters**

Force specific dimensions (ignoring ZPL commands):

```bash
ZPL2PDF -i label.txt -o output/ -w 10 -h 15 -u cm
```

This creates a 10cm × 15cm PDF, regardless of `^PW` and `^LL` values.

---

## 🌍 **Unit Conversion**

### **Supported Units**

| Unit | Abbreviation | Example |
|------|--------------|---------|
| **Millimeters** | `mm` | `-w 100 -u mm` |
| **Centimeters** | `cm` | `-w 10 -u cm` |
| **Inches** | `in` | `-w 4 -u in` |

### **Conversion Table**

| mm | cm | inches |
|----|-----|--------|
| 100 | 10 | 3.94 |
| 150 | 15 | 5.91 |
| 200 | 20 | 7.87 |

---

## 🎯 **Usage Examples**

### **Example 1: Simple Conversion**

Convert a ZPL file to PDF with default settings:

```bash
ZPL2PDF -i shipping_label.txt -o C:\PDFs
```

**Output:** `C:\PDFs\shipping_label.pdf`

### **Example 2: Custom Dimensions in Inches**

Convert with specific dimensions:

```bash
ZPL2PDF -i label.txt -o output/ -w 4 -h 6 -u in -n product_label.pdf
```

**Output:** `output\product_label.pdf` (4" × 6")

### **Example 3: High-Resolution Conversion**

Convert with 300 DPI for better quality:

```bash
ZPL2PDF -i label.txt -o output/ -d 300
```

### **Example 4: Convert ZPL String**

Convert ZPL content directly:

```bash
ZPL2PDF -z "^XA^FO50,50^A0N,30,30^FDTest Label^FS^FO50,100^BY3,2,100^BCN,,N,N,N^FD1234567890^FS^XZ" -o output/ -n test.pdf
```

### **Example 5: Batch Processing**

Process all ZPL files in a folder:

**Windows (PowerShell):**
```powershell
Get-ChildItem C:\Labels\*.txt | ForEach-Object {
    ZPL2PDF -i $_.FullName -o C:\PDFs -w 10 -h 15 -u cm
}
```

**Linux/macOS (Bash):**
```bash
for file in /home/user/labels/*.txt; do
    ZPL2PDF -i "$file" -o /home/user/pdfs -w 10 -h 15 -u cm
done
```

### **Example 6: Integration with C# Application**

```csharp
using System.Diagnostics;

public void ConvertZplToPdf(string inputFile, string outputFolder)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ZPL2PDF.exe",
            Arguments = $"-i \"{inputFile}\" -o \"{outputFolder}\" -w 10 -h 15 -u cm",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        }
    };
    
    process.Start();
    process.WaitForExit();
    
    if (process.ExitCode == 0)
    {
        Console.WriteLine("Conversion successful!");
    }
    else
    {
        Console.WriteLine("Conversion failed!");
    }
}
```

### **Example 7: Integration with Python**

```python
import subprocess

def convert_zpl_to_pdf(input_file, output_folder, width=10, height=15):
    """Convert ZPL file to PDF using ZPL2PDF"""
    
    cmd = [
        "ZPL2PDF",
        "-i", input_file,
        "-o", output_folder,
        "-w", str(width),
        "-h", str(height),
        "-u", "cm"
    ]
    
    result = subprocess.run(cmd, capture_output=True, text=True)
    
    if result.returncode == 0:
        print(f"Conversion successful: {input_file}")
    else:
        print(f"Conversion failed: {result.stderr}")

# Usage
convert_zpl_to_pdf("label.txt", "/output", width=10, height=15)
```

---

## ✅ **Exit Codes**

ZPL2PDF returns standard exit codes:

| Code | Meaning | Description |
|------|---------|-------------|
| `0` | **Success** | Conversion completed successfully |
| `1` | **Error** | Conversion failed (see error message) |
| `2` | **Invalid Arguments** | Incorrect command-line parameters |

**Example: Check exit code (PowerShell)**
```powershell
ZPL2PDF -i label.txt -o output/
if ($LASTEXITCODE -eq 0) {
    Write-Host "Success!"
} else {
    Write-Host "Failed with code: $LASTEXITCODE"
}
```

---

## 🐛 **Troubleshooting**

### **Issue: File Not Found**

**Error:** `Input file not found: label.txt`

**Solutions:**
1. Check file path is correct
2. Use absolute path: `ZPL2PDF -i C:\Full\Path\label.txt -o output/`
3. Check file exists: `Test-Path label.txt` (PowerShell)

### **Issue: Invalid ZPL Content**

**Error:** `Invalid ZPL content: Missing ^XA or ^XZ`

**Solutions:**
1. Ensure ZPL starts with `^XA` and ends with `^XZ`
2. Validate ZPL syntax
3. Test with simple ZPL: `ZPL2PDF -z "^XA^FO50,50^A0N,30,30^FDTest^FS^XZ" -o output/`

### **Issue: Permission Denied**

**Error:** `Access denied to output folder: C:\Output`

**Solutions:**
1. Check folder permissions
2. Run as administrator (Windows)
3. Use different output folder: `ZPL2PDF -i label.txt -o C:\Users\YourName\Documents`

### **Issue: Invalid Dimensions**

**Error:** `Invalid width: -1`

**Solutions:**
1. Use positive numbers: `ZPL2PDF -i label.txt -o output/ -w 10 -h 15`
2. Check unit is valid (`mm`, `cm`, `in`)

---

## 📊 **Performance**

### **Typical Conversion Times**

| Label Complexity | Size | Time |
|-----------------|------|------|
| **Simple text** | 4" × 6" | ~50ms |
| **Barcode** | 4" × 6" | ~100ms |
| **Graphics** | 4" × 6" | ~200ms |
| **Complex** | 8" × 10" | ~500ms |

### **Optimization Tips**

1. ✅ **Use explicit dimensions** (`-w`, `-h`) to skip ZPL parsing
2. ✅ **Batch process** multiple files in parallel
3. ✅ **Use SSD** for faster file I/O
4. ✅ **Avoid network drives** for input/output

---

## 📚 **Related Documentation**

- 🚀 [Quick Start Guide](quick-start.md)
- 🔄 [Daemon Mode Guide](daemon-mode.md)
- ⚙️ [Configuration Guide](configuration.md)
- 🐛 [Troubleshooting Guide](../troubleshooting/common-issues.md)
- 🐳 [Docker Guide](../installation/docker.md)

---

**Conversion mode provides precise control for individual ZPL to PDF conversions!** 🚀
