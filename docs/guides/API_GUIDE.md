# ZPL2PDF REST API Guide

Complete guide for using the ZPL2PDF REST API to convert ZPL content to PDF or PNG format.

## Overview

The ZPL2PDF REST API provides a simple HTTP interface to convert ZPL (Zebra Programming Language) content to PDF or PNG format. The API returns base64-encoded results in JSON format.

## Starting the API Server

### Basic Usage

```bash
# Start API server on default host and port (0.0.0.0:5000)
ZPL2PDF --api

# Start API server on custom port
ZPL2PDF --api --port 8080

# Start API server on custom host (localhost only)
ZPL2PDF --api --host localhost

# Start API server with custom host and port
ZPL2PDF --api --host 0.0.0.0 --port 8080

# Alternative: Use --web flag
ZPL2PDF --web --host 0.0.0.0 --port 5000
```

**Host Options:**
- `0.0.0.0` - Listen on all interfaces (default, Docker-friendly)
- `localhost` - Listen only on local machine
- `127.0.0.1` - Listen only on loopback interface
- Specific IP - Listen on specific network interface

The API server will start and display:
```
ZPL2PDF API is starting on 0.0.0.0:5000
API endpoint: http://0.0.0.0:5000/api/convert
Health check: http://0.0.0.0:5000/api/health
Press Ctrl+C to stop the API server
```

## API Endpoints

### POST /api/convert

Converts ZPL content to PDF or PNG format.

**Request**

```http
POST /api/convert
Content-Type: application/json
```

**Request Body**

```json
{
  "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
  "format": "pdf",
  "width": 10.0,
  "height": 5.0,
  "unit": "cm",
  "dpi": 203
}
```

**Request Parameters**

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `zpl` | string | Yes | - | ZPL content to convert |
| `format` | string | No | `"pdf"` | Output format: `"pdf"` or `"png"` |
| `width` | number | No | Auto | Label width (extracted from ZPL if not provided) |
| `height` | number | No | Auto | Label height (extracted from ZPL if not provided) |
| `unit` | string | No | `"mm"` | Unit of measurement: `"mm"`, `"cm"`, or `"in"` |
| `dpi` | number | No | `203` | Print density in DPI (72-600) |

**Response (PDF Format - Single Label)**

```json
{
  "success": true,
  "format": "pdf",
  "pdf": "base64_encoded_pdf_string",
  "pages": 1,
  "message": "Conversion successful"
}
```

**Response (PNG Format - Single Label)**

```json
{
  "success": true,
  "format": "png",
  "image": "base64_encoded_png_string",
  "pages": 1,
  "message": "Conversion successful"
}
```

**Response (PNG Format - Multiple Labels)**

```json
{
  "success": true,
  "format": "png",
  "images": [
    "base64_encoded_png_string_1",
    "base64_encoded_png_string_2"
  ],
  "pages": 2,
  "message": "Conversion successful"
}
```

**Error Response**

```json
{
  "success": false,
  "message": "Error description"
}
```

**HTTP Status Codes**

- `200 OK` - Conversion successful
- `400 Bad Request` - Invalid request (missing ZPL, invalid format, etc.)
- `500 Internal Server Error` - Server error during conversion

### GET /api/health

Health check endpoint to verify API server is running.

**Request**

```http
GET /api/health
```

**Response**

```json
{
  "status": "ok",
  "service": "ZPL2PDF API"
}
```

## Usage Examples

### cURL Examples

#### Convert to PDF

```bash
curl -X POST http://localhost:5000/api/convert \
  -H "Content-Type: application/json" \
  -d '{
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "pdf"
  }'
```

#### Convert to PNG

```bash
curl -X POST http://localhost:5000/api/convert \
  -H "Content-Type: application/json" \
  -d '{
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "png"
  }'
```

#### Convert with Custom Dimensions

```bash
curl -X POST http://localhost:5000/api/convert \
  -H "Content-Type: application/json" \
  -d '{
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "pdf",
    "width": 10.0,
    "height": 5.0,
    "unit": "cm",
    "dpi": 300
  }'
```

### PowerShell Examples

#### Convert to PDF

```powershell
$body = @{
    zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ"
    format = "pdf"
} | ConvertTo-Json

$response = Invoke-RestMethod -Method Post `
    -Uri "http://localhost:5000/api/convert" `
    -ContentType "application/json" `
    -Body $body

# Save PDF
$pdfBytes = [Convert]::FromBase64String($response.pdf)
[System.IO.File]::WriteAllBytes("output.pdf", $pdfBytes)
```

#### Convert to PNG

```powershell
$body = @{
    zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ"
    format = "png"
} | ConvertTo-Json

$response = Invoke-RestMethod -Method Post `
    -Uri "http://localhost:5000/api/convert" `
    -ContentType "application/json" `
    -Body $body

# Save PNG
$pngBytes = [Convert]::FromBase64String($response.image)
[System.IO.File]::WriteAllBytes("output.png", $pngBytes)
```

### C# Examples

#### Convert to PDF

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new HttpClient();
        var request = new
        {
            zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
            format = "pdf"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:5000/api/convert", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ConvertResponse>(responseJson);
        
        if (result.Success)
        {
            var pdfBytes = Convert.FromBase64String(result.Pdf);
            await System.IO.File.WriteAllBytesAsync("output.pdf", pdfBytes);
            Console.WriteLine("PDF saved successfully!");
        }
        else
        {
            Console.WriteLine($"Error: {result.Message}");
        }
    }
}

class ConvertResponse
{
    public bool Success { get; set; }
    public string Format { get; set; }
    public string Pdf { get; set; }
    public string Message { get; set; }
}
```

#### Convert to PNG

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new HttpClient();
        var request = new
        {
            zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
            format = "png"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("http://localhost:5000/api/convert", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ConvertResponse>(responseJson);
        
        if (result.Success)
        {
            if (!string.IsNullOrEmpty(result.Image))
            {
                // Single image
                var pngBytes = Convert.FromBase64String(result.Image);
                await System.IO.File.WriteAllBytesAsync("output.png", pngBytes);
            }
            else if (result.Images != null && result.Images.Count > 0)
            {
                // Multiple images
                for (int i = 0; i < result.Images.Count; i++)
                {
                    var pngBytes = Convert.FromBase64String(result.Images[i]);
                    await System.IO.File.WriteAllBytesAsync($"output_{i + 1}.png", pngBytes);
                }
            }
            Console.WriteLine("PNG saved successfully!");
        }
        else
        {
            Console.WriteLine($"Error: {result.Message}");
        }
    }
}

class ConvertResponse
{
    public bool Success { get; set; }
    public string Format { get; set; }
    public string Image { get; set; }
    public List<string> Images { get; set; }
    public string Message { get; set; }
}
```

### Python Examples

#### Convert to PDF

```python
import requests
import base64
import json

url = "http://localhost:5000/api/convert"
payload = {
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "pdf"
}

response = requests.post(url, json=payload)
result = response.json()

if result["success"]:
    pdf_bytes = base64.b64decode(result["pdf"])
    with open("output.pdf", "wb") as f:
        f.write(pdf_bytes)
    print("PDF saved successfully!")
else:
    print(f"Error: {result['message']}")
```

#### Convert to PNG

```python
import requests
import base64
import json

url = "http://localhost:5000/api/convert"
payload = {
    "zpl": "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ",
    "format": "png"
}

response = requests.post(url, json=payload)
result = response.json()

if result["success"]:
    if "image" in result:
        # Single image
        png_bytes = base64.b64decode(result["image"])
        with open("output.png", "wb") as f:
            f.write(png_bytes)
    elif "images" in result:
        # Multiple images
        for i, img_base64 in enumerate(result["images"]):
            png_bytes = base64.b64decode(img_base64)
            with open(f"output_{i + 1}.png", "wb") as f:
                f.write(png_bytes)
    print("PNG saved successfully!")
else:
    print(f"Error: {result['message']}")
```

### JavaScript/Node.js Examples

#### Convert to PDF

```javascript
const axios = require('axios');
const fs = require('fs');

async function convertToPdf() {
    try {
        const response = await axios.post('http://localhost:5000/api/convert', {
            zpl: '^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ',
            format: 'pdf'
        });

        if (response.data.success) {
            const pdfBuffer = Buffer.from(response.data.pdf, 'base64');
            fs.writeFileSync('output.pdf', pdfBuffer);
            console.log('PDF saved successfully!');
        } else {
            console.error('Error:', response.data.message);
        }
    } catch (error) {
        console.error('Request failed:', error.message);
    }
}

convertToPdf();
```

#### Convert to PNG

```javascript
const axios = require('axios');
const fs = require('fs');

async function convertToPng() {
    try {
        const response = await axios.post('http://localhost:5000/api/convert', {
            zpl: '^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ',
            format: 'png'
        });

        if (response.data.success) {
            if (response.data.image) {
                // Single image
                const pngBuffer = Buffer.from(response.data.image, 'base64');
                fs.writeFileSync('output.png', pngBuffer);
            } else if (response.data.images) {
                // Multiple images
                response.data.images.forEach((img, index) => {
                    const pngBuffer = Buffer.from(img, 'base64');
                    fs.writeFileSync(`output_${index + 1}.png`, pngBuffer);
                });
            }
            console.log('PNG saved successfully!');
        } else {
            console.error('Error:', response.data.message);
        }
    } catch (error) {
        console.error('Request failed:', error.message);
    }
}

convertToPng();
```

## Format Details

### PDF Format

- Returns a single PDF file containing all labels
- Each label is rendered on a separate page
- Base64-encoded in the `pdf` field of the response
- Suitable for printing or document storage

### PNG Format

- Returns PNG images (one per label)
- Single label: Returns `image` field with base64-encoded PNG
- Multiple labels: Returns `images` array with base64-encoded PNGs
- Suitable for web display or image processing

## Error Handling

The API returns appropriate HTTP status codes and error messages:

- **400 Bad Request**: Invalid request parameters
  - Missing ZPL content
  - Invalid format (must be "pdf" or "png")
  - Invalid dimension values

- **500 Internal Server Error**: Server-side errors
  - ZPL parsing errors
  - Conversion failures
  - Memory issues

Always check the `success` field in the response and handle errors appropriately.

## CORS Configuration

The API includes CORS support for development. For production deployments, configure CORS policies according to your security requirements.

## Performance Considerations

- **Single Label**: Typically processes in <100ms
- **Multiple Labels**: Processing time increases linearly with label count
- **Memory Usage**: Base64 encoding increases response size by ~33%
- **Large ZPL Files**: Consider chunking for very large ZPL content

## Best Practices

1. **Always validate ZPL content** before sending to the API
2. **Use appropriate format** - PDF for documents, PNG for web display
3. **Handle errors gracefully** - Check `success` field in responses
4. **Set explicit dimensions** when ZPL doesn't contain dimension commands
5. **Use appropriate DPI** - Higher DPI for better quality but larger file sizes
6. **Monitor API health** using the `/api/health` endpoint

## Troubleshooting

### API Server Won't Start

- Check if port is already in use
- Verify .NET 9.0 runtime is installed
- Check firewall settings

### Conversion Fails

- Verify ZPL content is valid
- Check ZPL contains `^XA` and `^XZ` commands
- Ensure dimensions are reasonable (not too large/small)

### Empty Response

- Check if ZPL content generates any labels
- Verify ZPL is not empty or whitespace only

## Related Documentation

- [Main README](../README.md) - General ZPL2PDF documentation
- [Docker Guide](DOCKER_GUIDE.md) - Running ZPL2PDF in Docker
- [Language Configuration](LANGUAGE_CONFIGURATION.md) - Multi-language support

## Support

For issues, questions, or feature requests:
- GitHub Issues: [https://github.com/brunoleocam/ZPL2PDF/issues](https://github.com/brunoleocam/ZPL2PDF/issues)
- GitHub Discussions: [https://github.com/brunoleocam/ZPL2PDF/discussions](https://github.com/brunoleocam/ZPL2PDF/discussions)
