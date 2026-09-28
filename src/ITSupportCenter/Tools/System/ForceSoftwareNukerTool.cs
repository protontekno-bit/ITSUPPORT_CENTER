using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.SystemTools
{
    public class ForceSoftwareNukerTool : IToolCommand
    {
        public string Id => "sys_force_software_nuker";
        public string Title => "Penghapus Paksa Program Macet (Force Uninstaller)";
        public string Description => "Hapus paksa aplikasi yang uninstaller-nya rusak, hilang, atau memunculkan pesan error saat dicopot dari Control Panel.";
        public string Category => ToolCategory.System;
        public string Keywords => "uninstall force nuker remove program rusak uninstaller corrupt registry clean stubborn geekuninstaller";
        public string Icon => "🗑️";
        public string ButtonText => "Penghapus Paksa Software";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Dark Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGHAPUS PAKSA SOFTWARE MACET (FORCE UNINSTALLER) ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Force Software Uninstaller",
                "Pilih Tindakan:\n" +
                "1 = Pindai & Cari Program Terdaftar di Registry (Filter Nama)\n" +
                "2 = Hapus Paksa Entri Registry Program yang Rusak / Nyangkut\n" +
                "0 = Batal",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice) || choice.Trim() == "0")
            {
                Logger.Log("ℹ️ Operasi dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(() =>
            {
                if (choice.Trim() == "1")
                {
                    SearchInstalledApps();
                }
                else if (choice.Trim() == "2")
                {
                    ForceDeleteAppRegistry();
                }
            });
        }

        private static void SearchInstalledApps()
        {
            string? keyword = InputDialog.Show(
                Form.ActiveForm,
                "Cari Nama Aplikasi",
                "Masukkan sebagian nama software yang ingin dicari (Kosongkan untuk menampilkan 30 teratas):",
                ""
            );

            Logger.Log("Memindai database instalasi Windows...", LogType.Info);
            var apps = GetInstalledSoftware(keyword ?? "");

            Logger.Log($"--- DITEMUKAN {apps.Count} PROGRAM ---", LogType.Success);
            int displayCount = 0;
            foreach (var a in apps)
            {
                Logger.Log($"• {a.DisplayName} (Registry Key: {a.KeyName})", LogType.Info);
                displayCount++;
                if (displayCount >= 40)
                {
                    Logger.Log($"... dan {apps.Count - 40} program lainnya. Gunakan kata kunci lebih spesifik jika belum terlihat.", LogType.Warning);
                    break;
                }
            }
            Logger.Log("💡 Catat nama Registry Key untuk dihapus paksa di menu Opsi 2.", LogType.Info);
        }

        private static void ForceDeleteAppRegistry()
        {
            string? targetKey = InputDialog.Show(
                Form.ActiveForm,
                "Hapus Paksa Entri Registry",
                "Masukkan persis nama Registry Key yang ingin dihapus paksa dari sistem:",
                ""
            );

            if (string.IsNullOrWhiteSpace(targetKey)) return;

            string[] paths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            bool removed = false;
            foreach (var p in paths)
            {
                try
                {
                    using var parent = Registry.LocalMachine.OpenSubKey(p, true);
                    if (parent != null)
                    {
                        var subKeys = parent.GetSubKeyNames();
                        foreach (var sk in subKeys)
                        {
                            if (sk.Equals(targetKey.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                parent.DeleteSubKeyTree(sk);
                                Logger.Log($"✅ Berhasil menghapus entri registry HKLM\\{p}\\{sk}!", LogType.Success);
                                removed = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"⚠️ Error saat menghapus: {ex.Message}", LogType.Warning);
                }
            }

            if (removed)
            {
                Logger.Log("🎉 Program telah berhasil dihapus dari daftar Installed Programs Windows.", LogType.Success);
                Logger.Log("Anda sekarang dapat menginstal ulang versi baru tanpa diblokir oleh instalasi lama yang rusak.", LogType.Success);
            }
            else
            {
                Logger.Log($"⚠️ Tidak ditemukan key registry '{targetKey}'. Pastikan penulisan key sudah tepat.", LogType.Warning);
            }
        }

        private static List<(string DisplayName, string KeyName)> GetInstalledSoftware(string filter)
        {
            var list = new List<(string DisplayName, string KeyName)>();
            string[] paths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var p in paths)
            {
                try
                {
                    using var parent = Registry.LocalMachine.OpenSubKey(p);
                    if (parent != null)
                    {
                        foreach (var sub in parent.GetSubKeyNames())
                        {
                            using var k = parent.OpenSubKey(sub);
                            var name = k?.GetValue("DisplayName")?.ToString();
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                if (string.IsNullOrWhiteSpace(filter) || name.Contains(filter, StringComparison.OrdinalIgnoreCase))
                                {
                                    list.Add((name, sub));
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            return list;
        }
    }
}
