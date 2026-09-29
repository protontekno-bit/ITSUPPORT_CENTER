using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class CleanSystemTempTool : IToolCommand
    {
        public string Id => "sys_clean_temp";
        public string Title => "Pembersih Sampah & File Temporary";
        public string Description => "Membersihkan file sampah %TEMP%, Windows Temp, Prefetch, dan crash dumps untuk melegakan penyimpanan C:\\.";
        public string Category => ToolCategory.System;
        public string Keywords => "clean temp temporary sampah storage disk c cleanup junk prefetch";
        public string Icon => "🧹";
        public string ButtonText => "Bersihkan Temp & Sampah";
        public Color ButtonColor => Color.FromArgb(39, 174, 96); // Nephritis Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMBERSIHAN FILE SEMENTARA & SAMPAH DISK ===", LogType.Info);

            await Task.Run(() =>
            {
                long totalBytesCleaned = 0;
                int filesDeleted = 0;

                string userTemp = Path.GetTempPath();
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string winTemp = Path.Combine(winDir, "Temp");
                string prefetch = Path.Combine(winDir, "Prefetch");

                CleanFolder(userTemp, "User Temp (%TEMP%)", ref totalBytesCleaned, ref filesDeleted);
                CleanFolder(winTemp, "Windows Temp (C:\\Windows\\Temp)", ref totalBytesCleaned, ref filesDeleted);
                CleanFolder(prefetch, "Windows Prefetch", ref totalBytesCleaned, ref filesDeleted);

                double mbCleaned = totalBytesCleaned / (1024.0 * 1024.0);
                Logger.Log($"✅ Pembersihan selesai! {filesDeleted} file dihapus, total {mbCleaned:F2} MB ruang disk berhasil dilegakan.", LogType.Success);
            });
        }

        private void CleanFolder(string folderPath, string displayName, ref long totalBytesCleaned, ref int filesDeleted)
        {
            if (!Directory.Exists(folderPath)) return;

            Logger.Log($"Memeriksa {displayName}...");
            try
            {
                var dirInfo = new DirectoryInfo(folderPath);
                foreach (var file in dirInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        if (file.Name.StartsWith("ITSupportCenter", StringComparison.OrdinalIgnoreCase))
                            continue;

                        long len = file.Length;
                        file.Delete();
                        totalBytesCleaned += len;
                        filesDeleted++;
                    }
                    catch
                    {
                        // File might be in use by active app, skip safely
                    }
                }

                foreach (var subDir in dirInfo.GetDirectories())
                {
                    try
                    {
                        if (subDir.Name.Equals(".net", StringComparison.OrdinalIgnoreCase) ||
                            subDir.Name.IndexOf("ITSupportCenter", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            continue;
                        }

                        subDir.Delete(true);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Catatan saat akses {displayName}: {ex.Message}", LogType.Info);
            }
        }
    }
}
