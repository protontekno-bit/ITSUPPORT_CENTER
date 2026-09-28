@echo off
setlocal EnableDelayedExpansion
title Pertolongan Pertama Microsoft Office & Outlook (Rescue CLI)
color 0E
cls

echo ======================================================================
echo          PERTOLONGAN PERTAMA MICROSOFT OFFICE ^& OUTLOOK (RESCUE)
echo ======================================================================
echo.
echo  1 = Buka Microsoft Word (Safe Mode /safe)
echo  2 = Buka Microsoft Excel (Safe Mode /safe)
echo  3 = Buka Microsoft Outlook (Safe Mode /safe)
echo  4 = Perbaiki Startup Outlook (Reset Navigation Pane /resetnavpane)
echo  5 = Cari ^& Luncurkan SCANPST.EXE (Inbox Repair Tool Outlook)
echo  6 = Reset Template Word Korup (Normal.dotm) ^& Excel (.xlb)
echo  7 = Bersihkan Cache Dokumen Macet (OfficeFileCache ^& Kill Zombie)
echo  8 = Picu Microsoft Office Click-to-Run Quick Repair
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "OFFICE_OPT="
set /p "OFFICE_OPT=Pilih Nomor Operasi [0-8]: "

if "%OFFICE_OPT%"=="1" (
    echo [*] Membuka Word dalam Safe Mode...
    start winword.exe /safe
    exit /b 0
)

if "%OFFICE_OPT%"=="2" (
    echo [*] Membuka Excel dalam Safe Mode...
    start excel.exe /safe
    exit /b 0
)

if "%OFFICE_OPT%"=="3" (
    echo [*] Membuka Outlook dalam Safe Mode...
    start outlook.exe /safe
    exit /b 0
)

if "%OFFICE_OPT%"=="4" (
    echo [*] Memperbaiki Navigation Pane Outlook...
    taskkill /f /im outlook.exe >nul 2>&1
    start outlook.exe /resetnavpane
    echo [OK] Outlook diluncurkan dengan reset views / navpane.
    pause
    exit /b 0
)

if "%OFFICE_OPT%"=="5" (
    echo [*] Mencari lokasi scanpst.exe...
    set "SCANPST_FOUND="
    for %%P in (
        "C:\Program Files\Microsoft Office\root\Office16\SCANPST.EXE"
        "C:\Program Files (x86)\Microsoft Office\root\Office16\SCANPST.EXE"
        "C:\Program Files\Microsoft Office\Office16\SCANPST.EXE"
        "C:\Program Files (x86)\Microsoft Office\Office16\SCANPST.EXE"
        "C:\Program Files\Microsoft Office\Office15\SCANPST.EXE"
        "C:\Program Files (x86)\Microsoft Office\Office15\SCANPST.EXE"
    ) do (
        if exist %%P (
            set "SCANPST_FOUND=%%~P"
            goto :LaunchScanPst
        )
    )
    echo [!] SCANPST.EXE tidak ditemukan di lokasi standar.
    pause
    exit /b 1

    :LaunchScanPst
    echo [OK] Ditemukan: !SCANPST_FOUND!
    start "" "!SCANPST_FOUND!"
    exit /b 0
)

if "%OFFICE_OPT%"=="6" (
    echo [*] Me-reset template Normal.dotm Word...
    taskkill /f /im winword.exe >nul 2>&1
    if exist "%APPDATA%\Microsoft\Templates\Normal.dotm" (
        ren "%APPDATA%\Microsoft\Templates\Normal.dotm" "Normal.dotm.bak_%random%"
        echo [OK] Normal.dotm berhasil di-rename/reset!
    ) else (
        echo [INFO] Normal.dotm tidak ditemukan.
    )
    del /f /q "%APPDATA%\Microsoft\Excel\*.xlb" >nul 2>&1
    echo [OK] Cache toolbar Excel berhasil dibersihkan.
    pause
    exit /b 0
)

if "%OFFICE_OPT%"=="7" (
    echo [*] Menghentikan proses Office dan membersihkan cache...
    taskkill /f /im msosync.exe >nul 2>&1
    taskkill /f /im winword.exe >nul 2>&1
    taskkill /f /im excel.exe >nul 2>&1
    taskkill /f /im outlook.exe >nul 2>&1
    rd /s /q "%LOCALAPPDATA%\Microsoft\Office\16.0\OfficeFileCache" >nul 2>&1
    rd /s /q "%LOCALAPPDATA%\Microsoft\Office\15.0\OfficeFileCache" >nul 2>&1
    echo [OK] OfficeFileCache berhasil dibersihkan!
    pause
    exit /b 0
)

if "%OFFICE_OPT%"=="8" (
    echo [*] Memulai Click-to-Run Quick Repair...
    set "C2R_EXE=C:\Program Files\Common Files\microsoft shared\ClickToRun\OfficeClickToRun.exe"
    if not exist "!C2R_EXE!" set "C2R_EXE=C:\Program Files (x86)\Common Files\microsoft shared\ClickToRun\OfficeClickToRun.exe"
    if exist "!C2R_EXE!" (
        start "" "!C2R_EXE!" scenario=QuickRepair platform=x64 culture=en-us ForceAppShutdown=True
        echo [OK] Jendela perbaikan Microsoft Office telah dibuka.
    ) else (
        echo [!] OfficeClickToRun.exe tidak ditemukan.
    )
    pause
    exit /b 0
)

exit /b 0
