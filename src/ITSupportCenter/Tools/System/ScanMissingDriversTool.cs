using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class ScanMissingDriversTool : IToolCommand
    {
        public string Id => "sys_scan_missing_drivers";
        public string Title => "Audit & Pindai Driver Hilang / Rusak";
        public string Description => "Mendeteksi hardware berstatus Tanda Seru Kuning (Error Code 28, 10, 43), ekstraksi Hardware ID (VEN/DEV), & auto-salin ke clipboard.";
        public string Category => ToolCategory.System;
        public string Keywords => "driver scan missing device manager hardware id pnp ven dev kuning tanda seru sdio";
        public string Icon => "🔍";
        public string ButtonText => "Pindai Driver Hilang";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT PERANGKAT & DRIVER BERMASALAH (PNP SCAN) ===", LogType.Info);

            await Task.Run(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("📋 LAPORAN AUDIT DRIVER & HARDWARE BERMASALAH");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | Tanggal: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                int problemCount = 0;

                try
                {
                    using var searcher = new ManagementObjectSearcher(
                        "SELECT Name, Description, PNPDeviceID, ConfigManagerErrorCode, Status FROM Win32_PnPEntity WHERE ConfigManagerErrorCode > 0");

                    foreach (ManagementObject obj in searcher.Get())
                    {
                        problemCount++;
                        string name = obj["Name"]?.ToString() ?? obj["Description"]?.ToString() ?? "Unknown Device";
                        string pnpId = obj["PNPDeviceID"]?.ToString() ?? "N/A";
                        uint errorCode = 0;
                        if (obj["ConfigManagerErrorCode"] != null)
                            uint.TryParse(obj["ConfigManagerErrorCode"].ToString(), out errorCode);

                        string errorExplanation = GetErrorCodeExplanation(errorCode);

                        sb.AppendLine($"\n[PERANGKAT #{problemCount}]");
                        sb.AppendLine($"• Nama Perangkat  : {name}");
                        sb.AppendLine($"• Kode Masalah    : Code {errorCode} ({errorExplanation})");
                        sb.AppendLine($"• Hardware PNP ID : {pnpId}");

                        Logger.Log($"[TANDA SERU] #{problemCount}: {name}", LogType.Warning);
                        Logger.Log($"   -> Masalah: Code {errorCode} ({errorExplanation})", LogType.Warning);
                        Logger.Log($"   -> Hardware ID: {pnpId}", LogType.Info);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal kueri WMI PnPEntity: {ex.Message}", LogType.Error);
                }

                if (problemCount == 0)
                {
                    Logger.Log("✅ SEMPURNA! Tidak ditemukan driver yang hilang atau tanda seru kuning di Device Manager.", LogType.Success);
                    sb.AppendLine("\n✅ Seluruh driver hardware terpasang dengan baik (0 Driver Bermasalah).");
                }
                else
                {
                    Logger.Log($"⚠️ Terdeteksi {problemCount} perangkat yang membutuhkan instalasi/perbaikan driver.", LogType.Warning);
                    sb.AppendLine("\n=========================================================");
                    sb.AppendLine("💡 TIPS: Gunakan Hardware PNP ID di atas untuk mencari driver di Microsoft Update Catalog.");
                    
                    // Copy report to clipboard
                    ThreadClipboardHelper.SetClipboardText(sb.ToString());
                    Logger.Log("📋 Data Hardware ID perangkat bermasalah berhasil disalin ke Clipboard!", LogType.Success);
                }

                // Check for SDIO / SDI tool presence in working directory or USB
                string? sdioExe = FindSdioExecutable();
                if (!string.IsNullOrEmpty(sdioExe))
                {
                    Logger.Log($"Ditemukan tool Open Source Snappy Driver Installer: {sdioExe}", LogType.Success);
                    Logger.Log("Membuka Snappy Driver Installer Origin...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = sdioExe,
                            UseShellExecute = true
                        });
                    }
                    catch { }
                }
                else
                {
                    Logger.Log("Info: Tool offline Snappy Driver Installer (SDIO) dapat diletakkan di folder aplikasi untuk integrasi auto-install.", LogType.Info);
                }
            });
        }

        private string GetErrorCodeExplanation(uint code) => code switch
        {
            1 => "Perangkat tidak terkonfigurasi dengan benar (Code 1)",
            10 => "Perangkat tidak dapat dinyalakan / Start failed (Code 10)",
            18 => "Harap install ulang driver perangkat ini (Code 18)",
            28 => "Driver belum terpasang / Driver missing (Code 28)",
            31 => "Windows tidak dapat memuat driver perangkat (Code 31)",
            39 => "Driver korup atau tidak ditemukan berkas biner (Code 39)",
            43 => "Perangkat dihentikan karena melaporkan masalah / Crash (Code 43)",
            _ => $"Status kode error Device Manager: {code}"
        };

        private string? FindSdioExecutable()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] potentialPaths =
            {
                Path.Combine(baseDir, "SDIO_x64.exe"),
                Path.Combine(baseDir, "SDIO_R760_x64.exe"),
                Path.Combine(baseDir, "SDIO.exe"),
                Path.Combine(baseDir, "SDI_x64.exe"),
                Path.Combine(baseDir, "sdio", "SDIO_x64.exe"),
                @"D:\SDIO\SDIO_x64.exe",
                @"D:\ITTOOLS\SDIO_x64.exe"
            };

            foreach (var path in potentialPaths)
            {
                if (File.Exists(path)) return path;
            }
            return null;
        }
    }
}
