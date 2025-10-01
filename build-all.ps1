# ZPL2PDF - Script de Build Cross-Platform para Windows
# PowerShell Script para compilar releases para todas as plataformas

param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "bin\Release",
    [switch]$Clean = $false,
    [switch]$Test = $false,
    [switch]$Help = $false
)

# Cores para output
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

# Função para escrever mensagens coloridas
function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

# Função para mostrar ajuda
function Show-Help {
    Write-ColorOutput "ZPL2PDF - Script de Build Cross-Platform" $InfoColor
    Write-ColorOutput "=============================================" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Uso: .\build-all.ps1 [opções]" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Opções:" $InfoColor
    Write-ColorOutput "  -Configuration <config>  Configuração de build (Debug|Release) [Padrão: Release]" $InfoColor
    Write-ColorOutput "  -OutputDir <dir>         Diretório de saída [Padrão: bin\Release]" $InfoColor
    Write-ColorOutput "  -Clean                   Limpar builds anteriores" $InfoColor
    Write-ColorOutput "  -Test                    Executar testes após build" $InfoColor
    Write-ColorOutput "  -Help                    Mostrar esta ajuda" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Exemplos:" $InfoColor
    Write-ColorOutput "  .\build-all.ps1                          # Build Release para todas as plataformas" $InfoColor
    Write-ColorOutput "  .\build-all.ps1 -Configuration Debug     # Build Debug" $InfoColor
    Write-ColorOutput "  .\build-all.ps1 -Clean -Test             # Limpar e testar" $InfoColor
    Write-ColorOutput ""
}

# Função para executar comando e verificar resultado
function Invoke-CommandSafe {
    param([string]$Command, [string]$Description)
    
    Write-ColorOutput "Executando: $Description..." $InfoColor
    Write-ColorOutput "   Comando: $Command" $InfoColor
    
    $result = Invoke-Expression $Command
    $exitCode = $LASTEXITCODE
    
    if ($exitCode -eq 0) {
        Write-ColorOutput "   Sucesso: $Description concluído!" $SuccessColor
        return $true
    } else {
        Write-ColorOutput "   Erro: $Description falhou com código: $exitCode" $ErrorColor
        return $false
    }
}

# Função para criar diretório se não existir
function Ensure-Directory {
    param([string]$Path)
    if (!(Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
        Write-ColorOutput "   Diretório criado: $Path" $InfoColor
    }
}

# Função para copiar arquivos de configuração
function Copy-ConfigFiles {
    param([string]$TargetDir)
    
    $configFiles = @(
        "zpl2pdf.json",
        "README.md",
        "LICENSE"
    )
    
    foreach ($file in $configFiles) {
        if (Test-Path $file) {
            Copy-Item $file $TargetDir -Force
            Write-ColorOutput "   Copiado: $file" $InfoColor
        }
    }
}

# Função para criar arquivo de informações da versão
function New-VersionInfo {
    param([string]$TargetDir, [string]$Platform)
    
    $versionInfo = @"
# ZPL2PDF - Conversor ZPL para PDF
## Versão: $(Get-Date -Format "yyyy.MM.dd")
## Plataforma: $Platform
## Build: $Configuration
## Data: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## Comandos Disponíveis:

### Modo Conversão:
ZPL2PDF.exe -i arquivo.txt -o pasta -n nome.pdf -w 7.5 -h 15 -u in

### Modo Daemon:
ZPL2PDF.exe start                                    # Pasta padrão, extrai do ZPL
ZPL2PDF.exe start -l "C:\Custom Path"               # Pasta customizada
ZPL2PDF.exe start -w 7.5 -h 15 -u in                # Parâmetros fixos
ZPL2PDF.exe stop                                     # Parar daemon
ZPL2PDF.exe status                                   # Verificar status

## Instalação:
1. Extrair arquivos para pasta desejada
2. Executar ZPL2PDF.exe --help para ver opções
3. Para modo daemon, criar pasta de monitoramento

## Suporte:
- Windows: Pasta padrão em %USERPROFILE%\Documents\ZPL2PDF Auto Converter
- Linux: Pasta padrão em $HOME/Documents/ZPL2PDF Auto Converter
"@
    
    $versionInfo | Out-File -FilePath "$TargetDir\VERSION.txt" -Encoding UTF8
    Write-ColorOutput "   Criado: VERSION.txt" $InfoColor
}

# Verificar se ajuda foi solicitada
if ($Help) {
    Show-Help
    exit 0
}

# Início do script
Write-ColorOutput "ZPL2PDF - Iniciando Build Cross-Platform" $InfoColor
Write-ColorOutput "===========================================" $InfoColor
Write-ColorOutput "Configuração: $Configuration" $InfoColor
Write-ColorOutput "Diretório de saída: $OutputDir" $InfoColor
Write-ColorOutput ""

# Verificar se dotnet está instalado
Write-ColorOutput "Verificando .NET SDK..." $InfoColor
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-ColorOutput ".NET SDK não encontrado! Instale o .NET 9.0 SDK." $ErrorColor
    exit 1
}
Write-ColorOutput ".NET SDK encontrado: $dotnetVersion" $SuccessColor
Write-ColorOutput ""

# Limpar builds anteriores se solicitado
if ($Clean) {
    Write-ColorOutput "Limpando builds anteriores..." $InfoColor
    if (Test-Path $OutputDir) {
        Remove-Item $OutputDir -Recurse -Force
        Write-ColorOutput "   Diretório $OutputDir removido" $SuccessColor
    }
    if (Test-Path "bin") {
        Remove-Item "bin" -Recurse -Force
        Write-ColorOutput "   Diretório bin removido" $SuccessColor
    }
    Write-ColorOutput ""
}

# Executar testes se solicitado
if ($Test) {
    Write-ColorOutput "Executando testes..." $InfoColor
    if (!(Invoke-CommandSafe "dotnet test --configuration $Configuration --verbosity quiet" "Testes")) {
        Write-ColorOutput "Testes falharam! Build cancelado." $ErrorColor
        exit 1
    }
    Write-ColorOutput ""
}

# Definir plataformas para build
$platforms = @(
    @{RID="win-x64"; Name="Windows x64"; ExeName="ZPL2PDF.exe"},
    @{RID="win-x86"; Name="Windows x86"; ExeName="ZPL2PDF.exe"},
    @{RID="linux-x64"; Name="Linux x64"; ExeName="ZPL2PDF"},
    @{RID="linux-arm64"; Name="Linux ARM64"; ExeName="ZPL2PDF"},
    @{RID="linux-arm"; Name="Linux ARM"; ExeName="ZPL2PDF"},
    @{RID="osx-x64"; Name="macOS x64"; ExeName="ZPL2PDF"},
    @{RID="osx-arm64"; Name="macOS ARM64"; ExeName="ZPL2PDF"}
)

$successCount = 0
$totalCount = $platforms.Count

# Build para cada plataforma
foreach ($platform in $platforms) {
    Write-ColorOutput "Buildando $($platform.Name)..." $InfoColor
    Write-ColorOutput "   RID: $($platform.RID)" $InfoColor
    
    $platformDir = "$OutputDir\$($platform.RID)"
    Ensure-Directory $platformDir
    
    $buildCommand = "dotnet publish -c $Configuration -r $($platform.RID) --self-contained true -o `"$platformDir`" -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true"
    
    if (Invoke-CommandSafe $buildCommand "Build $($platform.Name)") {
        # Copiar arquivos de configuração
        Copy-ConfigFiles $platformDir
        
        # Criar arquivo de versão
        New-VersionInfo $platformDir $platform.Name
        
        # Verificar se executável foi criado
        $exePath = "$platformDir\$($platform.ExeName)"
        if (Test-Path $exePath) {
            $fileSize = [math]::Round((Get-Item $exePath).Length / 1MB, 2)
            Write-ColorOutput "   Executável: $($platform.ExeName) ($fileSize MB)" $SuccessColor
            $successCount++
        } else {
            Write-ColorOutput "   Executável não encontrado: $exePath" $ErrorColor
        }
    }
    
    Write-ColorOutput ""
}

# Resumo final
Write-ColorOutput "Resumo do Build" $InfoColor
Write-ColorOutput "==================" $InfoColor
Write-ColorOutput "Sucessos: $successCount/$totalCount" $SuccessColor
Write-ColorOutput "Falhas: $($totalCount - $successCount)/$totalCount" $ErrorColor
Write-ColorOutput ""

if ($successCount -eq $totalCount) {
    Write-ColorOutput "Todos os builds foram concluídos com sucesso!" $SuccessColor
    Write-ColorOutput "Releases disponíveis em: $OutputDir" $InfoColor
    Write-ColorOutput ""
    Write-ColorOutput "Estrutura de saída:" $InfoColor
    Get-ChildItem $OutputDir -Directory | ForEach-Object {
        $exeName = if ($_.Name -like "win-*") { "ZPL2PDF.exe" } else { "ZPL2PDF" }
        $exePath = "$($_.FullName)\$exeName"
        if (Test-Path $exePath) {
            $size = [math]::Round((Get-Item $exePath).Length / 1MB, 2)
            Write-ColorOutput "   $($_.Name)\$exeName ($size MB)" $InfoColor
        }
    }
} else {
    Write-ColorOutput "Alguns builds falharam. Verifique os erros acima." $WarningColor
    exit 1
}

Write-ColorOutput ""
Write-ColorOutput "Build concluído!" $SuccessColor