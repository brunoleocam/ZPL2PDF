# ============================================================================
# Script 10: Update WinGet Manifests
# ============================================================================
# Updates version and SHA256 in WinGet manifests
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
    Write-Host "[10] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Updating WinGet manifests for version $Version..."

Set-Location $ProjectRoot

$manifestDir = Join-Path $ProjectRoot "manifests"
if (-not (Test-Path $manifestDir)) {
    Write-Error "Manifests directory not found: $manifestDir"
    exit 1
}

# Calculate installer SHA256 (check Assets first, then installer/Output as fallback)
$installerPath = Join-Path $ProjectRoot "Assets\ZPL2PDF-Setup-$Version.exe"
$sha256 = ""

if (-not (Test-Path $installerPath)) {
    $installerPath = Join-Path $ProjectRoot "installer\Output\ZPL2PDF-Setup-$Version.exe"
}

if (Test-Path $installerPath) {
    $sha256 = (Get-FileHash $installerPath -Algorithm SHA256).Hash
    Write-Info "Installer SHA256: $sha256"
} else {
    Write-Info "Installer not found, SHA256 will not be updated"
}

# Update all manifests
$manifestFiles = Get-ChildItem $manifestDir -Filter "*.yaml"

foreach ($file in $manifestFiles) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    # Update version
    $content = $content -replace 'PackageVersion: \d+\.\d+\.\d+', "PackageVersion: $Version"
    $content = $content -replace 'ReleaseDate: \d{4}-\d{2}-\d{2}', "ReleaseDate: $(Get-Date -Format 'yyyy-MM-dd')"
    
    # Update SHA256 and URL in installer file
    if ($sha256 -and $file.Name -like "*installer*") {
        $content = $content -replace 'InstallerSha256: [A-F0-9]+', "InstallerSha256: $sha256"
        $content = $content -replace 'ZPL2PDF-Setup-\d+\.\d+\.\d+\.exe', "ZPL2PDF-Setup-$Version.exe"
        $content = $content -replace '/v\d+\.\d+\.\d+/', "/v$Version/"
    }
    
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

Write-Success "WinGet manifests updated!"

# Save checkpoint
Mark-StepCompleted -Version $Version -ProjectRoot $ProjectRoot -StepNumber 10

