# ============================================================================
# Script auxiliar para fazer push das imagens Docker para GHCR
# Execute este script após completar a autenticação do GitHub com escopo write:packages
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$RepoOwner = "brunoleocam"
$GhcrImage = "ghcr.io/brunoleocam/zpl2pdf"

function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[!] $Message" -ForegroundColor Yellow
}

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "[ERRO] $Message" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Push GHCR - ZPL2PDF v$Version" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o Docker está disponível
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-ErrorMsg "Docker não encontrado. Instale o Docker primeiro."
    exit 1
}

# Verificar autenticação GitHub CLI
Write-Info "Verificando autenticação GitHub CLI..."
$ghAuth = gh auth status 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-ErrorMsg "GitHub CLI não autenticado. Execute: gh auth login"
    exit 1
}

# Verificar escopo write:packages
$ghScopes = gh auth status 2>&1 | Select-String "Token scopes"
if ($ghScopes -notmatch "write:packages") {
    Write-Warning "Token não tem escopo 'write:packages'"
    Write-Info "Execute: gh auth refresh -h github.com -s write:packages"
    Write-Info "Depois, acesse https://github.com/login/device e complete a autenticação"
    exit 1
}

# Fazer login no GHCR
Write-Info "Autenticando no GHCR..."
$ghToken = gh auth token 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-ErrorMsg "Falha ao obter token do GitHub CLI"
    exit 1
}

echo $ghToken | docker login ghcr.io -u $RepoOwner --password-stdin 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-ErrorMsg "Falha ao fazer login no GHCR"
    exit 1
}

Write-Success "Autenticado no GHCR!"

# Verificar se as imagens existem
Write-Info "Verificando imagens Docker..."
$images = docker images "$GhcrImage`:$Version" --format "{{.Repository}}:{{.Tag}}"
if (-not $images) {
    Write-ErrorMsg "Imagem $GhcrImage`:$Version não encontrada"
    Write-Info "Execute primeiro o script full-release.ps1 para buildar as imagens"
    exit 1
}

# Calcular tags
$majorMinor = $Version -replace '\.\d+$', ''
$major = $Version -replace '\.\d+\.\d+$', ''

# Push das imagens
Write-Info "Fazendo push das imagens para GHCR..."

$tags = @($Version, "latest", "alpine", $majorMinor, $major)
$failed = $false

foreach ($tag in $tags) {
    Write-Info "Pushing $GhcrImage`:$tag..."
    docker push "$GhcrImage`:$tag" 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Falha ao fazer push de $tag"
        $failed = $true
    } else {
        Write-Success "$tag publicado com sucesso!"
    }
}

if ($failed) {
    Write-Warning "Algumas tags podem não ter sido publicadas. Verifique os erros acima."
    exit 1
} else {
    Write-Success "Todas as imagens foram publicadas no GHCR!"
    Write-Host ""
    Write-Host "Link: https://github.com/$RepoOwner/ZPL2PDF/pkgs/container/zpl2pdf" -ForegroundColor Cyan
    Write-Host ""
}

