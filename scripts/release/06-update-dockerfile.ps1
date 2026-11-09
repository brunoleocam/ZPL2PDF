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
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)

# Load version utilities
. (Join-Path $ScriptDir "_version-utils.ps1")

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

$content = Read-ReleaseTextFile -Path $dockerfilePath
$originalContent = $content

# Only the image LABEL version is updated (e.g. version="3.1.3"). Base image tags such as
# dotnet/sdk:10.0.301 or alpine:3.24 are managed by Dependabot and must not be rewritten.
$content = $content -replace '(?m)^(\s*(?:LABEL\s+)?(?:org\.opencontainers\.image\.)?version=)"[^"]*"', ('${1}"' + $Version + '"')
if ($content -eq $originalContent -and $content -notmatch 'version="') {
    Write-Info "No LABEL version found in Dockerfile - nothing to update"
}

if ($content -ne $originalContent) {
    if (-not $DryRun) {
        Write-ReleaseTextFile -Path $dockerfilePath -Content $content
        Write-Success "Dockerfile updated"
    } else {
        Write-Info "[DRY RUN] Would update Dockerfile"
    }
} else {
    Write-Info "No changes needed in Dockerfile"
}

Write-Success "Dockerfile updated!"

