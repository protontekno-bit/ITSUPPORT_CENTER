@echo off
setlocal EnableDelayedExpansion
title Perbaikan Total Jaringan 1-Klik (Full Network Repair)
color 0C

echo ==============================================================================
echo        ⚡ PERBAIKAN TOTAL JARINGAN 1-KLIK (FULL NETWORK RECOVERY)
echo ==============================================================================
echo.

echo [1/6] Melepas Alamat IP Aktif (ipconfig /release)...
ipconfig /release >nul 2>&1

echo [2/6] Membersihkan Cache DNS & Tabel ARP...
ipconfig /flushdns >nul 2>&1
netsh interface ip delete arpcache >nul 2>&1

echo [3/6] Mereset Winsock Catalog...
netsh winsock reset >nul 2>&1

echo [4/6] Mereset TCP/IP Protocol Stack...
netsh int ip reset >nul 2>&1

echo [5/6] Mereset WinHTTP System Proxy...
netsh winhttp reset proxy >nul 2>&1

echo [6/6] Meminta Alamat IP Baru dari DHCP Router (ipconfig /renew)...
ipconfig /renew

echo.
echo [*] Menguji koneksi akhir ke Google DNS (8.8.8.8)...
ping -n 2 8.8.8.8 | findstr /i "Lost Average"

echo.
echo ==============================================================================
echo [SUKSES] Sekuens perbaikan jaringan selesai! Disarankan Restart jika perlu.
echo ==============================================================================
pause
