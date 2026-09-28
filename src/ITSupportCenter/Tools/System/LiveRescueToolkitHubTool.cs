using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class LiveRescueToolkitHubTool : IToolCommand
    {
        public string Id => "sys_live_rescue_hub";
        public string Title => "Pusat Live Rescue & WinPE Hub (Hiren's, DLC, SystemRescue, UBCD)";
        public string Description => "Panduan komparasi & prosedur teknis ekosistem Live Boot Rescue: Hiren's BootCD PE, DLC Boot, SystemRescue, dan UBCD untuk perbaikan darurat.";
        public string Category => ToolCategory.System;
        public string Keywords => "hiren hirens bootcd dlc boot systemrescue ubcd ultimate boot cd winpe live usb rescue recovery password ram disk";
        public string Icon => "🩺";
        public string ButtonText => "Buka Panduan Live Rescue Hub";
        public Color ButtonColor => Color.FromArgb(142, 68, 173); // Wisteria Purple

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT PANDUAN LIVE RESCUE & WINPE TOOLKIT MASTER ===", LogType.Info);

            await Task.Run(() =>
            {
                Logger.Log("\n📋 1. HIREN'S BOOTCD PE (Windows 11/10 PE Bawaan Resmi)", LogType.Success);
                Logger.Log("   • Basis OS: Windows 11/10 64-Bit PE (Mendukung UEFI & Secure Boot).");
                Logger.Log("   • Fungsi Utama: Reset Password Windows (NTFS / SAM), Recovery Data (Recuva, GetDataBack), Antivirus Offline.");
                Logger.Log("   • Kapan Digunakan: Saat Windows tidak bisa booting (BSOD) atau user lupa PIN/Password login.");
                Logger.Log("   • Situs Resmi: https://www.hirensbootcd.org\n");

                Logger.Log("📋 2. DLC BOOT (WinPE Multi-Utility Favorit Teknisi)", LogType.Success);
                Logger.Log("   • Basis OS: Mini Windows 10/11 & Mini Linux.");
                Logger.Log("   • Fungsi Utama: Sangat kaya tool hardware: Hard Disk Sentinel Pro, Victoria HDD, Ghost 32/64, Acronis True Image.");
                Logger.Log("   • Kapan Digunakan: Diagnostik kesehatan fisik HDD/SSD, partisi ulang disk, dan cloning offline.");
                Logger.Log("   • Catatan: Matikan sementara Windows Defender saat membuat USB DLC Boot karena tool password sering dianggap false-positive.\n");

                Logger.Log("📋 3. SYSTEMRESCUE (Linux Live Rescue Berbasis Arch)", LogType.Success);
                Logger.Log("   • Basis OS: Arch Linux Live x86_64.");
                Logger.Log("   • Fungsi Utama: GParted GUI (Resize partisi aman), TestDisk & PhotoRec (Kembalikan partisi terhapus), ddrescue (Copy disk rusak).");
                Logger.Log("   • Kapan Digunakan: Saat partisi berubah menjadi RAW, tabel partisi GPT rusak, atau ingin mem-backup disk dengan bad sector.");
                Logger.Log("   • Situs Resmi: https://www.system-rescue.org\n");

                Logger.Log("📋 4. ULTIMATE BOOT CD / UBCD (Hardware Low-Level Diagnostic)", LogType.Success);
                Logger.Log("   • Basis OS: DOS & Minimal Linux Bootable (Legacy BIOS).");
                Logger.Log("   • Fungsi Utama: MemTest86+ (Uji kerusakan keping RAM), CPU Burn-in / Stress Test, Low-Level Format Disk vendor (SeaTools, WD DLG).");
                Logger.Log("   • Kapan Digunakan: Saat PC sering restart/freeze tanpa sebab jelas (diduga kerusakan hardware fisik RAM/CPU/Motherboard).");
                Logger.Log("   • Situs Resmi: https://www.ultimatebootcd.com\n");

                Logger.Log("💡 TIPS TERBAIK TEKNISI:", LogType.Info);
                Logger.Log("   Gunakan modul 'Ventoy Multi-Boot' untuk memasukkan file ISO Hiren's, DLC Boot, dan SystemRescue ke dalam 1 Flashdisk yang sama!", LogType.Success);
            });
        }
    }
}
