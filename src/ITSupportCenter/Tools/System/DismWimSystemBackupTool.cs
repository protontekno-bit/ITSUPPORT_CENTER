using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class DismWimSystemBackupTool : IToolCommand
    {
        public string Id => "sys_dism_wim_backup";
        public string Title => "Backup Windows System ke WIM (DISM Capture)";
        public string Description => "Mengambil snapshot penuh partisi C:\\ menjadi file image master biner (.WIM) resmi Microsoft DISM untuk cadangan offline.";
        public string Category => ToolCategory.System;
        public string Keywords => "dism wim capture backup image system master windows offline restore";
        public string Icon => "📦";
        public string ButtonText => "Capture Image Windows (WIM)";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== BACKUP SISTEM WINDOWS KE FILE WIM (MICROSOFT DISM CAPTURE) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // Tentukan lokasi target backup (utamakan drive D:\ atau folder aman)
                string backupDir = @"D:\Backup_Windows_WIM";
                if (!Directory.Exists(@"D:\"))
                {
                    backupDir = @"C:\Backup_Windows_WIM";
                }

                try { Directory.CreateDirectory(backupDir); } catch { }

                string timestamp = DateTime.Now.ToString("yyyyMMdd");
                string wimFile = Path.Combine(backupDir, $"Windows_Master_Backup_{timestamp}.wim");

                Logger.Log($"Lokasi Target Backup: {wimFile}", LogType.Info);
                Logger.Log("Perintah DISM Capture akan mengompresi partisi sistem C:\\ ke format WIM...", LogType.Info);
                Logger.Log("Catatan: Proses memerlukan waktu beberapa menit tergantung kecepatan disk.", LogType.Info);

                string dismCmd = $"dism.exe /Capture-Image /CaptureDir:C:\\ /ImageFile:\"{wimFile}\" /Name:\"WindowsMasterBackup_{timestamp}\" /Description:\"IT Support Center Offline Backup\" /Compress:fast /Verify";

                int exitCode = await CommandRunner.RunCmdAsync(dismCmd, line =>
                {
                    if (!string.IsNullOrWhiteSpace(line)) Logger.Log($"   > {line.Trim()}", LogType.Info);
                });

                if (exitCode == 0)
                {
                    Logger.Log($"🎉 Backup DISM WIM berhasil dibuat di: {wimFile}", LogType.Success);
                    Logger.Log("File WIM ini dapat dipulihkan (Apply-Image) kapan saja menggunakan installer Windows PE / USB Bootable.", LogType.Success);
                }
                else
                {
                    Logger.Log($"⚠️ DISM Capture selesai dengan kode keluar: {exitCode}. Pastikan ruang penyimpanan drive target mencukupi (minimal 15-25 GB).", LogType.Warning);
                }
            });
        }
    }
}
