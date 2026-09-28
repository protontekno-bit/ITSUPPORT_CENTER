using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.Printer
{
    public class OpenDevicesAndPrintersClassicTool : IToolCommand
    {
        public string Id => "printer_open_classic_control";
        public string Title => "Pusat Kontrol Printer Klasik (Devices and Printers)";
        public string Description => "Pintasan cepat membuka Devices and Printers klasik Windows 7/10, Print Server Properties (Kelola Port IP), dan Print Management.";
        public string Category => ToolCategory.Printer;
        public string Keywords => "control printers devices and printers classic print server properties printui printmanagement";
        public string Icon => "🎛️";
        public string ButtonText => "Buka Kontrol Printer";
        public Color ButtonColor => Color.FromArgb(52, 73, 94); // Wet Asphalt

        public async Task ExecuteAsync()
        {
            Logger.Log("=== PUSAT KONTROL PRINTER KLASIK & SERVER PROPERTIES ===", LogType.Info);

            string choice = PromptForAction();
            if (string.IsNullOrEmpty(choice)) return;

            await Task.Run(() =>
            {
                if (choice == "CLASSIC_PRINTERS")
                {
                    Logger.Log("Membuka Devices and Printers Klasik...");
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = "shell:::{A8A91A66-3A7D-4424-8D24-04E180695C5A}",
                            UseShellExecute = true
                        });
                        Logger.Log("✅ Devices and Printers klasik terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal: {ex.Message}", LogType.Error);
                    }
                }
                else if (choice == "PRINT_SERVER_PROPS")
                {
                    Logger.Log("Membuka Print Server Properties (Kelola Port IP, Driver, & Forms)...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "rundll32.exe",
                            Arguments = "printui.dll,PrintUIEntry /s",
                            UseShellExecute = true
                        });
                        Logger.Log("✅ Print Server Properties terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal: {ex.Message}", LogType.Error);
                    }
                }
                else if (choice == "PRINT_MANAGEMENT")
                {
                    Logger.Log("Membuka Print Management Console (printmanagement.msc)...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = "printmanagement.msc", UseShellExecute = true });
                        Logger.Log("✅ Print Management Console terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal: {ex.Message}", LogType.Error);
                    }
                }
                else if (choice == "PRINTER_DIAGNOSTIC")
                {
                    Logger.Log("Membuka Windows Printer Troubleshooter bawaan...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = "msdt.exe", Arguments = "/id PrinterDiagnostic", UseShellExecute = true });
                        Logger.Log("✅ Windows Printer Diagnostic terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal: {ex.Message}", LogType.Error);
                    }
                }
            });
        }

        private string PromptForAction()
        {
            string selection = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pusat Kontrol Printer Windows";
                form.Width = 440;
                form.Height = 250;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih perkakas pengelola printer yang ingin dibuka:", Left = 20, Top = 12, Width = 390 };

                var btnClassic = new Button { Text = "🖨️ Buka Devices and Printers Klasik (Windows 7/10)", Left = 20, Top = 38, Width = 380, Height = 34 };
                btnClassic.Click += (s, e) => { selection = "CLASSIC_PRINTERS"; form.Close(); };

                var btnProps = new Button { Text = "⚙️ Buka Print Server Properties (Kelola Port IP & Driver)", Left = 20, Top = 78, Width = 380, Height = 34 };
                btnProps.Click += (s, e) => { selection = "PRINT_SERVER_PROPS"; form.Close(); };

                var btnMgmt = new Button { Text = "🖥️ Buka Print Management Console (printmanagement.msc)", Left = 20, Top = 118, Width = 380, Height = 34 };
                btnMgmt.Click += (s, e) => { selection = "PRINT_MANAGEMENT"; form.Close(); };

                var btnDiag = new Button { Text = "🩺 Buka Windows Printer Diagnostic Troubleshooter", Left = 20, Top = 158, Width = 380, Height = 34 };
                btnDiag.Click += (s, e) => { selection = "PRINTER_DIAGNOSTIC"; form.Close(); };

                form.Controls.AddRange(new Control[] { label, btnClassic, btnProps, btnMgmt, btnDiag });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return selection;
        }
    }
}
