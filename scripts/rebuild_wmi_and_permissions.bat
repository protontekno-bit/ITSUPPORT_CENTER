@echo off
setlocal EnableDelayedExpansion
title Perbaikan Total WMI & Hak Akses Sistem (CLI)
color 0C
cls

echo ======================================================================
echo       PERBAIKAN REPOSITORI WMI ^& HAK AKSES SISTEM (ACL REBUILDER)
echo ======================================================================
echo.
echo  1 = Bangun Ulang Repositori WMI (winmgmt /resetrepository)
echo  2 = Reset Hak Akses Default Folder Sistem (icacls System32)
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "WMI_OPT="
set /p "WMI_OPT=Pilih Nomor Operasi [0-2]: "

if "%WMI_OPT%"=="1" (
    echo.
    echo [*] Memverifikasi repositori WMI...
    winmgmt /verifyrepository
    echo [*] Menghentikan service winmgmt...
    net stop winmgmt /y >nul 2>&1
    echo [*] Me-reset repositori WMI...
    winmgmt /resetrepository
    echo [*] Menjalankan kembali service winmgmt...
    net start winmgmt >nul 2>&1
    echo [OK] Pembangunan ulang repositori WMI selesai!
    pause
    exit /b 0
)

if "%WMI_OPT%"=="2" (
    echo.
    echo [*] Mereset hak akses folder System32 (icacls /reset)...
    icacls "%windir%\System32" /reset /t /c /l /q >nul 2>&1
    echo [OK] Hak akses sistem berhasil dinormalkan!
    pause
    exit /b 0
)

exit /b 0
