@echo off
setlocal enabledelayedexpansion
title Pusat Buka / Tutup Port Layanan Kantor
color 0E

echo ===============================================================================
echo            PUSAT MANAJEMEN PORT LAYANAN FIREWALL KANTOR
echo ===============================================================================
echo.
echo Pilih Layanan yang Ingin Dibuka di Windows Firewall:
echo   [1] Buka Port Remote Desktop (RDP TCP 3389)
echo   [2] Buka Port Web Server (HTTP 80 ^& HTTPS 443)
echo   [3] Buka Port Database Server (MS SQL 1433 ^& MySQL 3306)
echo   [4] Buka Port Kustom Sendiri
echo   [0] Batal / Keluar
echo.

set "CHOICE="
set /p "CHOICE=Masukkan Pilihan Anda [0-4]: "

if "%CHOICE%"=="1" (
    netsh advfirewall firewall add rule name="IT_ALLOW_RDP_3389" dir=in action=allow protocol=TCP localport=3389 profile=any >nul 2>&1
    netsh advfirewall firewall set rule group="remote desktop" new enable=Yes >nul 2>&1
    echo [SUKSES] Port RDP 3389 berhasil dibuka di Firewall!
)

if "%CHOICE%"=="2" (
    netsh advfirewall firewall add rule name="IT_ALLOW_WEB_80_443" dir=in action=allow protocol=TCP localport=80,443 profile=any >nul 2>&1
    echo [SUKSES] Port Web 80 (HTTP) dan 443 (HTTPS) berhasil dibuka!
)

if "%CHOICE%"=="3" (
    netsh advfirewall firewall add rule name="IT_ALLOW_DB_1433_3306" dir=in action=allow protocol=TCP localport=1433,3306 profile=any >nul 2>&1
    echo [SUKSES] Port Database MS SQL (1433) dan MySQL (3306) berhasil dibuka!
)

if "%CHOICE%"=="4" (
    set "CPORT="
    set /p "CPORT=Masukkan nomor port TCP yang ingin dibuka: "
    if defined CPORT (
        netsh advfirewall firewall add rule name="IT_ALLOW_CUSTOM_!CPORT!" dir=in action=allow protocol=TCP localport=!CPORT! profile=any >nul 2>&1
        echo [SUKSES] Port TCP !CPORT! berhasil dibuka di Firewall!
    )
)

echo.
pause
