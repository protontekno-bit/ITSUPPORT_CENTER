using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.SystemTools
{
    public class UltimatePerformanceEngineTool : IToolCommand
    {
        public string Id => "sys_ultimate_performance_engine";
        public string Title => "Aktivasi Ultimate Performance & Optimasi Latensi";
        public string Description => "Buka skema daya 'Ultimate Performance' tersembunyi, matikan pembatasan network throttling Windows, dan bersihkan file memory dump BSOD.";
        public string Category => ToolCategory.System;
        public string Keywords => "ultimate performance power plan powercfg network throttling latency speed cpu memory dump bsod ctt winutil";
        public string Icon => "🚀";
        public string ButtonText => "Optimasi Ultimate Performance";
        public Color ButtonColor => Color.FromArgb(243, 156, 18); // Amber Gold

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AKTIVASI ULTIMATE PERFORMANCE & OPTIMASI LATENSI SISTEM ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Ultimate Performance Suite",
                "Pilih Tindakan Optimasi:\n" +
                "1 = Buka & Aktifkan Skema Daya 'Ultimate Performance' (Maksimal CPU Clock)\n" +
                "2 = Matikan Pembatasan Network Throttling (Optimasi Latensi Jaringan)\n" +
                "3 = Bersihkan Berkas Dump Crash Sistem (Memory.dmp & Minidump)\n" +
                "4 = Terapkan Seluruh Paket Optimasi Sekaligus (Rekomendasi PC Kantor/Gaming)\n" +
                "0 = Batal",
                "4"
            );

            if (string.IsNullOrWhiteSpace(choice) || choice.Trim() == "0")
            {
                Logger.Log("ℹ️ Operasi dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                switch (choice.Trim())
                {
                    case "1":
                        await UnlockUltimatePowerPlanAsync();
                        break;
                    case "2":
                        OptimizeNetworkThrottling();
                        break;
                    case "3":
                        CleanCrashDumps();
                        break;
                    case "4":
                        await UnlockUltimatePowerPlanAsync();
                        OptimizeNetworkThrottling();
                        CleanCrashDumps();
                        break;
                    default:
                        Logger.Log($"⚠️ Pilihan '{choice}' tidak dikenali.", LogType.Warning);
                        break;
                }
            });
        }

        private static async Task UnlockUltimatePowerPlanAsync()
        {
            Logger.Log("Membuka skema daya rahasia 'Ultimate Performance' bawaan Windows Pro Workstation...", LogType.Info);
            string cmd = "powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61";
            await CommandRunner.RunCmdAsync(cmd, s =>
            {
                if (!string.IsNullOrWhiteSpace(s)) Logger.Log(s.Trim(), LogType.Success);
            });
            Logger.Log("✅ Skema 'Ultimate Performance' telah diduplikasi ke sistem Anda.", LogType.Success);
            Logger.Log("   Silakan cek di Control Panel > Power Options untuk memastikan skema aktif.", LogType.Info);
        }

        private static void OptimizeNetworkThrottling()
        {
            Logger.Log("Mengoptimalkan Network Throttling Index & System Responsiveness...", LogType.Info);
            try
            {
                // NetworkThrottlingIndex = 0xffffffff (Menonaktifkan throttling jaringan Windows saat multitasking)
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex", unchecked((int)0xffffffff));
                // SystemResponsiveness = 0 (Memaksimalkan prioritas aplikasi foreground)
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0);

                Logger.Log("✅ Pembatasan bandwidth sistem (Network Throttling) berhasil dinonaktifkan.", LogType.Success);
                Logger.Log("Prioritas respon desktop dan throughput transfer jaringan ditingkatkan ke level maksimal.", LogType.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"⚠️ Gagal mengatur registry Multimedia: {ex.Message}", LogType.Warning);
            }
        }

        private static void CleanCrashDumps()
        {
            Logger.Log("Memeriksa dan menghapus berkas memory dump crash lama (BSOD)...", LogType.Info);
            string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string fullDump = Path.Combine(winDir, "MEMORY.DMP");
            string minidumpDir = Path.Combine(winDir, "Minidump");

            long freedBytes = 0;
            if (File.Exists(fullDump))
            {
                try
                {
                    long len = new FileInfo(fullDump).Length;
                    File.Delete(fullDump);
                    freedBytes += len;
                    Logger.Log($"✅ Berkas MEMORY.DMP ({Math.Round(len / (1024.0 * 1024.0), 1)} MB) berhasil dihapus.", LogType.Success);
                }
                catch { }
            }

            if (Directory.Exists(minidumpDir))
            {
                try
                {
                    foreach (var f in Directory.GetFiles(minidumpDir, "*.dmp"))
                    {
                        try
                        {
                            long len = new FileInfo(f).Length;
                            File.Delete(f);
                            freedBytes += len;
                        }
                        catch { }
                    }
                    Logger.Log("✅ Folder Minidump berhasil dibersihkan.", LogType.Success);
                }
                catch { }
            }

            double mb = Math.Round(freedBytes / (1024.0 * 1024.0), 2);
            Logger.Log($"🎉 Kapasitas disk yang berhasil dibebaskan dari crash dump: {mb} MB.", LogType.Success);
        }
    }
}
