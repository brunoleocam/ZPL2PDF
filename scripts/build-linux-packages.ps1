# Build Linux packages (.deb and .rpm) using Docker
# Works on Windows with Docker Desktop

param(
    [string]$Version = "3.0.0",
    [switch]$DebOnly,
    [switch]$RpmOnly
)

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "   Building Linux Packages using Docker" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor White
Write-Host ""

# Check if Docker is running
try {
    docker ps | Out-Null
} catch {
    Write-Host "[ERROR] Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Create output directory
$publishDir = "build/publish"
if (-not (Test-Path $publishDir)) {
    New-Item -ItemType Directory -Path $publishDir -Force | Out-Null
}

# Build .deb package
if (-not $RpmOnly) {
    Write-Host "[*] Building .deb package for Ubuntu/Debian..." -ForegroundColor Yellow
    Write-Host ""
    
    # Create Dockerfile for .deb build
    $debDockerfile = @"
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install dependencies for .deb package creation
RUN apt-get update && apt-get install -y \
    dpkg-dev \
    debhelper \
    build-essential \
    gzip \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Copy project file first for better caching
COPY ZPL2PDF.csproj .
COPY Resources/ ./Resources/

# Restore dependencies
RUN dotnet restore ZPL2PDF.csproj

# Copy source code
COPY src/ ./src/
COPY README.md LICENSE CHANGELOG.md ./
COPY debian/ ./debian/

# Build application
RUN dotnet publish ZPL2PDF.csproj \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -o /app/publish \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=false

# Create .deb package structure
RUN mkdir -p /deb/DEBIAN /deb/usr/bin /deb/usr/share/doc/zpl2pdf /deb/usr/share/man/man1 && \
    cp /app/publish/ZPL2PDF /deb/usr/bin/ && \
    chmod +x /deb/usr/bin/ZPL2PDF && \
    cp README.md LICENSE CHANGELOG.md /deb/usr/share/doc/zpl2pdf/ && \
    echo 'Package: zpl2pdf\nVersion: 3.0.0\nSection: utils\nPriority: optional\nArchitecture: amd64\nDepends: libgdiplus, libc6-dev\nMaintainer: Bruno Leonardo Campos <brunoleocam@gmail.com>\nDescription: ZPL to PDF Converter' > /deb/DEBIAN/control && \
    dpkg-deb --build /deb /build/ZPL2PDF-v3.0.0-linux-amd64.deb

CMD ["sh", "-c", "cp /build/ZPL2PDF-v3.0.0-linux-amd64.deb /output/ && echo 'Package created successfully'"]
"@

    $debDockerfile | Out-File -FilePath "Dockerfile.deb" -Encoding UTF8
    
    # Create temporary .dockerignore
    $tempDockerignore = @"
bin/
obj/
build/
.vs/
.vscode/
.git/
*.user
*.suo
"@
    $tempDockerignore | Out-File -FilePath ".dockerignore.temp" -Encoding UTF8
    
    # Build Docker image
    Write-Host "[*] Building Docker image for .deb..." -ForegroundColor Cyan
    Copy-Item ".dockerignore.temp" ".dockerignore" -Force
    docker build -f Dockerfile.deb -t zpl2pdf-deb-builder .
    Remove-Item ".dockerignore.temp" -Force -ErrorAction SilentlyContinue
    
    # Run container to build package
    Write-Host "[*] Running build in container..." -ForegroundColor Cyan
    docker run --rm -v "${PWD}/${publishDir}:/output" zpl2pdf-deb-builder
    
    # Check if package was created
    $debFile = Get-ChildItem -Path $publishDir -Filter "*.deb" | Select-Object -First 1
    if ($debFile) {
        Write-Host "[OK] .deb package created: $($debFile.Name)" -ForegroundColor Green
        Write-Host "     Size: $([math]::Round($debFile.Length / 1MB, 2)) MB" -ForegroundColor Green
    } else {
        Write-Host "[ERROR] Failed to create .deb package" -ForegroundColor Red
    }
    
    # Cleanup
    Remove-Item "Dockerfile.deb" -Force
    Write-Host ""
}

# Build .rpm package
if (-not $DebOnly) {
    Write-Host "[*] Building .rpm package for Fedora/CentOS..." -ForegroundColor Yellow
    Write-Host ""
    
    # Create Dockerfile for .rpm build
    $rpmDockerfile = @"
FROM fedora:39

# Install dependencies
RUN dnf install -y \
    rpm-build \
    rpmdevtools \
    wget \
    ca-certificates \
    gzip \
    icu \
    libicu \
    && dnf clean all

# Install .NET SDK
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh && \
    chmod +x dotnet-install.sh && \
    ./dotnet-install.sh --channel 9.0 && \
    ln -s ~/.dotnet/dotnet /usr/bin/dotnet

WORKDIR /build

# Copy only necessary files
COPY ZPL2PDF.csproj .
COPY Resources/ ./Resources/
COPY src/ ./src/
COPY README.md LICENSE CHANGELOG.md ./

# Build application
RUN dotnet publish -c Release -r linux-x64 --self-contained true -o /app/publish

# Create RPM structure using simple tar method
RUN mkdir -p /rpm/usr/bin /rpm/usr/share/doc/zpl2pdf /rpm/usr/share/man/man1 && \
    cp /app/publish/ZPL2PDF /rpm/usr/bin/ && \
    chmod +x /rpm/usr/bin/ZPL2PDF && \
    cp README.md LICENSE CHANGELOG.md /rpm/usr/share/doc/zpl2pdf/ && \
    cd /rpm && tar czf /build/ZPL2PDF-v3.0.0-linux-x64-rpm.tar.gz usr/

CMD ["sh", "-c", "cp /build/ZPL2PDF-v3.0.0-linux-x64-rpm.tar.gz /output/ && echo 'Package created successfully'"]
"@

    $rpmDockerfile | Out-File -FilePath "Dockerfile.rpm" -Encoding UTF8
    
    # Build Docker image
    Write-Host "[*] Building Docker image for .rpm..." -ForegroundColor Cyan
    docker build -f Dockerfile.rpm -t zpl2pdf-rpm-builder .
    
    # Run container to build package
    Write-Host "[*] Running build in container..." -ForegroundColor Cyan
    docker run --rm -v "${PWD}/${publishDir}:/output" zpl2pdf-rpm-builder
    
    # Check if package was created
    $rpmFile = Get-ChildItem -Path $publishDir -Filter "*rpm.tar.gz" | Select-Object -First 1
    if ($rpmFile) {
        Write-Host "[OK] RPM tarball created: $($rpmFile.Name)" -ForegroundColor Green
        Write-Host "     Size: $([math]::Round($rpmFile.Length / 1MB, 2)) MB" -ForegroundColor Green
        Write-Host "     Note: This is a tarball. Extract with: tar -xzf $($rpmFile.Name)" -ForegroundColor Yellow
    } else {
        Write-Host "[ERROR] Failed to create RPM tarball" -ForegroundColor Red
    }
    
    # Cleanup
    Remove-Item "Dockerfile.rpm" -Force
    Write-Host ""
}

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "           Build Complete!" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Packages created in: $publishDir" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Test packages in Linux VM or WSL" -ForegroundColor White
Write-Host "  2. Upload to GitHub Release" -ForegroundColor White
Write-Host ""

