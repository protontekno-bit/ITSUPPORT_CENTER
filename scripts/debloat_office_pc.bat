@echo off
setlocal enabledelayedexpansion
title Debloater & Optimasi PC Kantor
color 0E

echo ===============================================================================
echo                     DEBLOATER & OPTIMASI PC KANTOR
echo ===============================================================================
echo.
echo [INFO] Menjalankan optimasi sistem:
echo   - Mematikan service telemetri background (DiagTrack & WAP Push)
echo   - Menonaktifkan pencarian web Bing di Start Menu (Start menu jadi cepat)
echo   - Menonaktifkan GameBar / GameDVR desktop
echo   - Menonaktifkan popup survey Windows feedback
echo.

echo [1/4] Menonaktifkan Service Telemetri...
sc stop DiagTrack >nul 2>&1
sc config DiagTrack start= disabled >nul 2>&1
sc stop dmwappushservice >nul 2>&1
sc config dmwappushservice start= disabled >nul 2>&1
echo   [OK] Service DiagTrack dinonaktifkan.

echo [2/4] Menerapkan Kebijakan Anti-Telemetri...
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v "AllowTelemetry" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v "MaxTelemetryAllowed" /t REG_DWORD /d 0 /f >nul 2>&1
echo   [OK] Kebijakan telemetri dinonaktifkan.

echo [3/4] Menonaktifkan Pencarian Web Bing di Start Menu...
reg add "HKCU\Software\Policies\Microsoft\Windows\Explorer" /v "DisableSearchBoxSuggestions" /t REG_DWORD /d 1 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v "BingSearchEnabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v "CortanaConsent" /t REG_DWORD /d 0 /f >nul 2>&1
echo   [OK] Pencarian Bing dinonaktifkan.

echo [4/4] Menonaktifkan GameBar & Popup Feedback...
reg add "HKCU\System\GameConfigStore" /v "GameDVR_Enabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v "AllowGameDVR" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR" /v "AppCaptureEnabled" /t REG_DWORD /d 0 /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Siuf\Rules" /v "NumberOfSIUFInPeriod" /t REG_DWORD /d 0 /f >nul 2>&1
echo   [OK] GameBar & Popup Feedback dinonaktifkan.

echo.
echo ===============================================================================
echo [SUKSES] Optimasi PC kantor selesai! Windows kini lebih ringan & responsif.
echo ===============================================================================
echo.
pause
