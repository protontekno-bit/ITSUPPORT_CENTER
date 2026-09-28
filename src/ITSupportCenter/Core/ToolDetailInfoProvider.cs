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
            // === DIAGNOSIS & HEALTH CHECK ===
            ["auto_health_check"] = new ToolDetailInfo
            {
                ProblemSolved = "Pemeriksaan kesehatan sistem cepat 1-klik untuk mengetahui status Print Spooler, SMB Service, Port RDP, Disk Free Space, dan Firewall.",
                SystemImpact = "Memeriksa status service dan konfigurasi penting Windows secara otomatis dalam 2 detik. Read-only (100% aman).",
                UsageGuide = "Jalankan sebagai langkah awal diagnosa saat menerima tiket keluhan dari user kantor."
            },
            ["sys_pc_asset_info"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengumpulkan informasi inventaris lengkap PC (Serial Number, Model Motherboard, CPU, RAM, Sisa Disk C/D, IP, MAC Address) untuk pelaporan aset IT kantor.",
                SystemImpact = "Membaca WMI System Information dan menyajikan ringkasan rapi yang dapat langsung disalin ke clipboard.",
                UsageGuide = "Jalankan saat pendataan aset baru, audit inventaris IT tahunan, atau verifikasi spesifikasi laptop user."
            },

            // === PRINTER SHARING & SPOOLER ===
            ["fix_printer_sharing"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatasi error sharing printer Windows 10/11 'Operation could not be completed (error 0x0000011b atau 0x00000709)' akibat security patch RPC Windows Update.",
                SystemImpact = "Menambahkan registry key 'RpcAuthnLevelPrivacyEnabled=0' di Print Server dan mengaktifkan Point-and-Print Driver Restriction Policy. Tidak menghapus driver printer.",
                UsageGuide = "1. Klik tombol jalankan pada PC Client dan PC Host Printer.\n2. Lakukan Restart pada komputer host dan client.\n3. Hubungkan ulang printer sharing melalui Run (\\\\IP_HOST\\NamaPrinter)."
            },
            ["clear_print_queue"] = new ToolDetailInfo
            {
                ProblemSolved = "Dokumen cetak macet di antrean, status printer 'Deleting' atau 'Error - Printing', dan printer tidak mau mencetak dokumen baru.",
                SystemImpact = "Menghentikan service Spooler sementara, menghapus seluruh berkas antrean (.SHD & .SPL) di C:\\Windows\\System32\\spool\\PRINTERS, lalu me-restart service Spooler.",
                UsageGuide = "1. Pastikan tidak ada dokumen fisik yang sedang ditarik oleh printer.\n2. Jalankan tool ini 1-klik.\n3. Antrean cetak langsung bersih dan siap digunakan."
            },
            ["printer_spooler_factory_reset"] = new ToolDetailInfo
            {
                ProblemSolved = "Service Print Spooler mati sendiri secara berulang (Crash Loop), tidak bisa di-start, atau error 'Spooler SubSystem App stopped working'.",
                SystemImpact = "Mereset Print Monitors dan Print Processors registry ke setelan default Windows, membersihkan driver filter korup, dan me-restart dependencies RPC/Spooler.",
                UsageGuide = "1. Jalankan tool ini saat spooler crash terus-menerus.\n2. Setelah reset selesai, install ulang driver printer resmi jika printer belum terdeteksi."
            },
            ["printer_audit_test_print"] = new ToolDetailInfo
            {
                ProblemSolved = "Memverifikasi apakah komunikasi driver dan hardware printer fisik berjalan normal tanpa perlu membuka software Office.",
                SystemImpact = "Membaca seluruh printer yang terpasang melalui WMI, memeriksa status online/offline, dan mengirimkan sinyal perintah Windows Test Page resmi.",
                UsageGuide = "1. Pastikan printer menyala dan terisi kertas.\n2. Pilih nomor printer yang ingin diuji saat dialog muncul."
            },
            ["printer_open_classic_control"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuka Control Panel Devices and Printers klasik di Windows 11 yang sering dialihkan secara paksa ke Modern Settings.",
                SystemImpact = "Membuka shell klasik 'shell:::{A8A91A66-3A7D-4424-8D24-04E180695C7A}'. Tidak mengubah setelan sistem.",
                UsageGuide = "Gunakan untuk mengatur Default Printer, Print Server Properties, atau menghapus port printer lama."
            },

            // === NETWORK & SHARING ===
            ["net_full_network_repair"] = new ToolDetailInfo
            {
                ProblemSolved = "Status 'No Internet, Secured', Wi-Fi terhubung tapi tidak bisa browsing, adapter jaringan error, atau IP 169.254.x.x (APIPA).",
                SystemImpact = "Mereset Winsock Catalog, TCP/IP stack (netsh int ip reset), Flush DNS, membersihkan NetBIOS cache, reset WinHTTP proxy, dan merilis DHCP (ipconfig /renew).",
                UsageGuide = "1. Simpan pekerjaan yang membutuhkan internet.\n2. Jalankan tool ini. Tunggu hingga proses reset selesai dalam beberapa detik.\n3. Disarankan restart komputer jika diminta."
            },
            ["fix_smb_guest_auth"] = new ToolDetailInfo
            {
                ProblemSolved = "Gagal mengakses folder sharing kantor/NAS dengan error: 'You can't access this shared folder because your organization's security policies block unauthenticated guest access' (0x800704f8).",
                SystemImpact = "Mengaktifkan policy registry 'AllowInsecureGuestAuth=1' pada LanmanWorkstation di Windows 10/11 Pro & Enterprise.",
                UsageGuide = "1. Jalankan tool ini pada PC yang gagal membuka folder sharing.\n2. Akses kembali folder share target (\\\\192.168.x.x\\folder)."
            },
            ["diagnose_smb_target"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengecek apakah server file sharing, NAS, atau PC tujuan menyala, merespons ping, dan membuka port SMB 445 / 139.",
                SystemImpact = "Melakukan uji konektivitas soket TCP port 445/139 dan ping ICMP ke host target secara non-destruktif.",
                UsageGuide = "Masukkan IP atau nama komputer target (contoh: 192.168.1.100 atau SERVER-FILE) pada prompt input."
            },
            ["net_quality_jitter_tester"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencari tahu penyebab pasti internet lemot atau putus: apakah masalah di Kabel/Wi-Fi (Gateway), ISP Kantor, atau Server Global.",
                SystemImpact = "Melakukan uji ping 3-titik bertingkat (Router Gateway Lokal, DNS ISP, dan Server Global 8.8.8.8/1.1.1.1) serta menghitung packet loss & latency.",
                UsageGuide = "Jalankan saat internet kantor dilaporkan lambat untuk mengetahui apakah harus lapor ke admin router lokal atau komplain ke provider ISP."
            },
            ["net_audit_ip_netstat"] = new ToolDetailInfo
            {
                ProblemSolved = "Melihat konfigurasi IP, subnet mask, gateway, DNS aktif, serta daftar port listening dan koneksi jaringan TCP/UDP yang sedang terhubung.",
                SystemImpact = "Mengeksekusi ipconfig /all dan netstat -ano untuk inspeksi konektivitas jaringan secara aman.",
                UsageGuide = "Jalankan saat investigasi port asing, trojan network connection, atau pengecekan konfigurasi adapter."
            },
            ["net_open_connections_cpl"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuka Network Connections klasik (ncpa.cpl) secara instan tanpa harus tersesat di menu Modern Settings Windows 11.",
                SystemImpact = "Meluncurkan ncpa.cpl langsung. Tidak mengubah konfigurasi.",
                UsageGuide = "Gunakan untuk mengatur IP statis, ganti DNS adapter, atau enable/disable kartu jaringan LAN/Wi-Fi."
            },
            ["reset_network_cache"] = new ToolDetailInfo
            {
                ProblemSolved = "Website tidak ter-update akibat DNS cache usang, folder share tidak bisa dibuka karena sesi NetBIOS/SMB lama tersangkut.",
                SystemImpact = "Menjalankan ipconfig /flushdns, nbtstat -R, nbtstat -RR, dan klist purge untuk membersihkan tiket autentikasi jaringan lama.",
                UsageGuide = "Gunakan saat baru saja mengganti password domain atau mengubah DNS record server kantor."
            },
            ["reset_network_stack"] = new ToolDetailInfo
            {
                ProblemSolved = "Koneksi internet hilang total setelah uninstall VPN / antivirus pihak ketiga, atau socket TCP/IP rusak.",
                SystemImpact = "Mereset Winsock Catalog dan TCP/IP stack ke setelan pabrik murni menggunakan netsh.",
                UsageGuide = "Jalankan saat internet tidak bisa tersambung sama sekali meski adapter terbaca terhubung."
            },
            ["net_reset_proxy_hosts"] = new ToolDetailInfo
            {
                ProblemSolved = "Browser tiba-tiba tidak bisa internet karena terpasang Proxy terselubung oleh malware/adware, atau file hosts dibajak.",
                SystemImpact = "Menghapus pengaturan proxy WinINet di registry User & System, serta merestore berkas C:\\Windows\\System32\\drivers\\etc\\hosts ke standar Microsoft.",
                UsageGuide = "Gunakan saat Chrome/Edge menampilkan error 'Proxy server is refusing connections'."
            },
            ["net_dns_quick_switch"] = new ToolDetailInfo
            {
                ProblemSolved = "Website tertentu tidak bisa dibuka (DNS Filtering/Error), browsing lambat, atau ingin beralih cepat ke DNS Google / Cloudflare / DHCP otomatis.",
                SystemImpact = "Mengubah setelan Static DNS pada semua adapter jaringan aktif menggunakan netsh / PowerShell.",
                UsageGuide = "Pilih opsi 1 untuk Google DNS (8.8.8.8), opsi 2 untuk Cloudflare (1.1.1.1), atau opsi 3 untuk DHCP otomatis."
            },
            ["net_wifi_signal_audit"] = new ToolDetailInfo
            {
                ProblemSolved = "Menganalisis kekuatan sinyal Wi-Fi (dBm / persentase), channel frekuensi (2.4 GHz vs 5 GHz), dan SSID yang terhubung di laptop.",
                SystemImpact = "Membaca antarmuka WLAN via 'netsh wlan show interfaces' secara read-only.",
                UsageGuide = "Gunakan saat laptop user sering putus nyambung Wi-Fi untuk memastikan apakah sinyal lemah atau interferensi channel."
            },
            ["net_subnet_ip_scanner"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencari seluruh alamat IP komputer, printer, IP Camera, atau perangkat IoT yang sedang aktif di subnet LAN lokal.",
                SystemImpact = "Melakukan ping sweep asinkron cepat ke rentang 1-254 pada subnet aktif dan membaca tabel ARP lokal.",
                UsageGuide = "Jalankan saat mencari IP printer baru yang belum diketahui atau mendeteksi perangkat asing di jaringan kantor."
            },
            ["disconnect_shares"] = new ToolDetailInfo
            {
                ProblemSolved = "Mapped Network Drive (Z:, Y:, dll.) error tanda silang merah, tidak bisa diputus, atau menolak ganti akun kredensial.",
                SystemImpact = "Mengeksekusi 'net use * /delete /yes' untuk memutus seluruh koneksi share dan mapped drive yang tersangkut.",
                UsageGuide = "Gunakan sebelum login ulang ke server file sharing dengan username/password yang baru."
            },
            ["sync_time_ntp"] = new ToolDetailInfo
            {
                ProblemSolved = "Jam Windows tidak akurat/bergeser sehingga browser menampilkan error SSL/HTTPS Certificate Expired atau gagal login Domain Active Directory.",
                SystemImpact = "Mengonfigurasi service w32time ke server time.windows.com / pool.ntp.org dan memaksa resinkronisasi via w32tm /resync.",
                UsageGuide = "Jalankan 1-klik untuk menormalkan jam komputer seketika."
            },
            ["remote_credential_manager"] = new ToolDetailInfo
            {
                ProblemSolved = "Password network share, remote desktop, atau akun Windows lama tersangkut sehingga muncul error Access Denied.",
                SystemImpact = "Membuka Control Panel Credential Manager klasik (control keymgr.dll) langsung.",
                UsageGuide = "Cari entri kredensial server target di bawah 'Windows Credentials' lalu edit atau hapus."
            },
            ["sec_restore_sharing_normal"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengembalikan semua setelan sharing file dan printer ke kondisi normal setelah komputer selesai dikarantina atau diservis.",
                SystemImpact = "Mengaktifkan kembali profil Windows Firewall, membuka port sharing 445/139/print, dan me-restart service LanmanServer.",
                UsageGuide = "Gunakan sebagai langkah pemulihan (rollback) setelah pengujian isolasi jaringan selesai."
            },
            ["net_ad_domain_assistant"] = new ToolDetailInfo
            {
                ProblemSolved = "Proses bergabung ke Domain Active Directory kantor yang rumit, ganti nama komputer, atau gagal connect ke Domain Controller.",
                SystemImpact = "Mengeksekusi PowerShell Add-Computer / Rename-Computer / Remove-Computer dengan manajemen kredensial admin dan uji port AD (53, 88, 389, 445).",
                UsageGuide = "1. Pastikan DNS komputer mengarah ke IP Domain Controller.\n2. Pilih menu [5] Uji Koneksi DC untuk memastikan port terbuka.\n3. Pilih menu [3] Gabung ke Domain dan masukkan kredensial Admin Domain."
            },
            ["net_wol_ip_switcher"] = new ToolDetailInfo
            {
                ProblemSolved = "Menyalakan komputer/server yang mati dari jarak jauh via LAN tanpa menyentuh tombol power fisik, serta beralih cepat antara konfigurasi DHCP Otomatis dan IP Statis khusus.",
                SystemImpact = "Menyiarkan Magic Packet UDP broadcast (port 7 & 9) ke MAC Address tujuan dan mengatur IP stack via netsh.",
                UsageGuide = "1. Klik tombol 'Wake-on-LAN & Profil IP'.\n2. Pilih opsi 1 dan masukkan MAC address target untuk menyalakan PC dari jarak jauh.\n3. Pilih opsi 2 atau 3 untuk mengatur IP statis / DHCP instan."
            },
            ["net_wifi_profile_exporter"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencadangkan seluruh profil Wi-Fi beserta passwordnya (format XML) dan mengimpornya kembali secara massal ke laptop-laptop baru tanpa perlu mengetik ulang sandi satu per satu.",
                SystemImpact = "Mengeksekusi 'netsh wlan export profile' (key=clear) dan 'netsh wlan add profile' pada Windows WLAN Service.",
                UsageGuide = "1. Klik tombol 'Ekspor / Impor Profil Wi-Fi'.\n2. Pilih Opsi 1 untuk mengekspor semua Wi-Fi ke folder Desktop.\n3. Bawa folder tersebut ke laptop baru dan jalankan Opsi 2 untuk memasang otomatis."
            },

            // === SYSTEM MAINTENANCE & REPAIR ===
            ["sys_clean_temp"] = new ToolDetailInfo
            {
                ProblemSolved = "Drive C: merah / penuh akibat penumpukan file temporary, cache Windows Update, dan file sampah aplikasi.",
                SystemImpact = "Menghapus file sampah di %TEMP%, C:\\Windows\\Temp, dan C:\\Windows\\SoftwareDistribution\\Download secara aman.",
                UsageGuide = "Jalankan secara berkala untuk melegakan kapasitas penyimpanan sistem Windows."
            },
            ["sys_smart_ram_optimizer"] = new ToolDetailInfo
            {
                ProblemSolved = "Penggunaan RAM tinggi/bengkak (>80%), PC terasa lambat atau lagging, banyak memori cache idle tertahan, serta background updaters/telemetry yang berjalan diam-diam.",
                SystemImpact = "Memanggil Windows API EmptyWorkingSet untuk melepaskan cache RAM idle dari proses aktif tanpa menutup aplikasi pengguna, serta menghentikan background updater/telemetry non-esensial dengan proteksi kernel Windows.",
                UsageGuide = "1. Klik tombol 'Optimalkan RAM & Proses'.\n2. Tool akan memindai status RAM, mendeteksi proses terberat, dan memangkas memori idle secara instan.\n3. Pantau pembebasan RAM pada Terminal Log utama."
            },
            ["sys_reset_windows_update"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows Update macet di persentase 0% / 100%, memunculkan error download berulang, atau service wuauserv memakan CPU tinggi.",
                SystemImpact = "Menghentikan wuauserv, cryptSvc, bits, msiserver; me-rename folder SoftwareDistribution dan catroot2; lalu me-restart service update.",
                UsageGuide = "Jalankan saat update Windows gagal terus-menerus. Setelah reset selesai, coba periksa update kembali."
            },
            ["sys_file_repair"] = new ToolDetailInfo
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
            ["sys_safe_mode_bcd_repair"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatur komputer masuk ke Safe Mode (Minimal / Network) untuk pembersihan malware, atau memperbaiki konfigurasi booting BCD yang rusak.",
                SystemImpact = "Menggunakan bcdedit untuk mengatur boot safeboot atau memulihkan startup normal.",
                UsageGuide = "Pilih opsi Safe Mode untuk masuk ke mode aman pada restart berikutnya, atau opsi Normal untuk kembali ke booting reguler."
            },
            ["sys_admin_tools_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Akses cepat 1-klik ke utilitas administrasi Windows tersembunyi: Computer Management, Event Viewer, Task Scheduler, Disk Management, Registry Editor, dan Group Policy.",
                SystemImpact = "Meluncurkan konsol MMC resmi Windows tanpa mengubah konfigurasi.",
                UsageGuide = "Pilih nomor konsol administrasi yang ingin Anda buka saat menu muncul."
            },
            ["sys_battery_report"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengetahui tingkat keausan baterai laptop (Design Capacity vs Full Charge Capacity) dan estimasi daya tahan baterai riil.",
                SystemImpact = "Mengeksekusi 'powercfg /batteryreport' resmi Windows dan otomatis membuka laporan HTML di browser default.",
                UsageGuide = "Jalankan saat user mengeluh baterai laptop cepat habis atau sebelum membeli laptop bekas."
            },
            ["sys_startup_manager"] = new ToolDetailInfo
            {
                ProblemSolved = "Komputer lambat saat baru dinyalakan karena terlalu banyak aplikasi pihak ketiga yang otomatis berjalan di latar belakang.",
                SystemImpact = "Membaca entri autostart di Registry Run & RunOnce serta folder Startup, menyediakan opsi hapus autostart.",
                UsageGuide = "Matikan autostart aplikasi yang tidak esensial untuk mempercepat booting komputer hingga 50%."
            },
            ["sys_restart_explorer"] = new ToolDetailInfo
            {
                ProblemSolved = "Taskbar Windows hilang, jam freeze, file explorer macet tidak merespons, atau ikon desktop tidak bisa diklik.",
                SystemImpact = "Menghentikan proses explorer.exe secara aman dan me-restart shell baru dalam hitungan detik.",
                UsageGuide = "Solusi instan mengatasi taskbar freeze tanpa perlu restart komputer."
            },
            ["sys_scan_missing_drivers"] = new ToolDetailInfo
            {
                ProblemSolved = "Mendeteksi hardware komputer yang belum memiliki driver atau memiliki tanda seru kuning di Device Manager.",
                SystemImpact = "Mengeksekusi PowerShell Get-PnpDevice untuk menyaring perangkat dengan status 'Error' atau 'Degraded'. Read-only.",
                UsageGuide = "Jalankan setelah install ulang Windows untuk mengetahui driver apa saja yang masih kurang."
            },
            ["sys_backup_drivers"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencadangkan seluruh driver OEM pihak ketiga (Wi-Fi, VGA, Audio, Chipset) sebelum komputer diinstal ulang.",
                SystemImpact = "Mengeksekusi DISM /Online /Export-Driver ke folder cadangan di Drive D: atau flashdisk.",
                UsageGuide = "Sangat berguna untuk laptop merek lokal yang driver resminya sulit diunduh di internet."
            },
            ["sys_restore_drivers"] = new ToolDetailInfo
            {
                ProblemSolved = "Memasang kembali seluruh driver hasil backup secara otomatis tanpa harus menginstal satu per satu.",
                SystemImpact = "Mengeksekusi 'pnputil.exe /add-driver *.inf /subdirs /install' dari folder backup driver.",
                UsageGuide = "Arahkan ke folder hasil backup driver dan biarkan Windows memasang semua driver secara otomatis."
            },
            ["sys_winget_install_essentials"] = new ToolDetailInfo
            {
                ProblemSolved = "Menginstal paket software standar kantor baru (Chrome, 7-Zip, Adobe Reader, VLC, AnyDesk) secara otomatis tanpa iklan.",
                SystemImpact = "Memanggil Microsoft Winget Package Manager resmi untuk mengunduh dan memasang aplikasi secara silent.",
                UsageGuide = "Pastikan PC terhubung ke internet dan pilih paket aplikasi yang ingin dipasang otomatis."
            },
            ["sys_office_debloater"] = new ToolDetailInfo
            {
                ProblemSolved = "Mematikan telemetri Windows, Bing Search di Start Menu, cortana, dan aplikasi bloatware yang membebani kinerja PC kantor.",
                SystemImpact = "Mengatur registry Group Policy untuk mematikan DiagTrack, feedback frequency, dan web search di taskbar.",
                UsageGuide = "Jalankan pada PC kantor yang lambat untuk meningkatkan responsivitas sistem."
            },
            ["sys_disable_hibernation"] = new ToolDetailInfo
            {
                ProblemSolved = "Membebaskan kapasitas Drive C: sebesar ukuran RAM (4 GB – 32 GB) dengan menghapus berkas hiberfil.sys.",
                SystemImpact = "Mengeksekusi 'powercfg -h off' resmi Windows. Menonaktifkan fitur Fast Startup & Hibernasi.",
                UsageGuide = "Gunakan pada PC kantor dengan kapasitas SSD kecil (120 GB / 240 GB) yang Drive C: nya hampir penuh."
            },
            ["sys_classic_context_menu"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengembalikan menu klik kanan klasik Windows 10 di Windows 11 tanpa tombol 'Show more options' (Shift+F10).",
                SystemImpact = "Menambahkan CLSID '{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}' pada HKCU Software\\Classes dan me-restart Explorer.",
                UsageGuide = "Jalankan 1-klik untuk mengembalikan menu klik kanan lengkap yang disukai para teknisi."
            },
            ["sys_enable_dotnet35_dism"] = new ToolDetailInfo
            {
                ProblemSolved = "Aplikasi kantor lama (e-Faktur, Software Akuntansi, Delphi, VB6) meminta instalasi .NET Framework 3.5 / 2.0.",
                SystemImpact = "Mengunduh dan mengaktifkan fitur NetFx3 melalui DISM Online resmi Microsoft.",
                UsageGuide = "Pastikan terhubung ke internet saat proses instalasi .NET Framework berlangsung."
            },
            ["sys_compact_os"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengompresi file sistem Windows menggunakan algoritma LZX bawaan Microsoft untuk menghemat 3 – 8 GB ruang Drive C:.",
                SystemImpact = "Mengeksekusi 'compact.exe /CompactOS:always'. File sistem dikompresi tanpa memengaruhi kecepatan baca PC modern.",
                UsageGuide = "Jalankan pada komputer dengan drive penyimpanan C: yang sangat sempit."
            },
            ["sys_remove_uwp_bloatware"] = new ToolDetailInfo
            {
                ProblemSolved = "Menghapus aplikasi bawaan Windows yang tidak diperlukan di kantor (Xbox, Clipchamp, Disney+, Game Bar, Solitaire).",
                SystemImpact = "Mengeksekusi Remove-AppxPackage pada profil user aktif. Tidak merusak Windows Store dasar.",
                UsageGuide = "Gunakan untuk membersihkan laptop kantor baru agar lebih rapi dan bebas gangguan."
            },
            ["sys_disable_edge_background"] = new ToolDetailInfo
            {
                ProblemSolved = "Microsoft Edge terus berjalan diam-diam di background dan memakan 300–800 MB RAM meski browser sudah ditutup.",
                SystemImpact = "Mengatur Group Policy registry Edge: StartupBoostEnabled=0 dan BackgroundModeEnabled=0.",
                UsageGuide = "Sangat direkomendasikan untuk komputer kantor dengan kapasitas RAM terbatas (4 GB – 8 GB)."
            },
            ["sys_optimize_visual_effects"] = new ToolDetailInfo
            {
                ProblemSolved = "Menonaktifkan animasi jendela, bayangan kursor, dan efek transisi Windows yang membuat PC kantor tua terasa lambat.",
                SystemImpact = "Mengubah registry VisualFXSetting ke mode 'Adjust for best performance' namun tetap menjaga kerapian font layar.",
                UsageGuide = "Jalankan untuk mempercepat respons antarmuka Windows secara instan."
            },
            ["sys_toggle_win_update_lock"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows Update sering berjalan sendiri di latar belakang, membuat internet lemot, atau merusak driver printer/audio.",
                SystemImpact = "Menghentikan dan mendisable wuauserv, UsoSvc, WaaSMedicSvc, serta mengatur GPO NoAutoUpdate=1 di registry.",
                UsageGuide = "Klik 1 kali untuk mengunci update permanen, dan klik sekali lagi di kemudian hari jika ingin membuka kuncinya kembali."
            },
            ["sys_create_restore_point"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuat titik pemulihan sistem (System Restore Point) instan sebelum melakukan modifikasi besar atau instalasi driver baru.",
                SystemImpact = "Memanggil WMI SystemRestore CreateRestorePoint. Jika proteksi sistem mati, tool akan mengaktifkannya otomatis.",
                UsageGuide = "Jalankan selalu sebelum melakukan uji coba konfigurasi berisiko tinggi."
            },
            ["sys_dism_wim_backup"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuat cadangan seluruh sistem Windows (Drive C:) ke dalam file image .WIM terkompresi menggunakan mesin resmi DISM.",
                SystemImpact = "Mengeksekusi 'DISM /Capture-Image' ke file WIM di drive non-sistem (D: atau flashdisk eksternal).",
                UsageGuide = "Gunakan untuk membuat master backup image Windows sebelum PC diserahkan ke pengguna."
            },
            ["sys_live_rescue_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Panduan terorganisir perbaikan komputer mati total, bluescreen booting, reset password Windows SAM offline menggunakan WinPE (Hiren's, DLC Boot, SystemRescue, UBCD).",
                SystemImpact = "Menyediakan panduan taktis lapangan, download link resmi, dan prosedur perbaikan bare-metal.",
                UsageGuide = "Pilih toolkit WinPE yang ingin dipelajari dan ikuti instruksi langkah perbaikan."
            },
            ["sys_disk_cloning_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Panduan kloning massal puluhan PC kantor baru, backup full image ke NAS (Clonezilla/Rescuezilla), dan deployment jaringan PXE (FOG Project).",
                SystemImpact = "Menyediakan panduan langkah demi langkah skenario Device-to-Device dan Device-to-Image.",
                UsageGuide = "Gunakan saat mempersiapkan pergantian SSD massal atau backup berkala server."
            },
            ["sys_ventoy_multiboot_guide"] = new ToolDetailInfo
            {
                ProblemSolved = "Cara membuat 1 flashdisk multi-boot yang bisa memuat puluhan file ISO Windows, Linux, dan WinPE sekaligus tanpa format ulang.",
                SystemImpact = "Menyediakan panduan instalasi Ventoy, konfigurasi folder, dan tips kompatibilitas UEFI / Legacy BIOS.",
                UsageGuide = "Cukup pasang Ventoy ke flashdisk sekali, lalu copy file ISO langsung ke dalam flashdisk."
            },
            ["sys_deep_hardware_inspector"] = new ToolDetailInfo
            {
                ProblemSolved = "Membaca 100% informasi hardware fisik komputer secara mendalam (Tipe SSD/NVMe, S.M.A.R.T Health, Slot & Speed RAM MHz, TPM 2.0, BIOS, Baterai) tanpa software pihak ketiga.",
                SystemImpact = "Mengeksekusi query WMI/CIM mendalam dan PowerShell hardware collector. Murni read-only (100% aman).",
                UsageGuide = "Jalankan saat melakukan audit aset hardware, pengecekan upgrade RAM/SSD, atau diagnosa kesehatan PC."
            },
            ["sys_office_rescue_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatasi Word, Excel, PowerPoint, atau Outlook yang crash, freeze di splash screen, error tampilan/startup Outlook ('Cannot open window'), template default Normal.dotm korup, atau dokumen macet 'Upload Failed' / 'File Locked'.",
                SystemImpact = "Menyediakan opsi Safe Mode (/safe), pemulihan Navigation Pane Outlook (/resetnavpane), peluncur otomatis scanpst.exe, regenerasi template Word/Excel, pembersihan OfficeFileCache, dan pemicu Click-to-Run Quick Repair resmi.",
                UsageGuide = "1. Klik tombol 'Buka Rescue Hub Office & Outlook'.\n2. Pilih nomor opsi sesuai masalah yang dialami (1-6).\n3. Ikuti panduan di log terminal atau jendela Microsoft yang terbuka."
            },
            ["sys_user_data_backup_restore"] = new ToolDetailInfo
            {
                ProblemSolved = "Menyelamatkan data profil esensial user (Desktop, Documents, Downloads, Pictures, Bookmarks Chrome/Edge, Signatures Outlook, dan Sticky Notes) sebelum komputer diservis atau diinstal ulang.",
                SystemImpact = "Mengeksekusi penyalinan berkas multi-threaded (robocopy) ke direktori tujuan (Drive D:, E:, atau flashdisk eksternal) dan menyediakan mode pemulihan data.",
                UsageGuide = "1. Klik tombol 'Penyelamatan Data User'.\n2. Pilih Opsi 1 untuk mencadangkan profil ke drive D:/flashdisk.\n3. Gunakan Opsi 2 untuk memulihkan kembali data setelah Windows selesai diinstal."
            },
            ["sys_service_report_generator"] = new ToolDetailInfo
            {
                ProblemSolved = "Membuat Berita Acara & Laporan Servis Teknis PC resmi berformat HTML profesional siap cetak ke PDF untuk lampiran sistem tiket IT (Jira/GLPI) atau serah terima unit ke pengguna.",
                SystemImpact = "Membaca spesifikasi hardware, serial number motherboard, kapasitas & S.M.A.R.T disk, merangkum tindakan perbaikan, dan menghasilkan laporan HTML rapi lengkap dengan blok tanda tangan.",
                UsageGuide = "1. Klik tombol 'Cetak Laporan Servis IT'.\n2. Masukkan nama teknisi, nama pengguna, dan ringkasan tindakan perbaikan.\n3. Dokumen HTML akan terbuka otomatis di browser dan siap dicetak/disimpan ke PDF (Ctrl+P)."
            },
            ["sys_oneclick_tuneup_suite"] = new ToolDetailInfo
            {
                ProblemSolved = "Melakukan rangkaian pemeliharaan rutin PC kantor secara cepat tanpa harus mengklik tombol satu per satu: membersihkan sampah temporary, memangkas RAM idle, me-refresh cache DNS/NetBIOS, menormalkan antrean print spooler, dan mengoptimalkan responsivitas visual.",
                SystemImpact = "Mengeksekusi 5 tahap optimalisasi berantai secara otomatis dalam hitungan detik tanpa menutup software aktif pengguna.",
                UsageGuide = "1. Klik tombol 'Jalankan 1-Klik Tune-Up Rutin'.\n2. Konfirmasi 'Yes' saat prompt pemeliharaan muncul.\n3. Pantau progres eksekusi di terminal hingga muncul pesan selesai."
            },

            // === REMOTE DESKTOP & SUPPORT ===
            ["remote_rdp_manager_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows Remote Desktop (RDP) tidak bisa diakses, port 3389 terblokir firewall, error NLA (Network Level Authentication), atau teknisi ingin membimbing layar user tanpa me-logout sesi aktif.",
                SystemImpact = "Mengatur fDenyTSConnections pada Terminal Server registry, mengonfigurasi Service TermService ke Automatic, mengatur aturan Windows Firewall port 3389/custom, dan mengaktifkan RDP Shadowing policy.",
                UsageGuide = "1. Klik tombol 'Pusat Utilitas Windows RDP'.\n2. Pilih opsi 1 untuk mengaktifkan RDP Host penuh dan buka firewall secara instan.\n3. Pilih opsi 5 untuk Quick Connect RDP Client ke PC target.\n4. Pilih opsi 6 untuk Remote Shadowing layar pengguna tanpa logout."
            },
            ["remote_reset_anydesk_id"] = new ToolDetailInfo
            {
                ProblemSolved = "AnyDesk ID bentrok antara 2 komputer hasil kloning OS, atau terkena limit lisensi AnyDesk.",
                SystemImpact = "Menghentikan proses AnyDesk, menghapus service.conf dan system.conf di %ProgramData% dan %AppData%, lalu me-restart service AnyDesk.",
                UsageGuide = "Jalankan tool ini, lalu buka kembali AnyDesk untuk mendapatkan nomor ID 9-digit baru yang unik."
            },
            ["remote_change_tv_pwd"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatur password permanen TeamViewer untuk remote support tanpa pengawasan (Unattended Access) langsung lewat CLI.",
                SystemImpact = "Menyetel konfigurasi password TeamViewer melalui argumen command-line resmi TeamViewer.",
                UsageGuide = "Masukkan password baru yang diinginkan pada prompt yang muncul."
            },
            ["remote_rustdesk_manager_hub"] = new ToolDetailInfo
            {
                ProblemSolved = "Solusi remote desktop mandiri open-source tanpa limit komersial, konfigurasi relay server kantor sendiri, dan instalasi instan.",
                SystemImpact = "Mendukung auto-deteksi instalasi, reset config/ID jika terjadi duplikasi hasil kloning OS, pasang via Winget, dan konfigurasi server relay.",
                UsageGuide = "Pilih menu [1] Buka, [2] Reset ID, [3] Set Server Relay Kantor, atau [4] Install otomatis via Winget."
            },

            // === SECURITY & FIREWALL ===
            ["sec_user_password_recovery"] = new ToolDetailInfo
            {
                ProblemSolved = "User lupa password login Windows lokal, akun terkunci (Locked Out), atau ingin melihat password profil Wi-Fi yang pernah tersimpan di laptop.",
                SystemImpact = "Menjalankan perintah 'net user' administratif untuk reset password / unlock akun, dan 'netsh wlan show profile key=clear'.",
                UsageGuide = "Pilih opsi 1 untuk melihat daftar user, opsi 2 untuk me-reset password akun lokal, atau opsi 4 untuk melihat password Wi-Fi."
            },
            ["sec_bitlocker_audit"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencari dan mencatat kunci pemulihan 48-Digit BitLocker sebelum motherboard/SSD diganti atau sebelum update BIOS.",
                SystemImpact = "Mengeksekusi 'manage-bde -protectors -get C:' untuk membaca Numerical Password protector. Hanya membaca, tidak mengubah enkripsi.",
                UsageGuide = "Jalankan dan catat 48-digit angka yang muncul. Simpan di tempat aman sebelum melakukan perbaikan hardware."
            },
            ["sec_hardening_antiransomware"] = new ToolDetailInfo
            {
                ProblemSolved = "Mencegah eksploitasi celah keamanan SMBv1 (WannaCry), infeksi virus via Flashdisk USB AutoPlay, dan akses Remote Registry liar.",
                SystemImpact = "Menonaktifkan SMBv1 protocol, mematikan USB AutoRun/AutoPlay via registry, dan mendisable service RemoteRegistry.",
                UsageGuide = "Sangat direkomendasikan dijalankan pada seluruh PC kantor baru sebagai standar hardening dasar."
            },
            ["sec_toggle_firewall_ports"] = new ToolDetailInfo
            {
                ProblemSolved = "Menutup port rentan (SMB 445/139, RDP 3389, Telnet 23, FTP 21) di Windows Firewall saat terdeteksi ancaman siber.",
                SystemImpact = "Menambahkan atau menghapus rule inbound block pada Windows Firewall untuk port-port kritis.",
                UsageGuide = "Pilih nomor port yang ingin diisolasi untuk memutus celah serangan secara instan."
            },
            ["sec_port_scanner"] = new ToolDetailInfo
            {
                ProblemSolved = "Memeriksa port jaringan yang terbuka pada server lokal atau komputer target (Web 80/443, Database 1433/3306, SSH 22).",
                SystemImpact = "Melakukan koneksi soket TCP non-destruktif ke host dan rentang port yang ditentukan.",
                UsageGuide = "Masukkan IP target dan port atau gunakan default untuk memindai port umum."
            },
            ["sec_reset_firewall"] = new ToolDetailInfo
            {
                ProblemSolved = "Windows Firewall berantakan, terlalu banyak rule usang yang membingungkan, port bocor, atau service firewall error.",
                SystemImpact = "Mereset total konfigurasi Firewall ke setelan pabrik default ('netsh advfirewall reset') dan me-restart service mpssvc.",
                UsageGuide = "Gunakan saat ingin memulai konfigurasi firewall dari nol dalam kondisi bersih dan aman."
            },
            ["sec_toggle_icmp_ping"] = new ToolDetailInfo
            {
                ProblemSolved = "Komputer tidak bisa di-ping dari komputer lain di jaringan LAN, atau sebaliknya ingin menyembunyikan PC dari scanning ping.",
                SystemImpact = "Mengaktifkan atau menonaktifkan rule firewall 'File and Printer Sharing (Echo Request - ICMPv4-In)'.",
                UsageGuide = "Pilih opsi 1 untuk mengizinkan respon ping, atau opsi 2 untuk memblokir respon ping."
            },
            ["sec_emergency_quarantine"] = new ToolDetailInfo
            {
                ProblemSolved = "Komputer terinfeksi Ransomware / Malware agresif yang sedang menyebar ke PC lain di jaringan kantor (Lateral Movement).",
                SystemImpact = "Mengubah kebijakan Windows Firewall menjadi 'blockinbound,blockoutbound' (Air-Gap Isolation). Memutus 100% lalu lintas LAN & Internet tanpa mematikan driver kartu jaringan (tool internal tetap bisa berjalan).",
                UsageGuide = "1. Mode Darurat: Pilih Opsi 1 untuk mengisolasi PC seketika.\n2. Bersihkan malware.\n3. Mode Normalisasi: Pilih Opsi 2 untuk membuka kembali koneksi setelah PC bersih."
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

            // === LICENSE AUDIT & MANAGEMENT ===
            ["license_audit_status"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengecek apakah Windows berlisensi Asli (Retail / OEM) atau bajakan/KMS yang akan kedaluwarsa dalam 180 hari.",
                SystemImpact = "Mengeksekusi 'slmgr /dli' dan 'slmgr /xpr' untuk membaca status aktivasi resmi Microsoft.",
                UsageGuide = "Jalankan untuk audit kepatuhan lisensi perangkat lunak di perusahaan."
            },
            ["license_extract_oem_bios_key"] = new ToolDetailInfo
            {
                ProblemSolved = "Membaca kunci lisensi asli Windows yang tertanam di chip motherboard (MSDM Table BIOS) laptop/PC branded (Dell, HP, Lenovo, Asus).",
                SystemImpact = "Membaca ACPI MSDM table via WMI/PowerShell. 100% aman dan read-only.",
                UsageGuide = "Gunakan saat ingin menginstal ulang komputer agar Windows langsung teraktivasi otomatis dengan lisensi pabrik."
            },
            ["license_clean_kms_tokens"] = new ToolDetailInfo
            {
                ProblemSolved = "Menghapus residu server KMS bajakan, mereset token aktivasi (tokens.dat) yang rusak, dan mengatasi error aktivasi 0xC004C003.",
                SystemImpact = "Membersihkan KMS IP di registry, mengembalikan KMS port ke default, dan merefresh file token lisensi Windows.",
                UsageGuide = "Jalankan sebelum memasukkan lisensi Windows Retail/OEM yang baru."
            },
            ["license_diagnose_office"] = new ToolDetailInfo
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
            ["lic_office_conflict_cleaner"] = new ToolDetailInfo
            {
                ProblemSolved = "Mengatasi banner kuning 'Unlicensed Product' atau 'Activation Failed' akibat adanya sisa lisensi trial/grace lama yang bentrok di ospp.vbs, serta mengatasi loop login akun kantor/sekolah yang macet.",
                SystemImpact = "Memindai lisensi via OSPP.VBS, menghapus key spesifik dengan /unpkey:XXXXX, dan mereset token Modern Authentication (OneAuth, IdentityCache, dan kredensial Windows).",
                UsageGuide = "1. Klik tombol 'Pembersih Lisensi & Akun Office'.\n2. Pilih opsi 1 untuk memindai key sisa.\n3. Pilih opsi 2 dan masukkan 5 karakter terakhir key yang ingin dihapus, atau pilih opsi 3/4 untuk mereset cache login."
            },
            ["license_switch_windows_edition"] = new ToolDetailInfo
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
