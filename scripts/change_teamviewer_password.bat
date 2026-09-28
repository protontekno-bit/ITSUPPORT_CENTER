@echo off
title Script Ganti Password TeamViewer (Mode Paksa)
color 0E

:: 1. Cek Administrator
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Akses Administrator Diterima.
) else (
    echo [ERROR] Klik kanan file ini -> "Run as Administrator".
    pause
    exit
)

echo.
echo ===============================================
echo   GANTI PASSWORD TEAMVIEWER (METODE KILL)
echo ===============================================
echo.
echo PENTING: Password harus KUAT (Huruf Besar + Kecil + Angka).
echo Jika password lemah, TeamViewer akan menolak diam-diam.
echo.

set /p NewPass="Masukkan Password Baru: "

:: Validasi input kosong
if "%NewPass%"=="" (
    echo Password tidak boleh kosong!
    pause
    exit
)

:: 2. Cari Lokasi TeamViewer
set "TVPath="
if exist "C:\Program Files\TeamViewer\TeamViewer.exe" (
    set "TVPath=C:\Program Files\TeamViewer"
) else if exist "C:\Program Files (x86)\TeamViewer\TeamViewer.exe" (
    set "TVPath=C:\Program Files (x86)\TeamViewer"
) else (
    echo [ERROR] TeamViewer tidak ditemukan.
    pause
    exit
)

:: 3. MATIKAN PAKSA Semua Proses TeamViewer
echo.
echo [1/4] Mematikan semua proses TeamViewer yang aktif...
taskkill /F /IM TeamViewer.exe >nul 2>&1
taskkill /F /IM TeamViewer_Service.exe >nul 2>&1
net stop teamviewer >nul 2>&1

:: Tunggu 3 detik agar file ter-unlock
timeout /t 3 >nul

:: 4. Eksekusi Ganti Password
echo.
echo [2/4] Menerapkan password baru...
cd /d "%TVPath%"

:: Menggunakan start /wait agar CMD menunggu proses selesai
start /wait "" TeamViewer.exe --passwd "%NewPass%"

:: Cek error level (meskipun TV sering return 0 walau gagal)
if %errorLevel% == 0 (
    echo [OK] Perintah dikirim.
) else (
    echo [WARNING] Ada masalah saat mengirim perintah.
)

:: 5. Restart Service
echo.
echo [3/4] Menghidupkan kembali Service TeamViewer...
net start teamviewer

echo.
echo [4/4] Membuka TeamViewer GUI (Opsional)...
start "" TeamViewer.exe

echo.
echo ===============================================
echo   SELESAI. Silakan coba login dari PC lain.
echo ===============================================
echo Catatan: Cek di Menu TeamViewer:
echo Extras -> Options -> Security -> "Password for unattended access"
echo ===============================================
pause