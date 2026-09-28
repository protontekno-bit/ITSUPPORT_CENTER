<div align="center">

<img src="assets/app_logo.png" alt="AuraCore IT Support Center Logo" width="160" height="160" />

# ⚡ IT Support & Security Center 2026
### **Enterprise Modular Diagnostics, Network Engineering & System Repair Suite**
*All-in-One Field Toolkit untuk Senior IT Administrator, Teknisi Lapangan, dan Helpdesk Enterprise.*

[![Windows](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20%7C%20Server-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://microsoft.com)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Version](https://img.shields.io/badge/Version-v3.2.0%20Enterprise-27AE60?style=for-the-badge)](https://www.auracore.my.id)
[![Architecture](https://img.shields.io/badge/Architecture-x64%20Single--File-E67E22?style=for-the-badge)](https://www.auracore.my.id)
[![Developer](https://img.shields.io/badge/Developer-AuraCore-3498DB?style=for-the-badge&logo=googlechrome&logoColor=white)](https://www.auracore.my.id)

[🌐 Kunjungi Website Resmi Pengembang (AuraCore)](https://www.auracore.my.id) • [📖 Panduan Pengembang (AI_DEV_GUIDE)](AI_DEV_GUIDE.md) • [⚖️ Disclaimer Hukum](DISCLAIMER.md)

---

</div>

## 📌 Mengapa IT Support Center?

Dalam operasional harian IT Support dan Network Administrator, teknisi sering menghabiskan **berjam-jam mengetik puluhan perintah cmd/powershell yang berulang**, mencari utilitas terpisah di flashdisk, atau bergulat dengan printer macet, file sharing SMB error, IP conflict di kantor, Windows Update stuck, hingga repositori WMI yang rusak.

**IT Support Center** menyatukan **86 modul alat pemeliharaan dan diagnosa tingkat lanjut** ke dalam **satu aplikasi mandiri portabel (Single-File Executable)** yang dapat dijalankan langsung tanpa instalasi (*zero-install*), bebas bloatware, dan aman untuk standar korporasi.

---

## ✨ Fitur Unggulan (Core Advantages)

- 🖥️ **Dual Interface (GUI Modern + CLI Universal):**
  - **Modern WinForms GUI:** Tampilan *Dark Theme* elegan, kartu modular dinamis, pencarian instan, dan log konsol *real-time*.
  - **Zero-Dependency CLI Terminal (`IT_SUPPORT_CENTER.bat`):** 86 menu berbasis Windows Batch murni yang dapat berjalan di Windows PE, Safe Mode, server *headless*, maupun sesi remote command prompt.
- ⚡ **Auto-Admin Elevation & Staging Network (UNC / TrueNAS):**
  - Dilengkapi *smart bootstrapper* [`START.bat`](START.bat) yang otomatis meminta hak UAC Administrator dan dapat dijalankan langsung dari Network Share (`\\192.168.x.x\share`) tanpa risiko *UNC lock*.
- 🛠️ **Inspirasi Toolkit Kelas Dunia:**
  - Mengintegrasikan modul canggih terinspirasi dari **Sysinternals Autoruns** (deteksi malware startup), **Tweaking.com** (perbaikan total WMI & permission ACL), **Chris Titus Tech WinUtil** (skema daya *Ultimate Performance* & optimasi latensi), dan **GeekUninstaller** (force uninstall program macet).
- 🌐 **Deep Network Diagnostics (Layer 2 s/d Layer 7):**
  - Dilengkapi *TCP Port Ping (TCPing)* pengganti Telnet, *ARP Layer-2 Sweeper* anti-blokir firewall ICMP, deteksi duplikasi IP/MAC (IP Conflict), dan *NIC Hardware Power-Cycle*.
- 📜 **1-Klik Berita Acara & Laporan Servis (HTML Report):**
  - Otomatis mencatat riwayat perbaikan yang dilakukan dan mengekspornya ke format HTML rapi yang siap dicetak (*Print to PDF*) atau ditandatangani oleh klien.

---

## 🗂️ Katalog Lengkap 86 Modul Berdasarkan Kategori

Aplikasi mengelompokkan 86 fitur ke dalam 9 kategori strategis:

### 1. 📋 Diagnosa Sistem & Aset Perangkat
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **1** | **Audit Aset PC & Spesifikasi** | Mengaudit Serial Number BIOS, tipe CPU, kapasitas RAM, tipe disk (SSD/NVMe), IP & MAC Address dalam 1 kali klik. |
| **2** | **Laporan Baterai Laptop (HTML)** | Memicu `powercfg /batteryreport` untuk menganalisis sisa kapasitas baterai (mWh) dan riwayat degradasi. |
| **3** | **Pengelola Startup Windows** | Membuka manajer startup untuk menonaktifkan aplikasi latar belakang yang membebani booting. |
| **27**| **Audit Status Enkripsi BitLocker** | Memeriksa volume C/D apakah terenkripsi BitLocker, status proteksi, dan metode enkripsi (XTS-AES). |
| **28**| **Audit Kesehatan Fisik Harddisk/SSD** | Mengaudit status SMART storage drive dan memicu disk scan non-invasif. |
| **63**| **Inspeksi Hardware Mendalam** | Membaca sensor suhu CPU, clock speed, model motherboard, dan status thermal throttling. |

### 2. 🖨️ Rekayasa Printer & Print Spooler
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **5** | **Fix Printer Sharing (0x0000011b)** | Memperbaiki error RPC sharing printer jaringan pasca-update Windows via registry hardening bypass. |
| **6** | **Bersihkan Antrean Cetak Macet** | Mematikan spooler, menghapus file `.SHD` & `.SPL` di `spool\PRINTERS`, lalu merestart spooler otomatis. |
| **45**| **Audit Printer & Test Print Cepat** | Menampilkan daftar seluruh driver printer terpasang dan mengirim test print mandiri. |
| **46**| **Factory Reset Print Spooler** | Menghapus port printer korup, membersihkan spooler registry, dan mengembalikan service ke standar pabrik. |
| **47**| **Buka Devices & Printers Klasik** | Membuka kontrol panel printer klasik Windows 7/10 di Windows 11. |

### 3. 🌐 Jaringan, SMB & Pemecahan Masalah Koneksi
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **7** | **Fix SMB Guest Access (0x800704f8)** | Mengaktifkan `AllowInsecureGuestAuth` agar PC dapat mengakses NAS/Samba lawas tanpa login. |
| **8** | **Diagnosa Server SMB & Port 445** | Menguji Ping, port 445 file sharing, dan mengeksekusi `net view` ke IP server target. |
| **9** | **Reset Total TCP/IP Stack & Winsock** | Mengatasi status *"No Internet, Secured"* dengan mereset Winsock, interface IP, dan WinHTTP proxy. |
| **10**| **Bersihkan Cache Jaringan** | Mengosongkan DNS (`/flushdns`), cache NetBIOS (`nbtstat -R`), dan sesi SMB yang menggantung. |
| **11**| **Putus Semua Network Drive** | Memutuskan seluruh mapped drive yang macet (`net use * /delete /y`) dan menghapus tiket Kerberos. |
| **12**| **Sinkronisasi Waktu NTP / Domain** | Menyelaraskan jam komputer ke server NTP nasional / Active Directory (`w32tm /resync`). |
| **23**| **Pemindai Port Jaringan (Nmap Suite)** | Mengunduh, memasang, atau menjalankan Nmap port scanner untuk audit keamanan jaringan kantor. |
| **29**| **Pengganti Cepat DNS Resolver** | Beralih 1-klik antara DNS Google (8.8.8.8), Cloudflare (1.1.1.1), OpenDNS, atau DHCP Otomatis. |
| **32**| **Pemindai Subnet IP LAN (Ping Sweep)** | Memindai 254 IP aktif pada subnet lokal secara paralel untuk mendeteksi printer/PC baru. |
| **33**| **Uji Kualitas Koneksi 3-Titik** | Menganalisis latensi dan *packet loss* simultan ke Gateway, DNS ISP, dan Internet Global (8.8.8.8). |
| **34**| **Reset Proxy & File Hosts Windows** | Mengembalikan file `hosts` ke kondisi bawaan Microsoft dan mematikan proxy malware nakal. |
| **35**| **Audit Sinyal & BSSID Wi-Fi** | Memeriksa kekuatan sinyal (%), channel radio, standar 802.11ax/ac, dan security cipher Wi-Fi. |
| **36**| **Perbaikan Total Jaringan (All-in-One)** | Rangkaian eksekusi komprehensif: Flush DNS, Reset IP/Winsock, renew DHCP, dan restart stack. |
| **37**| **Audit Alamat IP & Koneksi Aktif** | Menampilkan konfigurasi IP lengkap dan memindai port listening mencurigakan (`netstat -ano`). |
| **38**| **Buka Network Connections (ncpa.cpl)** | Akses cepat ke jendela adapter jaringan klasik Windows. |
| **78**| **Wake-on-LAN (WoL) & Pengatur IP** | Mengirimkan Magic Packet WoL untuk menghidupkan PC dari jarak jauh dan mengganti profil IP Static/DHCP. |
| **79**| **Ekspor & Impor Profil Wi-Fi** | Mencadangkan seluruh profil Wi-Fi beserta password-nya ke file XML dan merestore-nya ke PC baru. |
| **84**| **Penguji Port TCP & Latensi (TCPing)** | Pengganti Telnet modern untuk menguji status port server (Web, DB, RDP, Mail, MikroTik) dengan RTT milidetik. |
| **85**| **Pemindai ARP Layer-2 & Deteksi Konflik IP** | Pindai seluruh host LAN via ARP (anti-blokir firewall ICMP), deteksi konflik IP duplikat, dan identifikasi vendor. |
| **86**| **Siklus Restart Keras Adapter (NIC Power-Cycle)**| Me-restart chip driver fisik kartu LAN/Wi-Fi yang macet/error (Code 43) tanpa perlu merestart PC. |

### 4. 🛡️ Keamanan Sistem & Tanggap Insiden
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **15**| **Buka Pengelola Kredensial (vault.cpl)**| Mengelola kredensial Windows, token Outlook, dan password share drive yang tersimpan. |
| **20**| **Hardening Anti-Ransomware & SMB1** | Mematikan protokol purba SMBv1 dan memblokir port eksploitasi worm/ransomware berbahaya. |
| **21**| **Tutup Port Rentan (135, 137-139, 445)** | Menambahkan rule Windows Firewall untuk menutup celah port RPC dan NetBIOS dari akses luar. |
| **31**| **Manajemen Sandi Akun Pengguna** | Mereset sandi akun lokal Windows atau mengaktifkan akun Administrator bawaan tersembunyi. |
| **64**| **Reset Total Windows Defender Firewall** | Mengembalikan seluruh aturan firewall ke kondisi bawaan Microsoft (*default factory policy*). |
| **65**| **Saklar Respon ICMP Ping (Stealth Mode)**| Mengaktifkan atau mematikan kemampuan komputer untuk merespons ping dari jaringan. |
| **66**| **Karantina Jaringan Darurat (Air-Gap)** | Memutus total koneksi jaringan dalam 1 detik saat komputer terindikasi terserang ransomware. |
| **67**| **Pusat Buka/Tutup Port Firewall Kantor** | Membuka atau menutup port kustom (RDP, Web Server, DB) dengan satu klik. |
| **68**| **Saklar Status Profil Firewall** | Mengontrol status firewall per-profil (Domain, Private, Public). |
| **80**| **Inspeksi Startup & Persistence (Autoruns Lite)**| Memindai titik persembunyian malware/trojan: Run, RunOnce, Winlogon Shell/Userinit, dan Task Scheduler. |

### 5. 🏢 Active Directory & Remote Support
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **16**| **Reset ID & Kredensial AnyDesk** | Mereset `service.conf` dan `system.conf` AnyDesk untuk mendapatkan ID baru saat terjadi limit/error. |
| **17**| **Ganti Sandi Permanen TeamViewer** | Memandu penggantian password unattended access TeamViewer secara aman. |
| **69**| **Pusat Manajemen RustDesk** | Mengunduh, meluncurkan, mereset ID/service, dan mengonfigurasi alamat server RustDesk internal. |
| **70**| **Asisten Active Directory & Join Domain** | Mengaudit status domain, mengubah nama komputer, menguji konektivitas ke Domain Controller (DC), dan join domain. |
| **71**| **Pusat Manajemen & Aktivator Windows RDP** | Mengaktifkan Remote Desktop, mengatur port listening (default 3389), membuka firewall, dan mengaktifkan RDP Shadowing. |

### 6. ⚙️ Pemeliharaan & Optimalisasi Kinerja OS
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **4** | **Reset Windows Update Engine** | Menghentikan service, membersihkan folder `SoftwareDistribution` & `Catroot2`, lalu mendaftarkan ulang DLL update. |
| **13**| **Bersihkan File Sampah & Temp Sistem** | Menghapus file sementara di `%TEMP%`, `C:\Windows\Temp`, Prefetch, dan crash dumps untuk melegakan storage. |
| **14**| **Pemeriksaan Integritas Sistem (SFC & DISM)**| Menjalankan `sfc /scannow` dan `DISM /RestoreHealth` untuk memperbaiki file sistem Windows yang korup. |
| **30**| **Restart Cepat Windows Explorer** | Mematikan dan menyalakan kembali proses `explorer.exe` saat taskbar atau desktop *freeze*. |
| **43**| **Boot Ulang ke Safe Mode Windows** | Mengatur konfigurasi boot BCD untuk masuk ke Safe Mode (Minimal atau dengan Jaringan) sekali klik. |
| **44**| **Pusat Alat Administrasi Windows** | Shortcut cepat ke Computer Management, Event Viewer, Services, Task Manager, dan Registry Editor. |
| **50**| **Nonaktifkan Hibernasi (Free up hiberfil.sys)**| Mematikan hibernasi Windows (`powercfg -h off`) untuk menghemat 4–16 GB ruang disk C:. |
| **51**| **Kembalikan Menu Klik Kanan Klasik Win11** | Mengembalikan context menu klasik Windows 10 pada Windows 11 tanpa perlu klik *"Show more options"*. |
| **52**| **Aktifkan .NET Framework 3.5 & 2.0** | Memasang fitur .NET 3.5 via DISM online untuk kompatibilitas aplikasi perkantoran lama. |
| **53**| **Kompresi Ruang Sistem (CompactOS)** | Mengaktifkan kompresi algoritma XPRESS pada file OS Windows untuk menghemat 2–4 GB pada SSD kecil. |
| **54**| **Pembersih Aplikasi Bawaan (Debloat UWP)** | Menghapus bloatware bawaan Windows (game sponsor, widget berita, iklan) yang membebani memori. |
| **55**| **Matikan Proses Latar Belakang Edge** | Mencegah Microsoft Edge berjalan terus di background saat browser telah ditutup. |
| **56**| **Optimalisasi Efek Visual (Mode Performa)** | Menyesuaikan efek animasi Windows untuk performa maksimal pada PC kantor spek rendah. |
| **57**| **Kunci / Buka Pembaruan Otomatis Windows**| Mematikan atau menghidupkan pembaruan otomatis Windows Update via Group Policy Registry. |
| **77**| **1-Klik Pemeliharaan & Tune-Up Rutin** | Solusi satu klik: pembersihan temp, flush DNS, optimasi RAM, dan pemeriksaan integritas singkat. |
| **81**| **Perbaikan Total WMI & Hak Akses (ACL Rebuilder)**| Membangun ulang repositori WMI (`winmgmt /resetrepository`), re-registrasi DLL WMI, dan reset permission sistem. |
| **82**| **Aktivasi Ultimate Performance (CTT Style)**| Membuka skema daya tersembunyi *Ultimate Performance*, mematikan network throttling, dan menekan latensi data. |
| **83**| **Penghapus Paksa Program Macet (Force Nuker)**| Mematikan proses, mencari uninstaller bawaan, dan menyapu bersih sisa folder `Program Files`, `AppData`, dan Registry. |

### 7. 💾 Penyelamatan Data, Migrasi & WinPE
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **58**| **Buat Titik Pemulihan (Restore Point)** | Membuat System Restore Point baru secara instan sebelum melakukan perubahan besar pada sistem. |
| **59**| **Cadangkan Citra Sistem (DISM WIM Backup)** | Menangkap partisi Windows yang berjalan ke dalam file image `.wim` mandiri untuk deployment/cadangan. |
| **60**| **Pusat Penyelamatan Sistem (Live Rescue Hub)**| Panduan dan utilitas integrasi ISO live rescue WinPE (Hiren's BootCD PE, Sergei Strelec, Bob.Omb). |
| **61**| **Pusat Kloning & Deployment Disk** | Akses cepat dan panduan perkakas kloning storage (Clonezilla, Rescuezilla, Macrium). |
| **62**| **Panduan USB Multi-Boot (Ventoy Guide)** | Petunjuk instalasi dan penyiapan flashdisk teknisi multi-ISO berbasis Ventoy. |
| **75**| **Penyelamatan & Migrasi Data Profil Pengguna**| Mencadangkan folder Desktop, Dokumen, Download, Gambar, dan Bookmark Browser pengguna ke drive eksternal. |

### 8. 📄 Perkantoran (Microsoft Office & Outlook Rescue)
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **41**| **Diagnosa & Pemulihan Lisensi Office** | Mengaudit status lisensi Office via skrip resmi `ospp.vbs` dan mendiagnosa status aktivasi. |
| **49**| **Debloat Komponen Office yang Tak Terpakai** | Menghapus komponen berat Office yang tidak dibutuhkan (misal: OneDrive Sync, Teams Personal, Telemetry). |
| **72**| **Kunci / Buka Update Office Permanen** | Mengunci pembaruan otomatis Click-to-Run (C2R) Office agar tidak terjadi masalah kompatibilitas add-in. |
| **73**| **Pertolongan Pertama Office & Outlook** | Menjalankan Word/Excel dalam Safe Mode, memicu `scanpst.exe` perbaikan PST/OST, dan mereset `Normal.dotm`. |
| **74**| **Pembersih Konflik Akun & Lisensi Office** | Menghapus sisa *ghost key* lisensi lama dan mereset cache login / token identitas Microsoft modern. |

### 9. 📜 Audit Lisensi & Pelaporan Servis
| No | Modul | Fungsi Utama |
|:---:|:---|:---|
| **18**| **Audit Status Lisensi Windows & Office** | Memeriksa detail kanal lisensi (OEM, Retail, Volume KMS/MAK) dan sisa masa aktif. |
| **39**| **Ekstraksi Product Key BIOS OEM** | Membaca lisensi asli pabrikan yang tertanam di chip motherboard (MSDM Table) PC/Laptop. |
| **40**| **Bersihkan Token & Host KMS Palsu** | Menghapus server KMS bajakan pihak ketiga yang tertanam di registry dan membersihkan token aktivasi. |
| **42**| **Alihkan Edisi Windows (Home ke Pro)** | Meningkatkan versi Windows 10/11 Home ke Professional menggunakan kunci generik resmi Microsoft tanpa format ulang. |
| **76**| **Cetak Berita Acara & Laporan Servis (HTML)**| Menghasilkan Berita Acara Servis resmi berformat HTML profesional dengan ringkasan status perangkat dan log perbaikan. |

---

## 🚀 Panduan Penggunaan Cepat (Quick Start)

### Cara 1: Menggunakan Smart Bootstrapper (Direkomendasikan)
1. Unduh atau ekstrak seluruh isi folder repositori ke drive lokal atau Flashdisk teknisi.
2. Klik ganda pada [`START.bat`](START.bat).
3. Klik **Yes** pada konfirmasi UAC Windows.
4. Aplikasi akan otomatis meluncurkan antarmuka grafis (GUI) modern dengan hak penuh Administrator.

### Cara 2: Menggunakan Terminal CLI (Untuk WinPE, Safe Mode, atau Low-End PC)
1. Buka Command Prompt sebagai Administrator.
2. Jalankan:
   ```cmd
   IT_SUPPORT_CENTER.bat
   ```
3. Masukkan nomor menu pilihan Anda `[1 - 86]` dan tekan Enter.

---

## 🛠️ Panduan Build dari Kode Sumber

Jika Anda ingin mengompilasi ulang aplikasi sendiri dari source code C#:

### Prasyarat:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10 / 11 / Server (x64)

### Perintah Kompilasi Single-File Terkompresi:
```bash
dotnet publish src/ITSupportCenter/ITSupportCenter.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:EnableCompressionInSingleFile=true \
  -o dist/release
```
Biner hasil build akan berada di `dist/release/ITSupportCenter.exe` (~71 MB), yang mencakup seluruh runtime .NET dan asset ikon tanpa membutuhkan ketergantungan luar.

---

## 🏛️ Arsitektur & Prinsip Desain

Aplikasi ini dirancang mengikuti panduan pengembangan ketat pada [`AI_DEV_GUIDE.md`](AI_DEV_GUIDE.md):
- **Command / Plugin Pattern:** Setiap tool terisolasi dalam 1 file class mandiri yang mengimplementasikan antarmuka `IToolCommand`.
- **Zero-Coupling:** Penambahan atau modifikasi modul baru tidak akan merusak modul lainnya.
- **Dual-Layer Fail-Safe:** Setiap manipulasi sistem memiliki mekanisme fallback (Win32 API ➔ CLI System Tools).

---

## ⚖️ Penafian Hukum (Legal Disclaimer)

Aplikasi ini disediakan secara **"AS-IS"** (sebagaimana adanya) untuk tujuan administratif dan pemeliharaan teknis profesional. Seluruh tindakan eksekusi perbaikan, kepatuhan lisensi, dan pencadangan data (*backup*) berada di bawah tanggung jawab penuh pengguna. Baca dokumen lengkap di [`DISCLAIMER.md`](DISCLAIMER.md).

---

## 👨‍💻 Pengembang Resmi & Kontak

<div align="center">

Aplikasi ini dikembangkan dan didukung secara resmi oleh:

### **AuraCore**
🌐 **Website:** [https://www.auracore.my.id](https://www.auracore.my.id)  
📧 **Kontak / Dukungan:** Kunjungi [auracore.my.id](https://www.auracore.my.id)  

*Copyright © 2026 AuraCore. Seluruh hak cipta dilindungi undang-undang.*

</div>
