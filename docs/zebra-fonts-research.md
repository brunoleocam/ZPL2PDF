# Zebra Internal Fonts Research

This document contains research on Zebra printer internal fonts and their open-source equivalents.

## Zebra Internal Fonts

### Bitmap Fonts (Fixed Size)

| Font ID | Name | Character Set | Notes |
|---------|------|---------------|-------|
| 0 | Font 0 | ASCII | Default bitmap font, monospace |
| C | OCR-B | ASCII | Optical Character Recognition font |
| P | OCR-A | ASCII | Optical Character Recognition font |
| Q | MICR E-13B | Numeric + symbols | Magnetic Ink Character Recognition |

### Scalable Fonts (CG Triumvirate Family)

| Font ID | Name | Style | Notes |
|---------|------|-------|-------|
| A | CG Triumvirate Bold Condensed | Bold, Condensed | Narrow width |
| B | CG Triumvirate | Regular | Standard width |
| D | CG Triumvirate Bold | Bold | Standard width |
| E | CG Triumvirate Italic | Italic | Standard width |
| F | CG Triumvirate Bold Italic | Bold Italic | Standard width |
| R | CG Triumvirate | Regular | Same as B |
| T | CG Triumvirate Bold | Bold | Same as D |
| U | CG Triumvirate Bold Condensed | Bold, Condensed | Same as A |

## Open-Source Equivalents

### Liberation Fonts Family

The Liberation Fonts are metric-compatible with common commercial fonts and provide good alternatives:

- **Liberation Sans** → CG Triumvirate
- **Liberation Sans Bold** → CG Triumvirate Bold
- **Liberation Sans Italic** → CG Triumvirate Italic
- **Liberation Sans Bold Italic** → CG Triumvirate Bold Italic
- **Liberation Sans Narrow Bold** → CG Triumvirate Bold Condensed

Download: https://github.com/liberationfonts/liberation-fonts

### DejaVu Fonts

Good for monospace/bitmap-style fonts:

- **DejaVu Sans Mono** → Font 0 equivalent

Download: https://dejavu-fonts.github.io/

### OCR Fonts

Several open-source OCR fonts are available:

- **OCR-A Extended** (various sources)
- **OCR-B** (various sources)

### MICR Fonts

For check printing:

- **MICR E-13B** (various open-source versions)
- **GnuMICR** (GPL licensed)

## Font Metrics Comparison

### CG Triumvirate vs Liberation Sans

| Metric | CG Triumvirate | Liberation Sans |
|--------|---------------|-----------------|
| x-height | ~0.52 em | ~0.52 em |
| Cap height | ~0.73 em | ~0.73 em |
| Ascender | ~0.93 em | ~0.93 em |
| Descender | ~-0.25 em | ~-0.25 em |

The metrics are very similar, making Liberation Sans a good substitute.

### Width Adjustments

For condensed fonts (A, U), a width factor of approximately 0.85 may be needed.

## Implementation Notes

### BinaryKits.Zpl.Viewer Current Behavior

The BinaryKits.Zpl.Viewer library currently:

1. Uses system fonts for rendering
2. Does not have built-in Zebra font equivalents
3. May produce different visual output compared to actual Zebra printers

### Proposed Improvements

1. **Font Registration API**: Allow users to register custom fonts for specific ZPL font IDs
2. **Default Font Mapping**: Include Liberation fonts as default fallbacks
3. **Metrics Adjustment**: Apply width/height factors to match Zebra font metrics

### Labelary Comparison

Labelary (http://labelary.com) provides pixel-perfect rendering by:

1. Using actual Zebra printer firmware for rendering
2. Maintaining exact font metrics and spacing
3. Supporting all Zebra-specific features

For applications requiring exact fidelity, the Labelary API integration (implemented in ZPL2PDF v3.0) provides the best results.

## References

1. Zebra ZPL II Programming Guide
2. Liberation Fonts Project: https://github.com/liberationfonts/liberation-fonts
3. DejaVu Fonts: https://dejavu-fonts.github.io/
4. Labelary API: http://labelary.com/service.html
5. BinaryKits.Zpl: https://github.com/BinaryKits/BinaryKits.Zpl

## Future Work

1. Create PR for BinaryKits.Zpl with font mapping support
2. Package Liberation fonts with ZPL2PDF
3. Implement automatic font metric adjustment
4. Add font preview/comparison tool

