using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;

namespace ITSupportCenter.Core
{
    public static class WindowsHelper
    {
        public static void SetRegistryDWordSafe(string rootPath, string keyPath, string valueName, int value)
        {
            try
            {
                using var baseKey = rootPath.Equals("HKLM", StringComparison.OrdinalIgnoreCase)
                    ? Registry.LocalMachine
                    : Registry.CurrentUser;

                using var key = baseKey.CreateSubKey(keyPath);
                key.SetValue(valueName, value, RegistryValueKind.DWord);
            }
            catch
            {
                string fullKey = $"{rootPath}\\{keyPath}";
                CommandRunner.RunCmdAsync($"reg add \"{fullKey}\" /v \"{valueName}\" /t REG_DWORD /d {value} /f", null).Wait(3000);
            }
        }

        public static void RestartService(string serviceName, string displayName)
        {
            bool success = false;
            try
            {
                using var sc = new ServiceController(serviceName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    Logger.Log($"Menghentikan {displayName}...");
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(8));
                }

                Logger.Log($"Menjalankan kembali {displayName}...");
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(8));
                Logger.Log($"{displayName} berhasil di-restart.", LogType.Success);
                success = true;
            }
            catch
            {
                Logger.Log($"Mencoba metode alternatif (net/sc command) untuk {displayName}...", LogType.Info);
            }

            if (!success)
            {
                try
                {
                    CommandRunner.RunCmdAsync($"net stop {serviceName} /y & timeout /t 2 /nobreak >nul & net start {serviceName}", s => Logger.Log(s, LogType.Info)).Wait(12000);
                    Logger.Log($"{displayName} berhasil di-restart via fallback command.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal me-restart service {displayName}: {ex.Message}", LogType.Warning);
                }
            }
        }

        public static void StopServiceIfExists(string serviceName)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(4));
                }
            }
            catch
            {
                CommandRunner.RunCmdAsync($"net stop {serviceName} /y", null).Wait(4000);
            }
        }

        public static void StartServiceIfExists(string serviceName)
        {
            try
            {
                using var sc = new ServiceController(serviceName);
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(4));
                }
            }
            catch
            {
                CommandRunner.RunCmdAsync($"net start {serviceName}", null).Wait(4000);
            }
        }

        public static void KillProcessIfExists(string processName)
        {
            try
            {
                foreach (var p in Process.GetProcessesByName(processName))
                {
                    try { p.Kill(); p.WaitForExit(2000); } catch { }
                }
            }
            catch { }
        }
    }
}
