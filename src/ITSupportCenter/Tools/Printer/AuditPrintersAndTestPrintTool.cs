using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Printer
{
    public class AuditPrintersAndTestPrintTool : IToolCommand
    {
        public string Id => "printer_audit_test_print";
        public string Title => "Audit Daftar Printer & Cetak Halaman Uji (Test Page)";
        public string Description => "Menampilkan daftar seluruh printer (Port IP/WSD, Driver, Status Online/Offline) dan tombol 1-klik mengirim Windows Test Page.";
        public string Category => ToolCategory.Printer;
        public string Keywords => "printer test page print cetak daftar status online offline port wsd ip driver";
        public string Icon => "🩺";
        public string ButtonText => "Cek Printer & Test Print";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== AUDIT DAFTAR PRINTER & CETAK HALAMAN UJI ===", LogType.Info);

            await Task.Run(async () =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("=========================================================");
                sb.AppendLine("🖨️ DAFTAR PRINTER TERPASANG DI KOMPUTER");
                sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=========================================================");

                var printerNames = new List<string>();

                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT Name, DriverName, PortName, Default, PrinterStatus, WorkOffline FROM Win32_Printer");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string name = obj["Name"]?.ToString() ?? "";
                        string driver = obj["DriverName"]?.ToString() ?? "";
                        string port = obj["PortName"]?.ToString() ?? "";
                        bool isDefault = (bool)(obj["Default"] ?? false);
                        bool isOffline = (bool)(obj["WorkOffline"] ?? false);

                        if (!string.IsNullOrEmpty(name))
                            printerNames.Add(name);

                        string statusStr = isOffline ? "[OFFLINE]" : "[ONLINE / READY]";
                        string defBadge = isDefault ? "⭐ [DEFAULT]" : "";

                        string entry = $"• Printer: {name} {defBadge}\n   - Status : {statusStr}\n   - Port   : {port}\n   - Driver : {driver}\n";
                        sb.AppendLine(entry);

                        if (isOffline)
                            Logger.Log($"[OFFLINE] {name} (Port: {port})", LogType.Warning);
                        else
                            Logger.Log($"[READY] {name} {defBadge} (Port: {port})", LogType.Success);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Gagal kueri WMI Printer: {ex.Message}", LogType.Warning);
                    foreach (string printer in PrinterSettings.InstalledPrinters)
                    {
                        printerNames.Add(printer);
                        Logger.Log($"• Printer: {printer}", LogType.Info);
                    }
                }

                sb.AppendLine("=========================================================");
                try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }

                if (printerNames.Count == 0)
                {
                    Logger.Log("Tidak ditemukan printer yang terpasang di komputer ini.", LogType.Warning);
                    return;
                }

                // Prompt user to select a printer for Test Page
                string selectedPrinter = PromptForTestPrint(printerNames);
                if (!string.IsNullOrEmpty(selectedPrinter))
                {
                    Logger.Log($"Mengirim perintah Windows Test Page ke '{selectedPrinter}'...", LogType.Info);
                    int exitCode = await CommandRunner.RunCmdAsync($"rundll32.exe printui.dll,PrintUIEntry /k /n \"{selectedPrinter}\"", null);
                    if (exitCode == 0)
                    {
                        Logger.Log($"✅ Halaman uji coba cetak berhasil dikirim ke printer '{selectedPrinter}'!", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"Gagal mengirim test print ke '{selectedPrinter}'. Pastikan printer dalam keadaan hidup.", LogType.Warning);
                    }
                }
            });
        }

        private string PromptForTestPrint(List<string> printers)
        {
            string chosen = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Cetak Halaman Uji Coba (Test Page)";
                form.Width = 420;
                form.Height = 220;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih Printer yang ingin diuji cetak Test Page:", Left = 20, Top = 15, Width = 360 };
                var combo = new ComboBox { Left = 20, Top = 40, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
                foreach (var p in printers) combo.Items.Add(p);
                if (combo.Items.Count > 0) combo.SelectedIndex = 0;

                var btnPrint = new Button { Text = "🖨️ Cetak Test Page", Left = 160, Top = 85, Width = 130, Height = 32, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Batal", Left = 300, Top = 85, Width = 80, Height = 32, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, combo, btnPrint, btnCancel });
                form.AcceptButton = btnPrint;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK && combo.SelectedItem != null)
                {
                    chosen = combo.SelectedItem.ToString() ?? "";
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return chosen;
        }
    }
}
