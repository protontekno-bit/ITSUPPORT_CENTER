using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class OneClickTuneUpSuiteTool : IToolCommand
    {
        [DllImport("psapi.dll")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        public string Id => "sys_oneclick_tuneup_suite";
        public string Title => "1-Klik Pemeliharaan & Tune-Up Rutin (All-in-One)";
        public string Description => "Eksekusi berantai otomatis: Bersihkan Temp, Optimalkan RAM, Flush DNS, Bersihkan Spooler, dan Optimasi Responsivitas Windows dalam 1 klik.";
        public string Category => ToolCategory.System;
        public string Keywords => "tune up maintenance rutin otomatis 1 klik clean temp ram flush dns spooler percepat pc";
        public string Icon => "⚡";
        public string ButtonText => "Jalankan 1-Klik Tune-Up Rutin";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Carrot Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== MEMULAI 1-KLIK PEMELIHARAAN & TUNE-UP RUTIN (ALL-IN-ONE) ===", LogType.Info);

            DialogResult confirm = MessageBox.Show(
                "Apakah Anda ingin menjalankan rangkaian pemeliharaan otomatis sekarang?\n\n" +
                "Tindakan yang akan dijalankan:\n" +
                "1. Pembersihan File Sampah / Temporary Disk\n" +
                "2. Pembebasan Memori RAM Idle (Working Set)\n" +
                "3. Reset DNS & Cache Jaringan Lokal\n" +
                "4. Pembersihan Antrean Cetak Printer Nyangkut\n" +
                "5. Optimalisasi Responsivitas Efek Visual",
                "Konfirmasi Tune-Up Rutin",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
            {
                Logger.Log("ℹ️ Pemeliharaan rutin dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();

                // 1. Bersihkan Temp
                Logger.Log("\n[LANGKAH 1/5] Membersihkan Berkas Temporary & Cache Sampah...", LogType.Info);
                int cleanedFiles = CleanTempFiles();
                Logger.Log($"✅ {cleanedFiles} berkas temporary berhasil dibersihkan.", LogType.Success);

                // 2. Optimasi RAM
                Logger.Log("\n[LANGKAH 2/5] Mengosongkan Working Set RAM & Melepaskan Memori Idle...", LogType.Info);
                int ramProcCount = OptimizeWorkingSetRam();
                Logger.Log($"✅ {ramProcCount} proses aplikasi berhasil dipangkas memori idle-nya.", LogType.Success);

                // 3. Flush DNS & Reset Cache Jaringan
                Logger.Log("\n[LANGKAH 3/5] Me-reset DNS Cache & Tiket Jaringan NetBIOS...", LogType.Info);
                await CommandRunner.RunCmdAsync("ipconfig /flushdns", null);
                await CommandRunner.RunCmdAsync("nbtstat -R", null);
                Logger.Log("✅ DNS Cache dan NetBIOS berhasil disegarkan.", LogType.Success);

                // 4. Bersihkan Antrean Cetak
                Logger.Log("\n[LANGKAH 4/5] Memeriksa & Membersihkan Antrean Cetak Spooler...", LogType.Info);
                await CommandRunner.RunCmdAsync("net stop spooler /y", null);
                string spoolDir = @"C:\Windows\System32\spool\PRINTERS";
                if (Directory.Exists(spoolDir))
                {
                    try
                    {
                        foreach (var f in Directory.GetFiles(spoolDir)) File.Delete(f);
                    }
                    catch { }
                }
                await CommandRunner.RunCmdAsync("net start spooler", null);
                Logger.Log("✅ Antrean printer berhasil dinormalkan.", LogType.Success);

                // 5. Optimasi Efek Visual
                Logger.Log("\n[LANGKAH 5/5] Menyelaraskan Responsivitas Visual Windows...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKCU", @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 3);
                Logger.Log("✅ Pengaturan responsivitas desktop diperbarui.", LogType.Success);

                stopwatch.Stop();
                Logger.Log($"\n🎉 [SELESAI] Pemeliharaan Rutin Selesai dalam {stopwatch.Elapsed.TotalSeconds:F1} detik!", LogType.Success);
                Logger.Log("Komputer kini berada dalam kondisi prima dan siap digunakan kembali.", LogType.Success);
            });
        }

        private static int CleanTempFiles()
        {
            int count = 0;
            string[] tempPaths = new[] { Path.GetTempPath(), @"C:\Windows\Temp" };

            foreach (var p in tempPaths)
            {
                if (Directory.Exists(p))
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(p))
                        {
                            try { File.Delete(file); count++; } catch { }
                        }
                    }
                    catch { }
                }
            }
            return count;
        }

        private static int OptimizeWorkingSetRam()
        {
            int count = 0;
            foreach (var proc in Process.GetProcesses())
            {
                try
                {
                    if (proc.Id > 4 && !proc.HasExited)
                    {
                        EmptyWorkingSet(proc.Handle);
                        count++;
                    }
                }
                catch { }
            }
            return count;
        }
    }
}
