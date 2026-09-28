using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class ResetNetworkCacheTool : IToolCommand
    {
        public string Id => "reset_network_cache";
        public string Title => "Reset Cache Jaringan & Sesi";
        public string Description => "Flush DNS, refresh NetBIOS (nbtstat -RR), purge tiket sesi (klist), dan restart Workstation.";
        public string Category => ToolCategory.Network;
        public string Keywords => "dns flush netbios reset cache klist purge workstation sessions";
        public string Icon => "🧹";
        public string ButtonText => "Reset Cache";
        public Color ButtonColor => Color.FromArgb(230, 126, 34);

        public async Task ExecuteAsync()
        {
            Logger.Log("Membersihkan cache jaringan, tiket sesi, dan me-refresh NetBIOS...");
            await Task.Run(async () =>
            {
                await CommandRunner.RunCmdAsync("net use * /delete /y", s => Logger.Log(s, LogType.Info));
                await CommandRunner.RunCmdAsync("klist purge", s => Logger.Log(s, LogType.Info));
                await CommandRunner.RunCmdAsync("ipconfig /flushdns", s => Logger.Log(s, LogType.Info));
                await CommandRunner.RunCmdAsync("nbtstat -R", s => Logger.Log(s, LogType.Info));
                await CommandRunner.RunCmdAsync("nbtstat -RR", s => Logger.Log(s, LogType.Info));
                WindowsHelper.RestartService("LanmanWorkstation", "Workstation Service");
                Logger.Log("Seluruh cache jaringan & sesi login berhasil dibersihkan total!", LogType.Success);
            });
        }
    }
}
