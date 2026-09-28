using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class OfficeLicenseConflictCleanerTool : IToolCommand
    {
        public string Id => "lic_office_conflict_cleaner";
        public string Title => "Pembersih Konflik Lisensi & Akun Office (Ghost Key & Token Reset)";
        public string Description => "Hapus sisa key lisensi lama/ganda (ospp.vbs /unpkey) yang memicu banner 'Unlicensed Product', serta reset cache token login Microsoft 365 (OneAuth & IdentityCache).";
        public string Category => ToolCategory.License;
        public string Keywords => "office license conflict unpkey ospp vbs ghost key unlicensed product oneauth identitycache reset account token login loop";
        public string Icon => "🧹";
        public string ButtonText => "Pembersih Lisensi & Akun Office";
        public Color ButtonColor => Color.FromArgb(155, 89, 182); // Amethyst Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMBERSIH KONFLIK LISENSI & AKUN LOGIN MICROSOFT OFFICE ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Office License & Account Cleaner",
                "Pilih Tindakan Pembersihan Lisensi / Akun Office:\n" +
                "1 = Pindai Lisensi Terpasang (Cek Key Hantu / Grace / Trial)\n" +
                "2 = Hapus Key Lisensi Lama / Bentrok (Masukkan 5 Karakter Key)\n" +
                "3 = Reset Cache Akun Login & Modern Auth (OneAuth + IdentityCache)\n" +
                "4 = Bersihkan Lisensi Hantu + Reset Token Login Sekaligus (Rekomendasi Bersih)\n" +
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
                        await ScanOfficeKeysAsync();
                        break;
                    case "2":
                        await UninstallSpecificKeyPromptAsync();
                        break;
                    case "3":
                        await ResetOfficeModernAuthCacheAsync();
                        break;
                    case "4":
                        await ResetOfficeModernAuthCacheAsync();
                        await ScanOfficeKeysAsync();
                        await UninstallSpecificKeyPromptAsync();
                        break;
                    default:
                        Logger.Log($"⚠️ Pilihan '{choice}' tidak valid.", LogType.Warning);
                        break;
                }
            });
        }

        private string? LocateOsppVbs()
        {
            string[] candidatePaths = new[]
            {
                @"C:\Program Files\Microsoft Office\Office16\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\Office16\OSPP.VBS",
                @"C:\Program Files\Microsoft Office\Office15\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\Office15\OSPP.VBS",
                @"C:\Program Files\Microsoft Office\Office14\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\Office14\OSPP.VBS",
                @"C:\Program Files\Microsoft Office\root\Office16\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\root\Office16\OSPP.VBS"
            };

            foreach (var path in candidatePaths)
            {
                if (File.Exists(path)) return path;
            }
            return null;
        }

        private async Task ScanOfficeKeysAsync()
        {
            Logger.Log("--- MEMINDAI STATUS LISENSI & KUNCI TERPASANG ---", LogType.Info);
            string? ospp = LocateOsppVbs();
            if (ospp == null)
            {
                Logger.Log("❌ File OSPP.VBS tidak ditemukan di direktori Microsoft Office.", LogType.Error);
                return;
            }

            Logger.Log($"Menjalankan OSPP.VBS di: {ospp}", LogType.Info);
            int exitCode = await CommandRunner.RunCmdAsync($"cscript //nologo \"{ospp}\" /dstatus", s =>
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (s.Contains("LICENSE STATUS:  --- LICENSED ---"))
                        Logger.Log(s.Trim(), LogType.Success);
                    else if (s.Contains("LICENSE STATUS:") || s.Contains("--- GRACE ---") || s.Contains("NOTIFICATIONS"))
                        Logger.Log(s.Trim(), LogType.Warning);
                    else if (s.Contains("Last 5 characters of installed product key:"))
                        Logger.Log($"🔑 {s.Trim()}", LogType.Success);
                    else
                        Logger.Log(s.Trim(), LogType.Info);
                }
            });

            if (exitCode == 0)
            {
                Logger.Log("💡 Perhatikan baris 'Last 5 characters of installed product key: XXXXX'.", LogType.Warning);
                Logger.Log("   Jika ada lebih dari 1 key atau ada lisensi 'Grace/Trial' kedaluwarsa, catat 5 karakternya untuk dihapus dengan opsi 2.", LogType.Warning);
            }
        }

        private async Task UninstallSpecificKeyPromptAsync()
        {
            string? keySuffix = InputDialog.Show(
                Form.ActiveForm,
                "Hapus Key Office Lama",
                "Masukkan 5 Karakter Terakhir Product Key yang ingin dihapus:\n(Contoh: WFG99 atau KHGM9)",
                ""
            );

            if (string.IsNullOrWhiteSpace(keySuffix))
            {
                Logger.Log("ℹ️ Input key kosong, pembatalan penghapusan key.", LogType.Info);
                return;
            }

            keySuffix = keySuffix.Trim().ToUpperInvariant();
            if (keySuffix.Length != 5)
            {
                Logger.Log($"⚠️ Format key '{keySuffix}' salah. Harus tepat 5 karakter (contoh: 2T8R8).", LogType.Warning);
                return;
            }

            string? ospp = LocateOsppVbs();
            if (ospp == null)
            {
                Logger.Log("❌ File OSPP.VBS tidak ditemukan.", LogType.Error);
                return;
            }

            Logger.Log($"Menghapus key dengan 5 karakter terakhir: {keySuffix}...", LogType.Info);
            int exitCode = await CommandRunner.RunCmdAsync($"cscript //nologo \"{ospp}\" /unpkey:{keySuffix}", s =>
            {
                if (!string.IsNullOrWhiteSpace(s)) Logger.Log(s.Trim(), LogType.Info);
            });

            if (exitCode == 0)
            {
                Logger.Log($"✅ Selesai memproses pencopotan key {keySuffix}.", LogType.Success);
                Logger.Log("Buka kembali Word atau Excel untuk memeriksa status aktivasi.", LogType.Info);
            }
            else
            {
                Logger.Log($"⚠️ Perintah unpkey selesai dengan kode {exitCode}.", LogType.Warning);
            }
        }

        private async Task ResetOfficeModernAuthCacheAsync()
        {
            Logger.Log("--- RESET CACHE TOKEN LOGIN & MODERN AUTHENTICATION ---", LogType.Info);
            Logger.Log("Menghentikan proses aplikasi Office aktif...", LogType.Info);

            await CommandRunner.RunCmdAsync("taskkill /f /im winword.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im excel.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im outlook.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im powerpnt.exe", null);
            await CommandRunner.RunCmdAsync("taskkill /f /im msosync.exe", null);

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // 1. Bersihkan OneAuth cache
            string oneAuthDir = Path.Combine(localAppData, @"Microsoft\OneAuth");
            if (Directory.Exists(oneAuthDir))
            {
                try
                {
                    Directory.Delete(oneAuthDir, true);
                    Logger.Log("✅ Cache Microsoft OneAuth berhasil dihapus.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"⚠️ OneAuth folder terkunci: {ex.Message}", LogType.Warning);
                }
            }

            // 2. Bersihkan IdentityCache
            string idCacheDir = Path.Combine(localAppData, @"Microsoft\IdentityCache");
            if (Directory.Exists(idCacheDir))
            {
                try
                {
                    Directory.Delete(idCacheDir, true);
                    Logger.Log("✅ Cache Microsoft IdentityCache berhasil dihapus.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"⚠️ IdentityCache folder terkunci: {ex.Message}", LogType.Warning);
                }
            }

            // 3. Bersihkan Credential Manager entri Office
            Logger.Log("Membersihkan kredensial Office lama di Windows Credential Manager...", LogType.Info);
            await CommandRunner.RunCmdAsync("cmdkey /list | findstr /i \"MicrosoftOffice\" > \"%TEMP%\\off_creds.txt\"", null);
            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "off_creds.txt");
                if (File.Exists(tempFile))
                {
                    var lines = await File.ReadAllLinesAsync(tempFile);
                    foreach (var line in lines)
                    {
                        var trimmed = line.Trim();
                        if (trimmed.StartsWith("Target:", StringComparison.OrdinalIgnoreCase))
                        {
                            string target = trimmed.Substring(7).Trim();
                            await CommandRunner.RunCmdAsync($"cmdkey /delete:\"{target}\"", null);
                        }
                    }
                    try { File.Delete(tempFile); } catch { }
                }
            }
            catch { }

            Logger.Log("✅ Cache login & token akun Office berhasil di-reset total.", LogType.Success);
            Logger.Log("Sekarang Anda dapat login kembali ke Microsoft 365 / Office tanpa tersangkut di akun lama!", LogType.Success);
        }
    }
}
