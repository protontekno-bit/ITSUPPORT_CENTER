@echo off
setlocal enabledelayedexpansion
title Backup Sistem Windows ke File WIM (DISM Capture)
color 0B

echo ===============================================================================
echo         BACKUP SISTEM WINDOWS KE FILE WIM (MICROSOFT DISM CAPTURE)
echo ===============================================================================
echo.

set "TARGET_DIR=D:\Backup_Windows_WIM"
if not exist "D:\" set "TARGET_DIR=C:\Backup_Windows_WIM"
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%" >nul 2>&1

set "WIM_NAME=Windows_Master_Backup_%date:~-4%%date:~7,2%%date:~4,2%.wim"
set "WIM_PATH=%TARGET_DIR%\%WIM_NAME%"

echo Lokasi Target File WIM: %WIM_PATH%
echo.
echo [INFO] Memulai proses penulisan file master WIM dari partisi C:\...
echo        Proses memerlukan waktu beberapa menit tergantung kecepatan drive.
echo.

dism.exe /Capture-Image /CaptureDir:C:\ /ImageFile:"%WIM_PATH%" /Name:"WindowsMasterBackup" /Description:"IT Support Center Backup" /Compress:fast /Verify

if %ERRORLEVEL% equ 0 (
    echo.
    echo ===============================================================================
    echo [SUKSES] Backup WIM berhasil dibuat di: %WIM_PATH%
    echo ===============================================================================
) else (
    echo.
    echo [ERROR] DISM Capture menghasilkan kode: %ERRORLEVEL%. Pastikan sisa ruang mencukupi.
)

echo.
pause
