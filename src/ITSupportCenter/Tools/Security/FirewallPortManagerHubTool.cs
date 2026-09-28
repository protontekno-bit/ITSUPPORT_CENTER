using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.Security
{
    public class FirewallPortManagerHubTool : IToolCommand
    {
        public string Id => "sec_firewall_port_manager";
        public string Title => "Pusat Buka / Tutup Port Firewall Kantor";
        public string Description => "Membuka atau menutup port layanan umum kantor (RDP 3389, Web 80/443, Database SQL 1433/3306) serta port kustom secara terorganisir.";
        public string Category => ToolCategory.Security;
        public string Keywords => "firewall port rdp 3389 web 80 443 sql 1433 mysql 3306 open allow block rule custom";
        public string Icon => "🚪";
        public string ButtonText => "Kelola Port Layanan Kantor";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Carrot Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT MANAJEMEN PORT LAYANAN FIREWALL KANTOR ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Pusat Kelola Port Firewall",
                "Pilih Layanan yang Ingin Dibuka / Dikelola:\n" +
                "1 = Remote Desktop (RDP TCP 3389)\n" +
                "2 = Web Server (HTTP 80 & HTTPS 443)\n" +
                "3 = Database Server (MS SQL 1433 & MySQL 3306)\n" +
                "4 = Masukkan Nomor Port Kustom Sendiri",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice))
            {
                Logger.Log("ℹ️ Operasi dibatalkan oleh pengguna.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                if (choice == "1")
                {
                    Logger.Log("Membuka Port Remote Desktop (TCP 3389)...", LogType.Info);
                    string cmd = "netsh advfirewall firewall add rule name=\"IT_ALLOW_RDP_3389\" dir=in action=allow protocol=TCP localport=3389 profile=any";
                    await CommandRunner.RunCmdAsync(cmd, null);
                    await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule group=\"remote desktop\" new enable=Yes", null);
                    Logger.Log("✅ Port RDP 3389 berhasil DIIZINKAN di Windows Firewall.", LogType.Success);
                }
                else if (choice == "2")
                {
                    Logger.Log("Membuka Port Web Server (HTTP 80 & HTTPS 443)...", LogType.Info);
                    string cmd = "netsh advfirewall firewall add rule name=\"IT_ALLOW_WEB_80_443\" dir=in action=allow protocol=TCP localport=80,443 profile=any";
                    await CommandRunner.RunCmdAsync(cmd, null);
                    Logger.Log("✅ Port Web 80 (HTTP) dan 443 (HTTPS) berhasil DIIZINKAN.", LogType.Success);
                }
                else if (choice == "3")
                {
                    Logger.Log("Membuka Port Database Server (SQL 1433 & MySQL 3306)...", LogType.Info);
                    string cmd = "netsh advfirewall firewall add rule name=\"IT_ALLOW_DB_1433_3306\" dir=in action=allow protocol=TCP localport=1433,3306 profile=any";
                    await CommandRunner.RunCmdAsync(cmd, null);
                    Logger.Log("✅ Port Database MS SQL (1433) & MySQL (3306) berhasil DIIZINKAN.", LogType.Success);
                }
                else if (choice == "4")
                {
                    string? customPort = InputDialog.Show(
                        Form.ActiveForm,
                        "Input Port Kustom",
                        "Masukkan nomor port TCP yang ingin dibuka di Firewall (contoh: 8080 atau 5432):",
                        "8080"
                    );

                    if (!string.IsNullOrWhiteSpace(customPort))
                    {
                        Logger.Log($"Membuka Port Kustom (TCP {customPort})...", LogType.Info);
                        string cmd = $"netsh advfirewall firewall add rule name=\"IT_ALLOW_CUSTOM_{customPort}\" dir=in action=allow protocol=TCP localport={customPort} profile=any";
                        await CommandRunner.RunCmdAsync(cmd, null);
                        Logger.Log($"🎉 Port TCP {customPort} berhasil DIIZINKAN di Windows Firewall!", LogType.Success);
                    }
                }
                else
                {
                    Logger.Log("⚠️ Pilihan menu tidak valid.", LogType.Warning);
                }
            });
        }
    }
}
