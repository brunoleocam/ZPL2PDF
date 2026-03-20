# ============================================================================
# Script 13: Create/Update GitHub Release
# ============================================================================
# Creates Git tag and GitHub release with all assets
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,

    [Parameter(Mandatory=$false)]
    [switch]$UpdateExistingReleaseNotes
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$RepoOwner = "brunoleocam"
$RepoName = "ZPL2PDF"

# Emoji literals as codepoints to avoid mojibake due to script file encoding.
$EmojiBox = [System.Char]::ConvertFromUtf32(0x1F4E6)      # package
$EmojiDocker = [System.Char]::ConvertFromUtf32(0x1F433)   # whale (docker)
$EmojiRocket = [System.Char]::ConvertFromUtf32(0x1F680)   # rocket
$EmojiParty = [System.Char]::ConvertFromUtf32(0x1F389)     # party
$EmojiBook = [System.Char]::ConvertFromUtf32(0x1F4DA)      # books

# Load checkpoint utilities
. (Join-Path $ScriptDir "_checkpoint-utils.ps1")

function Write-Step {
    param([string]$Message)
    Write-Host "[13] $Message" -ForegroundColor Yellow
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

function Generate-ReleaseNotes {
    param(
        [string]$Version,
        [string]$ProjectRoot
    )
    
    $releaseNotes = @"
## ZPL2PDF v$Version

"@
    
    # Try to extract changelog entry for this version
    $changelogPath = Join-Path $ProjectRoot "CHANGELOG.md"
    if (Test-Path $changelogPath) {
        Write-Info "Reading CHANGELOG.md for version $Version..."
        $changelogContent = Get-Content $changelogPath -Raw -Encoding UTF8
        
        # Extract the section for this version
        # Pattern: ## [Version] - Date or ## [Version]
        $versionPattern = "(?s)##\s*\[$([regex]::Escape($Version))\](?:\s*-\s*[^\r\n]+)?(.*?)(?=##\s*\[|\z)"
        $match = [regex]::Match($changelogContent, $versionPattern)
        
        if ($match.Success) {
            $versionSection = $match.Groups[1].Value.Trim()
            
            # Clean up the section (remove extra whitespace, normalize)
            $versionSection = $versionSection -replace '\r\n\r\n+', "`n`n"
            $versionSection = $versionSection.Trim()
            
            if ($versionSection.Length -gt 50) {
                Write-Info "Found changelog entry for version $Version"
                $releaseNotes += $versionSection
                $releaseNotes += "`n`n---`n`n"
            } else {
                Write-Warning "Changelog entry for version $Version is too short, using fallback"
            }
        } else {
            Write-Warning "Changelog entry for version $Version not found, using fallback"
        }
    } else {
        Write-Warning "CHANGELOG.md not found, using fallback"
    }
    
    # Add downloads section
    $releaseNotes += @"
### $EmojiBox Downloads

| Platform | File |
|----------|------|
| Windows Installer | ZPL2PDF-Setup-$Version.exe |
| Windows x64 | ZPL2PDF-v$Version-win-x64.zip |
| Windows x86 | ZPL2PDF-v$Version-win-x86.zip |
| Windows ARM64 | ZPL2PDF-v$Version-win-arm64.zip |
| Linux x64 | ZPL2PDF-v$Version-linux-x64.tar.gz |
| Linux ARM64 | ZPL2PDF-v$Version-linux-arm64.tar.gz |
| Linux ARM | ZPL2PDF-v$Version-linux-arm.tar.gz |
| macOS Intel | ZPL2PDF-v$Version-osx-x64.tar.gz |
| macOS Apple Silicon | ZPL2PDF-v$Version-osx-arm64.tar.gz |

### $EmojiDocker Docker

``````bash
docker pull brunoleocam/zpl2pdf:$Version
docker pull ghcr.io/brunoleocam/zpl2pdf:$Version
``````

### $EmojiBook Documentation

- [Full Documentation](https://github.com/$RepoOwner/$RepoName#readme)
- [Installation Guide](https://github.com/$RepoOwner/$RepoName#-installation)
- [Usage Examples](https://github.com/$RepoOwner/$RepoName#-usage)

"@
    
    # Try to extract "What's New" section from README if available
    $readmePath = Join-Path $ProjectRoot "README.md"
    if (Test-Path $readmePath) {
        Write-Info "Reading README.md for additional context..."
        $readmeContent = Get-Content $readmePath -Raw -Encoding UTF8
        
        # Look for "What's New" section that mentions this version
        $whatsNewPattern = "(?s)##\s*$([regex]::Escape($EmojiRocket))\s*\*\*What's New in v$([regex]::Escape($Version))\*\*(.*?)(?=##|### v\d|$)"
        $match = [regex]::Match($readmeContent, $whatsNewPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        
        if ($match.Success) {
            $whatsNewSection = $match.Groups[1].Value.Trim()
            # Extract only the first few lines to avoid too much content
            $whatsNewLines = $whatsNewSection -split "`n" | Select-Object -First 15
            $whatsNewShort = ($whatsNewLines -join "`n").Trim()
            
            if ($whatsNewShort.Length -gt 50) {
                Write-Info "Found 'What's New' section for version $Version"
                $releaseNotes += @"

### $EmojiParty What's New

$whatsNewShort

"@
            }
        }
    }
    
    # Add changelog link
    $prevVersion = Get-PreviousVersion -Version $Version -ProjectRoot $ProjectRoot
    if ($prevVersion) {
        $releaseNotes += @"
**Full Changelog**: https://github.com/$RepoOwner/$RepoName/compare/v$prevVersion...v$Version
"@
    } else {
        $releaseNotes += @"
**Full Changelog**: https://github.com/$RepoOwner/$RepoName/compare/v$(([version]$Version).Major).$(([version]$Version).Minor).$([math]::Max(0, ([version]$Version).Build - 1))...v$Version
"@
    }
    
    return $releaseNotes
}

function Get-PreviousVersion {
    param(
        [string]$Version,
        [string]$ProjectRoot
    )
    
    $changelogPath = Join-Path $ProjectRoot "CHANGELOG.md"
    if (-not (Test-Path $changelogPath)) {
        return $null
    }
    
    $changelogContent = Get-Content $changelogPath -Raw -Encoding UTF8
    # Find all version entries
    $versionPattern = '##\s*\[(\d+\.\d+\.\d+)\]'
    $versionMatches = [regex]::Matches($changelogContent, $versionPattern)
    
    $currentIndex = -1
    for ($i = 0; $i -lt $versionMatches.Count; $i++) {
        if ($versionMatches[$i].Groups[1].Value -eq $Version) {
            $currentIndex = $i
            break
        }
    }
    
    if ($currentIndex -gt 0 -and $currentIndex -lt $versionMatches.Count) {
        return $versionMatches[$currentIndex + 1].Groups[1].Value
    }
    
    return $null
}

Write-Step "Creating GitHub release for version $Version..."

Set-Location $ProjectRoot

# Check GitHub CLI
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) not found. Install: https://cli.github.com/"
    exit 1
}

# Check authentication
$null = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "GitHub CLI not authenticated. Run: gh auth login"
    exit 1
}

$tagName = "v$Version"
$assetsDir = Join-Path $ProjectRoot "release"

if ($DryRun) {
    Write-Info "[DRY RUN] Would create tag: $tagName"
    Write-Info "[DRY RUN] Would create GitHub release"
    Write-Info "[DRY RUN] Would upload files from: $assetsDir"
    Write-Success "Dry run completed"
    exit 0
}

# Create Git tag (if it doesn't exist)
$existingTag = git tag -l $tagName
if (-not $existingTag) {
    Write-Info "Creating Git tag: $tagName"
    git tag -a $tagName -m "Release $tagName"
    git push origin $tagName
    Write-Success "Tag created and pushed"
} else {
    Write-Info "Tag $tagName already exists"
}

# Check if release already exists (SilentlyContinue prevents "release not found" from stopping the script)
$prevErrAction = $ErrorActionPreference
$ErrorActionPreference = 'SilentlyContinue'
try {
    $null = gh release view $tagName --repo "$RepoOwner/$RepoName" 2>$null
    $releaseExists = ($LASTEXITCODE -eq 0)
} finally {
    $ErrorActionPreference = $prevErrAction
}
if ($releaseExists -and -not $UpdateExistingReleaseNotes) {
    Write-Warning "Release $tagName already exists"
    Write-Info "Skipping release creation"
    exit 0
}

# Generate release notes from CHANGELOG and README
Write-Info "Generating release notes from CHANGELOG.md and README.md..."
$releaseNotes = Generate-ReleaseNotes -Version $Version -ProjectRoot $ProjectRoot

# Ensure release directory exists
if (-not (Test-Path $assetsDir)) {
    New-Item -ItemType Directory -Path $assetsDir -Force | Out-Null
    Write-Info "Created release directory"
}

# Collect files for upload (installer, packages, checksums; no source archives)
$releaseFiles = @()
if (Test-Path $assetsDir) {
    $releaseFiles = Get-ChildItem $assetsDir -File | Where-Object {
        ($_.Name -like "*$Version*" -or $_.Name -eq "SHA256SUMS.txt") -and
        $_.Name -notlike "*-source.zip" -and
        $_.Name -notlike "*-source.tar.gz"
    }
}
# Deduplicate by full path (e.g. SHA256SUMS.txt was only added once)
$releaseFiles = $releaseFiles | Sort-Object -Property FullName -Unique

# Create release (use --notes-file to avoid escaping issues with CHANGELOG content)
Write-Info "Creating GitHub release..."
$notesFile = Join-Path $env:TEMP "zpl2pdf-release-notes-$Version.md"
# UTF-8 without BOM so GitHub API displays emojis correctly (no "ðŸ³" mojibake)
$utf8NoBom = [System.Text.UTF8Encoding]::new($false)
[System.IO.File]::WriteAllText($notesFile, $releaseNotes, $utf8NoBom)

if ($releaseExists) {
    Write-Info "Release exists; updating notes only..."
    & gh release edit $tagName --repo "$RepoOwner/$RepoName" --notes-file $notesFile
    $exitCode = $LASTEXITCODE
} else {
    $ghArgs = @(
        "release", "create", $tagName,
        "--repo", "$RepoOwner/$RepoName",
        "--title", "ZPL2PDF v$Version",
        "--notes-file", $notesFile
    )
    foreach ($f in $releaseFiles) {
        $ghArgs += $f.FullName
    }
    & gh @ghArgs
    $exitCode = $LASTEXITCODE
}

# Remove temp notes file
if (Test-Path $notesFile) { Remove-Item $notesFile -Force }

if ($exitCode -eq 0) {
    if ($releaseExists) {
        Write-Success "Release notes updated!"
    } else {
        Write-Success "GitHub release created!"
    }
    Write-Info "URL: https://github.com/$RepoOwner/$RepoName/releases/tag/$tagName"
    Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 13 -Data @{
        ReleaseUrl = "https://github.com/$RepoOwner/$RepoName/releases/tag/$tagName"
        TagName = $tagName
    }
} else {
    if ($releaseExists) {
        Write-Error "Failed to update release notes (exit code: $exitCode)."
        Mark-StepFailed -Version $Version -ProjectRoot $ProjectRoot -StepNumber 13 -ErrorMessage "gh release edit failed with exit code $exitCode"
    } else {
        Write-Error "Failed to create release (exit code: $exitCode). Check 'gh release create --help' and ensure tag v$Version exists and release/ has the files."
        Mark-StepFailed -Version $Version -ProjectRoot $ProjectRoot -StepNumber 13 -ErrorMessage "gh release create failed with exit code $exitCode"
    }
    exit 1
}

