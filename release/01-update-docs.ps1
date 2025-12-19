# ============================================================================
# Script 01: Update Documentation
# ============================================================================
# Updates version in all documentation files
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
    Write-Host "[01] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Updating documentation for version $Version..."

Set-Location $ProjectRoot

# Update docs/README.md
$docsReadme = Join-Path $ProjectRoot "docs\README.md"
if (Test-Path $docsReadme) {
    $content = Get-Content $docsReadme -Raw
    $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
    $content = $content -replace 'version \d+\.\d+\.\d+', "version $Version"
    if (-not $DryRun) {
        Set-Content $docsReadme $content -NoNewline
        Write-Success "Updated: docs/README.md"
    } else {
        Write-Info "[DRY RUN] Would update: docs/README.md"
    }
}

# Update main README.md
$mainReadme = Join-Path $ProjectRoot "README.md"
if (Test-Path $mainReadme) {
    $content = Get-Content $mainReadme -Raw
    $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
    $content = $content -replace 'version \d+\.\d+\.\d+', "version $Version"
    if (-not $DryRun) {
        Set-Content $mainReadme $content -NoNewline
        Write-Success "Updated: README.md"
    } else {
        Write-Info "[DRY RUN] Would update: README.md"
    }
}

# Update other documentation files that may have version
$docFiles = @(
    "docs\development\CI_CD_WORKFLOW.md",
    "docs\development\DOCKER_PUBLISH_GUIDE.md",
    "docs\guides\DOCKER_GUIDE.md"
)

foreach ($docFile in $docFiles) {
    $fullPath = Join-Path $ProjectRoot $docFile
    if (Test-Path $fullPath) {
        $content = Get-Content $fullPath -Raw
        $originalContent = $content
        
        # Update version references
        $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
        $content = $content -replace 'version \d+\.\d+\.\d+', "version $Version"
        $content = $content -replace 'Version: \d+\.\d+\.\d+', "Version: $Version"
        
        if ($content -ne $originalContent) {
            if (-not $DryRun) {
                Set-Content $fullPath $content -NoNewline
                Write-Success "Updated: $docFile"
            } else {
                Write-Info "[DRY RUN] Would update: $docFile"
            }
        }
    }
}

Write-Success "Documentation updated!"

# Save checkpoint
Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 1

