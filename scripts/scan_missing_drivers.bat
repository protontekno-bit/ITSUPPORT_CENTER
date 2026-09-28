@echo off
setlocal EnableDelayedExpansion
title Audit & Pindai Driver Hilang / Tanda Seru Kuning
color 0E

echo ==============================================================================
echo        🔍 AUDIT PERANGKAT & DRIVER BERMASALAH (DEVICE MANAGER SCAN)
echo ==============================================================================
echo.

echo [*] Memeriksa perangkat berstatus Error / Missing Driver di Windows...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$devices = Get-PnpDevice | Where-Object { $_.Status -eq 'Error' -or $_.ConfigManagerErrorCode -ne 0 }; " ^
    "if ($devices.Count -eq 0) { " ^
    "    Write-Host '✅ SEMPURNA: Tidak ditemukan driver yang hilang atau tanda seru kuning!' -ForegroundColor Green; " ^
    "} else { " ^
    "    Write-Host ('⚠️ Ditemukan ' + $devices.Count + ' perangkat yang membutuhkan perbaikan driver:') -ForegroundColor Yellow; " ^
    "    $devices | Format-Table -Property Status, Class, FriendlyName, InstanceId -AutoSize; " ^
    "}"

echo.
echo ==============================================================================
echo  TIPS: Jika ditemukan tanda seru, catat InstanceId/Hardware ID untuk mencari driver.
echo ==============================================================================
pause
