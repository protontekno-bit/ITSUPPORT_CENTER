using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.Network
{
    public class WifiProfileExporterTool : IToolCommand
    {
        public string Id => "net_wifi_profile_exporter";
        public string Title => "Ekspor & Impor Massal Profil Wi-Fi (.XML)";
        public string Description => "Cadangkan seluruh profil Wi-Fi kantor beserta passwordnya ke file XML dan pasang kembali secara instan di laptop baru tanpa ketik manual.";
        public string Category => ToolCategory.Network;
        public string Keywords => "wifi wireless export import profile xml password cadangkan pulihkan laptop baru wlan netsh";
        public string Icon => "📶";
        public string ButtonText => "Ekspor / Impor Profil Wi-Fi";
        public Color ButtonColor => Color.FromArgb(52, 152, 219); // Peter River Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== EKSPOR & IMPOR MASSAL PROFIL WI-FI KANTOR (.XML) ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Ekspor / Impor Wi-Fi",
                "Pilih Tindakan Profil Wi-Fi:\n" +
                "1 = Ekspor Semua Profil Wi-Fi Aktif ke Folder (Sertakan Password .XML)\n" +
                "2 = Impor Massal Profil Wi-Fi dari Folder .XML ke Komputer Ini\n" +
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
                        await ExportWifiProfilesAsync();
                        break;
                    case "2":
                        await ImportWifiProfilesAsync();
                        break;
                    default:
                        Logger.Log($"⚠️ Pilihan '{choice}' tidak dikenali.", LogType.Warning);
                        break;
                }
            });
        }

        private async Task ExportWifiProfilesAsync()
        {
            string defaultFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WIFI_PROFILES_EXPORT");
            string? targetFolder = InputDialog.Show(
                Form.ActiveForm,
                "Tentukan Folder Ekspor",
                "Masukkan folder tujuan penyimpanan file profil Wi-Fi (.xml):",
                defaultFolder
            );

            if (string.IsNullOrWhiteSpace(targetFolder)) return;

            targetFolder = targetFolder.Trim().TrimEnd('\\');
            try
            {
                Directory.CreateDirectory(targetFolder);
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ Gagal membuat folder tujuan: {ex.Message}", LogType.Error);
                return;
            }

            Logger.Log($"Mengekspor seluruh profil Wi-Fi ke: {targetFolder}...", LogType.Info);
            string cmd = $"netsh wlan export profile folder=\"{targetFolder}\" key=clear";
            await CommandRunner.RunCmdAsync(cmd, s =>
            {
                if (!string.IsNullOrWhiteSpace(s)) Logger.Log(s.Trim(), LogType.Info);
            });

            var xmlFiles = Directory.GetFiles(targetFolder, "*.xml");
            if (xmlFiles.Length > 0)
            {
                Logger.Log($"✅ Berhasil mengekspor {xmlFiles.Length} profil Wi-Fi beserta passwordnya.", LogType.Success);
                Logger.Log($"📁 Lokasi file XML: {targetFolder}", LogType.Success);
            }
            else
            {
                Logger.Log("ℹ️ Tidak ditemukan profil Wi-Fi tersimpan di komputer ini.", LogType.Warning);
            }
        }

        private async Task ImportWifiProfilesAsync()
        {
            string? sourceFolder = InputDialog.Show(
                Form.ActiveForm,
                "Folder Profil Wi-Fi Sumber",
                "Masukkan path folder yang berisi file-file profil Wi-Fi (.xml):",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WIFI_PROFILES_EXPORT")
            );

            if (string.IsNullOrWhiteSpace(sourceFolder) || !Directory.Exists(sourceFolder.Trim()))
            {
                Logger.Log("⚠️ Direktori sumber tidak valid atau tidak ditemukan.", LogType.Warning);
                return;
            }

            sourceFolder = sourceFolder.Trim().TrimEnd('\\');
            var xmlFiles = Directory.GetFiles(sourceFolder, "*.xml");
            if (xmlFiles.Length == 0)
            {
                Logger.Log("⚠️ Tidak ditemukan file .xml profil Wi-Fi di folder tersebut.", LogType.Warning);
                return;
            }

            Logger.Log($"Ditemukan {xmlFiles.Length} file profil. Memulai impor massal...", LogType.Info);
            int imported = 0;
            foreach (var xml in xmlFiles)
            {
                string cmd = $"netsh wlan add profile filename=\"{xml}\" user=all";
                await CommandRunner.RunCmdAsync(cmd, s =>
                {
                    if (!string.IsNullOrWhiteSpace(s) && s.Contains("added", StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Log($"✅ Ditambahkan: {Path.GetFileName(xml)}", LogType.Success);
                        imported++;
                    }
                });
            }

            Logger.Log($"🎉 [SELESAI] Berhasil mengimpor {imported} profil Wi-Fi ke sistem!", LogType.Success);
            Logger.Log("Laptop kini dapat langsung tersambung otomatis ke hotspot/SSID kantor tanpa memasukkan sandi.", LogType.Success);
        }
    }
}
