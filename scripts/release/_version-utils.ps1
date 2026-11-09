# ============================================================================
# Release Version Utilities
# ============================================================================
# Shared helpers for the release scripts.
#
# Why this exists: the previous scripts replaced EVERY "vX.Y.Z" / "version X.Y.Z"
# in README/docs with the new version, which rewrote history (e.g. "landed in
# v3.1.2" became "landed in v3.1.3") and corrupted older references. These helpers
# only touch markers that must always point to the current/latest release:
#   - README version badge            (badge/version-X.Y.Z-)
#   - "What's New in vX.Y.Z" / "Recent highlights (vX.Y.Z)"
#   - release download URLs/assets    (releases/download/vX.Y.Z/, ZPL2PDF-vX.Y.Z-<platform>, ZPL2PDF-Setup-X.Y.Z.exe)
#   Section headings/prose in translated READMEs ("What's New", thanks) are NOT rewritten; edit them by hand.
# Files are read and written as UTF-8 without altering line endings, and the BOM
# state of each file is preserved (Windows PowerShell 5.1 safe).
# ============================================================================

function Read-ReleaseTextFile {
    param([Parameter(Mandatory=$true)][string]$Path)
    return [System.IO.File]::ReadAllText($Path, (New-Object System.Text.UTF8Encoding($false)))
}

function Write-ReleaseTextFile {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][AllowEmptyString()][string]$Content
    )
    $bytes = [System.IO.File]::ReadAllBytes($Path)
    $hadBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    [System.IO.File]::WriteAllText($Path, $Content, (New-Object System.Text.UTF8Encoding($hadBom)))
}

function Update-ReleaseReferences {
    param(
        [Parameter(Mandatory=$true)][AllowEmptyString()][string]$Content,
        [Parameter(Mandatory=$true)][string]$Version
    )

    $semver = '\d+\.\d+\.\d+'
    $result = $Content

    # README badge: badge/version-3.1.3-blue.svg
    $result = $result -replace "(badge/version-)$semver(-)", ('${1}' + $Version + '${2}')
    # "What's New in v3.1.3" and "Recent highlights (v3.1.3)"
    $result = $result -replace "(What's New in v)$semver", ('${1}' + $Version)
    $result = $result -replace "(Recent highlights \(v)$semver(\))", ('${1}' + $Version + '${2}')
    # Download URLs and asset names
    $result = $result -replace "(releases/download/v)$semver(/)", ('${1}' + $Version + '${2}')
    $result = $result -replace "(ZPL2PDF-v)$semver(-(?:win|linux|osx))", ('${1}' + $Version + '${2}')
    $result = $result -replace "(ZPL2PDF-Setup-)$semver(\.exe)", ('${1}' + $Version + '${2}')

    return $result
}

function Update-ReleaseReferencesInFile {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][string]$Version,
        [switch]$DryRun
    )

    if (-not (Test-Path $Path)) { return $false }

    $original = Read-ReleaseTextFile -Path $Path
    $updated = Update-ReleaseReferences -Content $original -Version $Version

    if ($updated -eq $original) { return $false }

    if (-not $DryRun) {
        Write-ReleaseTextFile -Path $Path -Content $updated
    }
    return $true
}

function Get-ProjectVersion {
    param([Parameter(Mandatory=$true)][string]$ProjectRoot)

    $csproj = Join-Path $ProjectRoot "ZPL2PDF.csproj"
    if (-not (Test-Path $csproj)) { return $null }
    $m = [regex]::Match((Read-ReleaseTextFile -Path $csproj), '<Version>([^<]+)</Version>')
    if ($m.Success) { return $m.Groups[1].Value }
    return $null
}

function Set-ProjectVersion {
    param(
        [Parameter(Mandatory=$true)][string]$ProjectRoot,
        [Parameter(Mandatory=$true)][string]$Version,
        [switch]$DryRun
    )

    $csproj = Join-Path $ProjectRoot "ZPL2PDF.csproj"
    if (-not (Test-Path $csproj)) { throw "ZPL2PDF.csproj not found: $csproj" }

    $content = Read-ReleaseTextFile -Path $csproj
    if ($content -notmatch '<Version>[^<]+</Version>') { throw "No <Version> element found in ZPL2PDF.csproj" }

    $updated = $content -replace '<Version>[^<]+</Version>', "<Version>$Version</Version>"
    if ($updated -eq $content) { return $false }

    if (-not $DryRun) {
        Write-ReleaseTextFile -Path $csproj -Content $updated
    }
    return $true
}
