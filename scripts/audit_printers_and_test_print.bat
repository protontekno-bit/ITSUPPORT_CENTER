@echo off
setlocal EnableDelayedExpansion
title Audit Status Printer & Cetak Halaman Uji (Test Page)
color 0B

echo ==============================================================================
echo        🖨️ AUDIT STATUS PRINTER TERPASANG & CETAK TEST PAGE
echo ==============================================================================
echo.

echo [*] Daftar printer terpasang di komputer ini:
powershell -Command "Get-Printer | Select-Object Name, DriverName, PortName, PrinterStatus, Default | Format-Table -AutoSize"

echo.
set /p "PRINTER_NAME=Ketik nama printer untuk mengirim Test Page (atau kosongkan untuk lewati): "

if not "%PRINTER_NAME%"=="" (
    echo [*] Mengirim Windows Test Page ke: %PRINTER_NAME%...
    rundll32.exe printui.dll,PrintUIEntry /k /n "%PRINTER_NAME%"
    echo [SUKSES] Perintah cetak terkirim!
)

echo.
echo ==============================================================================
pause
