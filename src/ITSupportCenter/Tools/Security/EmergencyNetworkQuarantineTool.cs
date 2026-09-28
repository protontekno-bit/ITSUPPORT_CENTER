using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Security
{
    public class EmergencyNetworkQuarantineTool : IToolCommand
    {
        public string Id => "sec_emergency_quarantine";
        public string Title => "Karantina Jaringan Darurat (Ransomware Air-Gap)";
        public string Description => "Mode isolasi darurat: 1-Klik memutus seluruh trafik masuk & keluar LAN/Internet saat PC terkena virus/ransomware agar tidak menular ke TrueNAS/PC lain.";
        public string Category => ToolCategory.Security;
        public string Keywords => "emergency quarantine air gap isolasi ransomware malware virus block all network disconnect cut off";
        public string Icon => "☣️";
        public string ButtonText => "Karantina / Buka Isolasi LAN";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Alizarin Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== KARANTINA JARINGAN DARURAT (EMERGENCY AIR-GAP ISOLATION) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // Cek apakah policy saat ini adalah blockoutbound
                string currentPolicy = "";
                await CommandRunner.RunCmdAsync("netsh advfirewall show allprofiles firewallpolicy", line => currentPolicy += line);

                bool isCurrentlyQuarantined = currentPolicy.Contains("BlockOutbound", StringComparison.OrdinalIgnoreCase);

                if (!isCurrentlyQuarantined)
                {
                    // AKTIFKAN KARANTINA TOTAL
                    Logger.Log("🚨 MENGAKTIFKAN MODE KARANTINA DARURAT...", LogType.Warning);
                    Logger.Log("Memblokir seluruh paket data Inbound & Outbound pada semua profil jaringan...", LogType.Warning);

                    await CommandRunner.RunCmdAsync("netsh advfirewall set allprofiles firewallpolicy blockinbound,blockoutbound", null);

                    Logger.Log("🛑 [TERISOLASI TOTAL] Seluruh akses LAN, Internet, dan File Sharing telah DIPUTUS!", LogType.Success);
                    Logger.Log("Virus / Ransomware pada PC ini tidak akan bisa menyebar ke komputer lain atau menghubungi server C&C.", LogType.Success);
                    Logger.Log("💡 Klik tombol ini lagi jika proses pembersihan malware selesai untuk membuka isolasi.", LogType.Info);
                }
                else
                {
                    // BUKA KARANTINA / NORMALISASI
                    Logger.Log("Status saat ini: Komputer sedang dalam status TERKARANTINA.", LogType.Info);
                    Logger.Log("Membuka karantina dan menormalkan kembali kebijakan firewall...", LogType.Info);

                    await CommandRunner.RunCmdAsync("netsh advfirewall set allprofiles firewallpolicy blockinbound,allowoutbound", null);

                    Logger.Log("✅ [ISOLASI DIBUKA] Kebijakan jaringan telah DINORMALKAN KEMBALI.", LogType.Success);
                    Logger.Log("Koneksi internet dan LAN komputer telah pulih.", LogType.Success);
                }
            });
        }
    }
}
