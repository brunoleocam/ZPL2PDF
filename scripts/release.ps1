# ZPL2PDF Release Script
# This script automates the release process for ZPL2PDF

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$PreRelease = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$DryRun = $false
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Colors for output
$Red = "`e[31m"
$Green = "`e[32m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = $Reset)
    Write-Host "${Color}${Message}${Reset}"
}

function Write-Step {
    param([string]$Message)
    Write-ColorOutput "`n🔧 $Message" $Blue
}

function Write-Success {
    param([string]$Message)
    Write-ColorOutput "✅ $Message" $Green
}

function Write-Warning {
    param([string]$Message)
    Write-ColorOutput "⚠️  $Message" $Yellow
}

function Write-Error {
    param([string]$Message)
    Write-ColorOutput "❌ $Message" $Red
}

# Validate version format
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "Invalid version format. Use semantic versioning (e.g., 2.0.0)"
    exit 1
}

Write-ColorOutput "🚀 ZPL2PDF Release Script v2.0.1" $Blue
Write-ColorOutput "=================================" $Blue
Write-ColorOutput "Version: $Version" $Yellow
Write-ColorOutput "Pre-release: $PreRelease" $Yellow
Write-ColorOutput "Dry run: $DryRun" $Yellow

# Check prerequisites
Write-Step "Checking prerequisites..."

# Check if git is available
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Error "Git is not installed or not in PATH"
    exit 1
}

# Check if dotnet is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET SDK is not installed or not in PATH"
    exit 1
}

# Check if we're in a git repository
if (-not (Test-Path ".git")) {
    Write-Error "Not in a git repository"
    exit 1
}

# Check if working directory is clean
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Error "Working directory is not clean. Please commit or stash changes first."
    Write-ColorOutput "Uncommitted changes:" $Yellow
    Write-ColorOutput $gitStatus $Yellow
    exit 1
}

Write-Success "Prerequisites check passed"

# Update version in project files
Write-Step "Updating version in project files..."

# Update ZPL2PDF.csproj
$csprojPath = "ZPL2PDF.csproj"
if (Test-Path $csprojPath) {
    $csprojContent = Get-Content $csprojPath -Raw
    $csprojContent = $csprojContent -replace '<Version>.*</Version>', "<Version>$Version</Version>"
    Set-Content $csprojPath $csprojContent
    Write-Success "Updated $csprojPath"
}

# Update ApplicationConstants.cs
$constantsPath = "src/Shared/Constants/ApplicationConstants.cs"
if (Test-Path $constantsPath) {
    $constantsContent = Get-Content $constantsPath -Raw
    $constantsContent = $constantsContent -replace 'APPLICATION_VERSION = ".*"', "APPLICATION_VERSION = `"$Version`""
    Set-Content $constantsPath $constantsContent
    Write-Success "Updated $constantsPath"
}

# Update winget manifest
$wingetPath = "winget-manifest.yaml"
if (Test-Path $wingetPath) {
    $wingetContent = Get-Content $wingetPath -Raw
    $wingetContent = $wingetContent -replace 'PackageVersion: .*', "PackageVersion: $Version"
    Set-Content $wingetPath $wingetContent
    Write-Success "Updated $wingetPath"
}

# Update RPM spec
$rpmPath = "rpm/zpl2pdf.spec"
if (Test-Path $rpmPath) {
    $rpmContent = Get-Content $rpmPath -Raw
    $rpmContent = $rpmContent -replace 'Version:\s+.*', "Version:        $Version"
    Set-Content $rpmPath $rpmContent
    Write-Success "Updated $rpmPath"
}

# Update Debian control
$debianPath = "debian/control"
if (Test-Path $debianPath) {
    $debianContent = Get-Content $debianPath -Raw
    $debianContent = $debianContent -replace 'Version: .*', "Version: $Version"
    Set-Content $debianPath $debianContent
    Write-Success "Updated $debianPath"
}

# Update CHANGELOG.md
Write-Step "Updating CHANGELOG.md..."
$changelogPath = "CHANGELOG.md"
if (Test-Path $changelogPath) {
    $changelogContent = Get-Content $changelogPath -Raw
    
    # Update version in changelog
    $changelogContent = $changelogContent -replace '## \[Unreleased\]', "## [$Version]"
    $changelogContent = $changelogContent -replace '## \[Unreleased\] - \d{4}-\d{2}-\d{2}', "## [$Version] - $(Get-Date -Format 'yyyy-MM-dd')"
    Set-Content $changelogPath $changelogContent
    Write-Success "Updated $changelogPath"
}

# Build and test
Write-Step "Building and testing..."

# Restore dependencies
Write-ColorOutput "Restoring dependencies..." $Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to restore dependencies"
    exit 1
}

# Build solution
Write-ColorOutput "Building solution..." $Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to build solution"
    exit 1
}

# Run tests
Write-ColorOutput "Running tests..." $Yellow
dotnet test --configuration Release --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed"
    exit 1
}

Write-Success "Build and tests completed successfully"

# Create release builds using build-all-platforms script
Write-Step "Creating release builds..."

$buildDir = "build/publish"
$buildScript = "scripts\build-all-platforms.ps1"

if (Test-Path $buildScript) {
    & $buildScript -OutputDir $buildDir
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build script failed"
        exit 1
    }
    
    Write-Success "All platforms built successfully"
} else {
    Write-Error "Build script not found: $buildScript"
    exit 1
}

# Build Windows installer
Write-Step "Building Windows installer..."
$installerScript = "scripts\build-installer.ps1"
if (Test-Path $installerScript) {
    & $installerScript -Version $Version
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Windows installer built successfully"
        
        # Copy installer to release directory
        $installerFiles = Get-ChildItem "install\output" -Filter "*.exe" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
        if ($installerFiles.Count -gt 0) {
            $installerFile = $installerFiles[0]
            Copy-Item $installerFile.FullName $buildDir
            Write-ColorOutput "Installer copied: $($installerFile.Name)" $Yellow
        }
    } else {
        Write-Warning "Windows installer build failed (Inno Setup may not be installed)"
    }
} else {
    Write-Warning "Installer build script not found: $installerScript"
}

# Checksums are already created by build-all-platforms script
Write-Success "Checksums available at: $buildDir/SHA256SUMS.txt"

# Create Git tag
if ($DryRun -eq $false) {
    Write-Step "Creating Git tag..."
    
    $tagName = "v$Version"
    if ($PreRelease -eq $true) {
        $tagName = "$tagName-rc"
    }
    
    git add .
    git commit -m "chore: release $Version"
    git tag -a $tagName -m "Release $Version"
    
    Write-Success "Created tag $tagName"
    
    # Push changes
    Write-Step "Pushing changes..."
    git push origin main
    git push origin $tagName
    
    Write-Success "Pushed changes and tag"
} else {
    Write-Warning "Dry run mode - skipping Git operations"
}

# Create GitHub release
if ($DryRun -eq $false) {
    Write-Step "Creating GitHub release..."
    Write-Warning "GitHub release creation requires manual intervention or GitHub CLI (gh)"
    Write-ColorOutput "To create release manually:" $Yellow
    Write-ColorOutput "1. Go to: https://github.com/YOUR-USERNAME/ZPL2PDF/releases/new" $Yellow
    Write-ColorOutput "2. Choose tag: v$Version" $Yellow
    Write-ColorOutput "3. Upload files from: $buildDir" $Yellow
}

# Summary
Write-ColorOutput "`n🎉 Release process completed!" $Green
Write-ColorOutput "================================" $Green
Write-ColorOutput "Version: $Version" $Yellow
Write-ColorOutput "Build directory: $buildDir" $Yellow
Write-ColorOutput "Tag: v$Version" $Yellow

if ($DryRun -eq $false) {
    Write-ColorOutput "`nNext steps:" $Blue
    Write-ColorOutput "1. Verify the build artifacts in $buildDir" $Yellow
    Write-ColorOutput "2. Create GitHub release with the prepared notes" $Yellow
    Write-ColorOutput "3. Update package repositories (winget, apt, etc.)" $Yellow
    Write-ColorOutput "4. Announce the release" $Yellow
} else {
    Write-ColorOutput "`nThis was a dry run. No changes were pushed to Git." $Blue
}

Write-ColorOutput "`nThank you for using ZPL2PDF! 🚀" $Green
