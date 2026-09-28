@echo off
setlocal EnableDelayedExpansion
title Wake-on-LAN & Pengatur Profil IP (CLI)
color 0B
cls

echo ======================================================================
echo         WAKE-ON-LAN (WOL) ^& PENGATUR PROFIL ALAMAT IP (CLI)
echo ======================================================================
echo.
echo  1 = Setel Semua Adapter ke DHCP Otomatis (IP ^& DNS Dinamis)
echo  2 = Setel IP Statis Cepat pada Ethernet (192.168.1.150 / 24)
echo  3 = Kirim Magic Packet Wake-on-LAN via PowerShell
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "NET_OPT="
set /p "NET_OPT=Pilih Nomor Operasi [0-3]: "

if "%NET_OPT%"=="1" (
    echo [*] Menerapkan DHCP Otomatis pada Ethernet ^& Wi-Fi...
    netsh interface ip set address "Ethernet" dhcp >nul 2>&1
    netsh interface ip set dns "Ethernet" dhcp >nul 2>&1
    netsh interface ip set address "Wi-Fi" dhcp >nul 2>&1
    netsh interface ip set dns "Wi-Fi" dhcp >nul 2>&1
    ipconfig /renew >nul 2>&1
    echo [OK] Adapter telah beralih ke DHCP Otomatis.
    pause
    exit /b 0
)

if "%NET_OPT%"=="2" (
    echo [*] Menerapkan IP Statis: 192.168.1.150, Mask: 255.255.255.0, GW: 192.168.1.1...
    netsh interface ip set address "Ethernet" static 192.168.1.150 255.255.255.0 192.168.1.1 1 >nul 2>&1
    netsh interface ip set dns "Ethernet" static 8.8.8.8 primary >nul 2>&1
    echo [OK] Setelan IP Statis berhasil diterapkan pada Ethernet.
    pause
    exit /b 0
)

if "%NET_OPT%"=="3" (
    set "TARGET_MAC="
    set /p "TARGET_MAC=Masukkan MAC Address target (contoh: 00-11-22-33-44-55): "
    if "!TARGET_MAC!"=="" (
        echo [!] MAC Address kosong.
        pause
        exit /b 1
    )
    echo [*] Menyiarkan Magic Packet WoL ke !TARGET_MAC!...
    powershell -NoProfile -Command "$mac='!TARGET_MAC!'.Replace(':','').Replace('-',''); $b=[byte[]](,0xFF*6); 1..16|%%{$b+=[byte[]](0..5|%%{[Convert]::ToByte($mac.Substring($_*2,2),16)})}; $u=New-Object Net.Sockets.UdpClient; $u.Connect([Net.IPAddress]::Broadcast,9); $u.Send($b,$b.Length); $u.Close(); Write-Host '[OK] Packet terkirim.'"
    pause
    exit /b 0
)

exit /b 0
