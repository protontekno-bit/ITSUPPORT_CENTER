@echo off
setlocal enabledelayedexpansion
title Optimasi Efek Visual & Kecepatan UI
color 0D

echo ===============================================================================
echo            OPTIMASI EFEK VISUAL & KECEPATAN ANTARMUKA WINDOWS
echo ===============================================================================
echo.
echo [INFO] Menyetel performa visual cepat kantor (Tanpa animasi lag):
echo   - Matikan animasi jendela & tooltip
echo   - Pertahankan ClearType Font Smoothing (Teks tetap tajam)
echo   - Pertahankan bayangan nama icon desktop
echo.

reg add "HKCU\Control Panel\Desktop\WindowMetrics" /v "MinAnimate" /t REG_SZ /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects" /v "VisualFXSetting" /t REG_DWORD /d 3 /f >nul 2>&1
reg add "HKCU\Control Panel\Desktop" /v "FontSmoothing" /t REG_SZ /d 2 /f >nul 2>&1
reg add "HKCU\Control Panel\Desktop" /v "FontSmoothingType" /t REG_DWORD /d 2 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ListviewShadow" /t REG_DWORD /d 1 /f >nul 2>&1

echo [INFO] Me-restart Windows Explorer untuk menerapkan perubahan...
taskkill /f /im explorer.exe >nul 2>&1
timeout /t 1 /nobreak >nul
start explorer.exe

echo.
echo ===============================================================================
echo [SUKSES] Efek visual Windows berhasil dioptimalkan untuk responsivitas maksimal!
echo ===============================================================================
echo.
pause
