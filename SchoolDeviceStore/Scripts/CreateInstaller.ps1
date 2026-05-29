param(
    [string]$Configuration = "Release",
    [string]$AppVersion = "1.0.0"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
$singleExeScript = Join-Path $scriptDir "PublishSingleExe.ps1"
$singleExeDir = Join-Path $repoRoot "publish\single-exe"
$installerDir = Join-Path $repoRoot "Installer"
$payloadDir = Join-Path $installerDir "payload"
$payloadExe = Join-Path $payloadDir "SchoolDeviceStore.GUI.exe"
$issFile = Join-Path $installerDir "SchoolDeviceStoreInstaller.iss"

Write-Host "==> Publishing single executable..."
powershell -ExecutionPolicy Bypass -File $singleExeScript -Configuration $Configuration

$latestExe = Get-ChildItem $singleExeDir -Filter "SchoolDeviceStore.GUI_*.exe" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $latestExe) {
    throw "No published executable found in: $singleExeDir"
}

if (!(Test-Path $payloadDir)) {
    New-Item -ItemType Directory -Path $payloadDir | Out-Null
}

Copy-Item $latestExe.FullName $payloadExe -Force
Write-Host "==> Payload prepared: $payloadExe"

$isccCandidates = @(
    "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles(x86)\Inno Setup 6\ISCC.exe",
    "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
)

$iscc = $isccCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $iscc) {
    $cmd = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($cmd) {
        $iscc = $cmd.Source
    }
}

if (-not $iscc) {
    throw "Inno Setup compiler (ISCC.exe) not found. Please install Inno Setup 6 first: https://jrsoftware.org/isdl.php"
}

Write-Host "==> Building installer with ISCC..."
& $iscc "/DMyAppVersion=$AppVersion" $issFile
if ($LASTEXITCODE -ne 0) {
    throw "ISCC failed with exit code $LASTEXITCODE"
}

$installerOut = Join-Path $repoRoot "publish\installer\SchoolDeviceStoreSetup.exe"
if (!(Test-Path $installerOut)) {
    throw "Installer output not found: $installerOut"
}

$sizeMb = [Math]::Round((Get-Item $installerOut).Length / 1MB, 2)
Write-Host "==> Installer created successfully:" -ForegroundColor Green
Write-Host "    $installerOut"
Write-Host "    Size: $sizeMb MB"
