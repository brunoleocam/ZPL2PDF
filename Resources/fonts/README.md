# ZPL2PDF Custom Fonts

This directory contains configuration for custom TrueType fonts used by ZPL2PDF.

## Font Mapping

The `font-mapping.json` file defines mappings between ZPL font identifiers and TrueType font files.

### ZPL Font Identifiers

| ID | Zebra Name | Type | Suggested Equivalent |
|----|------------|------|---------------------|
| 0 | Font 0 | Bitmap | DejaVu Sans Mono |
| A | CG Triumvirate Bold Condensed | Scalable | Liberation Sans Narrow Bold |
| B | CG Triumvirate | Scalable | Liberation Sans |
| C | OCR-B | Bitmap | OCR-B |
| D | CG Triumvirate Bold | Scalable | Liberation Sans Bold |
| E | CG Triumvirate Italic | Scalable | Liberation Sans Italic |
| F | CG Triumvirate Bold Italic | Scalable | Liberation Sans Bold Italic |
| P | OCR-A | Bitmap | OCR-A Extended |
| Q | MICR E-13B | Bitmap | MICR E-13B |
| R | CG Triumvirate | Scalable | Liberation Sans |
| T | CG Triumvirate Bold | Scalable | Liberation Sans Bold |
| U | CG Triumvirate Bold Condensed | Scalable | Liberation Sans Narrow Bold |

## Usage

### Command Line

```bash
# Load fonts from a directory
ZPL2PDF -i label.txt -o output.pdf --fonts-dir "C:\Fonts\Zebra"

# Map individual fonts
ZPL2PDF -i label.txt -o output.pdf --font "A=C:\Fonts\Arial.ttf"
ZPL2PDF -i label.txt -o output.pdf --font "P=C:\Fonts\OCRA.ttf" --font "C=C:\Fonts\OCRB.ttf"
```

### Configuration File

Edit `font-mapping.json` to define your font mappings:

```json
{
  "defaultFontsDirectory": "C:\\Fonts\\Zebra",
  "fontMappings": {
    "A": {
      "zplFontId": "A",
      "name": "My Custom Font A",
      "fontFile": "MyFont.ttf"
    }
  }
}
```

## Recommended Open-Source Fonts

1. **Liberation Sans** - Good replacement for CG Triumvirate
   - https://github.com/liberationfonts/liberation-fonts

2. **DejaVu Sans Mono** - Good for monospace/bitmap-style fonts
   - https://dejavu-fonts.github.io/

3. **OCR-A / OCR-B** - For OCR fonts
   - Various open-source versions available

4. **MICR E-13B** - For check printing fonts
   - Various open-source versions available

## Notes

- Font files should be TrueType (.ttf) or OpenType (.otf) format
- Place font files in this directory or specify full paths in the configuration
- The `widthFactor` and `heightFactor` can be used to adjust character spacing
- Set `isBitmapStyle` to true for fixed-width fonts like OCR-A, OCR-B, and MICR

