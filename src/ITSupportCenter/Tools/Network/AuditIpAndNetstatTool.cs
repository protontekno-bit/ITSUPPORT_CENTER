using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class AuditIpAndNetstatTool : IToolCommand
    {
        public string Id => "net_audit_ip_netstat";
        public string Title => "Audit Konfigurasi IP Lengkap & Netstat (Koneksi Aktif)";
        public string Description => "Menampilkan konfigurasi jaringan detail (ipconfig /all) dan memindai port/koneksi aktif aplikasi di background (netstat -ano).";
        public string Category => ToolCategory.Network;
        public string Keywords => "ipconfig all netstat socket connections port process mac dhcp lease ipv6";
        public string Icon => "📋";
        public string ButtonText => "Cek IP & Netstat";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT KONFIGURASI IP LENGKAP & NETSTAT ===", LogType.Info);

            await Task.Run(async () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("📋 LAPORAN KONFIGURASI IP & KONEKSI AKTIF NETSTAT");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                // 1. IPCONFIG /ALL
                Logger.Log("1/2: Membaca Konfigurasi Adapter Lengkap (ipconfig /all)...", LogType.Info);
                sb.AppendLine("\n[--- 1. DETAIL IPCONFIG /ALL ---]");
                await CommandRunner.RunCmdAsync("ipconfig /all", s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        sb.AppendLine(s);
                        if (s.Contains("IPv4") || s.Contains("Physical Address") || s.Contains("Default Gateway") || s.Contains("DHCP Server") || s.Contains("DNS Servers"))
                        {
                            Logger.Log(s.Trim(), LogType.Success);
                        }
                    }
                });

                // 2. NETSTAT STATS & ACTIVE CONNECTIONS
                Logger.Log("2/2: Membaca Statistik Lalu Lintas & Koneksi Aktif (netstat -e & -ano)...", LogType.Info);
                sb.AppendLine("\n[--- 2. STATISTIK TRAFIK INTERFACE ---]");
                await CommandRunner.RunCmdAsync("netstat -e", s => { if (!string.IsNullOrWhiteSpace(s)) { sb.AppendLine(s); Logger.Log(s.Trim(), LogType.Info); } });

                sb.AppendLine("\n[--- 3. KONEKSI SOCKET AKTIF (ESTABLISHED / LISTENING) ---]");
                await CommandRunner.RunCmdAsync("netstat -ano", s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        sb.AppendLine(s);
                        if (s.Contains("ESTABLISHED") || s.Contains("445") || s.Contains("3389") || s.Contains("8080"))
                        {
                            Logger.Log(s.Trim(), LogType.Info);
                        }
                    }
                });

                sb.AppendLine("=========================================================");

                try
                {
                    ThreadClipboardHelper.SetClipboardText(sb.ToString());
                    Logger.Log("✅ Data IPConfig & Netstat berhasil disalin ke Clipboard!", LogType.Success);
                }
                catch { }
            });
        }
    }
}
