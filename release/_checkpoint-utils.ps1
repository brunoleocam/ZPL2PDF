# ============================================================================
# Release Checkpoint Utilities
# ============================================================================
# Shared functions for managing release checkpoints
# ============================================================================

function Get-CheckpointPath {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot
    )
    
    return Join-Path $ProjectRoot ".release-checkpoint-$Version.json"
}

function Get-Checkpoint {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot
    )
    
    $checkpointPath = Get-CheckpointPath -Version $Version -ProjectRoot $ProjectRoot
    
    if (Test-Path $checkpointPath) {
        try {
            $content = Get-Content $checkpointPath -Raw | ConvertFrom-Json
            return $content
        } catch {
            Write-Warning "Error reading checkpoint: $_"
            return $null
        }
    }
    
    return $null
}

function Initialize-Checkpoint {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot
    )
    
    $checkpointPath = Get-CheckpointPath -Version $Version -ProjectRoot $ProjectRoot
    
    # Check if already exists
    $existing = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    if ($existing) {
        return $existing
    }
    
    # Create new checkpoint
    $checkpoint = @{
        Version = $Version
        StartedAt = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
        LastUpdated = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
        CompletedSteps = @()
        FailedSteps = @()
        SkippedSteps = @()
        Data = @{}
    }
    
    Save-Checkpoint -Version $Version -ProjectRoot $ProjectRoot -Checkpoint $checkpoint
    
    return $checkpoint
}

function Save-Checkpoint {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot,
        
        [Parameter(Mandatory=$true)]
        [hashtable]$Checkpoint
    )
    
    $checkpointPath = Get-CheckpointPath -Version $Version -ProjectRoot $ProjectRoot
    
    # Update timestamp
    $Checkpoint.LastUpdated = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    
    try {
        $Checkpoint | ConvertTo-Json -Depth 10 | Set-Content $checkpointPath -Force
        return $true
    } catch {
        Write-Warning "Error saving checkpoint: $_"
        return $false
    }
}

function Mark-StepCompleted {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot,
        
        [Parameter(Mandatory=$true)]
        [int]$StepNumber,
        
        [Parameter(Mandatory=$false)]
        [hashtable]$Data = @{}
    )
    
    $checkpoint = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    if (-not $checkpoint) {
        $checkpoint = Initialize-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    }
    
    # Convert PSObject to hashtable if necessary
    if ($checkpoint -is [PSCustomObject]) {
        $checkpointHash = @{
            Version = $checkpoint.Version
            StartedAt = $checkpoint.StartedAt
            LastUpdated = $checkpoint.LastUpdated
            CompletedSteps = @()
            FailedSteps = @()
            SkippedSteps = @()
            Data = @{}
        }
        
        if ($checkpoint.CompletedSteps) {
            $checkpointHash.CompletedSteps = @($checkpoint.CompletedSteps)
        }
        if ($checkpoint.FailedSteps) {
            $checkpointHash.FailedSteps = @($checkpoint.FailedSteps)
        }
        if ($checkpoint.SkippedSteps) {
            $checkpointHash.SkippedSteps = @($checkpoint.SkippedSteps)
        }
        if ($checkpoint.Data) {
            $checkpointHash.Data = @{}
            $checkpoint.Data.PSObject.Properties | ForEach-Object {
                $checkpointHash.Data[$_.Name] = $_.Value
            }
        }
        $checkpoint = $checkpointHash
    }
    
    # Add step to completed (if not already)
    if ($checkpoint.CompletedSteps -notcontains $StepNumber) {
        $checkpoint.CompletedSteps += $StepNumber
    }
    
    # Remove from failed if it's there
    if ($checkpoint.FailedSteps -contains $StepNumber) {
        $checkpoint.FailedSteps = $checkpoint.FailedSteps | Where-Object { $_ -ne $StepNumber }
    }
    
    # Add data if provided
    foreach ($key in $Data.Keys) {
        $checkpoint.Data[$key] = $Data[$key]
    }
    
    Save-Checkpoint -Version $Version -ProjectRoot $ProjectRoot -Checkpoint $checkpoint
}

function Mark-StepFailed {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot,
        
        [Parameter(Mandatory=$true)]
        [int]$StepNumber,
        
        [Parameter(Mandatory=$false)]
        [string]$ErrorMessage = ""
    )
    
    $checkpoint = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    if (-not $checkpoint) {
        $checkpoint = Initialize-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    }
    
    # Convert PSObject to hashtable if necessary
    if ($checkpoint -is [PSCustomObject]) {
        $checkpointHash = @{
            Version = $checkpoint.Version
            StartedAt = $checkpoint.StartedAt
            LastUpdated = $checkpoint.LastUpdated
            CompletedSteps = @()
            FailedSteps = @()
            SkippedSteps = @()
            Data = @{}
        }
        
        if ($checkpoint.CompletedSteps) {
            $checkpointHash.CompletedSteps = @($checkpoint.CompletedSteps)
        }
        if ($checkpoint.FailedSteps) {
            $checkpointHash.FailedSteps = @($checkpoint.FailedSteps)
        }
        if ($checkpoint.SkippedSteps) {
            $checkpointHash.SkippedSteps = @($checkpoint.SkippedSteps)
        }
        if ($checkpoint.Data) {
            $checkpointHash.Data = @{}
            $checkpoint.Data.PSObject.Properties | ForEach-Object {
                $checkpointHash.Data[$_.Name] = $_.Value
            }
        }
        $checkpoint = $checkpointHash
    }
    
    # Add step to failed (if not already)
    if ($checkpoint.FailedSteps -notcontains $StepNumber) {
        $checkpoint.FailedSteps += $StepNumber
    }
    
    # Add error message
    if ($ErrorMessage) {
        if (-not $checkpoint.Data.ErrorMessages) {
            $checkpoint.Data.ErrorMessages = @{}
        }
        $checkpoint.Data.ErrorMessages["Step$StepNumber"] = $ErrorMessage
    }
    
    Save-Checkpoint -Version $Version -ProjectRoot $ProjectRoot -Checkpoint $checkpoint
}

function Mark-StepSkipped {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot,
        
        [Parameter(Mandatory=$true)]
        [int]$StepNumber
    )
    
    $checkpoint = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    if (-not $checkpoint) {
        $checkpoint = Initialize-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    }
    
    # Convert PSObject to hashtable if necessary
    if ($checkpoint -is [PSCustomObject]) {
        $checkpointHash = @{
            Version = $checkpoint.Version
            StartedAt = $checkpoint.StartedAt
            LastUpdated = $checkpoint.LastUpdated
            CompletedSteps = @()
            FailedSteps = @()
            SkippedSteps = @()
            Data = @{}
        }
        
        if ($checkpoint.CompletedSteps) {
            $checkpointHash.CompletedSteps = @($checkpoint.CompletedSteps)
        }
        if ($checkpoint.FailedSteps) {
            $checkpointHash.FailedSteps = @($checkpoint.FailedSteps)
        }
        if ($checkpoint.SkippedSteps) {
            $checkpointHash.SkippedSteps = @($checkpoint.SkippedSteps)
        }
        if ($checkpoint.Data) {
            $checkpointHash.Data = @{}
            $checkpoint.Data.PSObject.Properties | ForEach-Object {
                $checkpointHash.Data[$_.Name] = $_.Value
            }
        }
        $checkpoint = $checkpointHash
    }
    
    # Add step to skipped (if not already)
    if ($checkpoint.SkippedSteps -notcontains $StepNumber) {
        $checkpoint.SkippedSteps += $StepNumber
    }
    
    Save-Checkpoint -Version $Version -ProjectRoot $ProjectRoot -Checkpoint $checkpoint
}

function Test-StepCompleted {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot,
        
        [Parameter(Mandatory=$true)]
        [int]$StepNumber
    )
    
    $checkpoint = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    if (-not $checkpoint) {
        return $false
    }
    
    if ($checkpoint.Version -ne $Version) {
        return $false
    }
    
    $steps = if ($checkpoint.CompletedSteps) { @($checkpoint.CompletedSteps) } else { @() }
    return $steps -contains $StepNumber
}

function Clear-Checkpoint {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot
    )
    
    $checkpointPath = Get-CheckpointPath -Version $Version -ProjectRoot $ProjectRoot
    
    if (Test-Path $checkpointPath) {
        Remove-Item $checkpointPath -Force
        return $true
    }
    
    return $false
}

function Show-CheckpointStatus {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Version,
        
        [Parameter(Mandatory=$true)]
        [string]$ProjectRoot
    )
    
    $checkpoint = Get-Checkpoint -Version $Version -ProjectRoot $ProjectRoot
    
    if (-not $checkpoint) {
        Write-Host "No checkpoint found for version $Version" -ForegroundColor Yellow
        return
    }
    
    Write-Host ""
    Write-Host "Checkpoint Status - Version $Version" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Started at: $($checkpoint.StartedAt)" -ForegroundColor White
    Write-Host "Last updated: $($checkpoint.LastUpdated)" -ForegroundColor White
    Write-Host ""
    
    $completed = if ($checkpoint.CompletedSteps) { @($checkpoint.CompletedSteps) } else { @() }
    $failed = if ($checkpoint.FailedSteps) { @($checkpoint.FailedSteps) } else { @() }
    $skipped = if ($checkpoint.SkippedSteps) { @($checkpoint.SkippedSteps) } else { @() }
    
    if ($completed.Count -gt 0) {
        Write-Host "Completed steps ($($completed.Count)): " -ForegroundColor Green -NoNewline
        Write-Host ($completed -join ', ') -ForegroundColor White
    }
    
    if ($skipped.Count -gt 0) {
        Write-Host "Skipped steps ($($skipped.Count)): " -ForegroundColor Yellow -NoNewline
        Write-Host ($skipped -join ', ') -ForegroundColor White
    }
    
    if ($failed.Count -gt 0) {
        Write-Host "Failed steps ($($failed.Count)): " -ForegroundColor Red -NoNewline
        Write-Host ($failed -join ', ') -ForegroundColor White
    }
    
    Write-Host ""
}
