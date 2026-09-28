using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class BackupDriversTool : IToolCommand
    {
        public string Id => "sys_backup_drivers";
        public string Title => "Backup Semua Driver OEM / Pihak Ketiga";
        public string Description => "Mengekstrak dan mencadangkan seluruh driver OEM aktif (WiFi, LAN, Audio, GPU) ke folder sebelum install ulang via DISM.";
        public string Category => ToolCategory.System;
        public string Keywords => "driver backup export oem dism simpan wifi lan audio display";
        public string Icon => "💾";
        public string ButtonText => "Backup Driver PC Ini";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== BACKUP SEMUA DRIVER PERANGKAT OEM (DISM) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // Select target drive: Prefer D:\ or secondary fixed drive, fallback to C:\
                string targetBaseDir = @"C:\Driver_Backup";
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Fixed && !drive.Name.StartsWith("C", StringComparison.OrdinalIgnoreCase))
                    {
                        targetBaseDir = Path.Combine(drive.Name, "Driver_Backup");
                        break;
                    }
                }

                string backupPath = Path.Combine(targetBaseDir, $"{Environment.MachineName}_{DateTime.Now:yyyyMMdd_HHmmss}");

                try
                {
                    if (!Directory.Exists(backupPath))
                        Directory.CreateDirectory(backupPath);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal membuat folder backup: {ex.Message}", LogType.Error);
                    return;
                }

                Logger.Log($"Menyimpan file driver ke direktori: {backupPath}...", LogType.Info);
                Logger.Log("Sedang mengekstraksi seluruh driver dari Windows Driver Store. Mohon tunggu...", LogType.Warning);

                string cmd = $"dism /online /export-driver /destination:\"{backupPath}\"";
                int exitCode = await CommandRunner.RunCmdAsync(cmd, s =>
                {
                    if (s.Contains("Exporting") || s.Contains("Driver package") || s.Contains("operation completed"))
                    {
                        Logger.Log(s, LogType.Info);
                    }
                });

                if (exitCode == 0)
                {
                    int infCount = Directory.GetFiles(backupPath, "*.inf", SearchOption.AllDirectories).Length;
                    Logger.Log($"✅ Backup Driver Berhasil! {infCount} paket driver tersimpan di: {backupPath}", LogType.Success);

                    // Open folder in Explorer
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = backupPath,
                            UseShellExecute = true
                        });
                    }
                    catch { }
                }
                else
                {
                    Logger.Log($"Ekspor driver selesai dengan kode: {exitCode}", LogType.Warning);
                }
            });
        }
    }
}
