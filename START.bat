@echo off
setlocal enabledelayedexpansion

:: ==============================================================================
:: IT SUPPORT CENTER - SMART UNIVERSAL ZERO-FAIL BOOTSTRAPPER
:: Mendukung Local Drive (C:, D:) & Network Share / TrueNAS / Samba (UNC Path)
:: Otomatis meminta hak Administrator dan HANYA meluncurkan 1 antarmuka
:: ==============================================================================

:: Dukungan Network Share / UNC (\\192.168.x.x\share): Otomatis buat drive letter sementara
pushd "%~dp0" 2>nul
if %errorlevel% neq 0 cd /d "%~dp0" 2>nul

title IT Support Center 2026 v3.2.0 - Smart Launcher

:: 1. AUTO-ELEVATE ADMINISTRATOR (Dual-Layer: PowerShell + VBScript Fallback)
net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd.exe -ArgumentList '/c pushd \"%~dp0\" && \"%~nx0\"' -Verb RunAs" >nul 2>&1
    if %errorlevel% neq 0 (
        echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
        echo UAC.ShellExecute "cmd.exe", "/c pushd ""%~dp0"" && ""%~nx0""", "", "runas", 1 >> "%temp%\getadmin.vbs"
        "%temp%\getadmin.vbs"
        del "%temp%\getadmin.vbs" >nul 2>&1
    )
    popd 2>nul
    exit /b 0
)

:: 2. DETEKSI ARSITEKTUR OS (64-BIT vs 32-BIT)
set "IS_64BIT=0"
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" set "IS_64BIT=1"
if "%PROCESSOR_ARCHITEW6432%"=="AMD64" set "IS_64BIT=1"

:: 3. CARI LOKASI EXE TERBAIK & SINKRONISASI UPDATE
if exist "%~dp0dist\final\ITSupportCenter.exe" (
    copy /y "%~dp0dist\final\ITSupportCenter.exe" "%~dp0ITSupportCenter.exe" >nul 2>&1
)

set "SOURCE_EXE="
if "%IS_64BIT%"=="1" (
    if exist "%~dp0dist\final\ITSupportCenter.exe" (
        set "SOURCE_EXE=%~dp0dist\final\ITSupportCenter.exe"
    ) else if exist "%cd%\dist\final\ITSupportCenter.exe" (
        set "SOURCE_EXE=%cd%\dist\final\ITSupportCenter.exe"
    ) else if exist "%~dp0ITSupportCenter.exe" (
        set "SOURCE_EXE=%~dp0ITSupportCenter.exe"
    ) else if exist "%cd%\ITSupportCenter.exe" (
        set "SOURCE_EXE=%cd%\ITSupportCenter.exe"
    )
)

:: 4. JIKA EXE DITEMUKAN, LUNCURKAN DENGAN AUTO NETWORK-SAFE STAGING
if defined SOURCE_EXE (
    set "RUN_EXE=!SOURCE_EXE!"
    set "IS_NET=0"

    :: Cek apakah path UNC atau Network Drive
    if "%~dp0:~0,2%"=="\\" set "IS_NET=1"
    for /f "tokens=*" %%a in ('powershell -NoProfile -Command "(Get-Item -LiteralPath '!SOURCE_EXE!').FullName -like '\\*' -or ((Get-CimInstance Win32_LogicalDisk -Filter \"DeviceID='%~d0'\").DriveType -eq 4)" 2^>nul') do (
        if /i "%%a"=="True" set "IS_NET=1"
    )

    if "!IS_NET!"=="1" (
        set "LOCAL_DIR=%LOCALAPPDATA%\ITSupportCenter"
        if not exist "!LOCAL_DIR!" mkdir "!LOCAL_DIR!" >nul 2>&1
        set "RUN_EXE=!LOCAL_DIR!\ITSupportCenter.exe"

        set "NEED_COPY=1"
        if exist "!RUN_EXE!" (
            for %%A in ("!SOURCE_EXE!") do set "SRC_SZ=%%~zA"
            for %%B in ("!RUN_EXE!") do set "DST_SZ=%%~zB"
            if "!SRC_SZ!"=="!DST_SZ!" set "NEED_COPY=0"
        )
        if "!NEED_COPY!"=="1" (
            copy /y "!SOURCE_EXE!" "!RUN_EXE!" >nul 2>&1
        )
    )

    start "" "!RUN_EXE!"
    popd 2>nul
    exit /b 0
)

:: 5. FALLBACK KE CLI FAST MENU (Hanya berjalan jika EXE tidak ada / pada OS 32-Bit)
cls
call "IT_SUPPORT_CENTER.bat"
popd 2>nul
exit /b 0
