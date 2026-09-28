using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class FixSmbGuestAuthTool : IToolCommand
    {
        public string Id => "fix_smb_guest_auth";
        public string Title => "Fix SMB Guest Error 0x800704f8";
        public string Description => "Aktifkan AllowInsecureGuestAuth di Policies & Services agar bisa buka share tanpa password.";
        public string Category => ToolCategory.Network;
        public string Keywords => "smb guest share folder sharing 0x800704f8 insecure auth";
        public string Icon => "📁";
        public string ButtonText => "Fix Guest Auth";
        public Color ButtonColor => Color.FromArgb(52, 152, 219);

        public async Task ExecuteAsync()
        {
            Logger.Log("Memulai perbaikan SMB Error 0x800704f8 (Insecure Guest Logon)...");
            await Task.Run(() =>
            {
                try
                {
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\LanmanWorkstation", "AllowInsecureGuestAuth", 1);
                    Logger.Log(@"Registry Policies\LanmanWorkstation -> AllowInsecureGuestAuth = 1", LogType.Success);

                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters", "AllowInsecureGuestAuth", 1);
                    Logger.Log(@"Registry Services\LanmanWorkstation\Parameters -> AllowInsecureGuestAuth = 1", LogType.Success);

                    WindowsHelper.RestartService("LanmanWorkstation", "Workstation (SMB Client)");
                    Logger.Log("Akses folder sharing tanpa password (Guest) telah diizinkan.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log("Gagal mengubah konfigurasi Guest Auth: " + ex.Message, LogType.Error);
                }
            });
        }
    }
}
