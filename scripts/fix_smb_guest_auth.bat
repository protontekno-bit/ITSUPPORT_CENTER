@echo off
title Fix Network Error 0x800704f8 (Enable Insecure Guest)
echo ======================================================
echo  SCRIPT PERBAIKAN AKSES SHARING (GUEST LOGON)
echo ======================================================
echo.

:: Cek apakah dijalankan sebagai Administrator
net session >nul 2>&1
if %errorLevel% == 0 (
    echo Menjalankan perintah Registry...
) else (
    echo ERROR: Script ini butuh akses Administrator!
    echo Mohon klik kanan file ini dan pilih 'Run as Administrator'.
    echo.
    pause
    exit
)

:: Mengubah Registry untuk mengizinkan Insecure Guest Auth
reg add "HKLM\SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters" /v AllowInsecureGuestAuth /t REG_DWORD /d 1 /f

if %errorlevel% equ 0 (
    echo.
    echo [SUKSES] Konfigurasi berhasil diterapkan!
    echo Anda sekarang bisa mencoba akses kembali ke folder network share Anda (contoh \\SERVER-SHARE atau \\192.168.1.100).
    echo.
    echo Catatan: Jika masih belum bisa, silakan Restart komputer.
) else (
    echo.
    echo [GAGAL] Terjadi kesalahan saat menulis registry.
)

echo.
pause