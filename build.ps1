Write-Host "Building Template.CLI tool..." -ForegroundColor Cyan

# Build the CLI project - CORRECT PATH
dotnet build Template.CLI\Template.CLI.csproj -c Release

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful!" -ForegroundColor Green
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}