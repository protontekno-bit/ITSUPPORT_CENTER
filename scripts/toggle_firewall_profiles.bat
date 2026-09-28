@echo off
setlocal enabledelayedexpansion
title Saklar Status Profil Firewall (Domain, Private, Public)
color 0D

echo ===============================================================================
echo            SAKLAR STATUS PROFIL FIREWALL (DOMAIN, PRIVATE, PUBLIC)
echo ===============================================================================
echo.
echo [STATUS PROFIL SAAT INI]
netsh advfirewall show allprofiles state
echo.
echo Pilih Aksi:
echo   [1] AKTIFKAN PROTEKSI SEMUA PROFIL (ON - Sangat Direkomendasikan)
echo   [2] MATIKAN SEMENTARA SEMUA PROFIL (OFF - Untuk Pengujian Jaringan)
echo   [0] Batal / Keluar
echo.

set "CHOICE="
set /p "CHOICE=Masukkan Pilihan Anda [0-2]: "

if "%CHOICE%"=="1" (
    netsh advfirewall set allprofiles state on >nul 2>&1
    echo [SUKSES] Seluruh profil Firewall berhasil DIAKTIFKAN (ON)!
)

if "%CHOICE%"=="2" (
    netsh advfirewall set allprofiles state off >nul 2>&1
    echo [PERINGATAN] Seluruh profil Firewall telah DINONAKTIFKAN (OFF)!
)

echo.
pause
