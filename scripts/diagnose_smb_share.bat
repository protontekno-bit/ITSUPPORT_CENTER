@echo off
setlocal
title Network Share Diagnostic Tool
color 0f

echo ========================================================
echo          DIAGNOSA KONEKSI FILE SHARING (SMB)
echo ========================================================
echo.

set "TARGET=192.168.1.100"
set /p "USER_INPUT=Masukkan IP Server Target [Default: 192.168.1.100]: "
if not "%USER_INPUT%"=="" set "TARGET=%USER_INPUT%"

echo.
echo [*] Melakukan diagnosa ke target: \\%TARGET%
echo.

:: ---------------------------------------------------
:: 1. CEK KONEKSI FISIK (PING)
:: ---------------------------------------------------
echo [1/4] Mengecek Ping ke %TARGET%...
ping -n 2 %TARGET% >nul
if %errorlevel% == 0 (
    echo    [OK] IP Terdeteksi (Reply received).
) else (
    echo    [GAGAL] IP tidak merespon. 
    echo    SOLUSI: Cek kabel LAN/WiFi atau Firewall di server tujuan.
    goto :end
)
echo.

:: ---------------------------------------------------
:: 2. CEK PORT SMB (445)
:: ---------------------------------------------------
echo [2/4] Mengecek Port Sharing (TCP 445)...
powershell -Command "if ((Test-NetConnection -ComputerName %TARGET% -Port 445 -WarningAction SilentlyContinue).TcpTestSucceeded) { exit 0 } else { exit 1 }"
if %errorlevel% == 0 (
    echo    [OK] Port 445 Terbuka (Service Sharing jalan).
) else (
    echo    [GAGAL] Port 445 Tertutup.
    echo    SOLUSI: Cek Firewall di %TARGET% atau pastikan service SMB server nyala.
    goto :end
)
echo.

:: ---------------------------------------------------
:: 3. CEK KONFIGURASI WINDOWS CLIENT (PC INI)
:: ---------------------------------------------------
echo [3/4] Mengecek Settingan Windows Kamu...

:: Cek SMB 1.0 Feature
echo    - Memeriksa status fitur SMB 1.0...
dism /online /Get-FeatureInfo /FeatureName:SMB1Protocol | find "State : Enabled" >nul
if %errorlevel% == 0 (
    echo      [INFO] SMB 1.0: AKTIF (Bagus untuk perangkat jadul).
) else (
    echo      [INFO] SMB 1.0: NON-AKTIF (Default Windows 10/11).
    echo      *Jika server tujuan adalah alat tua (NAS lama/WinXP), ini harus AKTIF.
)

:: Cek Insecure Guest Logon
echo    - Memeriksa status Insecure Guest Logon...
reg query HKLM\SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters /v AllowInsecureGuestAuth 2>nul | find "0x1" >nul
if %errorlevel% == 0 (
    echo      [INFO] Insecure Guest: DIPERBOLEHKAN (Settingan sudah benar).
) else (
    echo      [INFO] Insecure Guest: DIBLOKIR.
    echo      *Jika server tidak pakai password, ini mungkin penyebab error.
)
echo.

:: ---------------------------------------------------
:: 4. TES KONEKSI LANGSUNG (NET VIEW)
:: ---------------------------------------------------
echo [4/4] Mencoba melihat daftar folder...
echo    Sedang menghubungi server... (bisa memakan waktu 10-20 detik)
echo.
net view \\%TARGET%
echo.

if %errorlevel% == 0 (
    color 0A
    echo ========================================================
    echo  HASIL: KONEKSI SUKSES!
    echo ========================================================
    echo  Windows berhasil melihat folder sharing.
    echo  Silakan buka File Explorer dan ketik \\%TARGET%
) else (
    color 0C
    echo ========================================================
    echo  HASIL: KONEKSI GAGAL (Lihat Pesan Error di atas)
    echo ========================================================
    echo.
    echo  ANALISA ERROR UMUM:
    echo  - Error 5 (Access Denied): Butuh username/password, atau Guest dimatikan di server.
    echo  - Error 53 (Network Path Not Found): Masalah DNS atau Port terblokir.
    echo  - Error 2182 (Service not started): Server tidak siap.
    echo  - System error 1231: Masalah driver jaringan/lokal komputer ini.
)

:end
echo.
pause