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
    public class UserDataBackupRestoreTool : IToolCommand
    {
        public string Id => "sys_user_data_backup_restore";
        public string Title => "Penyelamatan & Migrasi Data User (Backup / Restore Profil)";
        public string Description => "Cadangkan atau pulihkan data esensial user (Desktop, Documents, Downloads, Pictures, Bookmarks Chrome/Edge, Signature Outlook, Sticky Notes) sebelum servis atau format ulang.";
        public string Category => ToolCategory.System;
        public string Keywords => "backup data user restore profil dokumen desktop download bookmarks chrome edge outlook signature stickynotes pindah pc format";
        public string Icon => "💾";
        public string ButtonText => "Penyelamatan Data User";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT PENYELAMATAN & MIGRASI DATA PROFIL PENGGUNA ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Backup & Restore Profil Pengguna",
                "Pilih Tindakan Penyelamatan Data:\n" +
                "1 = Cadangkan Profil User Aktif (Ke Drive D: / External Drive)\n" +
                "2 = Pulihkan Data dari Folder Cadangan ke Komputer Ini\n" +
                "3 = Hitung Estimasi Total Ukuran Berkas User Saat Ini\n" +
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
                        await RunBackupAsync();
                        break;
                    case "2":
                        await RunRestoreAsync();
                        break;
                    case "3":
                        CalculateUserSize();
                        break;
                    default:
                        Logger.Log($"⚠️ Pilihan '{choice}' tidak dikenali.", LogType.Warning);
                        break;
                }
            });
        }

        private async Task RunBackupAsync()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string userName = Environment.UserName;
            string defaultBackup = Directory.Exists("D:\\")
                ? $@"D:\BACKUP_USER_{userName}_{DateTime.Now:yyyyMMdd_HHmm}"
                : Path.Combine(Path.GetTempPath(), $"BACKUP_USER_{userName}_{DateTime.Now:yyyyMMdd_HHmm}");

            string? destDir = InputDialog.Show(
                Form.ActiveForm,
                "Tentukan Folder Tujuan Cadangan",
                "Masukkan folder tujuan penyimpanan backup data user:\n(Disarankan di Drive D:, E:, atau Flashdisk)",
                defaultBackup
            );

            if (string.IsNullOrWhiteSpace(destDir))
            {
                Logger.Log("ℹ️ Lokasi tujuan tidak ditentukan. Operasi dibatalkan.", LogType.Info);
                return;
            }

            destDir = destDir.Trim().TrimEnd('\\');
            try
            {
                Directory.CreateDirectory(destDir);
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ Gagal membuat direktori tujuan: {ex.Message}", LogType.Error);
                return;
            }

            Logger.Log($"📁 Memulai pencadangan data profil '{userName}' ke:", LogType.Info);
            Logger.Log($"   {destDir}", LogType.Success);

            // Daftar folder standar yang dicadangkan
            var standardFolders = new (string Name, string Source)[]
            {
                ("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                ("Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                ("Downloads", Path.Combine(userProfile, "Downloads")),
                ("Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
                ("Videos", Path.Combine(userProfile, "Videos"))
            };

            foreach (var f in standardFolders)
            {
                if (Directory.Exists(f.Source))
                {
                    string target = Path.Combine(destDir, f.Name);
                    Logger.Log($"Mencadangkan folder {f.Name}...", LogType.Info);
                    await CopyFolderRobocopyAsync(f.Source, target);
                }
            }

            // 1. Chrome Bookmarks
            string chromeBookmarks = Path.Combine(userProfile, @"AppData\Local\Google\Chrome\User Data\Default\Bookmarks");
            if (File.Exists(chromeBookmarks))
            {
                try
                {
                    string bTarget = Path.Combine(destDir, "Browser_Data", "Chrome");
                    Directory.CreateDirectory(bTarget);
                    File.Copy(chromeBookmarks, Path.Combine(bTarget, "Bookmarks"), true);
                    Logger.Log("✅ Bookmarks Google Chrome berhasil dicadangkan.", LogType.Success);
                }
                catch { }
            }

            // 2. Microsoft Edge Bookmarks
            string edgeBookmarks = Path.Combine(userProfile, @"AppData\Local\Microsoft\Edge\User Data\Default\Bookmarks");
            if (File.Exists(edgeBookmarks))
            {
                try
                {
                    string bTarget = Path.Combine(destDir, "Browser_Data", "Edge");
                    Directory.CreateDirectory(bTarget);
                    File.Copy(edgeBookmarks, Path.Combine(bTarget, "Bookmarks"), true);
                    Logger.Log("✅ Bookmarks Microsoft Edge berhasil dicadangkan.", LogType.Success);
                }
                catch { }
            }

            // 3. Outlook Signatures
            string signatures = Path.Combine(userProfile, @"AppData\Roaming\Microsoft\Signatures");
            if (Directory.Exists(signatures))
            {
                string sTarget = Path.Combine(destDir, "Outlook_Signatures");
                await CopyFolderRobocopyAsync(signatures, sTarget);
                Logger.Log("✅ Tanda tangan (Signatures) Outlook berhasil dicadangkan.", LogType.Success);
            }

            // 4. Sticky Notes Database
            string stickyNotes = Path.Combine(userProfile, @"AppData\Local\Packages\Microsoft.MicrosoftStickyNotes_8wekyb3d8bbwe\LocalState\plum.sqlite");
            if (File.Exists(stickyNotes))
            {
                try
                {
                    string snTarget = Path.Combine(destDir, "Sticky_Notes");
                    Directory.CreateDirectory(snTarget);
                    File.Copy(stickyNotes, Path.Combine(snTarget, "plum.sqlite"), true);
                    Logger.Log("✅ Catatan Sticky Notes berhasil dicadangkan.", LogType.Success);
                }
                catch { }
            }

            Logger.Log("🎉 [SELESAI] Seluruh data esensial profil pengguna berhasil dicadangkan!", LogType.Success);
            Logger.Log($"Lokasi Hasil Backup: {destDir}", LogType.Success);
        }

        private async Task RunRestoreAsync()
        {
            string? sourceDir = InputDialog.Show(
                Form.ActiveForm,
                "Lokasi Folder Cadangan",
                "Masukkan path lengkap folder cadangan hasil backup sebelumnya:\n(Contoh: D:\\BACKUP_USER_Admin_20260928)",
                ""
            );

            if (string.IsNullOrWhiteSpace(sourceDir) || !Directory.Exists(sourceDir.Trim()))
            {
                Logger.Log("⚠️ Direktori sumber cadangan tidak valid atau tidak ditemukan.", LogType.Warning);
                return;
            }

            sourceDir = sourceDir.Trim().TrimEnd('\\');
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Logger.Log($"Memulai pemulihan data dari {sourceDir} ke profil aktif saat ini...", LogType.Info);

            var foldersToRestore = new (string FolderName, string Destination)[]
            {
                ("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                ("Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                ("Downloads", Path.Combine(userProfile, "Downloads")),
                ("Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
                ("Videos", Path.Combine(userProfile, "Videos"))
            };

            foreach (var f in foldersToRestore)
            {
                string src = Path.Combine(sourceDir, f.FolderName);
                if (Directory.Exists(src))
                {
                    Logger.Log($"Memulihkan {f.FolderName}...", LogType.Info);
                    await CopyFolderRobocopyAsync(src, f.Destination);
                }
            }

            // Pulihkan Signatures Outlook
            string sigSrc = Path.Combine(sourceDir, "Outlook_Signatures");
            if (Directory.Exists(sigSrc))
            {
                string sigDest = Path.Combine(userProfile, @"AppData\Roaming\Microsoft\Signatures");
                Directory.CreateDirectory(sigDest);
                await CopyFolderRobocopyAsync(sigSrc, sigDest);
                Logger.Log("✅ Signatures Outlook berhasil dipulihkan.", LogType.Success);
            }

            Logger.Log("🎉 [SELESAI] Data pengguna telah berhasil dipulihkan ke profil saat ini.", LogType.Success);
        }

        private void CalculateUserSize()
        {
            Logger.Log("--- ESTIMASI UKURAN FOLDER PENGGUNA ---", LogType.Info);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var targets = new (string Name, string Path)[]
            {
                ("Desktop", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)),
                ("Documents", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                ("Downloads", Path.Combine(userProfile, "Downloads")),
                ("Pictures", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
                ("Videos", Path.Combine(userProfile, "Videos"))
            };

            long totalBytes = 0;
            foreach (var t in targets)
            {
                if (Directory.Exists(t.Path))
                {
                    long bytes = GetDirectorySize(t.Path);
                    totalBytes += bytes;
                    double mb = Math.Round(bytes / (1024.0 * 1024.0), 2);
                    Logger.Log($"• {t.Name,-12}: {mb,8} MB ({t.Path})", LogType.Info);
                }
            }

            double totalGb = Math.Round(totalBytes / (1024.0 * 1024.0 * 1024.0), 2);
            Logger.Log($"📊 Total Estimasi Ukuran Data: {totalGb} GB", LogType.Success);
            Logger.Log("Pastikan drive cadangan Anda memiliki kapasitas bebas minimal sebesar estimasi di atas.", LogType.Info);
        }

        private static long GetDirectorySize(string folderPath)
        {
            try
            {
                var dir = new DirectoryInfo(folderPath);
                long size = 0;
                foreach (var fi in dir.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    try { size += fi.Length; } catch { }
                }
                return size;
            }
            catch
            {
                return 0;
            }
        }

        private static async Task CopyFolderRobocopyAsync(string source, string destination)
        {
            try
            {
                Directory.CreateDirectory(destination);
                string cmd = $"robocopy \"{source}\" \"{destination}\" /E /MT:8 /R:1 /W:1 /NP /NFL /NDL /XJ";
                await CommandRunner.RunCmdAsync(cmd, null);
            }
            catch (Exception ex)
            {
                Logger.Log($"⚠️ Error saat menyalin {source}: {ex.Message}", LogType.Warning);
            }
        }
    }
}
