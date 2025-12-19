# ============================================================================
# Script 12: Create Version Branch and PR
# ============================================================================
# Creates branch release/v{Version} and Pull Request to main
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
    Write-Host "[12] $Message" -ForegroundColor Yellow
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

Write-Step "Creating version branch and PR for version $Version..."

Set-Location $ProjectRoot

# Check if in a Git repository
if (-not (Test-Path ".git")) {
    Write-Error "Not in a Git repository"
    exit 1
}

# Check if there are uncommitted changes
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Info "Changes detected. They will be committed in the version branch."
}

$branchName = "release/v$Version"

if ($DryRun) {
    Write-Info "[DRY RUN] Would create branch: $branchName"
    Write-Info "[DRY RUN] Would commit all changes"
    Write-Info "[DRY RUN] Would create PR to main"
    Write-Success "Dry run completed"
    exit 0
}

# Check if branch already exists
$existingBranch = git branch --list $branchName
if ($existingBranch) {
    Write-Warning "Branch $branchName already exists"
    Write-Info "Using existing branch"
    git checkout $branchName
} else {
    # Create new branch
    git checkout -b $branchName
    Write-Success "Branch created: $branchName"
}

# Add all changes
git add .

# Make commit
$commitMessage = "chore: release v$Version"
git commit -m $commitMessage

# Push branch
git push origin $branchName --set-upstream

# Create Pull Request using GitHub CLI
if (Get-Command gh -ErrorAction SilentlyContinue) {
    $prTitle = "Release v$Version"
    $prBody = @"
## Release v$Version

This PR contains all changes for release v$Version.

### Changes
- Version updated in all files
- Documentation updated
- CHANGELOG updated
- Builds generated
- Manifests updated

### Checklist
- [x] Version updated
- [x] Documentation updated
- [x] CHANGELOG updated
- [x] Builds generated
- [x] Manifests updated

**Waiting for approval to merge to main.**
"@
    
    gh pr create --title $prTitle --body $prBody --base main --head $branchName
    Write-Success "Pull Request created!"
} else {
    Write-Warning "GitHub CLI not found. Create PR manually:"
    Write-Info "https://github.com/brunoleocam/ZPL2PDF/compare/main...$branchName"
}

Write-Success "Version branch created and PR submitted!"

# Save checkpoint
Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 12

