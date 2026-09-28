@echo off
setlocal EnableDelayedExpansion
title Kunci / Buka Update Microsoft Office Otomatis
color 0C
cls

echo ======================================================================
echo       PENGATUR STATUS UPDATE MICROSOFT OFFICE (CLICK-TO-RUN)
echo ======================================================================
echo.

:: Cek status saat ini di ClickToRun Configuration
set "OFFICE_UPD_STATUS=Aktif (Normal)"
for /f "tokens=3" %%A in ('reg query "HKLM\SOFTWARE\Microsoft\Office\ClickToRun\Configuration" /v UpdatesEnabled 2^>nul ^| findstr "UpdatesEnabled"') do (
    if /i "%%A"=="False" set "OFFICE_UPD_STATUS=Terkunci (Mati Permanen)"
)

echo [STATUS SAAT INI] Update Microsoft Office: %OFFICE_UPD_STATUS%
echo.
echo  1 = [KUNCI] Matikan Update Otomatis Office Permanen (Registry + Tasks)
echo  2 = [BUKA]  Normalkan Kembali Update Otomatis Office
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "OFFICE_CHOICE="
set /p "OFFICE_CHOICE=Masukkan Pilihan Anda [0-2]: "

if "%OFFICE_CHOICE%"=="1" (
    echo.
    echo [*] Menerapkan penguncian total update Microsoft Office...
    reg add "HKLM\SOFTWARE\Microsoft\Office\ClickToRun\Configuration" /v UpdatesEnabled /t REG_SZ /d "False" /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate" /v EnableAutomaticUpdates /t REG_DWORD /d 0 /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate" /v HideEnableDisableUpdates /t REG_DWORD /d 1 /f >nul 2>&1
    reg add "HKLM\SOFTWARE\Policies\Microsoft\Office\15.0\Common\OfficeUpdate" /v EnableAutomaticUpdates /t REG_DWORD /d 0 /f >nul 2>&1
    schtasks /change /tn "\Microsoft\Office\Office Automatic Updates 2.0" /disable >nul 2>&1
    schtasks /change /tn "\Microsoft\Office\Office Feature Updates" /disable >nul 2>&1
    schtasks /change /tn "\Microsoft\Office\Office Feature Updates Logon" /disable >nul 2>&1
    echo [OK] Update Otomatis Microsoft Office berhasil DIMATIKAN PERMANEN!
    echo [INFO] Lisensi KMS/Volume dan macro Excel kini terlindungi dari update mendadak.
    pause
    exit /b 0
)

if "%OFFICE_CHOICE%"=="2" (
    echo.
    echo [*] Membuka kunci dan menormalkan kembali update Office...
    reg add "HKLM\SOFTWARE\Microsoft\Office\ClickToRun\Configuration" /v UpdatesEnabled /t REG_SZ /d "True" /f >nul 2>&1
    reg delete "HKLM\SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate" /v EnableAutomaticUpdates /f >nul 2>&1
    reg delete "HKLM\SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate" /v HideEnableDisableUpdates /f >nul 2>&1
    reg delete "HKLM\SOFTWARE\Policies\Microsoft\Office\15.0\Common\OfficeUpdate" /v EnableAutomaticUpdates /f >nul 2>&1
    schtasks /change /tn "\Microsoft\Office\Office Automatic Updates 2.0" /enable >nul 2>&1
    echo [OK] Update Otomatis Microsoft Office telah DINORMALKAN KEMBALI.
    pause
    exit /b 0
)

exit /b 0
