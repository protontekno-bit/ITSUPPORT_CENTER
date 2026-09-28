@echo off
setlocal enabledelayedexpansion
title Karantina Jaringan Darurat (Ransomware Air-Gap Isolation)
color 0C

echo ===============================================================================
echo        KARANTINA JARINGAN DARURAT (RANSOMWARE AIR-GAP ISOLATION)
echo ===============================================================================
echo.
echo Pilih Aksi:
echo   [1] AKTIFKAN KARANTINA TOTAL (Putus Semua Koneksi LAN ^& Internet)
echo   [2] BUKA ISOLASI (Kembalikan Jaringan ke Normal)
echo   [0] Batal / Keluar
echo.

set "CHOICE="
set /p "CHOICE=Masukkan Pilihan Anda [0-2]: "

if "%CHOICE%"=="1" (
    echo.
    echo [INFO] Mengisolasi komputer... Memblokir semua paket Inbound ^& Outbound...
    netsh advfirewall set allprofiles firewallpolicy blockinbound,blockoutbound >nul 2>&1
    echo [SUKSES] Komputer berhasil DIISOLASI TOTAL dari seluruh jaringan LAN/Internet!
)

if "%CHOICE%"=="2" (
    echo.
    echo [INFO] Membuka isolasi... Mengembalikan kebijakan normal...
    netsh advfirewall set allprofiles firewallpolicy blockinbound,allowoutbound >nul 2>&1
    echo [SUKSES] Isolasi dibuka! Koneksi LAN dan Internet telah pulih.
)

echo.
pause
