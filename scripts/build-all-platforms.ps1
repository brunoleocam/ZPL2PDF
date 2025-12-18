# ZPL2PDF - Build All Platforms Script
# This script builds ZPL2PDF for all supported platforms

param(
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputDir = "build/publish"
)

# Set error action preference
$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "[*] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "     $Message" -ForegroundColor Yellow
}

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

Write-Host "============================================" -ForegroundColor Blue
Write-Host "  ZPL2PDF - Build All Platforms v3.0.0" -ForegroundColor Blue
Write-Host "============================================" -ForegroundColor Blue
Write-Host ""

# Check prerequisites
Write-Step "Checking prerequisites..."

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-ErrorMsg ".NET SDK is not installed or not in PATH"
    exit 1
}

$dotnetVersion = dotnet --version
Write-Success ".NET SDK found: $dotnetVersion"

# Clean previous builds
Write-Step "Cleaning previous builds..."
if (Test-Path $OutputDir) {
    Remove-Item $OutputDir -Recurse -Force
    Write-Success "Cleaned $OutputDir"
}
New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null

# Run tests
if (-not $SkipTests) {
    Write-Step "Running tests..."
    dotnet test --configuration Release --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Success "All tests passed"
    } else {
        Write-ErrorMsg "Tests failed"
        exit 1
    }
}

# Define platforms
$platforms = @(
    @{ Runtime = "win-x64";     Name = "Windows 64-bit";              Archive = "zip" },
    @{ Runtime = "win-x86";     Name = "Windows 32-bit";              Archive = "zip" },
    @{ Runtime = "win-arm64";   Name = "Windows ARM64";               Archive = "zip" },
    @{ Runtime = "linux-x64";   Name = "Linux 64-bit";                Archive = "tar.gz" },
    @{ Runtime = "linux-arm64"; Name = "Linux ARM64";                 Archive = "tar.gz" },
    @{ Runtime = "linux-arm";   Name = "Linux ARM";                   Archive = "tar.gz" },
    @{ Runtime = "osx-x64";     Name = "macOS Intel (x64)";           Archive = "tar.gz" },
    @{ Runtime = "osx-arm64";   Name = "macOS Apple Silicon (ARM64)"; Archive = "tar.gz" }
)

Write-Host ""
Write-Host "${Blue}Building for $($platforms.Count) platforms...${Reset}"
Write-Host ""

$successCount = 0
$failCount = 0

foreach ($platform in $platforms) {
    $runtime = $platform.Runtime
    $name = $platform.Name
    $archiveType = $platform.Archive
    
    Write-Step "Building: $name ($runtime)"
    
    $platformDir = "$OutputDir/$runtime"
    
    try {
        # Build for platform
        dotnet publish ZPL2PDF.csproj `
            --configuration Release `
            --runtime $runtime `
            --self-contained true `
            --output $platformDir `
            --verbosity quiet `
            -p:PublishSingleFile=true `
            -p:PublishTrimmed=false
        
        if ($LASTEXITCODE -eq 0) {
            # Get executable name
            if ($runtime -like "win-*") {
                $exeName = "ZPL2PDF.exe"
            } else {
                $exeName = "ZPL2PDF"
            }
            
            # Check if executable exists
            $exePath = Join-Path $platformDir $exeName
            if (Test-Path $exePath) {
                $fileSize = (Get-Item $exePath).Length / 1MB
                Write-Success "Built successfully! Size: $([math]::Round($fileSize, 2)) MB"
                
                # Create archive
                $archiveName = "ZPL2PDF-v3.0.0-$runtime"
                if ($archiveType -eq "zip") {
                    Compress-Archive -Path "$platformDir/*" -DestinationPath "$OutputDir/$archiveName.zip" -Force
                    Write-Info "   Archive: $archiveName.zip"
                } else {
                    # Create tar.gz (requires tar command)
                    if (Get-Command tar -ErrorAction SilentlyContinue) {
                        tar -czf "$OutputDir/$archiveName.tar.gz" -C $platformDir .
                        Write-Info "   Archive: $archiveName.tar.gz"
                    } else {
                        Write-Info "   Archive: skipped (tar not available)"
                    }
                }
                
                $successCount++
            } else {
                Write-ErrorMsg "Build completed but executable not found!"
                $failCount++
            }
        } else {
            Write-ErrorMsg "Build failed!"
            $failCount++
        }
    }
    catch {
        Write-ErrorMsg "Build error: $_"
        $failCount++
    }
    
    Write-Host ""
}

# Create checksums
Write-Step "Creating checksums..."
$checksumFile = "$OutputDir/SHA256SUMS.txt"
Remove-Item $checksumFile -ErrorAction SilentlyContinue

Get-ChildItem $OutputDir -Filter "*.zip" | ForEach-Object {
    $hash = Get-FileHash $_.FullName -Algorithm SHA256
    Add-Content $checksumFile "$($hash.Hash)  $($_.Name)"
}

Get-ChildItem $OutputDir -Filter "*.tar.gz" | ForEach-Object {
    $hash = Get-FileHash $_.FullName -Algorithm SHA256
    Add-Content $checksumFile "$($hash.Hash)  $($_.Name)"
}

Write-Success "Checksums created: SHA256SUMS.txt"

# Summary
Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "           Build Summary                    " -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""
Write-Host "  [OK] Successful builds: $successCount" -ForegroundColor Green
if ($failCount -gt 0) {
    Write-Host "  [ERROR] Failed builds: $failCount" -ForegroundColor Red
}
Write-Host ""
Write-Host "  Output directory: $OutputDir" -ForegroundColor Yellow
Write-Host ""

# List all generated files
Write-Step "Generated files:"
Get-ChildItem $OutputDir -File | ForEach-Object {
    $size = $_.Length / 1MB
    Write-Host "  $($_.Name) - $([math]::Round($size, 2)) MB" -ForegroundColor Yellow
}

Write-Host ""
if ($successCount -eq $platforms.Count) {
    Write-Host "[SUCCESS] All platforms built successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Blue
    Write-Host "  1. Test each platform build" -ForegroundColor Yellow
    Write-Host "  2. Create installers (Inno Setup for Windows)" -ForegroundColor Yellow
    Write-Host "  3. Create packages (.deb, .rpm, Docker)" -ForegroundColor Yellow
    Write-Host "  4. Upload to distribution channels" -ForegroundColor Yellow
} else {
    Write-Host "[WARNING] Some builds failed. Check errors above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Thank you for using ZPL2PDF!" -ForegroundColor Green
Write-Host ""
