@echo off
setlocal EnableDelayedExpansion
title Pusat Kontrol Adapter Jaringan & Wi-Fi Settings
color 0B

echo ==============================================================================
echo        🎛️ PUSAT KONTROL ADAPTER JARINGAN & WI-FI SETTINGS
echo ==============================================================================
echo.
echo  Pilih Kontrol Jaringan:
echo   1. 🎛️ Buka Adapter Jaringan Klasik (ncpa.cpl)
echo   2. 📶 Buka Pengaturan Wi-Fi Windows (ms-settings:network-wifi)
echo   3. 🔄 Minta Ulang IP dari Router (ipconfig /release & /renew)
echo   4. 🌐 Buka Pengaturan Proxy Windows (ms-settings:network-proxy)
echo.
set /p "NET_OP=Masukkan nomor pilihan [1-4]: "

if "%NET_OP%"=="1" start ncpa.cpl
if "%NET_OP%"=="2" start ms-settings:network-wifi
if "%NET_OP%"=="3" (
    echo [*] Melepas IP...
    ipconfig /release
    echo [*] Meminta IP baru...
    ipconfig /renew
    echo [SUKSES] IP berhasil diperbarui!
)
if "%NET_OP%"=="4" start ms-settings:network-proxy

echo.
echo ==============================================================================
pause
