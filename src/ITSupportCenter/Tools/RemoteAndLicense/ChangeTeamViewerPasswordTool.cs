using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class ChangeTeamViewerPasswordTool : IToolCommand
    {
        public string Id => "remote_change_tv_pwd";
        public string Title => "Ubah Password Statis TeamViewer";
        public string Description => "Mengatur password akses jarak jauh tetap (unattended access) TeamViewer secara otomatis melalui command line.";
        public string Category => ToolCategory.Remote;
        public string Keywords => "teamviewer password change unattended remote access static";
        public string Icon => "🔑";
        public string ButtonText => "Atur Password TeamViewer";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATURAN PASSWORD TEAMVIEWER ===", LogType.Info);

            string? tvPath = FindTeamViewerExe();
            if (string.IsNullOrEmpty(tvPath))
            {
                Logger.Log("Aplikasi TeamViewer tidak ditemukan di direktori standar Program Files.", LogType.Warning);
                return;
            }

            // Prompt user for new password
            string newPass = PromptForPassword();
            if (string.IsNullOrWhiteSpace(newPass))
            {
                Logger.Log("Pengubahan password dibatalkan oleh pengguna.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                Logger.Log("Menghentikan TeamViewer...");
                WindowsHelper.StopServiceIfExists("TeamViewer");
                WindowsHelper.KillProcessIfExists("TeamViewer");

                await Task.Delay(1000);

                Logger.Log($"Menerapkan password baru ke TeamViewer...");
                int exitCode = await CommandRunner.RunCmdAsync($"\"{tvPath}\" --passwd {newPass}", s => Logger.Log(s, LogType.Info));

                WindowsHelper.StartServiceIfExists("TeamViewer");
                Logger.Log("✅ Password TeamViewer berhasil diperbarui!", LogType.Success);
            });
        }

        private string? FindTeamViewerExe()
        {
            string[] possiblePaths =
            {
                @"C:\Program Files\TeamViewer\TeamViewer.exe",
                @"C:\Program Files (x86)\TeamViewer\TeamViewer.exe"
            };

            foreach (var p in possiblePaths)
            {
                if (File.Exists(p)) return p;
            }
            return null;
        }

        private string PromptForPassword()
        {
            string pass = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Masukkan Password TeamViewer Baru";
                form.Width = 380;
                form.Height = 160;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Password Baru (Min 6 Karakter):", Left = 20, Top = 15, Width = 320 };
                var textBox = new TextBox { Left = 20, Top = 40, Width = 320, UseSystemPasswordChar = false, Text = "SupportOffice2026!" };
                var buttonOk = new Button { Text = "Simpan", Left = 160, Width = 85, Top = 75, DialogResult = DialogResult.OK };
                var buttonCancel = new Button { Text = "Batal", Left = 255, Width = 85, Top = 75, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    pass = textBox.Text;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return pass;
        }
    }
}
