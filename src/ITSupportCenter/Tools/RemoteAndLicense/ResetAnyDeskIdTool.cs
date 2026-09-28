using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class ResetAnyDeskIdTool : IToolCommand
    {
        public string Id => "remote_reset_anydesk_id";
        public string Title => "Reset ID & Konfigurasi AnyDesk";
        public string Description => "Mereset ID AnyDesk baru jika terkena limit atau error koneksi: Hentikan AnyDesk, hapus service_conf & system.conf.";
        public string Category => ToolCategory.Remote;
        public string Keywords => "anydesk reset id remote support helpdesk license limit";
        public string Icon => "⚡";
        public string ButtonText => "Reset ID AnyDesk";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Pomegranate Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESET ID & KONFIGURASI ANYDESK ===", LogType.Info);

            await Task.Run(async () =>
            {
                Logger.Log("Menghentikan service dan proses AnyDesk...");
                WindowsHelper.StopServiceIfExists("AnyDesk");
                WindowsHelper.KillProcessIfExists("AnyDesk");

                await Task.Delay(1000);

                string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string anydeskProgData = Path.Combine(programData, "AnyDesk");

                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string anydeskAppData = Path.Combine(appData, "AnyDesk");

                DeleteAnyDeskConfig(anydeskProgData);
                DeleteAnyDeskConfig(anydeskAppData);

                Logger.Log("Menjalankan kembali AnyDesk Service...");
                WindowsHelper.StartServiceIfExists("AnyDesk");

                Logger.Log("✅ ID AnyDesk berhasil di-reset! Silakan buka kembali AnyDesk untuk mendapatkan nomor ID baru.", LogType.Success);
            });
        }

        private void DeleteAnyDeskConfig(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            string[] targetFiles = { "service.conf", "system.conf", "user.conf" };
            foreach (var tf in targetFiles)
            {
                string fp = Path.Combine(folderPath, tf);
                if (File.Exists(fp))
                {
                    try
                    {
                        File.Delete(fp);
                        Logger.Log($"Konfigurasi terhapus: {fp}", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal menghapus {tf}: {ex.Message}", LogType.Warning);
                    }
                }
            }
        }
    }
}
