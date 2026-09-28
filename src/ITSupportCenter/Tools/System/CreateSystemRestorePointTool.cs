using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class CreateSystemRestorePointTool : IToolCommand
    {
        public string Id => "sys_create_restore_point";
        public string Title => "1-Klik Buat System Restore Point";
        public string Description => "Membuat titik pemulihan sistem (System Restore Point) Windows via PowerShell untuk perlindungan sebelum perbaikan/tweak.";
        public string Category => ToolCategory.System;
        public string Keywords => "restore point snapshot checkpoint computer system restore backup recovery proteksi";
        public string Icon => "🔄";
        public string ButtonText => "Buat System Restore Point";
        public Color ButtonColor => Color.FromArgb(39, 174, 96); // Nephritis Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMBUAT TITIK PEMULIHAN SISTEM (SYSTEM RESTORE POINT) ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("1. Memastikan System Protection aktif pada drive C:\\...", LogType.Info);
                await CommandRunner.RunPowerShellAsync("Enable-ComputerRestore -Drive 'C:\\' -ErrorAction SilentlyContinue", null);

                Logger.Log("2. Menyetel Registry agar mengizinkan pembuatan Restore Point frekuensi tinggi...", LogType.Info);
                WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", "SystemRestorePointCreationFrequency", 0);

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string desc = $"ITSupportCenter_AutoSnapshot_{DateTime.Now:yyyyMMdd_HHmm}";
                Logger.Log($"3. Membuat Checkpoint-Computer dengan label: '{desc}'...", LogType.Info);

                string psCmd = $"Checkpoint-Computer -Description '{desc}' -RestorePointType 'MODIFY_SETTINGS' -ErrorAction Stop";
                int exitCode = await CommandRunner.RunPowerShellAsync(psCmd, line =>
                {
                    if (!string.IsNullOrWhiteSpace(line)) Logger.Log($"   > {line.Trim()}", LogType.Info);
                });

                if (exitCode == 0)
                {
                    Logger.Log($"🎉 System Restore Point berhasil dibuat pada [{timestamp}]!", LogType.Success);
                    Logger.Log("Jika terjadi kendala di masa depan, sistem dapat dipulihkan melalui menu 'rstrui.exe' (System Restore).", LogType.Success);
                }
                else
                {
                    Logger.Log("⚠️ Pembuatan restore point memerlukan hak Administrator dan layanan VSS (Volume Shadow Copy) aktif.", LogType.Warning);
                }
            });
        }
    }
}
