using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class ToggleIcmpPingResponseTool : IToolCommand
    {
        public string Id => "sec_toggle_icmp_ping";
        public string Title => "Izinkan / Blokir Respon Ping LAN (ICMPv4)";
        public string Description => "Saklar 1-Klik untuk mengizinkan (Allow) atau memblokir respon Ping (ICMPv4 Echo Request) agar PC dapat di-ping di jaringan LAN kantor.";
        public string Category => ToolCategory.Security;
        public string Keywords => "icmp ping echo request lan firewall allow block netsh test connection";
        public string Icon => "📡";
        public string ButtonText => "Izinkan / Blokir Ping LAN";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATUR STATUS RESPON PING LAN (ICMPv4 ECHO REQUEST) ===", LogType.Info);

            await Task.Run(async () =>
            {
                string ruleName = "IT_ALLOW_ICMPV4_PING";
                int checkCode = await CommandRunner.RunCmdAsync($"netsh advfirewall firewall show rule name=\"{ruleName}\"", null);

                if (checkCode == 0)
                {
                    // Rule exists -> Delete it to Block
                    Logger.Log("Status saat ini: Aturan Izin Ping ditemukan (AKTIF).", LogType.Info);
                    Logger.Log("Menonaktifkan respon Ping (Menghapus aturan firewall)...", LogType.Info);

                    await CommandRunner.RunCmdAsync($"netsh advfirewall firewall delete rule name=\"{ruleName}\"", null);
                    await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule group=\"File and Printer Sharing\" new enable=No", null);

                    Logger.Log("🔒 Respon Ping LAN kini TELAH DIBLOKIR. Komputer tidak akan merespon ping.", LogType.Success);
                }
                else
                {
                    // Rule doesn't exist -> Add it to Allow
                    Logger.Log("Status saat ini: Respon Ping DIBLOKIR / Belum Terbuka.", LogType.Info);
                    Logger.Log("Membuka aturan firewall untuk mengizinkan respon Ping (ICMPv4 Inbound)...", LogType.Info);

                    string addCmd = $"netsh advfirewall firewall add rule name=\"{ruleName}\" protocol=icmpv4:8,any dir=in action=allow description=\"IT Support Center Ping Permit\"";
                    await CommandRunner.RunCmdAsync(addCmd, null);
                    await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule name=\"File and Printer Sharing (Echo Request - ICMPv4-In)\" new enable=Yes", null);

                    Logger.Log("🎉 Respon Ping LAN kini TELAH DIIZINKAN (ALLOW).", LogType.Success);
                    Logger.Log("Komputer sekarang dapat di-ping secara normal oleh teknisi di jaringan lokal.", LogType.Success);
                }
            });
        }
    }
}
