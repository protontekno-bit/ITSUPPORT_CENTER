@echo off
setlocal EnableDelayedExpansion
title Audit Konfigurasi IP Lengkap & Netstat Koneksi Aktif
color 0B

echo ==============================================================================
echo        📋 AUDIT KONFIGURASI IP LENGKAP & NETSTAT KONEKSI AKTIF
echo ==============================================================================
echo.

echo [1/3] Konfigurasi IP Seluruh Adapter (ipconfig /all):
ipconfig /all
echo.

echo [2/3] Statistik Lalu Lintas Interface (netstat -e):
netstat -e
echo.

echo [3/3] Daftar Socket Koneksi Aktif (netstat -ano):
netstat -ano | findstr /i "ESTABLISHED LISTENING"

echo.
echo ==============================================================================
pause
