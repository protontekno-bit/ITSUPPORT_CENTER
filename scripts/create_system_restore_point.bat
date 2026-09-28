@echo off
setlocal enabledelayedexpansion
title Pembuat Titik Pemulihan Sistem (System Restore Point)
color 0A

echo ===============================================================================
echo            PEMBUAT TITIK PEMULIHAN SISTEM (SYSTEM RESTORE POINT)
echo ===============================================================================
echo.
echo [INFO] Mengaktifkan System Protection pada drive C:\ dan membuat Restore Point...
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "Enable-ComputerRestore -Drive 'C:\' -ErrorAction SilentlyContinue; Set-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore' -Name 'SystemRestorePointCreationFrequency' -Value 0 -Force -ErrorAction SilentlyContinue; Checkpoint-Computer -Description 'ITSupportCenter_AutoSnapshot' -RestorePointType 'MODIFY_SETTINGS' -ErrorAction Continue"

if %ERRORLEVEL% equ 0 (
    echo.
    echo ===============================================================================
    echo [SUKSES] Titik pemulihan sistem (System Restore Point) berhasil dibuat!
    echo ===============================================================================
) else (
    echo.
    echo [WARNING] Pembuatan restore point memerlukan hak Administrator dan layanan VSS aktif.
)

echo.
pause
