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
$ProjectRoot = Split-Path -Parent (Split-Path -Parent $ScriptDir)

# Load version utilities
. (Join-Path $ScriptDir "_version-utils.ps1")

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

$content = Read-ReleaseTextFile -Path $changelogPath

# Check if entry already exists for this version
if ($content -match "(?m)^## \[$([regex]::Escape($Version))\]") {
    Write-Warning "CHANGELOG.md already contains an entry for version $Version"
    Write-Info "Skipping update (to avoid overwriting existing content)"
    exit 0
}

# The [Unreleased] heading becomes the new version heading, in place (top of the file).
# It does NOT append an empty [Unreleased] template at the end of the file (older behavior put it
# below the oldest release). Add a new [Unreleased] section by hand when the next change lands.
$unreleasedMatches = [regex]::Matches($content, '(?m)^## \[Unreleased\][^\r\n]*')
if ($unreleasedMatches.Count -eq 0) {
    Write-Warning "[Unreleased] section not found in CHANGELOG.md"
    Write-Info "Manually add entry for version $Version"
} else {
    if ($unreleasedMatches.Count -gt 1) {
        Write-Warning "Found $($unreleasedMatches.Count) [Unreleased] headings - merge them into one before releasing."
        Write-Info "Only the first one will be renamed."
    }

    # Warn when the section has no real bullet (only the empty template "- ")
    $first = $unreleasedMatches[0]
    $rest = $content.Substring($first.Index + $first.Length)
    $nextHeading = [regex]::Match($rest, '(?m)^## \[')
    $body = if ($nextHeading.Success) { $rest.Substring(0, $nextHeading.Index) } else { $rest }
    if ($body -notmatch '(?m)^\s*[-*]\s+\S') {
        Write-Warning "[Unreleased] has no entries - fill in the release notes before publishing."
    }

    $date = Get-Date -Format 'yyyy-MM-dd'
    $updated = $content.Substring(0, $first.Index) + "## [$Version] - $date" + $content.Substring($first.Index + $first.Length)

    if (-not $DryRun) {
        Write-ReleaseTextFile -Path $changelogPath -Content $updated
        Write-Success "CHANGELOG.md updated: [Unreleased] -> [$Version] - $date"
    } else {
        Write-Info "[DRY RUN] Would rename [Unreleased] -> [$Version] - $date"
    }
}

Write-Success "CHANGELOG updated!"
