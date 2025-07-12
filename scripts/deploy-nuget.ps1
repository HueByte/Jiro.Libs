#!/usr/bin/env pwsh

# Test NuGet deployment script
param(
    [string]$Version = "3.0.0",
    [string]$Source = "https://api.nuget.org/v3/index.json",
    [switch]$DryRun = $false,
    [switch]$Local = $false
)

Write-Host "🚀 Jiro.Commands NuGet Deployment Script" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Yellow
Write-Host "Source: $Source" -ForegroundColor Yellow

# Set working directory
$RepoRoot = Split-Path -Parent $PSScriptRoot
$PackagesDir = Join-Path $RepoRoot "packages"
$ProjectPath = Join-Path $RepoRoot "src\Jiro.Commands\Jiro.Commands.csproj"

Write-Host "📁 Repository: $RepoRoot" -ForegroundColor Gray
Write-Host "📁 Packages: $PackagesDir" -ForegroundColor Gray

# Ensure packages directory exists
if (-not (Test-Path $PackagesDir)) {
    New-Item -ItemType Directory -Path $PackagesDir -Force | Out-Null
    Write-Host "✅ Created packages directory" -ForegroundColor Green
}

try {
    # Build and pack
    Write-Host "🔨 Building solution..." -ForegroundColor Blue
    dotnet build "$RepoRoot\src\Main.sln" --configuration Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }

    Write-Host "📦 Creating NuGet package..." -ForegroundColor Blue
    dotnet pack $ProjectPath `
        --no-build `
        --configuration Release `
        --output $PackagesDir `
        -p:PackageVersion=$Version `
        -p:AssemblyVersion=$Version `
        -p:FileVersion=$Version

    if ($LASTEXITCODE -ne 0) { throw "Package creation failed" }

    # Find the package
    $PackageFile = Get-ChildItem -Path $PackagesDir -Filter "Jiro.Commands.$Version.nupkg" | Select-Object -First 1
    if (-not $PackageFile) {
        throw "Package file not found: Jiro.Commands.$Version.nupkg"
    }

    Write-Host "✅ Package created: $($PackageFile.FullName)" -ForegroundColor Green

    if ($Local) {
        Write-Host "🏠 Local package creation complete!" -ForegroundColor Green
        return
    }

    # Test push command (dry run or actual)
    if ($DryRun) {
        Write-Host "🧪 DRY RUN - Would execute:" -ForegroundColor Yellow
        Write-Host "dotnet nuget push `"$($PackageFile.FullName)`" --source `"$Source`" --skip-duplicate" -ForegroundColor Gray
    } else {
        if (-not $env:NUGET_TOKEN -and $Source -like "*nuget.org*") {
            Write-Warning "NUGET_TOKEN environment variable not set. Set it before running without --DryRun"
            Write-Host "Example: `$env:NUGET_TOKEN = 'your-api-key'" -ForegroundColor Gray
            return
        }

        Write-Host "🚀 Pushing to NuGet..." -ForegroundColor Blue

        if ($Source -like "*nuget.org*") {
            dotnet nuget push $PackageFile.FullName `
                --api-key $env:NUGET_TOKEN `
                --source $Source `
                --skip-duplicate
        } elseif ($Source -like "*github.com*") {
            dotnet nuget push $PackageFile.FullName `
                --api-key $env:GITHUB_TOKEN `
                --source $Source `
                --skip-duplicate
        } else {
            # Generic source
            dotnet nuget push $PackageFile.FullName `
                --source $Source `
                --skip-duplicate
        }

        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Package deployed successfully!" -ForegroundColor Green
        } else {
            throw "Package deployment failed"
        }
    }

} catch {
    Write-Error "❌ Deployment failed: $_"
    exit 1
}

Write-Host "🎉 Deployment script completed!" -ForegroundColor Green
