@echo off
:: Judul Window
title Windows License Auditor
:: Warna hijau Matrix (0A) biar jelas kontrasnya
color 0A

:: --- CEK ADMIN ---
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo.
    echo [ERROR] Script ini membutuhkan akses Administrator!
    echo Silakan Klik Kanan file ini -> "Run as Administrator"
    echo.
    pause
    exit /b
)

:START
cls
echo ========================================================
echo          LAPORAN STATUS LISENSI WINDOWS
echo ========================================================
echo.
echo [1] INFORMASI SISTEM OPERASI
echo --------------------------------------------------------
systeminfo | findstr /B /C:"OS Name" /C:"OS Version"
echo.

echo [2] JENIS LISENSI DAN CHANNEL (Retail/OEM/Volume)
echo --------------------------------------------------------
echo Sedang membaca data lisensi (mohon tunggu)...
cscript //nologo %windir%\system32\slmgr.vbs /dli
echo.

echo [3] STATUS MASA AKTIF (Permanen/Expired)
echo --------------------------------------------------------
cscript //nologo %windir%\system32\slmgr.vbs /xpr
echo.

echo [4] INFO DETAIL LISENSI (Untuk Analisis Lanjut)
echo --------------------------------------------------------
wmic path SoftwareLicensingProduct where (LicenseStatus='1') get Name,PartialProductKey,LicenseStatus,Description /format:list 2>nul | findstr /v "^$"
echo.

echo ========================================================
echo                ANALISIS SINGKAT
echo ========================================================
echo.
echo PANDUAN CARA BACA:
echo.
echo 1. Lihat Bagian [2] "Description":
echo    - RETAIL / OEM_DM  = Aman (Resmi).
echo    - VOLUME_KMSCLIENT = Indikasi KMS Client / Volume.
echo.
echo 2. Lihat Bagian [3]:
echo    - "Permanently activated" = Lisensi Permanen.
echo    - "Volume activation will expire..." = Lisensi Berjangka (Akan expired berkala).
echo.
echo ========================================================
echo.
pause
