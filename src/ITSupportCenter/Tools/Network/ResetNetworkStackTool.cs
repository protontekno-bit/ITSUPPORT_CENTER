using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class ResetNetworkStackTool : IToolCommand
    {
        public string Id => "reset_network_stack";
        public string Title => "Reset Total Network Stack & Proxy";
        public string Description => "Reset WinSock, TCP/IP, rilis WinHTTP proxy (pasca VPN), flush DNS & renew DHCP.";
        public string Category => ToolCategory.Network;
        public string Keywords => "network internet reset winsock proxy winhttp tcp ip no internet secured";
        public string Icon => "🌐";
        public string ButtonText => "Reset Jaringan";
        public Color ButtonColor => Color.FromArgb(41, 128, 185);

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESET TOTAL STACK JARINGAN, WINSOCK & WINHTTP PROXY ===");
            await Task.Run(async () =>
            {
                Logger.Log("[1/5] Reset WinSock Catalog...");
                await CommandRunner.RunCmdAsync("netsh winsock reset", s => Logger.Log(s, LogType.Info));

                Logger.Log("[2/5] Reset TCP/IP Stack...");
                await CommandRunner.RunCmdAsync("netsh int ip reset", s => Logger.Log(s, LogType.Info));

                Logger.Log("[3/5] Reset WinHTTP Proxy (Mengatasi proxy nyangkut pasca VPN)...");
                await CommandRunner.RunCmdAsync("netsh winhttp reset proxy", s => Logger.Log(s, LogType.Success));

                Logger.Log("[4/5] Flush DNS Cache & Rilis NetBIOS...");
                await CommandRunner.RunCmdAsync("ipconfig /flushdns", null);
                await CommandRunner.RunCmdAsync("nbtstat -R", null);
                await CommandRunner.RunCmdAsync("nbtstat -RR", null);

                Logger.Log("[5/5] Meminta pembaruan IP dari DHCP (ipconfig /renew)...");
                await CommandRunner.RunCmdAsync("ipconfig /renew", s => Logger.Log(s, LogType.Info));

                Logger.Log("RESET JARINGAN SELESAI! Silakan coba akses kembali koneksi internet / intranet kantor.", LogType.Success);
            });
        }
    }
}
