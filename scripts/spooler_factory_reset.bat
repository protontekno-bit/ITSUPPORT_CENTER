@echo off
setlocal EnableDelayedExpansion
title Reset Total Spooler & Solusi Spooler Crash Loop
color 0C

echo ==============================================================================
echo        🔄 RESET TOTAL PRINT SPOOLER & SOLUSI CRASH LOOP
echo ==============================================================================
echo.

echo [1/3] Menghentikan Print Spooler Service...
net stop spooler /y >nul 2>&1

echo [2/3] Membersihkan seluruh berkas antrean macet...
del /q /f /s "%systemroot%\System32\spool\PRINTERS\*.*" >nul 2>&1

echo [3/3] Menjalankan kembali Print Spooler Service...
net start spooler

echo.
echo ==============================================================================
echo [SUKSES] Print Spooler berhasil di-reset dan aktif kembali.
echo ==============================================================================
pause
