using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class CompactOsCompressionTool : IToolCommand
    {
        public string Id => "sys_compact_os";
        public string Title => "Kompresi Sistem Windows (CompactOS)";
        public string Description => "Deteksi status cerdas & kompresi file biner sistem Windows (compact.exe) untuk menghemat 4–8 GB drive C:\\ tanpa mengurangi performa.";
        public string Category => ToolCategory.System;
        public string Keywords => "compactos compact os lzx compression storage hemat disk c lega space decompress";
        public string Icon => "🗜️";
        public string ButtonText => "Smart CompactOS (Kompres/Cek)";
        public Color ButtonColor => Color.FromArgb(22, 160, 133); // Green Sea

        public async Task ExecuteAsync()
        {
            Logger.Log("=== SMART KOMPRESI SISTEM WINDOWS (COMPACTOS MANAGER) ===", LogType.Info);

            await Task.Run(async () =>
            {
                var osInfo = OsDetector.GetOsInfo();
                Logger.Log($"Deteksi OS Target: {osInfo.OsName} (Build {osInfo.BuildNumber}, {osInfo.Architecture})", LogType.Info);
                Logger.Log($"Tipe Penyimpanan Drive C:\\: {osInfo.DriveCType} | Sisa Kapasitas: {osInfo.DriveCFreeGb:F2} GB / {osInfo.DriveCTotalGb:F2} GB", LogType.Info);

                if (osInfo.BuildNumber < 10240)
                {
                    Logger.Log("⚠️ Fitur CompactOS hanya didukung pada Windows 10 dan Windows 11 (Build 10240+). Langkah dibatalkan.", LogType.Warning);
                    return;
                }

                if (osInfo.DriveCType.Equals("HDD", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Log("ℹ️ Perhatian: Drive target terdeteksi HDD mekanik. Proses kompresi mungkin memerlukan waktu hingga 5-10 menit.", LogType.Warning);
                }

                // 1. Cek Status CompactOS Saat Ini
                Logger.Log("1. Memeriksa status kompresi saat ini (compact.exe /compactos:query)...", LogType.Info);
                string queryResult = "";
                await CommandRunner.RunCmdAsync("compact.exe /compactos:query", line =>
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        queryResult += line + " ";
                        Logger.Log($"   > {line.Trim()}", LogType.Info);
                    }
                });

                bool isAlreadyCompact = queryResult.Contains("in the Compact state", StringComparison.OrdinalIgnoreCase) ||
                                       queryResult.Contains("dalam status Padat", StringComparison.OrdinalIgnoreCase);

                if (isAlreadyCompact)
                {
                    Logger.Log("✅ Sistem saat ini SUDAH dalam status terkompresi optimal (Compact state).", LogType.Success);
                    Logger.Log("💡 Jika ingin membatalkan kompresi (kembalikan ke normal), jalankan skrip 'compact.exe /compactos:never'.", LogType.Info);
                    return;
                }

                // 2. Jalankan Kompresi
                double beforeFreeGb = 0;
                try
                {
                    var drive = new DriveInfo("C");
                    beforeFreeGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                }
                catch { }

                Logger.Log("2. Menjalankan kompresi 'compact.exe /compactos:always'...", LogType.Info);

                int exitCode = await CommandRunner.RunCmdAsync("compact.exe /compactos:always", line =>
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Logger.Log($"   > {line.Trim()}", LogType.Info);
                    }
                });

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
                    Logger.Log("🎉 Kompresi CompactOS berhasil diselesaikan!", LogType.Success);
                    if (gainedGb > 0.05)
                    {
                        Logger.Log($"Ruang disk C:\\ berhasil dilegakan sebesar: +{gainedGb:F2} GB! (Total sisa saat ini: {afterFreeGb:F2} GB)", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"Kapasitas sisa drive C:\\ sekarang: {afterFreeGb:F2} GB.", LogType.Info);
                    }
                }
                else
                {
                    Logger.Log($"⚠️ CompactOS menghasilkan kode keluar: {exitCode}.", LogType.Warning);
                }
            });
        }
    }
}
