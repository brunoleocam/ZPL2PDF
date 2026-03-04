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
$ProjectRoot = Split-Path -Parent $ScriptDir

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
        $content = Get-Content $fullPath -Raw -Encoding UTF8
        $originalContent = $content
        
        # Update version in comments or metadata if present
        # Note: .resx files usually don't have explicit version,
        # but we can update if there are references
        $content = $content -replace 'v\d+\.\d+\.\d+', "v$Version"
        $content = $content -replace 'version \d+\.\d+\.\d+', "version $Version"
        
        if ($content -ne $originalContent) {
            if (-not $DryRun) {
                Set-Content $fullPath $content -NoNewline -Encoding UTF8
                Write-Success "Updated: $resourceFile"
            } else {
                Write-Info "[DRY RUN] Would update: $resourceFile"
            }
        } else {
            Write-Info "No changes needed: $resourceFile"
        }
    } else {
        Write-Info "File not found (may be optional): $resourceFile"
    }
}

Write-Success "Resources Messages updated!"

