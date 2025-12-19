# ============================================================================
# Utility Script: Show Checkpoint Status
# ============================================================================
# Displays the current release checkpoint status
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir

# Load checkpoint utilities
. (Join-Path $ScriptDir "_checkpoint-utils.ps1")

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host ""
}

Write-Header "Checkpoint Status - Version $Version"

Show-CheckpointStatus -Version $Version -ProjectRoot $ProjectRoot

# Step mapping
$stepNames = @{
    1 = "Update documentation"
    2 = "Update Resources Messages"
    3 = "Update i18n files"
    4 = "Update Localization.ResourceKeys"
    5 = "Update CHANGELOG"
    6 = "Update Dockerfile"
    7 = "Build all platforms"
    8 = "Build Windows installer"
    9 = "Build Linux packages (.deb and .rpm)"
    10 = "Update WinGet manifests"
    11 = "Build Docker images"
    12 = "Create version branch and PR"
    13 = "Create GitHub release"
    14 = "Publish to Docker Hub"
    15 = "Publish to GitHub Package Registry"
    16 = "Submit WinGet PR"
}

$checkpoint = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot

if ($checkpoint) {
    Write-Host "Step Details:" -ForegroundColor Cyan
    Write-Host ""
    
    for ($i = 1; $i -le 16; $i++) {
        $stepName = if ($stepNames.ContainsKey($i)) { $stepNames[$i] } else { "Step $i" }
        $completed = if ($checkpoint.CompletedSteps) { @($checkpoint.CompletedSteps) -contains $i } else { $false }
        $failed = if ($checkpoint.FailedSteps) { @($checkpoint.FailedSteps) -contains $i } else { $false }
        $skipped = if ($checkpoint.SkippedSteps) { @($checkpoint.SkippedSteps) -contains $i } else { $false }
        
        $status = "⏳ Pending"
        $color = "White"
        
        if ($completed) {
            $status = "✅ Completed"
            $color = "Green"
        } elseif ($failed) {
            $status = "❌ Failed"
            $color = "Red"
        } elseif ($skipped) {
            $status = "⏭️  Skipped"
            $color = "Yellow"
        }
        
        Write-Host "  [$i] $stepName" -ForegroundColor $color -NoNewline
        Write-Host " - $status" -ForegroundColor $color
    }
    
    Write-Host ""
    Write-Host "Checkpoint file:" -ForegroundColor Cyan
    $checkpointPath = Get-CheckpointPath -Version $Version -ProjectRoot $ProjectRoot
    Write-Host "  $checkpointPath" -ForegroundColor White
    Write-Host ""
    
    # Show next steps
    $completed = if ($checkpoint.CompletedSteps) { @($checkpoint.CompletedSteps) } else { @() }
    $failed = if ($checkpoint.FailedSteps) { @($checkpoint.FailedSteps) } else { @() }
    
    if ($failed.Count -gt 0) {
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "  1. Fix issues in failed steps" -ForegroundColor White
        Write-Host "  2. Resume release with:" -ForegroundColor White
        Write-Host "     .\release\release-main.ps1 -Version `"$Version`" -Resume" -ForegroundColor Cyan
        Write-Host ""
    } elseif ($completed.Count -lt 16) {
        $nextStep = 1
        for ($i = 1; $i -le 16; $i++) {
            if (-not ($completed -contains $i)) {
                $nextStep = $i
                break
            }
        }
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "  Resume release from step $nextStep:" -ForegroundColor White
        Write-Host "    .\release\release-main.ps1 -Version `"$Version`" -StartFromStep $nextStep" -ForegroundColor Cyan
        Write-Host ""
    } else {
        Write-Host "✅ All steps completed!" -ForegroundColor Green
        Write-Host ""
    }
} else {
    Write-Host "No checkpoint found for version $Version" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To start a release:" -ForegroundColor Cyan
    Write-Host "  .\release\release-main.ps1 -Version `"$Version`"" -ForegroundColor White
    Write-Host ""
}
