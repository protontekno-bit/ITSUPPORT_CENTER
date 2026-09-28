using System;
using System.Drawing;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class ExtractOemBiosKeyTool : IToolCommand
    {
        public string Id => "license_extract_oem_bios_key";
        public string Title => "Ekstrak Kunci Produk OEM Asli dari BIOS Motherboard";
        public string Description => "Membaca 25-digit lisensi resmi Windows yang tertanam di ACPI MSDM Table motherboard (Dell/HP/Lenovo/Asus) & salin ke clipboard.";
        public string Category => ToolCategory.License;
        public string Keywords => "oem key bios msdm motherboard original product key serial laptop dell hp lenovo asus";
        public string Icon => "🏷️";
        public string ButtonText => "Baca Key BIOS OEM";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== EKSTRAKSI LISENSI OEM ASLI DARI BIOS (MSDM TABLE) ===", LogType.Info);

            await Task.Run(async () =>
            {
                string? oemKey = null;

                // 1. WMI SoftwareLicensingService
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT OA3xOriginalProductKey FROM SoftwareLicensingService");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var val = obj["OA3xOriginalProductKey"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(val))
                        {
                            oemKey = val.Trim();
                            break;
                        }
                    }
                }
                catch { }

                // 2. PowerShell Fallback
                if (string.IsNullOrEmpty(oemKey))
                {
                    await CommandRunner.RunCmdAsync("powershell -Command \"(Get-CimInstance -ClassName SoftwareLicensingService).OA3xOriginalProductKey\"", s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s) && s.Length == 29 && s.Contains("-"))
                        {
                            oemKey = s.Trim();
                        }
                    });
                }

                if (!string.IsNullOrEmpty(oemKey))
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("=========================================================");
                    sb.AppendLine("🔑 KUNCI LISENSI WINDOWS ASLI BAWAAN PABRIK (OEM BIOS)");
                    sb.AppendLine($"📅 Komputer: {Environment.MachineName}");
                    sb.AppendLine("=========================================================");
                    sb.AppendLine($"• OEM Product Key : {oemKey}");
                    sb.AppendLine("=========================================================");
                    sb.AppendLine("💡 Kunci lisensi ini permanen dan terikat pada motherboard perangkat ini.");

                    Logger.Log($"✅ Ditemukan Kunci OEM Asli Pabrik: {oemKey}", LogType.Success);
                    try
                    {
                        ThreadClipboardHelper.SetClipboardText(oemKey);
                        Logger.Log("📋 25-Digit Product Key berhasil disalin ke Clipboard!", LogType.Success);
                    }
                    catch { }
                }
                else
                {
                    Logger.Log("Tidak ditemukan kunci OEM di BIOS (MSDM Table).", LogType.Warning);
                    Logger.Log("Info: Komputer ini kemungkinan adalah PC Rakitan, lisensi Digital Entitlement, atau lisensi Volume KMS/Retail.", LogType.Info);
                }
            });
        }
    }
}
