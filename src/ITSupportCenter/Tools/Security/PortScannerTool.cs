using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class PortScannerTool : IToolCommand
    {
        public string Id => "sec_port_scanner";
        public string Title => "Pemindai Port Jaringan (TCP Scanner)";
        public string Description => "Memindai port kritis (SMB 445, RDP 3389, HTTP 80, Printer 9100) pada IP target untuk diagnostik konektivitas.";
        public string Category => ToolCategory.Scanner;
        public string Keywords => "port scanner scan tcp 445 3389 80 9100 ip network audit probe";
        public string Icon => "📡";
        public string ButtonText => "Pindai Port Target";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT KONEKSI & PEMINDAI PORT TCP ===", LogType.Info);

            string targetHost = PromptForTargetHost();
            if (string.IsNullOrWhiteSpace(targetHost))
            {
                Logger.Log("Pemindaian port dibatalkan.", LogType.Info);
                return;
            }

            var portsToCheck = new Dictionary<int, string>
            {
                { 21, "FTP File Transfer" },
                { 22, "SSH Secure Shell" },
                { 23, "Telnet" },
                { 80, "HTTP Web Service" },
                { 135, "RPC Endpoint Mapper" },
                { 139, "NetBIOS Session Service" },
                { 443, "HTTPS Secure Web" },
                { 445, "SMB Direct File Sharing" },
                { 3389, "RDP Remote Desktop" },
                { 5900, "VNC Remote Display" },
                { 8080, "HTTP Alternate Proxy" },
                { 9100, "RAW Network Printer (JetDirect)" }
            };

            Logger.Log($"Memulai pemindaian pada target: {targetHost}...", LogType.Info);

            int openCount = 0;
            var tasks = new List<Task>();

            foreach (var kvp in portsToCheck)
            {
                int port = kvp.Key;
                string desc = kvp.Value;

                tasks.Add(Task.Run(async () =>
                {
                    bool isOpen = await CheckPortAsync(targetHost, port, 1500);
                    if (isOpen)
                    {
                        Interlocked.Increment(ref openCount);
                        Logger.Log($"[OPEN]  Port {port,-5} ({desc}) -> TERBUKA / AKTIF", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"[CLOSE] Port {port,-5} ({desc}) -> Tertutup / Filtered", LogType.Info);
                    }
                }));
            }

            await Task.WhenAll(tasks);
            Logger.Log($"✅ Selesai memindai {targetHost}. Total {openCount} port terbuka terdeteksi.", LogType.Success);
        }

        private async Task<bool> CheckPortAsync(string host, int port, int timeoutMs)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var delayTask = Task.Delay(timeoutMs);

                var completedTask = await Task.WhenAny(connectTask, delayTask);
                return completedTask == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }

        private string PromptForTargetHost()
        {
            string host = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Masukkan IP Address / Host Target";
                form.Width = 360;
                form.Height = 150;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "IP / Hostname (contoh: 192.168.1.1 atau 127.0.0.1):", Left = 20, Top = 15, Width = 310 };
                var textBox = new TextBox { Left = 20, Top = 40, Width = 300, Text = "127.0.0.1" };
                var buttonOk = new Button { Text = "Mulai Pindai", Left = 130, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                var buttonCancel = new Button { Text = "Batal", Left = 240, Width = 80, Top = 70, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    host = textBox.Text.Trim();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return host;
        }
    }
}
