using System;
using System.Drawing;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.SystemTools
{
    public class DiskSmartHealthTool : IToolCommand
    {
        public string Id => "sys_disk_smart_health";
        public string Title => "Audit Kesehatan Fisik Disk (S.M.A.R.T)";
        public string Description => "Memeriksa status fisik S.M.A.R.T, deteksi bad sector / degradasi aus SSD/HDD, tipe media (NVMe/SSD/HDD), dan status operasional.";
        public string Category => ToolCategory.System;
        public string Keywords => "disk smart health hdd ssd nvme bad sector rusak wear lifetime storage";
        public string Icon => "🩺";
        public string ButtonText => "Cek S.M.A.R.T Disk";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT KESEHATAN FISIK STORAGE & S.M.A.R.T ===", LogType.Info);

            await Task.Run(async () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("💾 LAPORAN KESEHATAN DISK & PREDIKSI KEGAGALAN S.M.A.R.T");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                int diskIndex = 0;

                // 1. WMI Win32_DiskDrive inspection
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT DeviceID, Model, Status, InterfaceType, Size, MediaType FROM Win32_DiskDrive");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        diskIndex++;
                        string model = obj["Model"]?.ToString() ?? "Unknown Storage";
                        string status = obj["Status"]?.ToString() ?? "OK";
                        string interfaceType = obj["InterfaceType"]?.ToString() ?? "N/A";
                        double sizeGB = 0;
                        if (obj["Size"] != null && double.TryParse(obj["Size"].ToString(), out double rawSize))
                            sizeGB = rawSize / (1024.0 * 1024.0 * 1024.0);

                        sb.AppendLine($"\n[DRIVE #{diskIndex}]");
                        sb.AppendLine($"• Model Disk   : {model}");
                        sb.AppendLine($"• Kapasitas    : {sizeGB:F1} GB ({interfaceType})");
                        sb.AppendLine($"• Status WMI   : {status}");

                        if (status.Equals("OK", StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.Log($"[DISK #{diskIndex}] {model} ({sizeGB:F0} GB) -> Status: SEHAT (OK)", LogType.Success);
                        }
                        else
                        {
                            Logger.Log($"[PERINGATAN] [DISK #{diskIndex}] {model} -> Status: {status} (BERMASALAH)", LogType.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal kueri Win32_DiskDrive: {ex.Message}", LogType.Warning);
                }

                // 2. WMI MSStorageDriver_FailurePredictStatus (S.M.A.R.T Predictive Failure)
                try
                {
                    using var wmiSearcher = new ManagementObjectSearcher(@"root\wmi", "SELECT InstanceName, PredictFailure, Reason FROM MSStorageDriver_FailurePredictStatus");
                    foreach (ManagementObject obj in wmiSearcher.Get())
                    {
                        bool predictFailure = (bool)(obj["PredictFailure"] ?? false);
                        string instance = obj["InstanceName"]?.ToString() ?? "Disk";

                        if (predictFailure)
                        {
                            Logger.Log($"⚠️ BAHAYA S.M.A.R.T: Drive {instance} memprediksi kegagalan fisik (Predict Failure = TRUE)! Segera backup data!", LogType.Error);
                            sb.AppendLine($"🚨 PERINGATAN S.M.A.R.T: {instance} berpotensi rusak fisik segera!");
                        }
                        else
                        {
                            Logger.Log($"S.M.A.R.T Self-Test: {instance} -> Normal / Lolos Uji Mandiri.", LogType.Success);
                        }
                    }
                }
                catch { }

                // 3. PowerShell Get-PhysicalDisk detailed output
                Logger.Log("Memeriksa status operasional via Get-PhysicalDisk...", LogType.Info);
                await CommandRunner.RunCmdAsync("powershell -Command \"Get-PhysicalDisk | Select-Object DeviceId, FriendlyName, MediaType, HealthStatus, OperationalStatus | Format-Table -AutoSize\"", s =>
                {
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        if (s.Contains("Healthy") || s.Contains("OK"))
                            Logger.Log(s, LogType.Success);
                        else if (s.Contains("Unhealthy") || s.Contains("Warning"))
                            Logger.Log(s, LogType.Error);
                        else
                            Logger.Log(s, LogType.Info);
                    }
                });

                Logger.Log("✅ Audit kesehatan disk selesai. Seluruh data disalin ke Clipboard.", LogType.Success);
                try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }
            });
        }
    }
}
