@echo off
setlocal EnableDelayedExpansion
title Restore & Pasang Driver dari Folder Backup (PnPUtil)
color 0A

echo ==============================================================================
echo        📥 RESTORE & PASANG DRIVER DARI FOLDER BACKUP (PNPUTIL)
echo ==============================================================================
echo.

set "DEFAULT_FOLDER=D:\Driver_Backup_%COMPUTERNAME%"
if not exist "%DEFAULT_FOLDER%" set "DEFAULT_FOLDER=C:\Driver_Backup_%COMPUTERNAME%"

echo Folder default backup terdeteksi: %DEFAULT_FOLDER%
set /p "TARGET_FOLDER=Masukkan path folder driver [.inf] (Tekan ENTER untuk default): "

if "%TARGET_FOLDER%"=="" set "TARGET_FOLDER=%DEFAULT_FOLDER%"

if not exist "%TARGET_FOLDER%" (
    echo [ERROR] Folder "%TARGET_FOLDER%" tidak ditemukan!
    pause
    exit /b 1
)

echo.
echo [*] Memasang seluruh paket driver (.inf) dari: %TARGET_FOLDER%
echo [*] Proses sedang berjalan...
echo.

pnputil /add-driver "%TARGET_FOLDER%\*.inf" /subdirs /install

echo.
echo ==============================================================================
echo [INFO] Proses instalasi driver selesai.
echo ==============================================================================
pause
