import subprocess
import ctypes
import sys
import os

# =================================================================
# DAFTAR PORT YANG AKAN DIBLOKIR / DIBUKA
# =================================================================
TARGET_PORTS = {
    "445":  "SMB (Ransomware Wannacry)",
    "139":  "NetBIOS",
    "3389": "RDP (Remote Desktop)",
    "23":   "Telnet",
    "21":   "FTP",
    "4444": "Metasploit (Backdoor)",
}
# =================================================================

class Colors:
    HEADER = '\033[95m'
    OKGREEN = '\033[92m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'

def is_admin():
    """Cek apakah skrip dijalankan sebagai Administrator"""
    try:
        return ctypes.windll.shell32.IsUserAnAdmin()
    except Exception:
        return False

def run_command(cmd):
    """Menjalankan perintah CMD via Python dan menangkap hasilnya"""
    # subprocess.DEVNULL menyembunyikan output sampah dari netsh
    result = subprocess.run(cmd, shell=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    return result.returncode == 0

def block_ports():
    print(f"\n{Colors.HEADER}>>> MENJALANKAN MISI: BENTENG API (BLOCKING) <<<{Colors.ENDC}")
    print("-" * 60)
    
    sukses_count = 0
    for port, desc in TARGET_PORTS.items():
        rule_name = f"[BLOCK_RANSOMWARE] {desc} ({port})"
        
        # Perintah netsh yang sama, tapi dikendalikan Python
        cmd = f'netsh advfirewall firewall add rule name="{rule_name}" dir=in action=block protocol=TCP localport={port}'
        
        if run_command(cmd):
            print(f"{Colors.OKGREEN}[SUKSES] Port {port:<5} | {desc} -> DITUTUP.{Colors.ENDC}")
            sukses_count += 1
        else:
            print(f"{Colors.FAIL}[GAGAL ] Port {port:<5} | {desc} -> Gagal dieksekusi.{Colors.ENDC}")
            
    print("-" * 60)
    print(f"Selesai. {sukses_count} aturan firewall berhasil dibuat.")

def unblock_ports():
    print(f"\n{Colors.HEADER}>>> MENJALANKAN MISI: BUKA GERBANG (UNDO) <<<{Colors.ENDC}")
    print("-" * 60)
    
    for port, desc in TARGET_PORTS.items():
        rule_name = f"[BLOCK_RANSOMWARE] {desc} ({port})"
        
        # Perintah delete rule
        cmd = f'netsh advfirewall firewall delete rule name="{rule_name}"'
        
        # Kita tidak perlu cek sukses/gagal ketat disini, karena kalau rule tidak ada dia akan error, itu wajar.
        run_command(cmd)
        print(f"{Colors.OKGREEN}[HAPUS ] Aturan untuk Port {port} dihapus.{Colors.ENDC}")
        
    print("-" * 60)
    print("Semua blokir telah dibuka. Sharing & RDP kembali normal.")

if __name__ == "__main__":
    # 1. CEK ADMIN OTOMATIS
    if not is_admin():
        # Jika bukan admin, script akan me-restart dirinya sendiri sebagai Admin
        print("Meminta akses Administrator...")
        params = " ".join([f'"{arg}"' for arg in sys.argv])
        ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, params, None, 1)
        sys.exit()

    # 2. MENU INTERAKTIF
    os.system('cls' if os.name == 'nt' else 'clear')
    print(f"{Colors.BOLD}=== FIREWALL MANAGER PRO (RANSOMWARE DEFENSE) ==={Colors.ENDC}")
    print("Skrip ini menggunakan Windows Firewall (Netsh) via Python.\n")
    
    print("[1] LOCKDOWN (Blokir Semua Port Berbahaya)")
    print("[2] RESTORE  (Hapus Blokir / Buka Kembali)")
    print("[X] Keluar")
    
    pilihan = input("\nMasukkan Pilihan (1/2): ").upper()
    
    if pilihan == "1":
        block_ports()
    elif pilihan == "2":
        unblock_ports()
    elif pilihan == "X":
        sys.exit()
    else:
        print("Pilihan tidak valid.")
    
    input("\nTekan Enter untuk keluar...")