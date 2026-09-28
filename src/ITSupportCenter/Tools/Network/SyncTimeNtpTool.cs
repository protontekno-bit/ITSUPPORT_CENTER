using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class SyncTimeNtpTool : IToolCommand
    {
        public string Id => "sync_time_ntp";
        public string Title => "Sinkronkan Jam (NTP / Domain)";
        public string Description => "Paksa sinkronisasi jam sistem dengan server waktu (w32tm /resync) untuk fix Kerberos sharing.";
        public string Category => ToolCategory.Network;
        public string Keywords => "jam waktu time ntp domain controller kerberos sync w32tm";
        public string Icon => "🕒";
        public string ButtonText => "Sync Waktu";
        public Color ButtonColor => Color.FromArgb(230, 126, 34);

        public async Task ExecuteAsync()
        {
            Logger.Log("=== MENYINKRONKAN WAKTU SISTEM (NTP / DOMAIN CONTROLLER) ===");
            await Task.Run(async () =>
            {
                Logger.Log("[1/2] Menjalankan w32time service...");
                WindowsHelper.StartServiceIfExists("w32time");
                Logger.Log("[2/2] Memaksa sinkronisasi jam dengan w32tm /resync...");
                await CommandRunner.RunCmdAsync("w32tm /resync /force", s => Logger.Log(s, LogType.Info));
                Logger.Log("Sinkronisasi waktu selesai. Waktu sistem sekarang akurat.", LogType.Success);
            });
        }
    }
}
