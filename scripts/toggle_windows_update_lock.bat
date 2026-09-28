@echo off
setlocal enabledelayedexpansion
title Saklar Pengatur Windows Update (Kunci / Buka Permanen)
color 0E

echo ===============================================================================
echo            SAKLAR PENGATUR STATUS WINDOWS UPDATE (KUNCI / BUKA PERMANEN)
echo ===============================================================================
echo.
echo Pilih Aksi:
echo   [1] Kunci ^& Matikan Windows Update Secara Permanen (Stop Semua Update Otomatis)
echo   [2] Buka Kunci ^& Kembalikan Windows Update ke Normal (Aktifkan Kembali)
echo   [0] Batal / Keluar
echo.

set "ACT="
set /p "ACT=Masukkan Pilihan Anda [0-2]: "

if "%ACT%"=="1" goto LOCK_UPDATE
if "%ACT%"=="2" goto UNLOCK_UPDATE
exit /b

:LOCK_UPDATE
echo.
echo [INFO] Mengunci dan mematikan layanan Windows Update...
net stop wuauserv /y >nul 2>&1
sc config wuauserv start= disabled >nul 2>&1
net stop WaaSMedicSvc /y >nul 2>&1
sc config WaaSMedicSvc start= disabled >nul 2>&1
net stop UsoSvc /y >nul 2>&1
sc config UsoSvc start= disabled >nul 2>&1
net stop bits /y >nul 2>&1
sc config bits start= disabled >nul 2>&1

echo [INFO] Menerapkan Registry Group Policy NoAutoUpdate...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU" /v "NoAutoUpdate" /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU" /v "AUOptions" /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate" /v "DisableWindowsUpdateAccess" /t REG_DWORD /d 1 /f >nul 2>&1

echo.
echo ===============================================================================
echo [SUKSES] Windows Update telah DIKUNCI ^& DIMATIKAN SECARA PERMANEN!
echo Komputer tidak akan lagi mengunduh atau memasang update otomatis.
echo ===============================================================================
echo.
pause
exit /b

:UNLOCK_UPDATE
echo.
echo [INFO] Menghapus kebijakan penguncian Registry...
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU" /v "NoAutoUpdate" /f >nul 2>&1
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU" /v "AUOptions" /f >nul 2>&1
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate" /v "DisableWindowsUpdateAccess" /f >nul 2>&1

echo [INFO] Mengaktifkan kembali layanan Windows Update...
sc config wuauserv start= demand >nul 2>&1
sc config bits start= delayed-auto >nul 2>&1
sc config UsoSvc start= demand >nul 2>&1
sc config WaaSMedicSvc start= demand >nul 2>&1
net start wuauserv >nul 2>&1
net start bits >nul 2>&1

echo.
echo ===============================================================================
echo [SUKSES] Windows Update telah DIBUKA KEMBALI dan BERFUNGSI NORMAL!
echo ===============================================================================
echo.
pause
exit /b
