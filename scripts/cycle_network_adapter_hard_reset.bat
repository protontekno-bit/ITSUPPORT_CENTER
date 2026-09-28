@echo off
setlocal enabledelayedexpansion

title Siklus Restart Keras Hardware Kartu Jaringan (NIC Power-Cycle)
cls
echo ==============================================================================
echo        ⚡ SIKLUS RESTART KERAS HARDWARE KARTU JARINGAN (NIC POWER-CYCLE)
echo ==============================================================================
echo Mematikan dan menyalakan kembali driver kartu jaringan fisik (LAN/Wi-Fi)
echo secara paksa via PowerShell NetAdapter API & PnP Bus Rescan untuk mengatasi
echo adapter macet, error driver, atau koneksi no internet tanpa reboot PC.
echo ==============================================================================
echo.

echo Daftar Adapter Jaringan Fisik Terdeteksi:
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "Get-NetAdapter | Where-Object { $_.HardwareInterface -eq $true } | Format-Table -Property Name, InterfaceDescription, Status, LinkSpeed -AutoSize"

echo.
set "TARGET_NIC="
set /p "TARGET_NIC=Masukkan Nama Interface (Contoh: Ethernet / Wi-Fi atau tekan ENTER untuk SEMUA): "

echo.
if "%TARGET_NIC%"=="" (
    echo [*] Memulai restart seluruh kartu jaringan fisik...
    powershell -NoProfile -ExecutionPolicy Bypass -Command ^
        "Get-NetAdapter | Where-Object { $_.HardwareInterface -eq $true } | ForEach-Object {" ^
        "    Write-Host ('[*] Merestart: ' + $_.Name + ' (' + $_.InterfaceDescription + ')...') -ForegroundColor Yellow;" ^
        "    Restart-NetAdapter -Name $_.Name -Confirm:$false -ErrorAction SilentlyContinue;" ^
        "}"
) else (
    echo [*] Merestart interface: '%TARGET_NIC%'...
    powershell -NoProfile -ExecutionPolicy Bypass -Command ^
        "Restart-NetAdapter -Name '%TARGET_NIC%' -Confirm:$false -ErrorAction SilentlyContinue; if (!$?) { netsh interface set interface '%TARGET_NIC%' disable; Start-Sleep 2; netsh interface set interface '%TARGET_NIC%' enable }"
)

echo.
echo [*] Melakukan PnP Bus Rescan (pnputil /scan-devices)...
pnputil /scan-devices >nul 2>&1

echo [*] Mengosongkan DNS Cache (ipconfig /flushdns)...
ipconfig /flushdns >nul 2>&1

echo [*] Memperbarui Alamat IP DHCP (ipconfig /renew)...
ipconfig /renew >nul 2>&1

echo.
echo [*] Menguji koneksi internet pasca-restart...
ping -n 3 8.8.8.8

echo.
echo ==============================================================================
echo Siklus restart kartu jaringan selesai.
pause
