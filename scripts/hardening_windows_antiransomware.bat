@echo off
color 0A
title Skrip Hardening Windows Anti-Ransomware
CLS

ECHO ========================================================
ECHO    SKRIP HARDENING WINDOWS (ANTI-RANSOMWARE)
ECHO ========================================================
ECHO.

:: --- CEK ADMIN ---
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [INFO] Menjalankan sebagai Administrator. Lanjut...
) else (
    echo [ERROR] Skrip ini butuh hak akses ADMINISTRATOR.
    echo Silakan Klik Kanan file ini lalu pilih "Run as Administrator".
    pause
    exit
)

ECHO.
ECHO [1/5] Mematikan SMBv1 (WannaCry Prevention)...
:: Menggunakan DISM untuk mematikan fitur
dism /online /Disable-Feature /FeatureName:SMB1Protocol /NoRestart >nul 2>&1
IF %ERRORLEVEL% EQU 0 (
    ECHO      [OK] SMBv1 berhasil dimatikan atau sudah mati.
) ELSE (
    ECHO      [INFO] Gagal atau fitur tidak ditemukan (Mungkin Windows 11 terbaru).
)

ECHO.
ECHO [2/5] Konfigurasi Firewall (Block Discovery & Sharing)...
:: Menggunakan Netsh untuk mematikan rule grup
netsh advfirewall firewall set rule group="Network Discovery" new enable=No >nul
netsh advfirewall firewall set rule group="File and Printer Sharing" new enable=No >nul
ECHO      [OK] Port SMB dan NetBIOS ditutup di Firewall.

ECHO.
ECHO [3/5] Mematikan Remote Desktop (RDP)...
:: Edit Registry untuk menutup RDP
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections /t REG_DWORD /d 1 /f >nul
ECHO      [OK] Remote Desktop (Port 3389) dimatikan.

ECHO.
ECHO [4/5] Mematikan AutoPlay & AutoRun (USB Security)...
:: Edit Registry untuk mematikan AutoPlay drive
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoDriveTypeAutoRun /t REG_DWORD /d 255 /f >nul
ECHO      [OK] AutoPlay dimatikan total.

ECHO.
ECHO [5/5] Mematikan Pencarian Device Otomatis...
:: Edit Registry untuk stop driver searching
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching" /v SearchOrderConfig /t REG_DWORD /d 0 /f >nul
ECHO      [OK] Auto install network devices dimatikan.

ECHO.
ECHO ========================================================
ECHO  PROSES SELESAI. KOMPUTER ANDA LEBIH AMAN SEKARANG.
ECHO ========================================================
ECHO Catatan: Sebaiknya RESTART komputer untuk memastikan efek SMBv1.
ECHO.
PAUSE