@echo off
setlocal enabledelayedexpansion
title Panduan Master Flashdisk Multi-Boot (Ventoy)
color 0A

echo ===============================================================================
echo            PANDUAN MASTER FLASHDISK MULTI-BOOT (VENTOY MULTI-ISO)
echo ===============================================================================
echo.
echo [KENAPA VENTOY WAJIB UNTUK TEKNISI IT?]
echo   - Format Flashdisk CUKUP 1 KALI seumur hidup.
echo   - Menambah OS baru CUKUP COPY-PASTE file .ISO seperti copy lagu/film biasa!
echo.
echo [STRUKTUR FOLDER MASTER FLASHDISK (64 GB / 128 GB)]
echo   USB_VENTOY (Drive E:\)
echo   +-- 01_Windows_Installers/ (Win 11 / Win 10 LTSC ISO)
echo   +-- 02_Kiosk_Specialist/   (Porteus-Kiosk-6.2.0.iso)
echo   +-- 03_Disk_Cloning/       (Rescuezilla.iso / Clonezilla.iso)
echo   +-- 04_Live_Rescue_WinPE/  (Hiren's BootCD / DLC Boot / SystemRescue)
echo   +-- 05_Portable_Tools/     (Folder ITTOOLS ini!)
echo.
echo [CARA INSTALL VENTOY KE FLASHDISK]
echo   1. Unduh Ventoy resmi di: https://www.ventoy.net
echo   2. Jalankan Ventoy2Disk.exe
echo   3. Pilih Flashdisk -> Option -> Partition Style -> Pilih 'GPT' -> Install.
echo   4. Copy semua file ISO yang Anda miliki ke dalam Flashdisk. Selesai!
echo.
echo ===============================================================================
echo.
pause
