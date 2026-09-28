@echo off
setlocal EnableDelayedExpansion
title Diagnosa Status Lisensi Microsoft Office (OSPP.VBS)
color 0E

echo ==============================================================================
echo        📑 DIAGNOSA STATUS LISENSI MICROSOFT OFFICE (OSPP.VBS)
echo ==============================================================================
echo.

set "OSPP="
if exist "%ProgramFiles%\Microsoft Office\Office16\OSPP.VBS" set "OSPP=%ProgramFiles%\Microsoft Office\Office16\OSPP.VBS"
if exist "%ProgramFiles(x86)%\Microsoft Office\Office16\OSPP.VBS" set "OSPP=%ProgramFiles(x86)%\Microsoft Office\Office16\OSPP.VBS"
if exist "%ProgramFiles%\Microsoft Office\Office15\OSPP.VBS" set "OSPP=%ProgramFiles%\Microsoft Office\Office15\OSPP.VBS"
if exist "%ProgramFiles(x86)%\Microsoft Office\Office15\OSPP.VBS" set "OSPP=%ProgramFiles(x86)%\Microsoft Office\Office15\OSPP.VBS"

if "%OSPP%"=="" (
    echo [ERROR] Tidak ditemukan mesin OSPP.VBS Office di sistem ini!
    pause
    exit /b 1
)

echo File OSPP terdeteksi di: %OSPP%
echo.
echo  Pilih Operasi:
echo   1. 📑 Cek Status Lisensi Office (/dstatus)
echo   2. 🗑️ Hapus 5-Digit Kunci Lisensi Bentrok (/unpkey:XXXXX)
echo.
set /p "OFFICE_OP=Masukkan pilihan [1-2]: "

if "%OFFICE_OP%"=="1" (
    echo.
    cscript //nologo "%OSPP%" /dstatus
)
if "%OFFICE_OP%"=="2" (
    echo.
    set /p "UNP_KEY=Masukkan 5-digit kunci parsial yang ingin dihapus: "
    if not "!UNP_KEY!"=="" (
        cscript //nologo "%OSPP%" /unpkey:!UNP_KEY!
        echo [SUKSES] Kunci !UNP_KEY! berhasil dicopot.
    )
)

echo.
echo ==============================================================================
pause
