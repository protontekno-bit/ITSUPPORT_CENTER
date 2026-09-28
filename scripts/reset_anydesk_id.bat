@echo off
setlocal
title AnyDesk Hard Reset Tool

echo ==================================================
echo      SCRIPT RESET ID ANYDESK (WINDOWS)
echo ==================================================
echo.

:: 1. Cek Hak Akses Administrator
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Administrator rights terdeteksi.
) else (
    echo [ERROR] Script ini butuh akses Administrator.
    echo Mohon klik kanan file ini dan pilih "Run as Administrator".
    pause
    exit
)

echo.
echo [1/4] Menghentikan semua proses AnyDesk...
:: Mematikan task aplikasi
taskkill /F /IM AnyDesk.exe /T >nul 2>&1
:: Mematikan service (mencoba beberapa kemungkinan nama service)
net stop "AnyDesk Service" >nul 2>&1
net stop "AnyDesk" >nul 2>&1

timeout /t 2 /nobreak >nul

echo.
echo [2/4] Memproses file konfigurasi System (ID Utama)...
IF EXIST "%programdata%\AnyDesk\service.conf" (
    :: Membuat backup file lama menjadi service.conf.bak
    ren "%programdata%\AnyDesk\service.conf" "service.conf.bak_%random%"
    echo      - File service.conf lama telah di-backup dan di-reset.
) ELSE (
    echo      - File service.conf tidak ditemukan (mungkin sudah terhapus).
)

IF EXIST "%programdata%\AnyDesk\system.conf" (
    del /f /q "%programdata%\AnyDesk\system.conf"
    echo      - File system.conf dihapus.
)

echo.
echo [3/4] Membersihkan cache dan setting User (AppData)...
:: Bagian ini menghapus history koneksi dan preferensi user
IF EXIST "%appdata%\AnyDesk" (
    rd /s /q "%appdata%\AnyDesk"
    echo      - Folder AppData AnyDesk berhasil dibersihkan.
) ELSE (
    echo      - Folder AppData tidak ditemukan.
)

echo.
echo [4/4] Restart Service AnyDesk...
:: Mencoba menyalakan service kembali jika terinstall
net start "AnyDesk Service" >nul 2>&1
net start "AnyDesk" >nul 2>&1

echo.
echo ==================================================
echo PROSES SELESAI!
echo ==================================================
echo Silakan buka aplikasi AnyDesk sekarang. 
echo Anda seharusnya sudah mendapatkan ID baru.
echo.
pause