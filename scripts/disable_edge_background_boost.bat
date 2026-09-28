@echo off
setlocal enabledelayedexpansion
title Matikan Edge Background & Startup Boost
color 09

echo ===============================================================================
echo            MENONAKTIFKAN EDGE BACKGROUND & STARTUP BOOST
echo ===============================================================================
echo.
echo [INFO] Menerapkan Registry Policy Microsoft Edge...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v "StartupBoostEnabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v "BackgroundModeEnabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Edge" /v "WebWidgetIsEnabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Policies\Microsoft\Edge" /v "StartupBoostEnabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Policies\Microsoft\Edge" /v "BackgroundModeEnabled" /t REG_DWORD /d 0 /f >nul 2>&1

echo [INFO] Menutup proses latar belakang msedge.exe yang sedang berjalan...
taskkill /f /im msedge.exe >nul 2>&1

echo.
echo ===============================================================================
echo [SUKSES] Microsoft Edge kini tidak akan berjalan di background saat idle!
echo ===============================================================================
echo.
pause
