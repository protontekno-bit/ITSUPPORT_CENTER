using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class ResetWindowsUpdateTool : IToolCommand
    {
        public string Id => "sys_reset_windows_update";
        public string Title => "Reset Komponen Windows Update";
        public string Description => "Memperbaiki Windows Update macet/gagal download: Stop update services, bersihkan SoftwareDistribution & Catroot2 cache.";
        public string Category => ToolCategory.System;
        public string Keywords => "windows update reset wuauserv bits catroot softwaredistribution update error 0x800";
        public string Icon => "🔄";
        public string ButtonText => "Reset Windows Update";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Pumpkin Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESET KOMPONEN WINDOWS UPDATE ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Menghentikan service Windows Update (wuauserv, cryptSvc, bits, msiserver)...");
                WindowsHelper.StopServiceIfExists("wuauserv");
                WindowsHelper.StopServiceIfExists("cryptSvc");
                WindowsHelper.StopServiceIfExists("bits");
                WindowsHelper.StopServiceIfExists("msiserver");

                await Task.Delay(1000);

                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string softDist = Path.Combine(winDir, "SoftwareDistribution");
                string catroot2 = Path.Combine(winDir, "System32", "catroot2");

                // SoftwareDistribution Cleanup
                try
                {
                    if (Directory.Exists(softDist))
                    {
                        Logger.Log($"Membersihkan cache download Windows Update: {softDist}...");
                        string softDistBak = Path.Combine(winDir, $"SoftwareDistribution.bak_{DateTime.Now:yyyyMMddHHmmss}");
                        try
                        {
                            Directory.Move(softDist, softDistBak);
                            Logger.Log("Folder SoftwareDistribution berhasil di-backup & di-reset.", LogType.Success);
                            _ = Task.Run(() => { try { Directory.Delete(softDistBak, true); } catch { } });
                        }
                        catch
                        {
                            foreach (var file in Directory.GetFiles(softDist, "*.*", SearchOption.AllDirectories))
                            {
                                try { File.Delete(file); } catch { }
                            }
                            Logger.Log("Sebagian isi SoftwareDistribution dibersihkan.", LogType.Info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Catatan SoftwareDistribution: {ex.Message}", LogType.Warning);
                }

                // Catroot2 Cleanup
                try
                {
                    if (Directory.Exists(catroot2))
                    {
                        Logger.Log($"Mereset cache katalog tanda tangan: {catroot2}...");
                        string catrootBak = Path.Combine(winDir, "System32", $"catroot2.bak_{DateTime.Now:yyyyMMddHHmmss}");
                        try
                        {
                            Directory.Move(catroot2, catrootBak);
                            Logger.Log("Folder Catroot2 berhasil di-backup & di-reset.", LogType.Success);
                            _ = Task.Run(() => { try { Directory.Delete(catrootBak, true); } catch { } });
                        }
                        catch
                        {
                            await CommandRunner.RunCmdAsync($"ren \"{catroot2}\" catroot2.old", null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Catatan Catroot2: {ex.Message}", LogType.Warning);
                }

                Logger.Log("Menjalankan kembali service Windows Update...");
                WindowsHelper.StartServiceIfExists("cryptSvc");
                WindowsHelper.StartServiceIfExists("bits");
                WindowsHelper.StartServiceIfExists("msiserver");
                WindowsHelper.StartServiceIfExists("wuauserv");

                Logger.Log("✅ Reset Windows Update selesai. Silakan cek pembaruan kembali via Windows Settings.", LogType.Success);
            });
        }
    }
}
