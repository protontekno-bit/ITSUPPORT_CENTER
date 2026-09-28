using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class ToggleFirewallProfilesTool : IToolCommand
    {
        public string Id => "sec_toggle_firewall_profiles";
        public string Title => "Saklar Profil Firewall (Domain, Private, Public)";
        public string Description => "Mengaudit status ketiga profil firewall (Domain, Private, Public) dan menyediakan saklar On/Off proteksi per profil.";
        public string Category => ToolCategory.Security;
        public string Keywords => "firewall profile domain private public state on off disable enable status audit";
        public string Icon => "🎛️";
        public string ButtonText => "Saklar Status Profil Firewall";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Wisteria Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT & PENGATUR STATUS PROFIL WINDOWS FIREWALL ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("1. Membaca status seluruh profil firewall saat ini...", LogType.Info);

                string auditOutput = "";
                await CommandRunner.RunCmdAsync("netsh advfirewall show allprofiles state", line =>
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        auditOutput += line + "\n";
                        Logger.Log($"   {line.Trim()}", LogType.Info);
                    }
                });

                bool anyProfileOff = auditOutput.Contains("OFF", StringComparison.OrdinalIgnoreCase);

                if (anyProfileOff)
                {
                    // AKSI: AKTIFKAN SELURUH PROFIL
                    Logger.Log("\n2. Terdeteksi profil firewall nonaktif. Mengaktifkan proteksi penuh seluruh profil...", LogType.Info);
                    await CommandRunner.RunCmdAsync("netsh advfirewall set allprofiles state on", null);
                    Logger.Log("🛡️ [PROTEKSI PENUH] Seluruh profil Firewall (Domain, Private, Public) kini TELAH DIAKTIFKAN (ON).", LogType.Success);
                }
                else
                {
                    // AKSI: NONAKTIFKAN SEMENTARA (UNTUK TESTING)
                    Logger.Log("\n2. Seluruh profil saat ini AKTIF. Menonaktifkan sementara untuk pengujian jaringan kantor...", LogType.Warning);
                    await CommandRunner.RunCmdAsync("netsh advfirewall set allprofiles state off", null);
                    Logger.Log("⚠️ [PROTEKSI MATI] Seluruh profil Windows Firewall telah DINONAKTIFKAN (OFF).", LogType.Warning);
                    Logger.Log("💡 Catatan: Jangan lupa mengaktifkannya kembali setelah pengujian selesai!", LogType.Info);
                }
            });
        }
    }
}
