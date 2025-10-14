#!/bin/bash
# Build .deb package for Debian/Ubuntu

set -e

VERSION="2.0.0"
ARCH="amd64"  # or arm64, armhf
PROJECT_NAME="zpl2pdf"
BUILD_DIR="build/deb/${PROJECT_NAME}_${VERSION}_${ARCH}"

echo "============================================"
echo "   Building .deb Package for ZPL2PDF"
echo "============================================"
echo "Version: ${VERSION}"
echo "Architecture: ${ARCH}"
echo ""

# Clean previous build
rm -rf build/deb
mkdir -p "${BUILD_DIR}"

# Create directory structure
echo "[*] Creating directory structure..."
mkdir -p "${BUILD_DIR}/DEBIAN"
mkdir -p "${BUILD_DIR}/usr/bin"
mkdir -p "${BUILD_DIR}/usr/share/doc/${PROJECT_NAME}"
mkdir -p "${BUILD_DIR}/usr/share/man/man1"
mkdir -p "${BUILD_DIR}/usr/share/applications"
mkdir -p "${BUILD_DIR}/usr/share/pixmaps"

# Copy control file
echo "[*] Creating DEBIAN/control..."
cat > "${BUILD_DIR}/DEBIAN/control" << EOF
Package: ${PROJECT_NAME}
Version: ${VERSION}
Section: utils
Priority: optional
Architecture: ${ARCH}
Depends: libgdiplus, libc6-dev
Maintainer: Bruno Leonardo Campos <brunoleocam@gmail.com>
Description: ZPL to PDF Converter
 A powerful, cross-platform command-line tool that converts ZPL
 (Zebra Programming Language) files to high-quality PDF documents.
 .
 Features:
  - Dual operation modes: Conversion and Daemon
  - Intelligent dimension handling with automatic ZPL extraction
  - Cross-platform support (Windows, Linux, macOS)
  - Clean Architecture with SOLID principles
  - High performance with async processing
  - Enterprise-ready with PID management and logging
Homepage: https://github.com/brunoleocam/ZPL2PDF
EOF

# Copy binary
echo "[*] Copying binary..."
if [ "${ARCH}" = "amd64" ]; then
    RUNTIME="linux-x64"
elif [ "${ARCH}" = "arm64" ]; then
    RUNTIME="linux-arm64"
elif [ "${ARCH}" = "armhf" ]; then
    RUNTIME="linux-arm"
fi

# Build if not exists
if [ ! -f "bin/Release/net9.0/${RUNTIME}/publish/ZPL2PDF" ]; then
    echo "[*] Building for ${RUNTIME}..."
    dotnet publish -c Release -r ${RUNTIME} --self-contained true -o "bin/Release/net9.0/${RUNTIME}/publish"
fi

cp "bin/Release/net9.0/${RUNTIME}/publish/ZPL2PDF" "${BUILD_DIR}/usr/bin/"
chmod +x "${BUILD_DIR}/usr/bin/ZPL2PDF"

# Copy documentation
echo "[*] Copying documentation..."
cp README.md "${BUILD_DIR}/usr/share/doc/${PROJECT_NAME}/"
cp LICENSE "${BUILD_DIR}/usr/share/doc/${PROJECT_NAME}/"
cp CHANGELOG.md "${BUILD_DIR}/usr/share/doc/${PROJECT_NAME}/"

# Create man page
echo "[*] Creating man page..."
cat > "${BUILD_DIR}/usr/share/man/man1/zpl2pdf.1" << 'EOF'
.TH ZPL2PDF 1 "2024" "ZPL2PDF 2.0.0" "User Commands"
.SH NAME
zpl2pdf \- ZPL to PDF Converter
.SH SYNOPSIS
.B zpl2pdf
[\fIOPTIONS\fR]
.SH DESCRIPTION
ZPL2PDF is a powerful, cross-platform command-line tool that converts ZPL (Zebra Programming Language) files to high-quality PDF documents.
.SH OPTIONS
.TP
\fB\-i\fR, \fB\-\-input\fR
Input ZPL file (.txt or .prn)
.TP
\fB\-z\fR, \fB\-\-zpl\fR
ZPL content as string
.TP
\fB\-o\fR, \fB\-\-output\fR
Output folder for PDF
.TP
\fB\-n\fR, \fB\-\-name\fR
Output PDF filename (optional)
.TP
\fB\-w\fR, \fB\-\-width\fR
Label width
.TP
\fB\-h\fR, \fB\-\-height\fR
Label height
.TP
\fB\-u\fR, \fB\-\-unit\fR
Unit (mm, cm, in)
.TP
\fB\-d\fR, \fB\-\-dpi\fR
Print density (203, 300, etc.)
.TP
\fBstart\fR
Start daemon mode
.TP
\fBstop\fR
Stop daemon mode
.TP
\fBstatus\fR
Check daemon status
.SH EXAMPLES
.TP
Convert a single file:
.B zpl2pdf -i label.txt -o output_folder -n my_label.pdf
.TP
Start daemon mode:
.B zpl2pdf start -l /path/to/watch -w 7.5 -h 15 -u in
.SH AUTHOR
Bruno Leonardo Campos
.SH SEE ALSO
https://github.com/brunoleocam/ZPL2PDF
EOF

gzip -9 "${BUILD_DIR}/usr/share/man/man1/zpl2pdf.1"

# Create postinst script
echo "[*] Creating postinst script..."
cat > "${BUILD_DIR}/DEBIAN/postinst" << 'EOF'
#!/bin/bash
set -e

echo ""
echo "ZPL2PDF installed successfully!"
echo ""
echo "Quick start:"
echo "  zpl2pdf --help"
echo "  zpl2pdf -i label.txt -o output"
echo "  zpl2pdf start -l /var/zpl2pdf/watch"
echo ""

exit 0
EOF

chmod +x "${BUILD_DIR}/DEBIAN/postinst"

# Create prerm script
echo "[*] Creating prerm script..."
cat > "${BUILD_DIR}/DEBIAN/prerm" << 'EOF'
#!/bin/bash
set -e

# Stop daemon if running
if command -v ZPL2PDF &> /dev/null; then
    ZPL2PDF stop 2>/dev/null || true
fi

exit 0
EOF

chmod +x "${BUILD_DIR}/DEBIAN/prerm"

# Calculate installed size
INSTALLED_SIZE=$(du -sk "${BUILD_DIR}" | cut -f1)
echo "Installed-Size: ${INSTALLED_SIZE}" >> "${BUILD_DIR}/DEBIAN/control"

# Build package
echo "[*] Building .deb package..."
dpkg-deb --build --root-owner-group "${BUILD_DIR}"

# Move to release folder
mkdir -p build/publish
mv "${BUILD_DIR}.deb" "build/publish/${PROJECT_NAME}_${VERSION}_${ARCH}.deb"

echo ""
echo "============================================"
echo "           Build Complete!"
echo "============================================"
echo "Package: build/publish/${PROJECT_NAME}_${VERSION}_${ARCH}.deb"
echo "Size: $(du -h build/publish/${PROJECT_NAME}_${VERSION}_${ARCH}.deb | cut -f1)"
echo ""
echo "To install:"
echo "  sudo dpkg -i build/publish/${PROJECT_NAME}_${VERSION}_${ARCH}.deb"
echo "  sudo apt-get install -f  # Fix dependencies"
echo ""
echo "To test:"
echo "  dpkg-deb -c build/publish/${PROJECT_NAME}_${VERSION}_${ARCH}.deb"
echo "  dpkg-deb -I build/publish/${PROJECT_NAME}_${VERSION}_${ARCH}.deb"
echo ""

