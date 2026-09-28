@echo off
title Windows 11 LTSC KMS Activator
color 0B

:: Cek Administrator
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Skrip aktivasi ini membutuhkan hak akses Administrator!
    echo Silakan Klik Kanan file ini dan pilih "Run as Administrator".
    pause
    exit /b
)

echo ========================================================
echo       AKTIVASI WINDOWS 11 IoT ENTERPRISE LTSC (KMS)
echo ========================================================
echo.
echo [1/4] Mereset konfigurasi lisensi lama...
cscript //nologo %windir%\system32\slmgr.vbs /rilc >nul 2>&1
cscript //nologo %windir%\system32\slmgr.vbs /upk >nul 2>&1
cscript //nologo %windir%\system32\slmgr.vbs /ckms >nul 2>&1
cscript //nologo %windir%\system32\slmgr.vbs /cpky >nul 2>&1

echo [2/4] Memasang Generic Volume License Key (GVLK)...
cscript //nologo %windir%\system32\slmgr.vbs /ipk M7XTQ-FN8P6-TTKYV-9D4CC-J462D

echo [3/4] Menghubungkan ke Server KMS (kms.digiboy.ir)...
cscript //nologo %windir%\system32\slmgr.vbs /skms kms.digiboy.ir

echo [4/4] Mengaktifkan Windows...
cscript //nologo %windir%\system32\slmgr.vbs /ato

echo.
echo ========================================================
echo Selesai. Silakan periksa status aktivasi di Pengaturan Windows.
echo ========================================================
pause