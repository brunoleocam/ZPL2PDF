# ZPL2PDF Installer Build Script
# This script builds the Windows installer using Inno Setup

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [string]$InnoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    [string]$Configuration = "Release",
    [switch]$Help = $false
)

# Colors for output
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

# Function to write colored output
function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

# Function to show help
function Show-Help {
    Write-ColorOutput "ZPL2PDF Installer Build Script" $InfoColor
    Write-ColorOutput "=================================" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Usage: .\scripts\build-installer.ps1 [options]" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Options:" $InfoColor
    Write-ColorOutput "  -InnoSetupPath <path>    Path to Inno Setup compiler [Default: C:\Program Files (x86)\Inno Setup 6\ISCC.exe]" $InfoColor
    Write-ColorOutput "  -Version <version>       Version to build [Default: 2.0.0]" $InfoColor
    Write-ColorOutput "  -Configuration <config>  Build configuration [Default: Release]" $InfoColor
    Write-ColorOutput "  -Help                    Show this help" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Examples:" $InfoColor
    Write-ColorOutput "  .\scripts\build-installer.ps1" $InfoColor
    Write-ColorOutput "  .\scripts\build-installer.ps1 -Version 2.1.0" $InfoColor
    Write-ColorOutput "  .\scripts\build-installer.ps1 -InnoSetupPath 'C:\Inno Setup 6\ISCC.exe'" $InfoColor
    Write-ColorOutput ""
}

# Check if help was requested
if ($Help) {
    Show-Help
    exit 0
}

Write-ColorOutput "ZPL2PDF Installer Build Script" $InfoColor
Write-ColorOutput "===============================" $InfoColor
Write-ColorOutput "Version: $Version" $InfoColor
Write-ColorOutput "Configuration: $Configuration" $InfoColor
Write-ColorOutput "Inno Setup Path: $InnoSetupPath" $InfoColor
Write-ColorOutput ""

# Check if Inno Setup is installed
Write-ColorOutput "Checking Inno Setup installation..." $InfoColor
if (-not (Test-Path $InnoSetupPath)) {
    Write-ColorOutput "Inno Setup not found at: $InnoSetupPath" $ErrorColor
    Write-ColorOutput "Please install Inno Setup 6 or specify the correct path with -InnoSetupPath" $ErrorColor
    Write-ColorOutput "Download from: https://jrsoftware.org/isinfo.php" $InfoColor
    exit 1
}
Write-ColorOutput "Inno Setup found!" $SuccessColor

# Check if project is built
Write-ColorOutput "Checking if project is built..." $InfoColor
$PublishPath = "bin\$Configuration\net9.0\win-x64\publish"
if (-not (Test-Path $PublishPath)) {
    Write-ColorOutput "Project not built! Building now..." $WarningColor
    dotnet publish ZPL2PDF.csproj --configuration $Configuration --runtime win-x64 --self-contained true --output $PublishPath
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "Build failed!" $ErrorColor
        exit 1
    }
    Write-ColorOutput "Build completed!" $SuccessColor
} else {
    Write-ColorOutput "Project already built!" $SuccessColor
}

# Check if executable exists
$ExePath = "$PublishPath\ZPL2PDF.exe"
if (-not (Test-Path $ExePath)) {
    Write-ColorOutput "Executable not found: $ExePath" $ErrorColor
    exit 1
}
Write-ColorOutput "Executable found: $ExePath" $SuccessColor

# Check if Inno Setup script exists
$InnoScriptPath = "installer\ZPL2PDF-Setup.iss"
if (-not (Test-Path $InnoScriptPath)) {
    Write-ColorOutput "Inno Setup script not found: $InnoScriptPath" $ErrorColor
    exit 1
}
Write-ColorOutput "Inno Setup script found: $InnoScriptPath" $SuccessColor

# Create output directory
$OutputDir = "installer\Output"
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-ColorOutput "Created output directory: $OutputDir" $InfoColor
}

# Build installer
Write-ColorOutput "Building installer..." $InfoColor
$BuildCommand = "& `"$InnoSetupPath`" `"$InnoScriptPath`" /DMyAppVersion=$Version /DConfiguration=$Configuration /O`"$OutputDir`""
Write-ColorOutput "Command: $BuildCommand" $InfoColor

$result = Invoke-Expression $BuildCommand
$exitCode = $LASTEXITCODE

if ($exitCode -eq 0) {
    Write-ColorOutput "Installer built successfully!" $SuccessColor
    
    # Find the generated installer
    $InstallerFiles = Get-ChildItem $OutputDir -Filter "*.exe" | Sort-Object LastWriteTime -Descending
    if ($InstallerFiles.Count -gt 0) {
        $InstallerFile = $InstallerFiles[0]
        $InstallerSize = [math]::Round($InstallerFile.Length / 1MB, 2)
        Write-ColorOutput "Installer created: $($InstallerFile.Name) ($InstallerSize MB)" $SuccessColor
        Write-ColorOutput "Location: $($InstallerFile.FullName)" $InfoColor
        
        # Copy to Assets directory
        $AssetsDir = "Assets"
        if (-not (Test-Path $AssetsDir)) {
            New-Item -ItemType Directory -Path $AssetsDir -Force | Out-Null
        }
        $AssetsInstallerPath = Join-Path $AssetsDir "ZPL2PDF-Setup-$Version.exe"
        Copy-Item $InstallerFile.FullName -Destination $AssetsInstallerPath -Force
        Write-ColorOutput "Copied to Assets: $AssetsInstallerPath" $SuccessColor
        
        # Append installer hash to Assets\SHA256SUMS.txt (same as other releases)
        Write-ColorOutput "Calculating SHA256..." $InfoColor
        $hash = (Get-FileHash $InstallerFile.FullName -Algorithm SHA256).Hash
        Write-ColorOutput "SHA256: $hash" $SuccessColor
        $sha256SumsPath = Join-Path $AssetsDir "SHA256SUMS.txt"
        $installerLine = "$hash  ZPL2PDF-Setup-$Version.exe"
        Add-Content -Path $sha256SumsPath -Value $installerLine -Encoding ASCII
        Write-ColorOutput "Added to $sha256SumsPath" $SuccessColor
        
        # Test installer (silent run; use Start-Process so /SILENT etc. are not parsed by PowerShell)
        Write-ColorOutput "Testing installer..." $InfoColor
        $psi = Start-Process -FilePath $InstallerFile.FullName -ArgumentList "/SILENT", "/NORESTART", "/SUPPRESSMSGBOXES", "/LOG" -Wait -PassThru
        if ($psi.ExitCode -eq 0) {
            Write-ColorOutput "Installer test passed!" $SuccessColor
        } else {
            Write-ColorOutput "Installer test failed! (ExitCode: $($psi.ExitCode))" $WarningColor
        }
    } else {
        Write-ColorOutput "Installer file not found in output directory!" $WarningColor
    }
} else {
    Write-ColorOutput "Installer build failed with exit code: $exitCode" $ErrorColor
    Write-ColorOutput "Check the Inno Setup log for details." $ErrorColor
    exit 1
}

Write-ColorOutput ""
Write-ColorOutput "Build completed!" $SuccessColor
Write-ColorOutput "Installer is ready for distribution." $InfoColor
