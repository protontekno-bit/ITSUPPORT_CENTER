using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class SubnetIpScannerTool : IToolCommand
    {
        public string Id => "net_subnet_ip_scanner";
        public string Title => "Pindai Seluruh IP Aktif di LAN (Subnet IP Sweeper)";
        public string Description => "Memindai seluruh rentang IP (1-254) di subnet kantor secara paralel untuk menemukan PC aktif, printer jaringan, router, dan MAC address.";
        public string Category => ToolCategory.Scanner;
        public string Keywords => "subnet ip sweep scanner lan arp find printer router range live devices";
        public string Icon => "📡";
        public string ButtonText => "Pindai Subnet LAN";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMINDAIAN SUBSET IP JARINGAN LOKAL (PING SWEEP) ===", LogType.Info);

            string defaultSubnet = DetectLocalSubnetPrefix();
            string subnetPrefix = PromptForSubnet(defaultSubnet);

            if (string.IsNullOrWhiteSpace(subnetPrefix))
            {
                Logger.Log("Pemindaian dibatalkan.", LogType.Info);
                return;
            }

            Logger.Log($"Memulai pemindaian 254 host pada subnet: {subnetPrefix}.1 s/d {subnetPrefix}.254...", LogType.Info);

            var discoveredDevices = new ConcurrentBag<(string IP, string Hostname, long Rtt)>();
            var tasks = new List<Task>();
            int scannedCount = 0;

            for (int i = 1; i <= 254; i++)
            {
                string targetIp = $"{subnetPrefix}.{i}";
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using var ping = new Ping();
                        var reply = await ping.SendPingAsync(targetIp, 800);
                        if (reply.Status == IPStatus.Success)
                        {
                            string hostName = "Unknown Host";
                            try
                            {
                                var hostEntry = await Dns.GetHostEntryAsync(targetIp);
                                hostName = hostEntry.HostName;
                            }
                            catch { }

                            discoveredDevices.Add((targetIp, hostName, reply.RoundtripTime));
                            Logger.Log($"[LIVE] {targetIp,-15} ({reply.RoundtripTime}ms) -> {hostName}", LogType.Success);
                        }
                    }
                    catch { }
                    finally
                    {
                        Interlocked.Increment(ref scannedCount);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            var sortedList = discoveredDevices.OrderBy(d =>
            {
                var parts = d.IP.Split('.');
                return int.Parse(parts[3]);
            }).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("=========================================================");
            sb.AppendLine($"📡 LAPORAN PERANGKAT AKTIF DI SUBSET {subnetPrefix}.0/24");
            sb.AppendLine($"📅 Tanggal: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Total Aktif: {sortedList.Count} Host");
            sb.AppendLine("=========================================================");

            foreach (var item in sortedList)
            {
                string line = $"• IP: {item.IP,-15} | Ping: {item.Rtt,3}ms | Hostname: {item.Hostname}";
                sb.AppendLine(line);
            }

            sb.AppendLine("=========================================================");

            Logger.Log($"✅ Pemindaian selesai! Total {sortedList.Count} perangkat aktif terdeteksi.", LogType.Success);
            try
            {
                ThreadClipboardHelper.SetClipboardText(sb.ToString());
                Logger.Log("📋 Daftar IP & Hostname aktif berhasil disalin ke Clipboard!", LogType.Success);
            }
            catch { }
        }

        private string DetectLocalSubnetPrefix()
        {
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        foreach (var ip in nic.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                            {
                                string ipStr = ip.Address.ToString();
                                int lastDot = ipStr.LastIndexOf('.');
                                if (lastDot > 0)
                                    return ipStr.Substring(0, lastDot);
                            }
                        }
                    }
                }
            }
            catch { }
            return "192.168.1";
        }

        private string PromptForSubnet(string defaultSubnet)
        {
            string subnet = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pindai Subnet IP Jaringan Lokal (LAN)";
                form.Width = 380;
                form.Height = 160;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Masukkan 3 blok Subnet Prefix (contoh: 192.168.1):", Left = 20, Top = 15, Width = 330 };
                var textBox = new TextBox { Left = 20, Top = 40, Width = 320, Text = defaultSubnet };
                var buttonOk = new Button { Text = "Mulai Pindai", Left = 150, Width = 100, Top = 75, DialogResult = DialogResult.OK };
                var buttonCancel = new Button { Text = "Batal", Left = 260, Width = 80, Top = 75, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    subnet = textBox.Text.Trim().TrimEnd('.');
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return subnet;
        }
    }
}
