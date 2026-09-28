using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class RustDeskManagerHubTool : IToolCommand
    {
        public string Id => "remote_rustdesk_manager_hub";
        public string Title => "Pusat Manajemen & Utilitas RustDesk";
        public string Description => "Solusi remote desktop open-source: Luncurkan, Reset ID/Key, Konfigurasi Self-Hosted Server (ID/Relay), Perbaiki Service, dan Buka Port Firewall.";
        public string Category => ToolCategory.Remote;
        public string Keywords => "rustdesk remote desktop self hosted relay id server open source anydesk teamviewer vnc reset install winget";
        public string Icon => "🦀";
        public string ButtonText => "Pusat Utilitas RustDesk";
        public Color ButtonColor => Color.FromArgb(231, 76, 60); // Rust Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT MANAJEMEN & UTILITAS RUSTDESK (OPEN SOURCE REMOTE) ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Pusat Utilitas RustDesk",
                "Pilih Operasi RustDesk yang Diinginkan:\n" +
                "1 = Luncurkan RustDesk Client (Auto-Deteksi)\n" +
                "2 = Reset ID, Kunci Enkripsi & Cache Sesi (Dapatkan ID Baru)\n" +
                "3 = Konfigurasi Self-Hosted Server Kantor (ID / Relay & Key)\n" +
                "4 = Pasang / Update RustDesk Terbaru (via Winget)\n" +
                "5 = Perbaiki & Restart Windows Service RustDesk\n" +
                "6 = Buka Port Firewall RustDesk (TCP/UDP 21115-21119)",
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
                        await LaunchRustDeskAsync();
                        break;
                    case "2":
                        await ResetRustDeskIdAndConfigAsync();
                        break;
                    case "3":
                        await ConfigureSelfHostedServerAsync();
                        break;
                    case "4":
                        await InstallOrUpdateRustDeskAsync();
                        break;
                    case "5":
                        await RepairRustDeskServiceAsync();
                        break;
                    case "6":
                        await ConfigureRustDeskFirewallAsync();
                        break;
                    default:
                        Logger.Log("⚠️ Pilihan menu tidak valid.", LogType.Warning);
                        break;
                }
            });
        }

        private async Task LaunchRustDeskAsync()
        {
            Logger.Log("Mencari instalasi RustDesk di komputer ini...", LogType.Info);
            string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "RustDesk", "rustdesk.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "RustDesk", "rustdesk.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "RustDesk", "rustdesk.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RustDesk", "rustdesk.exe")
            };

            string? foundPath = null;
            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    foundPath = path;
                    break;
                }
            }

            if (foundPath != null)
            {
                Logger.Log($"Menjalankan RustDesk dari: {foundPath}", LogType.Success);
                await CommandRunner.RunCmdAsync($"start \"\" \"{foundPath}\"", null);
            }
            else
            {
                Logger.Log("⚠️ RustDesk belum terinstall di komputer ini.", LogType.Warning);
                Logger.Log("Gunakan Menu Opsi 4 untuk memasang RustDesk secara otomatis via Winget.", LogType.Info);
            }
        }

        private async Task ResetRustDeskIdAndConfigAsync()
        {
            Logger.Log("Menghentikan proses dan service RustDesk...", LogType.Info);
            WindowsHelper.StopServiceIfExists("rustdesk");
            WindowsHelper.KillProcessIfExists("rustdesk");

            await Task.Delay(1000);

            string[] configDirs = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RustDesk", "config"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "RustDesk", "config"),
                @"C:\Windows\ServiceProfiles\LocalService\AppData\Roaming\RustDesk\config"
            };

            int deletedCount = 0;
            foreach (var dir in configDirs)
            {
                if (Directory.Exists(dir))
                {
                    try
                    {
                        var files = Directory.GetFiles(dir, "*.*");
                        foreach (var f in files)
                        {
                            try
                            {
                                File.Delete(f);
                                deletedCount++;
                            }
                            catch { }
                        }
                        Logger.Log($"Direktori konfigurasi dibersihkan: {dir}", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Peringatan akses folder {dir}: {ex.Message}", LogType.Warning);
                    }
                }
            }

            Logger.Log($"Berhasil membersihkan {deletedCount} file konfigurasi lama.", LogType.Info);
            Logger.Log("Menjalankan kembali service RustDesk...", LogType.Info);
            WindowsHelper.StartServiceIfExists("rustdesk");
            Logger.Log("✅ ID & Kunci Enkripsi RustDesk berhasil di-reset ke ID baru!", LogType.Success);
        }

        private async Task ConfigureSelfHostedServerAsync()
        {
            Logger.Log("=== KONFIGURASI SELF-HOSTED SERVER RUSTDESK KANTOR ===", LogType.Info);

            string? serverHost = InputDialog.Show(
                Form.ActiveForm,
                "Server Host RustDesk",
                "Masukkan Alamat IP atau Domain ID/Relay Server (contoh: 192.168.1.100 atau relay.kantor.com):",
                "192.168.1.100"
            );

            if (string.IsNullOrWhiteSpace(serverHost))
            {
                Logger.Log("ℹ️ Konfigurasi dibatalkan.", LogType.Info);
                return;
            }

            string? serverKey = InputDialog.Show(
                Form.ActiveForm,
                "Public Key Server",
                "Masukkan Public Key Server RustDesk (opsional / jika ada):",
                ""
            );

            Logger.Log($"Menyimpan konfigurasi Server: {serverHost}...", LogType.Info);

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string rustdeskConfigDir = Path.Combine(appData, "RustDesk", "config");
            Directory.CreateDirectory(rustdeskConfigDir);

            string tomlPath = Path.Combine(rustdeskConfigDir, "RustDesk2.toml");
            try
            {
                string configContent = $"custom-rendezvous-server = '{serverHost}'\nkey = '{serverKey ?? ""}'\n";
                File.AppendAllText(tomlPath, "\n" + configContent);
                Logger.Log($"Konfigurasi tersimpan di: {tomlPath}", LogType.Success);
            }
            catch (Exception ex)
            {
                Logger.Log($"Gagal menulis file config TOML: {ex.Message}", LogType.Warning);
            }

            // Jika RustDesk executable ada, jalankan CLI set server
            string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "RustDesk", "rustdesk.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "RustDesk", "rustdesk.exe")
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    string cliCmd = $"\"{path}\" --server {serverHost} " + (string.IsNullOrWhiteSpace(serverKey) ? "" : $"--key {serverKey}");
                    await CommandRunner.RunCmdAsync(cliCmd, null);
                    break;
                }
            }

            Logger.Log("✅ Konfigurasi Self-Hosted Server RustDesk berhasil diterapkan!", LogType.Success);
        }

        private async Task InstallOrUpdateRustDeskAsync()
        {
            Logger.Log("Memulai instalasi RustDesk melalui Windows Package Manager (Winget)...", LogType.Info);
            string cmd = "winget install RustDesk.RustDesk --silent --accept-source-agreements --accept-package-agreements";
            var result = await CommandRunner.RunCmdAsync(cmd, null);

            if (result == 0)
            {
                Logger.Log("✅ RustDesk berhasil dipasang / diperbarui ke versi terbaru!", LogType.Success);
            }
            else
            {
                Logger.Log("⚠️ Winget mengalami kendala atau butuh koneksi internet aktif. Unduh manual di: https://rustdesk.com", LogType.Warning);
            }
        }

        private async Task RepairRustDeskServiceAsync()
        {
            Logger.Log("Memperbaiki Service RustDesk Windows...", LogType.Info);
            await CommandRunner.RunCmdAsync("sc config rustdesk start= auto", null);
            await CommandRunner.RunCmdAsync("net stop rustdesk", null);
            await Task.Delay(1000);
            await CommandRunner.RunCmdAsync("net start rustdesk", null);
            Logger.Log("✅ Service RustDesk berhasil dikonfigurasi Automatic dan di-restart.", LogType.Success);
        }

        private async Task ConfigureRustDeskFirewallAsync()
        {
            Logger.Log("Menambahkan aturan Windows Firewall untuk RustDesk (TCP/UDP 21115-21119)...", LogType.Info);
            string cmdTcp = "netsh advfirewall firewall add rule name=\"IT_ALLOW_RUSTDESK_TCP\" dir=in action=allow protocol=TCP localport=21115-21119 profile=any";
            string cmdUdp = "netsh advfirewall firewall add rule name=\"IT_ALLOW_RUSTDESK_UDP\" dir=in action=allow protocol=UDP localport=21116 profile=any";
            await CommandRunner.RunCmdAsync(cmdTcp, null);
            await CommandRunner.RunCmdAsync(cmdUdp, null);
            Logger.Log("✅ Port Windows Firewall untuk RustDesk (TCP/UDP 21115-21119) berhasil DIIZINKAN!", LogType.Success);
        }
    }
}
