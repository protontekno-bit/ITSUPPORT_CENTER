using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Network
{
    public class OpenNetworkConnectionsTool : IToolCommand
    {
        public string Id => "net_open_connections_cpl";
        public string Title => "Pusat Kontrol Adapter Jaringan & Wi-Fi Settings";
        public string Description => "Pintasan cepat membuka Network Connections (ncpa.cpl), Wi-Fi Settings modern, Proxy Settings, atau Release/Renew IP.";
        public string Category => ToolCategory.Network;
        public string Keywords => "ncpa cpl adapter connections network wifi settings proxy release renew";
        public string Icon => "🎛️";
        public string ButtonText => "Buka Kontrol Adapter";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT KONTROL ADAPTER JARINGAN & PENGATURAN WI-FI ===", LogType.Info);

            string action = PromptForChoice();
            if (string.IsNullOrEmpty(action)) return;

            await Task.Run(async () =>
            {
                if (action == "NCPA")
                {
                    Logger.Log("Membuka Kontrol Adapter Jaringan Klasik (ncpa.cpl)...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = "ncpa.cpl", UseShellExecute = true });
                        Logger.Log("✅ Jendela Network Connections (ncpa.cpl) terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal membuka ncpa.cpl: {ex.Message}", LogType.Error);
                    }
                }
                else if (action == "WIFI_SETTINGS")
                {
                    Logger.Log("Membuka Pengaturan Wi-Fi Modern Windows...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = "ms-settings:network-wifi", UseShellExecute = true });
                        Logger.Log("✅ Halaman Wi-Fi Settings terbuka.", LogType.Success);
                    }
                    catch
                    {
                        try { Process.Start(new ProcessStartInfo { FileName = "ms-settings:network", UseShellExecute = true }); } catch { }
                    }
                }
                else if (action == "RELEASE_RENEW")
                {
                    Logger.Log("Menjalankan Release dan Renew IP Address...", LogType.Info);
                    await CommandRunner.RunCmdAsync("ipconfig /release", s => { if (s.Contains("Ethernet") || s.Contains("Wi-Fi")) Logger.Log(s, LogType.Info); });
                    await Task.Delay(1000);
                    await CommandRunner.RunCmdAsync("ipconfig /renew", s =>
                    {
                        if (s.Contains("IPv4") || s.Contains("Alamat IPv4") || s.Contains("Default Gateway"))
                            Logger.Log(s.Trim(), LogType.Success);
                    });
                    Logger.Log("✅ Alamat IP berhasil diperbarui dari DHCP Router!", LogType.Success);
                }
                else if (action == "PROXY_SETTINGS")
                {
                    Logger.Log("Membuka Pengaturan Proxy Windows...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = "ms-settings:network-proxy", UseShellExecute = true });
                        Logger.Log("✅ Halaman Proxy Settings terbuka.", LogType.Success);
                    }
                    catch { }
                }
            });
        }

        private string PromptForChoice()
        {
            string choice = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pusat Kontrol Jaringan & Adapter";
                form.Width = 420;
                form.Height = 260;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih pintasan kontrol jaringan yang diinginkan:", Left = 20, Top = 12, Width = 370 };

                var btnNcpa = new Button { Text = "🎛️ Buka Adapter Jaringan Klasik (ncpa.cpl)", Left = 20, Top = 38, Width = 360, Height = 34 };
                btnNcpa.Click += (s, e) => { choice = "NCPA"; form.Close(); };

                var btnWifi = new Button { Text = "📶 Buka Pengaturan Wi-Fi Windows", Left = 20, Top = 78, Width = 360, Height = 34 };
                btnWifi.Click += (s, e) => { choice = "WIFI_SETTINGS"; form.Close(); };

                var btnRelRenew = new Button { Text = "🔄 Minta Ulang IP dari Router (Release & Renew IP)", Left = 20, Top = 118, Width = 360, Height = 34 };
                btnRelRenew.Click += (s, e) => { choice = "RELEASE_RENEW"; form.Close(); };

                var btnProxy = new Button { Text = "🌐 Buka Pengaturan Proxy Windows", Left = 20, Top = 158, Width = 360, Height = 34 };
                btnProxy.Click += (s, e) => { choice = "PROXY_SETTINGS"; form.Close(); };

                form.Controls.AddRange(new Control[] { label, btnNcpa, btnWifi, btnRelRenew, btnProxy });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return choice;
        }
    }
}
