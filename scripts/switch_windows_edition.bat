@echo off
setlocal EnableDelayedExpansion
title Upgrade / Ganti Edisi Windows (Home ke Pro Tanpa Format)
color 0B

echo ==============================================================================
echo        📈 UPGRADE / GANTI EDISI WINDOWS (CHANGEPK GENERIC UPGRADE)
echo ==============================================================================
echo.
echo  Pilih Target Upgrade Edisi:
echo   1. ⭐ Upgrade ke Windows 10/11 Pro (VK7JG-NPHTM-C97JM-9MPGT-3V66T)
echo   2. 🏢 Upgrade ke Windows 10/11 Enterprise (NPPR9-FWDCX-D2C8J-H872K-2YT43)
echo   3. ✏️ Masukkan Product Key Kustom
echo.
set /p "ED_CHOICE=Masukkan pilihan [1-3]: "

if "%ED_CHOICE%"=="1" set "GEN_KEY=VK7JG-NPHTM-C97JM-9MPGT-3V66T"
if "%ED_CHOICE%"=="2" set "GEN_KEY=NPPR9-FWDCX-D2C8J-H872K-2YT43"
if "%ED_CHOICE%"=="3" (
    set /p "GEN_KEY=Masukkan 25-digit Product Key: "
)

if not "%GEN_KEY%"=="" (
    echo.
    echo [*] Memicu upgrade edisi dengan Product Key: %GEN_KEY%...
    changepk.exe /ProductKey %GEN_KEY%
    echo.
    echo [INFO] Proses verifikasi selesai. Silakan cek di Settings > System > Activation.
)

echo.
echo ==============================================================================
pause
