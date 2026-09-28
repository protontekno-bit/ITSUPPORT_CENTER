using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class DiagnoseSmbTargetTool : IToolCommand
    {
        public string Id => "diagnose_smb_target";
        public string Title => "Diagnosa Server SMB / Port 445";
        public string Description => "Uji Ping, cek status TCP Port 445 sharing, dan jalankan net view ke IP server tujuan.";
        public string Category => ToolCategory.Network;
        public string Keywords => "diagnosa ping test port 445 smb cek koneksi server";
        public string Icon => "🔍";
        public string ButtonText => "Diagnosa Server";
        public Color ButtonColor => Color.FromArgb(155, 89, 182);

        public async Task ExecuteAsync()
        {
            string targetIp = PromptInput("Masukkan IP Server Target:", "192.168.1.100");
            if (string.IsNullOrWhiteSpace(targetIp)) return;

            Logger.Log($"=== MEMULAI DIAGNOSA SERVER SMB: {targetIp} ===");
            await Task.Run(async () =>
            {
                // 1. Ping
                Logger.Log($"[1/3] Menguji Ping ke {targetIp}...");
                try
                {
                    using var ping = new Ping();
                    var reply = ping.Send(targetIp, 2000);
                    if (reply.Status == IPStatus.Success)
                    {
                        Logger.Log($"[PING OK] Reply dari {targetIp}: Waktu={reply.RoundtripTime}ms", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"[PING GAGAL] Status: {reply.Status}", LogType.Error);
                        Logger.Log("Periksa kabel LAN, WiFi, atau IP server tujuan.", LogType.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"[PING ERROR] {ex.Message}", LogType.Error);
                }

                // 2. Test Port 445
                Logger.Log($"[2/3] Menguji Port TCP 445 (SMB File Sharing)...");
                try
                {
                    using var tcp = new TcpClient();
                    var connectTask = tcp.ConnectAsync(targetIp, 445);
                    if (await Task.WhenAny(connectTask, Task.Delay(2000)) == connectTask && tcp.Connected)
                    {
                        Logger.Log($"[PORT 445 TERBUKA] Layanan File Sharing di {targetIp} AKTIF!", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"[PORT 445 TERTUTUP] Tidak bisa membuka koneksi ke port 445 di {targetIp}.", LogType.Error);
                        Logger.Log("Periksa Windows Firewall di server tujuan atau pastikan file sharing aktif.", LogType.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"[PORT ERROR] {ex.Message}", LogType.Error);
                }

                // 3. Net View
                Logger.Log($"[3/3] Menjalankan perintah 'net view \\\\{targetIp}'...");
                await CommandRunner.RunCmdAsync($"net view \\\\{targetIp}", s => Logger.Log(s, LogType.Info));
                Logger.Log($"Diagnosa ke {targetIp} selesai.");
            });
        }

        private static string PromptInput(string prompt, string defaultValue)
        {
            using var form = new Form
            {
                Width = 420,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Input Parameter",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(24, 28, 38),
                ForeColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Left = 20, Top = 20, Text = prompt, AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            var txt = new TextBox { Left = 20, Top = 50, Width = 360, Text = defaultValue, Font = new Font("Segoe UI", 10f), BackColor = Color.FromArgb(35, 41, 55), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var btnOk = new Button { Text = "OK", Left = 210, Width = 80, Top = 90, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Batal", Left = 300, Width = 80, Top = 90, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : "";
        }
    }
}
