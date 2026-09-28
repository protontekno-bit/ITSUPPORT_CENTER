using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.Network
{
    public class ResetProxyAndHostsTool : IToolCommand
    {
        public string Id => "net_reset_proxy_hosts";
        public string Title => "Reset Proxy Browser (WinINet) & File Hosts Default";
        public string Description => "Memperbaiki 'No Internet' di browser: Matikan proxy tersembunyi/PAC script, reset WinHTTP, dan kembalikan file hosts ke standar bersih.";
        public string Category => ToolCategory.Network;
        public string Keywords => "proxy hosts browser pac wpad inetcpl reset internet settings wininet";
        public string Icon => "🧹";
        public string ButtonText => "Reset Proxy & Hosts";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Pumpkin Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== RESET PROXY BROWSER (WININET) & PEMULIHAN FILE HOSTS ===", LogType.Info);

            await Task.Run(async () =>
            {
                // 1. Reset WinINet Registry Settings
                Logger.Log("1/4: Menonaktifkan Proxy User di Registry (Internet Settings)...");
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);
                    if (key != null)
                    {
                        key.SetValue("ProxyEnable", 0, RegistryValueKind.DWord);
                        try { key.DeleteValue("ProxyServer"); } catch { }
                        try { key.DeleteValue("AutoConfigURL"); } catch { }
                        key.SetValue("ProxyHttp1.1", 1, RegistryValueKind.DWord);
                        Logger.Log("Proxy Browser WinINet & AutoConfigURL berhasil dimatikan.", LogType.Success);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Catatan Registry WinINet: {ex.Message}", LogType.Warning);
                }

                // 2. Reset WinHTTP system proxy
                Logger.Log("2/4: Mereset WinHTTP System Proxy via Netsh...");
                await CommandRunner.RunCmdAsync("netsh winhttp reset proxy", s => Logger.Log(s, LogType.Info));

                // 3. Reset Hosts File
                Logger.Log("3/4: Memulihkan file hosts Windows ke default bersih...");
                string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string hostsPath = Path.Combine(winDir, @"System32\drivers\etc\hosts");

                try
                {
                    if (File.Exists(hostsPath))
                    {
                        string hostsBak = Path.Combine(winDir, $@"System32\drivers\etc\hosts.bak_{DateTime.Now:yyyyMMdd_HHmmss}");
                        File.Copy(hostsPath, hostsBak, true);
                        Logger.Log($"Backup file hosts lama tersimpan di: {hostsBak}", LogType.Info);
                    }

                    var cleanHosts = new StringBuilder();
                    cleanHosts.AppendLine("# Copyright (c) 1993-2009 Microsoft Corp.");
                    cleanHosts.AppendLine("# Standard Clean Windows Hosts File");
                    cleanHosts.AppendLine("#");
                    cleanHosts.AppendLine("127.0.0.1       localhost");
                    cleanHosts.AppendLine("::1             localhost");

                    File.WriteAllText(hostsPath, cleanHosts.ToString(), Encoding.ASCII);
                    Logger.Log("File hosts berhasil dikembalikan ke standar Microsoft!", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal menulis file hosts: {ex.Message}", LogType.Error);
                }

                // 4. Flush DNS & ARP Cache
                Logger.Log("4/4: Membersihkan cache DNS & ARP...");
                await CommandRunner.RunCmdAsync("ipconfig /flushdns", null);
                await CommandRunner.RunCmdAsync("netsh interface ip delete arpcache", null);

                Logger.Log("✅ Seluruh konfigurasi Proxy & File Hosts berhasil dinormalisasi!", LogType.Success);
            });
        }
    }
}
