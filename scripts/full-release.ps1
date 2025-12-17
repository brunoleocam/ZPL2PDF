# ============================================================================
# ZPL2PDF - Full Release Automation Script
# ============================================================================
# Este script automatiza TODO o processo de release do ZPL2PDF:
# 1. Atualiza versão em todos os arquivos
# 2. Gera builds para todas as plataformas (Windows, Linux, macOS)
# 3. Gera pacotes .deb e .rpm via Docker
# 4. Gera instalador Windows (Inno Setup)
# 5. Gera imagens Docker e faz push para Docker Hub e GHCR
# 6. Cria release no GitHub com todos os assets
# 7. Prepara e submete PR para WinGet
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDocker,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipWinGet,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipGitHubRelease,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubToken = ""
)

# ============================================================================
# Configuração
# ============================================================================
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$BuildDir = Join-Path $ProjectRoot "build\publish"
$RepoOwner = "brunoleocam"
$RepoName = "ZPL2PDF"
$DockerImage = "brunoleocam/zpl2pdf"
$GhcrImage = "ghcr.io/brunoleocam/zpl2pdf"

# ============================================================================
# Funções de Output
# ============================================================================
function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
}

function Write-Step {
    param([string]$Step, [string]$Message)
    Write-Host ""
    Write-Host "[$Step] $Message" -ForegroundColor Yellow
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

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "[ERRO] $Message" -ForegroundColor Red
}

function Test-Command {
    param([string]$Command)
    return [bool](Get-Command $Command -ErrorAction SilentlyContinue)
}

# ============================================================================
# Validação de Pré-requisitos
# ============================================================================
function Test-Prerequisites {
    Write-Step "1/12" "Verificando pré-requisitos..."
    
    $missing = @()
    
    if (-not (Test-Command "git")) { $missing += "git" }
    if (-not (Test-Command "dotnet")) { $missing += "dotnet" }
    if (-not $SkipDocker -and -not (Test-Command "docker")) { $missing += "docker" }
    if (-not $SkipWinGet -and -not (Test-Command "gh")) { $missing += "gh (GitHub CLI)" }
    
    if ($missing.Count -gt 0) {
        Write-ErrorMsg "Ferramentas não encontradas: $($missing -join ', ')"
        Write-Info "Instale as ferramentas faltantes ou use os parâmetros -Skip* para pular etapas"
        return $false
    }
    
    # Verificar se está no diretório correto
    if (-not (Test-Path (Join-Path $ProjectRoot "ZPL2PDF.csproj"))) {
        Write-ErrorMsg "Não foi possível encontrar ZPL2PDF.csproj. Execute o script do diretório correto."
        return $false
    }
    
    # Verificar autenticação do GitHub CLI
    if (-not $SkipWinGet -and -not $SkipGitHubRelease) {
        $ghAuth = gh auth status 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "GitHub CLI não autenticado. Execute: gh auth login"
            Write-Info "Ou use -SkipWinGet e -SkipGitHubRelease para pular essas etapas"
        }
    }
    
    # Verificar Docker login
    if (-not $SkipDocker) {
        $dockerInfo = docker info 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-ErrorMsg "Docker não está rodando. Inicie o Docker Desktop."
            return $false
        }
    }
    
    Write-Success "Todos os pré-requisitos verificados!"
    return $true
}

# ============================================================================
# Atualização de Versão
# ============================================================================
function Update-Version {
    Write-Step "2/12" "Atualizando versão para $Version..."
    
    Set-Location $ProjectRoot
    
    # ZPL2PDF.csproj
    $csprojPath = Join-Path $ProjectRoot "ZPL2PDF.csproj"
    if (Test-Path $csprojPath) {
        $content = Get-Content $csprojPath -Raw
        $content = $content -replace '<Version>.*</Version>', "<Version>$Version</Version>"
        Set-Content $csprojPath $content -NoNewline
        Write-Info "Atualizado: ZPL2PDF.csproj"
    }
    
    # ApplicationConstants.cs
    $constantsPath = Join-Path $ProjectRoot "src\Shared\Constants\ApplicationConstants.cs"
    if (Test-Path $constantsPath) {
        $content = Get-Content $constantsPath -Raw
        $content = $content -replace 'APPLICATION_VERSION = ".*"', "APPLICATION_VERSION = `"$Version`""
        Set-Content $constantsPath $content -NoNewline
        Write-Info "Atualizado: ApplicationConstants.cs"
    }
    
    # Inno Setup
    $issPath = Join-Path $ProjectRoot "installer\ZPL2PDF-Setup.iss"
    if (Test-Path $issPath) {
        $content = Get-Content $issPath -Raw
        $content = $content -replace '#define MyAppVersion ".*"', "#define MyAppVersion `"$Version`""
        Set-Content $issPath $content -NoNewline
        Write-Info "Atualizado: ZPL2PDF-Setup.iss"
    }
    
    # build-all-platforms.ps1
    $buildScript = Join-Path $ProjectRoot "scripts\build-all-platforms.ps1"
    if (Test-Path $buildScript) {
        $content = Get-Content $buildScript -Raw
        $content = $content -replace 'ZPL2PDF-v\d+\.\d+\.\d+-', "ZPL2PDF-v$Version-"
        $content = $content -replace 'v\d+\.\d+\.\d+\s*"', "v$Version`""
        Set-Content $buildScript $content -NoNewline
        Write-Info "Atualizado: build-all-platforms.ps1"
    }
    
    # build-linux-packages.ps1
    $linuxScript = Join-Path $ProjectRoot "scripts\build-linux-packages.ps1"
    if (Test-Path $linuxScript) {
        $content = Get-Content $linuxScript -Raw
        $content = $content -replace '\$Version = "\d+\.\d+\.\d+"', "`$Version = `"$Version`""
        $content = $content -replace 'ZPL2PDF-v\d+\.\d+\.\d+-', "ZPL2PDF-v$Version-"
        $content = $content -replace 'Version: \d+\.\d+\.\d+', "Version: $Version"
        Set-Content $linuxScript $content -NoNewline
        Write-Info "Atualizado: build-linux-packages.ps1"
    }
    
    # RPM spec
    $rpmPath = Join-Path $ProjectRoot "rpm\zpl2pdf.spec"
    if (Test-Path $rpmPath) {
        $content = Get-Content $rpmPath -Raw
        $content = $content -replace 'Version:\s+\d+\.\d+\.\d+', "Version: $Version"
        Set-Content $rpmPath $content -NoNewline
        Write-Info "Atualizado: zpl2pdf.spec"
    }
    
    # Debian control
    $debPath = Join-Path $ProjectRoot "debian\control"
    if (Test-Path $debPath) {
        $content = Get-Content $debPath -Raw
        $content = $content -replace 'Version: \d+\.\d+\.\d+', "Version: $Version"
        Set-Content $debPath $content -NoNewline
        Write-Info "Atualizado: debian/control"
    }
    
    Write-Success "Versão atualizada em todos os arquivos!"
}

# ============================================================================
# Build de Todas as Plataformas
# ============================================================================
function Build-AllPlatforms {
    Write-Step "3/12" "Gerando builds para todas as plataformas..."
    
    Set-Location $ProjectRoot
    
    # Limpar builds anteriores
    if (Test-Path $BuildDir) {
        Remove-Item $BuildDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $BuildDir -Force | Out-Null
    
    # Executar testes (se não pulados)
    if (-not $SkipTests) {
        Write-Info "Executando testes..."
        dotnet test --configuration Release --verbosity quiet
        if ($LASTEXITCODE -ne 0) {
            Write-ErrorMsg "Testes falharam!"
            return $false
        }
        Write-Info "Testes passaram!"
    }
    
    # Plataformas para build
    $platforms = @(
        @{ Runtime = "win-x64";     Archive = "zip" },
        @{ Runtime = "win-x86";     Archive = "zip" },
        @{ Runtime = "win-arm64";   Archive = "zip" },
        @{ Runtime = "linux-x64";   Archive = "tar.gz" },
        @{ Runtime = "linux-arm64"; Archive = "tar.gz" },
        @{ Runtime = "linux-arm";   Archive = "tar.gz" },
        @{ Runtime = "osx-x64";     Archive = "tar.gz" },
        @{ Runtime = "osx-arm64";   Archive = "tar.gz" }
    )
    
    foreach ($platform in $platforms) {
        $runtime = $platform.Runtime
        Write-Info "Building: $runtime..."
        
        $platformDir = Join-Path $BuildDir $runtime
        
        dotnet publish ZPL2PDF.csproj `
            --configuration Release `
            --runtime $runtime `
            --self-contained true `
            --output $platformDir `
            --verbosity quiet `
            -p:PublishSingleFile=true `
            -p:PublishTrimmed=false
        
        if ($LASTEXITCODE -ne 0) {
            Write-ErrorMsg "Build falhou para $runtime"
            return $false
        }
        
        # Criar arquivo compactado
        $archiveName = "ZPL2PDF-v$Version-$runtime"
        if ($platform.Archive -eq "zip") {
            Compress-Archive -Path "$platformDir\*" -DestinationPath "$BuildDir\$archiveName.zip" -Force
        } else {
            if (Test-Command "tar") {
                tar -czf "$BuildDir\$archiveName.tar.gz" -C $platformDir .
            }
        }
    }
    
    Write-Success "Builds gerados para todas as plataformas!"
    return $true
}

# ============================================================================
# Build de Pacotes Linux (.deb e .rpm)
# ============================================================================
function Build-LinuxPackages {
    Write-Step "4/12" "Gerando pacotes Linux (.deb e .rpm)..."
    
    if ($SkipDocker) {
        Write-Warning "Docker pulado - pacotes Linux não serão gerados"
        return $true
    }
    
    Set-Location $ProjectRoot
    
    $linuxScript = Join-Path $ScriptDir "build-linux-packages.ps1"
    if (Test-Path $linuxScript) {
        & $linuxScript -Version $Version
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Falha ao gerar pacotes Linux (pode ser problema de conexão Docker)"
        } else {
            Write-Success "Pacotes Linux gerados!"
        }
    } else {
        Write-Warning "Script build-linux-packages.ps1 não encontrado"
    }
    
    return $true
}

# ============================================================================
# Build do Instalador Windows
# ============================================================================
function Build-WindowsInstaller {
    Write-Step "5/12" "Gerando instalador Windows..."
    
    Set-Location $ProjectRoot
    
    # Verificar se Inno Setup está instalado
    $innoPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
    if (-not (Test-Path $innoPath)) {
        $innoPath = "C:\Program Files\Inno Setup 6\ISCC.exe"
    }
    
    if (-not (Test-Path $innoPath)) {
        Write-Warning "Inno Setup não encontrado - instalador não será gerado"
        return $true
    }
    
    $issPath = Join-Path $ProjectRoot "installer\ZPL2PDF-Setup.iss"
    if (Test-Path $issPath) {
        & $innoPath $issPath
        if ($LASTEXITCODE -eq 0) {
            # Copiar instalador para build directory
            $installerSource = Join-Path $ProjectRoot "installer\Output\ZPL2PDF-Setup-$Version.exe"
            if (Test-Path $installerSource) {
                Copy-Item $installerSource $BuildDir -Force
                Write-Success "Instalador Windows gerado!"
            }
        } else {
            Write-Warning "Falha ao gerar instalador Windows"
        }
    }
    
    return $true
}

# ============================================================================
# Gerar Checksums
# ============================================================================
function Generate-Checksums {
    Write-Step "6/12" "Gerando checksums SHA256..."
    
    $checksumFile = Join-Path $BuildDir "SHA256SUMS.txt"
    if (Test-Path $checksumFile) {
        Remove-Item $checksumFile -Force
    }
    
    Get-ChildItem $BuildDir -File | Where-Object { $_.Name -ne "SHA256SUMS.txt" -and $_.Extension -ne "" } | ForEach-Object {
        $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
        "$hash  $($_.Name)" | Add-Content $checksumFile
    }
    
    Write-Success "Checksums gerados!"
    return $true
}

# ============================================================================
# Build e Push Docker Images
# ============================================================================
function Build-DockerImages {
    Write-Step "7/12" "Gerando e publicando imagens Docker..."
    
    if ($SkipDocker) {
        Write-Warning "Docker pulado"
        return $true
    }
    
    Set-Location $ProjectRoot
    
    # Build da imagem
    Write-Info "Building Docker image..."
    docker build -t "$DockerImage`:$Version" -t "$DockerImage`:latest" .
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMsg "Falha ao buildar imagem Docker"
        return $false
    }
    
    # Tags adicionais
    docker tag "$DockerImage`:$Version" "$DockerImage`:alpine"
    
    $majorMinor = $Version -replace '\.\d+$', ''
    $major = $Version -replace '\.\d+\.\d+$', ''
    docker tag "$DockerImage`:$Version" "$DockerImage`:$majorMinor"
    docker tag "$DockerImage`:$Version" "$DockerImage`:$major"
    
    # Tags para GHCR
    docker tag "$DockerImage`:$Version" "$GhcrImage`:$Version"
    docker tag "$DockerImage`:$Version" "$GhcrImage`:latest"
    docker tag "$DockerImage`:$Version" "$GhcrImage`:alpine"
    docker tag "$DockerImage`:$Version" "$GhcrImage`:$majorMinor"
    docker tag "$DockerImage`:$Version" "$GhcrImage`:$major"
    
    if (-not $DryRun) {
        # Push para Docker Hub
        Write-Info "Pushing para Docker Hub..."
        docker push "$DockerImage`:$Version"
        docker push "$DockerImage`:latest"
        docker push "$DockerImage`:alpine"
        docker push "$DockerImage`:$majorMinor"
        docker push "$DockerImage`:$major"
        
        # Push para GHCR
        Write-Info "Pushing para GHCR..."
        docker push "$GhcrImage`:$Version"
        docker push "$GhcrImage`:latest"
        docker push "$GhcrImage`:alpine"
        docker push "$GhcrImage`:$majorMinor"
        docker push "$GhcrImage`:$major"
        
        Write-Success "Imagens Docker publicadas!"
    } else {
        Write-Warning "Dry run - imagens não foram publicadas"
    }
    
    return $true
}

# ============================================================================
# Criar Release no GitHub
# ============================================================================
function Create-GitHubRelease {
    Write-Step "8/12" "Criando release no GitHub..."
    
    if ($SkipGitHubRelease) {
        Write-Warning "GitHub Release pulado"
        return $true
    }
    
    if ($DryRun) {
        Write-Warning "Dry run - release não será criada"
        return $true
    }
    
    Set-Location $ProjectRoot
    
    # Criar tag
    git tag -a "v$Version" -m "Release v$Version" 2>$null
    git push origin "v$Version" 2>$null
    
    # Criar release notes
    $releaseNotes = @"
## ZPL2PDF v$Version

### Downloads

| Platform | File |
|----------|------|
| Windows Installer | ZPL2PDF-Setup-$Version.exe |
| Windows x64 | ZPL2PDF-v$Version-win-x64.zip |
| Windows x86 | ZPL2PDF-v$Version-win-x86.zip |
| Windows ARM64 | ZPL2PDF-v$Version-win-arm64.zip |
| Linux x64 | ZPL2PDF-v$Version-linux-x64.tar.gz |
| Linux x64 (DEB) | ZPL2PDF-v$Version-linux-amd64.deb |
| Linux x64 (RPM) | ZPL2PDF-v$Version-linux-x64-rpm.tar.gz |
| Linux ARM64 | ZPL2PDF-v$Version-linux-arm64.tar.gz |
| Linux ARM | ZPL2PDF-v$Version-linux-arm.tar.gz |
| macOS Intel | ZPL2PDF-v$Version-osx-x64.tar.gz |
| macOS Apple Silicon | ZPL2PDF-v$Version-osx-arm64.tar.gz |

### Docker

``````bash
docker pull brunoleocam/zpl2pdf:$Version
``````

**Full Changelog**: https://github.com/$RepoOwner/$RepoName/compare/v$(([version]$Version).Major).$(([version]$Version).Minor).$([math]::Max(0, ([version]$Version).Build - 1))...v$Version
"@

    # Criar release via gh CLI
    $releaseFiles = Get-ChildItem $BuildDir -File | Where-Object { $_.Extension -ne "" }
    $filesArg = ($releaseFiles | ForEach-Object { "`"$($_.FullName)`"" }) -join " "
    
    $ghCmd = "gh release create `"v$Version`" --repo `"$RepoOwner/$RepoName`" --title `"ZPL2PDF v$Version`" --notes `"$releaseNotes`" $filesArg"
    
    try {
        Invoke-Expression $ghCmd
        Write-Success "Release criada no GitHub!"
    } catch {
        Write-Warning "Falha ao criar release automaticamente"
        Write-Info "Crie manualmente em: https://github.com/$RepoOwner/$RepoName/releases/new"
    }
    
    return $true
}

# ============================================================================
# Atualizar Manifests do WinGet
# ============================================================================
function Update-WinGetManifests {
    Write-Step "9/12" "Atualizando manifests do WinGet..."
    
    Set-Location $ProjectRoot
    
    $manifestDir = Join-Path $ProjectRoot "manifests"
    $installerPath = Join-Path $BuildDir "ZPL2PDF-Setup-$Version.exe"
    
    # Calcular SHA256 do instalador
    $sha256 = ""
    if (Test-Path $installerPath) {
        $sha256 = (Get-FileHash $installerPath -Algorithm SHA256).Hash
    }
    
    # Atualizar todos os manifests
    Get-ChildItem $manifestDir -Filter "*.yaml" | ForEach-Object {
        $content = Get-Content $_.FullName -Raw
        $content = $content -replace 'PackageVersion: \d+\.\d+\.\d+', "PackageVersion: $Version"
        $content = $content -replace 'ReleaseDate: \d{4}-\d{2}-\d{2}', "ReleaseDate: $(Get-Date -Format 'yyyy-MM-dd')"
        
        if ($sha256 -and $_.Name -like "*installer*") {
            $content = $content -replace 'InstallerSha256: [A-F0-9]+', "InstallerSha256: $sha256"
            $content = $content -replace 'ZPL2PDF-Setup-\d+\.\d+\.\d+\.exe', "ZPL2PDF-Setup-$Version.exe"
            $content = $content -replace '/v\d+\.\d+\.\d+/', "/v$Version/"
        }
        
        Set-Content $_.FullName $content -NoNewline
        Write-Info "Atualizado: $($_.Name)"
    }
    
    Write-Success "Manifests do WinGet atualizados!"
    return $true
}

# ============================================================================
# Submeter PR para WinGet
# ============================================================================
function Submit-WinGetPR {
    Write-Step "10/12" "Submetendo PR para WinGet..."
    
    if ($SkipWinGet) {
        Write-Warning "WinGet pulado"
        return $true
    }
    
    if ($DryRun) {
        Write-Warning "Dry run - PR não será criado"
        return $true
    }
    
    $tempDir = Join-Path $env:TEMP "winget-pkgs-$Version"
    $branchName = "brunoleocam.ZPL2PDF-$Version"
    $manifestSource = Join-Path $ProjectRoot "manifests"
    
    try {
        # Limpar diretório temporário
        if (Test-Path $tempDir) {
            Remove-Item $tempDir -Recurse -Force
        }
        
        # Clonar fork
        Write-Info "Clonando fork do winget-pkgs..."
        gh repo clone "$RepoOwner/winget-pkgs" $tempDir -- --depth=1
        
        Set-Location $tempDir
        
        # Configurar git
        git config user.name $RepoOwner
        git config user.email "$RepoOwner@users.noreply.github.com"
        
        # Sincronizar com upstream
        git remote add upstream https://github.com/microsoft/winget-pkgs.git
        git fetch upstream master --depth=1
        git reset --hard upstream/master
        
        # Criar branch
        git checkout -b $branchName
        
        # Criar estrutura e copiar manifests
        $targetDir = Join-Path $tempDir "manifests\b\brunoleocam\ZPL2PDF\$Version"
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
        Copy-Item "$manifestSource\*.yaml" $targetDir -Force
        
        # Commit e push
        git add .
        git commit -m "New version: brunoleocam.ZPL2PDF version $Version"
        git push origin $branchName --force
        
        # Criar PR
        gh pr create --repo "microsoft/winget-pkgs" `
            --title "brunoleocam.ZPL2PDF version $Version" `
            --body "Automated submission for brunoleocam.ZPL2PDF version $Version" `
            --head "$RepoOwner`:$branchName" `
            --base master
        
        Write-Success "PR submetido para WinGet!"
        
    } catch {
        Write-Warning "Falha ao submeter PR: $_"
        Write-Info "Submeta manualmente em: https://github.com/microsoft/winget-pkgs/compare"
    } finally {
        Set-Location $ProjectRoot
        if (Test-Path $tempDir) {
            Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
    
    return $true
}

# ============================================================================
# Commit das Alterações
# ============================================================================
function Commit-Changes {
    Write-Step "11/12" "Commitando alterações..."
    
    if ($DryRun) {
        Write-Warning "Dry run - alterações não serão commitadas"
        return $true
    }
    
    Set-Location $ProjectRoot
    
    git add .
    git commit -m "chore: release v$Version" 2>$null
    git push origin HEAD 2>$null
    
    Write-Success "Alterações commitadas!"
    return $true
}

# ============================================================================
# Resumo Final
# ============================================================================
function Show-Summary {
    Write-Step "12/12" "Resumo do Release"
    
    Write-Header "Release v$Version Concluída!"
    
    Write-Host ""
    Write-Host "Arquivos gerados em: $BuildDir" -ForegroundColor Cyan
    Write-Host ""
    
    if (Test-Path $BuildDir) {
        Get-ChildItem $BuildDir -File | ForEach-Object {
            $size = [math]::Round($_.Length / 1MB, 2)
            Write-Host "  $($_.Name) ($size MB)" -ForegroundColor White
        }
    }
    
    Write-Host ""
    Write-Host "Links:" -ForegroundColor Cyan
    Write-Host "  GitHub Release: https://github.com/$RepoOwner/$RepoName/releases/tag/v$Version" -ForegroundColor White
    Write-Host "  Docker Hub: https://hub.docker.com/r/$DockerImage/tags" -ForegroundColor White
    Write-Host "  GHCR: https://github.com/$RepoOwner/$RepoName/pkgs/container/zpl2pdf" -ForegroundColor White
    Write-Host ""
}

# ============================================================================
# Main
# ============================================================================
function Main {
    Write-Header "ZPL2PDF Full Release v$Version"
    
    if ($DryRun) {
        Write-Warning "MODO DRY RUN - Nenhuma alteração será publicada"
    }
    
    # Executar etapas
    if (-not (Test-Prerequisites)) { exit 1 }
    
    Update-Version
    
    if (-not (Build-AllPlatforms)) { exit 1 }
    
    Build-LinuxPackages
    
    Build-WindowsInstaller
    
    if (-not (Generate-Checksums)) { exit 1 }
    
    if (-not (Build-DockerImages)) { exit 1 }
    
    Update-WinGetManifests
    
    Commit-Changes
    
    Create-GitHubRelease
    
    Submit-WinGetPR
    
    Show-Summary
    
    Write-Host ""
    Write-Host "Release v$Version concluída com sucesso!" -ForegroundColor Green
    Write-Host ""
}

# Executar
Main

