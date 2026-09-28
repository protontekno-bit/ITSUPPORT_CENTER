@echo off
setlocal enabledelayedexpansion

title Uji Konektivitas Port TCP & Latensi Soket (TCPing Lite)
cls
echo ==============================================================================
echo        🔌 PENGUJI PORT TCP & LATENSI SOKET JARINGAN (TCPING LITE)
echo ==============================================================================
echo Alat penguji keterbukaan port TCP spesifik (Web, DB, RDP, Mail, MikroTik)
echo Pengganti Telnet Client Windows dengan pengukuran respon latensi milidetik.
echo ==============================================================================
echo.

set "TARGET_HOST="
set /p "TARGET_HOST=Masukkan Alamat IP atau Domain Host [Contoh: 192.168.1.1]: "
if "%TARGET_HOST%"=="" set "TARGET_HOST=192.168.1.1"

echo.
echo Port Umum Populer:
echo   80   : HTTP Web Server       443  : HTTPS Web SSL
echo   3389 : Remote Desktop (RDP)  445  : SMB File Sharing
echo   1433 : Microsoft SQL Server  3306 : MySQL / MariaDB
echo   22   : SSH Secure Shell      8291 : MikroTik Winbox
echo.
set "TARGET_PORT="
set /p "TARGET_PORT=Masukkan Nomor Port TCP Target [Default: 80]: "
if "%TARGET_PORT%"=="" set "TARGET_PORT=80"

echo.
echo [*] Memulai 4x probe TCP SYN ke %TARGET_HOST%:%TARGET_PORT%...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$hostTarget = '%TARGET_HOST%';" ^
    "$port = [int]'%TARGET_PORT%';" ^
    "$success = 0; $totalMs = 0;" ^
    "for ($i=1; $i -le 4; $i++) {" ^
    "    $sw = [System.Diagnostics.Stopwatch]::StartNew();" ^
    "    $client = New-Object System.Net.Sockets.TcpClient;" ^
    "    $async = $client.BeginConnect($hostTarget, $port, $null, $null);" ^
    "    $wait = $async.AsyncWaitHandle.WaitOne(2500, $false);" ^
    "    $sw.Stop();" ^
    "    if ($wait -and $client.Connected) {" ^
    "        $client.EndConnect($async);" ^
    "        $ms = [Math]::Max(1, $sw.ElapsedMilliseconds);" ^
    "        $success++; $totalMs += $ms;" ^
    "        Write-Host ('Probe ' + $i + '/4: [TERBUKA / OPEN] Respon dalam ' + $ms + 'ms') -ForegroundColor Green;" ^
    "    } elseif (!$wait) {" ^
    "        Write-Host ('Probe ' + $i + '/4: [TIMEOUT / FILTERED] Tidak ada respon (2500ms) - Diblokir Firewall') -ForegroundColor Red;" ^
    "    } else {" ^
    "        Write-Host ('Probe ' + $i + '/4: [DITOLAK / REFUSED] Host hidup, tidak ada service listening') -ForegroundColor Yellow;" ^
    "    }" ^
    "    $client.Close();" ^
    "    Start-Sleep -Milliseconds 400;" ^
    "}" ^
    "Write-Host '';" ^
    "if ($success -gt 0) {" ^
    "    $avg = [Math]::Round($totalMs / $success);" ^
    "    Write-Host ('[+] Ringkasan: Port ' + $port + ' TERBUKA (' + $success + '/4 sukses) | Rata-rata: ' + $avg + 'ms') -ForegroundColor Cyan;" ^
    "} else {" ^
    "    Write-Host ('[-] Ringkasan: Port ' + $port + ' TERTUTUP / DIBLOKIR FIREWALL!') -ForegroundColor Red;" ^
    "}"

echo.
echo ==============================================================================
echo Pengujian port TCP selesai.
pause
