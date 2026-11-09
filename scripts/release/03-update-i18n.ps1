# ============================================================================
# Script 03: Update i18n Files
# ============================================================================
# Updates version in internationalization files
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
    Write-Host "[03] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Updating i18n files for version $Version..."

Set-Location $ProjectRoot

$i18nDir = Join-Path $ProjectRoot "docs\i18n"
if (-not (Test-Path $i18nDir)) {
    Write-Info "i18n directory not found (may be optional): $i18nDir"
    Write-Success "No update needed"
    exit 0
}

# Update all README.*.md files in i18n directory
$i18nFiles = Get-ChildItem $i18nDir -Filter "README.*.md"

if ($i18nFiles.Count -eq 0) {
    Write-Info "No i18n files found"
    Write-Success "No update needed"
    exit 0
}

foreach ($file in $i18nFiles) {
    # -Headings: translated "What's New" headings (e.g. "## Novidades na v3.1.3") also follow the release
    if (Update-ReleaseReferencesInFile -Path $file.FullName -Version $Version -Headings -DryRun:$DryRun) {
        if ($DryRun) { Write-Info "[DRY RUN] Would update: $($file.Name)" } else { Write-Success "Updated: $($file.Name)" }
    } else {
        Write-Info "No changes needed: $($file.Name)"
    }
}

Write-Success "i18n files updated!"

