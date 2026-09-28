@echo off
setlocal EnableDelayedExpansion
title Pusat Kontrol Printer Klasik (Devices and Printers)
color 0B

echo ==============================================================================
echo        🎛️ PUSAT KONTROL PRINTER KLASIK WINDOWS
echo ==============================================================================
echo.
echo  Pilih Perkakas Pengelola Printer:
echo   1. 🖨️ Buka Devices and Printers Klasik (Windows 7/10 Control Panel)
echo   2. ⚙️ Buka Print Server Properties (Kelola Port IP & Driver)
echo   3. 🖥️ Buka Print Management Console (printmanagement.msc)
echo   4. 🩺 Buka Windows Printer Diagnostic Troubleshooter
echo.
set /p "PRINT_OP=Masukkan nomor pilihan [1-4]: "

if "%PRINT_OP%"=="1" start explorer.exe shell:::{A8A91A66-3A7D-4424-8D24-04E180695C5A}
if "%PRINT_OP%"=="2" start rundll32.exe printui.dll,PrintUIEntry /s
if "%PRINT_OP%"=="3" start printmanagement.msc
if "%PRINT_OP%"=="4" start msdt.exe /id PrinterDiagnostic

echo.
echo ==============================================================================
pause
