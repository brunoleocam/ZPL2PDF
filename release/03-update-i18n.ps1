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
$ProjectRoot = Split-Path -Parent $ScriptDir

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
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    # Update version references
    $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
    $content = $content -replace 'version \d+\.\d+\.\d+', "version $Version"
    $content = $content -replace 'Version: \d+\.\d+\.\d+', "Version: $Version"
    
    if ($content -ne $originalContent) {
        if (-not $DryRun) {
            Set-Content $file.FullName $content -NoNewline -Encoding UTF8
            Write-Success "Updated: $($file.Name)"
        } else {
            Write-Info "[DRY RUN] Would update: $($file.Name)"
        }
    } else {
        Write-Info "No changes needed: $($file.Name)"
    }
}

Write-Success "i18n files updated!"

