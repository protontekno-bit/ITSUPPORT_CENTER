using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class RestartExplorerShellTool : IToolCommand
    {
        public string Id => "sys_restart_explorer";
        public string Title => "Restart Windows Explorer & Taskbar";
        public string Description => "Memulai ulang proses explorer.exe secara instan untuk mengatasi taskbar macet/hang, desktop freeze, atau icon tidak muncul.";
        public string Category => ToolCategory.System;
        public string Keywords => "explorer taskbar freeze hang restart desktop shell macet icon";
        public string Icon => "🔄";
        public string ButtonText => "Restart Windows Explorer";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESTART WINDOWS EXPLORER & TASKBAR SHELL ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Menghentikan seluruh proses explorer.exe...");
                WindowsHelper.KillProcessIfExists("explorer");

                await Task.Delay(1500);

                Logger.Log("Menjalankan kembali shell Windows Explorer...");
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        UseShellExecute = true
                    });
                    Logger.Log("✅ Windows Explorer & Taskbar berhasil di-restart dan dimuat ulang!", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal memulai explorer.exe: {ex.Message}. Mencoba via CMD fallback...", LogType.Warning);
                    await CommandRunner.RunCmdAsync("start explorer.exe", null);
                }
            });
        }
    }
}
