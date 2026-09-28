using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.Tools.SystemTools;

namespace ITSupportCenter.Tools.Security
{
    public class UserPasswordRecoveryTool : IToolCommand
    {
        public string Id => "sec_user_password_recovery";
        public string Title => "Manajemen & Reset Password User (Lupa Password)";
        public string Description => "Solusi klien lupa password: Audit akun lokal, Buka akun terkunci (Unlock), Reset password baru (net user), dan Ekstrak password WiFi tersimpan.";
        public string Category => ToolCategory.Security;
        public string Keywords => "password user login lupa reset akun unlock net user wifi sam lusrmgr credential";
        public string Icon => "👤";
        public string ButtonText => "Kelola / Reset Password User";
        public Color ButtonColor => Color.FromArgb(192, 57, 43); // Red

        public async Task ExecuteAsync()
        {
            Logger.Log("=== MANAJEMEN AKUN & RESET PASSWORD USER WINDOWS ===", LogType.Info);

            string action = PromptForAction();
            if (string.IsNullOrEmpty(action))
            {
                Logger.Log("Operasi dibatalkan.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                if (action == "AUDIT_USERS")
                {
                    Logger.Log("Audit Daftar Akun Pengguna Lokal (Win32_UserAccount)...", LogType.Info);
                    var sb = new StringBuilder();
                    sb.AppendLine("=========================================================");
                    sb.AppendLine("📋 DAFTAR AKUN PENGGUNA LOKAL (WINDOWS USERS)");
                    sb.AppendLine($"📅 Komputer: {Environment.MachineName} | {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine("=========================================================");

                    try
                    {
                        using var searcher = new ManagementObjectSearcher($"SELECT Name, FullName, Disabled, Lockout, PasswordRequired, PasswordChangeable FROM Win32_UserAccount WHERE LocalAccount = True");
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            string name = obj["Name"]?.ToString() ?? "";
                            string fullName = obj["FullName"]?.ToString() ?? "";
                            bool disabled = (bool)(obj["Disabled"] ?? false);
                            bool lockout = (bool)(obj["Lockout"] ?? false);

                            string status = disabled ? "[NONAKTIF]" : (lockout ? "[TERKUNCI / LOCKED]" : "[AKTIF]");
                            string line = $"• User: {name,-16} | Status: {status,-18} | FullName: {fullName}";
                            sb.AppendLine(line);

                            if (lockout)
                                Logger.Log(line, LogType.Warning);
                            else if (disabled)
                                Logger.Log(line, LogType.Info);
                            else
                                Logger.Log(line, LogType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"WMI User Query: {ex.Message}", LogType.Warning);
                        await CommandRunner.RunCmdAsync("net user", s => Logger.Log(s, LogType.Info));
                    }

                    try { ThreadClipboardHelper.SetClipboardText(sb.ToString()); } catch { }
                }
                else if (action == "RESET_PASSWORD")
                {
                    var (username, newPassword) = PromptForUserAndNewPassword();
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        Logger.Log("Reset password dibatalkan: Nama user tidak boleh kosong.", LogType.Warning);
                        return;
                    }

                    Logger.Log($"Mereset password untuk user '{username}'...", LogType.Info);

                    string cmd = string.IsNullOrEmpty(newPassword)
                        ? $"net user \"{username}\" \"\""
                        : $"net user \"{username}\" \"{newPassword}\"";

                    int exitCode = await CommandRunner.RunCmdAsync(cmd, s => Logger.Log(s, LogType.Info));
                    if (exitCode == 0)
                    {
                        // Ensure account is unlocked and active
                        await CommandRunner.RunCmdAsync($"net user \"{username}\" /active:yes", null);
                        Logger.Log($"✅ SUKSES! Password user '{username}' berhasil diatur ulang dan akun diaktifkan.", LogType.Success);
                    }
                    else
                    {
                        Logger.Log($"Gagal mereset password user '{username}'. Pastikan nama akun benar.", LogType.Error);
                    }
                }
                else if (action == "UNLOCK_ACCOUNT")
                {
                    string username = PromptForSingleInput("Masukkan Nama Akun User yang Terkunci:", Environment.UserName);
                    if (string.IsNullOrWhiteSpace(username)) return;

                    Logger.Log($"Membuka status kunci (Unlock) untuk akun '{username}'...");
                    await CommandRunner.RunCmdAsync($"net user \"{username}\" /active:yes", s => Logger.Log(s, LogType.Info));
                    await CommandRunner.RunCmdAsync($"net user \"{username}\" /lockout:no", s => Logger.Log(s, LogType.Info));
                    Logger.Log($"✅ Akun '{username}' berhasil di-Unlock / diaktifkan kembali!", LogType.Success);
                }
                else if (action == "WIFI_PASSWORDS")
                {
                    Logger.Log("Mengekstrak seluruh Profil & Password WiFi yang tersimpan di PC ini...", LogType.Info);
                    var sbWifi = new StringBuilder();
                    sbWifi.AppendLine("=========================================================");
                    sbWifi.AppendLine("📶 DAFTAR PROFIL & PASSWORD WIFI TERSIMPAN");
                    sbWifi.AppendLine($"📅 Komputer: {Environment.MachineName}");
                    sbWifi.AppendLine("=========================================================");

                    var profiles = new List<string>();
                    await CommandRunner.RunCmdAsync("netsh wlan show profiles", s =>
                    {
                        if (s.Contains("All User Profile") || s.Contains("Profil Semua Pengguna"))
                        {
                            var parts = s.Split(':');
                            if (parts.Length > 1)
                            {
                                string pName = parts[1].Trim();
                                if (!string.IsNullOrEmpty(pName)) profiles.Add(pName);
                            }
                        }
                    });

                    if (profiles.Count == 0)
                    {
                        Logger.Log("Tidak ditemukan profil WiFi tersimpan pada adapter WLAN ini.", LogType.Warning);
                        return;
                    }

                    foreach (var p in profiles)
                    {
                        string pass = "Tidak ada / Open";
                        await CommandRunner.RunCmdAsync($"netsh wlan show profile name=\"{p}\" key=clear", s =>
                        {
                            if (s.Contains("Key Content") || s.Contains("Konten Kunci"))
                            {
                                var parts = s.Split(':');
                                if (parts.Length > 1) pass = parts[1].Trim();
                            }
                        });

                        string entry = $"• SSID: {p,-25} | Password: {pass}";
                        sbWifi.AppendLine(entry);
                        Logger.Log(entry, LogType.Success);
                    }

                    try
                    {
                        ThreadClipboardHelper.SetClipboardText(sbWifi.ToString());
                        Logger.Log("📋 Seluruh password WiFi berhasil disalin ke Clipboard!", LogType.Success);
                    }
                    catch { }
                }
                else if (action == "OPEN_LUSRMGR")
                {
                    Logger.Log("Membuka Konsol Local Users and Groups (lusrmgr.msc)...", LogType.Info);
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "lusrmgr.msc",
                            UseShellExecute = true
                        });
                        Logger.Log("✅ Konsol Manajemen Pengguna Windows terbuka.", LogType.Success);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Gagal membuka lusrmgr.msc: {ex.Message}", LogType.Error);
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
                form.Text = "Manajemen & Pemulihan Password Pengguna";
                form.Width = 440;
                form.Height = 290;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = "Pilih operasi akun login yang diinginkan:", Left = 20, Top = 12, Width = 390 };

                var btnReset = new Button { Text = "🔑 Reset / Atur Password Baru Akun", Left = 20, Top = 40, Width = 380, Height = 32 };
                btnReset.Click += (s, e) => { selection = "RESET_PASSWORD"; form.Close(); };

                var btnUnlock = new Button { Text = "🔓 Buka Kunci Akun Terkunci (Unlock / Active)", Left = 20, Top = 78, Width = 380, Height = 32 };
                btnUnlock.Click += (s, e) => { selection = "UNLOCK_ACCOUNT"; form.Close(); };

                var btnAudit = new Button { Text = "📋 Audit Seluruh Daftar Akun User Lokal", Left = 20, Top = 116, Width = 380, Height = 32 };
                btnAudit.Click += (s, e) => { selection = "AUDIT_USERS"; form.Close(); };

                var btnWifi = new Button { Text = "📶 Ekstrak Semua Password WiFi Tersimpan", Left = 20, Top = 154, Width = 380, Height = 32 };
                btnWifi.Click += (s, e) => { selection = "WIFI_PASSWORDS"; form.Close(); };

                var btnLusrmgr = new Button { Text = "⚙️ Buka GUI Manajemen User (lusrmgr.msc)", Left = 20, Top = 192, Width = 380, Height = 32 };
                btnLusrmgr.Click += (s, e) => { selection = "OPEN_LUSRMGR"; form.Close(); };

                form.Controls.AddRange(new Control[] { label, btnReset, btnUnlock, btnAudit, btnWifi, btnLusrmgr });
                form.ShowDialog();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return selection;
        }

        private (string Username, string Password) PromptForUserAndNewPassword()
        {
            string user = "";
            string pass = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Reset Password User Windows";
                form.Width = 380;
                form.Height = 210;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lblUser = new Label { Text = "Nama Akun Pengguna (Username):", Left = 20, Top = 15, Width = 320 };
                var txtUser = new TextBox { Left = 20, Top = 38, Width = 320, Text = Environment.UserName };

                var lblPass = new Label { Text = "Password Baru (Kosongkan jika tanpa password):", Left = 20, Top = 72, Width = 320 };
                var txtPass = new TextBox { Left = 20, Top = 95, Width = 320, Text = "Office2026!" };

                var btnOk = new Button { Text = "Reset Password", Left = 130, Top = 132, Width = 110, Height = 28, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Batal", Left = 250, Top = 132, Width = 90, Height = 28, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { lblUser, txtUser, lblPass, txtPass, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    user = txtUser.Text.Trim();
                    pass = txtPass.Text;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return (user, pass);
        }

        private string PromptForSingleInput(string prompt, string defaultVal)
        {
            string input = "";
            var thread = new Thread(() =>
            {
                using var form = new Form();
                form.Text = "Input Data";
                form.Width = 360;
                form.Height = 150;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var label = new Label { Text = prompt, Left = 20, Top = 15, Width = 300 };
                var textBox = new TextBox { Left = 20, Top = 40, Width = 300, Text = defaultVal };
                var btnOk = new Button { Text = "OK", Left = 140, Top = 72, Width = 80, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Batal", Left = 230, Top = 72, Width = 80, DialogResult = DialogResult.Cancel };

                form.Controls.AddRange(new Control[] { label, textBox, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    input = textBox.Text.Trim();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return input;
        }
    }
}
