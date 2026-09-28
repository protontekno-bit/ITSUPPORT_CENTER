using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class DisableHibernationStorageTool : IToolCommand
    {
        public string Id => "sys_disable_hibernation";
        public string Title => "Bebaskan Kapasitas Disk C: (Disable Hibernation)";
        public string Description => "Menonaktifkan file hiberfil.sys (powercfg -h off) untuk membebaskan ruang disk 8–32 GB di drive C:\\ secara instan.";
        public string Category => ToolCategory.System;
        public string Keywords => "hibernation hiberfil powercfg free space disk c lega storage reclaim ram sleep fast startup";
        public string Icon => "💾";
        public string ButtonText => "Bebaskan Kapasitas Disk C:";
        public Color ButtonColor => Color.FromArgb(39, 174, 96); // Nephritis Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RECLAIM DISK C: (DISABLE HIBERNATION / HIBERFIL.SYS) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // Cek kapasitas disk C: sebelum
                double beforeFreeGb = 0;
                try
                {
                    var drive = new DriveInfo("C");
                    beforeFreeGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    Logger.Log($"Kapasitas free space drive C:\\ saat ini: {beforeFreeGb:F2} GB", LogType.Info);
                }
                catch { }

                Logger.Log("Menjalankan perintah 'powercfg.exe /hibernate off'...", LogType.Info);
                int exitCode = await CommandRunner.RunCmdAsync("powercfg.exe /hibernate off", s => Logger.Log(s, LogType.Info));

                if (exitCode == 0)
                {
                    double afterFreeGb = 0;
                    try
                    {
                        var drive = new DriveInfo("C");
                        afterFreeGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    }
                    catch { }

                    double gainedGb = afterFreeGb - beforeFreeGb;
                    Logger.Log("✅ Hibernasi berhasil dinonaktifkan dan file hiberfil.sys telah dihapus.", LogType.Success);
                    if (gainedGb > 0.1)
                    {
                        Logger.Log($"🎉 Kapasitas disk C:\\ bertambah sebesar: +{gainedGb:F2} GB! (Total sisa sekarang: {afterFreeGb:F2} GB)", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"Kapasitas sisa drive C:\\ sekarang: {afterFreeGb:F2} GB.", LogType.Info);
                    }
                }
                else
                {
                    Logger.Log($"⚠️ Gagal menjalankan powercfg (Kode keluar: {exitCode}). Pastikan aplikasi berjalan sebagai Administrator.", LogType.Warning);
                }
            });
        }
    }
}
