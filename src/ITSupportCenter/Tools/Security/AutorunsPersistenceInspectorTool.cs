using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.Security
{
    public class AutorunsPersistenceInspectorTool : IToolCommand
    {
        public string Id => "sec_autoruns_persistence_inspector";
        public string Title => "Inspeksi Pembajakan Startup & Persistence (Sysinternals Lite)";
        public string Description => "Deteksi malware & script terselubung di titik persistensi kritis: Winlogon Shell/Userinit hijack, IFEO debugger hijacking, dan Registry Run startup.";
        public string Category => ToolCategory.Security;
        public string Keywords => "autoruns sysinternals persistence hijack winlogon ifeo registry run startup malware trojan virus";
        public string Icon => "🕵️";
        public string ButtonText => "Inspeksi Autoruns & Pembajakan";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== DETEKTOR PEMBAJAKAN PERSISTENSI STARTUP (AUTORUNS LITE) ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Inspeksi Autoruns & Persistence",
                "Pilih Tindakan Diagnostik:\n" +
                "1 = Pindai Titik Persistensi Kritis (Winlogon, IFEO, Registry Run)\n" +
                "2 = Pulihkan Nilai Winlogon Shell & Userinit ke Standar Windows\n" +
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
                    ScanPersistencePoints();
                }
                else if (choice.Trim() == "2")
                {
                    RestoreWinlogonDefaults();
                }
            });
        }

        private static void ScanPersistencePoints()
        {
            Logger.Log("\n[1/3] Memeriksa Winlogon Integrity (Titik Pembajakan Favorit Trojan)...", LogType.Info);
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
                if (key != null)
                {
                    string shell = key.GetValue("Shell")?.ToString() ?? "";
                    string userinit = key.GetValue("Userinit")?.ToString() ?? "";

                    if (shell.Equals("explorer.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Log($"✅ Winlogon Shell: '{shell}' (Normal/Aman)", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"🚨 [WASPADA] Winlogon Shell dibajak! Nilai saat ini: '{shell}'", LogType.Error);
                        Logger.Log("   Gunakan opsi 2 untuk mengembalikannya ke 'explorer.exe'.", LogType.Warning);
                    }

                    if (userinit.Contains("userinit.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Log($"✅ Winlogon Userinit: '{userinit}' (Normal/Aman)", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"🚨 [WASPADA] Winlogon Userinit mencurigakan: '{userinit}'", LogType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"⚠️ Gagal membaca Winlogon: {ex.Message}", LogType.Warning);
            }

            Logger.Log("\n[2/3] Memeriksa Image File Execution Options (IFEO Debugger Hijacking)...", LogType.Info);
            try
            {
                using var ifeoKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options");
                if (ifeoKey != null)
                {
                    int hijackedCount = 0;
                    foreach (var subName in ifeoKey.GetSubKeyNames())
                    {
                        using var sub = ifeoKey.OpenSubKey(subName);
                        var debugger = sub?.GetValue("Debugger");
                        if (debugger != null)
                        {
                            Logger.Log($"🚨 [HIJACKED] {subName} dialihkan debugger ke: {debugger}", LogType.Error);
                            hijackedCount++;
                        }
                    }
                    if (hijackedCount == 0)
                    {
                        Logger.Log("✅ Tidak ditemukan pembajakan proses pada IFEO (Bersih).", LogType.Success);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"⚠️ Gagal memeriksa IFEO: {ex.Message}", LogType.Warning);
            }

            Logger.Log("\n[3/3] Membaca Entri Autostart Registry Run (HKLM & HKCU)...", LogType.Info);
            ScanRunKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "HKLM Run");
            ScanRunKey(Registry.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Run", "HKCU Run");

            Logger.Log("\n🎉 [SELESAI] Pemindaian persistensi startup selesai.", LogType.Success);
        }

        private static void ScanRunKey(RegistryKey root, string subPath, string label)
        {
            try
            {
                using var key = root.OpenSubKey(subPath);
                if (key != null)
                {
                    var names = key.GetValueNames();
                    Logger.Log($"--- {label} ({names.Length} entri ditemukan) ---", LogType.Info);
                    foreach (var name in names)
                    {
                        string val = key.GetValue(name)?.ToString() ?? "";
                        Logger.Log($"  • {name,-22} ➔ {val}", LogType.Info);
                    }
                }
            }
            catch { }
        }

        private static void RestoreWinlogonDefaults()
        {
            Logger.Log("Mereset Winlogon Shell & Userinit ke nilai default bawaan Microsoft...", LogType.Info);
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
                if (key != null)
                {
                    key.SetValue("Shell", "explorer.exe", RegistryValueKind.String);
                    key.SetValue("Userinit", @"C:\Windows\system32\userinit.exe,", RegistryValueKind.String);
                    Logger.Log("✅ Winlogon Shell berhasil di-reset ke: explorer.exe", LogType.Success);
                    Logger.Log(@"✅ Winlogon Userinit berhasil di-reset ke: C:\Windows\system32\userinit.exe,", LogType.Success);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ Gagal merestore Winlogon: {ex.Message}", LogType.Error);
            }
        }
    }
}
