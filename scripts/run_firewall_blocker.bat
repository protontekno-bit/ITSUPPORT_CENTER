@echo off
:: Pindah ke folder tempat file ini berada (PENTING)
cd /d "%~dp0"

:: Jalankan skrip Python
python firewall_port_blocker.py

:: Tahan layar agar tidak langsung tertutup
pause