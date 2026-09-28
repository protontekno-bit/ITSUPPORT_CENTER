using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Printer
{
    public class FixPrinterSharingTool : IToolCommand
    {
        public string Id => "fix_printer_sharing";
        public string Title => "Fix Printer Sharing 0x0000011b";
        public string Description => "Nonaktifkan RPC Auth Privacy & Bypass Point and Print Driver Policy agar client bisa connect.";
        public string Category => ToolCategory.Printer;
        public string Keywords => "printer spooler cetak 0x0000011b rpc driver point and print";
        public string Icon => "🖨️";
        public string ButtonText => "Fix Printer";
        public Color ButtonColor => Color.FromArgb(46, 204, 113);

        public async Task ExecuteAsync()
        {
            Logger.Log("Memulai perbaikan Printer Sharing Error 0x0000011b...");
            await Task.Run(() =>
            {
                try
                {
                    // 1. Set RPC Auth Privacy = 0
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Print", "RpcAuthnLevelPrivacyEnabled", 0);
                    Logger.Log(@"Registry HKLM\System\CurrentControlSet\Control\Print -> RpcAuthnLevelPrivacyEnabled = 0", LogType.Success);

                    // 2. Set Point and Print Policy = 0
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"Software\Policies\Microsoft\Windows NT\Printers\PointAndPrint", "RestrictDriverInstallationToAdministrators", 0);
                    Logger.Log(@"Registry PointAndPrint -> RestrictDriverInstallationToAdministrators = 0", LogType.Success);

                    // 3. Restart Print Spooler Service
                    WindowsHelper.RestartService("spooler", "Print Spooler");
                    Logger.Log("Perbaikan printer selesai! Komputer client sekarang dapat menghubungkan printer.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log("Gagal memperbaiki printer sharing: " + ex.Message, LogType.Error);
                }
            });
        }
    }
}
