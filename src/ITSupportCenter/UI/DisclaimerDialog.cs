using System;
using System.Drawing;
using System.Windows.Forms;

namespace ITSupportCenter.UI
{
    public class DisclaimerDialog : Form
    {
        public DisclaimerDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "⚖️ Disclaimer Hukum & Batasan Tanggung Jawab (Limitation of Liability)";
            Size = new Size(820, 640);
            MinimumSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(24, 26, 36);
            ForeColor = Color.FromArgb(236, 240, 241);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Top Header Panel
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 34, 48),
                Padding = new Padding(16, 12, 16, 12)
            };

            var lblTitle = new Label
            {
                Text = "⚖️ PERNYATAAN BEBAS TANGGUNG JAWAB HUKUM",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60), // Coral / Red Alert
                Location = new Point(14, 10),
                AutoSize = true
            };

            var lblSubtitle = new Label
            {
                Text = "IT Support Center 2026 (v3.2.0 Enterprise) — Klausul Penggunaan 'AS-IS' & Penolakan Liabilitas",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(189, 195, 199),
                Location = new Point(16, 38),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            // Bottom Action Panel
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = Color.FromArgb(30, 34, 48),
                Padding = new Padding(16, 10, 16, 10)
            };

            var btnAccept = new Button
            {
                Text = "✓ Saya Mengerti & Menyetujui Ketentuan Ini",
                Dock = DockStyle.Right,
                Width = 280,
                BackColor = Color.FromArgb(39, 174, 96), // Emerald Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnAccept.FlatAppearance.BorderSize = 0;
            btnAccept.Click += (s, e) => Close();

            pnlBottom.Controls.Add(btnAccept);

            // Center Content (RichTextBox)
            var rtbContent = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 20, 28),
                ForeColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 9.5F, FontStyle.Regular),
                ReadOnly = true,
                Padding = new Padding(12)
            };

            rtbContent.Rtf = GenerateDisclaimerRtf();

            Controls.Add(rtbContent);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
        }

        private string GenerateDisclaimerRtf()
        {
            // Rich format text explaining all legal liability disclaimers
            return @"{\rtf1\ansi\deff0
{\colortbl ;\red231\green76\blue60;\red46\green204\blue113;\red52\green152\blue219;\red241\green196\blue15;\red236\green240\blue241;\red189\green195\blue199;}
\cf5\b ==========================================================================================\b0\par
\cf3\b PENOLAKAN TANGGUNG JAWAB HUKUM (LEGAL DISCLAIMER & LIMITATION OF LIABILITY)\b0\par
\cf4 Aplikasi: IT Support Center 2026 (v3.2.0 Enterprise Edition)\par
\cf5\b ==========================================================================================\b0\par\par

\cf1\b 1. PENYEDIAAN 'SEBAGAIMANA ADANYA' (AS-IS BASIS & NO WARRANTY)\b0\par
\cf6 Perangkat lunak ini, beserta seluruh skrip pendukung, modul otomasi sistem, dan berkas di dalamnya, disediakan secara \b 'SEBAGAIMANA ADANYA' (AS-IS)\b0  dan \b 'SEBAGAIMANA TERSEDIA' (AS-AVAILABLE)\b0  tanpa jaminan dalam bentuk apa pun, baik tersurat maupun tersirat.\par
Penyedia/pembuat perangkat lunak tidak memberikan garansi atas kelayakan komersial, kesesuaian untuk kebutuhan spesifik tertentu, ketiadaan kesalahan (bug-free), maupun kompatibilitas mutlak dengan seluruh konfigurasi sistem operasi atau hardware komputer pengguna.\par\par

\cf1\b 2. BATASAN TANGGUNG JAWAB MUTLAK (LIMITATION OF LIABILITY)\b0\par
\cf6 Dalam keadaan dan dasar hukum apa pun (baik dalam tuntutan perdata, perbuatan melawan hukum/tort, kelalaian, wanprestasi kontrak, atau tanggung jawab hukum mutlak), \b PEMBUAT, PENGEMBANG, MAUPUN PENYEDIA APLIKASI INI TIDAK DAPAT DITUNTUT MAUPUN DIKENAKAN GANTI RUGI\b0  atas segala bentuk kerugian yang timbul dari pemakaian atau ketidakmampuan memakai aplikasi ini, termasuk namun tidak terbatas pada:\par
  \cf4\b a.\b0\cf6  Kerusakan fisik perangkat keras (motherboard, SSD/HDD, RAM, power supply, printer, adapter);\par
  \cf4\b b.\b0\cf6  Kehilangan data, terhapusnya file kerja, korupsi database, atau kerusakan dokumen penting;\par
  \cf4\b c.\b0\cf6  Gangguan jaringan, terputusnya koneksi internet, hilangnya konfigurasi domain Active Directory;\par
  \cf4\b d.\b0\cf6  Kerugian finansial, hilangnya potensi bisnis, atau berhentinya operasional kantor (downtime);\par
  \cf4\b e.\b0\cf6  Kerugian tidak langsung, insidental, atau konsekuensial lainnya.\par\par

\cf1\b 3. HAK AKSES ADMINISTRATOR & RISIKO PENGGUNA (USE AT YOUR OWN RISK)\b0\par
\cf6 Perangkat lunak ini dirancang untuk teknisi IT profesional dan memerlukan hak akses Administrator penuh (UAC Elevation) untuk mengubah Registry, Service Windows, Firewall, BCD, WMI, dan TCP/IP Stack. Pengguna secara sadar mengakui bahwa:\par
  \cf2\b •\b0\cf6  Seluruh eksekusi tool dilakukan atas inisiatif, izin, dan \b RISIKO SENDIRI PENGGUNA (AT YOUR OWN RISK)\b0 .\par
  \cf2\b •\b0\cf6  Pengguna bertanggung jawab penuh melakukan pencadangan data (backup) dan membuat \b System Restore Point\b0  sebelum menjalankan fungsi perbaikan mendalam.\par
  \cf2\b •\b0\cf6  Pengguna wajib memahami dampak setiap tombol yang ditekan terhadap konfigurasi komputernya.\par\par

\cf1\b 4. BUKAN PENGGANTI DUKUNGAN RESMI VENDOR\b0\par
\cf6 Aplikasi ini merupakan perangkat bantu diagnostik mandiri dan \b BUKAN\b0  pengganti layanan dukungan resmi bergaransi dari vendor resmi (seperti Microsoft Corporation, Dell, HP, Lenovo, atau penyedia hardware resmi lainnya).\par\par

\cf5\b ------------------------------------------------------------------------------------------\b0\par
\cf2\b DENGAN MEMBUKA, MENJALANKAN, ATAU MENGEKSEKUSI PERANGKAT LUNAK INI, PENGGUNA DENGAN INI MENYATAKAN TELAH MEMBACA, MEMAHAMI, DAN MENYETUJUI SELURUH KETENTUAN DI ATAS SERTA MELEPASKAN PEMBUAT APLIKASI DARI SEGALA BENTUK TUNTUTAN HUKUM.\b0\par
\cf5\b ------------------------------------------------------------------------------------------\b0\par
}";
        }
    }
}
