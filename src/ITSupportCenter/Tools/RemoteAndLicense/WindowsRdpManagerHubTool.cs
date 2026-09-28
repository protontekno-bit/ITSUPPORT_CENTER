using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class WindowsRdpManagerHubTool : IToolCommand
    {
        public string Id => "remote_rdp_manager_hub";
        public string Title => "Pusat Manajemen & Aktivator Windows RDP";
        public string Description => "Kelola Windows Remote Desktop: Aktivasi 1-klik, Buka Firewall, Toggle NLA, Ganti Port RDP, Remote Shadowing tanpa logout, dan Quick Connect Client.";
        public string Category => ToolCategory.Remote;
        public string Keywords => "rdp remote desktop mstsc terminal server shadow nla 3389 port remote client qwinsta rwinsta termservice";
        public string Icon => "🖥️";
        public string ButtonText => "Pusat Utilitas Windows RDP";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("======================================================================");
            Logger.Log("         🖥️ PUSAT MANAJEMEN WINDOWS REMOTE DESKTOP (RDP)");
            Logger.Log("======================================================================");

            // Audit status RDP saat ini
            AuditCurrentRdpStatus();

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Pusat Manajemen Windows Remote Desktop",
                "Pilih Operasi RDP yang Diinginkan:\n\n" +
                "1 = [1-Klik] Aktifkan Penuh RDP Host (Aktifkan Service + Buka Firewall)\n" +
                "2 = [1-Klik] Matikan Total RDP Host (Tutup Port & Kunci Akses Masuk)\n" +
                "3 = Saklar NLA (Toggle Network Level Authentication On / Off)\n" +
                "4 = Ganti Port Standar RDP (Ubah 3389 ke Port Custom + Auto Firewall)\n" +
                "5 = Quick Connect Klien RDP (Ketik IP Target -> Luncurkan mstsc)\n" +
                "6 = RDP Remote Shadowing (Lihat / Bimbing Layar User Tanpa Logout)\n" +
                "7 = Audit Sesi Aktif & Tendang Sesi Nyangkut (qwinsta / rwinsta)\n" +
                "8 = Buka Kontrol Konfigurasi Remote Desktop Klasik (sysdm.cpl)",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice))
            {
                Logger.Log("ℹ️ Operasi dibatalkan oleh pengguna.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                switch (choice.Trim())
                {
                    case "1":
                        await EnableRdpHostAsync();
                        break;
                    case "2":
                        await DisableRdpHostAsync();
                        break;
                    case "3":
                        await ToggleNlaAsync();
                        break;
                    case "4":
                        await ChangeRdpPortAsync();
                        break;
                    case "5":
                        await LaunchQuickConnectAsync();
                        break;
                    case "6":
                        await PerformRdpShadowingAsync();
                        break;
                    case "7":
                        await AuditAndResetSessionsAsync();
                        break;
                    case "8":
                        await OpenClassicRdpSettingsAsync();
                        break;
                    default:
                        Logger.Log("⚠️ Pilihan menu tidak valid.", LogType.Warning);
                        break;
                }
            });
        }

        private void AuditCurrentRdpStatus()
        {
            try
            {
                // 1. Cek status aktif/nonaktif
                using var tsKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Terminal Server");
                int denyTs = Convert.ToInt32(tsKey?.GetValue("fDenyTSConnections") ?? 1);
                bool isRdpEnabled = (denyTs == 0);

                // 2. Cek port RDP
                using var tcpKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp");
                int port = Convert.ToInt32(tcpKey?.GetValue("PortNumber") ?? 3389);
                int userAuth = Convert.ToInt32(tcpKey?.GetValue("UserAuthentication") ?? 1);
                bool isNlaEnabled = (userAuth == 1);

                // 3. Cek Service
                string serviceStatus = "Tidak Diketahui";
                try
                {
                    using var sc = new ServiceController("TermService");
                    serviceStatus = sc.Status.ToString();
                }
                catch { }

                // 4. IP Lokal
                string localIp = GetPrimaryLocalIp();

                Logger.Log($"• Status RDP Host : {(isRdpEnabled ? "AKTIF (Bisa di-remote)" : "NON-AKTIF (Mati)")}", 
                    isRdpEnabled ? LogType.Success : LogType.Warning);
                Logger.Log($"• Nomor Port RDP  : TCP {port} {(port == 3389 ? "(Default)" : "(Port Kustom)")}");
                Logger.Log($"• Autentikasi NLA : {(isNlaEnabled ? "AKTIF (Wajib NLA)" : "NON-AKTIF (Kompatibel Semua Klien)")}");
                Logger.Log($"• Service Windows : TermService ({serviceStatus})");
                Logger.Log($"• IP Lokal PC Ini : {localIp} (Gunakan IP ini untuk me-remote dari PC lain)");
                Logger.Log("----------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                Logger.Log($"Catatan saat audit status RDP: {ex.Message}", LogType.Info);
            }
        }

        private async Task EnableRdpHostAsync()
        {
            Logger.Log("Mengaktifkan Windows Remote Desktop Host...", LogType.Info);

            // 1. Aktifkan di Registry
            WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 0);

            // 2. Konfigurasi Service TermService ke Automatic & Start
            await CommandRunner.RunCmdAsync("sc config TermService start= auto", null);
            WindowsHelper.StartServiceIfExists("TermService");

            // 3. Buka Firewall Port 3389
            Logger.Log("Membuka aturan Windows Firewall untuk Remote Desktop...", LogType.Info);
            await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule group=\"remote desktop\" new enable=Yes", null);
            await CommandRunner.RunCmdAsync("netsh advfirewall firewall add rule name=\"Windows Remote Desktop (TCP-3389)\" dir=in action=allow protocol=TCP localport=3389", null);
            await CommandRunner.RunCmdAsync("netsh advfirewall firewall add rule name=\"Windows Remote Desktop (UDP-3389)\" dir=in action=allow protocol=UDP localport=3389", null);

            Logger.Log("✅ SUKSES: Windows Remote Desktop (RDP) berhasil DIAKTIFKAN penuh!", LogType.Success);
            Logger.Log("ℹ️ Komputer ini sekarang siap menerima koneksi remote dari komputer lain di jaringan.", LogType.Success);
        }

        private async Task DisableRdpHostAsync()
        {
            Logger.Log("Mematikan Windows Remote Desktop Host...", LogType.Info);

            // 1. Matikan di Registry
            WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Terminal Server", "fDenyTSConnections", 1);

            // 2. Tutup Firewall Rule
            await CommandRunner.RunCmdAsync("netsh advfirewall firewall set rule group=\"remote desktop\" new enable=No", null);
            await CommandRunner.RunCmdAsync("netsh advfirewall firewall delete rule name=\"Windows Remote Desktop (TCP-3389)\"", null);

            Logger.Log("🔒 SUKSES: Windows Remote Desktop Host telah DIMATIKAN.", LogType.Success);
            Logger.Log("ℹ️ Komputer ini aman dan tidak dapat diakses melalui koneksi RDP dari luar.", LogType.Info);
        }

        private Task ToggleNlaAsync()
        {
            try
            {
                using var tcpKey = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp");
                int userAuth = Convert.ToInt32(tcpKey?.GetValue("UserAuthentication") ?? 1);

                if (userAuth == 1)
                {
                    // Matikan NLA
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "UserAuthentication", 0);
                    Logger.Log("✅ NLA (Network Level Authentication) berhasil DINONAKTIFKAN.", LogType.Success);
                    Logger.Log("ℹ️ Solusi jika klien mengalami error 'The remote computer requires Network Level Authentication'.", LogType.Info);
                }
                else
                {
                    // Aktifkan NLA
                    WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "UserAuthentication", 1);
                    Logger.Log("✅ NLA (Network Level Authentication) berhasil DIAKTIFKAN kembali.", LogType.Success);
                    Logger.Log("ℹ️ Keamanan standar Windows dipulihkan (Klien wajib memasukkan kredensial sebelum sesi grafis dibuka).", LogType.Info);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Gagal mengubah setelan NLA: {ex.Message}", LogType.Error);
            }
            return Task.CompletedTask;
        }

        private async Task ChangeRdpPortAsync()
        {
            string? portInput = InputDialog.Show(
                Form.ActiveForm,
                "Ganti Nomor Port RDP",
                "Masukkan nomor port RDP baru yang diinginkan (Rentang: 1025 - 65535):\n\n" +
                "Contoh rekomendasi aman: 33890, 3390, atau 53389.\n" +
                "(Port default adalah 3389)",
                "33890"
            );

            if (string.IsNullOrWhiteSpace(portInput) || !int.TryParse(portInput.Trim(), out int newPort) || newPort < 1024 || newPort > 65535)
            {
                Logger.Log("⚠️ Nomor port tidak valid. Port harus berupa angka antara 1024 dan 65535.", LogType.Warning);
                return;
            }

            Logger.Log($"Menerapkan nomor port RDP baru: {newPort}...", LogType.Info);

            // 1. Simpan ke Registry
            WindowsHelper.SetRegistryDWordSafe("HKLM", @"System\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "PortNumber", newPort);

            // 2. Daftarkan Port Baru ke Windows Firewall
            await CommandRunner.RunCmdAsync($"netsh advfirewall firewall add rule name=\"Custom RDP Port (TCP-{newPort})\" dir=in action=allow protocol=TCP localport={newPort}", null);

            Logger.Log($"✅ SUKSES: Port RDP berhasil diubah ke TCP {newPort}!", LogType.Success);
            Logger.Log("ℹ️ Aturan Firewall untuk port baru telah otomatis ditambahkan.", LogType.Success);
            Logger.Log("⚠️ PERHATIAN: Lakukan restart komputer atau restart service TermService agar port baru aktif sepenuhnya.", LogType.Warning);
            Logger.Log($"ℹ️ Untuk meremote, gunakan format IP:PORT (contoh: 192.168.1.50:{newPort}).", LogType.Info);
        }

        private async Task LaunchQuickConnectAsync()
        {
            string? targetIp = InputDialog.Show(
                Form.ActiveForm,
                "Quick Connect RDP Client",
                "Masukkan Alamat IP atau Hostname komputer target:\n\n" +
                "Contoh: 192.168.1.100 atau PC-SERVER:33890",
                ""
            );

            if (string.IsNullOrWhiteSpace(targetIp))
            {
                Logger.Log("ℹ️ Tidak ada target yang dimasukkan.", LogType.Info);
                return;
            }

            Logger.Log($"Meluncurkan Microsoft Remote Desktop Connection ke '{targetIp.Trim()}'...", LogType.Info);
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mstsc.exe",
                    Arguments = $"/v:{targetIp.Trim()} /f",
                    UseShellExecute = true
                });
                Logger.Log($"✅ Klien mstsc.exe berhasil diluncurkan untuk target {targetIp.Trim()}.", LogType.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"Gagal meluncurkan mstsc.exe: {ex.Message}", LogType.Error);
                await CommandRunner.RunCmdAsync($"start mstsc.exe /v:{targetIp.Trim()}", null);
            }
        }

        private async Task PerformRdpShadowingAsync()
        {
            Logger.Log("Memeriksa sesi pengguna aktif di komputer ini...", LogType.Info);

            // 1. Ambil daftar sesi via qwinsta
            await CommandRunner.RunCmdAsync("qwinsta", s => Logger.Log(s, LogType.Info));

            string? sessionInput = InputDialog.Show(
                Form.ActiveForm,
                "RDP Session Shadowing (Live Assistance)",
                "Masukkan Nomor ID Sesi pengguna yang ingin dibimbing/dipantau:\n\n" +
                "(Lihat kolom ID pada daftar Terminal Log di sebelah kanan, biasanya ID 1 atau 2)",
                "1"
            );

            if (string.IsNullOrWhiteSpace(sessionInput) || !int.TryParse(sessionInput.Trim(), out int sessionId))
            {
                Logger.Log("⚠️ ID sesi tidak valid.", LogType.Warning);
                return;
            }

            string? modeInput = InputDialog.Show(
                Form.ActiveForm,
                "Pilih Mode Shadowing",
                "Pilih Mode Pengendalian Layar:\n\n" +
                "1 = Kontrol Penuh (Teknisi bisa menggerakkan mouse & mengetik bersama user)\n" +
                "2 = Pantau Saja (View Only - Hanya melihat layar tanpa mengganggu)",
                "1"
            );

            bool fullControl = (modeInput?.Trim() != "2");
            string shadowArgs = fullControl ? $"/shadow:{sessionId} /control" : $"/shadow:{sessionId}";

            Logger.Log($"Meluncurkan Remote Shadowing ke Sesi {sessionId} (Mode: {(fullControl ? "Full Control" : "View Only")})...", LogType.Info);

            // Pastikan policy shadowing diaktifkan di registry
            WindowsHelper.SetRegistryDWordSafe("HKLM", @"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services", "Shadow", 1);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mstsc.exe",
                    Arguments = shadowArgs,
                    UseShellExecute = true
                });
                Logger.Log($"✅ RDP Shadowing aktif ke Sesi {sessionId}. Pengguna tidak akan ter-logout!", LogType.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"Gagal memulai RDP Shadowing: {ex.Message}", LogType.Error);
            }
        }

        private async Task AuditAndResetSessionsAsync()
        {
            Logger.Log("--- DAFTAR SESI REMOTE & KONSOL SAAT INI (QWINSTA) ---", LogType.Info);
            await CommandRunner.RunCmdAsync("qwinsta", s => Logger.Log(s, LogType.Info));

            string? killSessionInput = InputDialog.Show(
                Form.ActiveForm,
                "Reset / Putus Sesi RDP Nyangkut",
                "Jika ada sesi yang macet atau ingin diputus paksa, masukkan ID Sesi:\n\n" +
                "(Kosongkan dan klik OK jika hanya ingin melihat status)",
                ""
            );

            if (!string.IsNullOrWhiteSpace(killSessionInput) && int.TryParse(killSessionInput.Trim(), out int killId))
            {
                Logger.Log($"Mereset sesi ID {killId} via rwinsta...", LogType.Warning);
                await CommandRunner.RunCmdAsync($"rwinsta {killId}", s => Logger.Log(s, LogType.Info));
                Logger.Log($"✅ Sesi {killId} berhasil di-reset / diputus.", LogType.Success);
            }
            else
            {
                Logger.Log("ℹ️ Audit sesi selesai. Tidak ada sesi yang di-reset.", LogType.Info);
            }
        }

        private Task OpenClassicRdpSettingsAsync()
        {
            Logger.Log("Membuka jendela System Properties tab Remote klasik...", LogType.Info);
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "SystemPropertiesRemote.exe",
                    UseShellExecute = true
                });
                Logger.Log("✅ SystemPropertiesRemote berhasil dibuka.", LogType.Success);
            }
            catch
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "control.exe",
                    Arguments = "sysdm.cpl,,5",
                    UseShellExecute = true
                });
            }
            return Task.CompletedTask;
        }

        private string GetPrimaryLocalIp()
        {
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
                socket.Connect("8.8.8.8", 65530);
                if (socket.LocalEndPoint is IPEndPoint endPoint)
                {
                    return endPoint.Address.ToString();
                }
            }
            catch
            {
                try
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                            return ip.ToString();
                    }
                }
                catch { }
            }
            return "127.0.0.1";
        }
    }
}
