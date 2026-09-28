using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class DisableEdgeBackgroundBoostTool : IToolCommand
    {
        public string Id => "sys_disable_edge_background";
        public string Title => "Matikan Edge Background & Startup Boost";
        public string Description => "Menghentikan proses tersembunyi Microsoft Edge agar tidak berjalan di background dan menghemat 200–500 MB RAM saat idle.";
        public string Category => ToolCategory.System;
        public string Keywords => "edge startup boost background apps ram memory hemat performa browser microsoft edge";
        public string Icon => "🚫";
        public string ButtonText => "Matikan Edge Background";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== MENONAKTIFKAN EDGE BACKGROUND & STARTUP BOOST ===", LogType.Info);

            await Task.Run(() =>
            {
                Logger.Log("1. Menerapkan Registry Policy untuk Microsoft Edge...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Edge", "StartupBoostEnabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Edge", "BackgroundModeEnabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Edge", "WebWidgetIsEnabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"SOFTWARE\Policies\Microsoft\Edge", "StartupBoostEnabled", 0);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"SOFTWARE\Policies\Microsoft\Edge", "BackgroundModeEnabled", 0);
                Logger.Log("   ✅ Kebijakan Edge Startup Boost & Background dinonaktifkan.", LogType.Success);

                Logger.Log("2. Menutup proses latar belakang msedge.exe yang sedang berjalan...", LogType.Info);
                WindowsHelper.KillProcessIfExists("msedge");

                Logger.Log("\n🎉 Microsoft Edge kini tidak akan lagi memakan RAM dan CPU secara diam-diam saat tidak digunakan!", LogType.Success);
            });
        }
    }
}
