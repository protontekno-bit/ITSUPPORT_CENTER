@echo off
setlocal EnableDelayedExpansion
title Pusat Manajemen & Aktivator Windows RDP (mstsc)
color 0B
cls

echo ======================================================================
echo          PUSAT MANAJEMEN WINDOWS REMOTE DESKTOP (RDP)
echo ======================================================================
echo.

:: 1. Cek status registry RDP
set "RDP_STATUS=Mati"
for /f "tokens=3" %%A in ('reg query "HKLM\System\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections 2^>nul ^| findstr "fDenyTSConnections"') do (
    if "%%A"=="0x0" set "RDP_STATUS=Aktif (Bisa Di-Remote)"
    if "%%A"=="0" set "RDP_STATUS=Aktif (Bisa Di-Remote)"
)

:: 2. Cek port RDP
set "RDP_PORT=3389"
for /f "tokens=3" %%A in ('reg query "HKLM\System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp" /v PortNumber 2^>nul ^| findstr "PortNumber"') do (
    set /a RDP_PORT=%%A
)

echo [STATUS RDP] Status Host : %RDP_STATUS%
echo [STATUS RDP] Port RDP    : TCP %RDP_PORT%
echo.
echo  1 = [1-Klik] Aktifkan Penuh RDP Host (Aktivasi Service + Buka Firewall)
echo  2 = [1-Klik] Matikan Total RDP Host (Tutup Port & Amankan Komputer)
echo  3 = Saklar NLA (Matikan / Nyalakan Network Level Authentication)
echo  4 = Ganti Port Standar RDP (Ubah 3389 ke Port Custom + Auto Firewall)
echo  5 = Quick Connect Klien RDP (Ketik IP Target -^> Langsung Buka mstsc)
echo  6 = RDP Remote Shadowing (Pantau / Bimbing Layar User Tanpa Logout)
echo  7 = Audit Sesi Aktif & Tendang Sesi Nyangkut (qwinsta / rwinsta)
echo  8 = Buka Pengaturan Remote Desktop Klasik (System Properties)
echo  0 = Batal / Kembali
echo.
echo ======================================================================

set "RDP_CHOICE="
set /p "RDP_CHOICE=Masukkan Pilihan Anda [0-8]: "

if "%RDP_CHOICE%"=="1" (
    echo.
    echo [*] Mengaktifkan Windows Remote Desktop...
    reg add "HKLM\System\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections /t REG_DWORD /d 0 /f >nul
    sc config TermService start= auto >nul 2>&1
    net start TermService >nul 2>&1
    netsh advfirewall firewall set rule group="remote desktop" new enable=Yes >nul 2>&1
    netsh advfirewall firewall add rule name="Windows Remote Desktop (TCP-3389)" dir=in action=allow protocol=TCP localport=3389 >nul 2>&1
    echo [OK] Windows Remote Desktop (RDP) berhasil DIAKTIFKAN penuh!
    pause
    exit /b 0
)

if "%RDP_CHOICE%"=="2" (
    echo.
    echo [*] Mematikan Windows Remote Desktop...
    reg add "HKLM\System\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections /t REG_DWORD /d 1 /f >nul
    netsh advfirewall firewall set rule group="remote desktop" new enable=No >nul 2>&1
    echo [OK] Windows Remote Desktop Host telah DIMATIKAN dan diamankan.
    pause
    exit /b 0
)

if "%RDP_CHOICE%"=="3" (
    echo.
    echo [*] Mengubah setelan Network Level Authentication (NLA)...
    for /f "tokens=3" %%A in ('reg query "HKLM\System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp" /v UserAuthentication 2^>nul ^| findstr "UserAuthentication"') do (
        if "%%A"=="0x1" (
            reg add "HKLM\System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp" /v UserAuthentication /t REG_DWORD /d 0 /f >nul
            echo [OK] NLA dinonaktifkan (Kompatibel dengan semua jenis klien).
            pause
            exit /b 0
        )
    )
    reg add "HKLM\System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp" /v UserAuthentication /t REG_DWORD /d 1 /f >nul
    echo [OK] NLA diaktifkan kembali (Keamanan diperketat).
    pause
    exit /b 0
)

if "%RDP_CHOICE%"=="4" (
    echo.
    set /p "NEW_PORT=Masukkan Nomor Port RDP Baru [misal: 33890]: "
    if defined NEW_PORT (
        reg add "HKLM\System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp" /v PortNumber /t REG_DWORD /d !NEW_PORT! /f >nul
        netsh advfirewall firewall add rule name="Custom RDP Port (TCP-!NEW_PORT!)" dir=in action=allow protocol=TCP localport=!NEW_PORT! >nul 2>&1
        echo [OK] Port RDP berhasil diubah ke !NEW_PORT! dan Firewall telah dibuka.
        echo [INFO] Restart PC atau restart TermService agar port baru aktif.
    )
    pause
    exit /b 0
)

if "%RDP_CHOICE%"=="5" (
    echo.
    set /p "TARGET_IP=Masukkan IP atau Hostname Target: "
    if defined TARGET_IP (
        echo Meluncurkan mstsc.exe /v:!TARGET_IP!...
        start mstsc.exe /v:!TARGET_IP! /f
    )
    exit /b 0
)

if "%RDP_CHOICE%"=="6" (
    echo.
    echo [*] Sesi pengguna aktif saat ini:
    qwinsta
    echo.
    set /p "SESS_ID=Masukkan ID Sesi yang ingin di-shadow [misal: 1]: "
    if defined SESS_ID (
        reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services" /v Shadow /t REG_DWORD /d 1 /f >nul 2>&1
        echo Meluncurkan RDP Shadowing ke Sesi !SESS_ID!...
        start mstsc.exe /shadow:!SESS_ID! /control
    )
    exit /b 0
)

if "%RDP_CHOICE%"=="7" (
    echo.
    echo [*] Audit sesi aktif saat ini:
    qwinsta
    echo.
    set /p "RESET_ID=Masukkan ID Sesi yang ingin diputus [Kosongkan jika hanya cek]: "
    if defined RESET_ID (
        rwinsta !RESET_ID!
        echo [OK] Sesi !RESET_ID! berhasil diputus.
    )
    pause
    exit /b 0
)

if "%RDP_CHOICE%"=="8" (
    start SystemPropertiesRemote.exe
    exit /b 0
)

exit /b 0
