@echo off
setlocal EnableDelayedExpansion
title Audit Kualitas Sinyal Wi-Fi & Detail Adapter
color 0A

echo ==============================================================================
echo        📶 AUDIT KUALITAS SINYAL WI-FI & DETAIL ADAPTER WLAN
echo ==============================================================================
echo.

echo [*] Membaca status antarmuka Wireless LAN...
echo.

netsh wlan show interfaces

echo.
echo ==============================================================================
echo  CATATAN:
echo  - Sinyal di bawah 60%% rawan terjadi packet loss / koneksi putus-putus.
echo  - Channel 1, 6, 11 adalah standar 2.4 GHz; Channel 36+ adalah 5 GHz.
echo ==============================================================================
pause
