# ============================================================================
# Script 02: Update Resources Messages
# ============================================================================
# Updates version in all Resources/Messages.*.resx files
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
    Write-Host "[02] $Message" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "    $Message" -ForegroundColor White
}

Write-Step "Updating Resources Messages for version $Version..."

Set-Location $ProjectRoot

$resourcesDir = Join-Path $ProjectRoot "Resources"
if (-not (Test-Path $resourcesDir)) {
    Write-Error "Resources directory not found: $resourcesDir"
    exit 1
}

# List of resource files
$resourceFiles = @(
    "Messages.en.resx",
    "Messages.pt-BR.resx",
    "Messages.es.resx",
    "Messages.fr.resx",
    "Messages.de.resx",
    "Messages.it.resx",
    "Messages.ja.resx",
    "Messages.zh.resx"
)

foreach ($resourceFile in $resourceFiles) {
    $fullPath = Join-Path $resourcesDir $resourceFile
    if (Test-Path $fullPath) {
        # .resx files normally carry no version; only release markers are touched.
        if (Update-ReleaseReferencesInFile -Path $fullPath -Version $Version -DryRun:$DryRun) {
            if ($DryRun) { Write-Info "[DRY RUN] Would update: $resourceFile" } else { Write-Success "Updated: $resourceFile" }
        } else {
            Write-Info "No changes needed: $resourceFile"
        }
    } else {
        Write-Info "File not found (may be optional): $resourceFile"
    }
}

Write-Success "Resources Messages updated!"

