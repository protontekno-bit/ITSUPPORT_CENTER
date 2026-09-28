@echo off
setlocal EnableDelayedExpansion
title Manajemen & Reset Password User Windows
color 0C

echo ==============================================================================
echo        👤 MANAJEMEN AKUN & RESET PASSWORD USER WINDOWS (LUPA PASSWORD)
echo ==============================================================================
echo.
echo  Pilih Operasi Akun Pengguna:
echo   1. 📋 Audit Seluruh Daftar Akun User Lokal (net user)
echo   2. 🔑 Reset / Atur Password Baru Akun (net user <user> <pass>)
echo   3. 🔓 Buka Kunci Akun yang Terkunci / Nonaktif (Unlock / Active)
echo   4. 📶 Ekstrak Seluruh Password WiFi yang Tersimpan (key=clear)
echo   5. ⚙️ Buka GUI Manajemen User Windows (lusrmgr.msc)
echo.
set /p "OP_CHOICE=Masukkan nomor pilihan [1-5]: "

if "%OP_CHOICE%"=="1" (
    echo.
    echo [*] Daftar akun pengguna di komputer ini:
    net user
    echo.
    set /p "TARGET_USER=Ketik nama user untuk melihat detail lengkap: "
    if not "!TARGET_USER!"=="" net user "!TARGET_USER!"
)
if "%OP_CHOICE%"=="2" (
    echo.
    echo [*] DAFTAR AKUN USER AKTIF:
    net user
    echo.
    set /p "TARGET_USER=Masukkan nama username yang ingin di-reset password: "
    set /p "NEW_PASS=Masukkan password baru (kosongkan jika tanpa password): "
    if not "!TARGET_USER!"=="" (
        net user "!TARGET_USER!" "!NEW_PASS!"
        net user "!TARGET_USER!" /active:yes
        echo.
        echo [SUKSES] Password user '!TARGET_USER!' berhasil diubah!
    )
)
if "%OP_CHOICE%"=="3" (
    echo.
    set /p "TARGET_USER=Masukkan nama username yang terkunci: "
    if not "!TARGET_USER!"=="" (
        net user "!TARGET_USER!" /active:yes
        net user "!TARGET_USER!" /lockout:no
        echo.
        echo [SUKSES] Akun '!TARGET_USER!' berhasil dibuka (Unlocked)!
    )
)
if "%OP_CHOICE%"=="4" (
    echo.
    echo [*] Mengekstrak seluruh profil WiFi dan Password yang tersimpan:
    powershell -Command "netsh wlan show profiles | Select-String 'All User Profile|Profil Semua Pengguna' | ForEach-Object { $name = ($_.Line -split ':')[1].Trim(); $pass = (netsh wlan show profile name=\"$name\" key=clear | Select-String 'Key Content|Konten Kunci'); if ($pass) { $p = ($pass -split ':')[1].Trim() } else { $p = 'Open/None' }; Write-Host ('• SSID: ' + $name.PadRight(25) + ' | Password: ' + $p) -ForegroundColor Green }"
)
if "%OP_CHOICE%"=="5" (
    echo [*] Membuka lusrmgr.msc...
    start lusrmgr.msc
)

echo.
echo ==============================================================================
pause
