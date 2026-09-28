@echo off
setlocal enabledelayedexpansion
title Inspeksi Total Spesifikasi & Kesehatan Hardware
color 0B

echo ===============================================================================
echo        INSPEKSI TOTAL SPESIFIKASI & KESEHATAN HARDWARE (DEEP AUDIT)
echo ===============================================================================
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "Write-Host '[ 🖥️ SISTEM OPERASI & UPTIME ]' -ForegroundColor Cyan; " ^
  "$os = Get-CimInstance Win32_OperatingSystem; $bTime = $os.LastBootUpTime; $uptime = (Get-Date) - $bTime; " ^
  "Write-Host '  OS Name    :' $os.Caption '(' $os.OSArchitecture ') Build' $os.BuildNumber; " ^
  "Write-Host '  Uptime     :' $uptime.Days 'Hari,' $uptime.Hours 'Jam,' $uptime.Minutes 'Menit'; " ^
  "Write-Host ''; " ^
  "Write-Host '[ 🧠 PROCESSOR (CPU) ]' -ForegroundColor Cyan; " ^
  "$cpu = Get-CimInstance Win32_Processor; " ^
  "Write-Host '  Model CPU  :' $cpu.Name.Trim(); " ^
  "Write-Host '  Core/Thread:' $cpu.NumberOfCores 'Cores /' $cpu.NumberOfLogicalProcessors 'Threads @' $cpu.MaxClockSpeed 'MHz'; " ^
  "Write-Host '  Virtualize :' $cpu.VirtualizationFirmwareEnabled; " ^
  "Write-Host ''; " ^
  "Write-Host '[ 🧩 RAM FISIK PER-SLOT ]' -ForegroundColor Cyan; " ^
  "$rams = Get-CimInstance Win32_PhysicalMemory; $totalRam = 0; $i = 1; " ^
  "foreach ($r in $rams) { $gb = [math]::Round($r.Capacity / 1GB, 1); $totalRam += $r.Capacity; Write-Host ('  Slot ' + $i + ' (' + $r.DeviceLocator + ') : ' + $gb + ' GB @ ' + $r.Speed + ' MHz | ' + $r.Manufacturer.Trim() + ' (' + $r.PartNumber.Trim() + ')'); $i++ }; " ^
  "Write-Host '  Total RAM  :' ([math]::Round($totalRam / 1GB, 1)) 'GB Terpasang (' ($i - 1) 'Slot Terisi)'; " ^
  "Write-Host ''; " ^
  "Write-Host '[ 🖲️ MOTHERBOARD, BIOS & TPM ]' -ForegroundColor Cyan; " ^
  "$mb = Get-CimInstance Win32_BaseBoard; Write-Host '  Mainboard  :' $mb.Manufacturer $mb.Product '(Serial:' $mb.SerialNumber ')'; " ^
  "$bios = Get-CimInstance Win32_BIOS; Write-Host '  BIOS Versi :' $bios.SMBIOSBIOSVersion '(Rilis:' $bios.ReleaseDate ')'; " ^
  "$secBoot = (Get-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\SecureBoot\State' -ErrorAction SilentlyContinue).UEFISecureBootEnabled; " ^
  "Write-Host '  SecureBoot :' $(if ($secBoot -eq 1) {'Aktif (UEFI Secure Boot)'} else {'Tidak Aktif / Legacy'}); " ^
  "Write-Host ''; " ^
  "Write-Host '[ 💾 PENYIMPANAN FISIK & S.M.A.R.T ]' -ForegroundColor Cyan; " ^
  "$disks = Get-CimInstance Win32_DiskDrive; $dIdx = 0; " ^
  "foreach ($d in $disks) { $gb = [math]::Round($d.Size / 1GB, 0); Write-Host ('  Disk ' + $dIdx + ' : ' + $d.Model.Trim() + ' (' + $gb + ' GB, ' + $d.InterfaceType + ') | SMART: [' + $d.Status + ']'); $dIdx++ }; " ^
  "Write-Host ''; " ^
  "Write-Host '[ 🎮 GPU & DISPLAY ]' -ForegroundColor Cyan; " ^
  "$gpus = Get-CimInstance Win32_VideoController; " ^
  "foreach ($g in $gpus) { $vram = [math]::Round($g.AdapterRAM / 1GB, 1); Write-Host ('  GPU Model  : ' + $g.Name.Trim() + ' (' + $vram + ' GB VRAM) | Res: ' + $g.CurrentHorizontalResolution + 'x' + $g.CurrentVerticalResolution + ' @ ' + $g.CurrentRefreshRate + 'Hz') }; " ^
  "Write-Host ''; " ^
  "Write-Host '[ 🔋 KONDISI BATERAI ]' -ForegroundColor Cyan; " ^
  "$bat = Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue; " ^
  "if ($bat) { $hlth = [math]::Round(($bat.FullChargeCapacity / $bat.DesignCapacity) * 100, 1); Write-Host '  Battery    :' $bat.EstimatedChargeRemaining '% | Health:' $hlth '% (Design:' $bat.DesignCapacity 'mWh, Full:' $bat.FullChargeCapacity 'mWh)' } else { Write-Host '  Battery    : Desktop / No Battery Detected' }; "

echo.
echo ===============================================================================
echo [SELESAI] Seluruh data spesifikasi & kesehatan fisik telah diaudit!
echo ===============================================================================
echo.
pause
