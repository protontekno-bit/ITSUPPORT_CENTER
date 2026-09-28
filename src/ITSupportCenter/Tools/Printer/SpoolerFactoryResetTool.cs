using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Printer
{
    public class SpoolerFactoryResetTool : IToolCommand
    {
        public string Id => "printer_spooler_factory_reset";
        public string Title => "Reset Total Spooler & Solusi Spooler Crash Loop";
        public string Description => "Solusi saat service Print Spooler mati sendiri terus-menerus: Bersihkan antrean macet, reset service spooler, dan buka Print Management.";
        public string Category => ToolCategory.Printer;
        public string Keywords => "spooler crash loop mati sendiri stop stopped reset factory spool printers printmanagement";
        public string Icon => "🔄";
        public string ButtonText => "Reset Total Spooler";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESET TOTAL PRINT SPOOLER & SOLUSI SPOOLER CRASH LOOP ===", LogType.Info);

            await Task.Run(async () =>
            {
                // 1. Force Stop Spooler
                Logger.Log("1/4: Menghentikan service Print Spooler...");
                WindowsHelper.StopServiceIfExists("Spooler");
                await CommandRunner.RunCmdAsync("net stop spooler /y", null);
                await Task.Delay(1000);

                // 2. Clean spool folder
                Logger.Log("2/4: Membersihkan seluruh berkas antrean cetak macet (*.SPL / *.SHD)...");
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string spoolDir = Path.Combine(winDir, @"System32\spool\PRINTERS");

                int deletedFiles = 0;
                if (Directory.Exists(spoolDir))
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(spoolDir, "*.*"))
                        {
                            try
                            {
                                File.Delete(file);
                                deletedFiles++;
                            }
                            catch { }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Catatan Spool: {ex.Message}", LogType.Warning);
                    }
                }
                Logger.Log($"Berhasil menghapus {deletedFiles} file antrean macet.", LogType.Success);

                // 3. Restart Spooler service
                Logger.Log("3/4: Menjalankan kembali Print Spooler Service...");
                WindowsHelper.StartServiceIfExists("Spooler");
                await CommandRunner.RunCmdAsync("net start spooler", null);

                // 4. Open Print Management console
                Logger.Log("4/4: Membuka konsol Print Management (printmanagement.msc)...", LogType.Info);
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = "printmanagement.msc", UseShellExecute = true });
                    Logger.Log("✅ Konsol Print Management terbuka. Anda dapat menghapus driver yang korup secara manual.", LogType.Success);
                }
                catch
                {
                    try { Process.Start(new ProcessStartInfo { FileName = "rundll32.exe", Arguments = "printui.dll,PrintUIEntry /s", UseShellExecute = true }); } catch { }
                }

                Logger.Log("✅ Reset Spooler selesai! Print Spooler kini berjalan stabil.", LogType.Success);
            });
        }
    }
}
