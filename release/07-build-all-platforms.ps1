# ============================================================================
# Script 07: Build All Platforms
# ============================================================================
# Calls the existing build-all-platforms.ps1 script
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir

# Load checkpoint utilities
. (Join-Path $ScriptDir "_checkpoint-utils.ps1")

function Write-Step {
    param([string]$Message)
    Write-Host "[07] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Building for all platforms (version $Version)..."

Set-Location $ProjectRoot

$buildScript = Join-Path $ProjectRoot "scripts\build-all-platforms.ps1"
if (-not (Test-Path $buildScript)) {
    Write-Error "Build script not found: $buildScript"
    exit 1
}

if ($DryRun) {
    Write-Info "[DRY RUN] Would execute: $buildScript -Version $Version"
    Write-Success "Dry run completed"
    exit 0
}

# Execute build script
$params = @{
    Version = $Version
    OutputDir = "Assets"
}

if ($SkipTests) {
    $params.SkipTests = $true
}

& $buildScript @params

if ($LASTEXITCODE -eq 0) {
    Write-Success "Builds generated for all platforms!"
    # Save checkpoint
    Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 7
} else {
    Write-Error "Failed to generate builds"
    Mark-StepFailed -Version $Version -ProjectRoot $ProjectRoot -StepNumber 7 -ErrorMessage "Build failed with exit code $LASTEXITCODE"
    exit 1
}

