using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Services;
using ITSupportCenter.UI;

namespace ITSupportCenter
{
    public class MainForm : Form
    {
        private TextBox _searchBox = null!;
        private FlowLayoutPanel _cardsContainer = null!;
        private RichTextBox _logBox = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _lblStatusInfo = null!;
        private ToolStripProgressBar _progressBar = null!;
        private FlowLayoutPanel _categoryFilterPanel = null!;
        private string _selectedCategory = ToolCategory.All;
        private readonly Dictionary<string, Button> _categoryButtons = new();
        private readonly List<ActionCardControl> _activeCards = new();

        public MainForm()
        {
            InitializeComponent();
            Logger.Initialize(_logBox);
            LoadCategoryButtons();
            RefreshCards();
            LogWelcomeInfo();
        }

        private void InitializeComponent()
        {
            Text = "IT Support Center 2026 (v3.2.0 Enterprise) — AuraCore (https://www.auracore.my.id)";
            Size = new Size(1180, 780);
            MinimumSize = new Size(980, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(24, 26, 36); // Deep Navy/Dark Slate
            ForeColor = Color.FromArgb(236, 240, 241);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Main Split Layout: Left (Tools & Controls) / Right (Log Console)
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 720,
                SplitterWidth = 6,
                BackColor = Color.FromArgb(18, 20, 28)
            };

            // LEFT PANEL: Header + Search/Categories + FlowLayoutPanel
            var leftPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(24, 26, 36) };

            // 1. Top Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 34, 48),
                Padding = new Padding(16, 10, 16, 10)
            };

            var osInfo = OsDetector.GetOsInfo();

            var lblAppTitle = new Label
            {
                Text = "⚡ IT SUPPORT CENTER 2026  v3.2.0",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(14, 10),
                AutoSize = true
            };

            var lblSubtitle = new Label
            {
                Text = $"Target: {osInfo.OsName} ({osInfo.Architecture}, Build {osInfo.BuildNumber}) | RAM: {osInfo.TotalRamGb:F1} GB | Disk: {osInfo.DriveCType}",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(46, 204, 113), // Emerald Green
                Location = new Point(16, 38),
                AutoSize = true
            };

            var lnkDeveloper = new LinkLabel
            {
                Text = "🌐 AuraCore: https://www.auracore.my.id",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                LinkColor = Color.FromArgb(52, 152, 219),
                ActiveLinkColor = Color.FromArgb(46, 204, 113),
                VisitedLinkColor = Color.FromArgb(52, 152, 219),
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 16, 0),
                AutoSize = false,
                Width = 320,
                Cursor = Cursors.Hand
            };
            lnkDeveloper.LinkClicked += (s, e) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://www.auracore.my.id",
                        UseShellExecute = true
                    });
                }
                catch { }
            };

            headerPanel.Controls.Add(lnkDeveloper);
            headerPanel.Controls.Add(lblAppTitle);
            headerPanel.Controls.Add(lblSubtitle);

            // 2. Search & Filter Bar
            var filterContainer = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(28, 31, 44),
                Padding = new Padding(12, 8, 12, 4)
            };

            // Search Box Row
            var pnlSearchRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = Color.Transparent
            };

            var lblSearchIcon = new Label
            {
                Text = "🔍 Cari Fitur:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(189, 195, 199),
                Location = new Point(0, 5),
                Width = 90
            };

            _searchBox = new TextBox
            {
                Location = new Point(95, 3),
                Width = 370,
                Height = 26,
                BackColor = Color.FromArgb(45, 52, 70),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F)
            };
            _searchBox.TextChanged += (s, e) => RefreshCards();

            var btnClearSearch = new Button
            {
                Text = "✖ Reset",
                Location = new Point(472, 2),
                Width = 70,
                Height = 28,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClearSearch.FlatAppearance.BorderSize = 0;
            btnClearSearch.Click += (s, e) => { _searchBox.Clear(); _selectedCategory = ToolCategory.All; UpdateCategoryButtonStyles(); RefreshCards(); };

            var btnManual = new Button
            {
                Text = "📖 Manual",
                Location = new Point(548, 2),
                Width = 105,
                Height = 28,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnManual.FlatAppearance.BorderSize = 0;
            btnManual.Click += (s, e) =>
            {
                using var dlg = new OperationalManualForm(this);
                dlg.ShowDialog(this);
            };

            var btnDisclaimer = new Button
            {
                Text = "⚖️ Disclaimer",
                Location = new Point(660, 2),
                Width = 115,
                Height = 28,
                BackColor = Color.FromArgb(192, 57, 43), // Crimson / Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            btnDisclaimer.FlatAppearance.BorderSize = 0;
            btnDisclaimer.Click += (s, e) =>
            {
                using var dlg = new DisclaimerDialog();
                dlg.ShowDialog(this);
            };

            pnlSearchRow.Controls.Add(lblSearchIcon);
            pnlSearchRow.Controls.Add(_searchBox);
            pnlSearchRow.Controls.Add(btnClearSearch);
            pnlSearchRow.Controls.Add(btnManual);
            pnlSearchRow.Controls.Add(btnDisclaimer);

            // Category Badges Row
            _categoryFilterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 42,
                AutoScroll = true,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 4, 0, 0)
            };

            filterContainer.Controls.Add(_categoryFilterPanel);
            filterContainer.Controls.Add(pnlSearchRow);

            // 3. Card Container (Scrollable FlowPanel)
            _cardsContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(20, 22, 30),
                Padding = new Padding(12)
            };

            leftPanel.Controls.Add(_cardsContainer);
            leftPanel.Controls.Add(filterContainer);
            leftPanel.Controls.Add(headerPanel);

            // RIGHT PANEL: Log Console & Export Controls
            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(18, 20, 28) };

            var logHeaderPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(28, 31, 44),
                Padding = new Padding(12, 7, 12, 7)
            };

            var lblLogTitle = new Label
            {
                Text = "📋 Terminal Log",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(236, 240, 241),
                Dock = DockStyle.Left,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var pnlLogActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var btnClearLog = new Button
            {
                Text = "🗑️ Bersihkan",
                Size = new Size(88, 30),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Margin = new Padding(4, 0, 0, 0)
            };
            btnClearLog.FlatAppearance.BorderSize = 0;
            btnClearLog.Click += (s, e) => Logger.Clear();

            var btnExportLog = new Button
            {
                Text = "💾 Simpan Log",
                Size = new Size(98, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Margin = new Padding(4, 0, 0, 0)
            };
            btnExportLog.FlatAppearance.BorderSize = 0;
            btnExportLog.Click += (s, e) =>
            {
                string path = Logger.ExportLogToFile();
                if (!string.IsNullOrEmpty(path))
                    MessageBox.Show($"Laporan log berhasil disimpan di:\n{path}", "Log Tersimpan", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var btnReport = new Button
            {
                Text = "📄 Laporan HTML",
                Size = new Size(125, 30),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Margin = new Padding(4, 0, 0, 0)
            };
            btnReport.FlatAppearance.BorderSize = 0;
            btnReport.Click += (s, e) => ReportExporter.GenerateAndOpenHtmlReport();

            pnlLogActions.Controls.Add(btnClearLog);
            pnlLogActions.Controls.Add(btnExportLog);
            pnlLogActions.Controls.Add(btnReport);

            logHeaderPanel.Controls.Add(lblLogTitle);
            logHeaderPanel.Controls.Add(pnlLogActions);

            _logBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 14, 20),
                ForeColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Consolas", 9.25F, FontStyle.Regular),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };

            rightPanel.Controls.Add(_logBox);
            rightPanel.Controls.Add(logHeaderPanel);

            // BOTTOM STATUS STRIP
            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(28, 31, 44),
                ForeColor = Color.FromArgb(189, 195, 199),
                SizingGrip = false,
                Padding = new Padding(8, 2, 8, 2)
            };

            _lblStatusInfo = new ToolStripStatusLabel
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Status: Siap | v3.2.0 Enterprise | Administrator Mode",
                ForeColor = Color.FromArgb(189, 195, 199),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            _progressBar = new ToolStripProgressBar
            {
                Width = 140,
                Height = 16,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false,
                Margin = new Padding(4, 3, 4, 3)
            };

            var lblDeveloperLink = new ToolStripStatusLabel
            {
                Text = "🌐 AuraCore: https://www.auracore.my.id",
                IsLink = true,
                LinkColor = Color.FromArgb(52, 152, 219),
                ActiveLinkColor = Color.FromArgb(46, 204, 113),
                VisitedLinkColor = Color.FromArgb(52, 152, 219),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Alignment = ToolStripItemAlignment.Right
            };
            lblDeveloperLink.Click += (s, e) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://www.auracore.my.id",
                        UseShellExecute = true
                    });
                }
                catch { }
            };

            _statusStrip.Items.Add(_lblStatusInfo);
            _statusStrip.Items.Add(_progressBar);
            _statusStrip.Items.Add(lblDeveloperLink);

            splitContainer.Panel1.Controls.Add(leftPanel);
            splitContainer.Panel2.Controls.Add(rightPanel);

            Controls.Add(splitContainer);
            Controls.Add(_statusStrip);
        }

        private void LoadCategoryButtons()
        {
            _categoryFilterPanel.Controls.Clear();
            _categoryButtons.Clear();

            foreach (var cat in ToolCategory.GetAllCategories())
            {
                var btn = new Button
                {
                    Text = cat,
                    AutoSize = true,
                    Height = 30,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    Margin = new Padding(3, 0, 3, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) =>
                {
                    _selectedCategory = cat;
                    UpdateCategoryButtonStyles();
                    RefreshCards();
                };

                _categoryButtons[cat] = btn;
                _categoryFilterPanel.Controls.Add(btn);
            }

            UpdateCategoryButtonStyles();
        }

        private void UpdateCategoryButtonStyles()
        {
            foreach (var kvp in _categoryButtons)
            {
                bool isSelected = kvp.Key.Equals(_selectedCategory, StringComparison.OrdinalIgnoreCase);
                kvp.Value.BackColor = isSelected ? Color.FromArgb(52, 152, 219) : Color.FromArgb(40, 46, 62);
                kvp.Value.ForeColor = isSelected ? Color.White : Color.FromArgb(189, 195, 199);
                kvp.Value.Font = new Font("Segoe UI", 8.5F, isSelected ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void RefreshCards()
        {
            _cardsContainer.SuspendLayout();
            _cardsContainer.Controls.Clear();
            _activeCards.Clear();

            string search = _searchBox.Text;
            var filteredTools = ToolRegistry.Filter(_selectedCategory, search).ToList();

            foreach (var tool in filteredTools)
            {
                var card = new ActionCardControl(tool, async (c, t) => await ExecuteToolAsync(c, t));
                _activeCards.Add(card);
                _cardsContainer.Controls.Add(card);
            }

            _lblStatusInfo.ForeColor = Color.FromArgb(189, 195, 199);
            _lblStatusInfo.Text = $"Status: Siap | Menampilkan {filteredTools.Count} modul IT ({_selectedCategory})";
            _statusStrip.Refresh();
            _cardsContainer.ResumeLayout();
        }

        private async Task ExecuteToolAsync(ActionCardControl card, IToolCommand tool)
        {
            card.SetBusy(true);
            _progressBar.Visible = true;
            _lblStatusInfo.ForeColor = Color.FromArgb(52, 152, 219);
            _lblStatusInfo.Text = $"Status: Menjalankan '{tool.Title}'...";
            _statusStrip.Refresh();

            var sw = Stopwatch.StartNew();
            string status = "Success";
            string summary = "Operasi selesai dengan sukses";

            try
            {
                await tool.ExecuteAsync();
            }
            catch (Exception ex)
            {
                status = "Error";
                summary = ex.Message;
                Logger.Log($"[FATAL ERROR] Gagal menjalankan {tool.Title}: {ex.Message}", LogType.Error);
            }
            finally
            {
                sw.Stop();
                SessionHistoryTracker.Record(tool.Id, tool.Title, tool.Category, sw.Elapsed.TotalSeconds, status, summary);
                card.SetBusy(false);
                _progressBar.Visible = false;
                _lblStatusInfo.ForeColor = (status == "Error") ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);
                _lblStatusInfo.Text = $"Status: Siap | '{tool.Title}' selesai dalam {sw.Elapsed.TotalSeconds:F1}s";
                _statusStrip.Refresh();
            }
        }

        public async Task ExecuteToolDirectlyAsync(IToolCommand tool)
        {
            _progressBar.Visible = true;
            _lblStatusInfo.ForeColor = Color.FromArgb(52, 152, 219);
            _lblStatusInfo.Text = $"Status: Menjalankan '{tool.Title}'...";
            _statusStrip.Refresh();

            var sw = Stopwatch.StartNew();
            string status = "Success";
            string summary = "Operasi selesai dengan sukses";

            try
            {
                await tool.ExecuteAsync();
            }
            catch (Exception ex)
            {
                status = "Error";
                summary = ex.Message;
                Logger.Log($"[FATAL ERROR] Gagal menjalankan {tool.Title}: {ex.Message}", LogType.Error);
            }
            finally
            {
                sw.Stop();
                SessionHistoryTracker.Record(tool.Id, tool.Title, tool.Category, sw.Elapsed.TotalSeconds, status, summary);
                _progressBar.Visible = false;
                _lblStatusInfo.ForeColor = (status == "Error") ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);
                _lblStatusInfo.Text = $"Status: Siap | '{tool.Title}' selesai dalam {sw.Elapsed.TotalSeconds:F1}s";
                _statusStrip.Refresh();
            }
        }

        public void JumpAndHighlightTool(string toolId)
        {
            var tool = ToolRegistry.GetAllTools().FirstOrDefault(t => t.Id.Equals(toolId, StringComparison.OrdinalIgnoreCase));
            if (tool != null)
            {
                _selectedCategory = ToolCategory.All;
                UpdateCategoryButtonStyles();
                _searchBox.Text = tool.Title;
                RefreshCards();
            }
        }

        private void LogWelcomeInfo()
        {
            Logger.Log("=======================================================================", LogType.Info);
            Logger.Log("🚀 IT SUPPORT CENTER 2026 (v3.2.0 Enterprise Edition) - READY", LogType.Success);
            Logger.Log("🌐 Dikembangkan & Didukung oleh AuraCore: https://www.auracore.my.id", LogType.Info);
            Logger.Log("=======================================================================", LogType.Info);

            var osInfo = OsDetector.GetOsInfo();
            string hostName = Environment.MachineName;
            string userName = Environment.UserName;
            string localIp = GetLocalIpAddress();

            Logger.Log($"• Hostname: {hostName}  |  User: {userName}  |  Local IP: {localIp}");
            Logger.Log($"• Terdeteksi: {osInfo.OsName} ({osInfo.Architecture}, Build {osInfo.BuildNumber})", LogType.Success);
            Logger.Log($"• Hardware: RAM {osInfo.TotalRamGb:F1} GB | Drive C: {osInfo.DriveCFreeGb:F1} GB Bebas ({osInfo.DriveCType})", LogType.Info);
            Logger.Log("• Pilih modul dari kartu di sebelah kiri atau gunakan kotak pencarian.", LogType.Info);
            Logger.Log("⚖️ DISCLAIMER: Perangkat lunak ini disediakan 'AS-IS'. Segala tindakan eksekusi, backup data, dan kepatuhan sistem adalah tanggung jawab penuh pengguna.", LogType.Warning);
            Logger.Log("-----------------------------------------------------------------------", LogType.Info);
        }

        private string GetLocalIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                        return ip.ToString();
                }
            }
            catch { }
            return "127.0.0.1";
        }
    }
}
