using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class ResetWindowsFirewallTool : IToolCommand
    {
        public string Id => "sec_reset_firewall";
        public string Title => "Reset Total Windows Firewall ke Default Pabrik";
        public string Description => "Membersihkan seluruh aturan firewall yang rusak/bentrok dan mengembalikan konfigurasi Windows Firewall ke standar pabrik (netsh advfirewall reset).";
        public string Category => ToolCategory.Security;
        public string Keywords => "firewall reset default factory netsh advfirewall restore repair mpssvc rule";
        public string Icon => "🔄";
        public string ButtonText => "Reset Total Windows Firewall";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Pomegranate Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESET TOTAL WINDOWS FIREWALL KE KONDISI PABRIK ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("1. Menghentikan service sementara dan mereset basis data firewall...", LogType.Info);

                int exitCode = await CommandRunner.RunCmdAsync("netsh advfirewall reset", line =>
                {
                    if (!string.IsNullOrWhiteSpace(line)) Logger.Log($"   > {line.Trim()}", LogType.Info);
                });

                if (exitCode == 0)
                {
                    Logger.Log("2. Memastikan service Windows Defender Firewall (mpssvc) aktif...", LogType.Info);
                    await CommandRunner.RunCmdAsync("sc config mpssvc start= auto & net start mpssvc", null);

                    Logger.Log("🎉 Windows Firewall berhasil DIRESET TOTAL ke kondisi default pabrik!", LogType.Success);
                    Logger.Log("Seluruh aturan lama yang bentrok/rusak telah dibersihkan.", LogType.Success);
                }
                else
                {
                    Logger.Log($"⚠️ Gagal mereset firewall (Kode keluar: {exitCode}). Pastikan aplikasi berjalan sebagai Administrator.", LogType.Warning);
                }
            });
        }
    }
}
