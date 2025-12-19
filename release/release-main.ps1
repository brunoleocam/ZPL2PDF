# ============================================================================
# Main Script: Complete ZPL2PDF Release
# ============================================================================
# Orchestrates all release scripts in sequence
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDocker,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipWinGet,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipGitHubRelease,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,
    
    [Parameter(Mandatory=$false)]
    [int]$StartFromStep = 1,
    
    [Parameter(Mandatory=$false)]
    [switch]$Resume
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir

# Load checkpoint utilities
. (Join-Path $ScriptDir "_checkpoint-utils.ps1")

# ============================================================================
# Output Functions
# ============================================================================
function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Step {
    param([string]$Step, [string]$Message)
    Write-Host ""
    Write-Host "[$Step] $Message" -ForegroundColor Yellow
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

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# ============================================================================
# Version Validation
# ============================================================================
if ($Version -notmatch '^\d+\.\d+\.\d+') {
    Write-ErrorMsg "Invalid version format. Use SemVer (e.g., 3.1.0)"
    exit 1
}

# ============================================================================
# Main
# ============================================================================
Write-Header "ZPL2PDF Complete Release v$Version"

if ($DryRun) {
    Write-Warning "DRY RUN MODE - No changes will be published"
    Write-Host ""
}

Write-Info "Configuration:"
Write-Info "  Version: $Version"
Write-Info "  SkipTests: $SkipTests"
Write-Info "  SkipDocker: $SkipDocker"
Write-Info "  SkipWinGet: $SkipWinGet"
Write-Info "  SkipGitHubRelease: $SkipGitHubRelease"
Write-Info "  DryRun: $DryRun"
Write-Info "  StartFromStep: $StartFromStep"
Write-Info "  Resume: $Resume"
Write-Host ""

# Initialize checkpoint
$checkpoint = Initialize-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
Write-Info "Checkpoint initialized: .release-checkpoint-$Version.json"

# If Resume, show current status
if ($Resume) {
    Show-CheckpointStatus -Version $Version -ProjectRoot $ProjectRoot
    Write-Host ""
}

# List of scripts and their conditions
$scripts = @(
    @{ Number = 1; Name = "01-update-docs.ps1"; Description = "Update documentation"; Skip = $false },
    @{ Number = 2; Name = "02-update-resources.ps1"; Description = "Update Resources Messages"; Skip = $false },
    @{ Number = 3; Name = "03-update-i18n.ps1"; Description = "Update i18n files"; Skip = $false },
    @{ Number = 4; Name = "04-update-resource-keys.ps1"; Description = "Update Localization.ResourceKeys"; Skip = $false },
    @{ Number = 5; Name = "05-update-changelog.ps1"; Description = "Update CHANGELOG"; Skip = $false },
    @{ Number = 6; Name = "06-update-dockerfile.ps1"; Description = "Update Dockerfile"; Skip = $false },
    @{ Number = 7; Name = "07-build-all-platforms.ps1"; Description = "Build all platforms"; Skip = $false },
    @{ Number = 8; Name = "08-build-installer.ps1"; Description = "Build Windows installer"; Skip = $false },
    @{ Number = 9; Name = "09-build-linux-packages.ps1"; Description = "Build Linux packages (.deb and .rpm)"; Skip = $SkipDocker },
    @{ Number = 10; Name = "10-update-manifests.ps1"; Description = "Update WinGet manifests"; Skip = $false },
    @{ Number = 11; Name = "11-build-docker-images.ps1"; Description = "Build Docker images"; Skip = $SkipDocker },
    @{ Number = 12; Name = "12-create-version-branch.ps1"; Description = "Create version branch and PR"; Skip = $false },
    @{ Number = 13; Name = "13-create-github-release.ps1"; Description = "Create GitHub release"; Skip = $SkipGitHubRelease },
    @{ Number = 14; Name = "14-publish-dockerhub.ps1"; Description = "Publish to Docker Hub"; Skip = $SkipDocker },
    @{ Number = 15; Name = "15-publish-ghcr.ps1"; Description = "Publish to GitHub Package Registry"; Skip = $SkipDocker },
    @{ Number = 16; Name = "16-submit-winget-pr.ps1"; Description = "Submit WinGet PR"; Skip = $SkipWinGet }
)

$failedSteps = @()
$skippedSteps = @()
$completedSteps = @()

foreach ($script in $scripts) {
    # Skip if before StartFromStep
    if ($script.Number -lt $StartFromStep) {
        Write-Info "Skipping step $($script.Number) (StartFromStep = $StartFromStep)"
        continue
    }
    
    # Skip if marked to skip
    if ($script.Skip) {
        Write-Step "$($script.Number)/16" "$($script.Description) (SKIPPED)"
        Mark-StepSkipped -Version $Version -ProjectRoot $ProjectRoot -StepNumber $script.Number
        $skippedSteps += $script.Number
        continue
    }
    
    # Check if already completed (if Resume)
    if ($Resume -and (Test-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber $script.Number)) {
        Write-Step "$($script.Number)/16" "$($script.Description) (ALREADY COMPLETED)"
        Write-Info "Step was already completed previously. Skipping..."
        $completedSteps += $script.Number
        continue
    }
    
    Write-Step "$($script.Number)/16" "$($script.Description)"
    
    $scriptPath = Join-Path $ScriptDir $script.Name
    
    if (-not (Test-Path $scriptPath)) {
        Write-ErrorMsg "Script not found: $scriptPath"
        $failedSteps += $script.Number
        continue
    }
    
    try {
        # Prepare parameters
        $params = @{
            Version = $Version
        }
        
        if ($DryRun) {
            $params.DryRun = $true
        }
        
        if ($script.Number -eq 7 -and $SkipTests) {
            $params.SkipTests = $true
        }
        
        # Execute script
        & $scriptPath @params
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Step $($script.Number) completed: $($script.Description)"
            Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber $script.Number
            $completedSteps += $script.Number
        } else {
            Write-ErrorMsg "Step $($script.Number) failed: $($script.Description)"
            Mark-StepFailed -Version $Version -ProjectRoot $ProjectRoot -StepNumber $script.Number -ErrorMessage "Exit code: $LASTEXITCODE"
            $failedSteps += $script.Number
            
            Write-Host ""
            Write-Warning "Release interrupted at step $($script.Number)"
            Write-Info "To continue from where it stopped, run:"
            Write-Info "  .\release\release-main.ps1 -Version `"$Version`" -StartFromStep $($script.Number + 1)"
            Write-Host ""
            
            $continue = Read-Host "Do you want to continue anyway? (Y/N)"
            if ($continue -ne "Y" -and $continue -ne "y") {
                break
            }
        }
    } catch {
        Write-ErrorMsg "Error executing step $($script.Number): $_"
        Mark-StepFailed -Version $Version -ProjectRoot $ProjectRoot -StepNumber $script.Number -ErrorMessage $_.ToString()
        $failedSteps += $script.Number
        
        Write-Host ""
        Write-Warning "Release interrupted at step $($script.Number)"
        Write-Info "To continue from where it stopped, run:"
        Write-Info "  .\release\release-main.ps1 -Version `"$Version`" -StartFromStep $($script.Number + 1)"
        Write-Host ""
        
        $continue = Read-Host "Do you want to continue anyway? (Y/N)"
        if ($continue -ne "Y" -and $continue -ne "y") {
            break
        }
    }
}

# ============================================================================
# Final Summary
# ============================================================================
Write-Header "Release Summary"

Write-Host "Completed steps: $($completedSteps.Count)" -ForegroundColor Green
if ($completedSteps.Count -gt 0) {
    Write-Info "  $($completedSteps -join ', ')"
}

if ($skippedSteps.Count -gt 0) {
    Write-Host "Skipped steps: $($skippedSteps.Count)" -ForegroundColor Yellow
    Write-Info "  $($skippedSteps -join ', ')"
}

if ($failedSteps.Count -gt 0) {
    Write-Host "Failed steps: $($failedSteps.Count)" -ForegroundColor Red
    Write-Info "  $($failedSteps -join ', ')"
    Write-Host ""
    Write-Warning "Some steps failed. Review the errors above."
    Write-Info "To run a specific step:"
    Write-Info "  .\release\XX-script-name.ps1 -Version `"$Version`""
    exit 1
} else {
    Write-Host ""
    Write-Success "Release v$Version completed successfully!"
    Write-Host ""
    
    # Show final checkpoint status
    Show-CheckpointStatus -Version $Version -ProjectRoot $ProjectRoot
    
    Write-Info "Next steps:"
    Write-Info "  1. Review the PR created in step 12"
    Write-Info "  2. Approve and merge the PR to main"
    Write-Info "  3. Wait for WinGet PR approval (may take days)"
    Write-Info "  4. Announce the release"
    Write-Host ""
    Write-Info "Checkpoint saved at: .release-checkpoint-$Version.json"
    Write-Host ""
}
