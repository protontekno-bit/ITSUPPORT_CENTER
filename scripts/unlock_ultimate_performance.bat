@echo off
setlocal EnableDelayedExpansion
title Aktivasi Ultimate Performance & Optimasi Latensi (CLI)
color 0E
cls

echo ======================================================================
echo    AKTIVASI ULTIMATE PERFORMANCE ^& OPTIMASI LATENSI SISTEM
echo ======================================================================
echo.
echo  1 = Duplikasi Skema Daya 'Ultimate Performance' ke Sistem
echo  2 = Matikan Pembatasan Network Throttling (Registry)
echo  3 = Bersihkan Berkas Memory Dump Crash (MEMORY.DMP ^& Minidump)
echo  4 = Jalankan Seluruh Optimasi Sekaligus
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "PERF_OPT="
set /p "PERF_OPT=Pilih Nomor Operasi [0-4]: "

if "%PERF_OPT%"=="1" (
    echo [*] Membuka skema daya Ultimate Performance...
    powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61
    echo [OK] Skema Ultimate Performance berhasil ditambahkan.
    pause
    exit /b 0
)

if "%PERF_OPT%"=="2" (
    echo [*] Menonaktifkan network throttling...
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v NetworkThrottlingIndex /t REG_DWORD /d 0xFFFFFFFF /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v SystemResponsiveness /t REG_DWORD /d 0 /f >nul 2>&1
    echo [OK] Network Throttling dinonaktifkan.
    pause
    exit /b 0
)

if "%PERF_OPT%"=="3" (
    echo [*] Menghapus dump memory BSOD lama...
    del /f /q "%windir%\MEMORY.DMP" >nul 2>&1
    del /f /q "%windir%\Minidump\*.*" >nul 2>&1
    echo [OK] File dump berhasil dibersihkan!
    pause
    exit /b 0
)

if "%PERF_OPT%"=="4" (
    echo [*] Menerapkan paket lengkap Ultimate Performance...
    powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 >nul 2>&1
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v NetworkThrottlingIndex /t REG_DWORD /d 0xFFFFFFFF /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" /v SystemResponsiveness /t REG_DWORD /d 0 /f >nul 2>&1
    del /f /q "%windir%\MEMORY.DMP" >nul 2>&1
    del /f /q "%windir%\Minidump\*.*" >nul 2>&1
    echo [OK] Seluruh optimasi berhasil diterapkan!
    pause
    exit /b 0
)

exit /b 0
