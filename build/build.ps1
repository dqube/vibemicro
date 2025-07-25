[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$Target = "Build",
    [switch]$SkipTests,
    [switch]$CreatePackages,
    [switch]$PublishArtifacts,
    [string]$OutputPath = "",
    [string]$PackageVersion = ""
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Get the root directory
$RootDirectory = Split-Path $PSScriptRoot -Parent
$ArtifactsDirectory = Join-Path $RootDirectory "artifacts"

Write-Host "🏗️ BuildingBlocks Build Script" -ForegroundColor Cyan
Write-Host "==============================" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Target: $Target" -ForegroundColor Yellow
Write-Host "Root Directory: $RootDirectory" -ForegroundColor Yellow

# Clean artifacts directory
if (Test-Path $ArtifactsDirectory) {
    Write-Host "🧹 Cleaning artifacts directory..." -ForegroundColor Green
    Remove-Item $ArtifactsDirectory -Recurse -Force
}

# Create artifacts directory
New-Item -ItemType Directory -Path $ArtifactsDirectory -Force | Out-Null

try {
    switch ($Target.ToLower()) {
        "clean" {
            Write-Host "🧹 Cleaning solution..." -ForegroundColor Green
            dotnet clean $RootDirectory --configuration $Configuration --verbosity minimal
        }
        
        "restore" {
            Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Green
            dotnet restore $RootDirectory --verbosity minimal
        }
        
        "build" {
            Write-Host "🔨 Building solution..." -ForegroundColor Green
            dotnet build $RootDirectory --configuration $Configuration --no-restore --verbosity minimal
            
            if (!$SkipTests) {
                Write-Host "🧪 Running tests..." -ForegroundColor Green
                dotnet test $RootDirectory --configuration $Configuration --no-build --verbosity minimal --logger "trx" --results-directory "$ArtifactsDirectory\test-results"
                
                # Generate code coverage report
                Write-Host "📊 Generating code coverage report..." -ForegroundColor Green
                dotnet test $RootDirectory --configuration $Configuration --no-build --collect:"XPlat Code Coverage" --results-directory "$ArtifactsDirectory\coverage"
            }
        }
        
        "pack" {
            Write-Host "📦 Creating NuGet packages..." -ForegroundColor Green
            $packArgs = @(
                "pack"
                $RootDirectory
                "--configuration", $Configuration
                "--no-build"
                "--output", "$ArtifactsDirectory\packages"
                "--verbosity", "minimal"
            )
            
            if ($PackageVersion) {
                $packArgs += "--property:PackageVersion=$PackageVersion"
            }
            
            & dotnet @packArgs
        }
        
        "publish" {
            Write-Host "🚀 Publishing applications..." -ForegroundColor Green
            
            # Get all executable projects (those with <OutputType>Exe</OutputType>)
            $executableProjects = Get-ChildItem -Path $RootDirectory -Name "*.csproj" -Recurse | 
                Where-Object { 
                    $content = Get-Content (Join-Path $RootDirectory $_) -Raw
                    $content -match "<OutputType>Exe</OutputType>" -or $content -match "Microsoft\.NET\.Sdk\.Web"
                }
            
            foreach ($project in $executableProjects) {
                $projectName = [System.IO.Path]::GetFileNameWithoutExtension($project)
                $publishPath = Join-Path $ArtifactsDirectory "publish\$projectName"
                
                Write-Host "Publishing $projectName..." -ForegroundColor Yellow
                
                dotnet publish (Join-Path $RootDirectory $project) `
                    --configuration $Configuration `
                    --no-build `
                    --output $publishPath `
                    --verbosity minimal
            }
        }
        
        "ci" {
            Write-Host "🔄 Running CI build..." -ForegroundColor Green
            
            # Restore
            Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
            dotnet restore $RootDirectory --verbosity minimal
            
            # Build
            Write-Host "🔨 Building..." -ForegroundColor Yellow
            dotnet build $RootDirectory --configuration $Configuration --no-restore --verbosity minimal
            
            # Test
            Write-Host "🧪 Running tests..." -ForegroundColor Yellow
            dotnet test $RootDirectory --configuration $Configuration --no-build --verbosity minimal --logger "trx" --results-directory "$ArtifactsDirectory\test-results" --collect:"XPlat Code Coverage" --results-directory "$ArtifactsDirectory\coverage"
            
            # Pack
            if ($CreatePackages) {
                Write-Host "📦 Creating packages..." -ForegroundColor Yellow
                dotnet pack $RootDirectory --configuration $Configuration --no-build --output "$ArtifactsDirectory\packages" --verbosity minimal
            }
            
            # Publish
            if ($PublishArtifacts) {
                Write-Host "🚀 Publishing artifacts..." -ForegroundColor Yellow
                & $PSScriptRoot\build.ps1 -Target "publish" -Configuration $Configuration
            }
        }
        
        default {
            Write-Error "Unknown target: $Target. Valid targets are: clean, restore, build, pack, publish, ci"
        }
    }
    
    Write-Host "✅ Build completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Display artifacts summary
if (Test-Path $ArtifactsDirectory) {
    Write-Host "`n📁 Artifacts Summary:" -ForegroundColor Cyan
    Get-ChildItem $ArtifactsDirectory -Recurse -File | 
        Group-Object DirectoryName | 
        ForEach-Object {
            $relativePath = $_.Name.Replace($ArtifactsDirectory, "").TrimStart('\')
            Write-Host "  📂 $relativePath ($($_.Count) files)" -ForegroundColor Yellow
        }
} 