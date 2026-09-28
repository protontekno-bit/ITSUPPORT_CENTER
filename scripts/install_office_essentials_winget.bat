@echo off
setlocal enabledelayedexpansion
title Pemasang Software Kantor Otomatis (Winget)
color 0B

echo ===============================================================================
echo            PEMASANG PAKET SOFTWARE KANTOR OTOMATIS (MICROSOFT WINGET)
echo ===============================================================================
echo.

where winget >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Microsoft Winget tidak terdeteksi pada sistem ini.
    echo Pastikan Windows 10 (1809+) atau Windows 11 telah memperbarui App Installer.
    echo.
    pause
    exit /b 1
)

echo [INFO] Microsoft Winget terdeteksi. Memulai instalasi paket esensial kantor:
echo   1. Google Chrome
echo   2. 7-Zip
echo   3. Adobe Acrobat Reader 64-bit
echo   4. VLC Media Player
echo   5. AnyDesk
echo   6. Notepad++
echo.

echo -------------------------------------------------------------------------------
echo [1/6] Mengunduh & Memasang Google Chrome...
winget install --id "Google.Chrome" --silent --accept-source-agreements --accept-package-agreements --source winget

echo -------------------------------------------------------------------------------
echo [2/6] Mengunduh & Memasang 7-Zip...
winget install --id "7zip.7zip" --silent --accept-source-agreements --accept-package-agreements --source winget

echo -------------------------------------------------------------------------------
echo [3/6] Mengunduh & Memasang Adobe Acrobat Reader...
winget install --id "Adobe.Acrobat.Reader.64-bit" --silent --accept-source-agreements --accept-package-agreements --source winget

echo -------------------------------------------------------------------------------
echo [4/6] Mengunduh & Memasang VLC Media Player...
winget install --id "VideoLAN.VLC" --silent --accept-source-agreements --accept-package-agreements --source winget

echo -------------------------------------------------------------------------------
echo [5/6] Mengunduh & Memasang AnyDesk...
winget install --id "AnyDeskSoftwareGmbH.AnyDesk" --silent --accept-source-agreements --accept-package-agreements --source winget

echo -------------------------------------------------------------------------------
echo [6/6] Mengunduh & Memasang Notepad++...
winget install --id "Notepad++.Notepad++" --silent --accept-source-agreements --accept-package-agreements --source winget

echo.
echo ===============================================================================
echo [SELESAI] Seluruh paket software kantor telah diproses!
echo ===============================================================================
echo.
pause
