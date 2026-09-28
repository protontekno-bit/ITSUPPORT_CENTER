@echo off
setlocal enabledelayedexpansion
title Reset Total Windows Firewall ke Default Pabrik
color 0C

echo ===============================================================================
echo            RESET TOTAL WINDOWS FIREWALL KE KONDISI DEFAULT PABRIK
echo ===============================================================================
echo.
echo [INFO] Mereset seluruh basis data dan aturan firewall...
echo.

netsh advfirewall reset
sc config mpssvc start= auto >nul 2>&1
net start mpssvc >nul 2>&1

if %ERRORLEVEL% equ 0 (
    echo [SUKSES] Windows Firewall berhasil direset total ke default pabrik!
    echo Seluruh aturan yang bentrok atau corrupt telah dibersihkan.
) else (
    echo [ERROR] Gagal mereset firewall. Pastikan dijalankan sebagai Administrator.
)

echo.
pause
