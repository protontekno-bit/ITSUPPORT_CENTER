using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class CredentialManagerTool : IToolCommand
    {
        public string Id => "remote_credential_manager";
        public string Title => "Kelola Kredensial Windows (Password Tersimpan)";
        public string Description => "Membuka Credential Manager untuk menghapus atau memperbarui password login File Share & RDP yang tersimpan salah.";
        public string Category => ToolCategory.Network;
        public string Keywords => "credential manager password login share network rdp keymgr simpan";
        public string Icon => "🔐";
        public string ButtonText => "Buka Credential Manager";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PENGELOLA KREDENSIAL WINDOWS ===", LogType.Info);

            await Task.Run(() =>
            {
                try
                {
                    Logger.Log("Membuka Pengelola Kredensial (Credential Manager)...");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "control.exe",
                        Arguments = "/name Microsoft.CredentialManager",
                        UseShellExecute = true
                    });
                    Logger.Log("✅ Credential Manager terbuka. Cari 'Windows Credentials' dan hapus entri IP/Komputer yang bermasalah.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal membuka Credential Manager: {ex.Message}", LogType.Error);
                }
            });
        }
    }
}
