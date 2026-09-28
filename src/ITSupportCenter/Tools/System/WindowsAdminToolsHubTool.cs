using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.SystemTools
{
    public class WindowsAdminToolsHubTool : IToolCommand
    {
        public string Id => "sys_admin_tools_hub";
        public string Title => "Pusat Alat Administrasi & Diagnosa Windows (Admin Hub)";
        public string Description => "Pusat akses 1-klik ke alat diagnosa resmi Windows: Resource Monitor, Event Viewer, Computer Management, System Restore, & Registry Editor.";
        public string Category => ToolCategory.System;
        public string Keywords => "admin tools hub resmon eventvwr compmgmt rstrui regedit diskmgmt services devmgmt perfmon";
        public string Icon => "🎛️";
        public string ButtonText => "Buka Admin Hub";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT ALAT ADMINISTRASI & DIAGNOSA WINDOWS ===", LogType.Info);

            string toolToOpen = PromptForAdminTool();
            if (string.IsNullOrEmpty(toolToOpen)) return;

            await Task.Run(() =>
            {
                try
                {
                    Logger.Log($"Membuka {toolToOpen}...");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = toolToOpen,
                        UseShellExecute = true
                    });
                    Logger.Log($"✅ {toolToOpen} berhasil diluncurkan.", LogType.Success);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal membuka {toolToOpen}: {ex.Message}", LogType.Error);
                }
            });
        }

        private string PromptForAdminTool()
        {
            string target = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pusat Alat Diagnosa & Administrasi Windows";
                form.Width = 520;
                form.Height = 310;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih perkakas diagnosa Windows yang ingin dibuka:", Left = 20, Top = 12, Width = 460 };

                // Row 1
                var btnResmon = CreateToolButton("📊 Resource Monitor", "resmon.exe", 20, 38, form, t => target = t);
                var btnEventvwr = CreateToolButton("📋 Event Viewer (Crash Logs)", "eventvwr.msc", 260, 38, form, t => target = t);

                // Row 2
                var btnCompmgmt = CreateToolButton("🖥️ Computer Management", "compmgmt.msc", 20, 78, form, t => target = t);
                var btnDevmgmt = CreateToolButton("🔌 Device Manager", "devmgmt.msc", 260, 78, form, t => target = t);

                // Row 3
                var btnDiskmgmt = CreateToolButton("💾 Disk Management", "diskmgmt.msc", 20, 118, form, t => target = t);
                var btnServices = CreateToolButton("⚙️ Windows Services", "services.msc", 260, 118, form, t => target = t);

                // Row 4
                var btnRstrui = CreateToolButton("🕒 System Restore (rstrui)", "rstrui.exe", 20, 158, form, t => target = t);
                var btnRegedit = CreateToolButton("🔑 Registry Editor (regedit)", "regedit.exe", 260, 158, form, t => target = t);

                // Row 5
                var btnPerfmon = CreateToolButton("📈 Performance Monitor", "perfmon.msc", 20, 198, form, t => target = t);
                var btnMsinfo = CreateToolButton("ℹ️ System Information (msinfo32)", "msinfo32.exe", 260, 198, form, t => target = t);

                form.Controls.AddRange(new Control[]
                {
                    label, btnResmon, btnEventvwr, btnCompmgmt, btnDevmgmt,
                    btnDiskmgmt, btnServices, btnRstrui, btnRegedit, btnPerfmon, btnMsinfo
                });

                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return target;
        }

        private Button CreateToolButton(string text, string targetCmd, int left, int top, Form form, Action<string> onSelect)
        {
            var btn = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = 225,
                Height = 34,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.Click += (s, e) =>
            {
                onSelect(targetCmd);
                form.Close();
            };
            return btn;
        }
    }
}
