@echo off
title Enable SMB 1.0 Support
echo ======================================================
echo  MENGAKTIFKAN PROTOKOL SMB 1.0 (LEGACY SUPPORT)
echo ======================================================
echo.

:: 1. Cek apakah dijalankan sebagai Administrator
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Akses Administrator terdeteksi.
) else (
    echo [ERROR] Script ini butuh akses Administrator!
    echo Mohon klik kanan file ini dan pilih 'Run as Administrator'.
    echo.
    pause
    exit
)

echo.
echo Sedang menginstal fitur SMB 1.0...
echo Proses ini mungkin memakan waktu 1-3 menit.
echo Mohon jangan tutup jendela ini.
echo.

:: 2. Menjalankan perintah DISM untuk mengaktifkan SMB1
dism /online /Enable-Feature /FeatureName:SMB1Protocol /All /NoRestart

if %errorlevel% equ 0 (
    echo.
    echo ======================================================
    echo [SUKSES] SMB 1.0 berhasil diaktifkan!
    echo ======================================================
    echo.
    echo PENTING: Komputer WAJIB di-restart agar fitur ini aktif.
    echo.
    
    :: 3. Opsi Restart Otomatis
    set /p choice="Apakah Anda ingin Restart komputer sekarang? (Y/N): "
    if /i "%choice%"=="Y" (
        echo Merestart komputer...
        shutdown /r /t 0
    ) else (
        echo Silakan restart komputer secara manual nanti.
    )
) else (
    echo.
    echo [GAGAL] Tidak dapat mengaktifkan fitur.
    echo Pastikan Windows Update service berjalan normal.
)

echo.
pause