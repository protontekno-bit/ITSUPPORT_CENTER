using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class SafeModeAndBcdRepairTool : IToolCommand
    {
        public string Id => "sys_safe_mode_bcd_repair";
        public string Title => "Pengatur Safe Mode & Perbaikan Parameter Booting (BCD / MSConfig)";
        public string Description => "1-Klik atur reboot masuk Safe Mode (Minimal / Jaringan), normalkan kembali ke Boot Normal, reset limit core CPU/RAM, dan aktifkan menu F8 legacy.";
        public string Category => ToolCategory.System;
        public string Keywords => "safe mode safeboot bcdedit msconfig boot legacy f8 numproc truncatememory repair";
        public string Icon => "🛡️";
        public string ButtonText => "Kelola Safe Mode & Boot";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATUR SAFE MODE & PERBAIKAN PARAMETER BOOTING (BCD) ===", LogType.Info);

            string choice = PromptForBootOption();
            if (string.IsNullOrEmpty(choice)) return;

            await Task.Run(async () =>
            {
                if (choice == "SAFE_NET")
                {
                    Logger.Log("Mengatur Windows untuk reboot masuk ke Safe Mode with Networking...");
                    await CommandRunner.RunCmdAsync("bcdedit /set {current} safeboot network", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ Komputer disetel masuk SAFE MODE DENGAN JARINGAN pada restart berikutnya.", LogType.Success);
                }
                else if (choice == "SAFE_MIN")
                {
                    Logger.Log("Mengatur Windows untuk reboot masuk ke Safe Mode Minimal...");
                    await CommandRunner.RunCmdAsync("bcdedit /set {current} safeboot minimal", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ Komputer disetel masuk SAFE MODE MINIMAL pada restart berikutnya.", LogType.Success);
                }
                else if (choice == "NORMAL_BOOT")
                {
                    Logger.Log("Mengembalikan Windows ke mode Booting Normal...");
                    await CommandRunner.RunCmdAsync("bcdedit /deletevalue {current} safeboot", null);
                    await CommandRunner.RunCmdAsync("bcdedit /deletevalue {default} safeboot", null);
                    Logger.Log("✅ Parameter Safe Mode dihapus. Komputer akan boot NORMAL pada restart berikutnya.", LogType.Success);
                }
                else if (choice == "RESET_LIMITS")
                {
                    Logger.Log("Mereset limit Core Prosesor & Memory Limit BCD (Perbaikan salah setting msconfig)...");
                    await CommandRunner.RunCmdAsync("bcdedit /deletevalue {current} numproc", null);
                    await CommandRunner.RunCmdAsync("bcdedit /deletevalue {current} truncatememory", null);
                    await CommandRunner.RunCmdAsync("bcdedit /deletevalue {current} removememory", null);
                    Logger.Log("✅ Seluruh limit CPU & RAM pada BCD berhasil dinormalkan!", LogType.Success);
                }
                else if (choice == "ENABLE_F8")
                {
                    Logger.Log("Mengaktifkan menu boot F8 Klasik (Legacy Boot Menu Policy)...");
                    await CommandRunner.RunCmdAsync("bcdedit /set {default} bootmenupolicy legacy", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ Menu tombol F8 saat booting berhasil diaktifkan!", LogType.Success);
                }
                else if (choice == "OPEN_MSCONFIG")
                {
                    Logger.Log("Membuka jendela System Configuration (msconfig.exe)...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = "msconfig.exe", UseShellExecute = true });
                        Logger.Log("✅ msconfig.exe terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal membuka msconfig: {ex.Message}", LogType.Error);
                    }
                }
            });
        }

        private string PromptForBootOption()
        {
            string selection = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pengatur Safe Mode & Parameter Booting";
                form.Width = 460;
                form.Height = 330;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih konfigurasi booting yang diinginkan:", Left = 20, Top = 12, Width = 400 };

                var btnSafeNet = new Button { Text = "🌐 Setel Masuk Safe Mode dengan Jaringan (Networking)", Left = 20, Top = 38, Width = 400, Height = 34 };
                btnSafeNet.Click += (s, e) => { selection = "SAFE_NET"; form.Close(); };

                var btnSafeMin = new Button { Text = "🛡️ Setel Masuk Safe Mode Minimal (Standar)", Left = 20, Top = 78, Width = 400, Height = 34 };
                btnSafeMin.Click += (s, e) => { selection = "SAFE_MIN"; form.Close(); };

                var btnNormal = new Button { Text = "🔄 Kembalikan ke Booting Normal (Hapus Safe Mode)", Left = 20, Top = 118, Width = 400, Height = 34 };
                btnNormal.Click += (s, e) => { selection = "NORMAL_BOOT"; form.Close(); };

                var btnResetLim = new Button { Text = "⚡ Reset Limit CPU Core & RAM (Perbaikan Salah Setting)", Left = 20, Top = 158, Width = 400, Height = 34 };
                btnResetLim.Click += (s, e) => { selection = "RESET_LIMITS"; form.Close(); };

                var btnF8 = new Button { Text = "⌨️ Aktifkan Tombol F8 Boot Menu Klasik", Left = 20, Top = 198, Width = 400, Height = 34 };
                btnF8.Click += (s, e) => { selection = "ENABLE_F8"; form.Close(); };

                var btnMsconfig = new Button { Text = "⚙️ Buka GUI System Configuration (msconfig.exe)", Left = 20, Top = 238, Width = 400, Height = 34 };
                btnMsconfig.Click += (s, e) => { selection = "OPEN_MSCONFIG"; form.Close(); };

                form.Controls.AddRange(new Control[] { label, btnSafeNet, btnSafeMin, btnNormal, btnResetLim, btnF8, btnMsconfig });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return selection;
        }
    }
}
