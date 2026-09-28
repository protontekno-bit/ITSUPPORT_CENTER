using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class TcpPortCheckerTool : IToolCommand
    {
        public string Id => "net_tcp_port_checker";
        public string Title => "Penguji Port TCP & Latensi Soket (TCPing / Pengganti Telnet)";
        public string Description => "Menguji keterbukaan port TCP spesifik (Web, Database, RDP, Mail, MikroTik) pada IP/Domain target dengan pengukuran latensi milidetik dan diagnosa firewall.";
        public string Category => ToolCategory.Network;
        public string Keywords => "tcping port check test telnet socket web db rdp latency probe";
        public string Icon => "🔌";
        public string ButtonText => "Uji Port TCP";
        public Color ButtonColor => Color.FromArgb(41, 128, 185);

        public async Task ExecuteAsync()
        {
            var (targetHost, targetPort) = PromptHostAndPort();
            if (string.IsNullOrWhiteSpace(targetHost) || targetPort <= 0)
            {
                Logger.Log("Pengujian port dibatalkan.", LogType.Info);
                return;
            }

            Logger.Log($"=== UJI KONEKTIVITAS SOKET TCP: {targetHost}:{targetPort} ===", LogType.Info);
            Logger.Log($"Memulai 4x probe TCP SYN ke {targetHost}:{targetPort} (Timeout: 2500ms)...", LogType.Info);

            await Task.Run(async () =>
            {
                int successCount = 0;
                long totalRtt = 0;
                long minRtt = long.MaxValue;
                long maxRtt = 0;
                var sb = new StringBuilder();

                sb.AppendLine("=========================================================");
                sb.AppendLine($"🔌 HASIL PENGUJIAN PORT TCP ({targetHost}:{targetPort})");
                sb.AppendLine($"📅 Waktu Uji: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Komputer: {Environment.MachineName}");
                sb.AppendLine("=========================================================");

                for (int probe = 1; probe <= 4; probe++)
                {
                    var sw = Stopwatch.StartNew();
                    using var client = new TcpClient();
                    try
                    {
                        var connectTask = client.ConnectAsync(targetHost, targetPort);
                        var timeoutTask = Task.Delay(2500);

                        var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                        sw.Stop();

                        if (completedTask == connectTask && client.Connected)
                        {
                            long rtt = Math.Max(1, sw.ElapsedMilliseconds);
                            successCount++;
                            totalRtt += rtt;
                            minRtt = Math.Min(minRtt, rtt);
                            maxRtt = Math.Max(maxRtt, rtt);

                            string msg = $"Probe {probe}/4: [TERBUKA / OPEN] Respon dalam {rtt}ms";
                            Logger.Log(msg, LogType.Success);
                            sb.AppendLine($"• {msg}");
                        }
                        else
                        {
                            string msg = $"Probe {probe}/4: [TIMEOUT / FILTERED] Tidak ada respon dalam 2500ms (Diblokir Firewall / RTO)";
                            Logger.Log(msg, LogType.Error);
                            sb.AppendLine($"• {msg}");
                        }
                    }
                    catch (SocketException ex)
                    {
                        sw.Stop();
                        if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                        {
                            string msg = $"Probe {probe}/4: [DITOLAK / REFUSED] Host aktif, tetapi TIDAK ADA service listening di port {targetPort}";
                            Logger.Log(msg, LogType.Warning);
                            sb.AppendLine($"• {msg}");
                        }
                        else
                        {
                            string msg = $"Probe {probe}/4: [GAGAL / ERROR] {ex.SocketErrorCode}: {ex.Message}";
                            Logger.Log(msg, LogType.Error);
                            sb.AppendLine($"• {msg}");
                        }
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        string msg = $"Probe {probe}/4: [ERROR] {ex.Message}";
                        Logger.Log(msg, LogType.Error);
                        sb.AppendLine($"• {msg}");
                    }

                    if (probe < 4) await Task.Delay(500);
                }

                sb.AppendLine("=========================================================");
                int lossPercent = ((4 - successCount) * 100) / 4;
                sb.AppendLine($"📊 Ringkasan: Berhasil: {successCount}/4 ({100 - lossPercent}%), Gagal/Loss: {lossPercent}%");

                if (successCount > 0)
                {
                    long avgRtt = totalRtt / successCount;
                    string stats = $"⚡ Latensi Min: {minRtt}ms | Rata-rata: {avgRtt}ms | Maks: {maxRtt}ms";
                    sb.AppendLine(stats);
                    Logger.Log(stats, LogType.Info);
                    Logger.Log($"✅ Status Port {targetPort}: TERBUKA & DAPAT DIAKSES.", LogType.Success);
                }
                else
                {
                    Logger.Log($"❌ Status Port {targetPort}: TERTUTUP / DIBLOKIR FIREWALL.", LogType.Error);
                }
                sb.AppendLine("=========================================================");

                try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }
                Logger.Log("Hasil pengujian telah disalin otomatis ke Clipboard.", LogType.Info);
            });
        }

        private (string Host, int Port) PromptHostAndPort()
        {
            using var form = new Form
            {
                Width = 460,
                Height = 240,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Uji Port TCP & Soket Jaringan",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(24, 28, 38),
                ForeColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblHost = new Label { Left = 20, Top = 15, Text = "IP Address / Hostname Target:", AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            var txtHost = new TextBox { Left = 20, Top = 40, Width = 400, Text = "192.168.1.1", Font = new Font("Segoe UI", 10f), BackColor = Color.FromArgb(35, 41, 55), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            var lblPort = new Label { Left = 20, Top = 75, Text = "Nomor Port TCP (atau pilih Preset di bawah):", AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            var txtPort = new TextBox { Left = 20, Top = 100, Width = 150, Text = "80", Font = new Font("Segoe UI", 10f), BackColor = Color.FromArgb(35, 41, 55), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            var cmbPreset = new ComboBox
            {
                Left = 180,
                Top = 100,
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.FromArgb(35, 41, 55),
                ForeColor = Color.White
            };

            cmbPreset.Items.AddRange(new object[]
            {
                "Preset: Pilih Layanan Populer...",
                "80 - HTTP (Web)",
                "443 - HTTPS (Web SSL)",
                "3389 - RDP (Remote Desktop)",
                "445 - SMB (File Sharing)",
                "1433 - Microsoft SQL Server",
                "3306 - MySQL / MariaDB",
                "5432 - PostgreSQL",
                "22 - SSH (Secure Shell)",
                "8291 - MikroTik Winbox",
                "8080 - Web Proxy / Alt HTTP",
                "587 - SMTP Mail (Submission)"
            });
            cmbPreset.SelectedIndex = 0;
            cmbPreset.SelectedIndexChanged += (s, e) =>
            {
                if (cmbPreset.SelectedIndex > 0)
                {
                    string selected = cmbPreset.SelectedItem?.ToString() ?? "";
                    string portStr = selected.Split('-')[0].Trim();
                    txtPort.Text = portStr;
                }
            };

            var btnOk = new Button { Text = "Mulai Uji", Left = 240, Width = 90, Top = 145, Height = 32, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Batal", Left = 340, Width = 80, Top = 145, Height = 32, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White };

            form.Controls.Add(lblHost);
            form.Controls.Add(txtHost);
            form.Controls.Add(lblPort);
            form.Controls.Add(txtPort);
            form.Controls.Add(cmbPreset);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog() == DialogResult.OK)
            {
                int.TryParse(txtPort.Text.Trim(), out int port);
                return (txtHost.Text.Trim(), port);
            }

            return ("", 0);
        }
    }
}
