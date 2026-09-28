using System;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace ITSupportCenter.Core
{
    public class SystemOsInfo
    {
        public string OsName { get; set; } = "Windows";
        public string Version { get; set; } = "";
        public int BuildNumber { get; set; } = 0;
        public bool IsWindows11 => BuildNumber >= 22000;
        public bool IsWindows10 => BuildNumber >= 10240 && BuildNumber < 22000;
        public bool Is64Bit { get; set; } = Environment.Is64BitOperatingSystem;
        public string Architecture => Is64Bit ? "64-Bit" : "32-Bit";
        public double TotalRamGb { get; set; } = 0;
        public string DriveCType { get; set; } = "SSD/HDD";
        public double DriveCFreeGb { get; set; } = 0;
        public double DriveCTotalGb { get; set; } = 0;

        public string SummaryText => $"{OsName} ({Architecture}, Build {BuildNumber}) | RAM: {TotalRamGb:F1} GB | Disk C: {DriveCFreeGb:F1}/{DriveCTotalGb:F1} GB ({DriveCType})";
    }

    public static class OsDetector
    {
        private static SystemOsInfo? _cachedInfo;

        public static SystemOsInfo GetOsInfo()
        {
            if (_cachedInfo != null) return _cachedInfo;

            var info = new SystemOsInfo();

            // 1. Deteksi OS Name & Build Number via Registry CurrentVersion (Akurasi Tinggi)
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                if (key != null)
                {
                    string prodName = key.GetValue("ProductName")?.ToString() ?? "Windows";
                    string displayVer = key.GetValue("DisplayVersion")?.ToString() ?? "";
                    string currentBuild = key.GetValue("CurrentBuild")?.ToString() ?? "0";

                    if (int.TryParse(currentBuild, out int bld))
                    {
                        info.BuildNumber = bld;
                    }

                    // Windows 11 sering tercatat sebagai "Windows 10 Pro" di ProductName registry lama
                    if (info.BuildNumber >= 22000)
                    {
                        info.OsName = prodName.Replace("Windows 10", "Windows 11");
                    }
                    else
                    {
                        info.OsName = prodName;
                    }

                    if (!string.IsNullOrEmpty(displayVer))
                    {
                        info.Version = displayVer;
                    }
                }
            }
            catch { }

            // 2. Deteksi RAM & Drive Info
            try
            {
                var dInfo = new DriveInfo("C");
                info.DriveCFreeGb = dInfo.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                info.DriveCTotalGb = dInfo.TotalSize / (1024.0 * 1024.0 * 1024.0);
            }
            catch { }

            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject mo in searcher.Get())
                {
                    if (ulong.TryParse(mo["TotalPhysicalMemory"]?.ToString(), out ulong memBytes))
                    {
                        info.TotalRamGb = memBytes / (1024.0 * 1024.0 * 1024.0);
                    }
                }
            }
            catch { }

            // 3. Deteksi Tipe Media Penyimpanan (SSD vs HDD)
            try
            {
                using var searcher = new ManagementObjectSearcher(@"root\Microsoft\Windows\Storage", "SELECT MediaType, BusType FROM MSFT_PhysicalDisk");
                foreach (ManagementObject mo in searcher.Get())
                {
                    if (ushort.TryParse(mo["MediaType"]?.ToString(), out ushort mType))
                    {
                        if (mType == 4) info.DriveCType = "SSD";
                        else if (mType == 3) info.DriveCType = "HDD";
                        else if (mType == 5) info.DriveCType = "SCM";
                        break;
                    }
                }
            }
            catch
            {
                info.DriveCType = "SSD/HDD";
            }

            _cachedInfo = info;
            return info;
        }
    }
}
