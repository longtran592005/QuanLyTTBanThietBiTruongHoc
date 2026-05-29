@echo off
echo ================================
echo BUILD INSTALLER - 1 CLICK
echo ================================

REM ====== CONFIG ======
set PROJECT_DIR=d:\1_Tran Van Long\NCKH\LTNC
set INNO_PATH="C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
set ISS_FILE=%PROJECT_DIR%\installer\SchoolDeviceStore.iss

echo.
echo [1/3] Di chuyen toi project...
cd /d %PROJECT_DIR%

echo.
echo [2/3] Build va publish app...
powershell -ExecutionPolicy Bypass -File .\scripts\publish-windows-gui.ps1

echo.
echo [3/3] Build installer (.exe)...
%INNO_PATH% %ISS_FILE%

echo.
echo ================================
echo DONE! CHECK FILE SETUP .EXE
echo ================================
pause
