Write-Host "Installing Template.CLI tool globally..." -ForegroundColor Cyan

# Install from the nested nupkg directory - CORRECT PATH
dotnet tool install --global --add-source ./Template.CLI/nupkg Template.CLI

if ($LASTEXITCODE -eq 0) {
    Write-Host "Tool installed successfully!" -ForegroundColor Green
    Write-Host "Usage: dotnet-new-webapi [AppName]" -ForegroundColor Yellow
} else {
    Write-Host "Installation failed!" -ForegroundColor Red
    Write-Host "If tool is already installed, try: dotnet tool uninstall -g Template.CLI" -ForegroundColor Yellow
    exit 1
}