# ============================================================================
# Script 05: Update CHANGELOG
# ============================================================================
# Updates CHANGELOG.md with the new version
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
    Write-Host "[05] $Message" -ForegroundColor Yellow
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

Write-Step "Updating CHANGELOG.md for version $Version..."

Set-Location $ProjectRoot

$changelogPath = Join-Path $ProjectRoot "CHANGELOG.md"
if (-not (Test-Path $changelogPath)) {
    Write-Warning "CHANGELOG.md not found. Creating new file..."
    if (-not $DryRun) {
        $newChangelog = @"
# 📋 Changelog

All notable changes to ZPL2PDF will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [$Version] - $(Get-Date -Format 'yyyy-MM-dd')

### ✨ Added
- Initial release

---

## [Unreleased]

### ✨ Added
- 

### 🔧 Improved
- 

### 🐛 Fixed
- 

### 🔒 Security
- 

---
"@
        Set-Content $changelogPath $newChangelog -Encoding UTF8
        Write-Success "CHANGELOG.md created"
    } else {
        Write-Info "[DRY RUN] Would create new CHANGELOG.md"
    }
    exit 0
}

$content = Get-Content $changelogPath -Raw -Encoding UTF8
$originalContent = $content

# Check if entry already exists for this version
if ($content -match "## \[$Version\]") {
    Write-Warning "CHANGELOG.md already contains an entry for version $Version"
    Write-Info "Skipping update (to avoid overwriting existing content)"
    exit 0
}

# Replace [Unreleased] with [Version] with date
$date = Get-Date -Format 'yyyy-MM-dd'
$unreleasedPattern = '## \[Unreleased\](\s*-\s*\d{4}-\d{2}-\d{2})?'
$replacement = "## [$Version] - $date"

if ($content -match $unreleasedPattern) {
    $content = $content -replace $unreleasedPattern, $replacement
    
    # Add [Unreleased] section at the end (before the last ---)
    $unreleasedSection = @"

## [Unreleased]

### ✨ Added
- 

### 🔧 Improved
- 

### 🐛 Fixed
- 

### 🔒 Security
- 

---
"@
    
    # Insert before the last ---
    if ($content -match '(?s)(.*)(\n---\s*$)') {
        $content = $matches[1] + $unreleasedSection
    } else {
        $content = $content + $unreleasedSection
    }
    
    if ($content -ne $originalContent) {
        if (-not $DryRun) {
            Set-Content $changelogPath $content -NoNewline -Encoding UTF8
            Write-Success "CHANGELOG.md updated with version $Version"
        } else {
            Write-Info "[DRY RUN] Would update CHANGELOG.md"
        }
    }
} else {
    Write-Warning "[Unreleased] section not found in CHANGELOG.md"
    Write-Info "Manually add entry for version $Version"
}

Write-Success "CHANGELOG updated!"

