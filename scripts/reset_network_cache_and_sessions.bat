@echo off
title Reset Network Cache & Connections
color 0B
echo ===========================================
echo  MEMBERSIHKAN CACHE JARINGAN & SESI WINDOWS
echo ===========================================
echo.

:: 1. Cek Admin
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Harap Klik Kanan file ini dan pilih 'Run as Administrator'.
    pause
    exit /b
)

:: 2. Putuskan semua koneksi folder sharing yang nyangkut & purge session
echo [1/4] Menghapus mapped drive dan session ticket...
net use * /delete /y >nul 2>&1
klist purge >nul 2>&1

:: 3. Bersihkan Cache DNS dan NetBIOS
echo [2/4] Membersihkan cache DNS dan NetBIOS...
ipconfig /flushdns >nul 2>&1
nbtstat -R >nul 2>&1
nbtstat -RR >nul 2>&1

:: 4. Restart service Workstation (LanmanWorkstation)
echo [3/4] Merestart service Workstation (SMB Client)...
net stop workstation /y >nul 2>&1
net start workstation >nul 2>&1

echo.
echo [4/4] SELESAI! Semua cache jaringan dan sesi sharing telah di-reset.
echo.
pause