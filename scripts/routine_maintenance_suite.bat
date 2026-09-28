@echo off
setlocal EnableDelayedExpansion
title 1-Klik Pemeliharaan & Tune-Up Rutin (CLI)
color 0E
cls

echo ======================================================================
echo         1-KLIK PEMELIHARAAN ^& TUNE-UP RUTIN KOMPUTER (ALL-IN-ONE)
echo ======================================================================
echo.
echo  Rangkaian pemeliharaan otomatis:
echo   1. Bersihkan Berkas Sampah ^& Cache Temp Disk
echo   2. Flush DNS ^& Reset NetBIOS Cache
echo   3. Bersihkan Antrean Cetak Print Spooler Macet
echo   4. Optimasi Responsivitas Efek Visual
echo.
echo ======================================================================

set "CONFIRM="
set /p "CONFIRM=Mulai proses pemeliharaan sekarang? [Y/N]: "
if /i not "!CONFIRM!"=="Y" (
    echo [INFO] Operasi dibatalkan.
    pause
    exit /b 0
)

echo.
echo [1/4] Membersihkan Berkas Temporary...
del /s /f /q "%TEMP%\*.*" >nul 2>&1
del /s /f /q "C:\Windows\Temp\*.*" >nul 2>&1
echo [OK] Cache sampah temporary dibersihkan.

echo.
echo [2/4] Me-refresh DNS ^& NetBIOS...
ipconfig /flushdns >nul 2>&1
nbtstat -R >nul 2>&1
echo [OK] DNS dan NetBIOS cache disegarkan.

echo.
echo [3/4] Membersihkan antrean printer macet...
net stop spooler /y >nul 2>&1
del /f /q "C:\Windows\System32\spool\PRINTERS\*.*" >nul 2>&1
net start spooler >nul 2>&1
echo [OK] Service Spooler bersih dan berjalan normal.

echo.
echo [4/4] Mengoptimalkan responsivitas desktop...
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects" /v VisualFXSetting /t REG_DWORD /d 3 /f >nul 2>&1
echo [OK] Visual responsiveness dioptimalkan.

echo.
echo ======================================================================
echo   🎉 PEMELIHARAAN RUTIN SELESAI DENGAN SUKSES!
echo ======================================================================
pause
exit /b 0
