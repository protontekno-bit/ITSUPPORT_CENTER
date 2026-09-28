using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.UI
{
    public class OperationalManualForm : Form
    {
        private readonly MainForm _mainForm;
        private TextBox _searchBox = null!;
        private TreeView _treeTopics = null!;
        private RichTextBox _rtbContent = null!;
        private FlowLayoutPanel _pnlActionButtons = null!;
        private Label _lblActionHeader = null!;
        private Button _btnExport = null!;
        private Button _btnClose = null!;

        public OperationalManualForm(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            LoadTopics();
            ShowTopic("overview");
        }

        private void InitializeComponent()
        {
            Text = "📖 Manual Operasional Lengkap & Interactive Playbook - IT Support Center 2026";
            Size = new Size(1180, 800);
            MinimumSize = new Size(950, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(24, 26, 36);
            ForeColor = Color.FromArgb(236, 240, 241);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            // Top Header Panel
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(30, 34, 48),
                Padding = new Padding(16, 10, 16, 10)
            };

            var lblTitle = new Label
            {
                Text = "📖 BUKU MANUAL OPERASIONAL & PLAYBOOK TERINTEGRASI",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(14, 10),
                AutoSize = true
            };

            var lblSubtitle = new Label
            {
                Text = "Standard Operating Procedure (SOP), Petunjuk Penggunaan 70 Modul, Mitigasi Masalah & Eksekusi Langsung Alat",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(16, 36),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // Bottom Action Bar
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(30, 34, 48),
                Padding = new Padding(12, 8, 12, 8)
            };

            _btnExport = new Button
            {
                Text = "📋 Salin Bab Ini ke Clipboard",
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnExport.FlatAppearance.BorderSize = 0;
            _btnExport.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_rtbContent.Text))
                {
                    Clipboard.SetText(_rtbContent.Text);
                    MessageBox.Show(this, "Konten bab berhasil disalin ke Clipboard!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            _btnClose = new Button
            {
                Text = "Tutup Manual",
                Dock = DockStyle.Right,
                Width = 120,
                BackColor = Color.FromArgb(70, 78, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.Click += (s, e) => Close();

            pnlBottom.Controls.Add(_btnExport);
            pnlBottom.Controls.Add(_btnClose);

            // Main Split: Left (Navigation Tree & Search) / Right (Rich Content Reader + Action Dock)
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 330,
                SplitterWidth = 6,
                BackColor = Color.FromArgb(18, 20, 28)
            };

            // Left Navigation Container
            var pnlNav = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(28, 31, 44), Padding = new Padding(8) };

            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.Transparent };
            _searchBox = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 52, 70),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                PlaceholderText = "🔍 Cari topik (mis: Ransomware, AD, Spooler)..."
            };
            _searchBox.TextChanged += (s, e) => FilterTopics(_searchBox.Text.Trim());
            pnlSearch.Controls.Add(_searchBox);

            _treeTopics = new TreeView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 26, 36),
                ForeColor = Color.FromArgb(220, 230, 242),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5F),
                FullRowSelect = true,
                ShowLines = true,
                ShowPlusMinus = true
            };
            _treeTopics.AfterSelect += (s, e) =>
            {
                if (e.Node?.Tag is string tag)
                {
                    ShowTopic(tag);
                }
            };

            pnlNav.Controls.Add(_treeTopics);
            pnlNav.Controls.Add(pnlSearch);

            // Right Content Viewer
            var pnlReader = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(20, 22, 30), Padding = new Padding(12) };

            // Dynamic Action Dock (Bottom of Reader Panel)
            var pnlActionDock = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 110,
                BackColor = Color.FromArgb(28, 32, 46),
                Padding = new Padding(10, 6, 10, 6)
            };

            _lblActionHeader = new Label
            {
                Text = "⚡ ALAT TERKAIT UNTUK TOPIK INI (KLIK UNTUK EKSEKUSI LANGSUNG):",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15), // Sunflower Gold
                Dock = DockStyle.Top,
                Height = 22
            };

            _pnlActionButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 4, 0, 0)
            };

            pnlActionDock.Controls.Add(_pnlActionButtons);
            pnlActionDock.Controls.Add(_lblActionHeader);

            _rtbContent = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 22, 30),
                ForeColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10.5F),
                ReadOnly = true
            };

            pnlReader.Controls.Add(_rtbContent);
            pnlReader.Controls.Add(pnlActionDock);

            splitContainer.Panel1.Controls.Add(pnlNav);
            splitContainer.Panel2.Controls.Add(pnlReader);

            Controls.Add(splitContainer);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
        }

        private void LoadTopics()
        {
            _treeTopics.Nodes.Clear();

            var nodeOverview = new TreeNode("📌 1. Pendahuluan & Arsitektur") { Tag = "overview" };
            var nodeSop = new TreeNode("🚨 2. Standard Operating Procedures (SOP)") { Tag = "sop_intro" };
            nodeSop.Nodes.Add(new TreeNode("☣️ SOP 1: Tanggap Darurat Ransomware") { Tag = "sop_ransomware" });
            nodeSop.Nodes.Add(new TreeNode("🖨️ SOP 2: Troubleshooting Printer Sharing (0x11b)") { Tag = "sop_printer" });
            nodeSop.Nodes.Add(new TreeNode("🌐 SOP 3: Perbaikan Total Jaringan & SMB (0x800704f8)") { Tag = "sop_network" });
            nodeSop.Nodes.Add(new TreeNode("🏢 SOP 4: Join Domain Active Directory & Rename PC") { Tag = "sop_ad" });
            nodeSop.Nodes.Add(new TreeNode("🖥️ SOP 5: Pengelolaan Remote Desktop (AnyDesk/RustDesk)") { Tag = "sop_remote" });
            nodeSop.Nodes.Add(new TreeNode("💾 SOP 6: Kloning OS & Bare-Metal Rescue") { Tag = "sop_cloning" });
            nodeSop.Nodes.Add(new TreeNode("🔬 SOP 7: Deep Audit Hardware & Lisensi") { Tag = "sop_hardware" });

            var allTools = ToolRegistry.GetAllTools();
            var nodeModules = new TreeNode($"🗂️ 3. Katalog Detail Modul Terdaftar ({allTools.Count} Modul)") { Tag = "modules_intro" };
            nodeModules.Nodes.Add(new TreeNode($"🛠️ Perbaikan Sistem ({allTools.Count(t => t.Category == ToolCategory.System)} Modul)") { Tag = "cat_system" });
            nodeModules.Nodes.Add(new TreeNode($"🖨️ Printer Sharing ({allTools.Count(t => t.Category == ToolCategory.Printer)} Modul)") { Tag = "cat_printer" });
            nodeModules.Nodes.Add(new TreeNode($"🌐 Jaringan & File Sharing ({allTools.Count(t => t.Category == ToolCategory.Network)} Modul)") { Tag = "cat_network" });
            nodeModules.Nodes.Add(new TreeNode($"🖥️ Remote Desktop ({allTools.Count(t => t.Category == ToolCategory.Remote)} Modul)") { Tag = "cat_remote" });
            nodeModules.Nodes.Add(new TreeNode($"🛡️ Keamanan & Firewall ({allTools.Count(t => t.Category == ToolCategory.Security)} Modul)") { Tag = "cat_security" });
            nodeModules.Nodes.Add(new TreeNode($"📡 Audit & Scanner ({allTools.Count(t => t.Category == ToolCategory.Scanner)} Modul)") { Tag = "cat_scanner" });
            nodeModules.Nodes.Add(new TreeNode($"📜 Lisensi Windows ({allTools.Count(t => t.Category == ToolCategory.License)} Modul)") { Tag = "cat_license" });

            var nodeTips = new TreeNode("💡 4. Tips Lapangan & Best Practices") { Tag = "field_tips" };
            var nodeDisclaimer = new TreeNode("⚖️ 5. Disclaimer & Batasan Tanggung Jawab") { Tag = "disclaimer" };

            _treeTopics.Nodes.Add(nodeOverview);
            _treeTopics.Nodes.Add(nodeSop);
            _treeTopics.Nodes.Add(nodeModules);
            _treeTopics.Nodes.Add(nodeTips);
            _treeTopics.Nodes.Add(nodeDisclaimer);

            _treeTopics.ExpandAll();
        }

        private void FilterTopics(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadTopics();
                return;
            }

            keyword = keyword.ToLowerInvariant();
            LoadTopics();

            foreach (TreeNode parent in _treeTopics.Nodes)
            {
                bool matchParent = parent.Text.ToLowerInvariant().Contains(keyword);
                bool childMatch = false;

                foreach (TreeNode child in parent.Nodes)
                {
                    if (child.Text.ToLowerInvariant().Contains(keyword))
                    {
                        childMatch = true;
                        child.BackColor = Color.FromArgb(41, 128, 185);
                    }
                    else
                    {
                        child.BackColor = Color.Transparent;
                    }
                }

                if (matchParent || childMatch)
                {
                    parent.Expand();
                }
            }
        }

        private void ShowTopic(string topicTag)
        {
            _rtbContent.Clear();
            _pnlActionButtons.Controls.Clear();

            List<string> toolIdsToRender = new();

            switch (topicTag)
            {
                case "overview":
                    AppendHeading("📌 PENDAHULUAN & ARSITEKTUR APLIKASI");
                    AppendParagraph("IT Support Center 2026 adalah suite utilitas mandiri (zero-dependency) yang dirancang khusus untuk Teknisi IT Support, Helpdesk L1/L2, dan System Administrator dalam menangani pemeliharaan dan troubleshooting sistem operasi Windows.");
                    AppendSubheading("1. Dual-Interface Architecture (GUI & CLI)");
                    AppendBullet("GUI Interaktif (C# .NET 8 WPF/WinForms): Memberikan visual modern, pencarian instan, dan log real-time.");
                    AppendBullet("Master CLI Batch (IT_SUPPORT_CENTER.bat): 70 opsi zero-fail untuk eksekusi cepat di Safe Mode, Command Prompt WinPE, atau lingkungan terbatas.");
                    AppendSubheading("2. Mekanisme Zero-Fail & TrueNAS UNC Support");
                    AppendBullet("File 'START.bat' otomatis meminta hak Administrator (UAC Auto-Elevate).");
                    AppendBullet("Mendukung pembukaan dari Network Share UNC (misal: \\\\192.168.1.10\\TrueNAS\\ITTOOLS) dengan otomatis membuat pemetaan drive sementara (Z:).");
                    AppendSubheading("3. Standar Keamanan & Tanpa Antivirus False-Positive");
                    AppendBullet("100% utilitas menggunakan native command Windows (netsh, dism, pnputil, w32tm, bcdedit, powershell) sehingga aman dari pemblokiran EDR/Windows Defender.");
                    toolIdsToRender.AddRange(new[] { "sys_auto_health_check", "sys_pc_asset_info", "sys_deep_hardware_inspector" });
                    break;

                case "sop_intro":
                    AppendHeading("🚨 STANDARD OPERATING PROCEDURES (SOP) TEKNISI");
                    AppendParagraph("Pilih sub-menu di sebelah kiri untuk melihat langkah-langkah penanganan insiden darurat, troubleshooting printer, perbaikan jaringan, kloning OS, hingga Active Directory.");
                    toolIdsToRender.AddRange(new[] { "sys_auto_health_check", "net_full_repair", "print_fix_sharing" });
                    break;

                case "sop_ransomware":
                    AppendHeading("☣️ SOP 1: TANGGAP DARURAT SERANGAN RANSOMWARE");
                    AppendParagraph("Jika komputer terindikasi terkena serangan Ransomware atau Malware yang menyebar cepat (lateral movement), segera lakukan langkah berikut:");
                    AppendSubheading("Langkah 1: Isolasi Jaringan Instan (Air-Gap Quarantine)");
                    AppendBullet("Jalankan Modul 'Karantina Isolasi Jaringan Darurat (Air-Gap)'.");
                    AppendBullet("Firewall akan memblokir 100% lalu lintas masuk dan keluar (Inbound & Outbound block) secara instan tanpa mematikan kartu jaringan.");
                    AppendSubheading("Langkah 2: Amankan Kredensial & Putus Sesi");
                    AppendBullet("Jalankan 'Putus Drive Share & Kerberos' (net use * /delete & klist purge).");
                    AppendBullet("Jalankan 'Hardening Anti-Ransomware' untuk mematikan port SMBv1, AutoRun flashdisk, dan RemoteRegistry.");
                    AppendSubheading("Langkah 3: Amankan Kunci Pemulihan");
                    AppendBullet("Jalankan 'Audit Status BitLocker & Ekstrak 48-Digit Recovery Key' dan catat kunci pemulihan sebelum reboot.");
                    AppendSubheading("Langkah 4: Normalisasi Pasca Pembersihan");
                    AppendBullet("Setelah OS dibersihkan atau discan tuntas, gunakan opsi 'Normalisasi' pada modul Karantina untuk membuka kembali koneksi standar.");
                    toolIdsToRender.AddRange(new[] { "sec_emergency_quarantine", "sec_hardening_ransomware", "sec_bitlocker_audit", "net_disconnect_shares", "sec_reset_firewall", "net_restore_sharing" });
                    break;

                case "sop_printer":
                    AppendHeading("🖨️ SOP 2: SOLUSI MASALAH PRINTER SHARING & SPOOLER");
                    AppendParagraph("Masalah printer merupakan 40% tiket helpdesk harian. Ikuti alur penanganan berikut:");
                    AppendSubheading("Kasus 1: Error Printer Sharing 0x0000011b / 0x00000709");
                    AppendBullet("Jalankan Modul 'Fix Printer Sharing 0x0000011b' pada komputer Client maupun komputer Host Printer.");
                    AppendBullet("Modul akan mengatur RpcAuthnLevelPrivacyEnabled=0 dan mengaktifkan Point-and-Print Driver Restriction Fix.");
                    AppendBullet("Restart komputer host & client.");
                    AppendSubheading("Kasus 2: Dokumen Nyangkut / Spooler Freeze");
                    AppendBullet("Jalankan Modul 'Bersihkan Antrean Cetak Spooler Nyangkut' untuk menghapus file spool corrupt (.SHD / .SPL).");
                    AppendSubheading("Kasus 3: Print Spooler Mati Sendiri (Crash Loop)");
                    AppendBullet("Jalankan Modul 'Reset Total Spooler & Solusi Spooler Crash Loop'. Modul akan mereset Print Processors, Monitors, dan dependencies ke setelan pabrik.");
                    toolIdsToRender.AddRange(new[] { "print_fix_sharing", "print_clear_queue", "print_spooler_factory_reset", "print_audit_test_page", "print_open_cpl" });
                    break;

                case "sop_network":
                    AppendHeading("🌐 SOP 3: PERBAIKAN TOTAL JARINGAN & SHARING (0x800704f8)");
                    AppendSubheading("Kasus 1: Folder Share Tidak Bisa Dibuka (Guest Logon Blocked)");
                    AppendBullet("Gejala: Muncul pesan error 'You can't access this shared folder because your organization's security policies block unauthenticated guest access' (0x800704f8).");
                    AppendBullet("Solusi: Jalankan Modul 'Fix SMB Guest Authentication'. Modul akan mengaktifkan AllowInsecureGuestAuth=1 pada LanmanWorkstation.");
                    AppendSubheading("Kasus 2: Status 'No Internet, Secured' atau DNS Macet");
                    AppendBullet("Jalankan Modul 'Perbaikan Total Jaringan 1-Klik (Full Network Repair)'.");
                    AppendBullet("Modul akan mereset Winsock, TCP/IP Stack, Flush DNS, reset WinHTTP proxy, dan merilis DHCP IP.");
                    AppendSubheading("Kasus 3: Cek Titik Hambatan Koneksi (Diagnosa 3-Titik)");
                    AppendBullet("Gunakan modul 'Diagnosa Kualitas Koneksi 3-Titik' untuk mengetahui secara pasti apakah gangguan ada pada Router Lokal (Gateway), ISP Kantor, atau Koneksi Global.");
                    toolIdsToRender.AddRange(new[] { "net_smb_guest_auth", "net_full_repair", "net_connection_diagnostics", "net_reset_stack", "net_dns_switch", "net_reset_proxy_hosts" });
                    break;

                case "sop_ad":
                    AppendHeading("🏢 SOP 4: JOIN DOMAIN ACTIVE DIRECTORY & RENAME PC");
                    AppendSubheading("Prosedur Standar Komputer Baru Masuk Domain:");
                    AppendBullet("1. Pastikan DNS Adapter komputer sudah mengarah ke IP Domain Controller (Gunakan Modul 'Pengatur DNS Cepat Adapter').");
                    AppendBullet("2. Jalankan Modul 'Asisten Active Directory & Join Domain' -> Pilih Opsi [5] Uji Koneksi DC untuk memverifikasi port 53, 88, 389, 445 terbuka.");
                    AppendBullet("3. Pilih Opsi [2] Ganti Nama Komputer sesuai format aset kantor (misal: IT-WS-01).");
                    AppendBullet("4. Pilih Opsi [3] Gabung ke Domain Active Directory: Masukkan nama domain (corp.internal.net), username admin domain, password, dan target OU.");
                    AppendBullet("5. Lakukan Restart Komputer. Login menggunakan akun Domain (domain\\username).");
                    toolIdsToRender.AddRange(new[] { "net_ad_domain_assistant", "net_dns_switch", "net_credential_mgr", "net_sync_time_ntp" });
                    break;

                case "sop_remote":
                    AppendHeading("🖥️ SOP 5: PENGELOLAAN REMOTE DESKTOP (ANYDESK / TV / RUSTDESK)");
                    AppendSubheading("1. AnyDesk ID Bentrok / Limit Lisensi");
                    AppendBullet("Terjadi pada PC hasil kloning OS di mana 2 PC memiliki ID AnyDesk yang sama.");
                    AppendBullet("Solusi: Jalankan 'Reset ID & Konfigurasi AnyDesk'. Modul akan menghapus service.conf dan system.conf sehingga AnyDesk mendapatkan ID baru yang unik.");
                    AppendSubheading("2. TeamViewer Password Permanen");
                    AppendBullet("Jalankan 'Ganti Password TeamViewer' untuk mengatur password unattended access tanpa perlu membuka GUI TeamViewer.");
                    AppendSubheading("3. RustDesk Open-Source Remote & Self-Hosted Relay");
                    AppendBullet("Jalankan 'Pusat Manajemen & Utilitas RustDesk':");
                    AppendBullet("- Pasang via Winget otomatis (Opsi 4).");
                    AppendBullet("- Hubungkan ke Server Relay Kantor pribadi (Opsi 3: Masukkan Host & Key).");
                    AppendBullet("- Reset ID jika terjadi duplikasi ID pasca kloning (Opsi 2).");
                    toolIdsToRender.AddRange(new[] { "remote_rustdesk_manager_hub", "remote_reset_anydesk_id", "remote_change_tv_pass" });
                    break;

                case "sop_cloning":
                    AppendHeading("💾 SOP 6: KLONING OS & BARE-METAL RESCUE");
                    AppendParagraph("Panduan deployment massal dan pemulihan komputer mati total:");
                    AppendSubheading("1. Multi-Boot USB Master (Ventoy)");
                    AppendBullet("Gunakan 'Pusat Flashdisk Multi-Boot Master' untuk membuat flashdisk bootable universal. Cukup drag-and-drop file ISO ke flashdisk.");
                    AppendSubheading("2. Deployment Disk Kloning (Clonezilla & Rescuezilla)");
                    AppendBullet("Gunakan 'Pusat Kloning & Deployment Hub' untuk panduan Device-to-Image (backup sistem ke NAS/Samba) dan Device-to-Device (cloning SSD ke NVMe).");
                    AppendSubheading("3. WinPE & Live Rescue (Hiren's, DLC Boot, UBCD)");
                    AppendBullet("Gunakan 'Pusat Live Rescue & WinPE Hub' untuk perbaikan MBR/GPT, reset password Windows SAM offline, dan recovery data harddisk bad sector.");
                    toolIdsToRender.AddRange(new[] { "sys_ventoy_guide", "sys_disk_cloning_deployment", "sys_live_rescue_toolkit", "sys_dism_wim_backup" });
                    break;

                case "sop_hardware":
                    AppendHeading("🔬 SOP 7: DEEP AUDIT HARDWARE & LISENSI");
                    AppendSubheading("1. Inspeksi Total Fisik Komputer");
                    AppendBullet("Jalankan 'Inspeksi Total Spesifikasi & Kesehatan Hardware (Deep Audit)'.");
                    AppendBullet("Membaca rincian Slot RAM (Speed MHz, DDR3/4/5), Model SSD NVMe/SATA, Health Status S.M.A.R.T, TPM 2.0, Secure Boot, dan Battery Wear Level.");
                    AppendSubheading("2. Ekstraksi Lisensi OEM BIOS");
                    AppendBullet("Jalankan 'Ekstrak Kunci Lisensi OEM Asli dari BIOS' untuk membaca Product Key original yang tertanam di motherboard MSDM Table sebelum instal ulang.");
                    toolIdsToRender.AddRange(new[] { "sys_deep_hardware_inspector", "lic_extract_oem_key", "sys_disk_smart_health", "lic_audit_win_license", "sys_battery_health" });
                    break;

                case "modules_intro":
                    var allRegistered = ToolRegistry.GetAllTools();
                    AppendHeading($"🗂️ KATALOG DETAIL SELURUH MODUL APLIKASI ({allRegistered.Count} MODUL AKTIF)");
                    AppendParagraph("Buku manual ini terhubung secara otomatis dengan ToolRegistry aplikasi. Setiap kali ada fitur baru ditambahkan ke dalam sistem, seluruh rincian modul, mitigasi masalah, dampak sistem, dan tombol eksekusinya akan otomatis dimuat di sini secara real-time.");
                    AppendParagraph("Silakan klik salah satu kategori modul pada menu navigasi sebelah kiri untuk membaca SOP dan panduan teknis per modul.");
                    toolIdsToRender.AddRange(allRegistered.Take(10).Select(t => t.Id));
                    break;

                case "cat_system":
                    RenderCategoryTopic(ToolCategory.System, toolIdsToRender);
                    break;

                case "cat_printer":
                    RenderCategoryTopic(ToolCategory.Printer, toolIdsToRender);
                    break;

                case "cat_network":
                    RenderCategoryTopic(ToolCategory.Network, toolIdsToRender);
                    break;

                case "cat_remote":
                    RenderCategoryTopic(ToolCategory.Remote, toolIdsToRender);
                    break;

                case "cat_security":
                    RenderCategoryTopic(ToolCategory.Security, toolIdsToRender);
                    break;

                case "cat_scanner":
                    RenderCategoryTopic(ToolCategory.Scanner, toolIdsToRender);
                    break;

                case "cat_license":
                    RenderCategoryTopic(ToolCategory.License, toolIdsToRender);
                    break;

                case "field_tips":
                    AppendHeading("💡 TIPS LAPANGAN & BEST PRACTICES TEKNISI IT");
                    AppendSubheading("1. Penanganan PC Lemot & RAM Penuh (High Memory Usage):");
                    AppendBullet("Jalankan 'Optimasi RAM Cerdas & Pembersih Proses' (sys_smart_ram_optimizer) untuk memangkas memory leak cache idle secara instan tanpa menutup aplikasi aktif pengguna.");
                    AppendBullet("Lanjutkan dengan 'Pembersih File Temp & Sampah Disk' -> 'Restart Explorer Shell' -> 'Disable Edge Background Boost'.");
                    AppendSubheading("2. Menyiapkan PC Karyawan Baru (Fast Provisioning):");
                    AppendBullet("Pasang Paket Software Kantor Otomatis (Winget) -> Kembalikan Menu Klik Kanan Klasik -> Pasang .NET 3.5 -> Join Domain AD.");
                    AppendSubheading("3. Sebelum Melakukan Perubahan Besar:");
                    AppendBullet("Selalu gunakan '1-Klik Buat System Restore Point' atau 'Backup Drivers OEM' agar ada jalur rollback yang aman.");
                    toolIdsToRender.AddRange(new[] { "sys_smart_ram_optimizer", "sys_clean_temp", "sys_winget_installer", "sys_create_restore_point", "sys_restore_classic_context", "net_ad_domain_assistant" });
                    break;

                case "disclaimer":
                    AppendHeading("⚖️ PENOLAKAN TANGGUNG JAWAB HUKUM (LEGAL DISCLAIMER)");
                    AppendParagraph("IT Support Center 2026 (v3.2.0 Enterprise Edition) disediakan untuk teknisi IT dan administrator sistem profesional.");
                    AppendSubheading("1. Ketentuan 'Sebagaimana Adanya' (AS-IS Basis & No Warranty)");
                    AppendBullet("Perangkat lunak ini disediakan tanpa jaminan apa pun, baik tersurat maupun tersirat.");
                    AppendBullet("Tidak ada jaminan ketiadaan bug, kelayakan komersial, atau kesesuaian absolut terhadap seluruh varian perangkat keras dan build OS.");
                    AppendSubheading("2. Batasan Tanggung Jawab Mutlak (Limitation of Liability)");
                    AppendBullet("Pembuat dan penyedia perangkat lunak TIDAK BERTANGGUNG JAWAB atas kerusakan hardware, kehilangan data/dokumen, downtime jaringan, atau kerugian finansial apa pun.");
                    AppendBullet("Seluruh tuntutan ganti rugi perdata maupun tuntutan kelalaian tidak dapat dikenakan kepada pengembang atau penyedia.");
                    AppendSubheading("3. Risiko Pengguna (Use At Your Own Risk) & Hak Administrator");
                    AppendBullet("Modul memerlukan hak Administrator (UAC). Penggunaan dilakukan sepenuhnya atas inisiatif dan risiko sendiri pengguna.");
                    AppendBullet("Sangat disarankan membuat System Restore Point atau cadangan data berkala sebelum melakukan perbaikan mendalam.");
                    AppendSubheading("4. Bukan Pengganti Dukungan Resmi Vendor");
                    AppendBullet("Aplikasi ini adalah otomasi diagnostik mandiri dan bukan pengganti layanan bergaransi resmi dari vendor (Microsoft/OEM).");
                    toolIdsToRender.AddRange(new[] { "sys_create_restore_point", "sys_auto_health_check" });
                    break;

                default:
                    AppendHeading("📖 BUKU MANUAL OPERASIONAL IT SUPPORT CENTER");
                    AppendParagraph("Pilih salah satu topik pada menu navigasi sebelah kiri untuk membaca panduan detail.");
                    toolIdsToRender.AddRange(new[] { "sys_auto_health_check", "net_full_repair" });
                    break;
            }

            RenderActionButtons(toolIdsToRender);
        }

        private void RenderCategoryTopic(string categoryName, List<string> toolIdsToRender)
        {
            var tools = ToolRegistry.GetAllTools()
                .Where(t => t.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            AppendHeading($"🗂️ KATEGORI: {categoryName.ToUpperInvariant()} ({tools.Count} MODUL)");
            AppendParagraph($"Berikut adalah daftar seluruh modul operasional pada kategori {categoryName}. Setiap kali ada penambahan fitur baru di kategori ini, sistem akan otomatis mendaftarkannya ke dalam manual ini:");

            int idx = 1;
            foreach (var tool in tools)
            {
                AppendSubheading($"{idx}. {tool.Icon} {tool.Title}");
                AppendParagraph(tool.Description);

                var detail = ToolDetailInfoProvider.GetDetail(tool);
                if (!string.IsNullOrWhiteSpace(detail.ProblemSolved))
                    AppendBullet($"Masalah yang Diatasi: {detail.ProblemSolved}");
                if (!string.IsNullOrWhiteSpace(detail.SystemImpact))
                    AppendBullet($"Dampak Sistem: {detail.SystemImpact}");
                if (!string.IsNullOrWhiteSpace(detail.UsageGuide))
                    AppendBullet($"Petunjuk Penggunaan:\n     {detail.UsageGuide.Replace("\n", "\n     ")}");

                _rtbContent.AppendText("\n");
                toolIdsToRender.Add(tool.Id);
                idx++;
            }
        }

        private void RenderActionButtons(List<string> toolIds)
        {
            _pnlActionButtons.SuspendLayout();
            _pnlActionButtons.Controls.Clear();

            var allTools = ToolRegistry.GetAllTools();
            int count = 0;

            foreach (var id in toolIds)
            {
                var tool = allTools.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (tool == null) continue;

                count++;
                var btn = new Button
                {
                    Text = $"{tool.Icon} {tool.Title}",
                    AutoSize = true,
                    Height = 34,
                    BackColor = tool.ButtonColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Margin = new Padding(4, 3, 4, 3),
                    Tag = tool
                };
                btn.FlatAppearance.BorderSize = 0;

                // Tooltip
                var tip = new ToolTip();
                tip.SetToolTip(btn, $"Klik untuk menjalankan: {tool.Description}");

                // Click handler: Direct Execution with feedback
                btn.Click += async (s, e) =>
                {
                    btn.Enabled = false;
                    btn.Text = $"⏳ Menjalankan {tool.Icon}...";
                    try
                    {
                        await _mainForm.ExecuteToolDirectlyAsync(tool);
                        MessageBox.Show(this, $"Eksekusi '{tool.Title}' selesai! Silakan cek hasil di Terminal Log utama.", "Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"Terjadi error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btn.Enabled = true;
                        btn.Text = $"{tool.Icon} {tool.Title}";
                    }
                };

                // Context Menu: Option to Jump & Highlight on Main Dashboard
                var cm = new ContextMenuStrip();
                var miJump = new ToolStripMenuItem("🔍 Sorot & Buka di Menu Utama", null, (s, e) =>
                {
                    _mainForm.JumpAndHighlightTool(tool.Id);
                    Close();
                });
                cm.Items.Add(miJump);
                btn.ContextMenuStrip = cm;

                _pnlActionButtons.Controls.Add(btn);
            }

            if (count == 0)
            {
                _lblActionHeader.Text = "ℹ️ Tidak ada alat spesifik yang ditautkan pada topik ini.";
            }
            else
            {
                _lblActionHeader.Text = $"⚡ ALAT TERKAIT ({count} ALAT) - KLIK TOMBOL UNTUK EKSEKUSI LANGSUNG ATAU KLIK KANAN UNTUK SOROT:";
            }

            _pnlActionButtons.ResumeLayout();
        }

        private void AppendHeading(string text)
        {
            _rtbContent.SelectionFont = new Font("Segoe UI", 13F, FontStyle.Bold);
            _rtbContent.SelectionColor = Color.FromArgb(52, 152, 219);
            _rtbContent.AppendText(text + "\n\n");
        }

        private void AppendSubheading(string text)
        {
            _rtbContent.SelectionFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            _rtbContent.SelectionColor = Color.FromArgb(46, 204, 113);
            _rtbContent.AppendText(text + "\n");
        }

        private void AppendParagraph(string text)
        {
            _rtbContent.SelectionFont = new Font("Segoe UI", 10F, FontStyle.Regular);
            _rtbContent.SelectionColor = Color.FromArgb(236, 240, 241);
            _rtbContent.AppendText(text + "\n\n");
        }

        private void AppendBullet(string text)
        {
            _rtbContent.SelectionFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            _rtbContent.SelectionColor = Color.FromArgb(200, 215, 230);
            _rtbContent.AppendText("  • " + text + "\n");
        }
    }
}
