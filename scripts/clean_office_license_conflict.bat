@echo off
setlocal EnableDelayedExpansion
title Pembersih Konflik Lisensi & Akun Office
color 0D
cls

echo ======================================================================
echo       PEMBERSIH KONFLIK LISENSI ^& AKUN LOGIN MICROSOFT OFFICE
echo ======================================================================
echo.
echo  1 = Pindai Lisensi Terpasang (Cek Key Hantu / Grace / Trial via ospp.vbs)
echo  2 = Hapus Key Lisensi Lama / Bentrok (Masukkan 5 Karakter Key)
echo  3 = Reset Cache Akun Login ^& Modern Auth (OneAuth + IdentityCache)
echo  4 = Reset Total (Hapus Cache Akun Login + Buka Pindai Key)
echo  0 = Keluar
echo.
echo ======================================================================

set "OSPP_PATH="
for %%P in (
    "C:\Program Files\Microsoft Office\Office16\OSPP.VBS"
    "C:\Program Files (x86)\Microsoft Office\Office16\OSPP.VBS"
    "C:\Program Files\Microsoft Office\Office15\OSPP.VBS"
    "C:\Program Files (x86)\Microsoft Office\Office15\OSPP.VBS"
    "C:\Program Files\Microsoft Office\root\Office16\OSPP.VBS"
    "C:\Program Files (x86)\Microsoft Office\root\Office16\OSPP.VBS"
) do (
    if exist %%P (
        set "OSPP_PATH=%%~P"
        goto :FoundOspp
    )
)

:FoundOspp
set "CLEAN_CHOICE="
set /p "CLEAN_CHOICE=Pilih Nomor Operasi [0-4]: "

if "%CLEAN_CHOICE%"=="1" (
    if "!OSPP_PATH!"=="" (
        echo [!] OSPP.VBS tidak ditemukan.
        pause
        exit /b 1
    )
    echo [*] Membaca lisensi terpasang...
    cscript //nologo "!OSPP_PATH!" /dstatus
    echo.
    echo [INFO] Perhatikan baris 'Last 5 characters of installed product key'.
    echo        Jika ada lisensi lama kedaluwarsa, catat 5 karakternya untuk dihapus di opsi 2.
    pause
    exit /b 0
)

if "%CLEAN_CHOICE%"=="2" (
    if "!OSPP_PATH!"=="" (
        echo [!] OSPP.VBS tidak ditemukan.
        pause
        exit /b 1
    )
    set "KEY_SUFFIX="
    set /p "KEY_SUFFIX=Masukkan 5 Karakter Terakhir Product Key: "
    if "!KEY_SUFFIX!"=="" (
        echo [!] Key kosong. Dibatalkan.
        pause
        exit /b 1
    )
    echo [*] Menghapus key !KEY_SUFFIX!...
    cscript //nologo "!OSPP_PATH!" /unpkey:!KEY_SUFFIX!
    echo [OK] Selesai menghapus key !KEY_SUFFIX!.
    pause
    exit /b 0
)

if "%CLEAN_CHOICE%"=="3" (
    echo [*] Menghentikan aplikasi Office...
    taskkill /f /im winword.exe >nul 2>&1
    taskkill /f /im excel.exe >nul 2>&1
    taskkill /f /im outlook.exe >nul 2>&1
    taskkill /f /im powerpnt.exe >nul 2>&1
    taskkill /f /im msosync.exe >nul 2>&1

    echo [*] Membersihkan folder OneAuth dan IdentityCache...
    rd /s /q "%LOCALAPPDATA%\Microsoft\OneAuth" >nul 2>&1
    rd /s /q "%LOCALAPPDATA%\Microsoft\IdentityCache" >nul 2>&1

    echo [OK] Cache akun login Microsoft 365 / Office telah di-reset!
    pause
    exit /b 0
)

if "%CLEAN_CHOICE%"=="4" (
    echo [*] Membersihkan token login...
    taskkill /f /im winword.exe >nul 2>&1
    taskkill /f /im excel.exe >nul 2>&1
    taskkill /f /im outlook.exe >nul 2>&1
    taskkill /f /im powerpnt.exe >nul 2>&1
    rd /s /q "%LOCALAPPDATA%\Microsoft\OneAuth" >nul 2>&1
    rd /s /q "%LOCALAPPDATA%\Microsoft\IdentityCache" >nul 2>&1
    echo [OK] Token login bersih. Sekarang membaca daftar key terpasang:
    if not "!OSPP_PATH!"=="" (
        cscript //nologo "!OSPP_PATH!" /dstatus
    )
    pause
    exit /b 0
)

exit /b 0
