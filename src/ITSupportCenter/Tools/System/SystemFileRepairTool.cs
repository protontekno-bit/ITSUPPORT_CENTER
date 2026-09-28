using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class SystemFileRepairTool : IToolCommand
    {
        public string Id => "sys_file_repair";
        public string Title => "Perbaikan Berkas Sistem (SFC & DISM)";
        public string Description => "Memeriksa dan memperbaiki file Windows yang korup menggunakan SFC /scannow dan DISM Online RestoreHealth.";
        public string Category => ToolCategory.System;
        public string Keywords => "sfc scannow dism restorehealth corrupt file system repair rusak";
        public string Icon => "🩺";
        public string ButtonText => "Jalankan SFC & DISM";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Wisteria Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== REPARASI INTEGRITAS SISTEM WINDOWS (SFC + DISM) ===", LogType.Info);
            Logger.Log("Proses ini membutuhkan waktu beberapa menit. Harap jangan mematikan PC.", LogType.Warning);

            Logger.Log("Tahap 1/2: Menjalankan System File Checker (SFC /scannow)...", LogType.Info);
            int sfcCode = await CommandRunner.RunCmdAsync("sfc /scannow", s =>
            {
                if (s.Contains("%") || s.Contains("Windows Resource") || s.Contains("corrupt"))
                    Logger.Log(s, LogType.Info);
            });

            if (sfcCode == 0)
                Logger.Log("SFC Scannow selesai tanpa kesalahan.", LogType.Success);
            else
                Logger.Log($"SFC selesai dengan kode status: {sfcCode}", LogType.Info);

            Logger.Log("Tahap 2/2: Menjalankan DISM Online Image RestoreHealth...", LogType.Info);
            int dismCode = await CommandRunner.RunCmdAsync("dism /Online /Cleanup-Image /RestoreHealth", s =>
            {
                if (s.Contains("%") || s.Contains("operation completed") || s.Contains("Version"))
                    Logger.Log(s, LogType.Info);
            });

            if (dismCode == 0)
                Logger.Log("✅ Komponen Windows Image berhasil diperbaiki via DISM!", LogType.Success);
            else
                Logger.Log($"DISM selesai dengan kode status: {dismCode}", LogType.Info);

            Logger.Log("Disarankan untuk me-restart komputer setelah perbaikan berkas sistem.", LogType.Warning);
        }
    }
}
