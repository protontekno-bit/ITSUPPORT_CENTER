# 🤖 PANDUAN PENGEMBANGAN AI & MODULAR ARSITEKTUR (IT SUPPORT CENTER)

Dokumen ini adalah panduan standar bagi **AI Coding Assistant** dan **Pengembang Manusia** untuk memelihara, memodifikasi, dan menambahkan fitur/modul baru ke dalam aplikasi **IT Support Center** tanpa merusak modul yang sudah ada.

---

## 🏛️ 1. Filosofi & Prinsip Arsitektur (Command Pattern)

Aplikasi dibangun menggunakan prinsip **Command / Plugin Pattern** di C# .NET 8 WinForms:
1. **Single Responsibility (1 File = 1 Modul/Tool):** Setiap fitur berdiri sendiri sebagai 1 file class di folder `src/ITSupportCenter/Tools/<Kategori>/`.
2. **Zero-Coupling:** Modul tidak boleh saling bergantung langsung (*decoupled*). Modul hanya berkomunikasi melalui helper bersama di `Core/`.
3. **Dual-Layer Fail-Safe:** Setiap operasi Windows (Registry, Service, Network, Process) wajib memiliki fallback (misal: Win32 API gagal ➔ beralih ke `reg.exe` / `net.exe` / `sc.exe`).
4. **Dynamic UI Rendering:** `MainForm` tidak pernah mem-hardcode tombol atau kartu. Form membaca daftar modul secara dinamis dari `Core/ToolRegistry.cs`.

---

## 📁 2. Struktur Folder & Kode

```
d:\ITTOOLS\
├── ITSupportCenter.exe            # Standalone GUI Executable (Primary Launcher .NET 8)
├── START.bat                      # Master Universal Bootstrapper (Auto Admin Elevation)
├── IT_SUPPORT_CENTER.bat          # Master CLI Terminal Launcher (86 Menu Lengkap)
├── AI_DEV_GUIDE.md                # Dokumen Panduan Arsitektur & Standar Pengembangan
├── installers\                    # Utilitas & Installer Setup pihak ketiga (Nmap, dll.)
│   └── nmap-7.98-setup.exe
├── dist\                          # Output rilis standalone publish
│   ├── ITSupportCenter.exe
│   └── final\
│       └── ITSupportCenter.exe
├── scripts\                       # 64+ Skrip mandiri standalone (.bat, .py, .ps1, .cmd)
└── src\ITSupportCenter\           # Source Code C# .NET 8 WinForms
    ├── Core\                      # Jantung Arsitektur (IToolCommand, ToolRegistry, Logger)
    ├── Services\                  # Background Service & Exporter (Report, History)
    ├── UI\                        # Komponen Antarmuka (MainForm, Cards, Dialogs)
    └── Tools\                     # Modul Alat Mandiri (System, Printer, Network, dll.)
```

---

## 🛠️ 3. Cara Menambahkan Modul Baru dalam 3 Langkah Cepat

Jika Anda (atau AI) diminta menambahkan fitur baru (misal: *Tool Optimasi SSD TRIM*), ikuti langkah berikut:

### Langkah 1: Buat Class Baru di `Tools/<Kategori>/`
Buat file `Tools/System/SsdOptimizationTool.cs` yang mengimplementasikan `IToolCommand`:

```csharp
using System.Drawing;
using System.Threading.Tasks;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class SsdOptimizationTool : IToolCommand
    {
        public string Id => "sys_ssd_optimize";
        public string Title => "Optimasi & TRIM SSD";
        public string Description => "Menjalankan perintah Defrag /O untuk mengoptimalkan performa dan masa pakai SSD.";
        public string Category => ToolCategory.System;
        public string Keywords => "ssd trim optimize defrag disk performa";
        public string Icon => "⚡";
        public string ButtonText => "Optimalkan SSD";
        public Color ButtonColor => Color.FromArgb(41, 128, 185);

        public async Task ExecuteAsync()
        {
            Logger.Log("=== OPTIMASI DRIVE SSD (TRIM) ===", LogType.Info);
            await CommandRunner.RunCmdAsync("defrag C: /O", s => Logger.Log(s, LogType.Info));
            Logger.Log("✅ Optimasi SSD C: selesai!", LogType.Success);
        }
    }
}
```

### Langkah 2: Daftarkan ke `Core/ToolRegistry.cs`
Buka `src/ITSupportCenter/Core/ToolRegistry.cs`, tambahkan 1 baris di method `RegisterDefaults()`:

```csharp
Register(new SsdOptimizationTool());
```

### Langkah 3: Build & Publish
Jalankan perintah publish:
```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o "d:\ITTOOLS\dist"
Copy-Item "d:\ITTOOLS\dist\ITSupportCenter.exe" "d:\ITTOOLS\ITSupportCenter.exe" -Force
```

✨ **Selesai!** Kartu modul baru akan langsung muncul di GUI secara otomatis, lengkap dengan fitur pencarian, filter kategori, penanganan error, dan log warna.

---

## 🛡️ 4. Aturan Wajib untuk AI Developer

1. **JANGAN PERNAH** memanggil `Process.Start` mentah tanpa error handling. Gunakan `CommandRunner.RunCmdAsync(...)` atau `WindowsHelper`.
2. **JANGAN PERNAH** mem-bypass logging. Selalu gunakan `Logger.Log(message, LogType.Success/Warning/Error/Info)`.
3. **JANGAN PERNAH** melakukan *blocking UI thread*. Gunakan `async/await` dan `Task.Run(...)`.
4. **JANGAN PERNAH** menghapus implementasi batch script di `d:\ITTOOLS\scripts\`, karena script tersebut adalah fallback standalone jika PC user tidak dapat menjalankan GUI/EXE.

---

## 📖 5. Ketentuan Wajib: Integrasi Otomatis Fitur Baru ke Manual Aplikasi

Agar setiap fitur/modul baru yang dibuat oleh AI atau Pengembang **OTOMATIS masuk ke Buku Manual Operasional (`OperationalManualForm`) tanpa perlu mengubah kode UI Manual**, Anda **WAJIB** mematuhi 4 kontrak arsitektur berikut:

### Kontrak 1: Properti Standar Lengkap pada `IToolCommand`
Setiap class tool baru wajib mengisi properti antarmuka secara bermakna:
- `Id`: ID unik dengan awalan kategori (misal: `sys_smart_ram_optimizer`, `net_ad_sync`).
- `Title`: Judul resmi bahasa Indonesia yang informatif.
- `Description`: Penjelasan fungsi alat dalam 1-2 kalimat ringkas.
- `Category`: Wajib menggunakan konstanta resmi dari `ToolCategory` (misal: `ToolCategory.System`, `ToolCategory.Printer`, dll.).
- `Keywords`: Kata kunci untuk mempermudah pencarian (search bar).
- `Icon`: Emoji yang merepresentasikan fungsi (misal: `⚡`, `🩺`, `🧹`, `🛡️`).

### Kontrak 2: Registrasi Terpusat di `Core/ToolRegistry.cs`
Daftarkan instance modul di method `RegisterDefaults()`:
```csharp
Register(new ModulBaruTool());
```
✨ **Efek Otomatis:** Begitu terdaftar di `ToolRegistry`, modul akan **langsung dihitung jumlahnya**, **dimuat ke dalam katalog bab manual**, dan **dibuatkan tombol 1-klik eksekusi** pada kategori manual terkait secara otomatis.

### Kontrak 3: Dokumentasi Teknis di `Core/ToolDetailInfoProvider.cs`
Tambahkan entri dokumentasi di dictionary `_details` pada `src/ITSupportCenter/Core/ToolDetailInfoProvider.cs`:
```csharp
["id_modul_baru"] = new ToolDetailInfo
{
    ProblemSolved = "Jelaskan gejala kerusakan atau error yang dituntaskan oleh tool ini.",
    SystemImpact = "Jelaskan perintah sistem, registry, atau service yang dimodifikasi.",
    UsageGuide = "1. Langkah persiapan...\n2. Klik tombol eksekusi...\n3. Tindakan pasca-eksekusi..."
};
```
*(Jika developer belum sempat mengisi di provider, sistem memiliki fallback dinamis yang otomatis merangkum info dari Title dan Description).*

### Kontrak 4: Penanganan Kompatibilitas CLI (Optional Fallback)
Jika modul dapat dijalankan tanpa GUI, sediakan skrip mandiri di `d:\ITTOOLS\scripts\<nama_tool>.bat` dan daftarkan opsi nomornya di `IT_SUPPORT_CENTER.bat`.
