using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class ToggleOfficeUpdateLockTool : IToolCommand
    {
        public string Id => "lic_toggle_office_update_lock";
        public string Title => "Kunci / Buka Update Microsoft Office Permanen";
        public string Description => "Saklar opsional untuk mematikan update otomatis Microsoft Office (2016/2019/2021/365 Click-to-Run) secara permanen guna mencegah error aktivasi, lisensi terganggu, atau bug mendadak.";
        public string Category => ToolCategory.License;
        public string Keywords => "office update lock disable permanent matikan update office word excel clicktorun c2r c2rclient enable buka lisensi";
        public string Icon => "🔒";
        public string ButtonText => "Kunci / Buka Update Office";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Alizarin Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATUR STATUS UPDATE MICROSOFT OFFICE (KUNCI / BUKA PERMANEN) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // 1. Cek status update Office saat ini
                bool isCurrentlyDisabled = false;
                try
                {
                    // Cek Policy Registry
                    using var polKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate");
                    var autoVal = polKey?.GetValue("EnableAutomaticUpdates");
                    if (autoVal != null && Convert.ToInt32(autoVal) == 0)
                    {
                        isCurrentlyDisabled = true;
                    }

                    // Cek ClickToRun Configuration
                    using var c2rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration");
                    var updStr = c2rKey?.GetValue("UpdatesEnabled")?.ToString();
                    if (!string.IsNullOrEmpty(updStr) && updStr.Equals("False", StringComparison.OrdinalIgnoreCase))
                    {
                        isCurrentlyDisabled = true;
                    }
                }
                catch { }

                if (!isCurrentlyDisabled)
                {
                    // AKSI: KUNCI & MATIKAN PERMANEN
                    Logger.Log("Status saat ini: Update Otomatis Office AKTIF.", LogType.Info);
                    Logger.Log("Menerapkan penguncian total Update Office (Registry Policy & Scheduled Tasks)...", LogType.Warning);

                    // 1. Set ClickToRun Configuration UpdatesEnabled = False
                    try
                    {
                        using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration");
                        key.SetValue("UpdatesEnabled", "False", RegistryValueKind.String);
                    }
                    catch
                    {
                        await CommandRunner.RunCmdAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration\" /v UpdatesEnabled /t REG_SZ /d \"False\" /f", null);
                    }

                    // 2. Set GPO Policy Office 16.0 (Office 2016, 2019, 2021, 365)
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate", "EnableAutomaticUpdates", 0);
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate", "HideEnableDisableUpdates", 1);

                    // 3. Set GPO Policy Office 15.0 (Office 2013 fallback)
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Office\15.0\Common\OfficeUpdate", "EnableAutomaticUpdates", 0);

                    // 4. Matikan Scheduled Task pembaruan Office di Task Scheduler
                    Logger.Log("Menonaktifkan Task Scheduler background pembaruan Office...", LogType.Info);
                    await CommandRunner.RunCmdAsync("schtasks /change /tn \"\\Microsoft\\Office\\Office Automatic Updates 2.0\" /disable", null);
                    await CommandRunner.RunCmdAsync("schtasks /change /tn \"\\Microsoft\\Office\\Office Feature Updates\" /disable", null);
                    await CommandRunner.RunCmdAsync("schtasks /change /tn \"\\Microsoft\\Office\\Office Feature Updates Logon\" /disable", null);

                    Logger.Log("🛑 [TERKUNCI] Update Otomatis Microsoft Office telah DINONAKTIFKAN SECARA PERMANEN.", LogType.Success);
                    Logger.Log("🛡️ Manfaat:", LogType.Success);
                    Logger.Log("  • Mencegah notifikasi 'Your license is not genuine' akibat update Microsoft.");
                    Logger.Log("  • Menjaga kestabilan aktivasi KMS/Volume dan macro Excel/VBA.");
                    Logger.Log("  • Menghemat kuota bandwidth internet kantor (tidak mengunduh data 2-3 GB tiba-tiba).");
                    Logger.Log("💡 Catatan: Klik tombol ini lagi kapan saja jika ingin mengaktifkan update kembali.", LogType.Info);
                }
                else
                {
                    // AKSI: BUKA KUNCI & NORMALISASI
                    Logger.Log("Status saat ini: Update Otomatis Office TERKUNCI (Disabled).", LogType.Info);
                    Logger.Log("Membuka kunci dan mengembalikan update Microsoft Office ke normal...", LogType.Info);

                    // 1. Set ClickToRun Configuration UpdatesEnabled = True
                    try
                    {
                        using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Office\ClickToRun\Configuration");
                        key.SetValue("UpdatesEnabled", "True", RegistryValueKind.String);
                    }
                    catch
                    {
                        await CommandRunner.RunCmdAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\Office\\ClickToRun\\Configuration\" /v UpdatesEnabled /t REG_SZ /d \"True\" /f", null);
                    }

                    // 2. Hapus batasan GPO Policy Office
                    try
                    {
                        using var key16 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Office\16.0\Common\OfficeUpdate", true);
                        if (key16 != null)
                        {
                            key16.DeleteValue("EnableAutomaticUpdates", false);
                            key16.DeleteValue("HideEnableDisableUpdates", false);
                        }
                    }
                    catch { }

                    try
                    {
                        using var key15 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Office\15.0\Common\OfficeUpdate", true);
                        if (key15 != null) key15.DeleteValue("EnableAutomaticUpdates", false);
                    }
                    catch { }

                    // 3. Aktifkan kembali Scheduled Task
                    await CommandRunner.RunCmdAsync("schtasks /change /tn \"\\Microsoft\\Office\\Office Automatic Updates 2.0\" /enable", null);

                    Logger.Log("✅ [TERBUKA] Layanan Update Microsoft Office telah DINORMALKAN KEMBALI.", LogType.Success);
                    Logger.Log("Microsoft Office kini dapat memeriksa dan mengunduh pembaruan resmi Microsoft secara normal.", LogType.Success);
                }
            });
        }
    }
}
