# Linux / Docker rendering notes (offline renderer)

The offline renderer (BinaryKits + SkiaSharp) draws text with fonts installed on the host. On minimal Linux
hosts and containers the result can differ from Labelary if no suitable fonts are installed.

## How fonts are chosen

| ZPL font | Preferred families (first installed wins) | Fallbacks added by ZPL2PDF |
|----------|-------------------------------------------|----------------------------|
| `0` (scalable, bold, semi-condensed) | Swis721 Cn BT, TeX Gyre Heros Cn, Nimbus Sans Narrow, Roboto Condensed, Helvetica, Helvetica Neue, Arial | Liberation Sans Narrow, Arial Narrow, Liberation Sans, Arimo, Noto Sans, Open Sans, DejaVu Sans Condensed, DejaVu Sans, FreeSans |
| `A`-`Z` (bitmap fonts, monospace) | DejaVu Sans Mono, Lucida Console, Andale Mono, Droid Sans Mono | Liberation Mono, Noto Sans Mono, Ubuntu Mono, FreeMono, Courier New |

If none of these is installed the system default font is used, which is often a wide **serif** face
(symptom: text noticeably wider than in Labelary and overflowing the label - see issue #81).

## Install fonts

```bash
# Alpine
apk add --no-cache fontconfig ttf-liberation font-noto

# Debian / Ubuntu
apt-get install -y fontconfig fonts-liberation fonts-dejavu-core

# RHEL / AlmaLinux / Fedora
dnf install -y fontconfig liberation-fonts dejavu-sans-fonts
```

The official Docker image already includes `fontconfig`, `ttf-liberation` and `font-noto`.

## Use your own fonts

```bash
ZPL2PDF -i label.zpl -o out --fonts-dir ./fonts --font "0=MyNarrowBold.ttf" --font "A=MyMono.ttf"
```

Relative paths in `--font` are resolved under `--fonts-dir`. IDs without a mapping keep using the default stacks above.

## Other differences from Labelary

- `^POI` (inverted orientation) is applied by rotating the whole label 180° (offline renderer).
- Label size comes from the arguments, then from `^PW`/`^LL`; if only one of them is declared, the other axis uses
  the explicit value or the default (100 x 150 mm).
- For pixel-exact parity with a Zebra printer or Labelary, use `--renderer labelary` or `--renderer auto`.
