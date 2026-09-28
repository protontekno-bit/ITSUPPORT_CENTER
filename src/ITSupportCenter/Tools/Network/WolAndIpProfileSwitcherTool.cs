using System;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.Network
{
    public class WolAndIpProfileSwitcherTool : IToolCommand
    {
        public string Id => "net_wol_ip_switcher";
        public string Title => "Wake-on-LAN (WoL) & Pengatur Cepat Profil IP";
        public string Description => "Nyalakan komputer/server dari jarak jauh via Magic Packet LAN (WoL) dan beralih cepat antar profil alamat IP (DHCP Kantor, IP Statis Server/Lab, dll.).";
        public string Category => ToolCategory.Network;
        public string Keywords => "wake on lan wol magic packet nyalakan pc remote mac address ip switcher static dhcp vlan";
        public string Icon => "⚡";
        public string ButtonText => "Wake-on-LAN & Profil IP";
        public Color ButtonColor => Color.FromArgb(46, 204, 113); // Emerald Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== WAKE-ON-LAN (WOL) & PENGATUR CEPAT PROFIL IP ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "WoL & IP Profile Switcher",
                "Pilih Tindakan Jaringan yang Diinginkan:\n" +
                "1 = Kirim Magic Packet Wake-on-LAN (Nyalakan PC via MAC Address)\n" +
                "2 = Setel Adapter ke DHCP Otomatis (IP & DNS Otomatis)\n" +
                "3 = Terapkan Konfigurasi IP Statis Cepat (Input Manual)\n" +
                "0 = Batal",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice) || choice.Trim() == "0")
            {
                Logger.Log("ℹ️ Operasi dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                switch (choice.Trim())
                {
                    case "1":
                        SendWakeOnLanPrompt();
                        break;
                    case "2":
                        await ApplyDhcpProfileAsync();
                        break;
                    case "3":
                        await ApplyStaticIpPromptAsync();
                        break;
                    default:
                        Logger.Log($"⚠️ Pilihan '{choice}' tidak dikenali.", LogType.Warning);
                        break;
                }
            });
        }

        private void SendWakeOnLanPrompt()
        {
            string? macInput = InputDialog.Show(
                Form.ActiveForm,
                "Target Wake-on-LAN",
                "Masukkan MAC Address komputer/server yang ingin dinyalakan:\n(Contoh: 00:1A:2B:3C:4D:5E atau 00-1A-2B-3C-4D-5E)",
                ""
            );

            if (string.IsNullOrWhiteSpace(macInput))
            {
                Logger.Log("ℹ️ MAC address kosong. Pengiriman dibatalkan.", LogType.Info);
                return;
            }

            string cleanMac = Regex.Replace(macInput.Trim(), "[:\\-.]", "");
            if (cleanMac.Length != 12)
            {
                Logger.Log($"❌ Format MAC Address '{macInput}' tidak valid. Harus 12 karakter heksadesimal.", LogType.Error);
                return;
            }

            try
            {
                byte[] macBytes = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    macBytes[i] = byte.Parse(cleanMac.Substring(i * 2, 2), NumberStyles.HexNumber);
                }

                // Bangun Magic Packet: 6 bytes 0xFF diikuti 16 repetisi MAC address (total 102 bytes)
                byte[] packet = new byte[102];
                for (int i = 0; i < 6; i++) packet[i] = 0xFF;
                for (int i = 1; i <= 16; i++)
                {
                    Buffer.BlockCopy(macBytes, 0, packet, i * 6, 6);
                }

                using var client = new UdpClient();
                client.EnableBroadcast = true;
                client.Send(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 9));
                client.Send(packet, packet.Length, new IPEndPoint(IPAddress.Broadcast, 7));

                Logger.Log($"✅ Magic Packet WoL berhasil disiarkan ke {macInput} (Port 7 & 9).", LogType.Success);
                Logger.Log("Komputer target akan mulai booting jika fitur Wake-on-LAN di BIOS/UEFI dan kartu jaringannya aktif.", LogType.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ Gagal mengirim Magic Packet: {ex.Message}", LogType.Error);
            }
        }

        private async Task ApplyDhcpProfileAsync()
        {
            Logger.Log("Mengembalikan semua adapter aktif ke konfigurasi DHCP Otomatis...", LogType.Info);
            await CommandRunner.RunCmdAsync("netsh interface ip set address name=\"Ethernet\" source=dhcp", null);
            await CommandRunner.RunCmdAsync("netsh interface ip set dns name=\"Ethernet\" source=dhcp", null);
            await CommandRunner.RunCmdAsync("netsh interface ip set address name=\"Wi-Fi\" source=dhcp", null);
            await CommandRunner.RunCmdAsync("netsh interface ip set dns name=\"Wi-Fi\" source=dhcp", null);
            await CommandRunner.RunCmdAsync("ipconfig /renew", null);
            Logger.Log("✅ Konfigurasi DHCP Otomatis berhasil diterapkan pada adapter Ethernet & Wi-Fi.", LogType.Success);
        }

        private async Task ApplyStaticIpPromptAsync()
        {
            string? ipParams = InputDialog.Show(
                Form.ActiveForm,
                "Konfigurasi IP Statis",
                "Masukkan parameter dengan format: IP,Subnet,Gateway,DNS\n(Contoh: 192.168.1.150,255.255.255.0,192.168.1.1,8.8.8.8)",
                "192.168.1.150,255.255.255.0,192.168.1.1,8.8.8.8"
            );

            if (string.IsNullOrWhiteSpace(ipParams)) return;

            string[] parts = ipParams.Split(',');
            if (parts.Length < 3)
            {
                Logger.Log("⚠️ Format parameter kurang lengkap. Butuh minimal IP, Subnet, dan Gateway.", LogType.Warning);
                return;
            }

            string ip = parts[0].Trim();
            string mask = parts[1].Trim();
            string gateway = parts[2].Trim();
            string dns = parts.Length > 3 ? parts[3].Trim() : "8.8.8.8";

            Logger.Log($"Menerapkan IP Statis: {ip} / {mask}, GW: {gateway}, DNS: {dns}...", LogType.Info);

            string[] candidateAdapters = new[] { "Ethernet", "Wi-Fi" };
            foreach (var ad in candidateAdapters)
            {
                await CommandRunner.RunCmdAsync($"netsh interface ip set address name=\"{ad}\" static {ip} {mask} {gateway} 1", null);
                await CommandRunner.RunCmdAsync($"netsh interface ip set dns name=\"{ad}\" static {dns} primary", null);
            }
            Logger.Log("✅ Setelan IP Statis berhasil diterapkan.", LogType.Success);
        }
    }
}
