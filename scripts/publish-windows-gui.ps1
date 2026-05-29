Param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$ProjectPath = "SchoolDeviceStore\GUI.WinForms\SchoolDeviceStore.GUI.csproj",
    [string]$OutDir = "publish"
)

function Find-MSBuild {
    $candidates = @(
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
        "$env:ProgramFiles(x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"
    )

    foreach ($p in $candidates) {
        if (Test-Path $p) { return $p }
    }

    # try vswhere
    $vswhere = "$env:ProgramFiles(x86)\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $instance = & $vswhere -latest -requires Microsoft.Component.MSBuild -products * -property installationPath 2>$null
        if ($instance) {
            $ms = Join-Path $instance 'MSBuild\Current\Bin\MSBuild.exe'
            if (Test-Path $ms) { return $ms }
        }
    }

    return $null
}

Write-Host "Publishing project: $ProjectPath ($Configuration|$Platform)"

$projectFull = Join-Path (Get-Location) $ProjectPath
if (-not (Test-Path $projectFull)) { Write-Error "Project not found: $projectFull"; exit 2 }

Write-Host "Checking for dotnet CLI..."
$dotnetCmd = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnetCmd) {
    Write-Host "Using dotnet build: $($dotnetCmd.Source)"
    & dotnet build $projectFull -c $Configuration -p:Platform=$Platform
    if ($LASTEXITCODE -ne 0) { Write-Error "dotnet build failed (exit $LASTEXITCODE)"; exit $LASTEXITCODE }
}
else {
    $msbuild = Find-MSBuild
    if (-not $msbuild) {
        Write-Error "MSBuild not found. Install Visual Studio or Build Tools (with MSBuild) and re-run this script."; exit 3
    }

    # warn if using older .NET Framework MSBuild which doesn't support SDK-style projects
    if ($msbuild -match "\\Microsoft\\NET\\Framework") {
        Write-Warning "Found legacy MSBuild at $msbuild. This may fail for SDK-style projects. Prefer installing 'dotnet' or Visual Studio 2019/2022 and re-run."
    }

    Write-Host "Using MSBuild: $msbuild"
    & $msbuild $projectFull /p:Configuration=$Configuration /p:Platform=$Platform /t:Build
    if ($LASTEXITCODE -ne 0) { Write-Error "Build failed (exit $LASTEXITCODE)"; exit $LASTEXITCODE }
}

# locate output
$projDir = Split-Path $projectFull -Parent

# Prefer platform-specific output first (bin\x64\Release), then fallback (bin\Release)
$searchRoots = @(
    (Join-Path $projDir "bin\$Platform\$Configuration"),
    (Join-Path $projDir "bin\$Configuration")
) | Where-Object { Test-Path $_ }

if (-not $searchRoots -or $searchRoots.Count -eq 0) {
    Write-Error "Build succeeded but no expected bin output roots were found under $projDir"; exit 4
}

$exeFiles = @()
foreach ($root in $searchRoots) {
    $exeFiles += Get-ChildItem -Path $root -Recurse -Filter *.exe -ErrorAction SilentlyContinue | Where-Object { $_.Name -notmatch "vshost" }
}

if (-not $exeFiles -or $exeFiles.Count -eq 0) {
    Write-Error "No exe found under output roots: $($searchRoots -join ', ')"; exit 5
}

# Prefer project exe in platform-specific path, then newest file.
$projName = [System.IO.Path]::GetFileNameWithoutExtension($projectFull)
$preferred = $exeFiles |
    Where-Object { $_.Name -like "$projName*.exe" } |
    Sort-Object @{Expression={ $_.FullName -match "\\$Platform\\|win-$Platform" }; Descending=$true}, @{Expression={ $_.LastWriteTimeUtc }; Descending=$true} |
    Select-Object -First 1

if (-not $preferred) {
    $preferred = $exeFiles | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
}

$outputPath = $preferred.DirectoryName
Write-Host "Selected build output: $outputPath"

$publishDir = Join-Path (Get-Location) $OutDir
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
New-Item -ItemType Directory -Path $publishDir | Out-Null

Write-Host "Copying files from $outputPath to $publishDir"
Copy-Item -Path (Join-Path $outputPath '*') -Destination $publishDir -Recurse -Force

# If build emitted a platform-specific subfolder (e.g. win-x64, x64) that contains the real app files,
# move its contents up to the publish root so the installer places exe and DLLs together.
$platformDirs = Get-ChildItem -Path $publishDir -Directory -ErrorAction SilentlyContinue | Where-Object { $_.Name -match '^(win-)?(x64|x86)$' }
foreach ($pd in $platformDirs) {
    $exeInPlatform = Get-ChildItem -Path $pd.FullName -Filter *.exe -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -notmatch 'vshost' }
    if ($exeInPlatform) {
        Write-Host "Flattening platform folder $($pd.Name) into publish root"
        Copy-Item -Path (Join-Path $pd.FullName '*') -Destination $publishDir -Recurse -Force
        # optional: remove the now-redundant platform folder
        Remove-Item -Path $pd.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# Ensure SQLite.Interop.dll is present next to the exe by copying from x64/x86 subfolders if needed
$interopCandidates = @("$publishDir\SQLite.Interop.dll", "$publishDir\x64\SQLite.Interop.dll", "$publishDir\x86\SQLite.Interop.dll")
foreach ($c in $interopCandidates) {
    if ((Test-Path $c) -and (-not (Test-Path (Join-Path $publishDir 'SQLite.Interop.dll')))) {
        Copy-Item -Path $c -Destination (Join-Path $publishDir 'SQLite.Interop.dll') -Force
        Write-Host "Copied SQLite.Interop.dll into publish root from $c"
        break
    }
}

# create zip
$zip = "$publishDir.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path (Join-Path $publishDir '*') -DestinationPath $zip

Write-Host "Publish complete. Files are in: $publishDir" -ForegroundColor Green
Write-Host "Zip archive: $zip"
Write-Host "Check that 'SQLite.Interop.dll' exists next to the exe in $publishDir. If not, ensure Costura ran and the native DLLs are present under costura-win-x64 and costura-win-x86 in the project before building."

Write-Host "If target machines are 64-bit, include Visual C++ Redistributable x64 if you still see 'module could not be found' errors."
