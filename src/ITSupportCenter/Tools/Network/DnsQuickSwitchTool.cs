using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class DnsQuickSwitchTool : IToolCommand
    {
        public string Id => "net_dns_quick_switch";
        public string Title => "Pengatur DNS Cepat (Google / Cloudflare / DHCP)";
        public string Description => "1-Klik beralih DNS ke Google (8.8.8.8), Cloudflare (1.1.1.1), atau Auto DHCP untuk mengatasi situs/koneksi kantor terblokir/lemot.";
        public string Category => ToolCategory.Network;
        public string Keywords => "dns google 8.8.8.8 cloudflare 1.1.1.1 dhcp network internet adapter fast switch";
        public string Icon => "🌐";
        public string ButtonText => "Ganti DNS Adapter";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGATUR DNS CEPAT ADAPTER JARINGAN ===", LogType.Info);

            string choice = PromptForDnsOption();
            if (string.IsNullOrEmpty(choice))
            {
                Logger.Log("Operasi ganti DNS dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                // Find active primary network interface name (Ethernet or Wi-Fi)
                string? activeAdapter = GetActiveAdapterName();
                if (string.IsNullOrEmpty(activeAdapter))
                {
                    Logger.Log("Tidak ditemukan adapter jaringan yang aktif dan terhubung.", LogType.Warning);
                    return;
                }

                Logger.Log($"Menerapkan pengaturan DNS pada adapter: '{activeAdapter}'...", LogType.Info);

                if (choice == "GOOGLE")
                {
                    Logger.Log("Mengatur ke Google Public DNS (8.8.8.8 & 8.8.4.4)...");
                    await CommandRunner.RunCmdAsync($"netsh interface ip set dns name=\"{activeAdapter}\" static 8.8.8.8", s => Logger.Log(s, LogType.Info));
                    await CommandRunner.RunCmdAsync($"netsh interface ip add dns name=\"{activeAdapter}\" 8.8.4.4 index=2", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ DNS berhasil diubah ke Google DNS (8.8.8.8 / 8.8.4.4)!", LogType.Success);
                }
                else if (choice == "CLOUDFLARE")
                {
                    Logger.Log("Mengatur ke Cloudflare 1.1.1.1 Fast DNS (1.1.1.1 & 1.0.0.1)...");
                    await CommandRunner.RunCmdAsync($"netsh interface ip set dns name=\"{activeAdapter}\" static 1.1.1.1", s => Logger.Log(s, LogType.Info));
                    await CommandRunner.RunCmdAsync($"netsh interface ip add dns name=\"{activeAdapter}\" 1.0.0.1 index=2", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ DNS berhasil diubah ke Cloudflare DNS (1.1.1.1 / 1.0.0.1)!", LogType.Success);
                }
                else if (choice == "DHCP")
                {
                    Logger.Log("Mengembalikan DNS ke Otomatis (DHCP bawaan router/kantor)...");
                    await CommandRunner.RunCmdAsync($"netsh interface ip set dns name=\"{activeAdapter}\" dhcp", s => Logger.Log(s, LogType.Info));
                    Logger.Log("✅ DNS dikembalikan ke Otomatis / DHCP!", LogType.Success);
                }

                // Flush DNS cache
                await CommandRunner.RunCmdAsync("ipconfig /flushdns", null);
                Logger.Log("Cache DNS berhasil di-flush.", LogType.Info);
            });
        }

        private string? GetActiveAdapterName()
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                    !nic.Description.Contains("Virtual", StringComparison.OrdinalIgnoreCase) &&
                    !nic.Description.Contains("Hyper-V", StringComparison.OrdinalIgnoreCase) &&
                    !nic.Description.Contains("Loopback", StringComparison.OrdinalIgnoreCase))
                {
                    return nic.Name;
                }
            }
            return null;
        }

        private string PromptForDnsOption()
        {
            string selection = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pilih Konfigurasi DNS Target";
                form.Width = 400;
                form.Height = 220;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih DNS Server untuk adapter jaringan aktif:", Left = 20, Top = 15, Width = 350 };

                var btnGoogle = new Button { Text = "🌐 Google DNS (8.8.8.8 / 8.8.4.4)", Left = 20, Top = 45, Width = 340, Height = 32 };
                btnGoogle.Click += (s, e) => { selection = "GOOGLE"; form.Close(); };

                var btnCloudflare = new Button { Text = "⚡ Cloudflare Fast DNS (1.1.1.1 / 1.0.0.1)", Left = 20, Top = 85, Width = 340, Height = 32 };
                btnCloudflare.Click += (s, e) => { selection = "CLOUDFLARE"; form.Close(); };

                var btnDhcp = new Button { Text = "🔄 Kembalikan ke Otomatis (DHCP Router)", Left = 20, Top = 125, Width = 340, Height = 32 };
                btnDhcp.Click += (s, e) => { selection = "DHCP"; form.Close(); };

                form.Controls.AddRange(new Control[] { label, btnGoogle, btnCloudflare, btnDhcp });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return selection;
        }
    }
}
