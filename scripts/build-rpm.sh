#!/bin/bash
# Build .rpm package for Fedora/CentOS/RHEL

set -e

VERSION="2.0.0"
RELEASE="1"
ARCH="x86_64"  # or aarch64, armv7hl
PROJECT_NAME="zpl2pdf"
SPEC_FILE="rpm/${PROJECT_NAME}.spec"

echo "============================================"
echo "   Building .rpm Package for ZPL2PDF"
echo "============================================"
echo "Version: ${VERSION}-${RELEASE}"
echo "Architecture: ${ARCH}"
echo ""

# Install rpmbuild if not present
if ! command -v rpmbuild &> /dev/null; then
    echo "[!] rpmbuild not found. Install with:"
    echo "    sudo dnf install rpm-build rpmdevtools"
    echo "    or"
    echo "    sudo yum install rpm-build rpmdevtools"
    exit 1
fi

# Create rpmbuild directory structure
echo "[*] Creating rpmbuild directory structure..."
mkdir -p ~/rpmbuild/{BUILD,RPMS,SOURCES,SPECS,SRPMS}

# Build application
if [ "${ARCH}" = "x86_64" ]; then
    RUNTIME="linux-x64"
elif [ "${ARCH}" = "aarch64" ]; then
    RUNTIME="linux-arm64"
elif [ "${ARCH}" = "armv7hl" ]; then
    RUNTIME="linux-arm"
fi

echo "[*] Building for ${RUNTIME}..."
dotnet publish -c Release -r ${RUNTIME} --self-contained true

# Create source tarball
echo "[*] Creating source tarball..."
TARBALL_DIR="${PROJECT_NAME}-${VERSION}"
mkdir -p "/tmp/${TARBALL_DIR}"

cp -r bin/Release/net9.0/${RUNTIME}/publish/* "/tmp/${TARBALL_DIR}/"
cp README.md LICENSE CHANGELOG.md "/tmp/${TARBALL_DIR}/"

cd /tmp
tar -czf ~/rpmbuild/SOURCES/${PROJECT_NAME}-${VERSION}.tar.gz ${TARBALL_DIR}
cd -

# Copy spec file to SPECS directory
echo "[*] Copying spec file..."
cp ${SPEC_FILE} ~/rpmbuild/SPECS/

# Update spec file with current version
sed -i "s/Version:.*/Version:        ${VERSION}/" ~/rpmbuild/SPECS/${PROJECT_NAME}.spec
sed -i "s/Release:.*/Release:        ${RELEASE}%{?dist}/" ~/rpmbuild/SPECS/${PROJECT_NAME}.spec

# Build RPM
echo "[*] Building .rpm package..."
rpmbuild -bb ~/rpmbuild/SPECS/${PROJECT_NAME}.spec

# Copy to release folder
echo "[*] Copying to release folder..."
mkdir -p build/publish

# Find the built RPM
BUILT_RPM=$(find ~/rpmbuild/RPMS/${ARCH}/ -name "${PROJECT_NAME}-${VERSION}-${RELEASE}.*.${ARCH}.rpm" | head -n 1)

if [ -z "$BUILT_RPM" ]; then
    echo "[!] Error: RPM not found!"
    exit 1
fi

cp "$BUILT_RPM" build/publish/

echo ""
echo "============================================"
echo "           Build Complete!"
echo "============================================"
echo "Package: build/publish/$(basename $BUILT_RPM)"
echo "Size: $(du -h build/publish/$(basename $BUILT_RPM) | cut -f1)"
echo ""
echo "To install:"
echo "  sudo rpm -ivh build/publish/$(basename $BUILT_RPM)"
echo "  or"
echo "  sudo dnf install build/publish/$(basename $BUILT_RPM)"
echo ""
echo "To test:"
echo "  rpm -qlp build/publish/$(basename $BUILT_RPM)"
echo "  rpm -qip build/publish/$(basename $BUILT_RPM)"
echo ""

