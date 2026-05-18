@echo off
chcp 65001 >nul
echo ========================================================
echo   UPDATING AND STARTING SCHOOL DEVICE STORE
echo ========================================================
echo.

echo [1/3] Cleaning up old processes...
taskkill /F /IM SchoolDeviceStore.GUI.exe >nul 2>&1
timeout /t 1 /nobreak >nul

echo [2/3] Compiling the latest source code...

:: Try dotnet first
dotnet --version >nul 2>&1
if %errorlevel% equ 0 (
    dotnet build "%~dp0SchoolDeviceStore.sln"
    goto check_error
)

:: If dotnet fails, try to find MSBuild from Visual Studio
set "MSBUILD_PATH="
if exist "%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" (
    for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
        set "MSBUILD_PATH=%%i"
    )
)

if not "%MSBUILD_PATH%"=="" (
    echo Using MSBuild: "%MSBUILD_PATH%"
    "%MSBUILD_PATH%" "%~dp0SchoolDeviceStore.sln" /t:Build /p:Configuration=Debug
) else (
    echo.
    echo LỖI: Không tìm thấy 'dotnet' hoặc 'MSBuild'. Vui lòng chạy ứng dụng trực tiếp từ Visual Studio (bấm F5^).
    pause
    exit /b 1
)

:check_error
if %errorlevel% neq 0 (
    echo.
    echo COMPILATION ERROR: Source code contains errors, cannot run application!
    pause
    exit /b %errorlevel%
)

echo [3/3] Starting the application...
start "" "%~dp0GUI.WinForms\bin\Debug\net48\SchoolDeviceStore.GUI.exe"
exit
