using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class EnableDotNet35DismTool : IToolCommand
    {
        public string Id => "sys_enable_dotnet35_dism";
        public string Title => "Aktifkan .NET 3.5 / 2.0 (DISM)";
        public string Description => "Mengaktifkan fitur .NET Framework 3.5 & 2.0 via DISM Online untuk kompatibilitas software akuntansi, faktur, dan aplikasi kantor lawas.";
        public string Category => ToolCategory.System;
        public string Keywords => "dotnet netfx3 .net 3.5 2.0 dism akuntansi faktur aplikasi lama legacy feature framework";
        public string Icon => "🔌";
        public string ButtonText => "Aktifkan .NET Framework 3.5";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt Dark

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGAKTIFAN .NET FRAMEWORK 3.5 / 2.0 (DISM ONLINE) ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Menjalankan DISM untuk mengunduh dan mengaktifkan NetFx3...", LogType.Info);
                Logger.Log("Proses ini membutuhkan koneksi internet (Microsoft Windows Update servers)...", LogType.Info);

                string cmd = "dism.exe /online /enable-feature /featurename:NetFx3 /all /norestart";
                int exitCode = await CommandRunner.RunCmdAsync(cmd, line =>
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Logger.Log($"   > {line.Trim()}", LogType.Info);
                    }
                });

                if (exitCode == 0)
                {
                    Logger.Log("🎉 .NET Framework 3.5 / 2.0 berhasil diaktifkan pada sistem ini!", LogType.Success);
                    Logger.Log("Software akuntansi, faktur, dan aplikasi legacy sekarang dapat berjalan normal.", LogType.Success);
                }
                else if (exitCode == 3010) // Success with restart needed
                {
                    Logger.Log("✅ .NET Framework 3.5 berhasil dipasang. Diperlukan restart komputer untuk menyelesaikan.", LogType.Success);
                }
                else
                {
                    Logger.Log($"⚠️ Pengaktifan DISM menghasilkan kode: {exitCode}. Periksa koneksi internet atau gunakan installer media offline jika Windows Update dinonaktifkan.", LogType.Warning);
                }
            });
        }
    }
}
