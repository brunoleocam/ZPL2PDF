# ============================================================================
# Script auxiliar para criar checkpoint manual do release
# Use este script quando precisar criar um checkpoint manualmente
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string[]]$CompletedSteps = @()
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$CheckpointFile = Join-Path $ProjectRoot ".release-checkpoint-$Version.json"

$checkpoint = @{
    Version = $Version
    StartedAt = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    CompletedSteps = $CompletedSteps
    Data = @{}
    LastUpdated = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
}

# Adicionar dados específicos baseados nas etapas concluídas
if ($CompletedSteps -contains "BuildDockerImages") {
    # Verificar se Docker Hub foi publicado
    $dockerHubDone = $false
    if ($CompletedSteps -contains "DockerHubPushed") {
        $dockerHubDone = $true
    }
    
    # Verificar se GHCR foi publicado
    $ghcrDone = $false
    if ($CompletedSteps -contains "GHCRPushed") {
        $ghcrDone = $true
    }
    
    $checkpoint.Data.DockerHubPushed = $dockerHubDone
    $checkpoint.Data.GHCRPushed = $ghcrDone
}

try {
    $checkpoint | ConvertTo-Json -Depth 10 | Set-Content $CheckpointFile -Force
    Write-Host "[OK] Checkpoint criado: $CheckpointFile" -ForegroundColor Green
    Write-Host "Etapas concluídas: $($CompletedSteps -join ', ')" -ForegroundColor Cyan
} catch {
    Write-Host "[ERRO] Falha ao criar checkpoint: $_" -ForegroundColor Red
    exit 1
}

