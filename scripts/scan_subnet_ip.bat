@echo off
setlocal EnableDelayedExpansion
title Pemindai Subnet IP Jaringan Lokal (Ping Sweep)
color 0B

echo ==============================================================================
echo        📡 PEMINDAI RENTANG IP SUBSET LAN (PING SWEEP 1-254)
echo ==============================================================================
echo.

set "DEFAULT_PREFIX=192.168.1"
for /f "tokens=4" %%a in ('route print 0.0.0.0 ^| findstr "0.0.0.0" ^| findstr /v "Default"') do (
    for /f "tokens=1,2,3 delims=." %%b in ("%%a") do set "DEFAULT_PREFIX=%%b.%%c.%%d"
)

echo Subnet terdeteksi: %DEFAULT_PREFIX%
set /p "PREFIX=Masukkan 3 blok Subnet [contoh 192.168.1] (Tekan ENTER untuk default): "
if "%PREFIX%"=="" set "PREFIX=%DEFAULT_PREFIX%"

echo.
echo [*] Memulai pemindaian pada subnet %PREFIX%.1 - %PREFIX%.254...
echo [*] Mohon tunggu beberapa detik...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "1..254 | ForEach-Object -Parallel { " ^
    "    $ip = '%PREFIX%.' + $_; " ^
    "    if (Test-Connection -ComputerName $ip -Count 1 -Quiet -TimeoutSeconds 1) { " ^
    "        try { $hostName = [System.Net.Dns]::GetHostEntry($ip).HostName } catch { $hostName = 'Unknown' }; " ^
    "        Write-Host ('• LIVE IP: ' + $ip.PadRight(15) + ' | Hostname: ' + $hostName) -ForegroundColor Green " ^
    "    } " ^
    "} -ThrottleLimit 50"

echo.
echo ==============================================================================
echo  Pemindaian selesai.
echo ==============================================================================
pause
