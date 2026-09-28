using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class StartupManagerTool : IToolCommand
    {
        public string Id => "sys_startup_manager";
        public string Title => "Kelola Aplikasi Startup (Autostart)";
        public string Description => "Membuka pengelola aplikasi awal (Startup Manager) untuk menonaktifkan software berat yang memperlambat booting PC.";
        public string Category => ToolCategory.System;
        public string Keywords => "startup boot autostart slow lemot task manager startup apps";
        public string Icon => "🚀";
        public string ButtonText => "Buka Pengelola Startup";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGELOLA APLIKASI STARTUP ===", LogType.Info);

            await Task.Run(() =>
            {
                try
                {
                    Logger.Log("Membuka tab Startup pada Task Manager...");
                    var psi = new ProcessStartInfo
                    {
                        FileName = "taskmgr.exe",
                        Arguments = "/7",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    Logger.Log("✅ Pengelola Startup terbuka. Nonaktifkan aplikasi yang status Startup Impact-nya 'High' jika tidak penting.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal membuka Task Manager: {ex.Message}", LogType.Error);
                }
            });
        }
    }
}
