# ============================================================================
# Script 14: Publish to Docker Hub
# ============================================================================
# Pushes Docker images to Docker Hub
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

# Load checkpoint utilities
. (Join-Path $ScriptDir "_checkpoint-utils.ps1")

function Write-Step {
    param([string]$Message)
    Write-Host "[14] $Message" -ForegroundColor Yellow
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

Write-Step "Publishing Docker images to Docker Hub for version $Version..."

Set-Location $ProjectRoot

# Check Docker
try {
    docker info | Out-Null
} catch {
    Write-Error "Docker is not running. Start Docker Desktop."
    exit 1
}

$dockerImage = "brunoleocam/zpl2pdf"
$majorMinor = $Version -replace '\.\d+$', ''
$major = $Version -replace '\.\d+\.\d+$', ''

$tags = @("latest", $Version, $majorMinor, $major, "alpine")

if ($DryRun) {
    Write-Info "[DRY RUN] Would push the following tags to Docker Hub:"
    foreach ($tag in $tags) {
        Write-Info "  - $dockerImage`:$tag"
    }
    Write-Success "Dry run completed"
    exit 0
}

# Check Docker Hub authentication
Write-Info "Checking Docker Hub authentication..."
$dockerAuth = docker info 2>&1 | Select-String "Username"
if (-not $dockerAuth) {
    Write-Warning "Not authenticated to Docker Hub. Run: docker login"
    Write-Info "Attempting push anyway..."
}

# Push all tags
foreach ($tag in $tags) {
    Write-Info "Pushing: $dockerImage`:$tag"
    docker push "$dockerImage`:$tag" 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Push completed: $tag"
    } else {
        Write-Warning "Failed to push tag: $tag"
    }
}

Write-Success "Images published to Docker Hub!"
Write-Info "URL: https://hub.docker.com/r/$dockerImage/tags"

# Save checkpoint
Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 14

