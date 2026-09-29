using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Network
{
    public class NetworkAdapterPowerCyclerTool : IToolCommand
    {
        public string Id => "net_adapter_power_cycler";
        public string Title => "Siklus Restart Keras Hardware Kartu Jaringan (NIC Power-Cycle)";
        public string Description => "Melakukan restart fisik kartu jaringan (Ethernet & Wi-Fi) via PnP & NetAdapter API untuk mengatasi adapter macet, error code 43, atau no internet tanpa restart PC.";
        public string Category => ToolCategory.Network;
        public string Keywords => "nic adapter restart disable enable powercycle ethernet wifi hard reset driver";
        public string Icon => "⚡";
        public string ButtonText => "Restart Fisik Adapter";
        public Color ButtonColor => Color.FromArgb(230, 126, 34);

        public async Task ExecuteAsync()
        {
            var adapters = GetPhysicalAdapters();
            if (adapters.Count == 0)
            {
                Logger.Log("Tidak ditemukan adapter jaringan fisik aktif (Ethernet/Wi-Fi).", LogType.Warning);
                return;
            }

            string targetAdapter = PromptSelectAdapter(adapters);
            if (string.IsNullOrWhiteSpace(targetAdapter))
            {
                Logger.Log("Operasi restart adapter dibatalkan.", LogType.Info);
                return;
            }

            Logger.Log($"=== MEMULAI SIKLUS POWER-CYCLE KARTU JARINGAN: {targetAdapter} ===", LogType.Info);

            await Task.Run(async () =>
            {
                var targets = targetAdapter == "SEMUA ADAPTER FISIK (ALL)" ? adapters : new List<string> { targetAdapter };

                foreach (var nic in targets)
                {
                    Logger.Log($"[1/4] Merestart adapter hardware: '{nic}' via NetAdapter API...", LogType.Info);

                    int psExit = await CommandRunner.RunPowerShellAsync($"Restart-NetAdapter -Name '{nic}' -Confirm:$false");
                    if (psExit != 0)
                    {
                        Logger.Log($"Fallback: Menonaktifkan & mengaktifkan ulang '{nic}' via Netsh...", LogType.Warning);
                        await CommandRunner.RunCmdAsync($"netsh interface set interface \"{nic}\" admin=disable");
                        await Task.Delay(2000);
                        await CommandRunner.RunCmdAsync($"netsh interface set interface \"{nic}\" admin=enable");
                    }
                    else
                    {
                        Logger.Log($"Adapter '{nic}' berhasil di-restart melalui PowerShell NetAdapter API.", LogType.Success);
                    }

                    await Task.Delay(1500);
                }

                Logger.Log("[2/4] Melakukan PnP Hardware Bus Rescan (pnputil /scan-devices)...", LogType.Info);
                await CommandRunner.RunCmdAsync("pnputil /scan-devices", s => Logger.Log(s, LogType.Info));

                Logger.Log("[3/4] Mengosongkan DNS Cache & Memperbarui Alamat IP (DHCP Lease)...", LogType.Info);
                await CommandRunner.RunCmdAsync("ipconfig /flushdns");
                await CommandRunner.RunCmdAsync("ipconfig /renew", s =>
                {
                    if (s.Contains("IPv4") || s.Contains("Default Gateway") || s.Contains("Alamat"))
                        Logger.Log(s.Trim(), LogType.Success);
                });

                Logger.Log("[4/4] Menguji stabilitas koneksi pasca-restart...", LogType.Info);
                await Task.Delay(1500);

                try
                {
                    using var ping = new Ping();
                    var reply = await ping.SendPingAsync("8.8.8.8", 2500);
                    if (reply.Status == IPStatus.Success)
                    {
                        Logger.Log($"✅ SUKSES: Koneksi internet pulih normal! RTT: {reply.RoundtripTime}ms", LogType.Success);
                    }
                    else
                    {
                        Logger.Log("⚠️ Adapter telah di-restart. Tunggu beberapa detik hingga DHCP selesai bernegosiasi.", LogType.Warning);
                    }
                }
                catch
                {
                    Logger.Log("Siklus restart adapter selesai. Periksa ikon jaringan di taskbar Windows.", LogType.Info);
                }

                Logger.Log("🎉 [SELESAI] Seluruh rangkaian siklus restart adapter hardware telah selesai!", LogType.Success);
            });
        }

        private static List<string> GetPhysicalAdapters()
        {
            var list = new List<string>();
            try
            {
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        string desc = ni.Description.ToLower();
                        if (desc.Contains("virtual") || desc.Contains("vpn") || desc.Contains("hyper-v") ||
                            desc.Contains("vmware") || desc.Contains("loopback") || desc.Contains("pseudo"))
                            continue;

                        list.Add(ni.Name);
                    }
                }
            }
            catch { }
            return list;
        }

        private static string PromptSelectAdapter(List<string> adapters)
        {
            using var form = new Form
            {
                Width = 440,
                Height = 220,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Pilih Kartu Jaringan untuk Di-Restart",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(24, 28, 38),
                ForeColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Left = 20, Top = 20, Text = "Pilih Adapter Fisik yang Mengalami Gangguan / Macet:", AutoSize = true, Font = new Font("Segoe UI", 9.5f) };

            var cmb = new ComboBox
            {
                Left = 20,
                Top = 50,
                Width = 380,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(35, 41, 55),
                ForeColor = Color.White
            };

            if (adapters.Count > 1) cmb.Items.Add("SEMUA ADAPTER FISIK (ALL)");
            foreach (var a in adapters) cmb.Items.Add(a);
            cmb.SelectedIndex = 0;

            var btnOk = new Button { Text = "Restart Sekarang", Left = 175, Width = 135, Top = 110, Height = 32, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(230, 126, 34), ForeColor = Color.White };
            var btnCancel = new Button { Text = "Batal", Left = 320, Width = 80, Top = 110, Height = 32, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White };

            form.Controls.Add(lbl);
            form.Controls.Add(cmb);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog() == DialogResult.OK ? cmb.SelectedItem?.ToString() ?? "" : "";
        }
    }
}
