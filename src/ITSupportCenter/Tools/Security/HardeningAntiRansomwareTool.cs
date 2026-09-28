using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class HardeningAntiRansomwareTool : IToolCommand
    {
        public string Id => "sec_hardening_antiransomware";
        public string Title => "Hardening Keamanan & Anti-Ransomware";
        public string Description => "Menonaktifkan protokol berisiko tinggi: SMBv1 purba, AutoRun/AutoPlay USB, Remote Registry, dan WScript execute.";
        public string Category => ToolCategory.Security;
        public string Keywords => "security hardening ransomware smbv1 autorun autoplay usb malware exploit";
        public string Icon => "🛡️";
        public string ButtonText => "Terapkan Hardening";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Alizarin / Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== HARDENING SISTEM & PROTEKSI ANTI-RANSOMWARE ===", LogType.Warning);

            await Task.Run(async () =>
            {
                // 1. Disable SMBv1
                Logger.Log("1/4: Menonaktifkan SMBv1 (Protokol rentan exploit EternalBlue/WannaCry)...");
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "SMB1", 0);
                await CommandRunner.RunCmdAsync("powershell -Command \"Set-SmbServerConfiguration -EnableSMB1Protocol $false -Force\" 2>$null", null);
                Logger.Log("SMBv1 dinonaktifkan.", LogType.Success);

                // 2. Disable AutoRun & AutoPlay USB
                Logger.Log("2/4: Menonaktifkan fitur AutoPlay & AutoRun pada Flashdisk/USB...");
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 255);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 255);
                Logger.Log("AutoPlay USB dinonaktifkan.", LogType.Success);

                // 3. Disable Remote Registry
                Logger.Log("3/4: Mematikan service Remote Registry...");
                WindowsHelper.StopServiceIfExists("RemoteRegistry");
                await CommandRunner.RunCmdAsync("sc config RemoteRegistry start= disabled", null);
                Logger.Log("Remote Registry dinonaktifkan.", LogType.Success);

                // 4. Disable Auto-execution of WScript (.vbs malware scripts)
                Logger.Log("4/4: Membatasi eksekusi Windows Script Host (WSH)...");
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Microsoft\Windows Script Host\Settings", "Enabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows Script Host\Settings", "Enabled", 0);
                Logger.Log("Windows Script Host dinonaktifkan.", LogType.Success);

                Logger.Log("✅ Hardening Keamanan Selesai! Komputer Anda kini jauh lebih terlindungi dari serangan lateral jaringan & USB malware.", LogType.Success);
            });
        }
    }
}
