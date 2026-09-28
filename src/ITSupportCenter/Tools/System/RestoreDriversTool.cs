using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class RestoreDriversTool : IToolCommand
    {
        public string Id => "sys_restore_drivers";
        public string Title => "Restore / Pasang Driver dari Folder Backup";
        public string Description => "Menginstal seluruh file .inf driver dari folder backup secara otomatis ke sistem menggunakan PnPUtil Windows.";
        public string Category => ToolCategory.System;
        public string Keywords => "driver restore install pasang pnputil inf backup masukkan";
        public string Icon => "📥";
        public string ButtonText => "Pasang Driver dari Backup";
        public Color ButtonColor => Color.FromArgb(39, 174, 96); // Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESTORE & INSTALASI DRIVER DARI FOLDER BACKUP ===", LogType.Info);

            string defaultPath = FindLatestBackupFolder();
            string selectedFolder = PromptForFolder(defaultPath);

            if (string.IsNullOrWhiteSpace(selectedFolder) || !Directory.Exists(selectedFolder))
            {
                Logger.Log("Operasi pemasangan driver dibatalkan atau folder tidak ditemukan.", LogType.Info);
                return;
            }

            int infCount = Directory.GetFiles(selectedFolder, "*.inf", SearchOption.AllDirectories).Length;
            if (infCount == 0)
            {
                Logger.Log($"Tidak ditemukan file .inf driver pada direktori: {selectedFolder}", LogType.Warning);
                return;
            }

            Logger.Log($"Menemukan {infCount} paket driver (.inf) di: {selectedFolder}", LogType.Info);
            Logger.Log("Memulai instalasi driver via PnPUtil. Proses ini berjalan di latar belakang...", LogType.Warning);

            await Task.Run(async () =>
            {
                string cmd = $"pnputil /add-driver \"{selectedFolder}\\*.inf\" /subdirs /install";
                int exitCode = await CommandRunner.RunCmdAsync(cmd, s =>
                {
                    if (s.Contains("Processing package") || s.Contains("Driver package added") || s.Contains("Total driver packages"))
                    {
                        Logger.Log(s, LogType.Info);
                    }
                });

                if (exitCode == 0 || exitCode == 259 || exitCode == 3010) // 3010 = reboot required
                {
                    Logger.Log("✅ Seluruh paket driver berhasil dipasang ke sistem!", LogType.Success);
                    if (exitCode == 3010)
                    {
                        Logger.Log("⚠️ Beberapa driver membutuhkan Restart Komputer agar aktif sepenuhnya.", LogType.Warning);
                    }
                }
                else
                {
                    Logger.Log($"Instalasi driver selesai dengan kode status: {exitCode}", LogType.Info);
                }
            });
        }

        private string FindLatestBackupFolder()
        {
            string[] searchRoots = { @"D:\Driver_Backup", @"C:\Driver_Backup" };
            foreach (var root in searchRoots)
            {
                if (Directory.Exists(root))
                {
                    var dirs = new DirectoryInfo(root).GetDirectories().OrderByDescending(d => d.CreationTime).ToList();
                    if (dirs.Any()) return dirs.First().FullName;
                    return root;
                }
            }
            return @"C:\Driver_Backup";
        }

        private string PromptForFolder(string defaultPath)
        {
            string folder = "";
            var thread = new Thread(() =>
            {
                using var dialog = new FolderBrowserDialog();
                dialog.Description = "Pilih Folder Berisi File Driver (.inf) Hasil Backup:";
                dialog.SelectedPath = defaultPath;
                dialog.ShowNewFolderButton = false;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    folder = dialog.SelectedPath;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return folder;
        }
    }
}
