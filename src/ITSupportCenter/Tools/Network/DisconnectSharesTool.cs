using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class DisconnectSharesTool : IToolCommand
    {
        public string Id => "disconnect_shares";
        public string Title => "Putus Semua Mapped Drive";
        public string Description => "Menghapus semua koneksi drive jaringan (net use * /delete /y) dan membersihkan tiket login.";
        public string Category => ToolCategory.Network;
        public string Keywords => "disconnect putus mapped drive net use delete shares";
        public string Icon => "🔌";
        public string ButtonText => "Disconnect Drive";
        public Color ButtonColor => Color.FromArgb(231, 76, 60);

        public async Task ExecuteAsync()
        {
            Logger.Log("Memutus semua koneksi folder sharing dan mapped drive...");
            await Task.Run(async () =>
            {
                await CommandRunner.RunCmdAsync("net use * /delete /y", s => Logger.Log(s, LogType.Info));
                await CommandRunner.RunCmdAsync("klist purge", s => Logger.Log(s, LogType.Info));
                Logger.Log("Semua mapped drive dan tiket autentikasi telah diputus.", LogType.Success);
            });
        }
    }
}
