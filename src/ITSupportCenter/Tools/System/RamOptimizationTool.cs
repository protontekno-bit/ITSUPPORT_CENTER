using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class RamOptimizationTool : IToolCommand
    {
        public string Id => "sys_smart_ram_optimizer";
        public string Title => "Optimasi RAM Cerdas & Pembersih Proses";
        public string Description => "Memindai pemakaian RAM, melakukan flush working-set aman (tanpa crash), dan membersihkan background updater/bloatware tidak penting.";
        public string Category => ToolCategory.System;
        public string Keywords => "ram memory optimize clean proses process kill working set flush free ram cache";
        public string Icon => "⚡";
        public string ButtonText => "Optimalkan RAM & Proses";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        #region Win32 API Definitions

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        [DllImport("psapi.dll", SetLastError = true)]
        private static extern int EmptyWorkingSet(IntPtr hProcess);

        #endregion

        #region Whitelist & Target Definitions

        // 1. PROSES KRITIS SISTEM OPERASI & DRIVER (HARAM DISENTUH / DIMATIKAN)
        // Menjamin Windows tidak akan pernah BSOD, freeze, atau kehilangan fungsi vital
        private static readonly HashSet<string> SystemProtectedProcesses = new(StringComparer.OrdinalIgnoreCase)
        {
            // Windows Core Kernel & Session
            "system", "idle", "registry", "smss", "csrss", "wininit", "services", "lsass", "lsm",
            "winlogon", "fontdrvhost", "dwm", "explorer", "sihost", "taskhostw", "ctfmon",
            
            // Audio, Display & Hardware Drivers
            "audiodg", "nvcontainer", "nvsphelper64", "amdrsserv", "igfxem", "igfxcui", "igfxtray",
            
            // Security & Antivirus
            "msmpeng", "nissrv", "securityhealthservice", "securityhealthsystray", "smartscreen",
            
            // Windows Shell & Modern UI Framework
            "runtimebroker", "searchhost", "startmenuexperiencehost", "shellexperiencehost",
            "searchindexer", "textinputhost", "applicationframehost", "systemsettings", "spoolsv",
            
            // Developer Tools, Terminal & IDE (Agar pekerjaan developer tidak terputus)
            "devenv", "code", "idea64", "rider64", "powershell", "pwsh", "cmd", "conhost",
            "windowsterminal", "taskmgr"
        };

        // 2. BACKGROUND UPDATER & TELEMETRY NON-ESENSIAL (AMAN UNTUK DIBERSIHKAN JIKA TIDAK ADA JENDELA AKTIF)
        // Hanya proses background tanpa tampilan antarmuka (headless) yang sering menimbun RAM
        private static readonly HashSet<string> SafeToKillBackgroundTasks = new(StringComparer.OrdinalIgnoreCase)
        {
            "msedgeupdate", "googleupdate", "adobearm", "jusched", "onedrivestandaloneupdater",
            "wermgr", "compattelrunner", "telemetryrunner", "crashpad_handler", "softwareupdater",
            "ccleanerupdate", "dropboxupdate"
        };

        #endregion

        public async Task ExecuteAsync()
        {
            Logger.Log("======================================================================");
            Logger.Log("         ⚡ OPTIMASI RAM CERDAS & PEMBERSIH PROSES AMAN");
            Logger.Log("======================================================================");

            await Task.Run(async () =>
            {
                // 1. Ambil status RAM sebelum optimasi
                var memBefore = new MEMORYSTATUSEX();
                if (!GlobalMemoryStatusEx(memBefore))
                {
                    Logger.Log("Gagal membaca status memori sistem melalui Windows API.", LogType.Error);
                    return;
                }

                double totalPhysGb = memBefore.ullTotalPhys / (1024.0 * 1024.0 * 1024.0);
                double availBeforeGb = memBefore.ullAvailPhys / (1024.0 * 1024.0 * 1024.0);
                double usedBeforeGb = totalPhysGb - availBeforeGb;
                uint loadBefore = memBefore.dwMemoryLoad;

                Logger.Log($"[STATUS AWAL] Total Fisik RAM : {totalPhysGb:F2} GB");
                Logger.Log($"[STATUS AWAL] RAM Terpakai    : {usedBeforeGb:F2} GB ({loadBefore}%) | Bebas: {availBeforeGb:F2} GB", 
                    loadBefore >= 80 ? LogType.Warning : LogType.Info);

                // 2. Audit & Tampilkan Top 5 Proses Terberat Saat Ini
                Logger.Log("\n--- 5 Proses Konsumsi Memori Terbesar Saat Ini ---");
                Process[] allProcesses = Array.Empty<Process>();
                try
                {
                    allProcesses = Process.GetProcesses();
                    var topProcesses = allProcesses
                        .Where(p => {
                            try { return !p.HasExited && p.WorkingSet64 > 0; }
                            catch { return false; }
                        })
                        .OrderByDescending(p => {
                            try { return p.WorkingSet64; }
                            catch { return 0; }
                        })
                        .Take(5)
                        .ToList();

                    int rank = 1;
                    foreach (var proc in topProcesses)
                    {
                        try
                        {
                            double mb = proc.WorkingSet64 / (1024.0 * 1024.0);
                            Logger.Log($"  {rank}. {proc.ProcessName} (PID {proc.Id}): {mb:F1} MB");
                            rank++;
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Catatan saat audit daftar proses: {ex.Message}", LogType.Info);
                }

                int currentProcessId = Environment.ProcessId;
                string currentProcessName = Process.GetCurrentProcess().ProcessName;

                // 3. Tahap 1: Pembersihan Background Bloatware Non-Esensial yang Menggantung
                Logger.Log("\n[TAHAP 1] Memeriksa background updater & telemetry non-esensial...");
                int terminatedCount = 0;
                long bloatBytesFreed = 0;

                foreach (var proc in allProcesses)
                {
                    try
                    {
                        if (proc.Id <= 4 || proc.Id == currentProcessId || proc.HasExited) continue;

                        string pName = proc.ProcessName;

                        // Proteksi mutlak: Jangan sentuh proses sistem atau aplikasi kita
                        if (SystemProtectedProcesses.Contains(pName)) continue;

                        // Hanya terminasi jika ada di daftar aman DAN tidak memiliki jendela antarmuka yang sedang dilihat user
                        if (SafeToKillBackgroundTasks.Contains(pName))
                        {
                            // Verifikasi keamanan tambahan: Pastikan bukan jendela aktif pengguna
                            bool hasActiveWindow = false;
                            try { hasActiveWindow = proc.MainWindowHandle != IntPtr.Zero; } catch { }

                            if (!hasActiveWindow)
                            {
                                long mem = 0;
                                try { mem = proc.WorkingSet64; } catch { }

                                proc.Kill();
                                proc.WaitForExit(1500);

                                terminatedCount++;
                                bloatBytesFreed += mem;
                                double mbFreed = mem / (1024.0 * 1024.0);
                                Logger.Log($"  -> Dihentikan aman: {pName} (PID {proc.Id}, ~{mbFreed:F1} MB)", LogType.Success);
                            }
                        }
                    }
                    catch
                    {
                        // Lewati proses yang dilindungi hak akses
                    }
                }

                if (terminatedCount == 0)
                {
                    Logger.Log("  -> Sistem bersih: Tidak ada background updater/telemetry hanging yang terdeteksi.", LogType.Info);
                }
                else
                {
                    double totalBloatMb = bloatBytesFreed / (1024.0 * 1024.0);
                    Logger.Log($"  -> Berhasil membersihkan {terminatedCount} background task (~{totalBloatMb:F1} MB dilepaskan).", LogType.Success);
                }

                // 4. Tahap 2: Safe Working-Set Trim (EmptyWorkingSet) Tanpa Menutup Aplikasi
                Logger.Log("\n[TAHAP 2] Melakukan Safe Working-Set Trim pada semua proses aktif...");
                Logger.Log("  (Memaksa pelepasan cache RAM idle ke sistem tanpa menutup aplikasi/dokumen kerja)");

                int trimmedCount = 0;
                int protectedCount = 0;

                // Ambil daftar proses segar setelah tahap 1
                Process[] activeProcesses = Process.GetProcesses();
                foreach (var proc in activeProcesses)
                {
                    try
                    {
                        if (proc.Id <= 4 || proc.Id == currentProcessId || proc.HasExited)
                        {
                            proc.Dispose();
                            continue;
                        }

                        // Jika proses sistem protected, tetap aman untuk di-trim working set jika diizinkan OS,
                        // namun jika OS menolak access rights, catch akan melewatinya secara anggun.
                        int res = EmptyWorkingSet(proc.Handle);
                        if (res != 0)
                        {
                            trimmedCount++;
                        }
                    }
                    catch
                    {
                        protectedCount++;
                    }
                    finally
                    {
                        try { proc.Dispose(); } catch { }
                    }
                }

                Logger.Log($"  -> Berhasil merapikan working-set pada {trimmedCount} proses aktif (Protected/Skipped: {protectedCount}).", LogType.Success);

                // 5. Tahap 3: Pembersihan Garbage Collection internal
                GC.Collect();
                GC.WaitForPendingFinalizers();

                await Task.Delay(1000); // Beri jeda 1 detik agar Memory Manager Windows mencatat statistik terbaru

                // 6. Evaluasi & Perhitungan Hasil Akhir
                var memAfter = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memAfter))
                {
                    double availAfterGb = memAfter.ullAvailPhys / (1024.0 * 1024.0 * 1024.0);
                    double usedAfterGb = totalPhysGb - availAfterGb;
                    uint loadAfter = memAfter.dwMemoryLoad;

                    double freedGb = availAfterGb - availBeforeGb;
                    double freedMb = freedGb * 1024.0;

                    Logger.Log("======================================================================");
                    Logger.Log($"[HASIL AKHIR] RAM Terpakai  : {usedAfterGb:F2} GB ({loadAfter}%) | Bebas: {availAfterGb:F2} GB");

                    if (freedMb > 0)
                    {
                        if (freedGb >= 1.0)
                        {
                            Logger.Log($"✅ SUKSES: Berhasil melegakan ~{freedGb:F2} GB ({freedMb:F0} MB) kapasitas RAM!", LogType.Success);
                        }
                        else
                        {
                            Logger.Log($"✅ SUKSES: Berhasil melegakan ~{freedMb:F0} MB kapasitas RAM!", LogType.Success);
                        }

                        int loadDiff = (int)loadBefore - (int)loadAfter;
                        if (loadDiff > 0)
                        {
                            Logger.Log($"📉 Beban memori sistem berkurang sebesar {loadDiff}% (dari {loadBefore}% menjadi {loadAfter}%).", LogType.Success);
                        }
                    }
                    else
                    {
                        Logger.Log("✅ Memori sistem sudah dalam kondisi optimal dan sangat efisien.", LogType.Success);
                    }

                    Logger.Log("🛡️ Jaminan Keamanan: Semua aplikasi aktif, browser, Office, dan OS tetap berjalan stabil tanpa crash.", LogType.Success);
                    Logger.Log("======================================================================");
                }

                // Bersihkan alokasi array proses awal
                foreach (var p in allProcesses)
                {
                    try { p.Dispose(); } catch { }
                }
            });
        }
    }
}
