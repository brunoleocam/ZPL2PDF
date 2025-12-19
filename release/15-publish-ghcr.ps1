# ============================================================================
# Script 15: Publish to GitHub Package Registry
# ============================================================================
# Pushes Docker images to GHCR (ghcr.io)
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
$RepoOwner = "brunoleocam"
$GhcrImage = "ghcr.io/brunoleocam/zpl2pdf"

function Write-Step {
    param([string]$Message)
    Write-Host "[15] $Message" -ForegroundColor Yellow
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

Write-Step "Publishing Docker images to GHCR for version $Version..."

Set-Location $ProjectRoot

# Check Docker
try {
    docker info | Out-Null
} catch {
    Write-Error "Docker is not running. Start Docker Desktop."
    exit 1
}

# Check GitHub CLI
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) not found. Install: https://cli.github.com/"
    exit 1
}

# Check authentication
$ghAuth = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "GitHub CLI not authenticated. Run: gh auth login"
    exit 1
}

# Check write:packages scope
$ghScopes = gh auth status 2>&1 | Select-String "Token scopes"
if ($ghScopes -notmatch "write:packages") {
    Write-Warning "Token doesn't have 'write:packages' scope"
    Write-Info "Run: gh auth refresh -h github.com -s write:packages"
    exit 1
}

$majorMinor = $Version -replace '\.\d+$', ''
$major = $Version -replace '\.\d+\.\d+$', ''
$tags = @("latest", $Version, $majorMinor, $major, "alpine")

if ($DryRun) {
    Write-Info "[DRY RUN] Would push the following tags to GHCR:"
    foreach ($tag in $tags) {
        Write-Info "  - $GhcrImage`:$tag"
    }
    Write-Success "Dry run completed"
    exit 0
}

# Login to GHCR
Write-Info "Authenticating to GHCR..."
$ghToken = gh auth token 2>&1
if ($LASTEXITCODE -eq 0) {
    echo $ghToken | docker login ghcr.io -u $RepoOwner --password-stdin 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Authenticated to GHCR!"
    } else {
        Write-Error "Failed to login to GHCR"
        exit 1
    }
} else {
    Write-Error "Failed to get token from GitHub CLI"
    exit 1
}

# Push all tags
foreach ($tag in $tags) {
    Write-Info "Pushing: $GhcrImage`:$tag"
    docker push "$GhcrImage`:$tag" 2>&1 | Out-Null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Push completed: $tag"
    } else {
        Write-Warning "Failed to push tag: $tag"
    }
}

Write-Success "Images published to GHCR!"
Write-Info "URL: https://github.com/$RepoOwner?tab=packages&package_name=zpl2pdf"

