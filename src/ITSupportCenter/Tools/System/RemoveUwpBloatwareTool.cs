using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class RemoveUwpBloatwareTool : IToolCommand
    {
        public string Id => "sys_remove_uwp_bloatware";
        public string Title => "Pembersih Bloatware UWP Windows Kantor";
        public string Description => "Menghapus aplikasi bawaan non-kantor (Xbox, Clipchamp, TikTok, Disney+, Solitaire, Feedback Hub, Zune Video, Bing News) secara aman.";
        public string Category => ToolCategory.System;
        public string Keywords => "bloatware uwp remove uninstall xbox clipchamp solitaire tiktok disney spotify feedback hub zune cleaner";
        public string Icon => "🗑️";
        public string ButtonText => "Bersihkan Bloatware Windows";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Pomegranate Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMBERSIHAN BLOATWARE APLIKASI UWP KANTOR ===", LogType.Info);

            await Task.Run(async () =>
            {
                var bloatList = new (string Pattern, string DisplayName)[]
                {
                    ("*Xbox*", "Xbox Game Bar & Gaming Services"),
                    ("*GamingApp*", "Xbox App"),
                    ("*Clipchamp*", "Clipchamp Video Editor"),
                    ("*MicrosoftSolitaireCollection*", "Microsoft Solitaire Collection"),
                    ("*BingNews*", "Microsoft News / MSN"),
                    ("*BingWeather*", "MSN Weather"),
                    ("*ZuneMusic*", "Groove Music / Media Player Legacy"),
                    ("*ZuneVideo*", "Movies & TV"),
                    ("*WindowsFeedbackHub*", "Feedback Hub"),
                    ("*GetHelp*", "Get Help App"),
                    ("*MicrosoftFamily*", "Microsoft Family Safety"),
                    ("*Spotify*", "Spotify Music (Stub)"),
                    ("*Disney*", "Disney+ (Stub)"),
                    ("*TikTok*", "TikTok (Stub)")
                };

                Logger.Log("Menghapus paket aplikasi bawaan non-produktif (Calculator, Notepad, Snipping Tool tetap aman)...", LogType.Info);

                int removedCount = 0;
                foreach (var item in bloatList)
                {
                    Logger.Log($"Menghapus paket: {item.DisplayName}...", LogType.Info);
                    string psCmd = $"Get-AppxPackage -AllUsers -Name '{item.Pattern}' | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue; " +
                                  $"Get-AppxProvisionedPackage -Online | Where-Object {{ $_.PackageName -like '{item.Pattern}' }} | Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue";

                    await CommandRunner.RunPowerShellAsync(psCmd, null);
                    removedCount++;
                }

                Logger.Log($"\n✅ Pembersihan selesai! {removedCount} kategori paket bloatware telah diproses dan dihapus.", LogType.Success);
            });
        }
    }
}
