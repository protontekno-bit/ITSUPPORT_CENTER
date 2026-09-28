@echo off
color 4F
title SKRIP FIREWALL BLOCK RANSOMWARE PORTS
CLS

ECHO ========================================================
ECHO    MEMBLOKIR PORT RENTAN (FIREWALL HARDENING)
ECHO ========================================================
ECHO Skrip ini akan menutup Port Inbound (Masuk) berikut:
ECHO [1] TCP 445  (SMB / File Sharing - Jalur Utama Virus)
ECHO [2] TCP 139  (NetBIOS - Jalur Lama)
ECHO [3] TCP 3389 (RDP - Remote Desktop)
ECHO [4] TCP 23   (Telnet - Protokol Tua/Tidak Aman)
ECHO [5] TCP 21   (FTP - Transfer File Biasa)
ECHO.
ECHO PERINGATAN: 
ECHO - Anda tidak bisa lagi Sharing Folder/Printer dari PC ini.
ECHO - PC ini tidak bisa di-remote (RDP) dari PC lain.
ECHO.

:: --- CEK ADMIN ---
net session >nul 2>&1
if %errorLevel% == 0 (
    echo [INFO] Akses Administrator DITERIMA.
) else (
    echo [ERROR] Harap Klik Kanan -> Run as Administrator!
    pause
    exit
)

ECHO.
ECHO --------------------------------------------------------
ECHO SEDANG MEMPROSES BLOKIR PORT...
ECHO --------------------------------------------------------

:: 1. Blokir SMB (445)
netsh advfirewall firewall add rule name="[BLOCK_RANSOMWARE] SMB 445" dir=in action=block protocol=TCP localport=445
ECHO [OK] Port 445 (SMB) berhasil DIBLOKIR.

:: 2. Blokir NetBIOS (139)
netsh advfirewall firewall add rule name="[BLOCK_RANSOMWARE] NetBIOS 139" dir=in action=block protocol=TCP localport=139
ECHO [OK] Port 139 (NetBIOS) berhasil DIBLOKIR.

:: 3. Blokir RDP (3389)
netsh advfirewall firewall add rule name="[BLOCK_RANSOMWARE] RDP 3389" dir=in action=block protocol=TCP localport=3389
ECHO [OK] Port 3389 (RDP) berhasil DIBLOKIR.

:: 4. Blokir Telnet (23)
netsh advfirewall firewall add rule name="[BLOCK_RANSOMWARE] Telnet 23" dir=in action=block protocol=TCP localport=23
ECHO [OK] Port 23 (Telnet) berhasil DIBLOKIR.

:: 5. Blokir FTP (21)
:: FTP seringkali mengirim password tanpa enkripsi, sebaiknya ditutup jika tidak dipakai server
netsh advfirewall firewall add rule name="[BLOCK_RANSOMWARE] FTP 21" dir=in action=block protocol=TCP localport=21
ECHO [OK] Port 21 (FTP) berhasil DIBLOKIR.

ECHO.
ECHO --------------------------------------------------------
ECHO  PROSES SELESAI. KOMPUTER INI SUDAH DI-BENTENGI.
ECHO --------------------------------------------------------
ECHO Cara Tes: Jalankan skrip Audit Python dari PC lain.
ECHO IP PC ini seharusnya tidak lagi muncul MERAH.
ECHO.
PAUSE