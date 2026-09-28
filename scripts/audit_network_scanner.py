import socket
import concurrent.futures
import ipaddress
import time
import sys
import os
import re
from datetime import datetime

# ==============================================================================
# [BAGIAN CONFIG JARINGAN ANDA]
# ==============================================================================
DAFTAR_TARGET = {
    "1": {"nama": "Jaringan Utama (Admin)",    "ip": "192.168.1.0/24"},
    "2": {"nama": "Jaringan MikroTik (Alat)",  "ip": "192.168.88.0/24"},
    "3": {"nama": "Jaringan Server / NAS",     "ip": "10.10.10.0/24"},
    "4": {"nama": "WiFi Tamu / Public",        "ip": "192.168.50.0/24"}
}

TARGET_PORTS = {
    445:  "SMB (WannaCry/Sharing)",
    139:  "NetBIOS",
    3389: "RDP (Remote Desktop)",
    21:   "FTP",
    22:   "SSH",
    80:   "HTTP",
    8080: "HTTP-Alt",
    8291: "WinBox (MikroTik)",
    1433: "MSSQL",
    3306: "MySQL",
    5432: "PostgreSQL"
}

# Warna untuk tampilan Terminal
class Colors:
    HEADER = '\033[95m'
    BLUE = '\033[94m'
    GREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'

# Fungsi untuk membersihkan kode warna saat save ke file
def strip_ansi(text):
    ansi_escape = re.compile(r'\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])')
    return ansi_escape.sub('', text)

# Fungsi Log Hybrid (Print ke Layar + Tulis ke File)
def log(text, file_obj=None, color=""):
    # 1. Print ke Layar (dengan Warna)
    print(f"{color}{text}{Colors.ENDC}")
    
    # 2. Tulis ke File (Tanpa Warna)
    if file_obj:
        clean_text = strip_ansi(text)
        file_obj.write(clean_text + "\n")
        file_obj.flush()

def scan_host(ip):
    open_ports = []
    risk_score = 0
    
    for port, service_name in TARGET_PORTS.items():
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.settimeout(0.4) 
        result = sock.connect_ex((str(ip), port))
        sock.close()
        
        if result == 0:
            open_ports.append(port)
            if port in [445, 3389]: risk_score += 40
            elif port in [1433, 3306]: risk_score += 30
            elif port in [8291, 22]: risk_score += 20
            else: risk_score += 10

    return ip, open_ports, risk_score

def run_advanced_scan(network_cidr):
    # Buat folder khusus log jika belum ada
    if not os.path.exists("Hasil_Audit"):
        os.makedirs("Hasil_Audit")

    # Nama file unik berdasarkan Waktu dan Jaringan
    waktu_skrg = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
    safe_cidr = network_cidr.replace("/", "-") # Ganti tanda / biar valid nama file
    nama_file = f"Hasil_Audit/Laporan_{safe_cidr}_{waktu_skrg}.txt"

    try:
        network = ipaddress.ip_network(network_cidr, strict=False)
    except ValueError:
        print(f"{Colors.FAIL}[!] Format IP Salah!{Colors.ENDC}")
        return

    # Buka File untuk ditulis
    with open(nama_file, "w") as f:
        log(f"\n>>> MEMULAI SCANNING: {network_cidr} <<<", f, Colors.HEADER)
        log(f"Waktu Scan: {datetime.now()}", f)
        log("-" * 60, f)
        
        high_risk_hosts = []
        active_count = 0
        
        with concurrent.futures.ThreadPoolExecutor(max_workers=100) as executor:
            futures = {executor.submit(scan_host, ip): ip for ip in network.hosts()}
            
            for future in concurrent.futures.as_completed(futures):
                ip, open_ports, score = future.result()
                
                if open_ports:
                    active_count += 1
                    if score >= 40:
                        c = Colors.FAIL
                        lbl = "BAHAYA (RANSOMWARE)"
                        high_risk_hosts.append(ip)
                    elif score >= 20:
                        c = Colors.WARNING
                        lbl = "HIGH VALUE"
                    else:
                        c = Colors.GREEN
                        lbl = "INFO"

                    p_list = [str(p) for p in open_ports]
                    pesan = f"[TERPANTAU] {str(ip):<15} | Risk: {score:<3} | {lbl} -> Ports: {','.join(p_list)}"
                    log(pesan, f, c)

        log("-" * 60, f)
        if high_risk_hosts:
            log(f"KESIMPULAN: Ditemukan {len(high_risk_hosts)} host SANGAT RENTAN (Port 445/3389 Terbuka).", f, Colors.FAIL)
            log("DAFTAR IP KRITIS:", f, Colors.FAIL)
            for h in high_risk_hosts:
                log(f"- {h}", f)
        else:
            log("KESIMPULAN: Jaringan Aman. Tidak ada celah kritikal.", f, Colors.GREEN)
        
        log(f"\n[INFO] Laporan lengkap tersimpan di: {nama_file}", f, Colors.BLUE)

if __name__ == "__main__":
    os.system('cls' if os.name == 'nt' else 'clear')
    print(f"{Colors.BOLD}=== AUDIT KEAMANAN JARINGAN (AUTO-SAVE MODE) ==={Colors.ENDC}")
    
    while True:
        print("\nPilih Target Jaringan:")
        for key, info in DAFTAR_TARGET.items():
            print(f"[{key}] {info['nama']} \t-> {info['ip']}")
        
        print("[M] Input Manual")
        print("[A] SCAN SEMUA (Otomatis)")
        print("[X] Keluar")
        
        pilihan = input("\nMasukkan Pilihan Anda: ").upper()
        
        if pilihan in DAFTAR_TARGET:
            run_advanced_scan(DAFTAR_TARGET[pilihan]['ip'])
        elif pilihan == "M":
            manual_ip = input("Masukkan IP Target: ")
            if "/" not in manual_ip: manual_ip += "/24"
            run_advanced_scan(manual_ip)
        elif pilihan == "A":
            print(f"{Colors.WARNING}Scanning semua jaringan...{Colors.ENDC}")
            for key, info in DAFTAR_TARGET.items():
                run_advanced_scan(info['ip'])
        elif pilihan == "X":
            print("Keluar...")
            break
        else:
            print("Pilihan tidak valid.")