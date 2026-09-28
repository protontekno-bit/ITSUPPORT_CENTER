using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class RestoreAllSharingTool : IToolCommand
    {
        public string Id => "sec_restore_sharing_normal";
        public string Title => "Normalisasi Total File & Printer Sharing";
        public string Description => "Mengembalikan seluruh konfigurasi network sharing ke standar normal operasional kantor (Insecure Guest Auth, RPC Level 0, Network Discovery ON).";
        public string Category => ToolCategory.Network;
        public string Keywords => "restore sharing normalisasi default smb printer network discovery lan reset";
        public string Icon => "♻️";
        public string ButtonText => "Normalisasi Semua Sharing";
        public Color ButtonColor => Color.FromArgb(46, 204, 113); // Emerald Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== NORMALISASI TOTAL FILE & PRINTER SHARING ===", LogType.Info);

            await Task.Run(async () =>
            {
                // 1. Remove Firewall Block Rules
                Logger.Log("1/5: Membersihkan isolasi firewall jika ada...");
                await CommandRunner.RunCmdAsync("netsh advfirewall firewall delete rule name=\"IT_ISOLATION_PORT_BLOCK\"", null);
                await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule group=\"File and Printer Sharing\" new enable=Yes", null);
                await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule group=\"Network Discovery\" new enable=Yes", null);

                // 2. Enable Guest SMB Auth
                Logger.Log("2/5: Mengaktifkan Insecure Guest Auth untuk SMB...");
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters", "AllowInsecureGuestAuth", 1);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters", "RequireSecuritySignature", 0);

                // 3. Fix Printer RPC Auth
                Logger.Log("3/5: Mengonfigurasi RPC Printer Sharing (0x0000011b fix)...");
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Print", "RpcAuthnLevelPrivacyEnabled", 0);

                // 4. Start Discovery Services
                Logger.Log("4/5: Memastikan service Function Discovery & UPnP berjalan...");
                WindowsHelper.StartServiceIfExists("fdPHost");
                WindowsHelper.StartServiceIfExists("FDResPub");
                WindowsHelper.StartServiceIfExists("SSDPSRV");
                WindowsHelper.StartServiceIfExists("upnphost");

                // 5. Restart Spooler and Workstation
                Logger.Log("5/5: Me-restart Spooler & LanmanWorkstation...");
                WindowsHelper.RestartService("Spooler", "Print Spooler");
                WindowsHelper.RestartService("LanmanWorkstation", "Lanman Workstation");

                Logger.Log("✅ Semua pengaturan sharing telah dinormalisasi untuk operasional kantor!", LogType.Success);
            });
        }
    }
}
