using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class SwitchWindowsEditionTool : IToolCommand
    {
        public string Id => "license_switch_windows_edition";
        public string Title => "Upgrade / Ganti Edisi Windows (Home ke Pro Tanpa Format)";
        public string Description => "Memicu upgrade paket edisi Windows resmi (Home ke Pro / Enterprise) menggunakan generic upgrade key via changepk.exe tanpa install ulang.";
        public string Category => ToolCategory.License;
        public string Keywords => "upgrade edisi home to pro enterprise changepk switch edisi lisensi generic gvlk";
        public string Icon => "📈";
        public string ButtonText => "Upgrade Edisi Windows";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== UPGRADE & GANTI EDISI WINDOWS (CHANGEPK) ===", LogType.Info);

            string genericKey = PromptForEditionKey();
            if (string.IsNullOrWhiteSpace(genericKey))
            {
                Logger.Log("Proses ganti edisi dibatalkan.", LogType.Info);
                return;
            }

            Logger.Log($"Memulai proses upgrade edisi dengan Product Key: {genericKey}...", LogType.Info);
            Logger.Log("Windows akan memverifikasi paket edisi dan mengaktifkan fitur Pro/Enterprise. Mohon tunggu...", LogType.Warning);

            await Task.Run(async () =>
            {
                int exitCode = await CommandRunner.RunCmdAsync($"changepk.exe /ProductKey {genericKey}", s => Logger.Log(s, LogType.Info));
                if (exitCode == 0)
                {
                    Logger.Log("✅ Proses upgrade edisi Windows selesai! Komputer mungkin perlu di-restart untuk memuat seluruh fitur edisi baru.", LogType.Success);
                }
                else
                {
                    Logger.Log($"Perintah changepk selesai dengan kode: {exitCode}. Silakan cek status di Settings > System > Activation.", LogType.Info);
                }
            });
        }

        private string PromptForEditionKey()
        {
            string key = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pilih Target Upgrade Edisi Windows";
                form.Width = 440;
                form.Height = 220;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih target upgrade edisi Windows yang diinginkan:", Left = 20, Top = 12, Width = 390 };

                var btnPro = new Button { Text = "⭐ Upgrade ke Windows 10/11 Pro (VK7JG-...-3V66T)", Left = 20, Top = 40, Width = 380, Height = 34 };
                btnPro.Click += (s, e) => { key = "VK7JG-NPHTM-C97JM-9MPGT-3V66T"; form.Close(); };

                var btnEnterprise = new Button { Text = "🏢 Upgrade ke Windows 10/11 Enterprise (NPPR9-...-2YT43)", Left = 20, Top = 82, Width = 380, Height = 34 };
                btnEnterprise.Click += (s, e) => { key = "NPPR9-FWDCX-D2C8J-H872K-2YT43"; form.Close(); };

                var btnCustom = new Button { Text = "✏️ Masukkan Product Key Kustom...", Left = 20, Top = 124, Width = 380, Height = 34 };
                btnCustom.Click += (s, e) =>
                {
                    key = "CUSTOM";
                    form.Close();
                };

                form.Controls.AddRange(new Control[] { label, btnPro, btnEnterprise, btnCustom });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (key == "CUSTOM")
            {
                key = PromptForCustomKey();
            }

            return key;
        }

        private string PromptForCustomKey()
        {
            string customKey = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Masukkan 25-Digit Product Key";
                form.Width = 380;
                form.Height = 150;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;

                var label = new Label { Text = "Product Key (XXXXX-XXXXX-XXXXX-XXXXX-XXXXX):", Left = 20, Top = 15, Width = 320 };
                var textBox = new TextBox { Left = 20, Top = 40, Width = 320 };
                var buttonOk = new Button { Text = "Terapkan", Left = 150, Width = 90, Top = 72, DialogResult = DialogResult.OK };
                var buttonCancel = new Button { Text = "Batal", Left = 250, Width = 90, Top = 72, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    customKey = textBox.Text.Trim();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return customKey;
        }
    }
}
