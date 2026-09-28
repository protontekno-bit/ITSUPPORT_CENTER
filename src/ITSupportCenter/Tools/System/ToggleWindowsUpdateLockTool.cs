using System;
using System.Drawing;
using System.ServiceProcess;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.SystemTools
{
    public class ToggleWindowsUpdateLockTool : IToolCommand
    {
        public string Id => "sys_toggle_win_update_lock";
        public string Title => "Kunci / Buka Windows Update Permanen";
        public string Description => "Saklar opsional untuk mematikan Windows Update secara permanen (kunci service & policy) atau mengaktifkannya kembali ke normal saat dibutuhkan.";
        public string Category => ToolCategory.System;
        public string Keywords => "windows update lock disable permanent matikan update stop wuauserv gpo noautoupdate enable buka";
        public string Icon => "🔒";
        public string ButtonText => "Kunci / Buka Windows Update";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Alizarin Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATUR STATUS WINDOWS UPDATE (KUNCI / BUKA PERMANEN) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // Cek status service wuauserv saat ini
                bool isCurrentlyDisabled = false;
                try
                {
                    using var sc = new ServiceController("wuauserv");
                    isCurrentlyDisabled = (sc.StartType == ServiceStartMode.Disabled);
                }
                catch { }

                if (!isCurrentlyDisabled)
                {
                    // AKSI: KUNCI & MATIKAN PERMANEN
                    Logger.Log("Status saat ini: Windows Update AKTIF.", LogType.Info);
                    Logger.Log("Menerapkan penguncian total Windows Update (Disable Service & GPO Policy)...", LogType.Warning);

                    // 1. Matikan dan disable services
                    string[] services = { "wuauserv", "WaaSMedicSvc", "UsoSvc", "bits" };
                    foreach (var s in services)
                    {
                        await CommandRunner.RunCmdAsync($"net stop {s} /y & sc config {s} start= disabled", null);
                    }

                    // 2. Terapkan GPO Registry NoAutoUpdate
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 1);
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "AUOptions", 1);
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DisableWindowsUpdateAccess", 1);

                    Logger.Log("🛑 [TERKUNCI] Windows Update telah DINONAKTIFKAN SECARA PERMANEN.", LogType.Success);
                    Logger.Log("Komputer tidak akan mengunduh atau memasang update otomatis di latar belakang.", LogType.Info);
                    Logger.Log("💡 Catatan: Klik tombol ini lagi kapan saja jika ingin mengaktifkan kembali.", LogType.Info);
                }
                else
                {
                    // AKSI: BUKA KUNCI & NORMALISASI
                    Logger.Log("Status saat ini: Windows Update TERKUNCI (Disabled).", LogType.Info);
                    Logger.Log("Membuka kunci dan menormalkan kembali layanan Windows Update...", LogType.Info);

                    // 1. Hapus GPO Registry Lock
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", true);
                        if (key != null)
                        {
                            key.DeleteValue("NoAutoUpdate", false);
                            key.DeleteValue("AUOptions", false);
                        }
                    }
                    catch { }

                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", true);
                        if (key != null) key.DeleteValue("DisableWindowsUpdateAccess", false);
                    }
                    catch { }

                    // 2. Kembalikan mode start service
                    await CommandRunner.RunCmdAsync("sc config wuauserv start= demand", null);
                    await CommandRunner.RunCmdAsync("sc config bits start= delayed-auto", null);
                    await CommandRunner.RunCmdAsync("sc config UsoSvc start= demand", null);
                    await CommandRunner.RunCmdAsync("sc config WaaSMedicSvc start= demand", null);

                    await CommandRunner.RunCmdAsync("net start wuauserv", null);
                    await CommandRunner.RunCmdAsync("net start bits", null);

                    Logger.Log("✅ [TERBUKA] Layanan Windows Update telah DINORMALKAN KEMBALI.", LogType.Success);
                    Logger.Log("Komputer kini dapat memeriksa dan mengunduh pembaruan Microsoft secara normal.", LogType.Success);
                }
            });
        }
    }
}
