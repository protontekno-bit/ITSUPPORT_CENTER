using System;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class NetworkQualityTesterTool : IToolCommand
    {
        public string Id => "net_quality_jitter_tester";
        public string Title => "Diagnosa Kualitas Koneksi 3-Titik (Gateway vs ISP vs Internet)";
        public string Description => "Uji stabilitas & packet loss simultan ke Router Lokal, DNS ISP, dan Internet Global (8.8.8.8) untuk melacak titik putus koneksi.";
        public string Category => ToolCategory.Network;
        public string Keywords => "ping jitter latency packet loss kualitas gateway internet rtt putus lemot";
        public string Icon => "🩺";
        public string ButtonText => "Uji Stabilitas Koneksi";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== DIAGNOSA KUALITAS KONEKSI 3-TITIK (PACKET LOSS & LATENCY) ===", LogType.Info);

            await Task.Run(async () =>
            {
                string? gatewayIp = GetDefaultGatewayIp();
                string? dnsIp = GetDnsServerIp();
                string internetIp = "8.8.8.8";

                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("📊 HASIL DIAGNOSA STABILITAS & KUALITAS JARINGAN 3-TITIK");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                // Hop 1: Router / Default Gateway
                if (!string.IsNullOrEmpty(gatewayIp))
                {
                    Logger.Log($"[HOP 1/3] Menguji koneksi ke Router Lokal / Gateway ({gatewayIp})...", LogType.Info);
                    var (sent, received, avgRtt) = await RunPingBurstAsync(gatewayIp, 5);
                    double lossPct = ((sent - received) / (double)sent) * 100.0;
                    string res = $"• Hop 1 (Router Gateway {gatewayIp}): Loss {lossPct:F0}% ({received}/{sent} paket), Rata-rata RTT: {avgRtt}ms";
                    sb.AppendLine(res);

                    if (lossPct == 0 && avgRtt < 10)
                        Logger.Log($"[HOP 1 OK] Router Gateway merespon sempurna ({avgRtt}ms). Kabel LAN/WiFi lokal stabil.", LogType.Success);
                    else if (lossPct > 0)
                        Logger.Log($"[HOP 1 WARN] Terdeteksi Packet Loss {lossPct:F0}% ke Router lokal! Masalah pada Kabel LAN atau Sinyal WiFi.", LogType.Error);
                    else
                        Logger.Log($"[HOP 1 INFO] Router Gateway merespon dengan latensi agak tinggi ({avgRtt}ms).", LogType.Warning);
                }
                else
                {
                    Logger.Log("[HOP 1 ERROR] Tidak dapat menemukan Default Gateway! Komputer belum terhubung ke jaringan.", LogType.Error);
                    sb.AppendLine("• Hop 1 (Router Gateway): TIDAK TERDETEKSI");
                }

                // Hop 2: DNS Server
                if (!string.IsNullOrEmpty(dnsIp))
                {
                    Logger.Log($"[HOP 2/3] Menguji koneksi ke DNS Server ({dnsIp})...", LogType.Info);
                    var (sent, received, avgRtt) = await RunPingBurstAsync(dnsIp, 5);
                    double lossPct = ((sent - received) / (double)sent) * 100.0;
                    string res = $"• Hop 2 (DNS Server {dnsIp}): Loss {lossPct:F0}% ({received}/{sent} paket), Rata-rata RTT: {avgRtt}ms";
                    sb.AppendLine(res);

                    if (lossPct == 0)
                        Logger.Log($"[HOP 2 OK] DNS Server merespon baik ({avgRtt}ms).", LogType.Success);
                    else
                        Logger.Log($"[HOP 2 WARN] Terdeteksi Loss {lossPct:F0}% ke DNS Server.", LogType.Warning);
                }

                // Hop 3: Global Internet (Google 8.8.8.8)
                Logger.Log($"[HOP 3/3] Menguji koneksi ke Internet Global (Google DNS {internetIp})...", LogType.Info);
                var (sent3, received3, avgRtt3) = await RunPingBurstAsync(internetIp, 5);
                double lossPct3 = ((sent3 - received3) / (double)sent3) * 100.0;
                string res3 = $"• Hop 3 (Internet Global {internetIp}): Loss {lossPct3:F0}% ({received3}/{sent3} paket), Rata-rata RTT: {avgRtt3}ms";
                sb.AppendLine(res3);

                if (lossPct3 == 0)
                    Logger.Log($"[HOP 3 OK] Internet Global aktif stabil (Rata-rata: {avgRtt3}ms).", LogType.Success);
                else if (lossPct3 == 100)
                    Logger.Log("[HOP 3 ERROR] Tidak ada koneksi ke Internet Luar (100% Loss). Layanan ISP mati atau kuota habis.", LogType.Error);
                else
                    Logger.Log($"[HOP 3 WARN] Internet mengalami jitter/packet loss {lossPct3:F0}%.", LogType.Warning);

                sb.AppendLine("=========================================================");
                try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }
                Logger.Log("✅ Uji stabilitas selesai. Hasil disalin ke Clipboard.", LogType.Success);
            });
        }

        private async Task<(int Sent, int Received, long AvgRtt)> RunPingBurstAsync(string host, int count)
        {
            int sent = 0;
            int received = 0;
            long totalRtt = 0;

            using var ping = new Ping();
            for (int i = 0; i < count; i++)
            {
                sent++;
                try
                {
                    var reply = await ping.SendPingAsync(host, 1200);
                    if (reply.Status == IPStatus.Success)
                    {
                        received++;
                        totalRtt += reply.RoundtripTime;
                    }
                }
                catch { }
                await Task.Delay(200);
            }

            long avgRtt = received > 0 ? totalRtt / received : 0;
            return (sent, received, avgRtt);
        }

        private string? GetDefaultGatewayIp()
        {
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        var gw = nic.GetIPProperties().GatewayAddresses;
                        foreach (var g in gw)
                        {
                            if (g.Address.AddressFamily == AddressFamily.InterNetwork)
                                return g.Address.ToString();
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        private string? GetDnsServerIp()
        {
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up &&
                        (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        var dns = nic.GetIPProperties().DnsAddresses;
                        foreach (var d in dns)
                        {
                            if (d.AddressFamily == AddressFamily.InterNetwork)
                                return d.ToString();
                        }
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
