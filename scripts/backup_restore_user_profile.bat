@echo off
setlocal EnableDelayedExpansion
title Penyelamatan & Migrasi Data Profil User (CLI)
color 0B
cls

echo ======================================================================
echo         PENYELAMATAN ^& MIGRASI DATA PROFIL PENGGUNA (CLI)
echo ======================================================================
echo.
echo  1 = Cadangkan Profil User Aktif (Desktop, Dokumen, Download, Bookmarks)
echo  2 = Pulihkan Data dari Folder Cadangan ke Komputer Ini
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "USER_CHOICE="
set /p "USER_CHOICE=Pilih Nomor Operasi [0-2]: "

if "%USER_CHOICE%"=="1" (
    echo.
    set "DEST_DIR="
    set "DEF_DEST=D:\BACKUP_USER_%USERNAME%_%date:~10,4%%date:~4,2%%date:~7,2%"
    if not exist "D:\" set "DEF_DEST=%TEMP%\BACKUP_USER_%USERNAME%"
    
    echo Target folder default: !DEF_DEST!
    set /p "DEST_DIR=Masukkan lokasi folder tujuan cadangan (Enter untuk default): "
    if "!DEST_DIR!"=="" set "DEST_DIR=!DEF_DEST!"

    echo [*] Membuat folder cadangan di: !DEST_DIR!
    mkdir "!DEST_DIR!" 2>nul

    echo [*] Mencadangkan Desktop...
    robocopy "%USERPROFILE%\Desktop" "!DEST_DIR!\Desktop" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL /XJ >nul 2>&1
    echo [*] Mencadangkan Documents...
    robocopy "%USERPROFILE%\Documents" "!DEST_DIR!\Documents" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL /XJ >nul 2>&1
    echo [*] Mencadangkan Downloads...
    robocopy "%USERPROFILE%\Downloads" "!DEST_DIR!\Downloads" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL /XJ >nul 2>&1
    echo [*] Mencadangkan Pictures...
    robocopy "%USERPROFILE%\Pictures" "!DEST_DIR!\Pictures" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL /XJ >nul 2>&1

    echo [*] Mencadangkan Bookmarks Chrome ^& Edge...
    mkdir "!DEST_DIR!\Browser_Data\Chrome" 2>nul
    mkdir "!DEST_DIR!\Browser_Data\Edge" 2>nul
    copy /y "%LOCALAPPDATA%\Google\Chrome\User Data\Default\Bookmarks" "!DEST_DIR!\Browser_Data\Chrome\" >nul 2>&1
    copy /y "%LOCALAPPDATA%\Microsoft\Edge\User Data\Default\Bookmarks" "!DEST_DIR!\Browser_Data\Edge\" >nul 2>&1

    echo [*] Mencadangkan Signatures Outlook...
    if exist "%APPDATA%\Microsoft\Signatures" (
        robocopy "%APPDATA%\Microsoft\Signatures" "!DEST_DIR!\Outlook_Signatures" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL >nul 2>&1
    )

    echo.
    echo [OK] Pencadangan data profil pengguna selesai!
    echo [INFO] Disimpan di: !DEST_DIR!
    pause
    exit /b 0
)

if "%USER_CHOICE%"=="2" (
    echo.
    set "SRC_DIR="
    set /p "SRC_DIR=Masukkan path lengkap folder cadangan: "
    if "!SRC_DIR!"=="" (
        echo [!] Path sumber kosong.
        pause
        exit /b 1
    )

    echo [*] Memulihkan Desktop...
    if exist "!SRC_DIR!\Desktop" robocopy "!SRC_DIR!\Desktop" "%USERPROFILE%\Desktop" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL >nul 2>&1
    echo [*] Memulihkan Documents...
    if exist "!SRC_DIR!\Documents" robocopy "!SRC_DIR!\Documents" "%USERPROFILE%\Documents" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL >nul 2>&1
    echo [*] Memulihkan Downloads...
    if exist "!SRC_DIR!\Downloads" robocopy "!SRC_DIR!\Downloads" "%USERPROFILE%\Downloads" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL >nul 2>&1
    echo [*] Memulihkan Pictures...
    if exist "!SRC_DIR!\Pictures" robocopy "!SRC_DIR!\Pictures" "%USERPROFILE%\Pictures" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL >nul 2>&1

    if exist "!SRC_DIR!\Outlook_Signatures" (
        echo [*] Memulihkan Outlook Signatures...
        robocopy "!SRC_DIR!\Outlook_Signatures" "%APPDATA%\Microsoft\Signatures" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL >nul 2>&1
    )

    echo.
    echo [OK] Pemulihan data selesai!
    pause
    exit /b 0
)

exit /b 0
