@echo off
setlocal EnableDelayedExpansion
title Audit Status BitLocker & 48-Digit Recovery Key
color 0C

echo ==============================================================================
echo        🔐 AUDIT STATUS BITLOCKER & 48-DIGIT RECOVERY KEY
echo ==============================================================================
echo.

echo [1/2] Memeriksa status enkripsi volume harddisk...
manage-bde -status
echo.

echo [2/2] Mengekstrak Recovery Key Drive C: (Password Pelindung)...
manage-bde -protectors -get C:

echo.
echo ==============================================================================
echo  CATATAN: Pastikan Anda mencatat 48-digit Recovery Key di atas sebelum upgrade hardware!
echo ==============================================================================
pause
