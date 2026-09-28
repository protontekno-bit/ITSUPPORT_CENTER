using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Printer
{
    public class ClearPrintQueueTool : IToolCommand
    {
        public string Id => "clear_print_queue";
        public string Title => "Bersihkan Antrean Cetak (Stuck Spooler)";
        public string Description => "Hentikan spooler, hapus seluruh file antrean cetak nyangkut di System32\\spool\\PRINTERS.";
        public string Category => ToolCategory.Printer;
        public string Keywords => "print antrean cetak dokumen nyangkut macet spooler clear cancel";
        public string Icon => "📄";
        public string ButtonText => "Hapus Antrean";
        public Color ButtonColor => Color.FromArgb(231, 76, 60);

        public async Task ExecuteAsync()
        {
            Logger.Log("=== MEMBERSIHKAN ANTREAN CETAK PRINTER NYANGKUT (STUCK SPOOLER) ===");
            await Task.Run(() =>
            {
                try
                {
                    Logger.Log("[1/3] Menghentikan layanan Print Spooler...");
                    WindowsHelper.StopServiceIfExists("spooler");
                    Thread.Sleep(1500);

                    string systemDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    string spoolPrintersDir = Path.Combine(systemDir, "spool", "PRINTERS");

                    int deletedFiles = 0;
                    if (Directory.Exists(spoolPrintersDir))
                    {
                        var files = Directory.GetFiles(spoolPrintersDir, "*.*", SearchOption.AllDirectories);
                        foreach (var f in files)
                        {
                            try
                            {
                                File.Delete(f);
                                deletedFiles++;
                            }
                            catch { }
                        }
                    }

                    Logger.Log($"[2/3] Dihapus {deletedFiles} file antrean cetak yang korup/nyangkut.", LogType.Success);

                    Logger.Log("[3/3] Menyalakan kembali layanan Print Spooler...");
                    WindowsHelper.StartServiceIfExists("spooler");

                    Logger.Log("Antrean printer berhasil dibersihkan total! Printer siap digunakan kembali.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log("Gagal membersihkan antrean printer: " + ex.Message, LogType.Error);
                }
            });
        }
    }
}
