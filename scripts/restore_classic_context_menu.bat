@echo off
setlocal enabledelayedexpansion
title Kembalikan Menu Klik Kanan Klasik Windows 11
color 0D

echo ===============================================================================
echo            KEMBALIKAN MENU KLIK KANAN KLASIK WINDOWS 11 (WINDOWS 10 STYLE)
echo ===============================================================================
echo.
echo [INFO] Mendaftarkan key CLSID override pada Registry HKCU...
reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /f /ve

if %ERRORLEVEL% equ 0 (
    echo [OK] Registry berhasil diperbarui.
    echo [INFO] Me-restart Windows Explorer untuk menerapkan perubahan...
    taskkill /f /im explorer.exe >nul 2>&1
    timeout /t 1 /nobreak >nul
    start explorer.exe
    echo [SUKSES] Menu klik kanan klasik Windows 10 sekarang aktif di Windows 11!
) else (
    echo [ERROR] Gagal menambahkan key registry.
)

echo.
pause
