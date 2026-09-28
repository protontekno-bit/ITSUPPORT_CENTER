using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class VentoyMultiBootGuideTool : IToolCommand
    {
        public string Id => "sys_ventoy_multiboot_guide";
        public string Title => "Pusat Flashdisk Multi-Boot Master (Ventoy Multi-ISO)";
        public string Description => "Panduan & arsitektur pembuatan 1 Flashdisk Teknisi Serbaguna (Ventoy) untuk menampung seluruh file ISO tanpa perlu format ulang.";
        public string Category => ToolCategory.System;
        public string Keywords => "ventoy multi boot usb flashdisk iso windows porteus kiosk clonezilla rescuezilla hiren dlc boot";
        public string Icon => "🚀";
        public string ButtonText => "Buka Panduan Master USB Ventoy";
        public Color ButtonColor => Color.FromArgb(46, 204, 113); // Emerald Green

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PANDUAN PEMBUATAN MASTER FLASHDISK MULTI-BOOT (VENTOY) ===", LogType.Info);

            await Task.Run(() =>
            {
                Logger.Log("🚀 MENGAPA VENTOY WAJIB UNTUK TEKNISI IT?", LogType.Success);
                Logger.Log("   • Format Flashdisk Hanya 1 Kali Seumur Hidup (Mendukung GPT/UEFI & MBR/Legacy).");
                Logger.Log("   • Untuk menambah sistem operasi baru, Anda CUKUP COPY-PASTE file .ISO langsung ke Flashdisk seperti memindahkan file biasa!\n");

                Logger.Log("📂 REKOMENDASI STRUKTUR FOLDER FLASHDISK MASTER TEKNISI (64 GB / 128 GB):", LogType.Info);
                Logger.Log("   USB_VENTOY (Drive E:\\)");
                Logger.Log("   ├── 📁 01_Windows_Installers/");
                Logger.Log("   │    ├── Win11_23H2_IndoPro_x64.iso");
                Logger.Log("   │    └── Win10_22H2_LTSC_IoT_x64.iso");
                Logger.Log("   ├── 📁 02_Kiosk_Specialist/");
                Logger.Log("   │    └── Porteus-Kiosk-6.2.0-x86_64.iso     <-- (Untuk Mesin Tiket Antrian)");
                Logger.Log("   ├── 📁 03_Disk_Cloning/");
                Logger.Log("   │    ├── rescuezilla-2.5-64bit.iso          <-- (Kloning SSD via GUI)");
                Logger.Log("   │    └── clonezilla-live-amd64.iso          <-- (Kloning SSD via CLI)");
                Logger.Log("   ├── 📁 04_Live_Rescue_WinPE/");
                Logger.Log("   │    ├── HBCD_PE_x64.iso                    <-- (Bypass password & BSOD fix)");
                Logger.Log("   │    ├── DLC_Boot_2023.iso                  <-- (Hardware diagnostics & Mini Win)");
                Logger.Log("   │    └── systemrescue-11.00-amd64.iso       <-- (GParted partisi & data recovery)");
                Logger.Log("   └── 📁 05_Portable_Tools/");
                Logger.Log("        └── ITTOOLS/                           <-- (Folder aplikasi IT Support Center ini!)\n");

                Logger.Log("🔧 LANGKAH PEMASANGAN VENTOY KE FLASHDISK:", LogType.Info);
                Logger.Log("   1. Unduh Ventoy resmi dari situs: https://www.ventoy.net (Pilih 'ventoy-windows.zip').");
                Logger.Log("   2. Ekstrak dan jalankan 'Ventoy2Disk.exe'.");
                Logger.Log("   3. Pilih Device Flashdisk Anda -> Masuk ke menu 'Option' -> 'Partition Style' -> Pilih 'GPT' (atau 'MBR' untuk PC lama).");
                Logger.Log("   4. Klik tombol 'Install'.");
                Logger.Log("   5. Setelah selesai, copy file ISO apa saja ke dalam flashdisk. Saat booting, Ventoy akan otomatis memunculkan menu pilihan ISO yang indah!\n");

                Logger.Log("🎉 Dengan kombinasi Flashdisk Ventoy + folder ITTOOLS di dalamnya, Anda memiliki Toolkit Teknisi IT terlengkap dan paling modern!", LogType.Success);
            });
        }
    }
}
