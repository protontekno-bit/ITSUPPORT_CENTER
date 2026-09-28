using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class RestoreClassicContextMenuTool : IToolCommand
    {
        public string Id => "sys_classic_context_menu";
        public string Title => "Menu Klik Kanan Klasik Windows 11";
        public string Description => "Mengembalikan menu klik kanan penuh (klasik Windows 10) pada Windows 11 tanpa tombol 'Show more options'.";
        public string Category => ToolCategory.System;
        public string Keywords => "classic context menu windows 11 right click show more options clsid explorer klik kanan";
        public string Icon => "🖱️";
        public string ButtonText => "Kembalikan Klik Kanan Klasik";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Wisteria Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== KEMBALIKAN MENU KLIK KANAN KLASIK WINDOWS 11 ===", LogType.Info);

            await Task.Run(async () =>
            {
                var osInfo = OsDetector.GetOsInfo();
                Logger.Log($"Deteksi OS Target: {osInfo.OsName} (Build {osInfo.BuildNumber})", LogType.Info);

                if (!osInfo.IsWindows11)
                {
                    Logger.Log("ℹ️ Sistem terdeteksi bukan Windows 11. Menu klik kanan pada Windows 10 / Server sudah dalam format klasik secara default.", LogType.Success);
                    return;
                }

                Logger.Log("Mendaftarkan Registry CLSID override untuk Windows 11 Explorer...", LogType.Info);
                
                string regCmd = "reg add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f /ve";
                int exitCode = await CommandRunner.RunCmdAsync(regCmd, s => Logger.Log(s, LogType.Info));

                if (exitCode == 0)
                {
                    Logger.Log("✅ Registry CLSID berhasil dikonfigurasi.", LogType.Success);
                    Logger.Log("Me-restart Windows Explorer untuk menerapkan perubahan...", LogType.Info);
                    
                    WindowsHelper.KillProcessIfExists("explorer");
                    await Task.Delay(1000);
                    CommandRunner.RunCmdAsync("start explorer.exe", null).Wait(3000);
                    
                    Logger.Log("🎉 Selesai! Menu klik kanan klasik Windows 10 sekarang aktif di Windows 11.", LogType.Success);
                }
                else
                {
                    Logger.Log($"⚠️ Gagal menyetel registry (Kode keluar: {exitCode}).", LogType.Warning);
                }
            });
        }
    }
}
