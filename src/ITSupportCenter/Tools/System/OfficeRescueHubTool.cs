using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.SystemTools
{
    public class OfficeRescueHubTool : IToolCommand
    {
        public string Id => "sys_office_rescue_hub";
        public string Title => "Pertolongan Pertama Microsoft Office & Outlook (Rescue Hub)";
        public string Description => "Pusat perbaikan cepat saat Word, Excel, PowerPoint, atau Outlook crash, hang, macet splash screen, perbaikan profil Outlook (scanpst.exe), dan reset cache dokumen.";
        public string Category => ToolCategory.System;
        public string Keywords => "office word excel outlook safe mode scanpst repair normal dotm crash hang ost pst email template corrupt";
        public string Icon => "🚑";
        public string ButtonText => "Buka Rescue Hub Office & Outlook";
        public Color ButtonColor => Color.FromArgb(211, 84, 0); // Pumpkin Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT PERTOLONGAN PERTAMA MICROSOFT OFFICE & OUTLOOK ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Office & Outlook Rescue Hub",
                "Pilih Tindakan Pertolongan Cepat yang Dibutuhkan:\n" +
                "1 = Luncurkan Safe Mode (Word / Excel / Outlook / PPT)\n" +
                "2 = Perbaiki Startup & Tampilan Outlook (/resetnavpane & /cleanviews)\n" +
                "3 = Cari & Jalankan scanpst.exe (Inbox Repair Tool Outlook)\n" +
                "4 = Reset Template Rusak Word (Normal.dotm) & Excel (.xlb)\n" +
                "5 = Bersihkan Cache Dokumen Macet (OfficeFileCache & Kill Zombie)\n" +
                "6 = Picu C2R Quick Repair Resmi Microsoft Office\n" +
                "0 = Batal",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice) || choice.Trim() == "0")
            {
                Logger.Log("ℹ️ Operasi dibatalkan oleh pengguna.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                switch (choice.Trim())
                {
                    case "1":
                        LaunchOfficeSafeMode();
                        break;
                    case "2":
                        await RepairOutlookStartupViewsAsync();
                        break;
                    case "3":
                        LocateAndRunScanPst();
                        break;
                    case "4":
                        ResetOfficeTemplates();
                        break;
                    case "5":
                        await CleanOfficeDocumentCacheAsync();
                        break;
                    case "6":
                        TriggerClickToRunQuickRepair();
                        break;
                    default:
                        Logger.Log($"⚠️ Pilihan '{choice}' tidak valid.", LogType.Warning);
                        break;
                }
            });
        }

        private void LaunchOfficeSafeMode()
        {
            Logger.Log("--- LUNCURKAN OFFICE DALAM SAFE MODE ---", LogType.Info);
            string? appChoice = InputDialog.Show(
                Form.ActiveForm,
                "Pilih Aplikasi Office Safe Mode",
                "Pilih aplikasi yang ingin dibuka dalam Safe Mode:\n" +
                "1 = Microsoft Word (winword /safe)\n" +
                "2 = Microsoft Excel (excel /safe)\n" +
                "3 = Microsoft Outlook (outlook /safe)\n" +
                "4 = Microsoft PowerPoint (powerpnt /safe)",
                "1"
            );

            if (string.IsNullOrWhiteSpace(appChoice)) return;

            string cmd = appChoice.Trim() switch
            {
                "1" => "winword /safe",
                "2" => "excel /safe",
                "3" => "outlook /safe",
                "4" => "powerpnt /safe",
                _ => ""
            };

            if (string.IsNullOrEmpty(cmd))
            {
                Logger.Log("⚠️ Pilihan aplikasi tidak dikenali.", LogType.Warning);
                return;
            }

            Logger.Log($"Membuka aplikasi dengan parameter: {cmd}...", LogType.Info);
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c start {cmd}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                Logger.Log("✅ Perintah Safe Mode berhasil dikirim. Aplikasi akan terbuka tanpa memuat add-in bermasalah.", LogType.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ Gagal meluncurkan Safe Mode: {ex.Message}", LogType.Error);
            }
        }

        private async Task RepairOutlookStartupViewsAsync()
        {
            Logger.Log("--- PERBAIKAN STARTUP & TAMPILAN OUTLOOK ---", LogType.Info);
            Logger.Log("Menghentikan proses Outlook yang mungkin menggantung...", LogType.Info);
            await CommandRunner.RunCmdAsync("taskkill /f /im outlook.exe", null);

            Logger.Log("1. Mereset Navigation Pane (/resetnavpane)...", LogType.Info);
            Logger.Log("   (Menangani error: 'Cannot start Microsoft Outlook. Cannot open the Outlook window')", LogType.Info);
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c start outlook.exe /resetnavpane",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                Logger.Log("✅ Outlook diluncurkan dengan reset Navigation Pane.", LogType.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"⚠️ Error saat menjalankan outlook /resetnavpane: {ex.Message}", LogType.Warning);
            }

            Logger.Log("💡 Perintah alternatif yang dapat dicoba jika masih bermasalah:", LogType.Info);
            Logger.Log("   • outlook.exe /cleanviews (Kembalikan tampilan folder & inbox ke default)", LogType.Info);
            Logger.Log("   • outlook.exe /cleanreminders (Bersihkan kalender / reminder macet)", LogType.Info);
        }

        private void LocateAndRunScanPst()
        {
            Logger.Log("--- PENCARIAN & PELUNCURAN INBOX REPAIR TOOL (SCANPST.EXE) ---", LogType.Info);

            string[] candidatePaths = new[]
            {
                @"C:\Program Files\Microsoft Office\root\Office16\SCANPST.EXE",
                @"C:\Program Files (x86)\Microsoft Office\root\Office16\SCANPST.EXE",
                @"C:\Program Files\Microsoft Office\Office16\SCANPST.EXE",
                @"C:\Program Files (x86)\Microsoft Office\Office16\SCANPST.EXE",
                @"C:\Program Files\Microsoft Office\Office15\SCANPST.EXE",
                @"C:\Program Files (x86)\Microsoft Office\Office15\SCANPST.EXE",
                @"C:\Program Files\Microsoft Office\Office14\SCANPST.EXE",
                @"C:\Program Files (x86)\Microsoft Office\Office14\SCANPST.EXE"
            };

            string? foundPath = null;
            foreach (var path in candidatePaths)
            {
                if (File.Exists(path))
                {
                    foundPath = path;
                    break;
                }
            }

            if (foundPath != null)
            {
                Logger.Log($"✅ Ditemukan scanpst.exe di: {foundPath}", LogType.Success);
                Logger.Log("Meluncurkan Inbox Repair Tool...", LogType.Info);
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = foundPath,
                        UseShellExecute = true
                    });
                    Logger.Log("✅ SCANPST.EXE berhasil dibuka. Pilih file .PST atau .OST Anda lalu klik 'Start' untuk mulai scan perbaikan.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"❌ Gagal membuka scanpst.exe: {ex.Message}", LogType.Error);
                }
            }
            else
            {
                Logger.Log("⚠️ SCANPST.EXE tidak ditemukan di lokasi standar Office.", LogType.Warning);
                Logger.Log("Pastikan Microsoft Office / Outlook terpasang di komputer ini.", LogType.Info);
            }
        }

        private void ResetOfficeTemplates()
        {
            Logger.Log("--- RESET TEMPLATE RUSAK WORD & EXCEL ---", LogType.Info);
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // 1. Reset Word Normal.dotm
            string wordTemplatesDir = Path.Combine(appData, @"Microsoft\Templates");
            string normalDotm = Path.Combine(wordTemplatesDir, "Normal.dotm");

            if (File.Exists(normalDotm))
            {
                try
                {
                    string backupName = Path.Combine(wordTemplatesDir, $"Normal.dotm.bak_{DateTime.Now:yyyyMMdd_HHmmss}");
                    File.Move(normalDotm, backupName);
                    Logger.Log($"✅ Template Word korup berhasil di-backup & di-reset: {Path.GetFileName(backupName)}", LogType.Success);
                    Logger.Log("   Microsoft Word akan membuat Normal.dotm baru yang segar saat dibuka berikutnya.", LogType.Info);
                }
                catch (Exception ex)
                {
                    Logger.Log($"⚠️ Gagal me-reset Normal.dotm (mungkin Word sedang aktif): {ex.Message}", LogType.Warning);
                }
            }
            else
            {
                Logger.Log("ℹ️ File Normal.dotm tidak ditemukan di direktori templates.", LogType.Info);
            }

            // 2. Reset Excel Toolbars & Custom Settings (.xlb)
            string excelDir = Path.Combine(appData, @"Microsoft\Excel");
            if (Directory.Exists(excelDir))
            {
                var xlbFiles = Directory.GetFiles(excelDir, "*.xlb");
                foreach (var xlb in xlbFiles)
                {
                    try
                    {
                        File.Delete(xlb);
                        Logger.Log($"✅ Cache toolbar Excel korup dihapus: {Path.GetFileName(xlb)}", LogType.Success);
                    }
                    catch { }
                }
            }
            Logger.Log("Selesai me-reset template default dokumen.", LogType.Success);
        }

        private async Task CleanOfficeDocumentCacheAsync()
        {
            Logger.Log("--- PEMBERSIHAN CACHE DOKUMEN OFFICE (OFFICEFILECACHE) ---", LogType.Info);
            Logger.Log("Menghentikan proses background Office yang menahan file lock...", LogType.Info);

            await CommandRunner.RunCmdAsync("taskkill /f /im msosync.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im winword.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im excel.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im powerpnt.exe", null);

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string[] cachePaths = new[]
            {
                Path.Combine(localAppData, @"Microsoft\Office\16.0\OfficeFileCache"),
                Path.Combine(localAppData, @"Microsoft\Office\15.0\OfficeFileCache")
            };

            int cleanedCount = 0;
            foreach (var path in cachePaths)
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                        Logger.Log($"✅ Berhasil membersihkan direktori cache: {path}", LogType.Success);
                        cleanedCount++;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"⚠️ Sebagian file cache terkunci: {ex.Message}", LogType.Warning);
                    }
                }
            }

            if (cleanedCount > 0)
            {
                Logger.Log("✅ Cache dokumen berhasil disegarkan. Masalah 'Upload Failed' atau 'File Locked' pada Excel/Word teratasi.", LogType.Success);
            }
            else
            {
                Logger.Log("ℹ️ Tidak ada direktori OfficeFileCache yang tersisa.", LogType.Info);
            }
        }

        private void TriggerClickToRunQuickRepair()
        {
            Logger.Log("--- PICU CEPAT QUICK REPAIR MICROSOFT OFFICE ---", LogType.Info);
            string c2rRunner = @"C:\Program Files\Common Files\microsoft shared\ClickToRun\OfficeClickToRun.exe";

            if (!File.Exists(c2rRunner))
            {
                c2rRunner = @"C:\Program Files (x86)\Common Files\microsoft shared\ClickToRun\OfficeClickToRun.exe";
            }

            if (File.Exists(c2rRunner))
            {
                Logger.Log($"Ditemukan engine Click-to-Run di: {c2rRunner}", LogType.Info);
                Logger.Log("Memulai dialog perbaikan Quick Repair resmi Office...", LogType.Info);
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = c2rRunner,
                        Arguments = "scenario=QuickRepair platform=x64 culture=en-us ForceAppShutdown=True",
                        UseShellExecute = true
                    });
                    Logger.Log("✅ Engine Quick Repair Office telah dipicu. Ikuti jendela panduan Microsoft yang muncul.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"❌ Gagal memicu Quick Repair: {ex.Message}", LogType.Error);
                }
            }
            else
            {
                Logger.Log("⚠️ Engine Office Click-to-Run tidak ditemukan. Gunakan Control Panel > Programs and Features untuk perbaikan.", LogType.Warning);
            }
        }
    }
}
