using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class OptimizeVisualEffectsTool : IToolCommand
    {
        public string Id => "sys_optimize_visual_effects";
        public string Title => "Optimasi Efek Visual & Kecepatan UI";
        public string Description => "Menonaktifkan animasi lambat Windows & bayangan pointer dengan tetap mempertahankan ketajaman teks (ClearType) untuk performa responsif.";
        public string Category => ToolCategory.System;
        public string Keywords => "visual effects animations performance responsiveness speed sysdm cpl cleartype font smooth";
        public string Icon => "🏎️";
        public string ButtonText => "Optimasi Efek Visual";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Wisteria Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== OPTIMASI EFEK VISUAL & KECEPATAN ANTARMUKA WINDOWS ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Menyetel konfigurasi efek visual kantor (High Performance + Sharp Fonts)...", LogType.Info);

                // Matikan animasi jendela dan tooltip
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Control Panel\Desktop\WindowMetrics", "MinAnimate", 0);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 3);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Control Panel\Desktop", "UserPreferencesMask", 0);

                // Tetap aktifkan Font Smoothing (ClearType) agar teks tajam dan tidak pecah
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Control Panel\Desktop", "FontSmoothing", 2);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Control Panel\Desktop", "FontSmoothingType", 2);

                // Tetap aktifkan bayangan nama icon di desktop agar mudah dibaca
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow", 1);

                Logger.Log("✅ Efek visual telah dioptimalkan untuk responsivitas maksimal.", LogType.Success);
                Logger.Log("Me-restart Windows Explorer untuk menerapkan perubahan...", LogType.Info);

                WindowsHelper.KillProcessIfExists("explorer");
                await Task.Delay(1000);
                CommandRunner.RunCmdAsync("start explorer.exe", null).Wait(3000);

                Logger.Log("🎉 Selesai! Antarmuka Windows sekarang jauh lebih gesit dan tanpa lag animasi.", LogType.Success);
            });
        }
    }
}
