using System;
using System.Drawing;
using System.Windows.Forms;

namespace ITSupportCenter.UI
{
    public class InputDialog : Form
    {
        private TextBox _txtInput = null!;
        private Button _btnOk = null!;
        private Button _btnCancel = null!;
        private Label _lblPrompt = null!;

        public string Value => _txtInput.Text.Trim();

        private InputDialog(string title, string prompt, string defaultValue, bool isPassword)
        {
            InitializeComponent(title, prompt, defaultValue, isPassword);
        }

        private void InitializeComponent(string title, string prompt, string defaultValue, bool isPassword)
        {
            Text = title;
            Size = new Size(480, 220);
            MinimumSize = new Size(400, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(28, 31, 44);
            ForeColor = Color.FromArgb(236, 240, 241);
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            int promptHeight = Math.Max(45, TextRenderer.MeasureText(prompt, Font, new Size(424, 0), TextFormatFlags.WordBreak).Height + 10);
            _lblPrompt = new Label
            {
                Text = prompt,
                Location = new Point(20, 20),
                Size = new Size(424, promptHeight),
                ForeColor = Color.FromArgb(200, 210, 225)
            };

            int inputY = 20 + promptHeight + 12;
            _txtInput = new TextBox
            {
                Text = defaultValue,
                Location = new Point(20, inputY),
                Size = new Size(424, 28),
                BackColor = Color.FromArgb(45, 52, 70),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = isPassword
            };

            int btnY = inputY + 40;
            _btnOk = new Button
            {
                Text = "Konfirmasi (OK)",
                DialogResult = DialogResult.OK,
                Location = new Point(204, btnY),
                Size = new Size(130, 36),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnOk.FlatAppearance.BorderSize = 0;

            _btnCancel = new Button
            {
                Text = "Batal",
                DialogResult = DialogResult.Cancel,
                Location = new Point(344, btnY),
                Size = new Size(100, 36),
                BackColor = Color.FromArgb(70, 78, 95),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderSize = 0;

            ClientSize = new Size(464, btnY + 54);

            AcceptButton = _btnOk;
            CancelButton = _btnCancel;

            Controls.Add(_lblPrompt);
            Controls.Add(_txtInput);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);
        }

        public static string? Show(IWin32Window? parent = null, string title = "", string prompt = "", string defaultValue = "", bool isPassword = false)
        {
            using var dlg = new InputDialog(title, prompt, defaultValue, isPassword);
            if (dlg.ShowDialog(parent) == DialogResult.OK)
            {
                return dlg.Value;
            }
            return null;
        }
    }
}
