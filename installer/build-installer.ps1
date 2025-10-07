# =============================================================================
# ZPL2PDF - Build Windows Installer
# =============================================================================
# This script automates the Inno Setup installer compilation
# Usage: .\installer\build-installer.ps1 [-Version "2.0.0"]
# =============================================================================

param(
    [string]$Version = "2.0.0",
    [string]$InnoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
)

# Colors
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"
$Cyan = "Cyan"

function Write-Step {
    param([string]$Message)
    Write-Host "`n[*] $Message" -ForegroundColor $Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor $Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor $Red
}

# Header
Write-Host "`n========================================" -ForegroundColor $Cyan
Write-Host "  ZPL2PDF - Build Windows Installer" -ForegroundColor $Cyan
Write-Host "  Version: $Version" -ForegroundColor $Cyan
Write-Host "========================================`n" -ForegroundColor $Cyan

# Check if Inno Setup is installed
Write-Step "Checking Inno Setup installation..."

if (-not (Test-Path $InnoSetupPath)) {
    Write-Error "Inno Setup not found at: $InnoSetupPath"
    Write-Host "`nPlease install Inno Setup from: https://jrsoftware.org/isinfo.php" -ForegroundColor $Yellow
    Write-Host "Or update the path using: -InnoSetupPath parameter" -ForegroundColor $Yellow
    exit 1
}

Write-Success "Inno Setup found: $InnoSetupPath"

# Check if build artifacts exist
Write-Step "Checking build artifacts..."

$zipFile = "build\publish\ZPL2PDF-v$Version-win-x64.zip"
if (-not (Test-Path $zipFile)) {
    Write-Error "Build artifacts not found: $zipFile"
    Write-Host "`nPlease build all platforms first:" -ForegroundColor $Yellow
    Write-Host "  .\scripts\build-all-platforms.ps1" -ForegroundColor $Yellow
    exit 1
}

Write-Success "Found: $zipFile"

# Extract the built executable
Write-Step "Extracting build artifacts..."

$extractPath = "build\publish\ZPL2PDF-v$Version-win-x64"

# Remove old extraction if exists
if (Test-Path $extractPath) {
    Remove-Item $extractPath -Recurse -Force
}

# Extract zip
Expand-Archive -Path $zipFile -DestinationPath $extractPath -Force
Write-Success "Artifacts extracted to: $extractPath"

# Verify executable exists
$exePath = "$extractPath\ZPL2PDF.exe"
if (-not (Test-Path $exePath)) {
    Write-Error "Executable not found after extraction: $exePath"
    exit 1
}

Write-Success "Executable found: ZPL2PDF.exe"

# Update version in .iss file if different
Write-Step "Updating version in .iss file..."

$issFile = "installer\ZPL2PDF-Setup.iss"
$issContent = Get-Content $issFile -Raw

if ($issContent -match '#define MyAppVersion "([^"]+)"') {
    $currentVersion = $matches[1]
    if ($currentVersion -ne $Version) {
        $issContent = $issContent -replace '#define MyAppVersion "[^"]+"', "#define MyAppVersion `"$Version`""
        Set-Content -Path $issFile -Value $issContent -NoNewline
        Write-Success "Version updated: $currentVersion -> $Version"
    } else {
        Write-Success "Version already correct: $Version"
    }
}

# Compile installer
Write-Step "Compiling installer with Inno Setup..."

$issFilePath = Resolve-Path $issFile
Write-Host "  Script: $issFilePath" -ForegroundColor Gray

& $InnoSetupPath $issFilePath

if ($LASTEXITCODE -eq 0) {
    Write-Success "Installer compiled successfully!"
    
    # Check output
    $installerPath = "installer\ZPL2PDF-Setup-$Version.exe"
    if (Test-Path $installerPath) {
        $size = (Get-Item $installerPath).Length / 1MB
        Write-Success "Installer created: $installerPath"
        Write-Host "  Size: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
        
        # Calculate checksum
        Write-Step "Calculating SHA256 checksum..."
        $hash = (Get-FileHash $installerPath -Algorithm SHA256).Hash
        Write-Success "SHA256: $hash"
        
        # Save checksum to file
        $checksumFile = "installer\ZPL2PDF-Setup-$Version.exe.sha256"
        "$hash  ZPL2PDF-Setup-$Version.exe" | Out-File -FilePath $checksumFile -Encoding ASCII
        Write-Success "Checksum saved: $checksumFile"
    }
} else {
    Write-Error "Compilation failed!"
    exit 1
}

# Summary
Write-Host "`n========================================" -ForegroundColor $Green
Write-Host "  Build Summary" -ForegroundColor $Green
Write-Host "========================================" -ForegroundColor $Green
Write-Host "  Version: $Version" -ForegroundColor White
Write-Host "  Installer: installer\ZPL2PDF-Setup-$Version.exe" -ForegroundColor White
Write-Host "  Checksum: installer\ZPL2PDF-Setup-$Version.exe.sha256" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor $Yellow
Write-Host "  1. Test the installer on a clean Windows VM" -ForegroundColor White
Write-Host "  2. Test all language options" -ForegroundColor White
Write-Host "  3. Test file associations (.zpl)" -ForegroundColor White
Write-Host "  4. Sign the installer (optional)" -ForegroundColor White
Write-Host "  5. Upload to GitHub Releases" -ForegroundColor White
Write-Host ""
Write-Host "Thank you for using ZPL2PDF!" -ForegroundColor $Green
