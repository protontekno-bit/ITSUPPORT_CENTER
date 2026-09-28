using System;
using System.Collections.Generic;

namespace ITSupportCenter.Core
{
    public class ToolDetailInfo
    {
        public string ProblemSolved { get; set; } = string.Empty;
        public string SystemImpact { get; set; } = string.Empty;
        public string UsageGuide { get; set; } = string.Empty;
    }

    public static class ToolDetailInfoProvider
    {
        private static readonly Dictionary<string, ToolDetailInfo> _details = new(StringComparer.OrdinalIgnoreCase)
        {
            // === PRINTER SHARING & SPOOLER ===
            ["print_fix_sharing"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatasi error sharing printer Windows 10/11 'Operation could not be completed (error 0x0000011b atau 0x00000709)' akibat security patch RPC Windows Update.",
                SystemImpact = "Menambahkan registry key 'RpcAuthnLevelPrivacyEnabled=0' di Print Server dan mengaktifkan Point-and-Print Driver Restriction Policy. Tidak menghapus driver printer.",
                UsageGuide = "1. Klik tombol jalankan pada PC Client dan PC Host Printer.\n2. Lakukan Restart pada komputer host dan client.\n3. Hubungkan ulang printer sharing melalui Run (\\\\IP_HOST\\NamaPrinter)."
            },
            ["print_clear_queue"] = new ToolDetailInfo
            {
                ProblemSolved = "Dokumen cetak macet di antrean, status printer 'Deleting' atau 'Error - Printing', dan printer tidak mau mencetak dokumen baru.",
                SystemImpact = "Menghentikan service Spooler sementara, menghapus seluruh berkas antrean (.SHD & .SPL) di C:\\Windows\\System32\\spool\\PRINTERS, lalu me-restart service Spooler.",
                UsageGuide = "1. Pastikan tidak ada dokumen fisik yang sedang ditarik oleh printer.\n2. Jalankan tool ini 1-klik.\n3. Antrean cetak langsung bersih dan siap digunakan."
            },
            ["print_spooler_factory_reset"] = new ToolDetailInfo
            {
                ProblemSolved = "Service Print Spooler mati sendiri secara berulang (Crash Loop), tidak bisa di-start, atau error 'Spooler SubSystem App stopped working'.",
                SystemImpact = "Mereset Print Monitors dan Print Processors registry ke setelan default Windows, membersihkan driver filter korup, dan me-restart dependencies RPC/Spooler.",
                UsageGuide = "1. Jalankan tool ini saat spooler crash terus-menerus.\n2. Setelah reset selesai, install ulang driver printer resmi jika printer belum terdeteksi."
            },
            ["print_audit_test_page"] = new ToolDetailInfo
            {
                ProblemSolved = "Memverifikasi apakah komunikasi driver dan hardware printer fisik berjalan normal tanpa perlu membuka software Office.",
                SystemImpact = "Membaca seluruh printer yang terpasang melalui WMI, memeriksa status online/offline, dan mengirimkan sinyal perintah Windows Test Page resmi.",
                UsageGuide = "1. Pastikan printer menyala dan terisi kertas.\n2. Pilih nomor printer yang ingin diuji saat dialog muncul."
            },
            ["print_open_cpl"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuka Control Panel Devices and Printers klasik di Windows 11 yang sering dialihkan secara paksa ke Modern Settings.",
                SystemImpact = "Membuka shell klasik 'shell:::{A8A91A66-3A7D-4424-8D24-04E180695C7A}'. Tidak mengubah setelan sistem.",
                UsageGuide = "Gunakan untuk mengatur Default Printer, Print Server Properties, atau menghapus port printer lama."
            },

            // === NETWORK & SHARING ===
            ["net_full_repair"] = new ToolDetailInfo
            {
                ProblemSolved = "Status 'No Internet, Secured', Wi-Fi terhubung tapi tidak bisa browsing, adapter jaringan error, atau IP 169.254.x.x (APIPA).",
                SystemImpact = "Mereset Winsock Catalog, TCP/IP stack (netsh int ip reset), Flush DNS, membersihkan NetBIOS cache, reset WinHTTP proxy, dan merilis DHCP (ipconfig /renew).",
                UsageGuide = "1. Simpan pekerjaan yang membutuhkan internet.\n2. Jalankan tool ini. Tunggu hingga proses reset selesai dalam beberapa detik.\n3. Disarankan restart komputer jika diminta."
            },
            ["net_smb_guest_auth"] = new ToolDetailInfo
            {
                ProblemSolved = "Gagal mengakses folder sharing kantor/NAS dengan error: 'You can't access this shared folder because your organization's security policies block unauthenticated guest access' (0x800704f8).",
                SystemImpact = "Mengaktifkan policy registry 'AllowInsecureGuestAuth=1' pada LanmanWorkstation di Windows 10/11 Pro & Enterprise.",
                UsageGuide = "1. Jalankan tool ini pada PC yang gagal membuka folder sharing.\n2. Akses kembali folder share target (\\\\192.168.x.x\\folder)."
            },
            ["net_ad_domain_assistant"] = new ToolDetailInfo
            {
                ProblemSolved = "Proses bergabung ke Domain Active Directory kantor yang rumit, ganti nama komputer, atau gagal connect ke Domain Controller.",
                SystemImpact = "Mengeksekusi PowerShell Add-Computer / Rename-Computer / Remove-Computer dengan manajemen kredensial admin dan uji port AD (53, 88, 389, 445).",
                UsageGuide = "1. Pastikan DNS komputer mengarah ke IP Domain Controller.\n2. Pilih menu [5] Uji Koneksi DC untuk memastikan port terbuka.\n3. Pilih menu [3] Gabung ke Domain dan masukkan kredensial Admin Domain."
            },
            ["net_connection_diagnostics"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencari tahu penyebab pasti internet lemot atau putus: apakah masalah di Kabel/Wi-Fi (Gateway), ISP Kantor, atau Server Global.",
                SystemImpact = "Melakukan uji ping 3-titik bertingkat (Router Gateway Lokal, DNS ISP, dan Server Global 8.8.8.8/1.1.1.1) serta menghitung packet loss & latency.",
                UsageGuide = "Jalankan saat internet kantor dilaporkan lambat untuk mengetahui apakah harus lapor ke admin router lokal atau komplain ke provider ISP."
            },
            ["net_dns_switch"] = new ToolDetailInfo
            {
                ProblemSolved = "Website tertentu tidak bisa dibuka (DNS Filtering/Error), browsing lambat, atau ingin beralih cepat ke DNS Google / Cloudflare / DHCP otomatis.",
                SystemImpact = "Mengubah setelan Static DNS pada semua adapter jaringan aktif menggunakan netsh / PowerShell.",
                UsageGuide = "Pilih opsi 1 untuk Google DNS (8.8.8.8), opsi 2 untuk Cloudflare (1.1.1.1), atau opsi 3 untuk DHCP otomatis."
            },
            ["net_reset_proxy_hosts"] = new ToolDetailInfo
            {
                ProblemSolved = "Browser tiba-tiba tidak bisa internet karena terpasang Proxy terselubung oleh malware/adware, atau file hosts dibajak.",
                SystemImpact = "Menghapus pengaturan proxy WinINet di registry User & System, serta merestore berkas C:\\Windows\\System32\\drivers\\etc\\hosts ke standar Microsoft.",
                UsageGuide = "Gunakan saat Chrome/Edge menampilkan error 'Proxy server is refusing connections'."
            },

            // === SECURITY & FIREWALL ===
            ["sec_emergency_quarantine"] = new ToolDetailInfo
            {
                ProblemSolved = "Komputer terinfeksi Ransomware / Malware agresif yang sedang menyebar ke PC lain di jaringan kantor (Lateral Movement).",
                SystemImpact = "Mengubah kebijakan Windows Firewall menjadi 'blockinbound,blockoutbound' (Air-Gap Isolation). Memutus 100% lalu lintas LAN & Internet tanpa mematikan driver kartu jaringan (tool internal tetap bisa berjalan).",
                UsageGuide = "1. Mode Darurat: Pilih Opsi 1 untuk mengisolasi PC seketika.\n2. Bersihkan malware.\n3. Mode Normalisasi: Pilih Opsi 2 untuk membuka kembali koneksi setelah PC bersih."
            },
            ["sec_hardening_ransomware"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencegah eksploitasi celah keamanan SMBv1 (WannaCry), infeksi virus via Flashdisk USB AutoPlay, dan akses Remote Registry liar.",
                SystemImpact = "Menonaktifkan SMBv1 protocol, mematikan USB AutoRun/AutoPlay via registry, dan mendisable service RemoteRegistry.",
                UsageGuide = "Sangat direkomendasikan dijalankan pada seluruh PC kantor baru sebagai standar hardening dasar."
            },
            ["sec_bitlocker_audit"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencari dan mencatat kunci pemulihan 48-Digit BitLocker sebelum motherboard/SSD diganti atau sebelum update BIOS.",
                SystemImpact = "Mengeksekusi 'manage-bde -protectors -get C:' untuk membaca Numerical Password protector. Hanya membaca, tidak mengubah enkripsi.",
                UsageGuide = "Jalankan dan catat 48-digit angka yang muncul. Simpan di tempat aman sebelum melakukan perbaikan hardware."
            },
            ["sec_reset_firewall"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows Firewall berantakan, terlalu banyak rule usang yang membingungkan, port bocor, atau service firewall error.",
                SystemImpact = "Mereset total konfigurasi Firewall ke setelan pabrik default ('netsh advfirewall reset') dan me-restart service mpssvc.",
                UsageGuide = "Gunakan saat ingin memulai konfigurasi firewall dari nol dalam kondisi bersih dan aman."
            },
            ["sec_firewall_port_manager"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuka port layanan kantor (RDP 3389, Web 80/443, Database SQL 1433/3306, atau port custom) secara cepat dan teratur di Firewall.",
                SystemImpact = "Menambahkan rule inbound allow pada Windows Firewall sesuai nomor port yang dipilih.",
                UsageGuide = "Pilih nomor layanan yang ingin dibuka pada dialog yang muncul."
            },
            ["sec_toggle_firewall_profiles"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengecek status aktif profil Firewall (Domain, Private, Public) dan menyalakan/mematikan profil tertentu untuk kebutuhan pengujian.",
                SystemImpact = "Menjalankan perintah 'netsh advfirewall set [profile] state on/off'.",
                UsageGuide = "Gunakan untuk audit keamanan jaringan atau pengujian koneksi sementara."
            },

            // === REMOTE DESKTOP ===
            ["remote_rustdesk_manager_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Solusi remote desktop mandiri open-source tanpa limit komersial, konfigurasi relay server kantor sendiri, dan instalasi instan.",
                SystemImpact = "Mendukung auto-deteksi instalasi, reset config/ID jika terjadi duplikasi hasil kloning OS, pasang via Winget, dan konfigurasi server relay.",
                UsageGuide = "Pilih menu [1] Buka, [2] Reset ID, [3] Set Server Relay Kantor, atau [4] Install otomatis via Winget."
            },
            ["remote_reset_anydesk_id"] = new ToolDetailInfo
            {
                ProblemSolved = "AnyDesk ID bentrok antara 2 komputer hasil kloning OS, atau terkena limit lisensi AnyDesk.",
                SystemImpact = "Menghentikan proses AnyDesk, menghapus service.conf dan system.conf di %ProgramData% dan %AppData%, lalu me-restart service AnyDesk.",
                UsageGuide = "Jalankan tool ini, lalu buka kembali AnyDesk untuk mendapatkan nomor ID 9-digit baru yang unik."
            },
            ["remote_change_tv_pass"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatur password permanen TeamViewer untuk remote support tanpa pengawasan (Unattended Access) langsung lewat CLI.",
                SystemImpact = "Menyetel konfigurasi password TeamViewer melalui argumen command-line resmi TeamViewer.",
                UsageGuide = "Masukkan password baru yang diinginkan pada prompt yang muncul."
            },

            // === SYSTEM MAINTENANCE & RESCUE ===
            ["sys_deep_hardware_inspector"] = new ToolDetailInfo
            {
                ProblemSolved = "Membaca 100% informasi hardware fisik komputer secara mendalam (Tipe SSD/NVMe, S.M.A.R.T Health, Slot & Speed RAM MHz, TPM 2.0, BIOS, Baterai) tanpa software pihak ketiga.",
                SystemImpact = "Mengeksekusi query WMI/CIM mendalam dan PowerShell hardware collector. Murni read-only (100% aman).",
                UsageGuide = "Jalankan saat melakukan audit aset hardware, pengecekan upgrade RAM/SSD, atau diagnosa kesehatan PC."
            },
            ["sys_auto_health_check"] = new ToolDetailInfo
            {
                ProblemSolved = "Pemeriksaan kesehatan sistem cepat 1-klik untuk mengetahui status Print Spooler, SMB Service, Port RDP, Disk Free Space, dan Firewall.",
                SystemImpact = "Memeriksa status service dan konfigurasi penting Windows secara otomatis dalam 2 detik. Read-only.",
                UsageGuide = "Jalankan sebagai langkah awal diagnosa saat menerima tiket keluhan dari user kantor."
            },
            ["sys_clean_temp"] = new ToolDetailInfo
            {
                ProblemSolved = "Drive C: merah / penuh akibat penumpukan file temporary, cache Windows Update, dan file sampah aplikasi.",
                SystemImpact = "Menghapus file sampah di %TEMP%, C:\\Windows\\Temp, dan C:\\Windows\\SoftwareDistribution\\Download secara aman.",
                UsageGuide = "Jalankan secara berkala untuk melegakan kapasitas penyimpanan sistem Windows."
            },
            ["sys_sfc_dism"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows sering freeze, blue screen (BSOD), file sistem korup, atau aplikasi bawaan Windows crash.",
                SystemImpact = "Menjalankan 'sfc /scannow' untuk memperbaiki file sistem dan 'DISM /Online /Cleanup-Image /RestoreHealth' untuk memulihkan komponen Windows Component Store.",
                UsageGuide = "Proses membutuhkan waktu 5-15 menit. Biarkan proses berjalan hingga selesai 100%."
            },
            ["sys_disk_smart_health"] = new ToolDetailInfo
            {
                ProblemSolved = "Mendeteksi tanda-tanda kerusakan fisik (Predictive Failure) pada SSD atau HDD sebelum data hilang.",
                SystemImpact = "Membaca sensor S.M.A.R.T harddisk melalui WMI Storage Subsystem. Read-only.",
                UsageGuide = "Jika status menunjukkan 'BAD' atau 'Predictive Failure', segera lakukan backup data penting pengguna."
            },
            ["sys_live_rescue_toolkit"] = new ToolDetailInfo
            {
                ProblemSolved = "Panduan terorganisir perbaikan komputer mati total, bluescreen booting, reset password Windows SAM offline menggunakan WinPE (Hiren's, DLC Boot, SystemRescue, UBCD).",
                SystemImpact = "Menyediakan panduan taktis lapangan, download link resmi, dan prosedur perbaikan bare-metal.",
                UsageGuide = "Pilih toolkit WinPE yang ingin dipelajari dan ikuti instruksi langkah perbaikan."
            },
            ["sys_disk_cloning_deployment"] = new ToolDetailInfo
            {
                ProblemSolved = "Panduan kloning massal puluhan PC kantor baru, backup full image ke NAS (Clonezilla/Rescuezilla), dan deployment jaringan PXE (FOG Project).",
                SystemImpact = "Menyediakan panduan langkah demi langkah skenario Device-to-Device dan Device-to-Image.",
                UsageGuide = "Gunakan saat mempersiapkan pergantian SSD massal atau backup berkala server."
            },
            ["sys_ventoy_guide"] = new ToolDetailInfo
            {
                ProblemSolved = "Cara membuat 1 flashdisk multi-boot yang bisa memuat puluhan file ISO Windows, Linux, dan WinPE sekaligus tanpa format ulang.",
                SystemImpact = "Menyediakan panduan instalasi Ventoy, konfigurasi folder, dan tips kompatibilitas UEFI / Legacy BIOS.",
                UsageGuide = "Cukup pasang Ventoy ke flashdisk sekali, lalu copy file ISO langsung ke dalam flashdisk."
            },
            ["sys_smart_ram_optimizer"] = new ToolDetailInfo
            {
                ProblemSolved = "Penggunaan RAM tinggi/bengkak (>80%), PC terasa lambat atau lagging, banyak memori cache idle tertahan, serta background updaters/telemetry yang berjalan diam-diam.",
                SystemImpact = "Memanggil Windows API EmptyWorkingSet untuk melepaskan cache RAM idle dari proses aktif tanpa menutup aplikasi pengguna, serta menghentikan background updater/telemetry non-esensial dengan proteksi kernel Windows.",
                UsageGuide = "1. Klik tombol 'Optimalkan RAM & Proses'.\n2. Tool akan memindai status RAM, mendeteksi proses terberat, dan memangkas memori idle secara instan.\n3. Pantau pembebasan RAM pada Terminal Log utama."
            },
            ["remote_rdp_manager_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows Remote Desktop (RDP) tidak bisa diakses, port 3389 terblokir firewall, error NLA (Network Level Authentication), atau teknisi ingin membimbing layar user tanpa me-logout sesi aktif.",
                SystemImpact = "Mengatur fDenyTSConnections pada Terminal Server registry, mengonfigurasi Service TermService ke Automatic, mengatur aturan Windows Firewall port 3389/custom, dan mengaktifkan RDP Shadowing policy.",
                UsageGuide = "1. Klik tombol 'Pusat Utilitas Windows RDP'.\n2. Pilih opsi 1 untuk mengaktifkan RDP Host penuh dan buka firewall secara instan.\n3. Pilih opsi 5 untuk Quick Connect RDP Client ke PC target.\n4. Pilih opsi 6 untuk Remote Shadowing layar pengguna tanpa logout."
            },

            // === LICENSE AUDIT ===
            ["lic_audit_win_license"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengecek apakah Windows berlisensi Asli (Retail / OEM) atau bajakan/KMS yang akan kedaluwarsa dalam 180 hari.",
                SystemImpact = "Mengeksekusi 'slmgr /dli' dan 'slmgr /xpr' untuk membaca status aktivasi resmi Microsoft.",
                UsageGuide = "Jalankan untuk audit kepatuhan lisensi perangkat lunak di perusahaan."
            },
            ["lic_extract_oem_key"] = new ToolDetailInfo
            {
                ProblemSolved = "Membaca kunci lisensi asli Windows yang tertanam di chip motherboard (MSDM Table BIOS) laptop/PC branded (Dell, HP, Lenovo, Asus).",
                SystemImpact = "Membaca ACPI MSDM table via WMI/PowerShell. 100% aman dan read-only.",
                UsageGuide = "Gunakan saat ingin menginstal ulang komputer agar Windows langsung teraktivasi otomatis dengan lisensi pabrik."
            },
            ["lic_clean_kms"] = new ToolDetailInfo
            {
                ProblemSolved = "Menghapus residu server KMS bajakan, mereset token aktivasi (tokens.dat) yang rusak, dan mengatasi error aktivasi 0xC004C003.",
                SystemImpact = "Membersihkan KMS IP di registry, mengembalikan KMS port ke default, dan merefresh file token lisensi Windows.",
                UsageGuide = "Jalankan sebelum memasukkan lisensi Windows Retail/OEM yang baru."
            },
            ["lic_diagnose_office"] = new ToolDetailInfo
            {
                ProblemSolved = "Mendiagnosa lisensi Microsoft Office (2016/2019/2021/365), melihat 5-digit terakhir product key, atau mencabut lisensi Office lama.",
                SystemImpact = "Mengeksekusi script resmi Microsoft 'OSPP.VBS /dstatus' di folder Program Files Office.",
                UsageGuide = "Gunakan untuk melihat sisa masa aktif Office atau menghapus key Office yang bentrok."
            },
            ["lic_toggle_office_update_lock"] = new ToolDetailInfo
            {
                ProblemSolved = "Update otomatis Microsoft Office sering merusak aktivasi lisensi KMS/Volume, memunculkan notifikasi lisensi bajakan, atau merusak macro Excel/VBA.",
                SystemImpact = "Mengatur UpdatesEnabled='False' pada ClickToRun Configuration, mengunci GPO EnableAutomaticUpdates=0, dan menonaktifkan Task Scheduler background Office Automatic Updates 2.0.",
                UsageGuide = "1. Klik tombol 'Kunci / Buka Update Office'.\n2. Klik 1 kali untuk mengunci dan mematikan update permanen.\n3. Klik sekali lagi di kemudian hari jika ingin mengaktifkan kembali pembaruan normal."
            },
            ["sys_office_rescue_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatasi Word, Excel, PowerPoint, atau Outlook yang crash, freeze di splash screen, error tampilan/startup Outlook ('Cannot open window'), template default Normal.dotm korup, atau dokumen macet 'Upload Failed' / 'File Locked'.",
                SystemImpact = "Menyediakan opsi Safe Mode (/safe), pemulihan Navigation Pane Outlook (/resetnavpane), peluncur otomatis scanpst.exe, regenerasi template Word/Excel, pembersihan OfficeFileCache, dan pemicu Click-to-Run Quick Repair resmi.",
                UsageGuide = "1. Klik tombol 'Buka Rescue Hub Office & Outlook'.\n2. Pilih nomor opsi sesuai masalah yang dialami (1-6).\n3. Ikuti panduan di log terminal atau jendela Microsoft yang terbuka."
            },
            ["lic_office_conflict_cleaner"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatasi banner kuning 'Unlicensed Product' atau 'Activation Failed' akibat adanya sisa lisensi trial/grace lama yang bentrok di ospp.vbs, serta mengatasi loop login akun kantor/sekolah yang macet.",
                SystemImpact = "Memindai lisensi via OSPP.VBS, menghapus key spesifik dengan /unpkey:XXXXX, dan mereset token Modern Authentication (OneAuth, IdentityCache, dan kredensial Windows).",
                UsageGuide = "1. Klik tombol 'Pembersih Lisensi & Akun Office'.\n2. Pilih opsi 1 untuk memindai key sisa.\n3. Pilih opsi 2 dan masukkan 5 karakter terakhir key yang ingin dihapus, atau pilih opsi 3/4 untuk mereset cache login."
            },
            ["lic_switch_edition"] = new ToolDetailInfo
            {
                ProblemSolved = "Meng-upgrade Windows 10/11 Home ke edisi Professional (Pro) tanpa perlu instal ulang atau format ulang.",
                SystemImpact = "Memicu utilitas resmi Windows changepk.exe menggunakan generic activation switch key.",
                UsageGuide = "Pastikan komputer terhubung ke internet saat proses upgrade edisi berlangsung."
            }
        };

        public static ToolDetailInfo GetDetail(IToolCommand tool)
        {
            if (_details.TryGetValue(tool.Id, out var info))
            {
                return info;
            }

            // Smart Fallback jika tool belum didefinisikan secara khusus
            return new ToolDetailInfo
            {
                ProblemSolved = $"Menangani permasalahan dan optimalisasi pada kategori '{tool.Category}': {tool.Description}",
                SystemImpact = $"Mengeksekusi perintah sistem terstandarisasi untuk '{tool.Title}' menggunakan utilitas internal Windows tanpa merusak integritas sistem.",
                UsageGuide = "1. Klik tombol jalankan pada kartu atau dialog ini.\n2. Pantau proses eksekusi melalui Terminal Log utama.\n3. Ikuti prompt interaktif jika tool membutuhkan input parameter."
            };
        }
    }
}
