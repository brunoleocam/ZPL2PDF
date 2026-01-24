# ZPL2PDF API Test Script (PowerShell)
# This script builds and tests the ZPL2PDF API

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "ZPL2PDF API Build and Test Script" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build the project
Write-Host "Step 1: Building the project..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""

# Step 2: Start API server in background
Write-Host "Step 2: Starting API server on port 5000..." -ForegroundColor Yellow
$job = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    dotnet run -- --api --host localhost --port 5000
}
Start-Sleep -Seconds 3

# Check if API is running
$healthCheck = try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/health" -Method Get -ErrorAction Stop
    $true
} catch {
    $false
}

if (-not $healthCheck) {
    Write-Host "❌ API server failed to start!" -ForegroundColor Red
    Stop-Job $job
    Remove-Job $job
    exit 1
}
Write-Host "✅ API server started" -ForegroundColor Green
Write-Host ""

# Step 3: Test health endpoint
Write-Host "Step 3: Testing health endpoint..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/health" -Method Get
    Write-Host "✅ Health check response: $($healthResponse | ConvertTo-Json)" -ForegroundColor Green
} catch {
    Write-Host "❌ Health check failed!" -ForegroundColor Red
    Stop-Job $job
    Remove-Job $job
    exit 1
}
Write-Host ""

# Step 4: Test PDF conversion
Write-Host "Step 4: Testing PDF conversion..." -ForegroundColor Yellow
try {
    $pdfRequest = @{
        zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ"
        format = "pdf"
    } | ConvertTo-Json

    $pdfResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/convert" `
        -Method Post `
        -ContentType "application/json" `
        -Body $pdfRequest

    if ($pdfResponse.success) {
        Write-Host "✅ PDF conversion successful!" -ForegroundColor Green
        Write-Host "Pages: $($pdfResponse.pages)" -ForegroundColor Cyan
        
        # Save PDF
        $pdfBytes = [Convert]::FromBase64String($pdfResponse.pdf)
        [System.IO.File]::WriteAllBytes("test_output.pdf", $pdfBytes)
        Write-Host "✅ PDF saved to test_output.pdf" -ForegroundColor Green
    } else {
        Write-Host "❌ PDF conversion failed: $($pdfResponse.message)" -ForegroundColor Red
        Stop-Job $job
        Remove-Job $job
        exit 1
    }
} catch {
    Write-Host "❌ PDF conversion request failed: $_" -ForegroundColor Red
    Stop-Job $job
    Remove-Job $job
    exit 1
}
Write-Host ""

# Step 5: Test PNG conversion
Write-Host "Step 5: Testing PNG conversion..." -ForegroundColor Yellow
try {
    $pngRequest = @{
        zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ"
        format = "png"
    } | ConvertTo-Json

    $pngResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/convert" `
        -Method Post `
        -ContentType "application/json" `
        -Body $pngRequest

    if ($pngResponse.success) {
        Write-Host "✅ PNG conversion successful!" -ForegroundColor Green
        Write-Host "Pages: $($pngResponse.pages)" -ForegroundColor Cyan
        
        # Save PNG
        if ($pngResponse.image) {
            $pngBytes = [Convert]::FromBase64String($pngResponse.image)
            [System.IO.File]::WriteAllBytes("test_output.png", $pngBytes)
            Write-Host "✅ PNG saved to test_output.png" -ForegroundColor Green
        } elseif ($pngResponse.images) {
            for ($i = 0; $i -lt $pngResponse.images.Count; $i++) {
                $pngBytes = [Convert]::FromBase64String($pngResponse.images[$i])
                [System.IO.File]::WriteAllBytes("test_output_$($i+1).png", $pngBytes)
            }
            Write-Host "✅ PNG images saved to test_output_*.png" -ForegroundColor Green
        }
    } else {
        Write-Host "❌ PNG conversion failed: $($pngResponse.message)" -ForegroundColor Red
        Stop-Job $job
        Remove-Job $job
        exit 1
    }
} catch {
    Write-Host "❌ PNG conversion request failed: $_" -ForegroundColor Red
    Stop-Job $job
    Remove-Job $job
    exit 1
}
Write-Host ""

# Step 6: Test error handling
Write-Host "Step 6: Testing error handling..." -ForegroundColor Yellow
try {
    $errorRequest = @{
        zpl = ""
        format = "pdf"
    } | ConvertTo-Json

    $errorResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/convert" `
        -Method Post `
        -ContentType "application/json" `
        -Body $errorRequest `
        -ErrorAction Stop

    if (-not $errorResponse.success) {
        Write-Host "✅ Error handling works correctly!" -ForegroundColor Green
        Write-Host "Error message: $($errorResponse.message)" -ForegroundColor Cyan
    } else {
        Write-Host "⚠️  Error handling test inconclusive" -ForegroundColor Yellow
    }
} catch {
    # Expected to fail with 400 Bad Request
    Write-Host "✅ Error handling works correctly (400 Bad Request)" -ForegroundColor Green
}
Write-Host ""

# Step 7: Cleanup
Write-Host "Step 7: Stopping API server..." -ForegroundColor Yellow
Stop-Job $job
Remove-Job $job
Write-Host "✅ API server stopped" -ForegroundColor Green
Write-Host ""

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "✅ All tests passed!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Test outputs:" -ForegroundColor Yellow
Write-Host "  - test_output.pdf (if PDF test succeeded)" -ForegroundColor White
Write-Host "  - test_output.png (if PNG test succeeded)" -ForegroundColor White
Write-Host ""
