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
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)

# Load checkpoint utilities
. (Join-Path $ScriptDir "_checkpoint-utils.ps1")
. (Join-Path $ScriptDir "_version-utils.ps1")

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

# Project version (ZPL2PDF.csproj <Version>) - also the assembly version shown by the app.
# NOTE: previous releases bumped this by hand; the scripts did not touch it.
$previousVersion = Get-ProjectVersion -ProjectRoot $ProjectRoot
if (Set-ProjectVersion -ProjectRoot $ProjectRoot -Version $Version -DryRun:$DryRun) {
    if ($DryRun) {
        Write-Info "[DRY RUN] Would update: ZPL2PDF.csproj <Version> $previousVersion -> $Version"
    } else {
        Write-Success "Updated: ZPL2PDF.csproj <Version> $previousVersion -> $Version"
    }
} else {
    Write-Info "ZPL2PDF.csproj already at $Version"
}

# Files whose release references (badge, "What's New", download URLs/assets) must point to the new version.
# Only those markers are changed - historical mentions of older versions are left untouched.
$targets = @("README.md", "docs\README.md")
foreach ($dir in @("docs\development", "docs\guides", "wiki")) {
    $fullDir = Join-Path $ProjectRoot $dir
    if (Test-Path $fullDir) {
        $targets += Get-ChildItem $fullDir -Filter "*.md" -File | ForEach-Object { Join-Path $dir $_.Name }
    }
}

foreach ($target in $targets) {
    $fullPath = Join-Path $ProjectRoot $target
    if (Update-ReleaseReferencesInFile -Path $fullPath -Version $Version -DryRun:$DryRun) {
        if ($DryRun) { Write-Info "[DRY RUN] Would update: $target" } else { Write-Success "Updated: $target" }
    }
}

Write-Info "Reminder: review README 'What's New' / 'Thanks' text by hand - the scripts do not rewrite prose."

Write-Success "Documentation updated!"

# Save checkpoint
Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 1

