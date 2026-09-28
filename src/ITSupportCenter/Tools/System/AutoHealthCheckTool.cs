using System;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.SystemTools
{
    public class AutoHealthCheckTool : IToolCommand
    {
        public string Id => "auto_health_check";
        public string Title => "Auto Health Check & Diagnosa";
        public string Description => "Pemeriksaan instan 1-klik status Print Spooler, SMB Guest Auth, RDP, dan network interface.";
        public string Category => ToolCategory.System;
        public string Keywords => "health check diagnosa otomatis tes cek sistem status spooler smb rdp";
        public string Icon => "🩺";
        public string ButtonText => "Cek Status";
        public Color ButtonColor => Color.FromArgb(39, 174, 96);

        public async Task ExecuteAsync()
        {
            Logger.Log("======================================================================");
            Logger.Log("           🩺 AUTO HEALTH CHECK & SYSTEM DIAGNOSTICS");
            Logger.Log("======================================================================");

            await Task.Run(() =>
            {
                int issuesCount = 0;

                // 1. Check Print Spooler
                try
                {
                    using var sc = new ServiceController("spooler");
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        Logger.Log("[CHECK] Print Spooler Service: BERJALAN (Normal)", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"[CHECK] Print Spooler Service: MATI ({sc.Status})", LogType.Error);
                        Logger.Log("  -> Saran: Jalankan [Fix Printer Sharing] untuk menyalakan kembali.", LogType.Warning);
                        issuesCount++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("[CHECK] Print Spooler: " + ex.Message, LogType.Warning);
                }

                // 2. Check RPC Privacy & PointAndPrint Registry
                try
                {
                    using var printKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Print");
                    var rpcVal = printKey?.GetValue("RpcAuthnLevelPrivacyEnabled");
                    if (rpcVal != null && Convert.ToInt32(rpcVal) == 0)
                    {
                        Logger.Log("[CHECK] Printer RPC Privacy (Fix 0x0000011b): SUDAH DITERAPKAN", LogType.Success);
                    }
                    else
                    {
                        Logger.Log("[CHECK] Printer RPC Privacy: BELUM DITERAPKAN (Berpotensi Error 0x0000011b)", LogType.Warning);
                    }
                }
                catch { }

                // 3. Check SMB Insecure Guest Auth
                try
                {
                    using var smbKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters");
                    var guestVal = smbKey?.GetValue("AllowInsecureGuestAuth");
                    if (guestVal != null && Convert.ToInt32(guestVal) == 1)
                    {
                        Logger.Log("[CHECK] SMB Insecure Guest Auth: AKTIF (Bisa Buka Share Tanpa Password)", LogType.Success);
                    }
                    else
                    {
                        Logger.Log("[CHECK] SMB Insecure Guest Auth: DIBLOKIR (Error 0x800704f8)", LogType.Warning);
                        Logger.Log("  -> Saran: Jalankan [Fix SMB Guest Error] jika butuh akses share tanpa password.", LogType.Info);
                    }
                }
                catch { }

                // 4. Check RDP Status
                try
                {
                    using var rdpKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server");
                    var denyVal = rdpKey?.GetValue("fDenyTSConnections");
                    if (denyVal != null && Convert.ToInt32(denyVal) == 0)
                    {
                        Logger.Log("[CHECK] Remote Desktop (RDP Port 3389): DIAKTIFKAN", LogType.Success);
                    }
                    else
                    {
                        Logger.Log("[CHECK] Remote Desktop (RDP Port 3389): DINONAKTIFKAN / DITUTUP", LogType.Info);
                    }
                }
                catch { }

                // 5. Check Active Network Interfaces
                try
                {
                    var nics = NetworkInterface.GetAllNetworkInterfaces()
                        .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                    foreach (var nic in nics)
                    {
                        var ipProps = nic.GetIPProperties();
                        var ipv4 = ipProps.UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork)?.Address.ToString() ?? "-";
                        var gateway = ipProps.GatewayAddresses.FirstOrDefault()?.Address.ToString() ?? "-";
                        var dns = string.Join(", ", ipProps.DnsAddresses.Select(d => d.ToString()));

                        Logger.Log($"[JARINGAN] {nic.Name} ({nic.Description})", LogType.Info);
                        Logger.Log($"           IP: {ipv4} | Gateway: {gateway} | DNS: {dns}", LogType.Info);
                    }
                }
                catch { }

                Logger.Log("======================================================================");
                if (issuesCount == 0)
                    Logger.Log("🩺 DIAGNOSA SELESAI: Konfigurasi sistem dalam kondisi baik.", LogType.Success);
                else
                    Logger.Log($"🩺 DIAGNOSA SELESAI: Ditemukan {issuesCount} peringatan/masalah.", LogType.Warning);
                Logger.Log("======================================================================");
            });
        }
    }
}
