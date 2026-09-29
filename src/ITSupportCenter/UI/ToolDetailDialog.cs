using System;
using System.Drawing;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.UI
{
    public class ToolDetailDialog : Form
    {
        private readonly IToolCommand _tool;
        private readonly MainForm? _mainForm;
        private readonly Func<IToolCommand, System.Threading.Tasks.Task>? _onExecuteCallback;

        public ToolDetailDialog(IToolCommand tool, MainForm? mainForm = null, Func<IToolCommand, System.Threading.Tasks.Task>? onExecuteCallback = null)
        {
            _tool = tool;
            _mainForm = mainForm;
            _onExecuteCallback = onExecuteCallback;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = $"ℹ️ Panduan & Detail Teknis - {_tool.Title}";
            Size = new Size(720, 620);
            MinimumSize = new Size(600, 520);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(24, 26, 36);
            ForeColor = Color.FromArgb(236, 240, 241);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            var detail = ToolDetailInfoProvider.GetDetail(_tool);

            // 1. Header Panel
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(32, 36, 50),
                Padding = new Padding(16, 12, 16, 12)
            };

            var lblIcon = new Label
            {
                Text = _tool.Icon,
                Font = new Font("Segoe UI Emoji", 22F, FontStyle.Regular),
                Location = new Point(16, 12),
                Size = new Size(48, 48),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitle = new Label
            {
                Text = _tool.Title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(72, 12),
                Size = new Size(610, 26),
                AutoEllipsis = true
            };

            var lblCategory = new Label
            {
                Text = $"Kategori: {_tool.Category}  |  ID: {_tool.Id}",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(74, 40),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblIcon);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblCategory);

            // 2. Bottom Action Bar
            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55,
                BackColor = Color.FromArgb(30, 34, 48),
                Padding = new Padding(14, 10, 14, 10)
            };

            var btnRun = new Button
            {
                Text = $"⚡ Jalankan: {_tool.ButtonText}",
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = _tool.ButtonColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnRun.FlatAppearance.BorderSize = 0;
            btnRun.Click += async (s, e) =>
            {
                if (_mainForm != null && !_mainForm.CanExecuteTool(_tool.Id, out string reason))
                {
                    MessageBox.Show(this, reason, "Operasi Sedang Berjalan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                btnRun.Enabled = false;
                btnRun.Text = "⏳ Memproses...";
                try
                {
                    if (_onExecuteCallback != null)
                    {
                        await _onExecuteCallback(_tool);
                    }
                    else if (_mainForm != null)
                    {
                        await _mainForm.ExecuteToolDirectlyAsync(_tool);
                    }
                    else
                    {
                        await _tool.ExecuteAsync();
                    }
                    MessageBox.Show(this, $"Eksekusi '{_tool.Title}' selesai! Cek hasil di Terminal Log utama.", "Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnRun.Enabled = true;
                    btnRun.Text = $"⚡ Jalankan: {_tool.ButtonText}";
                }
            };

            var btnCopy = new Button
            {
                Text = "📋 Salin Info",
                Dock = DockStyle.Right,
                Width = 110,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.Click += (s, e) =>
            {
                string fullText = $"=== PANDUAN TEKNIS: {_tool.Title} ===\n" +
                                  $"Kategori: {_tool.Category}\n\n" +
                                  $"[1] PROBLEM YANG DISELESAIKAN:\n{detail.ProblemSolved}\n\n" +
                                  $"[2] EFEK & PERUBAHAN SISTEM:\n{detail.SystemImpact}\n\n" +
                                  $"[3] PANDUAN PENGGUNAAN:\n{detail.UsageGuide}\n";
                Clipboard.SetText(fullText);
                MessageBox.Show(this, "Panduan teknis berhasil disalin ke Clipboard!", "Tersalin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var btnClose = new Button
            {
                Text = "Tutup",
                Dock = DockStyle.Right,
                Width = 90,
                BackColor = Color.FromArgb(70, 78, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => Close();

            pnlBottom.Controls.Add(btnRun);
            pnlBottom.Controls.Add(btnCopy);
            pnlBottom.Controls.Add(btnClose);

            // 3. Content Panel (Scrollable 3-Pillar Cards)
            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(20, 22, 30),
                Padding = new Padding(16)
            };

            var rtbBody = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 22, 30),
                ForeColor = Color.FromArgb(236, 240, 241),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                ReadOnly = true
            };

            // Build Rich Formatted Text for the 3 Pillars
            AppendSection(rtbBody, "🎯 1. PROBLEM & GEJALA YANG DISELESAIKAN", Color.FromArgb(52, 152, 219), detail.ProblemSolved);
            AppendSection(rtbBody, "⚙️ 2. CARA KERJA & EFEK TERHADAP SISTEM", Color.FromArgb(230, 126, 34), detail.SystemImpact);
            AppendSection(rtbBody, "💡 3. PANDUAN PENGGUNAAN & LANGKAH MITIGASI", Color.FromArgb(46, 204, 113), detail.UsageGuide);

            pnlContent.Controls.Add(rtbBody);

            Controls.Add(pnlContent);
            Controls.Add(pnlBottom);
            Controls.Add(pnlHeader);
        }

        private static void AppendSection(RichTextBox rtb, string heading, Color headingColor, string bodyText)
        {
            rtb.SelectionFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            rtb.SelectionColor = headingColor;
            rtb.AppendText(heading + "\n");

            rtb.SelectionFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            rtb.SelectionColor = Color.FromArgb(220, 228, 238);
            rtb.AppendText(bodyText + "\n\n");
        }

        public static void Show(IWin32Window parent, IToolCommand tool, MainForm? mainForm = null, Func<IToolCommand, System.Threading.Tasks.Task>? onExecuteCallback = null)
        {
            using var dlg = new ToolDetailDialog(tool, mainForm, onExecuteCallback);
            dlg.ShowDialog(parent);
        }
    }
}
