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
$ProjectRoot = Split-Path -Parent $ScriptDir

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

$content = Get-Content $resourceKeysPath -Raw
$originalContent = $content

# ResourceKeys.cs usually doesn't contain explicit version,
# but we can check if there are comments or documentation with version
$content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
$content = $content -replace 'version \d+\.\d+\.\d+', "version $Version"

if ($content -ne $originalContent) {
    if (-not $DryRun) {
        Set-Content $resourceKeysPath $content -NoNewline
        Write-Success "Updated: ResourceKeys.cs"
    } else {
        Write-Info "[DRY RUN] Would update: ResourceKeys.cs"
    }
} else {
    Write-Info "No changes needed in ResourceKeys.cs"
    Write-Info "ResourceKeys.cs doesn't contain version references (expected behavior)"
}

Write-Success "Localization.ResourceKeys checked!"

