using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class CleanKmsAndResetTokensTool : IToolCommand
    {
        public string Id => "license_clean_kms_tokens";
        public string Title => "Pembersih Residu Server KMS & Reset Token Lisensi";
        public string Description => "Menghapus server KMS asing/rusak dari registry, memperbaiki tokens.dat korup, dan memulihkan file lisensi sistem via slmgr /rilc.";
        public string Category => ToolCategory.License;
        public string Keywords => "kms ckms tokens dat sppsvc rilc activation error 0xc004c003 0x80070005 reset lisensi";
        public string Icon => "🧹";
        public string ButtonText => "Bersihkan KMS & Reset Token";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PEMBERSIHAN RESIDU KMS & PERBAIKAN TOKEN LISENSI ===", LogType.Info);

            await Task.Run(async () =>
            {
                // 1. Clear KMS host and port from registry
                Logger.Log("1/4: Menghapus server KMS eksternal dari registry (slmgr /ckms)...", LogType.Info);
                await CommandRunner.RunCmdAsync("cscript //nologo %windir%\\system32\\slmgr.vbs /ckms", s => Logger.Log(s, LogType.Info));
                await CommandRunner.RunCmdAsync("cscript //nologo %windir%\\system32\\slmgr.vbs /clearrearm", null);

                // 2. Stop Software Protection Service (sppsvc)
                Logger.Log("2/4: Menghentikan Software Protection Service (sppsvc)...", LogType.Info);
                WindowsHelper.StopServiceIfExists("sppsvc");
                await Task.Delay(1000);

                // 3. Reset tokens.dat
                Logger.Log("3/4: Memeriksa dan mereset cache tokens.dat yang korup...", LogType.Info);
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string tokenStore = Path.Combine(winDir, @"System32\spp\store\2.0");
                string tokenFile = Path.Combine(tokenStore, "tokens.dat");

                try
                {
                    if (File.Exists(tokenFile))
                    {
                        string tokenBak = Path.Combine(tokenStore, $"tokens.bak_{DateTime.Now:yyyyMMddHHmmss}");
                        File.Move(tokenFile, tokenBak);
                        Logger.Log("File cache tokens.dat berhasil di-reset & di-backup.", LogType.Success);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Catatan tokens.dat: {ex.Message}", LogType.Warning);
                }

                // 4. Start sppsvc and run slmgr /rilc
                Logger.Log("4/4: Menjalankan kembali sppsvc & Menginstal ulang file lisensi sistem (slmgr /rilc)...", LogType.Info);
                WindowsHelper.StartServiceIfExists("sppsvc");
                await Task.Delay(1000);

                await CommandRunner.RunCmdAsync("cscript //nologo %windir%\\system32\\slmgr.vbs /rilc", s => Logger.Log(s, LogType.Info));

                Logger.Log("✅ Pembersihan KMS dan reset token lisensi selesai! Silakan periksa status aktivasi Windows.", LogType.Success);
            });
        }
    }
}
