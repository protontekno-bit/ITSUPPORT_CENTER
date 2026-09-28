@echo off
setlocal EnableDelayedExpansion
title Penghapus Paksa Program Macet (CLI)
color 0C
cls

echo ======================================================================
echo       PENGHAPUS PAKSA SOFTWARE MACET (FORCE UNINSTALLER)
echo ======================================================================
echo.
echo  1 = Cari Program Terpasang di Registry Uninstall
echo  2 = Hapus Paksa Entri Registry Program (Berdasarkan Nama Key)
echo  0 = Batal / Keluar
echo.
echo ======================================================================

set "NUKE_OPT="
set /p "NUKE_OPT=Pilih Nomor Operasi [0-2]: "

if "%NUKE_OPT%"=="1" (
    set "KEYWORD="
    set /p "KEYWORD=Masukkan nama software yang ingin dicari: "
    echo.
    echo [*] Memindai registry 64-bit...
    reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall" /s /f "!KEYWORD!" 2>nul | findstr /i "DisplayName"
    echo [*] Memindai registry 32-bit...
    reg query "HKLM\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" /s /f "!KEYWORD!" 2>nul | findstr /i "DisplayName"
    echo.
    echo [INFO] Perhatikan baris path registry untuk dihapus pada Opsi 2.
    pause
    exit /b 0
)

if "%NUKE_OPT%"=="2" (
    set "TARGET_KEY="
    set /p "TARGET_KEY=Masukkan nama Registry Key program yang ingin dihapus: "
    if "!TARGET_KEY!"=="" (
        echo [!] Nama key kosong.
        pause
        exit /b 1
    )
    echo [*] Menghapus dari registry 64-bit...
    reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\!TARGET_KEY!" /f >nul 2>&1
    echo [*] Menghapus dari registry 32-bit...
    reg delete "HKLM\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\!TARGET_KEY!" /f >nul 2>&1
    echo [OK] Entri program berhasil dihapus paksa dari sistem!
    pause
    exit /b 0
)

exit /b 0
