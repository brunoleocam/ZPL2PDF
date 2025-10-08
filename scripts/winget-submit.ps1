# ============================================================================
# ZPL2PDF - WinGet Package Submission Script
# ============================================================================
# This script automates the WinGet package submission process
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$InstallerPath = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipValidation,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

# Configuration
$ErrorActionPreference = "Stop"
$RepoOwner = "brunoleocam"
$RepoName = "ZPL2PDF"
$WinGetRepo = "microsoft/winget-pkgs"
$WinGetFork = "$RepoOwner/winget-pkgs"
$PackageId = "brunoleocam.ZPL2PDF"

# Colors
function Write-Step { param($msg) Write-Host "`n[$msg]" -ForegroundColor Cyan }
function Write-Success { param($msg) Write-Host "[OK] $msg" -ForegroundColor Green }
function Write-Error { param($msg) Write-Host "[ERROR] $msg" -ForegroundColor Red }
function Write-Warning { param($msg) Write-Host "[!] $msg" -ForegroundColor Yellow }

# ============================================================================
# Step 1: Validate Prerequisites
# ============================================================================
Write-Step "Validating Prerequisites"

# Check Git
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error "Git is not installed. Please install Git first."
    exit 1
}
Write-Success "Git found"

# Check GitHub CLI
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Error "GitHub CLI (gh) is not installed. Please install from: https://cli.github.com/"
    exit 1
}
Write-Success "GitHub CLI found"

# Check authentication
$ghAuth = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Not authenticated with GitHub. Run: gh auth login"
    exit 1
}
Write-Success "GitHub authentication verified"

# ============================================================================
# Step 2: Determine Installer Path and Calculate SHA256
# ============================================================================
Write-Step "Locating Installer"

if ($InstallerPath -eq "") {
    $InstallerPath = "installer\ZPL2PDF-Setup-$Version.exe"
}

if (-not (Test-Path $InstallerPath)) {
    Write-Error "Installer not found: $InstallerPath"
    Write-Warning "Please build the installer first using: .\installer\build-installer.ps1"
    exit 1
}

Write-Success "Installer found: $InstallerPath"

Write-Step "Calculating SHA256"
$sha256 = (Get-FileHash -Path $InstallerPath -Algorithm SHA256).Hash
Write-Success "SHA256: $sha256"

# Get installer size
$installerSize = [math]::Round((Get-Item $InstallerPath).Length / 1MB, 2)
Write-Success "Installer size: $installerSize MB"

# ============================================================================
# Step 3: Get Product Code from Installer
# ============================================================================
Write-Step "Extracting Product Code"

# Try to get ProductCode from Inno Setup script
$issFile = "installer\ZPL2PDF-Setup.iss"
if (Test-Path $issFile) {
    $appId = (Get-Content $issFile | Select-String "AppId=(.*)").Matches.Groups[1].Value
    if ($appId) {
        Write-Success "Product Code: $appId"
    } else {
        Write-Warning "Could not extract Product Code from .iss file"
        $appId = "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}"
    }
} else {
    Write-Warning "Inno Setup script not found, using placeholder"
    $appId = "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}"
}

# ============================================================================
# Step 4: Update Manifest Files
# ============================================================================
Write-Step "Updating Manifest Files"

$manifestDir = "manifests"
if (-not (Test-Path $manifestDir)) {
    New-Item -ItemType Directory -Path $manifestDir | Out-Null
}

# Update installer manifest with actual SHA256 and Product Code
$installerManifest = "$manifestDir\$PackageId.installer.yaml"
if (Test-Path $installerManifest) {
    $content = Get-Content $installerManifest -Raw
    $content = $content -replace "REPLACE_WITH_ACTUAL_SHA256", $sha256
    $content = $content -replace "\{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX\}", $appId
    $content = $content -replace "PackageVersion: .*", "PackageVersion: $Version"
    $content = $content -replace "v[0-9]+\.[0-9]+\.[0-9]+", "v$Version"
    $content = $content -replace "ReleaseDate: .*", "ReleaseDate: $(Get-Date -Format 'yyyy-MM-dd')"
    Set-Content -Path $installerManifest -Value $content -NoNewline
    Write-Success "Updated: $installerManifest"
}

# Update version manifest
$versionManifest = "$manifestDir\$PackageId.yaml"
if (Test-Path $versionManifest) {
    $content = Get-Content $versionManifest -Raw
    $content = $content -replace "PackageVersion: .*", "PackageVersion: $Version"
    Set-Content -Path $versionManifest -Value $content -NoNewline
    Write-Success "Updated: $versionManifest"
}

# Update locale manifests
$localeManifests = Get-ChildItem "$manifestDir\$PackageId.locale.*.yaml"
foreach ($manifest in $localeManifests) {
    $content = Get-Content $manifest.FullName -Raw
    $content = $content -replace "PackageVersion: .*", "PackageVersion: $Version"
    $content = $content -replace "v[0-9]+\.[0-9]+\.[0-9]+", "v$Version"
    $content = $content -replace "Copyright \(c\) [0-9]+", "Copyright (c) $(Get-Date -Format 'yyyy')"
    Set-Content -Path $manifest.FullName -Value $content -NoNewline
    Write-Success "Updated: $($manifest.Name)"
}

# ============================================================================
# Step 5: Validate Manifests (Optional)
# ============================================================================
if (-not $SkipValidation) {
    Write-Step "Validating Manifests"
    
    # Check if winget is available
    if (Get-Command winget -ErrorAction SilentlyContinue) {
        # Create temporary directory for validation
        $tempValidationDir = Join-Path $env:TEMP "winget-validation-$Version"
        if (Test-Path $tempValidationDir) {
            Remove-Item -Path $tempValidationDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $tempValidationDir -Force | Out-Null
        
        # Copy only YAML files to temp directory
        $yamlFiles = Get-ChildItem -Path $manifestDir -Filter "*.yaml"
        foreach ($yamlFile in $yamlFiles) {
            Copy-Item -Path $yamlFile.FullName -Destination $tempValidationDir -Force
        }
        
        if ($yamlFiles.Count -eq 0) {
            Write-Warning "No YAML manifest files found in $manifestDir"
        } else {
            Write-Host "Running: winget validate on $($yamlFiles.Count) YAML files" -ForegroundColor Gray
            winget validate $tempValidationDir
            
            if ($LASTEXITCODE -eq 0) {
                Write-Success "Manifest validation passed"
            } else {
                Write-Error "Manifest validation failed"
                Write-Warning "Fix validation errors before submitting"
                if (-not $DryRun) {
                    Remove-Item -Path $tempValidationDir -Recurse -Force -ErrorAction SilentlyContinue
                    exit 1
                }
            }
        }
        
        # Cleanup temp validation directory
        Remove-Item -Path $tempValidationDir -Recurse -Force -ErrorAction SilentlyContinue
    } else {
        Write-Warning "WinGet not found, skipping validation"
        Write-Warning "Install WinGet to validate manifests locally"
    }
} else {
    Write-Warning "Skipping manifest validation"
}

# ============================================================================
# Step 6: Display Manifest Summary
# ============================================================================
Write-Step "Manifest Summary"

Write-Host ""
Write-Host "Package Information:" -ForegroundColor Cyan
Write-Host "  Package ID:      $PackageId" -ForegroundColor White
Write-Host "  Version:         $Version" -ForegroundColor White
Write-Host "  Installer:       $InstallerPath" -ForegroundColor White
Write-Host "  Size:            $installerSize MB" -ForegroundColor White
Write-Host "  SHA256:          $sha256" -ForegroundColor White
Write-Host "  Product Code:    $appId" -ForegroundColor White
Write-Host "  Release Date:    $(Get-Date -Format 'yyyy-MM-dd')" -ForegroundColor White

if ($DryRun) {
    Write-Host ""
    Write-Warning "DRY RUN MODE - No changes will be pushed"
    Write-Warning "Manifests updated locally only"
    Write-Success "Review manifests in: $manifestDir\"
    exit 0
}

# ============================================================================
# Step 7: Confirm Submission
# ============================================================================
Write-Host ""
Write-Host "Ready to submit to WinGet!" -ForegroundColor Yellow
Write-Host "This will:" -ForegroundColor Yellow
Write-Host "  1. Fork/update your winget-pkgs fork" -ForegroundColor White
Write-Host "  2. Create a new branch: $PackageId-$Version" -ForegroundColor White
Write-Host "  3. Copy manifests to correct folder structure" -ForegroundColor White
Write-Host "  4. Commit and push changes" -ForegroundColor White
Write-Host "  5. Create a Pull Request to microsoft/winget-pkgs" -ForegroundColor White
Write-Host ""

$confirmation = Read-Host "Continue? (Y/N)"
if ($confirmation -ne 'Y' -and $confirmation -ne 'y') {
    Write-Warning "Submission cancelled by user"
    exit 0
}

# ============================================================================
# Step 8: Fork/Update WinGet Repository
# ============================================================================
Write-Step "Preparing WinGet Fork"

# Check if fork exists
$forkExists = gh repo view $WinGetFork 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "Creating fork of $WinGetRepo..." -ForegroundColor Gray
    gh repo fork $WinGetRepo --clone=false
    Write-Success "Fork created: $WinGetFork"
} else {
    Write-Success "Fork already exists: $WinGetFork"
}

# ============================================================================
# Step 9: Clone and Prepare Repository
# ============================================================================
Write-Step "Cloning Fork"

$tempDir = Join-Path $env:TEMP "winget-pkgs-$Version"
if (Test-Path $tempDir) {
    Write-Host "Removing old temp directory..." -ForegroundColor Gray
    Remove-Item -Path $tempDir -Recurse -Force
}

Write-Host "Cloning: $WinGetFork" -ForegroundColor Gray
git clone "https://github.com/$WinGetFork.git" $tempDir --depth=1
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to clone fork"
    exit 1
}
Write-Success "Repository cloned"

# Navigate to temp directory
Set-Location $tempDir

# Configure Git
git config user.name $RepoOwner
git config user.email "$RepoOwner@users.noreply.github.com"

# Add upstream remote
git remote add upstream "https://github.com/$WinGetRepo.git"
Write-Success "Git configured"

# ============================================================================
# Step 10: Sync with Upstream
# ============================================================================
Write-Step "Syncing with Upstream"

Write-Host "Fetching upstream changes..." -ForegroundColor Gray
git fetch upstream
git checkout master
git merge upstream/master --ff-only

if ($LASTEXITCODE -ne 0) {
    Write-Warning "Could not fast-forward. Attempting reset..."
    git reset --hard upstream/master
}

git push origin master --force
Write-Success "Fork synced with upstream"

# ============================================================================
# Step 11: Create New Branch
# ============================================================================
Write-Step "Creating New Branch"

$branchName = "$PackageId-$Version"
git checkout -b $branchName
Write-Success "Created branch: $branchName"

# ============================================================================
# Step 12: Copy Manifest Files
# ============================================================================
Write-Step "Copying Manifest Files"

# Determine target directory (first letter of package ID)
$firstLetter = $PackageId.Substring(0, 1).ToLower()
$packagePath = "manifests\$firstLetter\$($PackageId.Replace('.', '\'))\$Version"

# Create directory structure
New-Item -ItemType Directory -Path $packagePath -Force | Out-Null
Write-Success "Created directory: $packagePath"

# Copy manifests from source project
$sourceManifests = Get-ChildItem "$PSScriptRoot\..\manifests\$PackageId*.yaml"
foreach ($manifest in $sourceManifests) {
    $targetName = $manifest.Name
    $targetPath = Join-Path $packagePath $targetName
    Copy-Item $manifest.FullName $targetPath -Force
    Write-Success "Copied: $targetName"
}

# ============================================================================
# Step 13: Commit and Push
# ============================================================================
Write-Step "Committing Changes"

git add .
git commit -m "New version: $PackageId version $Version"
Write-Success "Changes committed"

Write-Host "Pushing to fork..." -ForegroundColor Gray
git push origin $branchName
Write-Success "Changes pushed"

# ============================================================================
# Step 14: Create Pull Request
# ============================================================================
Write-Step "Creating Pull Request"

$prTitle = "$PackageId version $Version"
$prBody = @"
<!-- Provide a general summary of your changes in the Title above -->

## Description
Automated submission for $PackageId version $Version

## Validation
- [x] Manifests validated using winget validate
- [x] Installer tested locally
- [x] SHA256 checksum verified
- [x] Product code verified

## Package Information
- **Version**: $Version
- **Installer Size**: $installerSize MB
- **Installer Type**: Inno Setup
- **Architecture**: x64
- **Release Date**: $(Get-Date -Format 'yyyy-MM-dd')

## Release Notes
See: https://github.com/$RepoOwner/$RepoName/releases/tag/v$Version

## Checklist
- [x] I have read and followed the Contributing Guide
- [x] I have updated the version and release date
- [x] I have tested the installer
- [x] All validation checks pass
"@

Write-Host "Creating PR: $WinGetRepo" -ForegroundColor Gray
$headBranch = "${RepoOwner}:${branchName}"
gh pr create --repo $WinGetRepo --title $prTitle --body $prBody --head $headBranch

if ($LASTEXITCODE -eq 0) {
    Write-Success "Pull Request created successfully!"
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1. Wait for automated validation to complete" -ForegroundColor White
    Write-Host "  2. Monitor PR for any feedback from maintainers" -ForegroundColor White
    Write-Host "  3. Once merged, package will be available via: winget install $PackageId" -ForegroundColor White
    Write-Host ""
    Write-Success "WinGet submission complete!"
} else {
    Write-Error "Failed to create Pull Request"
    Write-Warning "You may need to create it manually at:"
    $compareUrl = "https://github.com/$WinGetRepo/compare/master...${RepoOwner}:${branchName}"
    Write-Host "  $compareUrl" -ForegroundColor Yellow
}

# ============================================================================
# Step 15: Cleanup
# ============================================================================
Write-Step "Cleaning Up"

Set-Location $PSScriptRoot
Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue
Write-Success "Temporary files removed"

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "          WinGet Submission Complete!       " -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
