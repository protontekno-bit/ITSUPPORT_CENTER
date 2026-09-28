@echo off
title Pusat Manajemen ^& Utilitas RustDesk
color 0B
echo ============================================================================================
echo                    PUSAT MANAJEMEN ^& UTILITAS RUSTDESK (OPEN-SOURCE REMOTE)
echo ============================================================================================
echo.
echo  PILIH OPERASI RUSTDESK YANG INGIN DIJALANKAN:
echo  [1] Luncurkan RustDesk Client (Auto-Deteksi instalasi)
echo  [2] Reset ID, Kunci Enkripsi ^& Cache Sesi (Dapatkan ID Baru)
echo  [3] Konfigurasi Self-Hosted Server Kantor (ID/Relay Server ^& Key)
echo  [4] Pasang / Update RustDesk Versi Terbaru (via Winget)
echo  [5] Perbaiki ^& Restart Windows Service RustDesk
echo  [6] Buka Port Windows Firewall untuk RustDesk (TCP/UDP 21115-21119)
echo  [0] Batal / Kembali
echo.
set /p "rchoice= Masukkan pilihan [0-6]: "

if "%rchoice%"=="1" (
    echo.
    echo [*] Mencari instalasi RustDesk di komputer...
    if exist "%ProgramFiles%\RustDesk\rustdesk.exe" (
        start "" "%ProgramFiles%\RustDesk\rustdesk.exe"
        echo [OK] RustDesk berhasil dibuka.
    ) else if exist "%LocalAppData%\Programs\RustDesk\rustdesk.exe" (
        start "" "%LocalAppData%\Programs\RustDesk\rustdesk.exe"
        echo [OK] RustDesk berhasil dibuka.
    ) else (
        echo [!] RustDesk belum terpasang. Gunakan opsi [4] untuk memasangnya via Winget.
    )
) else if "%rchoice%"=="2" (
    echo.
    echo [*] Menghentikan proses dan service RustDesk...
    net stop rustdesk >nul 2>&1
    taskkill /f /im rustdesk.exe >nul 2>&1
    timeout /t 1 >nul
    echo [*] Menghapus file konfigurasi dan ID lama...
    del /q/f "%APPDATA%\RustDesk\config\*.*" >nul 2>&1
    del /q/f "%ProgramData%\RustDesk\config\*.*" >nul 2>&1
    del /q/f "C:\Windows\ServiceProfiles\LocalService\AppData\Roaming\RustDesk\config\*.*" >nul 2>&1
    echo [*] Memulai kembali service RustDesk...
    net start rustdesk >nul 2>&1
    echo [OK] ID ^& Kunci Enkripsi RustDesk berhasil di-reset! Silakan buka RustDesk untuk ID baru.
) else if "%rchoice%"=="3" (
    echo.
    set /p "rserver= Masukkan Alamat IP / Host Relay Server (contoh 192.168.1.100 atau relay.kantor.com): "
    if not "%rserver%"=="" (
        set /p "rkey= Masukkan Public Key Server (opsional): "
        if not exist "%APPDATA%\RustDesk\config" mkdir "%APPDATA%\RustDesk\config"
        echo custom-rendezvous-server = '%rserver%' >> "%APPDATA%\RustDesk\config\RustDesk2.toml"
        if not "%rkey%"=="" echo key = '%rkey%' >> "%APPDATA%\RustDesk\config\RustDesk2.toml"
        if exist "%ProgramFiles%\RustDesk\rustdesk.exe" (
            "%ProgramFiles%\RustDesk\rustdesk.exe" --server %rserver% --key %rkey% >nul 2>&1
        )
        echo [OK] Konfigurasi server RustDesk berhasil disimpan!
    )
) else if "%rchoice%"=="4" (
    echo.
    echo [*] Memasang / Memperbarui RustDesk via Winget...
    winget install RustDesk.RustDesk --silent --accept-source-agreements --accept-package-agreements
    echo [OK] Perintah instalasi Winget selesai dijalankan.
) else if "%rchoice%"=="5" (
    echo.
    echo [*] Memperbaiki service RustDesk...
    sc config rustdesk start= auto
    net stop rustdesk >nul 2>&1
    net start rustdesk
    echo [OK] Service RustDesk telah di-restart dan di-set ke Automatic.
) else if "%rchoice%"=="6" (
    echo.
    echo [*] Menambahkan aturan Firewall untuk RustDesk...
    netsh advfirewall firewall add rule name="IT_ALLOW_RUSTDESK_TCP" dir=in action=allow protocol=TCP localport=21115-21119 profile=any >nul 2>&1
    netsh advfirewall firewall add rule name="IT_ALLOW_RUSTDESK_UDP" dir=in action=allow protocol=UDP localport=21116 profile=any >nul 2>&1
    echo [OK] Port TCP/UDP RustDesk berhasil diizinkan di Windows Firewall!
)
echo.
