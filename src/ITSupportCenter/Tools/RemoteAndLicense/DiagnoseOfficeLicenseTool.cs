using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;

namespace ITSupportCenter.Tools.RemoteAndLicense
{
    public class DiagnoseOfficeLicenseTool : IToolCommand
    {
        public string Id => "license_diagnose_office";
        public string Title => "Diagnosa Lisensi Microsoft Office (OSPP.VBS)";
        public string Description => "Mendeteksi edisi lisensi Office 2016/2019/2021/365, status masa aktif, 5-digit key terakhir, dan opsi hapus lisensi bentrok.";
        public string Category => ToolCategory.License;
        public string Keywords => "office word excel lisensi ospp vbs unpkey dstatus unlicensed product activation 365 2021 2019 2016";
        public string Icon => "📑";
        public string ButtonText => "Diagnosa Lisensi Office";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== DIAGNOSA STATUS LISENSI MICROSOFT OFFICE (OSPP.VBS) ===", LogType.Info);

            string? osppPath = FindOsppVbs();
            if (string.IsNullOrEmpty(osppPath))
            {
                Logger.Log("Tidak ditemukan file OSPP.VBS. Microsoft Office desktop mungkin belum terpasang di direktori standar.", LogType.Warning);
                return;
            }

            Logger.Log($"Ditemukan mesin lisensi Office di: {osppPath}", LogType.Info);

            string choice = PromptForOption();
            if (string.IsNullOrEmpty(choice)) return;

            await Task.Run(async () =>
            {
                if (choice == "STATUS")
                {
                    Logger.Log("Membaca status lisensi seluruh edisi Office (/dstatus)...", LogType.Info);
                    await CommandRunner.RunCmdAsync($"cscript //nologo \"{osppPath}\" /dstatus", s =>
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            if (s.Contains("LICENSE STATUS:  --- LICENSED ---"))
                                Logger.Log(s.Trim(), LogType.Success);
                            else if (s.Contains("LICENSE STATUS:  --- NOTIFICATIONS ---") || s.Contains("--- GRACE ---"))
                                Logger.Log(s.Trim(), LogType.Warning);
                            else if (s.Contains("PRODUCT ID:") || s.Contains("Last 5 characters of installed product key:"))
                                Logger.Log(s.Trim(), LogType.Info);
                            else
                                Logger.Log(s.Trim(), LogType.Info);
                        }
                    });
                }
                else if (choice == "UNPKEY")
                {
                    string partialKey = PromptForPartialKey();
                    if (string.IsNullOrWhiteSpace(partialKey) || partialKey.Length != 5)
                    {
                        Logger.Log("Operasi dibatalkan. Kunci lisensi parsial harus terdiri dari 5 karakter.", LogType.Warning);
                        return;
                    }

                    Logger.Log($"Mencopot 5-digit lisensi Office: {partialKey} (/unpkey:{partialKey})...");
                    await CommandRunner.RunCmdAsync($"cscript //nologo \"{osppPath}\" /unpkey:{partialKey}", s => Logger.Log(s, LogType.Info));
                    Logger.Log($"✅ Lisensi parsial '{partialKey}' berhasil dicopot dari sistem!", LogType.Success);
                }
            });
        }

        private string? FindOsppVbs()
        {
            string[] searchPaths =
            {
                @"C:\Program Files\Microsoft Office\Office16\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\Office16\OSPP.VBS",
                @"C:\Program Files\Microsoft Office\Office15\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\Office15\OSPP.VBS",
                @"C:\Program Files\Microsoft Office\Office14\OSPP.VBS",
                @"C:\Program Files (x86)\Microsoft Office\Office14\OSPP.VBS"
            };

            foreach (var p in searchPaths)
            {
                if (File.Exists(p)) return p;
            }
            return null;
        }

        private string PromptForOption()
        {
            string selection = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Pilih Operasi Lisensi Office";
                form.Width = 380;
                form.Height = 180;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih tindakan lisensi Office yang diinginkan:", Left = 20, Top = 15, Width = 320 };

                var btnStatus = new Button { Text = "📑 Cek Status Lisensi Terpasang (/dstatus)", Left = 20, Top = 42, Width = 320, Height = 32 };
                btnStatus.Click += (s, e) => { selection = "STATUS"; form.Close(); };

                var btnUnpkey = new Button { Text = "🗑️ Hapus 5-Digit Key Office yang Bentrok (/unpkey)", Left = 20, Top = 82, Width = 320, Height = 32 };
                btnUnpkey.Click += (s, e) => { selection = "UNPKEY"; form.Close(); };

                form.Controls.AddRange(new Control[] { label, btnStatus, btnUnpkey });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return selection;
        }

        private string PromptForPartialKey()
        {
            string key = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Hapus 5-Digit Kunci Lisensi Office";
                form.Width = 360;
                form.Height = 150;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Masukkan 5 Karakter Terakhir Product Key Office:", Left = 20, Top = 15, Width = 310 };
                var textBox = new TextBox { Left = 20, Top = 40, Width = 300, MaxLength = 5 };
                var buttonOk = new Button { Text = "Hapus Key", Left = 130, Width = 95, Top = 70, DialogResult = DialogResult.OK };
                var buttonCancel = new Button { Text = "Batal", Left = 235, Width = 85, Top = 70, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    key = textBox.Text.Trim().ToUpper();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return key;
        }
    }
}
