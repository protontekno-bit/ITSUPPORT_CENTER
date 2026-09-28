@echo off
setlocal enabledelayedexpansion
title Izinkan / Blokir Respon Ping LAN (ICMPv4)
color 0B

echo ===============================================================================
echo            PENGATUR STATUS RESPON PING LAN (ICMPv4 ECHO REQUEST)
echo ===============================================================================
echo.

set "RULE_NAME=IT_ALLOW_ICMPV4_PING"
netsh advfirewall firewall show rule name="%RULE_NAME%" >nul 2>&1

if %ERRORLEVEL% equ 0 (
    echo [STATUS] Aturan izin ping aktif. Menonaktifkan respon Ping...
    netsh advfirewall firewall delete rule name="%RULE_NAME%" >nul 2>&1
    netsh advfirewall firewall set rule group="File and Printer Sharing" new enable=No >nul 2>&1
    echo [SUKSES] Respon Ping LAN sekarang TELAH DIBLOKIR.
) else (
    echo [STATUS] Respon ping diblokir. Mengizinkan respon Ping...
    netsh advfirewall firewall add rule name="%RULE_NAME%" protocol=icmpv4:8,any dir=in action=allow description="IT Support Ping Permit" >nul 2>&1
    netsh advfirewall firewall set rule name="File and Printer Sharing (Echo Request - ICMPv4-In)" new enable=Yes >nul 2>&1
    echo [SUKSES] Respon Ping LAN sekarang TELAH DIIZINKAN (ALLOW).
)

echo.
pause
