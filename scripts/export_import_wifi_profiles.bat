@echo off
setlocal EnableDelayedExpansion
title Ekspor & Impor Massal Profil Wi-Fi (CLI)
color 0B
cls

echo ======================================================================
echo         EKSPOR ^& IMPOR MASSAL PROFIL WI-FI (.XML) (CLI)
echo ======================================================================
echo.
echo  1 = Ekspor Semua Profil Wi-Fi ke Folder (Sertakan Password .XML)
echo  2 = Impor Massal Profil Wi-Fi XML ke Komputer Ini
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "WIFI_OPT="
set /p "WIFI_OPT=Pilih Nomor Operasi [0-2]: "

if "%WIFI_OPT%"=="1" (
    set "DEST_DIR=%USERPROFILE%\Desktop\WIFI_PROFILES_EXPORT"
    echo Target folder default: !DEST_DIR!
    set /p "USER_DEST=Masukkan folder tujuan (Enter untuk default): "
    if not "!USER_DEST!"=="" set "DEST_DIR=!USER_DEST!"

    mkdir "!DEST_DIR!" 2>nul
    echo [*] Mengekspor profil Wi-Fi ke !DEST_DIR!...
    netsh wlan export profile folder="!DEST_DIR!" key=clear
    echo [OK] Profil Wi-Fi berhasil diekspor!
    pause
    exit /b 0
)

if "%WIFI_OPT%"=="2" (
    set "SRC_DIR=%USERPROFILE%\Desktop\WIFI_PROFILES_EXPORT"
    set /p "USER_SRC=Masukkan folder yang berisi file .xml profil (Enter untuk default Desktop): "
    if not "!USER_SRC!"=="" set "SRC_DIR=!USER_SRC!"

    if not exist "!SRC_DIR!" (
        echo [!] Direktori tidak ditemukan: !SRC_DIR!
        pause
        exit /b 1
    )

    echo [*] Mengimpor file profil .xml...
    for %%F in ("!SRC_DIR!\*.xml") do (
        echo Import: %%~nxF
        netsh wlan add profile filename="%%F" user=all >nul 2>&1
    )
    echo [OK] Semua profil Wi-Fi berhasil diimpor!
    pause
    exit /b 0
)

exit /b 0
