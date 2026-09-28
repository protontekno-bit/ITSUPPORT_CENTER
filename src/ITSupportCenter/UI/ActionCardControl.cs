using System;
using System.Drawing;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.UI
{
    public class ActionCardControl : UserControl
    {
        private readonly IToolCommand _tool;
        private readonly Func<ActionCardControl, IToolCommand, System.Threading.Tasks.Task> _onExecute;
        private readonly Button _btnAction;

        public ActionCardControl(IToolCommand tool, Func<ActionCardControl, IToolCommand, System.Threading.Tasks.Task> onExecute)
        {
            _tool = tool;
            _onExecute = onExecute;

            Width = 280;
            Height = 160;
            Margin = new Padding(8);
            Padding = new Padding(12);
            BackColor = Color.FromArgb(37, 40, 54); // Dark Card Background
            DoubleBuffered = true;

            // Header Panel (Icon + Title)
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                BackColor = Color.Transparent
            };

            var lblIcon = new Label
            {
                Text = _tool.Icon,
                Font = new Font("Segoe UI Emoji", 15F, FontStyle.Regular),
                ForeColor = Color.White,
                AutoSize = false,
                Width = 32,
                Height = 32,
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitle = new Label
            {
                Text = _tool.Title,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(236, 240, 241),
                Location = new Point(36, 2),
                Width = 225,
                Height = 32,
                AutoEllipsis = true
            };

            pnlHeader.Controls.Add(lblIcon);
            pnlHeader.Controls.Add(lblTitle);

            // Description Label
            var lblDesc = new Label
            {
                Text = _tool.Description,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Regular),
                ForeColor = Color.FromArgb(170, 178, 189),
                Location = new Point(12, 48),
                Width = 256,
                Height = 60,
                AutoEllipsis = true,
                BackColor = Color.Transparent
            };

            // Action Button
            _btnAction = new Button
            {
                Text = _tool.ButtonText,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = _tool.ButtonColor,
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                Width = 212,
                Location = new Point(12, 114),
                Cursor = Cursors.Hand
            };
            _btnAction.FlatAppearance.BorderSize = 0;
            _btnAction.Click += async (s, e) =>
            {
                await _onExecute(this, _tool);
            };

            // In-Card Detail / Info Button
            var btnDetail = new Button
            {
                Text = "ℹ️",
                Font = new Font("Segoe UI Emoji", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(241, 196, 15), // Gold Icon
                BackColor = Color.FromArgb(50, 56, 74),
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                Width = 38,
                Location = new Point(230, 114),
                Cursor = Cursors.Hand
            };
            btnDetail.FlatAppearance.BorderSize = 0;

            var tip = new ToolTip();
            tip.SetToolTip(btnDetail, $"Lihat Panduan, Efek Sistem & Masalah yang Diselesaikan ({_tool.Title})");

            btnDetail.Click += (s, e) =>
            {
                ToolDetailDialog.Show((IWin32Window?)this.FindForm() ?? this, _tool, onExecuteCallback: async (t) => await _onExecute(this, t));
            };

            Controls.Add(_btnAction);
            Controls.Add(btnDetail);
            Controls.Add(lblDesc);
            Controls.Add(pnlHeader);

            Paint += OnCardPaint;
            MouseEnter += (s, e) => SetHoverState(true);
            MouseLeave += (s, e) => SetHoverState(false);
            lblDesc.MouseEnter += (s, e) => SetHoverState(true);
            lblDesc.MouseLeave += (s, e) => SetHoverState(false);
        }

        private void SetHoverState(bool isHovered)
        {
            BackColor = isHovered ? Color.FromArgb(45, 52, 70) : Color.FromArgb(37, 40, 54);
            Invalidate();
        }

        private void OnCardPaint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.FromArgb(55, 62, 82), 1);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        public void SetBusy(bool isBusy)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => SetBusy(isBusy)));
                return;
            }
            _btnAction.Enabled = !isBusy;
            if (isBusy)
            {
                _btnAction.Text = "⏳ Memproses...";
            }
            else
            {
                _btnAction.Text = _tool.ButtonText;
            }
        }
    }
}
