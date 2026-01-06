Write-Host "🧪 Testing the CLI tool..." -ForegroundColor Cyan

# Create a test directory
$testDir = "..\TestScaffold"
if (Test-Path $testDir) {
    Remove-Item $testDir -Recurse -Force
}

New-Item -ItemType Directory -Path $testDir -Force
Set-Location $testDir

Write-Host "Testing interactive mode..." -ForegroundColor Yellow
dotnet-new-webapi

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Test completed successfully!" -ForegroundColor Green
    Write-Host "📁 Test project created in: $testDir" -ForegroundColor Yellow
} else {
    Write-Host "❌ Test failed!" -ForegroundColor Red
}