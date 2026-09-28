@echo off
setlocal enabledelayedexpansion
title Pusat Kloning Disk & Mass Deployment
color 0E

echo ===============================================================================
echo     PUSAT KLONING DISK & MASS DEPLOYMENT (CLONEZILLA / RESCUEZILLA / FOG)
echo ===============================================================================
echo.
echo [1] CLONEZILLA (Standar Industri Kloning CLI)
echo     - Mode [device-image]: Backup SSD ke file image di TrueNAS/Samba Share.
echo     - Mode [device-device]: Kloning langsung dari HDD lama ke SSD baru.
echo     - Sangat cepat, hemat ruang, mendukung GPT/UEFI.
echo     - Web: https://clonezilla.org
echo.
echo [2] RESCUEZILLA ("Clonezilla Versi GUI yang Ramah Pengguna")
echo     - Tampilan Grafis Desktop (Point & Click).
echo     - 100% Kompatibel dengan backup buatan Clonezilla.
echo     - Dilengkapi browser web & GParted partisi.
echo     - Web: https://rescuezilla.com
echo.
echo [3] FOG PROJECT (Mass PXE Network Deployment Tanpa Flashdisk)
echo     - Server terpusat (Linux/TrueNAS VM) melayani booting LAN (PXE Boot).
echo     - Mampu menyebarkan 1 master image ke 10-50+ PC / mesin antrian sekaligus
echo       secara bersamaan melalui 'Multicast Deploy'.
echo     - Konfigurasi Router: DHCP Option 66 (Next Server) & Option 67 (ipxe.efi).
echo     - Web: https://fogproject.org
echo.
echo ===============================================================================
echo.
pause
