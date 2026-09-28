@echo off
setlocal EnableDelayedExpansion
title Pengatur Safe Mode & Perbaikan Parameter Booting (BCD / MSConfig)
color 0B

echo ==============================================================================
echo        🛡️ PENGATUR SAFE MODE & PERBAIKAN PARAMETER BOOTING (BCD)
echo ==============================================================================
echo.
echo  Pilih Konfigurasi Booting:
echo   1. 🌐 Setel Masuk Safe Mode dengan Jaringan (Networking)
echo   2. 🛡️ Setel Masuk Safe Mode Minimal (Standar)
echo   3. 🔄 Kembalikan ke Booting Normal (Hapus Safe Mode)
echo   4. ⚡ Reset Limit CPU Core & RAM (Perbaikan Salah Setting msconfig)
echo   5. ⌨️ Aktifkan Tombol F8 Boot Menu Klasik
echo   6. ⚙️ Buka GUI System Configuration (msconfig.exe)
echo.
set /p "BOOT_OP=Masukkan nomor pilihan [1-6]: "

if "%BOOT_OP%"=="1" (
    echo [*] Mengatur Safe Mode with Networking...
    bcdedit /set {current} safeboot network
    echo [SUKSES] Komputer akan masuk Safe Mode Jaringan saat restart.
)
if "%BOOT_OP%"=="2" (
    echo [*] Mengatur Safe Mode Minimal...
    bcdedit /set {current} safeboot minimal
    echo [SUKSES] Komputer akan masuk Safe Mode Minimal saat restart.
)
if "%BOOT_OP%"=="3" (
    echo [*] Menghapus parameter Safe Mode...
    bcdedit /deletevalue {current} safeboot >nul 2>&1
    bcdedit /deletevalue {default} safeboot >nul 2>&1
    echo [SUKSES] Komputer akan boot Normal saat restart.
)
if "%BOOT_OP%"=="4" (
    echo [*] Mereset limit prosesor dan memory di BCD...
    bcdedit /deletevalue {current} numproc >nul 2>&1
    bcdedit /deletevalue {current} truncatememory >nul 2>&1
    bcdedit /deletevalue {current} removememory >nul 2>&1
    echo [SUKSES] Limit CPU & RAM dinormalkan.
)
if "%BOOT_OP%"=="5" (
    echo [*] Mengaktifkan F8 Boot Menu...
    bcdedit /set {default} bootmenupolicy legacy
    echo [SUKSES] Tombol F8 aktif saat booting.
)
if "%BOOT_OP%"=="6" (
    start msconfig.exe
)

echo.
echo ==============================================================================
pause
