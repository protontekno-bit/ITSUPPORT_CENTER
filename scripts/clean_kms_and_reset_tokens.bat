@echo off
setlocal EnableDelayedExpansion
title Pembersihan Residu KMS & Reset Token Lisensi
color 0C

echo ==============================================================================
echo        🧹 PEMBERSIHAN RESIDU SERVER KMS & RESET TOKEN LISENSI
echo ==============================================================================
echo.

echo [1/4] Menghapus settingan server KMS eksternal di registry...
cscript //nologo %windir%\system32\slmgr.vbs /ckms
cscript //nologo %windir%\system32\slmgr.vbs /clearrearm

echo [2/4] Menghentikan Software Protection Service (sppsvc)...
net stop sppsvc /y >nul 2>&1

echo [3/4] Mereset tokens.dat...
set "TOKEN_PATH=%windir%\System32\spp\store\2.0\tokens.dat"
if exist "%TOKEN_PATH%" (
    ren "%TOKEN_PATH%" "tokens.bak_%random%" >nul 2>&1
    echo [OK] Cache tokens.dat di-reset.
)

echo [4/4] Menjalankan kembali sppsvc & Menginstal ulang file lisensi sistem...
net start sppsvc >nul 2>&1
cscript //nologo %windir%\system32\slmgr.vbs /rilc

echo.
echo ==============================================================================
echo [SUKSES] Pembersihan KMS dan reset token selesai.
echo ==============================================================================
pause
