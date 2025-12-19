# ============================================================================
# Script 09: Build Linux Packages (.deb and .rpm)
# ============================================================================
# Calls the existing build-linux-packages.ps1 script
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
    Write-Host "[09] $Message" -ForegroundColor Yellow
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

Write-Step "Building Linux packages (.deb and .rpm) for version $Version..."

Set-Location $ProjectRoot

# Check if Docker is running
try {
    docker ps | Out-Null
} catch {
    Write-Warning "Docker is not running. Linux packages will not be generated."
    Write-Info "Start Docker Desktop and run this script again"
    exit 0  # Don't fail the whole process
}

$buildScript = Join-Path $ProjectRoot "scripts\build-linux-packages.ps1"
if (-not (Test-Path $buildScript)) {
    Write-Warning "Linux packages script not found: $buildScript"
    Write-Info "Skipping Linux packages generation"
    exit 0
}

if ($DryRun) {
    Write-Info "[DRY RUN] Would execute: $buildScript -Version $Version"
    Write-Success "Dry run completed"
    exit 0
}

# Execute Linux packages build script
& $buildScript -Version $Version

if ($LASTEXITCODE -eq 0) {
    Write-Success "Linux packages generated!"
} else {
    Write-Warning "Failed to generate Linux packages (may be Docker connection issue)"
    Write-Info "Skipping Linux packages generation"
    exit 0  # Don't fail the whole process
}

