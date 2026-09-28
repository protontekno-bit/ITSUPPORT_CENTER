using System;
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class DiskCloningDeploymentHubTool : IToolCommand
    {
        public string Id => "sys_disk_cloning_hub";
        public string Title => "Pusat Kloning & Deployment (Clonezilla, Rescuezilla, FOG Project)";
        public string Description => "Panduan arsitektur & komparasi teknis kloning disk: Clonezilla (CLI), Rescuezilla (GUI), dan FOG Project (PXE Network Multicast Deploy).";
        public string Category => ToolCategory.System;
        public string Keywords => "clonezilla rescuezilla fog project pxe multicast disk cloning deployment image backup bare metal true nas";
        public string Icon => "💾";
        public string ButtonText => "Buka Panduan Kloning & Deployment";
        public Color ButtonColor => Color.FromArgb(230, 126, 34); // Carrot Orange

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT KLONING DISK & MASS DEPLOYMENT (CLONEZILLA / RESCUEZILLA / FOG) ===", LogType.Info);

            await Task.Run(() =>
            {
                Logger.Log("\n💿 1. CLONEZILLA (Standar Industri Kloning Berbasis CLI)", LogType.Success);
                Logger.Log("   • Metode Kerja: Live Linux Bootable USB (Teks/Curses Wizard).");
                Logger.Log("   • Mode Utama:");
                Logger.Log("     a. [device-image]: Menyimpan seluruh isi SSD ke file image di External Drive atau TrueNAS/Samba Share.");
                Logger.Log("     b. [device-device]: Kloning langsung dari Disk Sumber (HDD lama) ke Disk Target (SSD baru).");
                Logger.Log("   • Kelebihan: Sangat cepat, hemat ruang penyimpanan (hanya meng-copy sector terpakai), mendukung partisi MBR/GPT.");
                Logger.Log("   • Situs Resmi: https://clonezilla.org\n");

                Logger.Log("🐭 2. RESCUEZILLA (\"Clonezilla Versi GUI yang Ramah Pengguna\")", LogType.Success);
                Logger.Log("   • Metode Kerja: Live Linux Ubuntu/Debian dengan antarmuka grafis desktop (Point & Click).");
                Logger.Log("   • Kompatibilitas: 100% Kompatibel dengan file backup Clonezilla (bisa restore image buatan Clonezilla dan sebaliknya).");
                Logger.Log("   • Fitur Tambahan: Sudah dilengkapi browser web (Firefox) di dalam live environment & GParted partisi manager.");
                Logger.Log("   • Kapan Dipilih: Sangat direkomendasikan jika teknisi lebih nyaman dengan tampilan visual mouse daripada CLI teks.");
                Logger.Log("   • Situs Resmi: https://rescuezilla.com\n");

                Logger.Log("🌐 3. FOG PROJECT (Mass PXE Network Deployment Tanpa Flashdisk)", LogType.Success);
                Logger.Log("   • Metode Kerja: Server terpusat (Linux/TrueNAS VM) yang melayani booting komputer lewat kabel LAN (PXE Boot / iPXE).");
                Logger.Log("   • Cara Kerja:");
                Logger.Log("     1. 1 Komputer Master di-setup lengkap dengan OS, Driver, dan Software Kantor.");
                Logger.Log("     2. Server FOG mengambil image master tersebut via kabel LAN (Capture Image).");
                Logger.Log("     3. Server FOG menyebarkan image tersebut ke 10–50+ PC kantor / mesin antrian sekaligus via 'Multicast Deploy'.");
                Logger.Log("   • Kebutuhan Jaringan: Router DHCP Option 66 (Next Server IP) & Option 67 (Bootfile 'ipxe.efi' / 'undionly.kpxe').");
                Logger.Log("   • Kapan Dipilih: Pengadaan lab komputer baru, deployment kantor cabang massal, atau maintenance puluhan mesin serentak.");
                Logger.Log("   • Situs Resmi: https://fogproject.org\n");

                Logger.Log("💡 LANGKAH TERBAIK:", LogType.Info);
                Logger.Log("   • Untuk 1-2 Komputer: Gunakan Rescuezilla / Clonezilla via Flashdisk Ventoy.", LogType.Info);
                Logger.Log("   • Untuk 10-50+ Komputer: Setup Server FOG Project di jaringan TrueNAS/LAN kantor.", LogType.Success);
            });
        }
    }
}
