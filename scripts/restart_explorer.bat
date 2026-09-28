@echo off
setlocal EnableDelayedExpansion
title Restart Windows Explorer Shell & Taskbar
color 0A

echo ==============================================================================
echo        🔄 RESTART WINDOWS EXPLORER & TASKBAR SHELL
echo ==============================================================================
echo.

echo [*] Menghentikan proses explorer.exe...
taskkill /f /im explorer.exe >nul 2>&1

timeout /t 2 /nobreak >nul

echo [*] Menjalankan kembali Windows Explorer...
start explorer.exe

echo.
echo ==============================================================================
echo [SUKSES] Windows Explorer berhasil dimuat ulang!
echo ==============================================================================
timeout /t 2 >nul
