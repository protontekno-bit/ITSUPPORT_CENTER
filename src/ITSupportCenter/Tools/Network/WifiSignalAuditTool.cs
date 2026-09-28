using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class WifiSignalAuditTool : IToolCommand
    {
        public string Id => "net_wifi_signal_audit";
        public string Title => "Audit Kualitas Sinyal Wi-Fi & Detail Adapter";
        public string Description => "Menganalisis kekuatan sinyal (RSSI %), Radio Type (Wi-Fi 5/6), BSSID Access Point, Channel frekuensi, dan kecepatan link Mbps.";
        public string Category => ToolCategory.Network;
        public string Keywords => "wifi signal wlan bssid channel rssi kualitas sinyal drop putus lemot speed";
        public string Icon => "📶";
        public string ButtonText => "Audit Sinyal Wi-Fi";
        public Color ButtonColor => Color.FromArgb(22, 160, 133); // Green Sea

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT KUALITAS SINYAL WI-FI & DETAIL ADAPTER ===", LogType.Info);

            await Task.Run(async () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("📶 LAPORAN STATUS & KUALITAS KONEKSI WI-FI");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                bool hasWlan = false;

                await CommandRunner.RunCmdAsync("netsh wlan show interfaces", s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        hasWlan = true;
                        sb.AppendLine(s);

                        if (s.Contains("SSID") && !s.Contains("BSSID"))
                            Logger.Log(s.Trim(), LogType.Success);
                        else if (s.Contains("Signal") || s.Contains("Sinyal"))
                        {
                            Logger.Log(s.Trim(), LogType.Success);
                        }
                        else if (s.Contains("Radio type") || s.Contains("Channel") || s.Contains("Receive rate") || s.Contains("Transmit rate"))
                        {
                            Logger.Log(s.Trim(), LogType.Info);
                        }
                        else if (s.Contains("State") && s.Contains("disconnected"))
                        {
                            Logger.Log(s.Trim(), LogType.Warning);
                        }
                    }
                });

                if (!hasWlan)
                {
                    Logger.Log("Tidak ditemukan antarmuka Wireless LAN (Wi-Fi) aktif pada komputer ini (PC Desktop kabel LAN).", LogType.Warning);
                    return;
                }

                sb.AppendLine("=========================================================");
                try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }
                Logger.Log("✅ Data audit sinyal Wi-Fi berhasil disalin ke Clipboard!", LogType.Success);
            });
        }
    }
}
