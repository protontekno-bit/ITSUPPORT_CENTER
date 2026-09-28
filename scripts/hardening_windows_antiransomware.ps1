<#
.SYNOPSIS
    Skrip Hardening Windows Anti-Ransomware (Client Side)
    Tested on: Windows 10, Windows 11
    
.DESCRIPTION
    Skrip ini melakukan konfigurasi keamanan untuk mencegah penyebaran ransomware (lateral movement)
    dengan mematikan protokol rentan dan menutup akses jaringan yang tidak perlu.
#>

# --- BAGIAN 0: CEK ADMINISTRATOR ---
if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Warning "PERINGATAN: Skrip ini butuh akses ADMINISTRATOR!"
    Write-Warning "Silakan Klik Kanan file ini -> pilih 'Run with PowerShell' sebagai Administrator"
    Write-Host "Tekan Enter untuk keluar..."
    Read-Host
    Exit
}

Clear-Host
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "   SKRIP HARDENING WINDOWS ANTI-RANSOMWARE" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# --- BAGIAN 1: MATIKAN SMBv1 ---
Write-Host "[1/5] Mematikan Protokol SMBv1 (WannaCry Prevention)..." -ForegroundColor Yellow
try {
    $smbStatus = Get-WindowsOptionalFeature -Online -FeatureName SMB1Protocol -ErrorAction SilentlyContinue
    if ($smbStatus -and $smbStatus.State -eq "Enabled") {
        Disable-WindowsOptionalFeature -Online -FeatureName SMB1Protocol -NoRestart | Out-Null
        Write-Host "      [OK] SMBv1 berhasil dimatikan." -ForegroundColor Green
    } else {
        Write-Host "      [OK] SMBv1 sudah mati atau tidak aktif." -ForegroundColor Green
    }
} catch {
    Write-Host "      [INFO] Fitur SMBv1 tidak ditemukan (Mungkin Windows versi baru)." -ForegroundColor Gray
}

# --- BAGIAN 2: FIREWALL (Discovery & Sharing) ---
Write-Host "[2/5] Mengonfigurasi Firewall (Block Discovery & Sharing)..." -ForegroundColor Yellow
try {
    netsh advfirewall firewall set rule group="Network Discovery" new enable=No | Out-Null
    netsh advfirewall firewall set rule group="File and Printer Sharing" new enable=No | Out-Null
    Write-Host "      [OK] Port SMB (445) dan NetBIOS ditutup di Firewall." -ForegroundColor Green
} catch {
    Write-Host "      [ERROR] Gagal set firewall. Pastikan Windows Firewall aktif." -ForegroundColor Red
}

# --- BAGIAN 3: MATIKAN REMOTE DESKTOP (RDP) ---
Write-Host "[3/5] Mematikan Fitur Remote Desktop (RDP)..." -ForegroundColor Yellow
try {
    Set-ItemProperty -Path 'HKLM:\System\CurrentControlSet\Control\Terminal Server' -Name "fDenyTSConnections" -Value 1 -ErrorAction Stop
    Write-Host "      [OK] Remote Desktop (Port 3389) dimatikan." -ForegroundColor Green
} catch {
    Write-Host "      [ERROR] Gagal mematikan RDP via Registry." -ForegroundColor Red
}

# --- BAGIAN 4: MATIKAN AUTOPLAY (USB) ---
Write-Host "[4/5] Mematikan AutoPlay & AutoRun (USB Security)..." -ForegroundColor Yellow
try {
    $registryPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"
    if (!(Test-Path $registryPath)) { New-Item -Path $registryPath -Force | Out-Null }
    Set-ItemProperty -Path $registryPath -Name "NoDriveTypeAutoRun" -Value 255 -Force | Out-Null
    Write-Host "      [OK] AutoPlay dimatikan untuk semua drive." -ForegroundColor Green
} catch {
    Write-Host "      [ERROR] Gagal set registry AutoPlay." -ForegroundColor Red
}

# --- BAGIAN 5: MATIKAN AUTO ADD DEVICES ---
Write-Host "[5/5] Mematikan Pencarian Device Otomatis..." -ForegroundColor Yellow
try {
    $driverPath = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching"
    if (!(Test-Path $driverPath)) { New-Item -Path $driverPath -Force | Out-Null }
    New-ItemProperty -Path $driverPath -Name "SearchOrderConfig" -Value 0 -PropertyType DWORD -Force -ErrorAction SilentlyContinue | Out-Null
    Write-Host "      [OK] Auto install network devices dimatikan." -ForegroundColor Green
} catch {
    Write-Host "      [INFO] Registry key mungkin sudah ada atau dilindungi." -ForegroundColor Gray
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host " PROSES SELESAI" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Catatan:"
Write-Host "1. Komputer ini sekarang TERISOLASI dari sharing jaringan lokal."
Write-Host "2. Jika Anda menggunakan RustDesk/AnyDesk, aplikasi itu TETAP BISA jalan."
Write-Host "3. Sebaiknya RESTART komputer untuk memastikan semua efek (terutama SMBv1) aktif."
Write-Host ""
Write-Host "Tekan Enter untuk keluar..."
Read-Host
