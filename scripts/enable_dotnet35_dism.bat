@echo off
setlocal enabledelayedexpansion
title Pengaktifan .NET Framework 3.5 / 2.0 (DISM Online)
color 09

echo ===============================================================================
echo            PENGAKTIFAN .NET FRAMEWORK 3.5 / 2.0 (DISM ONLINE)
echo ===============================================================================
echo.
echo [INFO] Fitur ini mengaktifkan .NET Framework 3.5 & 2.0 untuk aplikasi kantor legacy
echo        (Aplikasi Akuntansi, Faktur Pajak, Software Kasir, dll).
echo [INFO] Mengunduh komponen resmi dari server Microsoft Windows Update...
echo.

dism.exe /online /enable-feature /featurename:NetFx3 /all /norestart

if %ERRORLEVEL% equ 0 (
    echo.
    echo [SUKSES] .NET Framework 3.5 / 2.0 berhasil diaktifkan pada sistem ini!
) else if %ERRORLEVEL% equ 3010 (
    echo.
    echo [SUKSES] .NET Framework 3.5 berhasil dipasang. Diperlukan restart komputer.
) else (
    echo.
    echo [WARNING] DISM menghasilkan kode keluar: %ERRORLEVEL%.
    echo Pastikan komputer terhubung ke internet.
)

echo.
pause
