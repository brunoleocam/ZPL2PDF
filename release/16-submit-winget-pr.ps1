# ============================================================================
# Script 16: Submit WinGet PR
# ============================================================================
# Updates WinGet fork and creates PR to microsoft/winget-pkgs
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
$RepoName = "ZPL2PDF"
$WinGetRepo = "microsoft/winget-pkgs"
$WinGetFork = "$RepoOwner/winget-pkgs"
$PackageId = "brunoleocam.ZPL2PDF"

function Write-Step {
    param([string]$Message)
    Write-Host "[16] $Message" -ForegroundColor Yellow
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

Write-Step "Submitting WinGet PR (version $Version)..."

Set-Location $ProjectRoot

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

$branchName = "$PackageId-$Version"
$tempDir = Join-Path $env:TEMP "winget-pkgs-$Version"
$manifestSource = Join-Path $ProjectRoot "manifests"

if ($DryRun) {
    Write-Info "[DRY RUN] Would clone fork: $WinGetFork"
    Write-Info "[DRY RUN] Would create branch: $branchName"
    Write-Info "[DRY RUN] Would copy manifests from: $manifestSource"
    Write-Info "[DRY RUN] Would create PR to: $WinGetRepo"
    Write-Success "Dry run completed"
    exit 0
}

try {
    # Clean temporary directory
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force
    }
    
    # Clone fork
    Write-Info "Cloning winget-pkgs fork..."
    gh repo clone $WinGetFork $tempDir -- --depth=1
    
    Set-Location $tempDir
    
    # Configure git
    git config user.name $RepoOwner
    git config user.email "$RepoOwner@users.noreply.github.com"
    
    # Sync with upstream
    Write-Info "Syncing with upstream..."
    git remote add upstream "https://github.com/$WinGetRepo.git" 2>$null
    git fetch upstream master --depth=1
    git reset --hard upstream/master
    
    # Create branch
    Write-Info "Creating branch: $branchName"
    git checkout -b $branchName
    
    # Create structure and copy manifests
    $targetDir = Join-Path $tempDir "manifests\b\brunoleocam\ZPL2PDF\$Version"
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    
    Write-Info "Copying manifests..."
    Copy-Item "$manifestSource\*.yaml" $targetDir -Force
    
    # Commit and push
    Write-Info "Committing..."
    git add .
    git commit -m "New version: $PackageId version $Version"
    
    Write-Info "Pushing..."
    git push origin $branchName --force
    
    # Create PR
    Write-Info "Creating Pull Request..."
    gh pr create --repo $WinGetRepo `
        --title "$PackageId version $Version" `
        --body "Automated submission for $PackageId version $Version" `
        --head "$RepoOwner`:$branchName" `
        --base master
    
    Write-Success "PR submitted to WinGet!"
    Write-Info "Track at: https://github.com/$WinGetRepo/pulls"
    
} catch {
    Write-Warning "Failed to submit PR: $_"
    Write-Info "Submit manually at: https://github.com/$WinGetRepo/compare"
} finally {
    Set-Location $ProjectRoot
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

