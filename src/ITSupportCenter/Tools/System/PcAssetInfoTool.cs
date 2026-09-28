using System;
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
    public class PcAssetInfoTool : IToolCommand
    {
        public string Id => "sys_pc_asset_info";
        public string Title => "Audit Info & Serial Number PC";
        public string Description => "Deteksi otomatis Spesifikasi Lengkap, Serial Number (WMI/BIOS), CPU, RAM, & Storage untuk Tagging Aset IT.";
        public string Category => ToolCategory.System;
        public string Keywords => "spec spesifikasi serial number asset tagging wmi processor ram harddisk";
        public string Icon => "🏷️";
        public string ButtonText => "Cek Info & Salin Aset";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT SPESIFIKASI & SERIAL NUMBER PC ===", LogType.Info);

            await Task.Run(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("========================================");
                sb.AppendLine("📋 LAPORAN ASET & SPESIFIKASI KOMPUTER");
                sb.AppendLine($"📅 Tanggal Audit: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("========================================");

                // OS & Hostname
                string hostName = Environment.MachineName;
                string userName = Environment.UserName;
                string osName = GetWmiProperty("Win32_OperatingSystem", "Caption") ?? Environment.OSVersion.ToString();
                string osArch = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

                sb.AppendLine($"• Nama Komputer   : {hostName}");
                sb.AppendLine($"• User Login Aktif : {userName}");
                sb.AppendLine($"• Sistem Operasi   : {osName} ({osArch})");

                // Motherboard & Serial Number
                string manufacturer = GetWmiProperty("Win32_BaseBoard", "Manufacturer") ?? "Unknown";
                string product = GetWmiProperty("Win32_BaseBoard", "Product") ?? "Unknown";
                string serialNumber = GetWmiProperty("Win32_BIOS", "SerialNumber") ?? "Unknown";
                string systemModel = GetWmiProperty("Win32_ComputerSystem", "Model") ?? $"{manufacturer} {product}";

                sb.AppendLine($"• Model Perangkat : {systemModel}");
                sb.AppendLine($"• Serial Number   : {serialNumber}");

                // Processor
                string cpuName = GetWmiProperty("Win32_Processor", "Name") ?? "Unknown";
                sb.AppendLine($"• Processor (CPU) : {cpuName.Trim()}");

                // Memory (RAM)
                try
                {
                    double totalRamBytes = 0;
                    using var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (obj["Capacity"] != null && double.TryParse(obj["Capacity"].ToString(), out double cap))
                            totalRamBytes += cap;
                    }
                    double totalRamGB = totalRamBytes / (1024 * 1024 * 1024);
                    sb.AppendLine($"• Total RAM       : {totalRamGB:F1} GB");
                }
                catch
                {
                    sb.AppendLine("• Total RAM       : Gagal mendeteksi WMI RAM");
                }

                // Storage Drives
                sb.AppendLine("• Penyimpanan Disk:");
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable))
                    {
                        double totalGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                        double freeGB = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                        double usedPercent = ((totalGB - freeGB) / totalGB) * 100.0;
                        sb.AppendLine($"   - Drive {drive.Name} [{drive.VolumeLabel}] : Bebas {freeGB:F1} GB dari {totalGB:F1} GB ({usedPercent:F0}% terpakai)");
                    }
                }

                sb.AppendLine("========================================");
                string report = sb.ToString();

                foreach (var line in report.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains("Serial Number"))
                            Logger.Log(line, LogType.Success);
                        else
                            Logger.Log(line, LogType.Info);
                    }
                }

                try
                {
                    // Copy to clipboard
                    ThreadClipboardHelper.SetClipboardText(report);
                    Logger.Log("✅ Data spesifikasi & Serial Number berhasil disalin ke Clipboard!", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal menyalin ke clipboard: {ex.Message}", LogType.Warning);
                }
            });
        }

        private string? GetWmiProperty(string wmiClass, string property)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {wmiClass}");
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj[property] != null)
                        return obj[property].ToString();
                }
            }
            catch { }
            return null;
        }
    }

    internal static class ThreadClipboardHelper
    {
        public static void SetClipboardText(string text)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    Clipboard.SetText(text);
                }
                catch { }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}
