Write-Host "Packing Template.CLI as NuGet package..." -ForegroundColor Cyan

# Create nupkg directory if it doesn't exist
$nupkgDir = "Template.CLI\nupkg"
if (-not (Test-Path $nupkgDir)) {
    New-Item -ItemType Directory -Path $nupkgDir -Force
}

# Pack the project - CORRECT PATH
dotnet pack Template.CLI\Template.CLI.csproj -c Release -o $nupkgDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "Pack successful!" -ForegroundColor Green
    Write-Host "Package created in: $nupkgDir" -ForegroundColor Yellow
} else {
    Write-Host "Pack failed!" -ForegroundColor Red
    exit 1
}