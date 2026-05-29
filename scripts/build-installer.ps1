Param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$ProjectPath = "SchoolDeviceStore\GUI.WinForms\SchoolDeviceStore.GUI.csproj",
    [string]$InnoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
    [string]$VcRedistPath = "", # optional path to vc_redist_x64.exe
    [string]$OutInstallerName = "SchoolDeviceStore-Setup.exe"
)

# 1) Run publish script
Write-Host "Step 1: Running publish script..."
$publishScript = Join-Path (Get-Location) "scripts\publish-windows-gui.ps1"
if (-not (Test-Path $publishScript)) { Write-Error "Publish script not found: $publishScript"; exit 2 }
& $publishScript -Configuration $Configuration -Platform $Platform
if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit $LASTEXITCODE }

# 2) Optionally copy VC++ redistributable into installer\tools
$installerTools = Join-Path (Get-Location) "installer\tools"
if (-not (Test-Path $installerTools)) { New-Item -ItemType Directory -Path $installerTools | Out-Null }
if ($VcRedistPath -ne "") {
    if (-not (Test-Path $VcRedistPath)) {
        Write-Warning "VC redistributable not found: $VcRedistPath. Continuing without bundling VC++ runtime."
    }
    else {
        Copy-Item -Path $VcRedistPath -Destination (Join-Path $installerTools "vc_redist_x64.exe") -Force
        Write-Host "Copied VC redistributable into installer\\tools"
    }
}

# 3) Find Inno Setup compiler
if (-not (Test-Path $InnoSetupPath)) {
    Write-Error "Inno Setup compiler not found at $InnoSetupPath. Edit script parameter or install Inno Setup."; exit 4
}

# 4) Compile the installer
$iss = Join-Path (Get-Location) "installer\SchoolDeviceStore.iss"
if (-not (Test-Path $iss)) { Write-Error "ISS file not found: $iss"; exit 5 }
Write-Host "Compiling installer with ISCC: $InnoSetupPath"
& "$InnoSetupPath" $iss
if ($LASTEXITCODE -ne 0) { Write-Error "ISCC failed (exit $LASTEXITCODE)"; exit $LASTEXITCODE }

Write-Host "Installer build complete. Check Inno output folder for $OutInstallerName" -ForegroundColor Green
Write-Host "If you provided VC redistributable it will be bundled and run silently during install."