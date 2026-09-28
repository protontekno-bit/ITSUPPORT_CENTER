@echo off
setlocal EnableDelayedExpansion
title Pusat Alat Diagnosa & Administrasi Windows (Admin Hub)
color 0B

echo ==============================================================================
echo        🎛️ PUSAT ALAT DIAGNOSA & ADMINISTRASI WINDOWS (ADMIN HUB)
echo ==============================================================================
echo.
echo  Pilih Perkakas Administrasi Windows:
echo   1. 📊 Resource Monitor (resmon.exe)
echo   2. 📋 Event Viewer - Crash Logs (eventvwr.msc)
echo   3. 🖥️ Computer Management (compmgmt.msc)
echo   4. 🔌 Device Manager (devmgmt.msc)
echo   5. 💾 Disk Management (diskmgmt.msc)
echo   6. ⚙️ Windows Services (services.msc)
echo   7. 🕒 System Restore (rstrui.exe)
echo   8. 🔑 Registry Editor (regedit.exe)
echo   9. 📈 Performance Monitor (perfmon.msc)
echo   10. ℹ️ System Information (msinfo32.exe)
echo.
set /p "HUB_OP=Masukkan nomor pilihan [1-10]: "

if "%HUB_OP%"=="1" start resmon.exe
if "%HUB_OP%"=="2" start eventvwr.msc
if "%HUB_OP%"=="3" start compmgmt.msc
if "%HUB_OP%"=="4" start devmgmt.msc
if "%HUB_OP%"=="5" start diskmgmt.msc
if "%HUB_OP%"=="6" start services.msc
if "%HUB_OP%"=="7" start rstrui.exe
if "%HUB_OP%"=="8" start regedit.exe
if "%HUB_OP%"=="9" start perfmon.msc
if "%HUB_OP%"=="10" start msinfo32.exe

echo.
echo ==============================================================================
pause
