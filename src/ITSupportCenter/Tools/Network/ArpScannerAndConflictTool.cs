using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class ArpScannerAndConflictTool : IToolCommand
    {
        public string Id => "net_arp_mac_sniffer";
        public string Title => "Pemindai ARP Layer-2 & Deteksi Konflik IP Jaringan";
        public string Description => "Pindai seluruh host aktif di LAN menggunakan protokol ARP (anti-blokir firewall ICMP), deteksi duplikasi IP/MAC (IP Conflict), dan identifikasi vendor perangkat.";
        public string Category => ToolCategory.Scanner;
        public string Keywords => "arp mac sniffer sweep layer2 ip conflict rogue dhcp vendor lan scanner";
        public string Icon => "📡";
        public string ButtonText => "Pindai ARP & Konflik IP";
        public Color ButtonColor => Color.FromArgb(142, 68, 173);

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int destIp, int srcIp, byte[] macAddr, ref int macAddrLen);

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMINDAIAN ARP LAYER-2 & DETEKSI KONFLIK IP LAN ===", LogType.Info);

            string defaultSubnet = DetectLocalSubnetPrefix();
            string subnetPrefix = PromptForSubnet(defaultSubnet);

            if (string.IsNullOrWhiteSpace(subnetPrefix))
            {
                Logger.Log("Pemindaian ARP dibatalkan.", LogType.Info);
                return;
            }

            Logger.Log($"Memulai pemindaian ARP 254 host ({subnetPrefix}.1 s/d {subnetPrefix}.254)...", LogType.Info);
            Logger.Log("Keunggulan ARP: Menemukan PC/Printer meskipun Windows Firewall memblokir Ping ICMP.", LogType.Info);

            await Task.Run(async () =>
            {
                var discovered = new ConcurrentBag<(string IP, string MAC, string Vendor, string Hostname)>();
                var tasks = new List<Task>();
                int scannedCount = 0;

                for (int i = 1; i <= 254; i++)
                {
                    string targetIp = $"{subnetPrefix}.{i}";
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            if (IPAddress.TryParse(targetIp, out var ipAddress))
                            {
                                byte[] macBytes = new byte[6];
                                int macLen = macBytes.Length;
                                int ipInt = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);

                                int result = SendARP(ipInt, 0, macBytes, ref macLen);
                                if (result == 0 && macLen == 6)
                                {
                                    string macStr = string.Join(":", macBytes.Select(b => b.ToString("X2")));
                                    string vendor = IdentifyVendor(macStr);
                                    string hostName = "Unknown Host";

                                    try
                                    {
                                        var hostEntry = Dns.GetHostEntry(targetIp);
                                        hostName = hostEntry.HostName;
                                    }
                                    catch { }

                                    discovered.Add((targetIp, macStr, vendor, hostName));
                                    Logger.Log($"[ARP LIVE] {targetIp,-15} | MAC: {macStr} | {vendor} -> {hostName}", LogType.Success);
                                }
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

                var sortedList = discovered.OrderBy(d =>
                {
                    var parts = d.IP.Split('.');
                    return int.Parse(parts[3]);
                }).ToList();

                // Analisa Konflik IP & Duplikasi MAC
                var macGroups = sortedList.GroupBy(x => x.MAC).Where(g => g.Count() > 1).ToList();

                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine($"📡 LAPORAN PEMINDAIAN ARP & AUDIT IP/MAC (SUBSET {subnetPrefix}.0/24)");
                sb.AppendLine($"📅 Waktu: {DateTime.Now:yyyy-MM-dd HH:mm:ss} | Host Aktif: {sortedList.Count} Unit");
                sb.AppendLine("=========================================================");

                foreach (var item in sortedList)
                {
                    sb.AppendLine($"• IP: {item.IP,-15} | MAC: {item.MAC} | Vendor: {item.Vendor,-12} | Host: {item.Hostname}");
                }

                sb.AppendLine("=========================================================");
                sb.AppendLine("🔍 STATUS AUDIT KONFLIK IP & KEAMANAN:");

                if (macGroups.Count > 0)
                {
                    Logger.Log("⚠️ PERINGATAN: Terdeteksi duplikasi alamat MAC pada IP berbeda!", LogType.Warning);
                    sb.AppendLine("⚠️ PERINGATAN: DITEMUKAN DUPLIKASI MAC ADDRESS (Indikasi Multi-IP / ARP Spoofing):");
                    foreach (var g in macGroups)
                    {
                        string ips = string.Join(", ", g.Select(x => x.IP));
                        string warn = $"   -> MAC [{g.Key}] ({g.First().Vendor}) dipakai oleh {g.Count()} IP: {ips}";
                        sb.AppendLine(warn);
                        Logger.Log(warn, LogType.Warning);
                    }
                }
                else
                {
                    sb.AppendLine("✅ Bebas Konflik: Tidak ditemukan duplikasi IP / MAC mencurigakan pada subnet ini.");
                    Logger.Log("✅ Bebas Konflik: Seluruh host memiliki pemetaan IP & MAC yang unik.", LogType.Success);
                }

                sb.AppendLine("=========================================================");
                try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }
                Logger.Log("Laporan pemindaian ARP telah disalin ke Clipboard.", LogType.Info);
            });
        }

        private static string DetectLocalSubnetPrefix()
        {
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus != OperationalStatus.Up || ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        continue;

                    var ipProps = ni.GetIPProperties();
                    foreach (var unicast in ipProps.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicast.Address))
                        {
                            string ipStr = unicast.Address.ToString();
                            if (ipStr.StartsWith("192.168.") || ipStr.StartsWith("10.") || ipStr.StartsWith("172."))
                            {
                                int lastDot = ipStr.LastIndexOf('.');
                                if (lastDot > 0) return ipStr.Substring(0, lastDot);
                            }
                        }
                    }
                }
            }
            catch { }
            return "192.168.1";
        }

        private static string PromptForSubnet(string defaultSubnet)
        {
            using var form = new Form
            {
                Width = 420,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Konfirmasi Subnet LAN",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(24, 28, 38),
                ForeColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Left = 20, Top = 20, Text = "Masukkan Awalan Subnet IP (Contoh: 192.168.1):", AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            var txt = new TextBox { Left = 20, Top = 50, Width = 360, Text = defaultSubnet, Font = new Font("Segoe UI", 10f), BackColor = Color.FromArgb(35, 41, 55), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            var btnOk = new Button { Text = "Mulai Pindai", Left = 200, Width = 95, Top = 90, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(142, 68, 173), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Batal", Left = 305, Width = 75, Top = 90, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White };

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : "";
        }

        private static string IdentifyVendor(string mac)
        {
            if (string.IsNullOrEmpty(mac) || mac.Length < 8) return "Generic Device";

            string prefix = mac.Substring(0, 8).ToUpper();

            // Peta OUI umum di lingkungan perkantoran & jaringan
            if (prefix.StartsWith("00:50:56") || prefix.StartsWith("00:0C:29") || prefix.StartsWith("00:05:69")) return "VMware";
            if (prefix.StartsWith("00:15:5D")) return "Microsoft Hyper-V";
            if (prefix.StartsWith("08:00:27") || prefix.StartsWith("0A:00:27")) return "VirtualBox";
            if (prefix.StartsWith("B8:27:EB") || prefix.StartsWith("DC:A6:32") || prefix.StartsWith("E4:5F:01")) return "Raspberry Pi";
            if (prefix.StartsWith("00:1A:11") || prefix.StartsWith("00:1E:58") || prefix.StartsWith("D4:CA:6D") || prefix.StartsWith("CC:2D:E0")) return "MikroTik";
            if (prefix.StartsWith("00:15:6D") || prefix.StartsWith("04:18:D6") || prefix.StartsWith("24:A4:3C") || prefix.StartsWith("F0:9F:C2")) return "Ubiquiti UniFi";
            if (prefix.StartsWith("00:11:22") || prefix.StartsWith("00:18:BA") || prefix.StartsWith("00:21:55") || prefix.StartsWith("C0:7C:D1")) return "Cisco";
            if (prefix.StartsWith("00:04:76") || prefix.StartsWith("00:08:74") || prefix.StartsWith("F4:EC:38") || prefix.StartsWith("50:C7:BF")) return "TP-Link";
            if (prefix.StartsWith("00:1B:78") || prefix.StartsWith("00:11:D8") || prefix.StartsWith("00:1A:92")) return "HP / Aruba";
            if (prefix.StartsWith("00:14:22") || prefix.StartsWith("00:16:B6") || prefix.StartsWith("18:03:73")) return "Dell";
            if (prefix.StartsWith("00:1F:3B") || prefix.StartsWith("00:23:14") || prefix.StartsWith("A4:4C:C8")) return "Intel";
            if (prefix.StartsWith("00:E0:4C") || prefix.StartsWith("52:54:00") || prefix.StartsWith("00:03:7F")) return "Realtek";
            if (prefix.StartsWith("00:00:85") || prefix.StartsWith("00:00:48") || prefix.StartsWith("AC:D1:B8")) return "Epson Printer";
            if (prefix.StartsWith("00:26:55") || prefix.StartsWith("00:1B:A9")) return "Brother Printer";
            if (prefix.StartsWith("00:11:32") || prefix.StartsWith("00:11:75")) return "Synology NAS";
            if (prefix.StartsWith("00:08:9B") || prefix.StartsWith("24:5E:BE")) return "QNAP NAS";
            if (prefix.StartsWith("44:19:B6") || prefix.StartsWith("BC:54:51") || prefix.StartsWith("E0:50:8B")) return "Hikvision CCTV";
            if (prefix.StartsWith("90:02:A9") || prefix.StartsWith("4C:11:BF")) return "Dahua CCTV";
            if (prefix.StartsWith("3C:22:FB") || prefix.StartsWith("F8:4D:89") || prefix.StartsWith("A4:83:E7")) return "Apple";
            if (prefix.StartsWith("00:16:6C") || prefix.StartsWith("50:01:D9") || prefix.StartsWith("94:E9:79")) return "Samsung";

            return "Network Device";
        }
    }
}
