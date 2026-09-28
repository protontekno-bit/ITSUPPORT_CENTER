using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class WingetSoftwareInstallerTool : IToolCommand
    {
        public string Id => "sys_winget_install_essentials";
        public string Title => "Paket Software Kantor Otomatis (Winget)";
        public string Description => "Memasang aplikasi esensial kantor (Chrome, 7-Zip, Adobe Reader, VLC, AnyDesk, Notepad++) otomatis via Microsoft Winget.";
        public string Category => ToolCategory.System;
        public string Keywords => "winget install software aplikasi chrome 7zip vlc anydesk adobe reader notepad package installer";
        public string Icon => "📦";
        public string ButtonText => "Pasang Paket Software Kantor";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMASANG PAKET SOFTWARE KANTOR (MICROSOFT WINGET) ===", LogType.Info);

            await Task.Run(async () =>
            {
                // Cek ketersediaan winget
                string checkOutput = "";
                int checkCode = await CommandRunner.RunCmdAsync("winget --version", line => checkOutput += line);
                if (checkCode != 0 || string.IsNullOrWhiteSpace(checkOutput))
                {
                    Logger.Log("⚠️ Microsoft Winget tidak terdeteksi pada sistem ini.", LogType.Warning);
                    Logger.Log("Catatan: Winget tersedia secara default di Windows 10 (versi 1809+) dan Windows 11 melalui App Installer.", LogType.Info);
                    return;
                }

                Logger.Log($"✅ Microsoft Winget aktif (Versi: {checkOutput.Trim()}).", LogType.Success);
                Logger.Log("Memulai proses instalasi paket software kantor esensial...", LogType.Info);

                var packages = new (string Id, string Name)[]
                {
                    ("Google.Chrome", "Google Chrome Browser"),
                    ("7zip.7zip", "7-Zip File Archiver"),
                    ("Adobe.Acrobat.Reader.64-bit", "Adobe Acrobat Reader PDF"),
                    ("VideoLAN.VLC", "VLC Media Player"),
                    ("AnyDeskSoftwareGmbH.AnyDesk", "AnyDesk Remote Desktop"),
                    ("Notepad++.Notepad++", "Notepad++ Text Editor")
                };

                int successCount = 0;
                foreach (var pkg in packages)
                {
                    Logger.Log($"\n[📦 Mengunduh & Memasang] {pkg.Name} ({pkg.Id})...", LogType.Info);
                    string cmd = $"winget install --id \"{pkg.Id}\" --silent --accept-source-agreements --accept-package-agreements --source winget";
                    
                    int exitCode = await CommandRunner.RunCmdAsync(cmd, line =>
                    {
                        if (!string.IsNullOrWhiteSpace(line) && !line.Contains("■") && !line.Contains("▒"))
                        {
                            Logger.Log($"   > {line.Trim()}", LogType.Info);
                        }
                    });

                    if (exitCode == 0 || exitCode == -1978335189) // -1978335189 / 0x8A15002B = Already installed
                    {
                        Logger.Log($"✅ {pkg.Name} berhasil dipasang / sudah terpasang.", LogType.Success);
                        successCount++;
                    }
                    else
                    {
                        Logger.Log($"⚠️ {pkg.Name} selesai dengan kode: {exitCode}. (Mungkin butuh persetujuan manual atau koneksi lambat)", LogType.Info);
                    }
                }

                Logger.Log($"\n=== SELESAI: {successCount}/{packages.Length} software kantor siap digunakan ===", LogType.Success);
            });
        }
    }
}
