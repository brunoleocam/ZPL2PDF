#!/bin/bash

# ZPL2PDF Release Script
# This script automates the release process for ZPL2PDF

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Functions
print_color() {
    printf "${1}${2}${NC}\n"
}

print_step() {
    print_color "$BLUE" "🔧 $1"
}

print_success() {
    print_color "$GREEN" "✅ $1"
}

print_warning() {
    print_color "$YELLOW" "⚠️  $1"
}

print_error() {
    print_color "$RED" "❌ $1"
}

# Parse command line arguments
VERSION=""
PRE_RELEASE=false
DRY_RUN=false

while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--version)
            VERSION="$2"
            shift 2
            ;;
        -p|--pre-release)
            PRE_RELEASE=true
            shift
            ;;
        -d|--dry-run)
            DRY_RUN=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 -v VERSION [OPTIONS]"
            echo "Options:"
            echo "  -v, --version VERSION    Version to release (required)"
            echo "  -p, --pre-release        Mark as pre-release"
            echo "  -d, --dry-run           Dry run (don't push changes)"
            echo "  -h, --help              Show this help"
            exit 0
            ;;
        *)
            print_error "Unknown option $1"
            exit 1
            ;;
    esac
done

# Validate version
if [[ -z "$VERSION" ]]; then
    print_error "Version is required. Use -v or --version"
    exit 1
fi

if [[ ! $VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    print_error "Invalid version format. Use semantic versioning (e.g., 2.0.0)"
    exit 1
fi

print_color "$BLUE" "🚀 ZPL2PDF Release Script v2.0.0"
print_color "$BLUE" "================================="
print_color "$YELLOW" "Version: $VERSION"
print_color "$YELLOW" "Pre-release: $PRE_RELEASE"
print_color "$YELLOW" "Dry run: $DRY_RUN"

# Check prerequisites
print_step "Checking prerequisites..."

# Check if git is available
if ! command -v git &> /dev/null; then
    print_error "Git is not installed or not in PATH"
    exit 1
fi

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed or not in PATH"
    exit 1
fi

# Check if we're in a git repository
if [[ ! -d ".git" ]]; then
    print_error "Not in a git repository"
    exit 1
fi

# Check if working directory is clean
if [[ -n $(git status --porcelain) ]]; then
    print_error "Working directory is not clean. Please commit or stash changes first."
    print_color "$YELLOW" "Uncommitted changes:"
    git status --porcelain
    exit 1
fi

print_success "Prerequisites check passed"

# Update version in project files
print_step "Updating version in project files..."

# Update ZPL2PDF.csproj
if [[ -f "ZPL2PDF.csproj" ]]; then
    sed -i.bak "s/<Version>.*<\/Version>/<Version>$VERSION<\/Version>/" ZPL2PDF.csproj
    rm ZPL2PDF.csproj.bak
    print_success "Updated ZPL2PDF.csproj"
fi

# Update ApplicationConstants.cs
if [[ -f "src/Shared/Constants/ApplicationConstants.cs" ]]; then
    sed -i.bak "s/APPLICATION_VERSION = \".*\"/APPLICATION_VERSION = \"$VERSION\"/" src/Shared/Constants/ApplicationConstants.cs
    rm src/Shared/Constants/ApplicationConstants.cs.bak
    print_success "Updated src/Shared/Constants/ApplicationConstants.cs"
fi

# Update winget manifest
if [[ -f "winget-manifest.yaml" ]]; then
    sed -i.bak "s/PackageVersion: .*/PackageVersion: $VERSION/" winget-manifest.yaml
    rm winget-manifest.yaml.bak
    print_success "Updated winget-manifest.yaml"
fi

# Update RPM spec
if [[ -f "rpm/zpl2pdf.spec" ]]; then
    sed -i.bak "s/Version:.*/Version:        $VERSION/" rpm/zpl2pdf.spec
    rm rpm/zpl2pdf.spec.bak
    print_success "Updated rpm/zpl2pdf.spec"
fi

# Update Debian control
if [[ -f "debian/control" ]]; then
    sed -i.bak "s/Version: .*/Version: $VERSION/" debian/control
    rm debian/control.bak
    print_success "Updated debian/control"
fi

# Update CHANGELOG.md
print_step "Updating CHANGELOG.md..."
if [[ -f "CHANGELOG.md" ]]; then
    TODAY=$(date +%Y-%m-%d)
    sed -i.bak "s/## \[Unreleased\]/## [$VERSION]/" CHANGELOG.md
    sed -i.bak "s/## \[Unreleased\] - [0-9]\{4\}-[0-9]\{2\}-[0-9]\{2\}/## [$VERSION] - $TODAY/" CHANGELOG.md
    rm CHANGELOG.md.bak
    print_success "Updated CHANGELOG.md"
fi

# Build and test
print_step "Building and testing..."

# Restore dependencies
print_color "$YELLOW" "Restoring dependencies..."
dotnet restore
if [[ $? -ne 0 ]]; then
    print_error "Failed to restore dependencies"
    exit 1
fi

# Build solution
print_color "$YELLOW" "Building solution..."
dotnet build --configuration Release
if [[ $? -ne 0 ]]; then
    print_error "Failed to build solution"
    exit 1
fi

# Run tests
print_color "$YELLOW" "Running tests..."
dotnet test --configuration Release --no-build
if [[ $? -ne 0 ]]; then
    print_error "Tests failed"
    exit 1
fi

print_success "Build and tests completed successfully"

# Create release builds using build-all-platforms script
print_step "Creating release builds..."

BUILD_DIR="build/publish"
BUILD_SCRIPT="scripts/build-all-platforms.sh"

if [[ -f "$BUILD_SCRIPT" ]]; then
    bash "$BUILD_SCRIPT" --output "$BUILD_DIR"
    
    if [[ $? -ne 0 ]]; then
        print_error "Build script failed"
        exit 1
    fi
    
    print_success "All platforms built successfully"
else
    print_error "Build script not found: $BUILD_SCRIPT"
    exit 1
fi

# Checksums are already created by build-all-platforms script
print_success "Checksums available at: $BUILD_DIR/SHA256SUMS.txt"

# Create Git tag
if [[ "$DRY_RUN" == false ]]; then
    print_step "Creating Git tag..."
    
    TAG_NAME="v$VERSION"
    if [[ "$PRE_RELEASE" == true ]]; then
        TAG_NAME="$TAG_NAME-rc"
    fi
    
    git add .
    git commit -m "chore: release $VERSION"
    git tag -a "$TAG_NAME" -m "Release $VERSION"
    
    print_success "Created tag $TAG_NAME"
    
    # Push changes
    print_step "Pushing changes..."
    git push origin main
    git push origin "$TAG_NAME"
    
    print_success "Pushed changes and tag"
else
    print_warning "Dry run mode - skipping Git operations"
fi

# Create GitHub release
if [[ "$DRY_RUN" == false ]]; then
    print_step "Creating GitHub release..."
    print_warning "GitHub release creation requires manual intervention or GitHub CLI (gh)"
    print_color "$YELLOW" "To create release manually:"
    print_color "$YELLOW" "1. Go to: https://github.com/YOUR-USERNAME/ZPL2PDF/releases/new"
    print_color "$YELLOW" "2. Choose tag: v$VERSION"
    print_color "$YELLOW" "3. Upload files from: $BUILD_DIR"
fi

# Summary
print_color "$GREEN" ""
print_color "$GREEN" "🎉 Release process completed!"
print_color "$GREEN" "================================"
print_color "$YELLOW" "Version: $VERSION"
print_color "$YELLOW" "Build directory: $BUILD_DIR"
print_color "$YELLOW" "Tag: v$VERSION"

if [[ "$DRY_RUN" == false ]]; then
    print_color "$BLUE" ""
    print_color "$BLUE" "Next steps:"
    print_color "$YELLOW" "1. Verify the build artifacts in $BUILD_DIR"
    print_color "$YELLOW" "2. Create GitHub release with the prepared notes"
    print_color "$YELLOW" "3. Update package repositories (winget, apt, etc.)"
    print_color "$YELLOW" "4. Announce the release"
else
    print_color "$BLUE" ""
    print_color "$BLUE" "This was a dry run. No changes were pushed to Git."
fi

print_color "$GREEN" ""
print_color "$GREEN" "Thank you for using ZPL2PDF! 🚀"
