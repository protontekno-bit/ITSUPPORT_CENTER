@echo off
setlocal enabledelayedexpansion
title Kompresi Sistem Windows (CompactOS)
color 0B

echo ===============================================================================
echo            KOMPRESI SISTEM WINDOWS (COMPACTOS LZX)
echo ===============================================================================
echo.
echo [INFO] Fitur ini mengompresi file biner sistem Windows (C:\Windows).
echo [INFO] Menghemat 4 - 8 GB ruang penyimpanan disk C:\ tanpa mengurangi kecepatan.
echo [INFO] Proses ini memerlukan waktu 1 hingga 3 menit. Harap tunggu...
echo.

compact.exe /compactos:always

echo.
echo ===============================================================================
echo [SELESAI] Kompresi CompactOS telah selesai diterapkan!
echo ===============================================================================
echo.
pause
