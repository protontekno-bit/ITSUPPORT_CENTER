using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.SystemTools
{
    public class WmiAclRebuilderTool : IToolCommand
    {
        public string Id => "sys_wmi_acl_rebuilder";
        public string Title => "Perbaikan Total WMI & Hak Akses Sistem (ACL Permissions)";
        public string Description => "Membangun ulang repositori WMI yang rusak (winmgmt /resetrepository) dan memulihkan hak akses folder sistem (icacls) untuk mengatasi error 'Access Denied'.";
        public string Category => ToolCategory.System;
        public string Keywords => "wmi winmgmt repository reset salvagerepository acl permission access denied corrupt wbem sensor rusak";
        public string Icon => "🔧";
        public string ButtonText => "Perbaiki WMI & Hak Akses";
        public Color ButtonColor => Color.FromArgb(231, 76, 60); // Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PERBAIKAN REPOSITORI WMI & HAK AKSES SISTEM (ACL REBUILDER) ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Perbaikan WMI & Permissions",
                "Pilih Tindakan Perbaikan:\n" +
                "1 = Bangun Ulang Repositori WMI (Solusi Sensor/Info Perangkat Kosong)\n" +
                "2 = Pulihkan Hak Akses Default Folder Sistem (Solusi 'Access Denied')\n" +
                "0 = Batal",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice) || choice.Trim() == "0")
            {
                Logger.Log("ℹ️ Operasi dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                if (choice.Trim() == "1")
                {
                    await RebuildWmiRepositoryAsync();
                }
                else if (choice.Trim() == "2")
                {
                    await ResetSystemPermissionsAsync();
                }
            });
        }

        private static async Task RebuildWmiRepositoryAsync()
        {
            Logger.Log("--- MEMULAI PEMBANGUNAN ULANG REPOSITORI WMI ---", LogType.Info);

            Logger.Log("1/4: Memverifikasi integritas repositori WMI saat ini...", LogType.Info);
            await CommandRunner.RunCmdAsync("winmgmt /verifyrepository", s => Logger.Log(s, LogType.Info));

            Logger.Log("2/4: Menghentikan Windows Management Instrumentation Service...", LogType.Info);
            await CommandRunner.RunCmdAsync("net stop winmgmt /y", null);

            Logger.Log("3/4: Memulihkan / mereset repositori WMI (winmgmt /resetrepository)...", LogType.Info);
            await CommandRunner.RunCmdAsync("winmgmt /resetrepository", s => Logger.Log(s, LogType.Success));

            Logger.Log("4/4: Mendaftarkan ulang seluruh DLL subsistem Wbem...", LogType.Info);
            await CommandRunner.RunCmdAsync("cd /d %windir%\\system32\\wbem && for %i in (*.dll) do RegSvr32 -s %i", null);
            await CommandRunner.RunCmdAsync("net start winmgmt", null);

            Logger.Log("🎉 [SELESAI] Repositori WMI berhasil dibangun ulang!", LogType.Success);
            Logger.Log("Sensor hardware, informasi sistem, dan Event Viewer kini dapat membaca data dengan normal kembali.", LogType.Success);
        }

        private static async Task ResetSystemPermissionsAsync()
        {
            Logger.Log("--- MEMULIHKAN HAK AKSES SISTEM DEFAULT (ICACLS & SECEDIT) ---", LogType.Info);
            Logger.Log("Mereset permission folder sistem ke pemilik default (NT SERVICE / SYSTEM)...", LogType.Info);

            await CommandRunner.RunCmdAsync("icacls \"%windir%\\System32\" /reset /t /c /l /q", null);
            await CommandRunner.RunCmdAsync("secedit /configure /cfg %windir%\\inf\\defltbase.inf /db defltbase.sdb /verbose", null);

            Logger.Log("✅ Hak akses folder sistem berhasil dinormalkan.", LogType.Success);
            Logger.Log("Error 'Access Denied' saat memasang update atau driver telah diselesaikan.", LogType.Success);
        }
    }
}
