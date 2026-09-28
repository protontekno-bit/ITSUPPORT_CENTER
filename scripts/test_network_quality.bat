@echo off
setlocal EnableDelayedExpansion
title Diagnosa Kualitas Koneksi 3-Titik (Gateway vs ISP vs Internet)
color 0B

echo ==============================================================================
echo        📊 DIAGNOSA KUALITAS KONEKSI 3-TITIK (PACKET LOSS & JITTER)
echo ==============================================================================
echo.

for /f "tokens=3" %%a in ('netsh interface ip show config ^| findstr /i "Default Gateway" ^| findstr /v "::"') do (
    if not "%%a"=="" set "GW=%%a"
)

echo [1/3] Menguji Hop 1: Router Default Gateway (%GW%)...
if not "%GW%"=="" (
    ping -n 5 %GW% | findstr /i "Lost Average"
) else (
    echo [ERROR] Gateway tidak terdeteksi!
)
echo.

echo [2/3] Menguji Hop 2: DNS Server / ISP...
ping -n 5 1.1.1.1 | findstr /i "Lost Average"
echo.

echo [3/3] Menguji Hop 3: Global Internet (Google 8.8.8.8)...
ping -n 5 8.8.8.8 | findstr /i "Lost Average"

echo.
echo ==============================================================================
echo  ANALISA CEPAT:
echo  - Jika Hop 1 Lost -> Masalah pada kabel LAN / sinyal Wi-Fi lokal.
echo  - Jika Hop 1 OK tapi Hop 3 Lost -> Masalah pada ISP / Modem Internet kantor.
echo ==============================================================================
pause
