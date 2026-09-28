@echo off
setlocal
cd /d "%~dp0"
title PUSH REPOSITORI KE GITHUB (ITSUPPORT_CENTER)
cls
echo ==============================================================================
echo        🚀 PUSH REPOSITORI IT SUPPORT CENTER KE GITHUB
echo ==============================================================================
echo Remote : https://github.com/protontekno-bit/ITSUPPORT_CENTER.git
echo Branch : main
echo ==============================================================================
echo.

git push -u origin main

echo.
echo ==============================================================================
if %errorlevel% equ 0 (
    echo [SUKSES] Seluruh kode dan dokumentasi berhasil di-push ke GitHub!
) else (
    echo [GAGAL] Terjadi kendala saat push. Pastikan izin write/token sudah benar.
)
echo ==============================================================================
echo.
pause
