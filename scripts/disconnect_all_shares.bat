@echo off
title Memutus Semua Koneksi Jaringan
color 4f

echo ==================================================
echo   MEMBERSIHKAN SEMUA KONEKSI DRIVE JARINGAN
echo ==================================================
echo.

:: 1. Memutus semua mapped drive (Z:, Y:, dll)
echo [PROSES] Memutus drive letter...
net use * /delete /y
echo.

:: 2. Opsional: Membersihkan tiket autentikasi (agar diminta password lagi)
:: Berguna jika Anda ingin login ulang sebagai user lain nanti
echo [PROSES] Membersihkan sesi login...
klist purge >nul 2>&1

echo.
echo ==================================================
echo   [SELESAI] Semua terputus. Jendela menutup dalam 3 detik.
echo ==================================================

:: Memberi jeda 3 detik sebelum menutup otomatis
timeout /t 3 >nul