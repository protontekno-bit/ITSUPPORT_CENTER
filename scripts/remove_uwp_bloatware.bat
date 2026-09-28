@echo off
setlocal enabledelayedexpansion
title Pembersih Bloatware UWP Windows Kantor
color 0C

echo ===============================================================================
echo                PEMBERSIHAN BLOATWARE UWP APLIKASI KANTOR
echo ===============================================================================
echo.
echo [INFO] Menghapus paket bloatware non-produktif bawaan Windows:
echo   - Xbox App, GameBar, Gaming Services
echo   - Clipchamp Video Editor
echo   - Microsoft Solitaire Collection
echo   - MSN News, Weather
echo   - Feedback Hub, Get Help, Family Safety
echo   - Stub apps (TikTok, Disney+, Spotify)
echo.
echo [INFO] Aplikasi produktivitas seperti Calculator, Notepad, Paint, Snipping Tool
echo        TETAP AMAN DAN TIDAK DIHAPUS.
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$apps = @('*Xbox*', '*GamingApp*', '*Clipchamp*', '*MicrosoftSolitaireCollection*', '*BingNews*', '*BingWeather*', '*ZuneMusic*', '*ZuneVideo*', '*WindowsFeedbackHub*', '*GetHelp*', '*MicrosoftFamily*', '*Spotify*', '*Disney*', '*TikTok*'); foreach ($app in $apps) { Write-Host 'Menghapus paket: ' $app -ForegroundColor Cyan; Get-AppxPackage -AllUsers -Name $app | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue; Get-AppxProvisionedPackage -Online | Where-Object { $_.PackageName -like $app } | Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue }"

echo.
echo ===============================================================================
echo [SUKSES] Seluruh paket bloatware yang tidak diperlukan berhasil dibersihkan!
echo ===============================================================================
echo.
pause
