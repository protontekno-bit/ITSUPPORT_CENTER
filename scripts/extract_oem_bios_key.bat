@echo off
setlocal EnableDelayedExpansion
title Ekstrak Kunci Lisensi OEM Asli dari BIOS Motherboard
color 0B

echo ==============================================================================
echo        🏷️ EKSTRAKSI KUNCI LISENSI OEM ASLI DARI BIOS (MSDM TABLE)
echo ==============================================================================
echo.

echo [*] Membaca tabel ACPI MSDM di motherboard...
powershell -Command "$key = (Get-CimInstance -ClassName SoftwareLicensingService).OA3xOriginalProductKey; if ($key) { Write-Host ('[SUKSES] OEM Product Key: ' + $key) -ForegroundColor Green; Set-Clipboard -Value $key; Write-Host 'Key berhasil disalin ke Clipboard!' -ForegroundColor Yellow } else { Write-Host '[INFO] Tidak ditemukan kunci OEM di BIOS perangkat ini.' -ForegroundColor Red }"

echo.
echo ==============================================================================
pause
