# ============================================================================
# Script 06: Update Dockerfile
# ============================================================================
# Updates version in Dockerfile
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
    Write-Host "[06] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Updating Dockerfile for version $Version..."

Set-Location $ProjectRoot

$dockerfilePath = Join-Path $ProjectRoot "Dockerfile"
if (-not (Test-Path $dockerfilePath)) {
    Write-Error "Dockerfile not found: $dockerfilePath"
    exit 1
}

$content = Get-Content $dockerfilePath -Raw
$originalContent = $content

# Update LABEL version
$content = $content -replace 'version="[^"]*"', "version=`"$Version`""
$content = $content -replace 'version=\d+\.\d+\.\d+', "version=$Version"
$content = $content -replace 'version: \d+\.\d+\.\d+', "version: $Version"

# Update other version references if any
$content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"

if ($content -ne $originalContent) {
    if (-not $DryRun) {
        Set-Content $dockerfilePath $content -NoNewline
        Write-Success "Dockerfile updated"
    } else {
        Write-Info "[DRY RUN] Would update Dockerfile"
    }
} else {
    Write-Info "No changes needed in Dockerfile"
}

Write-Success "Dockerfile updated!"

