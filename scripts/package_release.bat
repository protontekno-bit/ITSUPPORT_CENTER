@echo off
setlocal enabledelayedexpansion

title Packaging IT Support Center Portable Release
cls
echo ==============================================================================
echo        📦 PACKAGING IT SUPPORT CENTER (PORTABLE RELEASE ZIP)
echo ==============================================================================
echo Membuat paket distribusi rilis mandiri siap pakai (.zip) untuk diunggah
echo ke GitHub Releases atau didistribusikan langsung ke flashdisk teknisi.
echo ==============================================================================
echo.

set "DIST_DIR=%~dp0..\dist"
set "PKG_DIR=%TEMP%\ITSupportCenter_Package"
set "ZIP_OUT=%DIST_DIR%\ITSupportCenter-v3.2.0-Portable.zip"

if not exist "%DIST_DIR%\release\ITSupportCenter.exe" (
    echo [!] Biner dist\release\ITSupportCenter.exe tidak ditemukan.
    echo [*] Memulai kompilasi biner release terlebih dahulu...
    dotnet publish "%~dp0..\src\ITSupportCenter\ITSupportCenter.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o "%DIST_DIR%\release"
)

echo [*] Menyiapkan struktur folder paket...
if exist "%PKG_DIR%" rd /s /q "%PKG_DIR%" >nul 2>&1
mkdir "%PKG_DIR%" >nul 2>&1

echo [*] Menyalin biner utama ITSupportCenter.exe...
copy /y "%DIST_DIR%\release\ITSupportCenter.exe" "%PKG_DIR%\ITSupportCenter.exe" >nul 2>&1

echo [*] Menyalin master launchers ^& dokumentasi...
copy /y "%~dp0..\START.bat" "%PKG_DIR%\" >nul 2>&1
copy /y "%~dp0..\IT_SUPPORT_CENTER.bat" "%PKG_DIR%\" >nul 2>&1
copy /y "%~dp0..\README.md" "%PKG_DIR%\" >nul 2>&1
copy /y "%~dp0..\DISCLAIMER.md" "%PKG_DIR%\" >nul 2>&1

echo [*] Menyalin folder scripts ^& assets...
xcopy "%~dp0..\scripts" "%PKG_DIR%\scripts\" /e /i /y >nul 2>&1
xcopy "%~dp0..\assets" "%PKG_DIR%\assets\" /e /i /y >nul 2>&1

echo [*] Mengompresi paket menjadi ZIP: %ZIP_OUT%...
if exist "%ZIP_OUT%" del /f /q "%ZIP_OUT%" >nul 2>&1

powershell -NoProfile -Command "Compress-Archive -Path '%PKG_DIR%\*' -DestinationPath '%ZIP_OUT%' -CompressionLevel Optimal -Force"

rd /s /q "%PKG_DIR%" >nul 2>&1

echo.
if exist "%ZIP_OUT%" (
    echo ==============================================================================
    echo [SUKSES] Paket rilis berhasil dibuat di:
    echo %ZIP_OUT%
    echo ==============================================================================
    echo Berkas ini siap diunggah ke halaman GitHub Releases:
    echo https://github.com/protontekno-bit/ITSUPPORT_CENTER/releases
    explorer.exe /select,"%ZIP_OUT%"
) else (
    echo [ERROR] Gagal membuat file ZIP paket.
)

echo.
pause
