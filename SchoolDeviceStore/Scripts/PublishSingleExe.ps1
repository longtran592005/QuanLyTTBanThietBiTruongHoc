param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$guiProject = Join-Path $repoRoot "GUI.WinForms\SchoolDeviceStore.GUI.csproj"
$outputExe = Join-Path $repoRoot "GUI.WinForms\bin\$Configuration\net48\SchoolDeviceStore.GUI.exe"
$distDir = Join-Path $repoRoot "publish\single-exe"
$distExe = Join-Path $distDir "SchoolDeviceStore.GUI.exe"

Write-Host "==> Restoring and building GUI project ($Configuration)..."
dotnet restore $guiProject
if ($LASTEXITCODE -ne 0) {
    throw "dotnet restore failed with exit code $LASTEXITCODE"
}

dotnet build $guiProject -c $Configuration
if ($LASTEXITCODE -ne 0) {
    throw "dotnet build failed with exit code $LASTEXITCODE"
}

if (!(Test-Path $outputExe)) {
    throw "Build completed but output exe not found: $outputExe"
}

if (Test-Path $distDir) {
    Remove-Item $distDir -Recurse -Force
}

New-Item -ItemType Directory -Path $distDir | Out-Null

try {
    Copy-Item $outputExe $distExe -Force
}
catch {
    Write-Warning "Could not overwrite $distExe (possibly locked)."
}

$sizeMb = [Math]::Round((Get-Item $distExe).Length / 1MB, 2)
Write-Host "==> Done. Single-file package created:" -ForegroundColor Green
if (Test-Path $distExe) {
    Write-Host "    $distExe"
}
Write-Host "    Size: $sizeMb MB"
Write-Host ""
Write-Host "Note: This build targets .NET Framework 4.8. Ensure target machine has .NET Framework 4.8 installed."
Write-Host "Database and logs will be stored in: %LOCALAPPDATA%\SchoolDeviceStore\"
