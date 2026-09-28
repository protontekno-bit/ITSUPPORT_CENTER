@echo off
setlocal EnableDelayedExpansion

:: ==============================================================================
:: IT SUPPORT & SECURITY TOOL CENTER (MASTER LAUNCHER v3.0)
:: Teknologi: Pure Windows Batch Script (Zero-Dependency & Universal Windows Compatibility)
:: Fitur: Auto-Elevate Administrator, Zero-Latency, Fail-Safe Execution Loop
:: ==============================================================================

:: 1. PINDAH KE DIREKTORI KERJA AKTIF FILE INI (Mendukung Local Drive & UNC Network Share TrueNAS)
pushd "%~dp0" 2>nul
if %errorlevel% neq 0 cd /d "%~dp0" 2>nul

:: 2. AUTO-ELEVATE ADMINISTRATOR (Anti-Gagal: Otomatis meminta hak Admin jika belum)
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [INFO] Memeriksa hak Administrator...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd.exe -ArgumentList '/c pushd \"%~dp0\" && \"%~nx0\"' -Verb RunAs" >nul 2>&1
    if %errorlevel% neq 0 (
        echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
        echo UAC.ShellExecute "cmd.exe", "/c pushd ""%~dp0"" && ""%~nx0""", "", "runas", 1 >> "%temp%\getadmin.vbs"
        "%temp%\getadmin.vbs"
        del "%temp%\getadmin.vbs" >nul 2>&1
    )
    popd 2>nul
    exit /b
)

:: 3. SETUP TAMPILAN WINDOWS CONSOLE
title IT SUPPORT & SECURITY CENTER - [ADMINISTRATOR]
mode con cols=92 lines=42
color 0F

:MAIN_MENU
cls
echo ============================================================================================
echo                   IT SUPPORT ^& SECURITY TOOL CENTER (v3.2.0 Enterprise)
echo ============================================================================================
echo  Direktori Kerja: %~dp0
echo  DISCLAIMER     : Software disediakan 'AS-IS'. Segala risiko eksekusi ditanggung pengguna.
echo ============================================================================================
echo.
echo  [ 📋 INFO ASET ^& MAINTENANCE HARIAN ]
echo   1. 📋  Tampilkan ^& Salin Data Aset PC (Serial No, CPU, RAM, Sisa Disk C/D, IP, MAC)
echo   2. 🔋  Cek Kesehatan Baterai Laptop (Generate Battery Health Report HTML)
echo   3. 🚀  Buka Pengelola Startup PC (Matikan aplikasi auto-start yang membuat lemot)
echo   4. 🔄  Reset Windows Update Engine (Fix Update Stuck 0%% / CPU 100%%)
echo.
echo  [ 🖨️ PRINTER ^& SHARING KANTOR ]
echo   5. 🖨️  Fix Printer Sharing (Error 0x0000011b ^& Point and Print Driver)
echo   6. 📄  Bersihkan Antrean Cetak Nyangkut (Hapus file macet di spool\PRINTERS)
echo   7. 📁  Fix Akses Folder Share / Guest Logon Error (0x800704f8)
echo   8. 🔍  Diagnosa Koneksi SMB, Ping ^& Cek Port 445 Komputer Target
echo.
echo  [ 🌐 JARINGAN ^& KONEKTIVITAS ]
echo   9. 🌐  Reset Total Network Stack ^& WinHTTP Proxy (Mengatasi "No Internet, Secured")
echo   10. 🧹 Reset Cache Jaringan (DNS, NetBIOS, LanmanWorkstation ^& Putus Sesi)
echo   11. 🔌 Putus Semua Mapped Drive ^& Hapus Tiket Login (klist purge)
echo   12. 🕒 Sinkronkan Jam Sistem dengan NTP / Domain (w32tm /resync)
echo.
echo  [ 🛠️ PERAWATAN SISTEM ^& STORAGE ]
echo   13. 🧹 Bersihkan File Temp ^& Cache Update (Melegakan Drive C: Merah)
echo   14. 🛠️ Perbaikan File Sistem Windows (SFC ^& DISM 1-Click Repair)
echo   15. 🔑 Buka Windows Credential Manager (Mengatasi Akun Terkunci / Password Nyangkut)
echo.
echo  [ 🖥️ REMOTE DESKTOP ^& LISENSI ]
echo   16. 🖥️ Reset ID AnyDesk (Solusi ID Bentrok / PC Hasil Clone OS)
echo   17. 🔑 Ganti Password Permanen TeamViewer (Mode Paksa / Unattended)
echo   18. 📜 Audit Lisensi Windows (Cek Retail / OEM / Volume KMS ^& Masa Aktif)
echo   19. ⚡ Aktifkan Protokol Legacy SMB 1.0 (Untuk Printer/NAS Tua)
echo.
echo  [ 🛡️ KEAMANAN ^& FIREWALL ]
echo   20. 🛡️ Hardening Windows Anti-Ransomware (Tutup SMB, RDP, USB AutoPlay)
echo   21. 🧱 Blokir Port Masuk Rentan di Firewall (Port 445, 139, 3389, 23, 21)
echo   22. 🔄 Normalisasi ^& Buka Kembali Semua Sharing/RDP (Rollback Pemulihan)
echo   23. 📡 Audit ^& Scan Port Jaringan Kantor (Python Port Scanner)
echo.
echo  [ 🔌 MANAJEMEN DRIVER PERANGKAT ]
echo   24. 🔍 Pindai Driver Hilang / Tanda Seru Kuning (Get-PnpDevice / Hardware ID)
echo   25. 💾 Backup Seluruh Driver OEM Komputer ke Folder (DISM Export)
echo   26. 📥 Pasang / Restore Driver dari Folder Backup (PnPUtil Install)
echo.
echo  [ 🚀 UTILITY & DIAGNOSIS LANJUTAN ]
echo   27. 🔐 Audit Status BitLocker ^& 48-Digit Recovery Key (manage-bde)
echo   28. 🩺 Audit Kesehatan Fisik Disk S.M.A.R.T (SSD / HDD Degradasi)
echo   29. 🌐 Pengatur DNS Cepat Adapter (Google / Cloudflare / DHCP)
echo   30. 🔄 Restart Windows Explorer Shell ^& Taskbar Freeze
echo   31. 👤 Manajemen ^& Reset Password User (Lupa Password / Unlock / WiFi Pass)
echo   32. 📡 Pindai Seluruh IP Aktif di LAN (Subnet IP Sweeper 1-254)
echo   33. 📊 Diagnosa Kualitas Koneksi 3-Titik (Gateway vs ISP vs Internet)
echo   34. 🧹 Reset Proxy Browser (WinINet) ^& File Hosts Default
echo   35. 📶 Audit Kualitas Sinyal Wi-Fi ^& Detail Adapter WLAN
echo   36. ⚡ Perbaikan Total Jaringan 1-Klik (Full Network Recovery)
echo   37. 📋 Audit Konfigurasi IP Lengkap ^& Netstat (ipconfig /all ^& netstat)
echo   38. 🎛️ Pusat Kontrol Adapter Jaringan ^& Wi-Fi (ncpa.cpl ^& Settings)
echo   39. 🏷️ Ekstrak Kunci Lisensi OEM Asli dari BIOS (MSDM Table)
echo   40. 🧹 Bersihkan Residu Server KMS ^& Reset Token Lisensi (tokens.dat)
echo   41. 📑 Diagnosa Status Lisensi Microsoft Office (OSPP.VBS)
echo   42. 📈 Upgrade / Ganti Edisi Windows (Home ke Pro Tanpa Format)
echo   43. 🛡️ Pengatur Safe Mode ^& Perbaikan Booting (BCD / MSConfig)
echo   44. 🎛️ Pusat Alat Diagnosa ^& Administrasi Windows (Admin Hub)
echo   45. 🖨️ Audit Status Printer ^& Cetak Halaman Uji (Test Page)
echo   46. 🔄 Reset Total Spooler ^& Solusi Spooler Crash Loop
echo   47. 🎛️ Pusat Kontrol Printer Klasik (Devices and Printers / Server)
echo   48. 📦 Pasang Paket Software Kantor Otomatis (Winget: Chrome, 7-Zip, Reader, VLC, AnyDesk)
echo   49. ⚡ Debloater ^& Optimasi PC Kantor (Matikan Telemetri DiagTrack ^& Bing Start Menu)
echo   50. 💾 Bebaskan Kapasitas Disk C: (Disable Hibernation / hiberfil.sys)
echo   51. 🖱️ Kembalikan Menu Klik Kanan Klasik Windows 11 (Windows 10 Style)
echo   52. 🔌 Aktifkan .NET Framework 3.5 / 2.0 (DISM Online Legacy Fix)
echo   53. 🗜️ Kompresi Sistem Windows (CompactOS LZX Reclaim 4-8 GB)
echo   54. 🗑️ Pembersih Bloatware UWP Windows Kantor (Xbox, TikTok, Clipchamp)
echo   55. 🚫 Matikan Microsoft Edge Background ^& Startup Boost (Hemat RAM)
echo   56. 🏎️ Optimasi Efek Visual ^& Kecepatan UI (Best Office Responsiveness)
echo   57. 🔒 Saklar Kunci / Buka Windows Update Permanen (Lock / Unlock)
echo   58. 🔄 1-Klik Buat System Restore Point (Snapshot Pemulihan Instan)
echo   59. 📦 Backup Windows System ke WIM (Microsoft DISM Capture Offline)
echo   60. 🩺 Pusat Live Rescue ^& WinPE Hub (Hiren's, DLC Boot, SystemRescue, UBCD)
echo   61. 💾 Pusat Kloning ^& Deployment (Clonezilla, Rescuezilla, FOG Project)
echo   62. 🚀 Pusat Flashdisk Multi-Boot Master (Ventoy Multi-ISO)
echo   63. 🔬 Inspeksi Total Spesifikasi ^& Kesehatan Hardware (Deep Audit)
echo   64. 🔄 Reset Total Windows Firewall ke Default Pabrik (mpssvc Reset)
echo   65. 📡 Izinkan / Blokir Respon Ping LAN (ICMPv4 Echo Request)
echo   66. ☣️ Karantina Jaringan Darurat (Ransomware Air-Gap Isolation)
echo   67. 🚪 Pusat Buka / Tutup Port Firewall Kantor (RDP, Web, DB, Custom)
echo   68. 🎛️ Saklar Status Profil Firewall (Domain, Private, Public)
echo   69. 🦀 Pusat Manajemen ^& Utilitas RustDesk (Launch, Reset ID, Server, Service)
echo   70. 🏢 Asisten Active Directory ^& Join Domain (Audit, Rename PC, Join AD, DC Test)
echo   71. 🖥️ Pusat Manajemen ^& Aktivator Windows RDP (Enable, Port, Firewall, Shadowing)
echo   72. 🔒 Kunci / Buka Update Microsoft Office Permanen (C2R Registry ^& Tasks)
echo   73. 🚑 Pertolongan Pertama Office ^& Outlook (Safe Mode, scanpst.exe, Normal.dotm)
echo   74. 🧹 Pembersih Konflik Lisensi ^& Akun Office (Ghost Key ^& Token Reset)
echo.
echo ============================================================================================
echo   0.  ❌ Keluar
echo ============================================================================================
echo.

set "CHOICE="
set /p "CHOICE=Masukkan Nomor Menu Pilihan Anda [0-74]: "

if "%CHOICE%"=="1" goto SHOW_PC_ASSET
if "%CHOICE%"=="2" goto BATTERY_REPORT
if "%CHOICE%"=="3" goto STARTUP_MGR
if "%CHOICE%"=="4" goto RESET_WIN_UPDATE
if "%CHOICE%"=="5" goto FIX_PRINTER
if "%CHOICE%"=="6" goto CLEAR_PRINT_QUEUE
if "%CHOICE%"=="7" goto FIX_SMB_GUEST
if "%CHOICE%"=="8" goto DIAGNOSE_SMB
if "%CHOICE%"=="9" goto RESET_NET_STACK
if "%CHOICE%"=="10" goto RESET_NET_CACHE
if "%CHOICE%"=="11" goto DISCONNECT_SHARES
if "%CHOICE%"=="12" goto SYNC_NTP_TIME
if "%CHOICE%"=="13" goto CLEAN_TEMP
if "%CHOICE%"=="14" goto SFC_DISM_REPAIR
if "%CHOICE%"=="15" goto OPEN_CRED_MGR
if "%CHOICE%"=="16" goto RESET_ANYDESK
if "%CHOICE%"=="17" goto CHANGE_TV_PASS
if "%CHOICE%"=="18" goto AUDIT_LICENSE
if "%CHOICE%"=="19" goto ENABLE_SMB1
if "%CHOICE%"=="20" goto HARDENING_RANSOMWARE
if "%CHOICE%"=="21" goto BLOCK_PORTS
if "%CHOICE%"=="22" goto ROLLBACK_SHARING
if "%CHOICE%"=="23" goto SCAN_NETWORK
if "%CHOICE%"=="24" goto SCAN_DRIVERS
if "%CHOICE%"=="25" goto BACKUP_DRIVERS
if "%CHOICE%"=="26" goto RESTORE_DRIVERS
if "%CHOICE%"=="27" goto AUDIT_BITLOCKER
if "%CHOICE%"=="28" goto AUDIT_SMART
if "%CHOICE%"=="29" goto SWITCH_DNS
if "%CHOICE%"=="30" goto RESTART_EXPLORER
if "%CHOICE%"=="31" goto MANAGE_PASSWORDS
if "%CHOICE%"=="32" goto SCAN_SUBNET
if "%CHOICE%"=="33" goto TEST_NET_QUALITY
if "%CHOICE%"=="34" goto RESET_PROXY_HOSTS
if "%CHOICE%"=="35" goto AUDIT_WIFI
if "%CHOICE%"=="36" goto FULL_NET_REPAIR
if "%CHOICE%"=="37" goto AUDIT_IP_NETSTAT
if "%CHOICE%"=="38" goto OPEN_NET_CPL
if "%CHOICE%"=="39" goto EXTRACT_OEM_KEY
if "%CHOICE%"=="40" goto CLEAN_KMS_TOKENS
if "%CHOICE%"=="41" goto DIAGNOSE_OFFICE
if "%CHOICE%"=="42" goto SWITCH_EDITION
if "%CHOICE%"=="43" goto SAFE_MODE_BOOT
if "%CHOICE%"=="44" goto ADMIN_TOOLS_HUB
if "%CHOICE%"=="45" goto AUDIT_PRINTERS
if "%CHOICE%"=="46" goto SPOOLER_RESET
if "%CHOICE%"=="47" goto OPEN_PRINTER_CPL
if "%CHOICE%"=="48" goto WINGET_INSTALL
if "%CHOICE%"=="49" goto DEBLOAT_OFFICE
if "%CHOICE%"=="50" goto DISABLE_HIBERNATION
if "%CHOICE%"=="51" goto RESTORE_CONTEXT_MENU
if "%CHOICE%"=="52" goto ENABLE_DOTNET35
if "%CHOICE%"=="53" goto COMPACT_OS
if "%CHOICE%"=="54" goto REMOVE_UWP
if "%CHOICE%"=="55" goto DISABLE_EDGE_BG
if "%CHOICE%"=="56" goto OPTIMIZE_VISUAL
if "%CHOICE%"=="57" goto TOGGLE_UPDATE_LOCK
if "%CHOICE%"=="58" goto CREATE_RESTORE_POINT
if "%CHOICE%"=="59" goto DISM_WIM_BACKUP
if "%CHOICE%"=="60" goto LIVE_RESCUE_HUB
if "%CHOICE%"=="61" goto DISK_CLONING_HUB
if "%CHOICE%"=="62" goto VENTOY_GUIDE
if "%CHOICE%"=="63" goto DEEP_INSPECTOR
if "%CHOICE%"=="64" goto RESET_FIREWALL
if "%CHOICE%"=="65" goto TOGGLE_PING
if "%CHOICE%"=="66" goto AIRGAP_QUARANTINE
if "%CHOICE%"=="67" goto PORT_MANAGER
if "%CHOICE%"=="68" goto FIREWALL_PROFILES
if "%CHOICE%"=="69" goto RUSTDESK_HUB
if "%CHOICE%"=="70" goto AD_DOMAIN_ASSISTANT
if "%CHOICE%"=="71" goto RDP_MANAGER_HUB
if "%CHOICE%"=="72" goto TOGGLE_OFFICE_LOCK
if "%CHOICE%"=="73" goto OFFICE_RESCUE_HUB
if "%CHOICE%"=="74" goto CLEAN_OFFICE_CONFLICT
if "%CHOICE%"=="0" goto EXIT_APP

echo.
echo [!] Pilihan tidak valid. Silakan pilih nomor yang tersedia.
timeout /t 2 >nul
goto MAIN_MENU

:: ==============================================================================
:: SUBROUTINES EKSEKUSI TOOL
:: ==============================================================================

:SHOW_PC_ASSET
cls
echo ==================================================
echo           INFORMASI ASET PERANGKAT (PC)
echo ==================================================
echo Nama Komputer   : %COMPUTERNAME%
echo User Aktif      : %USERNAME%
echo Sistem Operasi  : Windows %PROCESSOR_ARCHITECTURE%
echo.
echo [1/3] Membaca Nomor Seri / Service Tag BIOS...
wmic bios get serialnumber,smbiosbiosversion 2>nul
echo.
echo [2/3] Membaca Tipe Prosesor & Memori RAM...
wmic cpu get name,numberofcores 2>nul
wmic computersystem get totalphysicalmemory,model 2>nul
echo.
echo [3/3] Membaca Kapasitas Drive Storage...
wmic logicaldisk get caption,drivetype,freespace,size 2>nul
echo ==================================================
goto BACK_TO_MENU

:BATTERY_REPORT
cls
echo [*] Menganalisis kesehatan baterai laptop...
powercfg /batteryreport /output "%TEMP%\battery_report.html"
if exist "%TEMP%\battery_report.html" (
    echo [OK] Laporan baterai berhasil dibuat! Membuka di browser...
    start "" "%TEMP%\battery_report.html"
) else (
    echo [WARNING] Tidak dapat membuat laporan. Pastikan ini adalah Laptop.
)
goto BACK_TO_MENU

:STARTUP_MGR
cls
echo [*] Membuka Pengelola Aplikasi Startup...
start taskmgr.exe /7
goto BACK_TO_MENU

:RESET_WIN_UPDATE
cls
echo [*] Mereset Windows Update Engine...
net stop wuauserv >nul 2>&1
net stop bits >nul 2>&1
net stop cryptsvc >nul 2>&1
net stop msiserver >nul 2>&1
rd /s /q "%systemroot%\SoftwareDistribution" >nul 2>&1
rd /s /q "%systemroot%\System32\catroot2" >nul 2>&1
net start cryptsvc >nul 2>&1
net start bits >nul 2>&1
net start wuauserv >nul 2>&1
net start msiserver >nul 2>&1
echo [OK] Windows Update Engine berhasil di-reset!
goto BACK_TO_MENU

:FIX_PRINTER
cls
echo [*] Menjalankan Perbaikan Printer Sharing...
call "%~dp0scripts\fix_printer_sharing_0x0000011b.bat"
goto BACK_TO_MENU

:CLEAR_PRINT_QUEUE
cls
echo [*] Membersihkan antrean cetak printer...
net stop spooler >nul 2>&1
del /Q /F /S "%systemroot%\System32\Spool\Printers\*.*" >nul 2>&1
net start spooler >nul 2>&1
echo [OK] Antrean cetak berhasil dibersihkan!
goto BACK_TO_MENU

:FIX_SMB_GUEST
cls
echo [*] Menjalankan Perbaikan Insecure Guest Auth...
call "%~dp0scripts\fix_smb_guest_auth.bat"
goto BACK_TO_MENU

:DIAGNOSE_SMB
cls
echo [*] Menjalankan Diagnosa Koneksi SMB...
call "%~dp0scripts\diagnose_smb_share.bat"
goto BACK_TO_MENU

:RESET_NET_STACK
cls
echo [*] Mereset Total Stack Jaringan, WinSock, dan Proxy...
netsh winsock reset
netsh int ip reset
netsh winhttp reset proxy
ipconfig /flushdns
ipconfig /renew
echo [OK] Reset Stack Jaringan Selesai!
goto BACK_TO_MENU

:RESET_NET_CACHE
cls
echo [*] Menjalankan Reset Cache Jaringan ^& Sesi...
call "%~dp0scripts\reset_network_cache_and_sessions.bat"
goto BACK_TO_MENU

:DISCONNECT_SHARES
cls
echo [*] Menjalankan Pemutus Drive Jaringan...
call "%~dp0scripts\disconnect_all_shares.bat"
goto BACK_TO_MENU

:SYNC_NTP_TIME
cls
echo [*] Memulai sinkronisasi jam sistem dengan w32tm...
net start w32time >nul 2>&1
w32tm /resync /force
echo [OK] Sinkronisasi waktu selesai.
goto BACK_TO_MENU

:CLEAN_TEMP
cls
echo [*] Membersihkan file sampah temporary dan cache update...
del /q/f/s "%TEMP%\*" >nul 2>&1
del /q/f/s "%systemroot%\Temp\*" >nul 2>&1
del /q/f/s "%systemroot%\SoftwareDistribution\Download\*" >nul 2>&1
echo [OK] File temporary dan cache update berhasil dibersihkan!
goto BACK_TO_MENU

:SFC_DISM_REPAIR
cls
echo [*] Memulai perbaikan file sistem Windows (SFC & DISM)...
echo Proses ini membutuhkan waktu beberapa menit. Mohon tunggu...
sfc /scannow
DISM /Online /Cleanup-Image /RestoreHealth
echo [OK] Perbaikan file sistem selesai!
goto BACK_TO_MENU

:OPEN_CRED_MGR
cls
echo [*] Membuka Windows Credential Manager...
start control.exe keymgr.dll
echo [OK] Credential Manager terbuka. Hapus password lama pada tab Windows Credentials.
goto BACK_TO_MENU

:RESET_ANYDESK
cls
echo [*] Menjalankan Reset ID AnyDesk...
call "%~dp0scripts\reset_anydesk_id.bat"
goto BACK_TO_MENU

:CHANGE_TV_PASS
cls
echo [*] Menjalankan Ganti Password TeamViewer...
call "%~dp0scripts\change_teamviewer_password.bat"
goto BACK_TO_MENU

:AUDIT_LICENSE
cls
echo [*] Menjalankan Audit Lisensi Windows...
call "%~dp0scripts\audit_windows_license.bat"
goto BACK_TO_MENU

:ENABLE_SMB1
cls
echo [*] Menjalankan Pengaktifan SMB 1.0 Legacy...
call "%~dp0scripts\enable_smb1_legacy.bat"
goto BACK_TO_MENU

:HARDENING_RANSOMWARE
cls
echo [*] Menjalankan Skrip Hardening Windows Anti-Ransomware...
call "%~dp0scripts\hardening_windows_antiransomware.bat"
goto BACK_TO_MENU

:BLOCK_PORTS
cls
echo [*] Menjalankan Pemblokiran Port di Firewall...
call "%~dp0scripts\block_ports_firewall.bat"
goto BACK_TO_MENU

:ROLLBACK_SHARING
cls
echo [*] Membuka kembali File & Printer Sharing, RDP, dan Network Discovery...
netsh advfirewall firewall set rule group="File and Printer Sharing" new enable=Yes >nul 2>&1
netsh advfirewall firewall set rule group="Network Discovery" new enable=Yes >nul 2>&1
netsh advfirewall firewall set rule group="Remote Desktop" new enable=Yes >nul 2>&1
reg add "HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server" /v fDenyTSConnections /t REG_DWORD /d 0 /f >nul 2>&1
reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v NoDriveTypeAutoRun /f >nul 2>&1
net start LanmanServer >nul 2>&1
net start LanmanWorkstation >nul 2>&1
echo [OK] Normalisasi Selesai. Akses Sharing & RDP telah dibuka normal.
goto BACK_TO_MENU

:SCAN_NETWORK
cls
echo [*] Memeriksa ketersediaan Python di sistem ini...
python --version >nul 2>&1
if %errorlevel% equ 0 (
    python "%~dp0scripts\audit_network_scanner.py"
) else (
    echo [ERROR] Python tidak terdeteksi pada PATH di komputer ini!
    echo Skrip scanner membutuhkan Python 3 terinstall.
    echo.
    echo Solusi:
    echo 1. Gunakan [ITSupportCenter.exe] untuk scanner native C# tanpa butuh Python!
    echo 2. Gunakan Nmap (installers\nmap-7.98-setup.exe) untuk pemindaian port jaringan.
    echo.
    pause
)
goto BACK_TO_MENU

:SCAN_DRIVERS
cls
call "%~dp0scripts\scan_missing_drivers.bat"
goto BACK_TO_MENU

:BACKUP_DRIVERS
cls
call "%~dp0scripts\backup_drivers.bat"
goto BACK_TO_MENU

:RESTORE_DRIVERS
cls
call "%~dp0scripts\restore_drivers.bat"
goto BACK_TO_MENU

:AUDIT_BITLOCKER
cls
call "%~dp0scripts\audit_bitlocker_recovery_key.bat"
goto BACK_TO_MENU

:AUDIT_SMART
cls
call "%~dp0scripts\audit_disk_smart.bat"
goto BACK_TO_MENU

:SWITCH_DNS
cls
call "%~dp0scripts\switch_dns_public.bat"
goto BACK_TO_MENU

:RESTART_EXPLORER
cls
call "%~dp0scripts\restart_explorer.bat"
goto BACK_TO_MENU

:MANAGE_PASSWORDS
cls
call "%~dp0scripts\manage_user_passwords.bat"
goto BACK_TO_MENU

:SCAN_SUBNET
cls
call "%~dp0scripts\scan_subnet_ip.bat"
goto BACK_TO_MENU

:TEST_NET_QUALITY
cls
call "%~dp0scripts\test_network_quality.bat"
goto BACK_TO_MENU

:RESET_PROXY_HOSTS
cls
call "%~dp0scripts\reset_proxy_and_hosts.bat"
goto BACK_TO_MENU

:AUDIT_WIFI
cls
call "%~dp0scripts\audit_wifi_signal.bat"
goto BACK_TO_MENU

:FULL_NET_REPAIR
cls
call "%~dp0scripts\full_network_repair.bat"
goto BACK_TO_MENU

:AUDIT_IP_NETSTAT
cls
call "%~dp0scripts\audit_ip_and_netstat.bat"
goto BACK_TO_MENU

:OPEN_NET_CPL
cls
call "%~dp0scripts\open_network_connections.bat"
goto BACK_TO_MENU

:EXTRACT_OEM_KEY
cls
call "%~dp0scripts\extract_oem_bios_key.bat"
goto BACK_TO_MENU

:CLEAN_KMS_TOKENS
cls
call "%~dp0scripts\clean_kms_and_reset_tokens.bat"
goto BACK_TO_MENU

:DIAGNOSE_OFFICE
cls
call "%~dp0scripts\diagnose_office_license.bat"
goto BACK_TO_MENU

:SWITCH_EDITION
cls
call "%~dp0scripts\switch_windows_edition.bat"
goto BACK_TO_MENU

:SAFE_MODE_BOOT
cls
call "%~dp0scripts\safe_mode_and_bcd_repair.bat"
goto BACK_TO_MENU

:ADMIN_TOOLS_HUB
cls
call "%~dp0scripts\windows_admin_tools_hub.bat"
goto BACK_TO_MENU

:AUDIT_PRINTERS
cls
call "%~dp0scripts\audit_printers_and_test_print.bat"
goto BACK_TO_MENU

:SPOOLER_RESET
cls
call "%~dp0scripts\spooler_factory_reset.bat"
goto BACK_TO_MENU

:OPEN_PRINTER_CPL
cls
call "%~dp0scripts\open_devices_and_printers_classic.bat"
goto BACK_TO_MENU

:WINGET_INSTALL
cls
call "%~dp0scripts\install_office_essentials_winget.bat"
goto BACK_TO_MENU

:DEBLOAT_OFFICE
cls
call "%~dp0scripts\debloat_office_pc.bat"
goto BACK_TO_MENU

:DISABLE_HIBERNATION
cls
call "%~dp0scripts\disable_hibernation_reclaim_disk.bat"
goto BACK_TO_MENU

:RESTORE_CONTEXT_MENU
cls
call "%~dp0scripts\restore_classic_context_menu.bat"
goto BACK_TO_MENU

:ENABLE_DOTNET35
cls
call "%~dp0scripts\enable_dotnet35_dism.bat"
goto BACK_TO_MENU

:COMPACT_OS
cls
call "%~dp0scripts\compact_os_compression.bat"
goto BACK_TO_MENU

:REMOVE_UWP
cls
call "%~dp0scripts\remove_uwp_bloatware.bat"
goto BACK_TO_MENU

:DISABLE_EDGE_BG
cls
call "%~dp0scripts\disable_edge_background_boost.bat"
goto BACK_TO_MENU

:OPTIMIZE_VISUAL
cls
call "%~dp0scripts\optimize_visual_effects.bat"
goto BACK_TO_MENU

:TOGGLE_UPDATE_LOCK
cls
call "%~dp0scripts\toggle_windows_update_lock.bat"
goto BACK_TO_MENU

:CREATE_RESTORE_POINT
cls
call "%~dp0scripts\create_system_restore_point.bat"
goto BACK_TO_MENU

:DISM_WIM_BACKUP
cls
call "%~dp0scripts\dism_wim_system_backup.bat"
goto BACK_TO_MENU

:LIVE_RESCUE_HUB
cls
call "%~dp0scripts\live_rescue_toolkit_hub.bat"
goto BACK_TO_MENU

:DISK_CLONING_HUB
cls
call "%~dp0scripts\disk_cloning_deployment_hub.bat"
goto BACK_TO_MENU

:VENTOY_GUIDE
cls
call "%~dp0scripts\ventoy_multiboot_guide.bat"
goto BACK_TO_MENU

:DEEP_INSPECTOR
cls
call "%~dp0scripts\deep_hardware_health_inspector.bat"
goto BACK_TO_MENU

:RESET_FIREWALL
cls
call "%~dp0scripts\reset_windows_firewall.bat"
goto BACK_TO_MENU

:TOGGLE_PING
cls
call "%~dp0scripts\toggle_icmp_ping_response.bat"
goto BACK_TO_MENU

:AIRGAP_QUARANTINE
cls
call "%~dp0scripts\emergency_network_quarantine.bat"
goto BACK_TO_MENU

:PORT_MANAGER
cls
call "%~dp0scripts\firewall_port_manager_hub.bat"
goto BACK_TO_MENU

:FIREWALL_PROFILES
cls
call "%~dp0scripts\toggle_firewall_profiles.bat"
goto BACK_TO_MENU

:RUSTDESK_HUB
cls
call "%~dp0scripts\rustdesk_manager_hub.bat"
goto BACK_TO_MENU

:AD_DOMAIN_ASSISTANT
cls
call "%~dp0scripts\active_directory_domain_assistant.bat"
goto BACK_TO_MENU

:RDP_MANAGER_HUB
cls
call "%~dp0scripts\windows_rdp_manager_hub.bat"
goto BACK_TO_MENU

:TOGGLE_OFFICE_LOCK
cls
call "%~dp0scripts\toggle_office_update_lock.bat"
goto BACK_TO_MENU

:OFFICE_RESCUE_HUB
cls
call "%~dp0scripts\office_rescue_hub.bat"
goto BACK_TO_MENU

:CLEAN_OFFICE_CONFLICT
cls
call "%~dp0scripts\clean_office_license_conflict.bat"
goto BACK_TO_MENU

:BACK_TO_MENU
echo.
echo ============================================================================================
echo Tekan Enter untuk kembali ke Menu Utama...
pause >nul
goto MAIN_MENU

:EXIT_APP
cls
echo.
echo Terima kasih telah menggunakan IT Support ^& Security Center.
timeout /t 1 >nul
exit /b
