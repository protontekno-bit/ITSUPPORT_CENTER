@echo off
setlocal enabledelayedexpansion
title Bebaskan Kapasitas Disk C: (Disable Hibernation)
color 0A

echo ===============================================================================
echo          BEBASKAN KAPASITAS DISK C: (DISABLE HIBERNATION / HIBERFIL.SYS)
echo ===============================================================================
echo.
echo [INFO] Fitur ini akan menonaktifkan mode hibernasi dan menghapus file hiberfil.sys.
echo Ruang disk di drive C:\ akan langsung bertambah sebesar 8 GB hingga 32 GB (sesuai kapasitas RAM).
echo.

powercfg.exe /hibernate off

if %ERRORLEVEL% equ 0 (
    echo [SUKSES] Hibernasi dinonaktifkan! File hiberfil.sys berhasil dihapus.
    echo Kapasitas penyimpanan drive C:\ telah dilegakan secara instan.
) else (
    echo [ERROR] Gagal menjalankan powercfg. Pastikan dijalankan sebagai Administrator.
)

echo.
pause
