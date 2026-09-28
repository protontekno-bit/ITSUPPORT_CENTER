using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class WindowsOfficeDebloaterTool : IToolCommand
    {
        public string Id => "sys_office_debloater";
        public string Title => "Debloater & Optimasi PC Kantor";
        public string Description => "Mematikan telemetri latar belakang (DiagTrack), pencarian Bing di Start Menu, dan GameBar agar PC kantor responsif & ringan.";
        public string Category => ToolCategory.System;
        public string Keywords => "debloat telemetry diagtrack bing search start menu gamebar optimize boost speed kantor privacy";
        public string Icon => "⚡";
        public string ButtonText => "Optimasi & Matikan Telemetri";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Carrot Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== DEBLOATER & OPTIMASI SISTEM PC KANTOR ===", LogType.Info);

            await Task.Run(async () =>
            {
                // 1. Matikan Service Telemetri Windows (DiagTrack & WAP Push)
                Logger.Log("1. Menonaktifkan Service Telemetri latar belakang (DiagTrack)...", LogType.Info);
                await CommandRunner.RunCmdAsync("sc stop DiagTrack & sc config DiagTrack start= disabled", null);
                await CommandRunner.RunCmdAsync("sc stop dmwappushservice & sc config dmwappushservice start= disabled", null);
                Logger.Log("   ✅ Telemetri DiagTrack dinonaktifkan.", LogType.Success);

                // 2. Matikan Telemetry Policy di Registry
                Logger.Log("2. Menerapkan Registry Anti-Telemetri & Data Collection...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "MaxTelemetryAllowed", 0);

                // 3. Matikan Pencarian Bing di Start Menu
                Logger.Log("3. Menonaktifkan integrasi pencarian web Bing di Start Menu...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 1);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows\CurrentVersion\Search", "CortanaConsent", 0);
                Logger.Log("   ✅ Pencarian web Bing di Start Menu dinonaktifkan (Start menu jadi jauh lebih cepat).", LogType.Success);

                // 4. Matikan GameBar & GameDVR untuk Desktop Kantor
                Logger.Log("4. Menonaktifkan Windows GameBar & GameDVR...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"System\GameConfigStore", "GameDVR_Enabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0);
                Logger.Log("   ✅ GameBar dinonaktifkan.", LogType.Success);

                // 5. Matikan Windows Feedback Notification Popups
                Logger.Log("5. Menonaktifkan pop-up survey/feedback Windows...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Siuf\Rules", "NumberOfSIUFInPeriod", 0);

                Logger.Log("\n✅ Optimasi sistem kantor selesai! Windows akan terasa lebih responsif dan hemat CPU/RAM.", LogType.Success);
            });
        }
    }
}
