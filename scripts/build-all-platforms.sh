#!/bin/bash

# ZPL2PDF - Build All Platforms Script
# This script builds ZPL2PDF for all supported platforms

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

# Default values
SKIP_TESTS=false
OUTPUT_DIR="build/publish"

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  --skip-tests      Skip running tests"
            echo "  --output DIR      Output directory (default: build/publish)"
            echo "  -h, --help        Show this help"
            exit 0
            ;;
        *)
            echo -e "${RED}Unknown option: $1${NC}"
            exit 1
            ;;
    esac
done

# Functions
print_step() {
    echo -e "${BLUE}🔧 $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}$1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

echo -e "${BLUE}╔════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║  ZPL2PDF - Build All Platforms v2.0.0     ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════╝${NC}"
echo ""

# Check prerequisites
print_step "Checking prerequisites..."

if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed or not in PATH"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
print_success ".NET SDK found: $DOTNET_VERSION"

# Clean previous builds
print_step "Cleaning previous builds..."
if [[ -d "$OUTPUT_DIR" ]]; then
    rm -rf "$OUTPUT_DIR"
    print_success "Cleaned $OUTPUT_DIR"
fi
mkdir -p "$OUTPUT_DIR"

# Run tests
if [[ "$SKIP_TESTS" == false ]]; then
    print_step "Running tests..."
    dotnet test --configuration Release --verbosity quiet
    if [[ $? -eq 0 ]]; then
        print_success "All tests passed"
    else
        print_error "Tests failed"
        exit 1
    fi
fi

# Define platforms
declare -a PLATFORMS=(
    "win-x64:Windows 64-bit:zip"
    "win-x86:Windows 32-bit:zip"
    "win-arm64:Windows ARM64:zip"
    "linux-x64:Linux 64-bit:tar.gz"
    "linux-arm64:Linux ARM64:tar.gz"
    "linux-arm:Linux ARM:tar.gz"
    "osx-x64:macOS Intel (x64):tar.gz"
    "osx-arm64:macOS Apple Silicon (ARM64):tar.gz"
)

echo ""
echo -e "${BLUE}Building for ${#PLATFORMS[@]} platforms...${NC}"
echo ""

SUCCESS_COUNT=0
FAIL_COUNT=0

for platform in "${PLATFORMS[@]}"; do
    IFS=':' read -r RUNTIME NAME ARCHIVE_TYPE <<< "$platform"
    
    print_step "Building: $NAME ($RUNTIME)"
    
    PLATFORM_DIR="$OUTPUT_DIR/$RUNTIME"
    
    # Build for platform
    if dotnet publish ZPL2PDF.csproj \
        --configuration Release \
        --runtime "$RUNTIME" \
        --self-contained true \
        --output "$PLATFORM_DIR" \
        --verbosity quiet \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=false > /dev/null 2>&1; then
        
        # Get executable name
        if [[ $RUNTIME == win-* ]]; then
            EXE_NAME="ZPL2PDF.exe"
        else
            EXE_NAME="ZPL2PDF"
        fi
        
        # Check if executable exists
        EXE_PATH="$PLATFORM_DIR/$EXE_NAME"
        if [[ -f "$EXE_PATH" ]]; then
            FILE_SIZE=$(du -h "$EXE_PATH" | cut -f1)
            print_success "Built successfully! Size: $FILE_SIZE"
            
            # Create archive
            ARCHIVE_NAME="ZPL2PDF-v2.0.0-$RUNTIME"
            if [[ "$ARCHIVE_TYPE" == "zip" ]]; then
                if command -v zip &> /dev/null; then
                    (cd "$PLATFORM_DIR" && zip -r "../../$ARCHIVE_NAME.zip" . > /dev/null)
                    print_info "   Archive: $ARCHIVE_NAME.zip"
                else
                    # Fallback to tar
                    tar -czf "$OUTPUT_DIR/$ARCHIVE_NAME.tar.gz" -C "$PLATFORM_DIR" .
                    print_info "   Archive: $ARCHIVE_NAME.tar.gz (zip not available)"
                fi
            else
                tar -czf "$OUTPUT_DIR/$ARCHIVE_NAME.tar.gz" -C "$PLATFORM_DIR" .
                print_info "   Archive: $ARCHIVE_NAME.tar.gz"
            fi
            
            ((SUCCESS_COUNT++))
        else
            print_error "Build completed but executable not found!"
            ((FAIL_COUNT++))
        fi
    else
        print_error "Build failed!"
        ((FAIL_COUNT++))
    fi
    
    echo ""
done

# Create checksums
print_step "Creating checksums..."
CHECKSUM_FILE="$OUTPUT_DIR/SHA256SUMS.txt"
rm -f "$CHECKSUM_FILE"

for file in "$OUTPUT_DIR"/*.zip "$OUTPUT_DIR"/*.tar.gz; do
    if [[ -f "$file" ]]; then
        FILENAME=$(basename "$file")
        if command -v sha256sum &> /dev/null; then
            HASH=$(sha256sum "$file" | cut -d' ' -f1)
        elif command -v shasum &> /dev/null; then
            HASH=$(shasum -a 256 "$file" | cut -d' ' -f1)
        else
            print_info "No checksum tool available, skipping checksums"
            break
        fi
        echo "$HASH  $FILENAME" >> "$CHECKSUM_FILE"
    fi
done

if [[ -f "$CHECKSUM_FILE" ]]; then
    print_success "Checksums created: SHA256SUMS.txt"
fi

# Summary
echo ""
echo -e "${GREEN}╔════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║           Build Summary                    ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════╝${NC}"
echo ""
echo -e "  ${GREEN}✅ Successful builds: $SUCCESS_COUNT${NC}"
if [[ $FAIL_COUNT -gt 0 ]]; then
    echo -e "  ${RED}❌ Failed builds: $FAIL_COUNT${NC}"
fi
echo ""
echo -e "  ${YELLOW}📁 Output directory: $OUTPUT_DIR${NC}"
echo ""

# List all generated files
print_step "Generated files:"
for file in "$OUTPUT_DIR"/*; do
    if [[ -f "$file" && ! "$file" == *"/SHA256SUMS.txt" ]]; then
        FILENAME=$(basename "$file")
        FILESIZE=$(du -h "$file" | cut -f1)
        echo -e "  ${YELLOW}$FILENAME${NC} - $FILESIZE"
    fi
done

echo ""
if [[ $SUCCESS_COUNT -eq ${#PLATFORMS[@]} ]]; then
    echo -e "${GREEN}🎉 All platforms built successfully!${NC}"
    echo ""
    echo -e "${BLUE}Next steps:${NC}"
    echo -e "  ${YELLOW}1. Test each platform build${NC}"
    echo -e "  ${YELLOW}2. Create installers (Inno Setup for Windows)${NC}"
    echo -e "  ${YELLOW}3. Create packages (.deb, .rpm, Docker)${NC}"
    echo -e "  ${YELLOW}4. Upload to distribution channels${NC}"
else
    echo -e "${RED}⚠️  Some builds failed. Check errors above.${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}Thank you for using ZPL2PDF! 🚀${NC}"
echo ""
