# ============================================================================
# Script 11: Build Docker Images
# ============================================================================
# Builds Docker images with all necessary tags
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
    Write-Host "[11] $Message" -ForegroundColor Yellow
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

Write-Step "Building Docker images for version $Version..."

Set-Location $ProjectRoot

# Check if Docker is running
try {
    docker info | Out-Null
} catch {
    Write-Error "Docker is not running. Start Docker Desktop."
    exit 1
}

$dockerImage = "brunoleocam/zpl2pdf"
$ghcrImage = "ghcr.io/brunoleocam/zpl2pdf"

# Calculate tags
$majorMinor = $Version -replace '\.\d+$', ''
$major = $Version -replace '\.\d+\.\d+$', ''

if ($DryRun) {
    Write-Info "[DRY RUN] Would build Docker image with tags:"
    Write-Info "  - $dockerImage`:latest"
    Write-Info "  - $dockerImage`:$Version"
    Write-Info "  - $dockerImage`:$majorMinor"
    Write-Info "  - $dockerImage`:$major"
    Write-Info "  - $dockerImage`:alpine"
    Write-Success "Dry run completed"
    exit 0
}

# Build image
Write-Info "Building Docker image..."
docker build -t "$dockerImage`:$Version" -t "$dockerImage`:latest" -t "$dockerImage`:alpine" .

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build Docker image"
    exit 1
}

# Create additional tags
Write-Info "Creating additional tags..."
docker tag "$dockerImage`:$Version" "$dockerImage`:$majorMinor"
docker tag "$dockerImage`:$Version" "$dockerImage`:$major"

# Tags for GHCR (will be used in step 15)
docker tag "$dockerImage`:$Version" "$ghcrImage`:$Version"
docker tag "$dockerImage`:$Version" "$ghcrImage`:latest"
docker tag "$dockerImage`:$Version" "$ghcrImage`:alpine"
docker tag "$dockerImage`:$Version" "$ghcrImage`:$majorMinor"
docker tag "$dockerImage`:$Version" "$ghcrImage`:$major"

Write-Success "Docker images generated successfully!"
Write-Info "Tags created: latest, $Version, $majorMinor, $major, alpine"

