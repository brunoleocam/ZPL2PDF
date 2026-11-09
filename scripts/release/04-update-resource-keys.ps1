# ============================================================================
# Script 04: Update Localization.ResourceKeys
# ============================================================================
# Checks and updates ResourceKeys.cs if necessary
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)

# Load version utilities
. (Join-Path $ScriptDir "_version-utils.ps1")

function Write-Step {
    param([string]$Message)
    Write-Host "[04] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Checking Localization.ResourceKeys for version $Version..."

Set-Location $ProjectRoot

$resourceKeysPath = Join-Path $ProjectRoot "src\Shared\Localization\ResourceKeys.cs"
if (-not (Test-Path $resourceKeysPath)) {
    Write-Error "ResourceKeys.cs file not found: $resourceKeysPath"
    exit 1
}

# ResourceKeys.cs holds resource key constants, not version text (the app version comes from
# ZPL2PDF.csproj). Only release markers would be touched; a blanket vX.Y.Z replace over C# source is unsafe.
if (Update-ReleaseReferencesInFile -Path $resourceKeysPath -Version $Version -DryRun:$DryRun) {
    if ($DryRun) { Write-Info "[DRY RUN] Would update: ResourceKeys.cs" } else { Write-Success "Updated: ResourceKeys.cs" }
} else {
    Write-Info "No changes needed in ResourceKeys.cs"
    Write-Info "ResourceKeys.cs doesn't contain version references (expected behavior)"
}

Write-Success "Localization.ResourceKeys checked!"

