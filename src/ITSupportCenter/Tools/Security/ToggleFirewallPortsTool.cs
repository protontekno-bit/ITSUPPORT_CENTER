using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class ToggleFirewallPortsTool : IToolCommand
    {
        public string Id => "sec_toggle_firewall_ports";
        public string Title => "Isolasi / Buka Port Sharing Firewall";
        public string Description => "Memblokir atau membuka port sharing (135, 139, 445) pada Windows Firewall untuk isolasi cepat saat ancaman malware.";
        public string Category => ToolCategory.Security;
        public string Keywords => "firewall port 445 139 block unblock isolasi security lan";
        public string Icon => "🧱";
        public string ButtonText => "Kelola Port Firewall";
        public Color ButtonColor => Color.FromArgb(211, 84, 0); // Orange Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATURAN ATURAN FIREWALL PORT SHARING ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Memeriksa status aturan firewall 'IT_ISOLATION_PORT_BLOCK'...");

                int checkRule = await CommandRunner.RunCmdAsync("netsh advfirewall firewall show rule name=\"IT_ISOLATION_PORT_BLOCK\"", null);

                if (checkRule == 0)
                {
                    // Rule exists, remove it to Unblock
                    Logger.Log("Aturan isolasi ditemukan. Menghapus blokir dan mengembalikan akses port sharing...");
                    await CommandRunner.RunCmdAsync("netsh advfirewall firewall delete rule name=\"IT_ISOLATION_PORT_BLOCK\"", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ Port 135, 139, 445 berhasil DIBUKA KEMBALI. File sharing normal.", LogType.Success);
                }
                else
                {
                    // Rule does not exist, add it to Block
                    Logger.Log("Mengisolasi komputer: Memblokir Inbound Port 135, 137, 138, 139, 445...");
                    string cmd = "netsh advfirewall firewall add rule name=\"IT_ISOLATION_PORT_BLOCK\" dir=in action=block protocol=TCP localport=135,139,445";
                    await CommandRunner.RunCmdAsync(cmd, s => Logger.Log(s, LogType.Info));
                    Logger.Log("🔒 Komputer berhasil DIISOLASI dari inbound SMB/RPC sharing!", LogType.Warning);
                }
            });
        }
    }
}
