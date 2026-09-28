@echo off
setlocal EnableDelayedExpansion
title Backup Semua Driver OEM / Pihak Ketiga (DISM)
color 0B

echo ==============================================================================
echo        💾 BACKUP SELURUH DRIVER OEM / PIHAK KETIGA KOMPUTER (DISM)
echo ==============================================================================
echo.

set "DEST=D:\Driver_Backup_%COMPUTERNAME%"
if not exist "D:\" set "DEST=C:\Driver_Backup_%COMPUTERNAME%"

echo [*] Target folder backup: %DEST%
if not exist "%DEST%" mkdir "%DEST%"

echo [*] Sedang mengekstrak seluruh driver dari DriverStore...
echo [*] Mohon tunggu beberapa saat...
echo.

dism /online /export-driver /destination:"%DEST%"

if %errorlevel% equ 0 (
    echo.
    echo ==============================================================================
    echo [SUKSES] Seluruh driver berhasil dicadangkan ke: %DEST%
    echo ==============================================================================
    start explorer.exe "%DEST%"
) else (
    echo.
    echo [GAGAL] Terjadi kesalahan saat mengekspor driver. Pastikan hak Administrator aktif.
)

pause
