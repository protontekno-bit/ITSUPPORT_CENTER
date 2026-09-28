import winreg
import ctypes
import sys
import subprocess
import time

def is_admin():
    """Periksa apakah script berjalan dengan hak administrator"""
    try:
        return ctypes.windll.shell32.IsUserAnAdmin()
    except Exception:
        return False

def enable_insecure_guest_auth():
    """Mengaktifkan akses tamu tidak aman melalui registry (Policies dan Services)"""
    key_paths = [
        r"SOFTWARE\Policies\Microsoft\Windows\LanmanWorkstation",
        r"SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters"
    ]
    success_count = 0
    for key_path in key_paths:
        try:
            key = winreg.CreateKeyEx(winreg.HKEY_LOCAL_MACHINE, key_path, 0, winreg.KEY_WRITE)
            winreg.SetValueEx(key, "AllowInsecureGuestAuth", 0, winreg.REG_DWORD, 1)
            winreg.CloseKey(key)
            print(f"[SUKSES] Registry diubah: HKLM\\{key_path} -> AllowInsecureGuestAuth = 1")
            success_count += 1
        except Exception as e:
            print(f"[WARNING] Gagal mengubah registry HKLM\\{key_path}: {e}")
            
    return success_count > 0

def restart_workstation_service():
    """Restart Workstation service untuk menerapkan perubahan"""
    try:
        print("[INFO] Merestart Workstation service...")
        subprocess.run(["net", "stop", "LanmanWorkstation"], check=True, capture_output=True)
        time.sleep(2)
        subprocess.run(["net", "start", "LanmanWorkstation"], check=True, capture_output=True)
        print("[SUKSES] Workstation service berhasil di-restart")
        return True
    except subprocess.CalledProcessError as e:
        print(f"[WARNING] Gagal restart service: {e.stderr.decode().strip()}")
        return False

def flush_dns_and_reset_network():
    """Flush DNS dan reset stack jaringan"""
    commands = [
        ["ipconfig", "/flushdns"],
        ["ipconfig", "/release"],
        ["ipconfig", "/renew"],
        ["netsh", "winsock", "reset", "catalog"],
        ["netsh", "int", "ip", "reset"],
    ]
    
    for cmd in commands:
        try:
            subprocess.run(cmd, check=True, capture_output=True)
            print(f"[INFO] Berhasil menjalankan: {' '.join(cmd)}")
        except subprocess.CalledProcessError:
            print(f"[WARNING] Gagal menjalankan: {' '.join(cmd)}")

def test_network_share(ip_address):
    """Uji koneksi ke shared folder"""
    try:
        print(f"[INFO] Menguji koneksi ke \\\\{ip_address}...")
        result = subprocess.run(["ping", "-n", "2", ip_address], 
                               capture_output=True, text=True, timeout=10)
        
        if "TTL=" in result.stdout:
            print(f"[SUKSES] IP {ip_address} dapat dihubungi")
            
            # Coba akses dengan net view
            net_view = subprocess.run(["net", "view", f"\\\\{ip_address}"], 
                                     capture_output=True, text=True, timeout=15)
            
            if net_view.returncode == 0:
                print(f"[SUKSES] Berhasil mengakses share di \\\\{ip_address}")
                print("\nDaftar share:")
                print(net_view.stdout)
            else:
                print(f"[INFO] Koneksi OK, tapi tidak bisa list shares: {net_view.stderr}")
        else:
            print(f"[ERROR] Tidak bisa ping ke {ip_address}")
            
    except Exception as e:
        print(f"[ERROR] Tes koneksi gagal: {e}")

def main():
    print("=" * 60)
    print("PERBAIKAN ERROR 0x800704f8 - GUEST ACCESS DIBLOKIR")
    print("=" * 60)
    
    # Periksa hak admin
    if not is_admin():
        print("[INFO] Memerlukan hak administrator...")
        ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, f'"{__file__}"', None, 1)
        sys.exit()
    
    print("[INFO] Script berjalan dengan hak administrator\n")
    
    # Minta input alamat IP
    ip_address = input("Masukkan alamat IP shared folder (contoh: 192.168.1.194): ").strip()
    if not ip_address:
        ip_address = "192.168.1.194"
        print(f"[INFO] Menggunakan IP default: {ip_address}")
    
    # 1. Ubah registry
    if not enable_insecure_guest_auth():
        print("[ERROR] Tidak bisa melanjutkan karena registry gagal diubah")
        return
    
    # 2. Restart Workstation service
    restart_workstation_service()
    
    # 3. Reset jaringan
    print("\n[INFO] Melakukan reset konfigurasi jaringan...")
    flush_dns_and_reset_network()
    
    # 4. Tes koneksi
    print("\n" + "=" * 60)
    print("HASIL TES KONEKSI")
    print("=" * 60)
    test_network_share(ip_address)
    
    # 5. Saran tambahan
    print("\n" + "=" * 60)
    print("SARAN TAMBAHAN")
    print("=" * 60)
    print("1. Jika masih tidak bisa, coba restart komputer")
    print("2. Pastikan sharing di komputer target sudah aktif")
    print("3. Coba akses dengan net use:")
    print(f'   net use Z: \\\\{ip_address}\\sharename /user:username password')
    print("4. Di komputer target, pastikan:")
    print("   - File sharing aktif di Windows Defender Firewall")
    print("   - Network discovery diaktifkan")
    print("   - Password Protected Sharing dimatikan (jika tidak butuh password)")

if __name__ == "__main__":
    main()