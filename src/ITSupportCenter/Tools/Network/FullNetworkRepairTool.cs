using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class FullNetworkRepairTool : IToolCommand
    {
        public string Id => "net_full_network_repair";
        public string Title => "Perbaikan Total Jaringan 1-Klik (Full Network Repair)";
        public string Description => "Solusi darurat no internet: Release IP -> Flush DNS & ARP -> Reset Winsock -> Reset TCP/IP -> Reset Proxy -> Renew IP -> Tes Koneksi.";
        public string Category => ToolCategory.Network;
        public string Keywords => "full network repair reset total winsock tcp ip release renew flushdns no internet secured repair darurat";
        public string Icon => "⚡";
        public string ButtonText => "Jalankan Full Repair";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Red Alert

        public async Task ExecuteAsync()
        {
            Logger.Log("======================================================================", LogType.Warning);
            Logger.Log("⚡ MEMULAI SEKUENS PERBAIKAN TOTAL JARINGAN (FULL NETWORK REPAIR)", LogType.Warning);
            Logger.Log("======================================================================", LogType.Warning);

            await Task.Run(async () =>
            {
                // Step 1: Release IP
                Logger.Log("[1/6] Melepas Alamat IP Aktif (ipconfig /release)...", LogType.Info);
                await CommandRunner.RunCmdAsync("ipconfig /release", s => { if (s.Contains("Ethernet") || s.Contains("Wi-Fi")) Logger.Log(s, LogType.Info); });

                // Step 2: Flush DNS & ARP Cache
                Logger.Log("[2/6] Membersihkan Cache DNS & Tabel ARP (flushdns & arpcache)...", LogType.Info);
                await CommandRunner.RunCmdAsync("ipconfig /flushdns", null);
                await CommandRunner.RunCmdAsync("netsh interface ip delete arpcache", null);
                Logger.Log("Cache DNS & ARP dibersihkan.", LogType.Success);

                // Step 3: Reset Winsock
                Logger.Log("[3/6] Mereset Winsock Catalog (netsh winsock reset)...", LogType.Info);
                await CommandRunner.RunCmdAsync("netsh winsock reset", null);
                Logger.Log("Winsock Catalog berhasil di-reset.", LogType.Success);

                // Step 4: Reset TCP/IP Stack
                Logger.Log("[4/6] Mereset Protokol TCP/IP Stack (netsh int ip reset)...", LogType.Info);
                await CommandRunner.RunCmdAsync("netsh int ip reset", null);
                Logger.Log("TCP/IP Stack berhasil di-reset.", LogType.Success);

                // Step 5: Reset WinHTTP & Browser Proxy
                Logger.Log("[5/6] Mereset WinHTTP System Proxy...", LogType.Info);
                await CommandRunner.RunCmdAsync("netsh winhttp reset proxy", null);

                // Step 6: Renew IP from DHCP
                Logger.Log("[6/6] Meminta Alamat IP Baru dari DHCP Router (ipconfig /renew)...", LogType.Info);
                await CommandRunner.RunCmdAsync("ipconfig /renew", s =>
                {
                    if (s.Contains("IPv4") || s.Contains("Subnet") || s.Contains("Default Gateway") || s.Contains("Alamat IPv4"))
                    {
                        Logger.Log(s.Trim(), LogType.Success);
                    }
                });

                // Verification Ping
                Logger.Log("Menguji koneksi akhir ke Google DNS (8.8.8.8)...", LogType.Info);
                try
                {
                    using var ping = new Ping();
                    var reply = await ping.SendPingAsync("8.8.8.8", 2000);
                    if (reply.Status == IPStatus.Success)
                    {
                        Logger.Log($"✅ SELESAI! Internet telah pulih normal (Ping 8.8.8.8: {reply.RoundtripTime}ms).", LogType.Success);
                    }
                    else
                    {
                        Logger.Log("⚠️ Reparasi selesai, namun ping ke 8.8.8.8 belum merespon. Disarankan Restart Komputer.", LogType.Warning);
                    }
                }
                catch
                {
                    Logger.Log("⚠️ Reparasi selesai. Jika masih bermasalah, silakan Restart Komputer Anda.", LogType.Warning);
                }
            });
        }
    }
}
