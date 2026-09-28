@echo off
setlocal EnableDelayedExpansion
title Inspeksi Pembajakan Startup & Persistence (Sysinternals Lite)
color 0D
cls

echo ======================================================================
echo    DETEKTOR PEMBAJAKAN PERSISTENSI STARTUP (AUTORUNS LITE)
echo ======================================================================
echo.
echo  1 = Periksa Integritas Winlogon (Shell ^& Userinit Hijacking)
echo  2 = Periksa Image File Execution Options (IFEO Debugger Hijack)
echo  3 = Tampilkan Seluruh Entri Autostart Registry Run
echo  4 = Pulihkan Winlogon ke Default Windows (explorer.exe)
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "AUTO_OPT="
set /p "AUTO_OPT=Pilih Nomor Operasi [0-4]: "

if "%AUTO_OPT%"=="1" (
    echo.
    echo [*] Memeriksa Winlogon Shell ^& Userinit...
    reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" /v Shell
    reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" /v Userinit
    echo.
    echo [INFO] Nilai normal: Shell = explorer.exe, Userinit = C:\Windows\system32\userinit.exe,
    pause
    exit /b 0
)

if "%AUTO_OPT%"=="2" (
    echo.
    echo [*] Memeriksa IFEO Debugger Hijack...
    reg query "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options" /s /f "Debugger" 2>nul
    if %errorlevel% neq 0 (
        echo [OK] Tidak ditemukan entri pembajakan debugger di IFEO (Bersih).
    )
    pause
    exit /b 0
)

if "%AUTO_OPT%"=="3" (
    echo.
    echo [*] Membaca HKLM Run...
    reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
    echo.
    echo [*] Membaca HKCU Run...
    reg query "HKCU\Software\Microsoft\Windows\CurrentVersion\Run"
    pause
    exit /b 0
)

if "%AUTO_OPT%"=="4" (
    echo.
    echo [*] Mereset Winlogon ke default Windows...
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" /v Shell /t REG_SZ /d "explorer.exe" /f
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon" /v Userinit /t REG_SZ /d "C:\Windows\system32\userinit.exe," /f
    echo [OK] Winlogon berhasil dipulihkan!
    pause
    exit /b 0
)

exit /b 0
