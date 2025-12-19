# ============================================================================
# Script 08: Build Windows Installer
# ============================================================================
# Calls the existing build-installer.ps1 script
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir

function Write-Step {
    param([string]$Message)
    Write-Host "[08] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[!] $Message" -ForegroundColor Yellow
}

Write-Step "Building Windows installer (version $Version)..."

Set-Location $ProjectRoot

$buildScript = Join-Path $ProjectRoot "scripts\build-installer.ps1"
if (-not (Test-Path $buildScript)) {
    Write-Warning "Installer script not found: $buildScript"
    Write-Info "Skipping installer generation"
    exit 0
}

if ($DryRun) {
    Write-Info "[DRY RUN] Would execute: $buildScript -Version $Version"
    Write-Success "Dry run completed"
    exit 0
}

# Execute installer build script
& $buildScript -Version $Version

if ($LASTEXITCODE -eq 0) {
    Write-Success "Windows installer generated!"
} else {
    Write-Warning "Failed to generate installer (Inno Setup may not be installed)"
    Write-Info "Skipping installer generation"
    exit 0  # Don't fail the whole process if installer fails
}

