@echo off
setlocal EnableDelayedExpansion
title Pengatur DNS Cepat Adapter Jaringan
color 0B

echo ==============================================================================
echo        🌐 PENGATUR DNS CEPAT ADAPTER JARINGAN (GOOGLE / CLOUDFLARE / DHCP)
echo ==============================================================================
echo.
echo  Pilih target DNS:
echo   1. Google Public DNS (8.8.8.8 & 8.8.4.4)
echo   2. Cloudflare Fast DNS (1.1.1.1 & 1.0.0.1)
echo   3. Reset ke Otomatis (DHCP Router)
echo.
set /p "DNS_CHOICE=Masukkan pilihan Anda [1-3]: "

for /f "tokens=4" %%a in ('netsh interface show interface ^| findstr "Connected" ^| findstr /v "Virtual Hyper-V"') do set "ADAPTER=%%a"

if "%ADAPTER%"=="" (
    echo [ERROR] Tidak ditemukan adapter jaringan aktif.
    pause
    exit /b 1
)

echo.
echo [*] Adapter terdeteksi: %ADAPTER%

if "%DNS_CHOICE%"=="1" (
    echo [*] Mengatur ke Google DNS...
    netsh interface ip set dns name="%ADAPTER%" static 8.8.8.8
    netsh interface ip add dns name="%ADAPTER%" 8.8.4.4 index=2
    echo [SUKSES] DNS Google berhasil diterapkan.
)
if "%DNS_CHOICE%"=="2" (
    echo [*] Mengatur ke Cloudflare DNS...
    netsh interface ip set dns name="%ADAPTER%" static 1.1.1.1
    netsh interface ip add dns name="%ADAPTER%" 1.0.0.1 index=2
    echo [SUKSES] DNS Cloudflare berhasil diterapkan.
)
if "%DNS_CHOICE%"=="3" (
    echo [*] Mengembalikan ke DHCP...
    netsh interface ip set dns name="%ADAPTER%" dhcp
    echo [SUKSES] DNS Otomatis berhasil diterapkan.
)

ipconfig /flushdns >nul 2>&1
echo [*] Cache DNS berhasil dibersihkan.
echo.
pause
