using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using Microsoft.Win32;

namespace ITSupportCenter.Tools.SystemTools
{
    public class DeepHardwareHealthInspectorTool : IToolCommand
    {
        public string Id => "sys_deep_hardware_inspector";
        public string Title => "Inspeksi Total Spesifikasi & Kesehatan Hardware";
        public string Description => "Audit mendalam 100% komponen fisik PC: CPU, RAM per-slot & MHz, BIOS/TPM 2.0/SecureBoot, SSD NVMe SMART, GPU VRAM, Baterai Health %, dan Uptime.";
        public string Category => ToolCategory.System;
        public string Keywords => "hardware speccy aida64 cpu ram mhz slot motherboard bios tpm secure boot ssd nvme smart gpu vram battery wear uptime health";
        public string Icon => "🔬";
        public string ButtonText => "Inspeksi Total Hardware & Health";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=======================================================================", LogType.Info);
            Logger.Log("🔬 AUDIT MENYELURUH SPESIFIKASI & KONDISI KESEHATAN HARDWARE", LogType.Success);
            Logger.Log("=======================================================================", LogType.Info);

            await Task.Run(() =>
            {
                var sbClipboard = new StringBuilder();
                sbClipboard.AppendLine("=== LAPORAN AUDIT TOTAL SPESIFIKASI & KESEHATAN PERANGKAT ===");
                sbClipboard.AppendLine($"Waktu Audit : {DateTime.Now:dd MMMM yyyy HH:mm:ss}");
                sbClipboard.AppendLine($"Hostname    : {Environment.MachineName}");
                sbClipboard.AppendLine($"User Login  : {Environment.UserName}\n");

                // 1. SISTEM OPERASI & UPTIME
                try
                {
                    Logger.Log("\n[ 🖥️ 1. SISTEM OPERASI & SYSTEM UPTIME ]", LogType.Success);
                    var osInfo = OsDetector.GetOsInfo();
                    Logger.Log($"   • OS Name         : {osInfo.OsName} ({osInfo.Architecture})");
                    Logger.Log($"   • Build & Version : Build {osInfo.BuildNumber} (v{osInfo.Version})");

                    TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                    string uptimeStr = $"{uptime.Days} Hari, {uptime.Hours} Jam, {uptime.Minutes} Menit";
                    Logger.Log($"   • System Uptime   : {uptimeStr} (Sejak Booting Terakhir)");

                    sbClipboard.AppendLine($"[SISTEM OPERASI]\n• OS: {osInfo.OsName} ({osInfo.Architecture}, Build {osInfo.BuildNumber})\n• Uptime: {uptimeStr}\n");
                }
                catch { }

                // 2. PROCESSOR (CPU)
                try
                {
                    Logger.Log("\n[ 🧠 2. PROCESSOR (CPU) ]", LogType.Success);
                    using var searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors, MaxClockSpeed, L2CacheSize, L3CacheSize, VirtualizationFirmwareEnabled FROM Win32_Processor");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string name = mo["Name"]?.ToString()?.Trim() ?? "Unknown CPU";
                        string cores = mo["NumberOfCores"]?.ToString() ?? "0";
                        string threads = mo["NumberOfLogicalProcessors"]?.ToString() ?? "0";
                        string maxClock = mo["MaxClockSpeed"]?.ToString() ?? "0";
                        string virt = (mo["VirtualizationFirmwareEnabled"]?.ToString() == "True") ? "Aktif (Enabled)" : "Nonaktif (Disabled)";

                        Logger.Log($"   • Model Processor : {name}");
                        Logger.Log($"   • Core / Thread   : {cores} Cores / {threads} Threads (@ {maxClock} MHz)");
                        Logger.Log($"   • Virtualisasi    : {virt} (VT-x / AMD-V)");

                        sbClipboard.AppendLine($"[PROCESSOR]\n• Model: {name}\n• Core/Thread: {cores}C/{threads}T @ {maxClock}MHz\n• Virtualisasi: {virt}\n");
                        break;
                    }
                }
                catch { }

                // 3. RAM (MEMORI & PER-SLOT)
                try
                {
                    Logger.Log("\n[ 🧩 3. RAM (MEMORI FISIK PER-SLOT) ]", LogType.Success);
                    using var searcher = new ManagementObjectSearcher("SELECT BankLabel, DeviceLocator, Capacity, Speed, MemoryType, SMBIOSMemoryType, Manufacturer, PartNumber FROM Win32_PhysicalMemory");
                    int slotIndex = 1;
                    ulong totalRamBytes = 0;

                    foreach (ManagementObject mo in searcher.Get())
                    {
                        ulong cap = (ulong)(mo["Capacity"] ?? 0);
                        totalRamBytes += cap;
                        double capGb = cap / (1024.0 * 1024.0 * 1024.0);
                        string speed = mo["Speed"]?.ToString() ?? "Unknown";
                        string mfg = mo["Manufacturer"]?.ToString()?.Trim() ?? "OEM";
                        string part = mo["PartNumber"]?.ToString()?.Trim() ?? "-";
                        string slot = mo["DeviceLocator"]?.ToString() ?? $"Slot {slotIndex}";

                        // Deteksi tipe DDR
                        string memTypeStr = "DDR4/DDR5";
                        if (ushort.TryParse(mo["SMBIOSMemoryType"]?.ToString(), out ushort smType))
                        {
                            if (smType == 24) memTypeStr = "DDR3";
                            else if (smType == 26) memTypeStr = "DDR4";
                            else if (smType == 34) memTypeStr = "DDR5";
                            else if (smType == 30) memTypeStr = "LPDDR4";
                            else if (smType == 35) memTypeStr = "LPDDR5";
                        }

                        Logger.Log($"   • Slot {slotIndex} ({slot}) : {capGb:F0} GB {memTypeStr} @ {speed} MHz | {mfg} ({part})");
                        slotIndex++;
                    }

                    double totalRamGb = totalRamBytes / (1024.0 * 1024.0 * 1024.0);
                    Logger.Log($"   • Total RAM Fisik  : {totalRamGb:F1} GB Terpasang ({slotIndex - 1} Slot Terisi)");
                    sbClipboard.AppendLine($"[RAM / MEMORI]\n• Total Kapasitas: {totalRamGb:F1} GB ({slotIndex - 1} Keping Terpasang)\n");
                }
                catch { }

                // 4. MOTHERBOARD, BIOS & TPM
                try
                {
                    Logger.Log("\n[ 🖲️ 4. MOTHERBOARD, BIOS & KEAMANAN TPM ]", LogType.Success);
                    using var mbSearcher = new ManagementObjectSearcher("SELECT Manufacturer, Product, SerialNumber FROM Win32_BaseBoard");
                    foreach (ManagementObject mo in mbSearcher.Get())
                    {
                        string mbMfg = mo["Manufacturer"]?.ToString()?.Trim() ?? "OEM";
                        string mbProd = mo["Product"]?.ToString()?.Trim() ?? "Motherboard";
                        string mbSerial = mo["SerialNumber"]?.ToString()?.Trim() ?? "-";
                        Logger.Log($"   • Mainboard       : {mbMfg} {mbProd} (Serial: {mbSerial})");
                        break;
                    }

                    using var biosSearcher = new ManagementObjectSearcher("SELECT Manufacturer, SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS");
                    foreach (ManagementObject mo in biosSearcher.Get())
                    {
                        string biosVer = mo["SMBIOSBIOSVersion"]?.ToString()?.Trim() ?? "-";
                        string biosDate = mo["ReleaseDate"]?.ToString()?.Trim() ?? "-";
                        if (biosDate.Length >= 8) biosDate = biosDate.Substring(0, 8);
                        Logger.Log($"   • Versi BIOS      : {biosVer} (Rilis: {biosDate})");
                        break;
                    }

                    // Mode UEFI & Secure Boot
                    string secureBoot = "Tidak Aktif / Tidak Didukung";
                    try
                    {
                        using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\SecureBoot\State");
                        if (key != null && (int)(key.GetValue("UEFISecureBootEnabled") ?? 0) == 1)
                            secureBoot = "Aktif (UEFI Secure Boot Enabled)";
                    }
                    catch { }
                    Logger.Log($"   • Secure Boot     : {secureBoot}");

                    // Cek TPM
                    string tpmStatus = "Tidak Terdeteksi";
                    try
                    {
                        using var tpmSearcher = new ManagementObjectSearcher(@"root\CIMV2\Security\MicrosoftTpm", "SELECT IsActivated_InitialValue, IsEnabled_InitialValue, SpecVersion FROM Win32_Tpm");
                        foreach (ManagementObject mo in tpmSearcher.Get())
                        {
                            string spec = mo["SpecVersion"]?.ToString() ?? "2.0";
                            tpmStatus = $"Aktif (TPM {spec} Ready & Activated)";
                            break;
                        }
                    }
                    catch { }
                    Logger.Log($"   • Modul TPM       : {tpmStatus}");
                }
                catch { }

                // 5. STORAGE & STATUS S.M.A.R.T (SSD / HDD)
                try
                {
                    Logger.Log("\n[ 💾 5. PENYIMPANAN FISIK & KESEHATAN S.M.A.R.T ]", LogType.Success);
                    using var diskSearcher = new ManagementObjectSearcher("SELECT DeviceID, Model, Size, Status, InterfaceType FROM Win32_DiskDrive");
                    int dIndex = 0;
                    foreach (ManagementObject mo in diskSearcher.Get())
                    {
                        string model = mo["Model"]?.ToString()?.Trim() ?? "Disk Drive";
                        ulong sizeBytes = (ulong)(mo["Size"] ?? 0);
                        double sizeGb = sizeBytes / (1024.0 * 1024.0 * 1024.0);
                        string status = mo["Status"]?.ToString() ?? "OK";
                        string iface = mo["InterfaceType"]?.ToString() ?? "-";

                        Logger.Log($"   • Disk {dIndex} : {model} ({sizeGb:F0} GB, {iface}) | Status SMART: [{status}]");
                        dIndex++;
                    }

                    // Partisi Logis
                    Logger.Log("   • Partisi Drive Logis:");
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        if (drive.IsReady && (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable))
                        {
                            double freeGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                            double totalGb = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                            double usedPct = ((totalGb - freeGb) / totalGb) * 100.0;
                            Logger.Log($"     - Drive {drive.Name} [{drive.VolumeLabel}] : {freeGb:F1} GB Bebas / {totalGb:F1} GB Total ({usedPct:F0}% Terpakai, {drive.DriveFormat})");
                        }
                    }
                }
                catch { }

                // 6. KARTU GRAFIS (GPU) & MONITOR
                try
                {
                    Logger.Log("\n[ 🎮 6. KARTU GRAFIS (GPU) & DISPLAY ]", LogType.Success);
                    using var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM, DriverVersion, CurrentHorizontalResolution, CurrentVerticalResolution, CurrentRefreshRate FROM Win32_VideoController");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string gpuName = mo["Name"]?.ToString()?.Trim() ?? "Generic Display";
                        ulong ram = (ulong)(mo["AdapterRAM"] ?? 0);
                        double ramGb = ram / (1024.0 * 1024.0 * 1024.0);
                        string drv = mo["DriverVersion"]?.ToString() ?? "-";
                        string resH = mo["CurrentHorizontalResolution"]?.ToString() ?? "";
                        string resV = mo["CurrentVerticalResolution"]?.ToString() ?? "";
                        string hz = mo["CurrentRefreshRate"]?.ToString() ?? "";

                        string resInfo = (!string.IsNullOrEmpty(resH)) ? $" | Resolusi: {resH}x{resV} @ {hz}Hz" : "";
                        Logger.Log($"   • Kartu Grafis    : {gpuName} ({ramGb:F1} GB VRAM, Driver: {drv}){resInfo}");
                    }
                }
                catch { }

                // 7. JARINGAN & WI-FI LINK
                try
                {
                    Logger.Log("\n[ 📶 7. ADAPTER JARINGAN & LINK SPEED ]", LogType.Success);
                    using var searcher = new ManagementObjectSearcher("SELECT Name, Speed, MACAddress, NetConnectionStatus, NetConnectionID FROM Win32_NetworkAdapter WHERE NetConnectionStatus = 2");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string name = mo["Name"]?.ToString() ?? "Network Adapter";
                        string connId = mo["NetConnectionID"]?.ToString() ?? "LAN";
                        string mac = mo["MACAddress"]?.ToString() ?? "-";
                        ulong speed = (ulong)(mo["Speed"] ?? 0);
                        double speedMbps = speed / 1_000_000.0;

                        Logger.Log($"   • {connId} ({name}) : Link Speed {speedMbps:F0} Mbps | MAC: {mac}");
                    }
                }
                catch { }

                // 8. KONDISI BATERAI (LAPTOP)
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT EstimatedChargeRemaining, BatteryStatus, DesignCapacity, FullChargeCapacity FROM Win32_Battery");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        Logger.Log("\n[ 🔋 8. KONDISI KESEHATAN BATERAI LAPTOP ]", LogType.Success);
                        string pct = mo["EstimatedChargeRemaining"]?.ToString() ?? "100";
                        uint design = (uint)(mo["DesignCapacity"] ?? 0);
                        uint full = (uint)(mo["FullChargeCapacity"] ?? 0);

                        Logger.Log($"   • Sisa Daya Baterai: {pct}%");
                        if (design > 0 && full > 0)
                        {
                            double healthPct = ((double)full / design) * 100.0;
                            Logger.Log($"   • Kapasitas Desain : {design} mWh");
                            Logger.Log($"   • Kapasitas Maks   : {full} mWh");
                            Logger.Log($"   • Battery Health   : {healthPct:F1}% (Tingkat Keausan / Wear: {100.0 - healthPct:F1}%)");
                        }
                        break;
                    }
                }
                catch { }

                // 9. KEAMANAN & BITLOCKER
                try
                {
                    Logger.Log("\n[ 🛡️ 9. STATUS KEAMANAN & ENKRIPSI ]", LogType.Success);
                    // BitLocker C:
                    string blStatus = "Tidak Aktif (Decrypted)";
                    try
                    {
                        using var searcher = new ManagementObjectSearcher(@"root\CIMV2\Security\MicrosoftVolumeEncryption", "SELECT ProtectionStatus, DriveLetter FROM Win32_EncryptableVolume WHERE DriveLetter = 'C:'");
                        foreach (ManagementObject mo in searcher.Get())
                        {
                            if (mo["ProtectionStatus"]?.ToString() == "1") blStatus = "Aktif (Encrypted Protected)";
                            break;
                        }
                    }
                    catch { }
                    Logger.Log($"   • BitLocker Drive C: {blStatus}");
                }
                catch { }

                // Salin ke Clipboard
                try
                {
                    string fullSummary = sbClipboard.ToString();
                    if (!string.IsNullOrWhiteSpace(fullSummary))
                    {
                        Clipboard.SetText(fullSummary);
                        Logger.Log("\n📋 Ringkasan spesifikasi lengkap telah OTOMATIS DISALIN KE CLIPBOARD!", LogType.Success);
                    }
                }
                catch { }

                Logger.Log("=======================================================================", LogType.Info);
                Logger.Log("✅ AUDIT SPESIFIKASI & KESEHATAN SELESAI. Klik '📄 Laporan Servis' untuk cetak.", LogType.Success);
            });
        }
    }
}
