@echo off
setlocal enabledelayedexpansion

title Pemindai ARP Layer-2 & Deteksi Konflik IP LAN
cls
echo ==============================================================================
echo        📡 PEMINDAI ARP LAYER-2 & DETEKSI KONFLIK IP (LAN SWEEPER)
echo ==============================================================================
echo Memindai seluruh host aktif via protokol ARP (Layer 2) yang tidak bisa
echo diblokir oleh Windows Firewall / Antivirus endpoint, mendeteksi duplikasi IP/MAC.
echo ==============================================================================
echo.

:: Deteksi prefix subnet otomatis
for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /c:"IPv4 Address" /c:"Alamat IPv4" 2^>nul') do (
    set "RAW_IP=%%a"
    set "RAW_IP=!RAW_IP: =!"
    if not defined DETECTED_IP set "DETECTED_IP=!RAW_IP!"
)

if defined DETECTED_IP (
    for /f "tokens=1,2,3 delims=." %%a in ("%DETECTED_IP%") do set "AUTO_PREFIX=%%a.%%b.%%c"
) else (
    set "AUTO_PREFIX=192.168.1"
)

set "SUBNET_PREFIX="
set /p "SUBNET_PREFIX=Masukkan Awalan Subnet LAN [Default: %AUTO_PREFIX%]: "
if "%SUBNET_PREFIX%"=="" set "SUBNET_PREFIX=%AUTO_PREFIX%"

echo.
echo [*] Memulai pemindaian ARP 254 host (%SUBNET_PREFIX%.1 s/d %SUBNET_PREFIX%.254)...
echo [*] Melakukan inisiasi probe ARP Layer-2 secara paralel...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$prefix = '%SUBNET_PREFIX%';" ^
    "$signature = @'" ^
    "[DllImport(\"iphlpapi.dll\", ExactSpelling=true)] public static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int macAddrLen);" ^
    "'@;" ^
    "$type = Add-Type -MemberDefinition $signature -Name 'ArpHelper' -Namespace 'NetUtils' -PassThru;" ^
    "$jobs = 1..254 | ForEach-Object {" ^
    "    $ipStr = \"$prefix.$_\";" ^
    "    [PSCustomObject]@{ IP = $ipStr; Task = [System.Threading.Tasks.Task]::Run([Action]{" ^
    "        try {" ^
    "            $ip = [System.Net.IPAddress]::Parse($ipStr);" ^
    "            $bytes = $ip.GetAddressBytes();" ^
    "            $ipInt = [System.BitConverter]::ToInt32($bytes, 0);" ^
    "            $mac = New-Object byte[] 6;" ^
    "            $len = 6;" ^
    "            $res = [NetUtils.ArpHelper]::SendARP($ipInt, 0, $mac, [ref]$len);" ^
    "            if ($res -eq 0) {" ^
    "                $macStr = ($mac | ForEach-Object { $_.ToString('X2') }) -join ':';" ^
    "                [PSCustomObject]@{ IP = $ipStr; MAC = $macStr }" ^
    "            }" ^
    "        } catch {}" ^
    "    }) }" ^
    "};" ^
    "[System.Threading.Tasks.Task]::WaitAll($jobs.Task, 4000) | Out-Null;" ^
    "$live = @();" ^
    "arp -a | Select-String \"$prefix\.\" | ForEach-Object {" ^
    "    $line = $_.Line.Trim() -replace '\s+', ' ';" ^
    "    $parts = $line.Split(' ');" ^
    "    if ($parts.Length -ge 2 -and $parts[0] -match '^\d+\.\d+\.\d+\.\d+$') {" ^
    "        $live += [PSCustomObject]@{ IP = $parts[0]; MAC = $parts[1].ToUpper().Replace('-', ':') };" ^
    "    }" ^
    "};" ^
    "$unique = $live | Sort-Object -Property IP -Unique;" ^
    "Write-Host '=========================================================' -ForegroundColor Cyan;" ^
    "Write-Host ('DAFTAR HOST AKTIF DI SUBSET ' + $prefix + '.0/24 (Total: ' + $unique.Count + ' Host):') -ForegroundColor Cyan;" ^
    "Write-Host '=========================================================' -ForegroundColor Cyan;" ^
    "$unique | ForEach-Object { Write-Host ('• IP: ' + $_.IP.PadRight(15) + ' | MAC: ' + $_.MAC) -ForegroundColor Green; };" ^
    "Write-Host '';" ^
    "$dup = $unique | Group-Object -Property MAC | Where-Object { $_.Count -gt 1 };" ^
    "if ($dup) {" ^
    "    Write-Host '[!] PERINGATAN: Ditemukan duplikasi alamat MAC pada beberapa IP (Potensi Konflik/Spoofing)!' -ForegroundColor Yellow;" ^
    "    $dup | ForEach-Object { Write-Host ('    MAC: ' + $_.Name + ' dipakai oleh: ' + (($_.Group | ForEach-Object { $_.IP }) -join ', ')) -ForegroundColor Yellow; };" ^
    "} else {" ^
    "    Write-Host '[OK] Tidak terdeteksi konflik IP / duplikasi MAC di subnet ini.' -ForegroundColor Green;" ^
    "}"

echo.
echo ==============================================================================
echo Pemindaian selesai.
pause
