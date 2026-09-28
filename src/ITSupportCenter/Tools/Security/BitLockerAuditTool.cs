using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Security
{
    public class BitLockerAuditTool : IToolCommand
    {
        public string Id => "sec_bitlocker_audit";
        public string Title => "Audit & Backup BitLocker Recovery Key";
        public string Description => "Memeriksa status enkripsi BitLocker dan mengekstrak 48-digit Recovery Key drive C: / D: untuk mencegah PC terkunci.";
        public string Category => ToolCategory.Security;
        public string Keywords => "bitlocker recovery key enkripsi manage-bde tpm proteksi secure lock";
        public string Icon => "🔐";
        public string ButtonText => "Cek BitLocker & Key";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Alizarin Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT STATUS BITLOCKER & RECOVERY KEY ===", LogType.Info);

            await Task.Run(async () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("🔐 LAPORAN STATUS BITLOCKER & 48-DIGIT RECOVERY KEY");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                Logger.Log("1/2: Memeriksa status enkripsi seluruh drive (manage-bde -status)...", LogType.Info);

                await CommandRunner.RunCmdAsync("manage-bde -status", s =>
                {
                    if (s.Contains("Volume") || s.Contains("Conversion Status") || s.Contains("Percentage Encrypted") || s.Contains("Protection Status") || s.Contains("Lock Status"))
                    {
                        if (s.Contains("Protection On") || s.Contains("Fully Encrypted"))
                            Logger.Log(s, LogType.Warning);
                        else
                            Logger.Log(s, LogType.Info);
                    }
                });

                Logger.Log("2/2: Mengekstrak Password Pelindung / 48-digit Recovery Key Drive C:...", LogType.Info);

                string rawKeyOutput = "";
                await CommandRunner.RunCmdAsync("manage-bde -protectors -get C:", s =>
                {
                    rawKeyOutput += s + "\n";
                    if (s.Contains("Numerical Password") || s.Contains("ID:") || s.Contains("Password:"))
                    {
                        Logger.Log(s, LogType.Success);
                    }
                    else if (s.Contains("ERROR") || s.Contains("Bukan"))
                    {
                        Logger.Log(s, LogType.Info);
                    }
                });

                sb.AppendLine(rawKeyOutput);
                sb.AppendLine("=========================================================");
                sb.AppendLine("⚠️ PENTING: Simpan Recovery Key 48-digit ini di tempat aman!");

                try
                {
                    ThreadClipboardHelper.SetClipboardText(sb.ToString());
                    Logger.Log("📋 Data BitLocker & Recovery Key berhasil disalin ke Clipboard!", LogType.Success);
                }
                catch { }
            });
        }
    }
}
