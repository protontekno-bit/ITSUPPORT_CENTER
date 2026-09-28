using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class BatteryHealthReportTool : IToolCommand
    {
        public string Id => "sys_battery_report";
        public string Title => "Audit Kesehatan Baterai Laptop";
        public string Description => "Menghasilkan laporan resmi Powercfg Battery Report untuk memeriksa kapasitas asli vs degradasi siklus baterai laptop.";
        public string Category => ToolCategory.System;
        public string Keywords => "battery health report baterai laptop capacity wear cycle drop bocor";
        public string Icon => "🔋";
        public string ButtonText => "Buat Laporan Baterai";
        public Color ButtonColor => Color.FromArgb(22, 160, 133); // Green Sea

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT KESEHATAN BATERAI LAPTOP ===", LogType.Info);

            await Task.Run(async () =>
            {
                string reportPath = Path.Combine(Path.GetTempPath(), $"Battery_Report_{Environment.MachineName}.html");

                Logger.Log($"Menjalankan powercfg /batteryreport...");
                int exitCode = await CommandRunner.RunCmdAsync($"powercfg /batteryreport /output \"{reportPath}\"", s => Logger.Log(s, LogType.Info));

                if (File.Exists(reportPath))
                {
                    Logger.Log($"✅ Laporan baterai berhasil dibuat di: {reportPath}", LogType.Success);
                    Logger.Log("Membuka laporan baterai di browser default...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = reportPath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Buka file manual di {reportPath}: {ex.Message}", LogType.Warning);
                    }
                }
                else
                {
                    Logger.Log("Tidak dapat mendeteksi baterai pada sistem ini (kemungkinan PC Desktop / All-in-One).", LogType.Warning);
                }
            });
        }
    }
}
