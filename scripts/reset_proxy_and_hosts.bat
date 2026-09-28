@echo off
setlocal EnableDelayedExpansion
title Reset Proxy Browser WinINet & File Hosts Default
color 0E

echo ==============================================================================
echo        🧹 RESET PROXY BROWSER (WININET) & PEMULIHAN FILE HOSTS DEFAULT
echo ==============================================================================
echo.

echo [1/4] Menonaktifkan Proxy di Registry Internet Settings...
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings" /v ProxyEnable /t REG_DWORD /d 0 /f >nul 2>&1
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings" /v ProxyServer /f >nul 2>&1
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Internet Settings" /v AutoConfigURL /f >nul 2>&1

echo [2/4] Mereset WinHTTP System Proxy...
netsh winhttp reset proxy

echo [3/4] Memulihkan file hosts ke standar bersih...
set "HOSTS_PATH=%windir%\System32\drivers\etc\hosts"
if exist "%HOSTS_PATH%" copy /y "%HOSTS_PATH%" "%HOSTS_PATH%.bak" >nul 2>&1

(
echo # Standard Clean Windows Hosts File
echo 127.0.0.1       localhost
echo ::1             localhost
) > "%HOSTS_PATH%"

echo [4/4] Membersihkan Cache DNS & ARP...
ipconfig /flushdns >nul 2>&1
netsh interface ip delete arpcache >nul 2>&1

echo.
echo ==============================================================================
echo [SUKSES] Konfigurasi Proxy dan file hosts berhasil dinormalisasi!
echo ==============================================================================
pause
