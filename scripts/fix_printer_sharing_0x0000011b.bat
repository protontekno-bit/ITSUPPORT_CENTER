@echo off
title Auto Fix Printer Sharing Error (0x0000011b & Point and Print)
color 0A

echo ========================================================
echo   AUTO FIX PRINTER SHARING ERROR (0x0000011b)
echo   + POINT AND PRINT DRIVER POLICY BYPASS
echo ========================================================
echo.

:: 1. Cek Hak Akses Administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [ERROR] Skrip ini membutuhkan hak akses Administrator!
    echo Silakan Klik Kanan file ini lalu pilih "Run as Administrator".
    echo.
    pause
    exit /b
)

echo [1/3] Menambahkan Registry RpcAuthnLevelPrivacyEnabled (Fix 0x0000011b)...
reg add "HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Print" /v RpcAuthnLevelPrivacyEnabled /t REG_DWORD /d 0 /f >nul
if %errorlevel% equ 0 (
    echo       [OK] Registry RPC Privacy berhasil dinonaktifkan.
) else (
    echo       [GAGAL] Gagal menambahkan registry Print RPC.
)

echo.
echo [2/3] Mengonfigurasi Point and Print Driver Policy (Bypass Client Driver Block)...
reg add "HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows NT\Printers\PointAndPrint" /v RestrictDriverInstallationToAdministrators /t REG_DWORD /d 0 /f >nul
if %errorlevel% equ 0 (
    echo       [OK] Policy instalasi driver printer client berhasil dibuka.
) else (
    echo       [WARNING] Gagal menambahkan policy PointAndPrint (Opsional).
)

echo.
echo [3/3] Merestart Layanan Print Spooler...
net stop spooler >nul 2>&1
timeout /t 2 /nobreak >nul
net start spooler >nul 2>&1
if %errorlevel% equ 0 (
    echo       [OK] Layanan Print Spooler berhasil di-restart.
) else (
    echo       [GAGAL] Gagal me-restart Print Spooler.
)

echo.
echo ========================================================
echo   SELESAI! Silakan coba hubungkan kembali printer dari
echo   komputer client.
echo ========================================================
echo.
pause
