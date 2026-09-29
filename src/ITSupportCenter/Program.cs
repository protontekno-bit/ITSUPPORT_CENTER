using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace ITSupportCenter
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1. Global Exception Handlers to prevent abrupt crash
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                MessageBox.Show($"Terjadi kesalahan sistem yang tidak tertangani:\n{ex?.Message}",
                    "IT Support Center - Error Trap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Application.ThreadException += (s, e) =>
            {
                try
                {
                    string msg = !string.IsNullOrWhiteSpace(e.Exception?.Message)
                        ? e.Exception.Message
                        : e.Exception?.ToString() ?? "Terjadi pengecualian internal antarmuka.";

                    Core.Logger.Log($"[PERINGATAN SISTEM] {msg}", Core.LogType.Warning);
                }
                catch { }
            };

            // 2. Network Share Staging & Self-Healing Launcher
            // When run from network share (UNC \\ or Mapped Drive), stage locally to prevent 0xc0000006 STATUS_IN_PAGE_ERROR
            if (IsRunningFromNetwork())
            {
                try
                {
                    string localDir = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "ITSupportCenter");
                    System.IO.Directory.CreateDirectory(localDir);

                    string currentExe = Process.GetCurrentProcess().MainModule?.FileName ?? Application.ExecutablePath;
                    string localExe = System.IO.Path.Combine(localDir, "ITSupportCenter.exe");

                    bool needsCopy = true;
                    if (System.IO.File.Exists(localExe))
                    {
                        var srcInfo = new System.IO.FileInfo(currentExe);
                        var dstInfo = new System.IO.FileInfo(localExe);
                        if (srcInfo.Length == dstInfo.Length &&
                            Math.Abs((srcInfo.LastWriteTimeUtc - dstInfo.LastWriteTimeUtc).TotalSeconds) < 3)
                        {
                            needsCopy = false;
                        }
                    }

                    if (needsCopy)
                    {
                        System.IO.File.Copy(currentExe, localExe, true);
                    }

                    var psi = new ProcessStartInfo
                    {
                        FileName = localExe,
                        WorkingDirectory = localDir,
                        UseShellExecute = true
                    };

                    if (!IsAdministrator())
                    {
                        psi.Verb = "runas";
                    }

                    Process.Start(psi);
                    return;
                }
                catch
                {
                    // If network staging fails, fall back to in-place execution
                }
            }

            // 3. Administrator Verification & Self-Elevation
            if (!IsAdministrator())
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process.Start(psi);
                    return;
                }
                catch
                {
                    MessageBox.Show(
                        "Aplikasi ini membutuhkan hak akses Administrator untuk mengelola Registry, Services, dan Jaringan.\n\n" +
                        "Silakan Klik Kanan file aplikasi lalu pilih 'Run as Administrator'.",
                        "Hak Administrator Diperlukan",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }

            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsRunningFromNetwork()
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Application.ExecutablePath;
                if (string.IsNullOrWhiteSpace(exePath))
                    return false;

                // 1. Direct UNC Path (e.g. \\192.168.1.100\share\... or \\server\...)
                if (exePath.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase))
                    return true;

                // 2. Network / Shared Drive (e.g. Z:\...)
                string? root = System.IO.Path.GetPathRoot(exePath);
                if (!string.IsNullOrEmpty(root))
                {
                    var driveInfo = new System.IO.DriveInfo(root);
                    if (driveInfo.DriveType == System.IO.DriveType.Network)
                        return true;
                }
            }
            catch
            {
                // Fallback safe
            }
            return false;
        }
    }
}
