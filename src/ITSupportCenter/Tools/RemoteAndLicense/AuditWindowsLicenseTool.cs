using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class AuditWindowsLicenseTool : IToolCommand
    {
        public string Id => "license_audit_status";
        public string Title => "Audit Status & Masa Aktif Lisensi Windows";
        public string Description => "Memeriksa status aktivasi resmi Windows, edisi lisensi (OEM/Retail/Volume/KMS), dan tanggal kedaluwarsa.";
        public string Category => ToolCategory.License;
        public string Keywords => "license lisensi windows aktivasi slmgr expiration kms retail oem key";
        public string Icon => "📜";
        public string ButtonText => "Cek Lisensi Windows";
        public Color ButtonColor => Color.FromArgb(127, 140, 141); // Asbestos Grey

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT LISENSI & AKTIVASI WINDOWS ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Memeriksa detail lisensi via slmgr.vbs /dli...");
                await CommandRunner.RunCmdAsync("cscript //nologo %windir%\\system32\\slmgr.vbs /dli", s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        if (s.Contains("Licensed") || s.Contains("License Status: Licensed"))
                            Logger.Log(s, LogType.Success);
                        else if (s.Contains("Notification") || s.Contains("Unlicensed"))
                            Logger.Log(s, LogType.Error);
                        else
                            Logger.Log(s, LogType.Info);
                    }
                });

                Logger.Log("Memeriksa masa kedaluwarsa via slmgr.vbs /xpr...");
                await CommandRunner.RunCmdAsync("cscript //nologo %windir%\\system32\\slmgr.vbs /xpr", s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        if (s.Contains("permanently activated"))
                            Logger.Log("✅ Windows diaktivasi secara PERMANEN (Permanent Retail/OEM).", LogType.Success);
                        else
                            Logger.Log(s, LogType.Info);
                    }
                });
            });
        }
    }
}
