using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ITSupportCenter.Core;
using ITSupportCenter.UI;

namespace ITSupportCenter.Tools.Network
{
    public class ActiveDirectoryDomainAssistantTool : IToolCommand
    {
        public string Id => "net_ad_domain_assistant";
        public string Title => "Asisten Active Directory & Join Domain";
        public string Description => "Asisten cepat manajemen Domain Active Directory: Audit status domain, ganti nama komputer, gabung (Join Domain), keluar (Unjoin), dan diagnosa Domain Controller.";
        public string Category => ToolCategory.Network;
        public string Keywords => "active directory domain join domain controller rename computer workgroup ad sysdm ldap kerberos";
        public string Icon => "🏢";
        public string ButtonText => "Asisten Active Directory";
        public Color ButtonColor => Color.FromArgb(41, 128, 185); // Belize Hole Blue

        public async Task ExecuteAsync()
        {
            Logger.Log("=== ASISTEN ACTIVE DIRECTORY & JOIN DOMAIN KANTOR ===", LogType.Info);

            string? choice = InputDialog.Show(
                Form.ActiveForm,
                "Asisten Active Directory & Domain",
                "Pilih Operasi Domain yang Diinginkan:\n" +
                "1 = Audit Status Domain & Nama Komputer Saat Ini\n" +
                "2 = Ganti Nama Komputer (Rename Computer)\n" +
                "3 = Gabung ke Domain Active Directory (Join Domain)\n" +
                "4 = Keluar dari Domain (Unjoin / Pindah ke Workgroup)\n" +
                "5 = Diagnosa Koneksi & Port ke Domain Controller (DC Test)\n" +
                "6 = Buka Pengaturan Nama Komputer Klasik (sysdm.cpl)",
                "1"
            );

            if (string.IsNullOrWhiteSpace(choice))
            {
                Logger.Log("ℹ️ Operasi dibatalkan oleh pengguna.", LogType.Info);
                return;
            }

            await Task.Run(async () =>
            {
                switch (choice.Trim())
                {
                    case "1":
                        await AuditDomainStatusAsync();
                        break;
                    case "2":
                        await RenameComputerAsync();
                        break;
                    case "3":
                        await JoinDomainAsync();
                        break;
                    case "4":
                        await UnjoinDomainAsync();
                        break;
                    case "5":
                        await TestDomainControllerAsync();
                        break;
                    case "6":
                        await OpenSysdmCplAsync();
                        break;
                    default:
                        Logger.Log("⚠️ Pilihan menu tidak valid.", LogType.Warning);
                        break;
                }
            });
        }

        private async Task AuditDomainStatusAsync()
        {
            Logger.Log("Mengaudit status Domain dan Nama Komputer...", LogType.Info);
            string ps = @"
                $cs = Get-CimInstance Win32_ComputerSystem
                $net = Get-CimInstance Win32_NetworkAdapterConfiguration | Where-Object { $_.IPEnabled -eq $true } | Select-Object -First 1
                Write-Host '=================================================='
                Write-Host ('Nama Komputer      : ' + $env:COMPUTERNAME)
                Write-Host ('Part of Domain     : ' + $cs.PartOfDomain)
                Write-Host ('Domain / Workgroup : ' + $cs.Domain)
                Write-Host ('Logon Server (DC)  : ' + $env:LOGONSERVER)
                Write-Host ('User Aktif         : ' + $env:USERDOMAIN + '\' + $env:USERNAME)
                Write-Host ('DNS Domain         : ' + $net.DNSDomain)
                Write-Host ('DNS Server Search  : ' + ($net.DNSServerSearchOrder -join ', '))
                Write-Host '=================================================='
            ";
            await CommandRunner.RunPowerShellAsync(ps, line => Logger.Log(line, LogType.Info));
            Logger.Log("✅ Audit Status Domain Selesai.", LogType.Success);
        }

        private async Task RenameComputerAsync()
        {
            Logger.Log("=== GANTI NAMA KOMPUTER (RENAME COMPUTER) ===", LogType.Info);
            string currentName = Environment.MachineName;

            string? newName = InputDialog.Show(
                Form.ActiveForm,
                "Ganti Nama Komputer",
                $"Nama Komputer Saat Ini: [{currentName}]\nMasukkan Nama Komputer Baru (maksimal 15 karakter standar NetBIOS):",
                ""
            );

            if (string.IsNullOrWhiteSpace(newName))
            {
                Logger.Log("ℹ️ Penggantian nama komputer dibatalkan.", LogType.Info);
                return;
            }

            newName = newName.Trim().ToUpperInvariant();
            Logger.Log($"Mengganti nama komputer dari [{currentName}] menjadi [{newName}]...", LogType.Info);

            string ps = $"Rename-Computer -NewName '{newName}' -Force -ErrorAction Stop";
            int res = await CommandRunner.RunPowerShellAsync(ps, line => Logger.Log(line, LogType.Info));

            if (res == 0)
            {
                Logger.Log($"🎉 Nama komputer berhasil diubah menjadi [{newName}]!", LogType.Success);
                Logger.Log("⚠️ Catatan: Perubahan nama komputer membutuhkan RESTART agar berlaku secara penuh.", LogType.Warning);
            }
            else
            {
                Logger.Log("❌ Gagal mengubah nama komputer. Pastikan hak Administrator terpenuhi dan nama valid.", LogType.Error);
            }
        }

        private async Task JoinDomainAsync()
        {
            Logger.Log("=== GABUNG KE DOMAIN ACTIVE DIRECTORY (JOIN DOMAIN) ===", LogType.Info);

            string? domainName = InputDialog.Show(
                Form.ActiveForm,
                "Nama Domain Active Directory",
                "Masukkan Nama Domain Lengkap Kantor (contoh: corp.internal.net atau kantor.local):",
                "corp.internal.net"
            );

            if (string.IsNullOrWhiteSpace(domainName))
            {
                Logger.Log("ℹ️ Proses Join Domain dibatalkan.", LogType.Info);
                return;
            }

            string? adminUser = InputDialog.Show(
                Form.ActiveForm,
                "Username Domain Admin",
                $"Masukkan Akun Administrator Domain yang berwenang memasukkan PC ke {domainName} (contoh: Administrator atau domain\\admin_user):",
                "Administrator"
            );

            if (string.IsNullOrWhiteSpace(adminUser))
            {
                Logger.Log("ℹ️ Username kosong, proses dibatalkan.", LogType.Info);
                return;
            }

            string? adminPass = InputDialog.Show(
                Form.ActiveForm,
                "Password Domain Admin",
                $"Masukkan Password untuk akun [{adminUser}]:",
                "",
                isPassword: true
            );

            if (string.IsNullOrWhiteSpace(adminPass))
            {
                Logger.Log("ℹ️ Password kosong, proses dibatalkan.", LogType.Info);
                return;
            }

            string? ouPath = InputDialog.Show(
                Form.ActiveForm,
                "OU (Organizational Unit) - Opsional",
                "Masukkan Target OU Path (Opsional / Kosongkan jika ingin masuk default Computers container):\nContoh: OU=Workstations,DC=corp,DC=internal,DC=net",
                ""
            );

            Logger.Log($"Menghubungkan komputer [{Environment.MachineName}] ke Domain [{domainName}]...", LogType.Info);

            string ouArg = string.IsNullOrWhiteSpace(ouPath) ? "" : $"-OUPath '{ouPath.Trim()}'";
            string ps = $@"
                try {{
                    $secPass = ConvertTo-SecureString '{adminPass.Replace("'", "''")}' -AsPlainText -Force
                    $cred = New-Object System.Management.Automation.PSCredential('{adminUser}', $secPass)
                    Add-Computer -DomainName '{domainName.Trim()}' -Credential $cred {ouArg} -Restart:$false -Force -ErrorAction Stop
                    Write-Host '[SUCCESS] Komputer berhasil digabungkan ke Domain $domainName!'
                }} catch {{
                    Write-Host ('[ERROR] ' + $_.Exception.Message)
                    exit 1
                }}
            ";

            int res = await CommandRunner.RunPowerShellAsync(ps, line =>
            {
                if (line.Contains("[SUCCESS]"))
                    Logger.Log(line, LogType.Success);
                else if (line.Contains("[ERROR]"))
                    Logger.Log(line, LogType.Error);
                else
                    Logger.Log(line, LogType.Info);
            });

            if (res == 0)
            {
                Logger.Log($"🎉 BERHASIL! Komputer telah resmi bergabung ke Active Directory Domain [{domainName}].", LogType.Success);
                Logger.Log("⚠️ Silakan RESTART komputer agar login dengan akun domain dapat digunakan.", LogType.Warning);
            }
            else
            {
                Logger.Log("❌ Gagal bergabung ke Domain. Periksa: 1. DNS Server harus mengarah ke Domain Controller, 2. Kredensial Admin, 3. Nama domain.", LogType.Error);
            }
        }

        private async Task UnjoinDomainAsync()
        {
            Logger.Log("=== KELUAR DARI DOMAIN (UNJOIN / WORKGROUP) ===", LogType.Info);

            string? workgroupName = InputDialog.Show(
                Form.ActiveForm,
                "Pindah ke Workgroup",
                "Masukkan Nama Workgroup Baru (contoh: WORKGROUP):",
                "WORKGROUP"
            );

            if (string.IsNullOrWhiteSpace(workgroupName))
            {
                Logger.Log("ℹ️ Operasi dibatalkan.", LogType.Info);
                return;
            }

            string? adminUser = InputDialog.Show(
                Form.ActiveForm,
                "Kredensial Domain Admin",
                "Masukkan Akun Administrator Domain yang berwenang mencabut PC (atau Administrator Lokal):",
                "Administrator"
            );

            string? adminPass = InputDialog.Show(
                Form.ActiveForm,
                "Password Admin",
                "Masukkan Password:",
                "",
                isPassword: true
            );

            Logger.Log($"Memindahkan komputer ke Workgroup [{workgroupName}]...", LogType.Info);

            string ps = $@"
                try {{
                    if ('{adminPass ?? ""}' -ne '') {{
                        $secPass = ConvertTo-SecureString '{(adminPass ?? "").Replace("'", "''")}' -AsPlainText -Force
                        $cred = New-Object System.Management.Automation.PSCredential('{adminUser}', $secPass)
                        Remove-Computer -UnjoinDomainCredential $cred -WorkgroupName '{workgroupName.Trim()}' -Restart:$false -Force -ErrorAction Stop
                    }} else {{
                        Remove-Computer -WorkgroupName '{workgroupName.Trim()}' -Restart:$false -Force -ErrorAction Stop
                    }}
                    Write-Host '[SUCCESS] Komputer berhasil dipindahkan ke Workgroup $workgroupName!'
                }} catch {{
                    Write-Host ('[ERROR] ' + $_.Exception.Message)
                    exit 1
                }}
            ";

            int res = await CommandRunner.RunPowerShellAsync(ps, line => Logger.Log(line, LogType.Info));

            if (res == 0)
            {
                Logger.Log($"✅ Komputer berhasil dikeluarkan dari Domain dan dipindahkan ke [{workgroupName}].", LogType.Success);
                Logger.Log("⚠️ Silakan RESTART komputer dan login menggunakan akun Administrator Lokal.", LogType.Warning);
            }
            else
            {
                Logger.Log("❌ Gagal mengeluarkan komputer dari domain. Pastikan kredensial admin tepat.", LogType.Error);
            }
        }

        private async Task TestDomainControllerAsync()
        {
            Logger.Log("=== DIAGNOSA KONEKTIVITAS DOMAIN CONTROLLER (DC TEST) ===", LogType.Info);

            string? dcOrDomain = InputDialog.Show(
                Form.ActiveForm,
                "Target Domain / DC",
                "Masukkan Nama Domain atau IP Domain Controller (contoh: corp.internal.net atau 192.168.1.10):",
                "corp.internal.net"
            );

            if (string.IsNullOrWhiteSpace(dcOrDomain))
            {
                Logger.Log("ℹ️ Diagnosa dibatalkan.", LogType.Info);
                return;
            }

            dcOrDomain = dcOrDomain.Trim();
            Logger.Log($"Menguji resolusi DNS & port layanan Active Directory ke [{dcOrDomain}]...", LogType.Info);

            string ps = $@"
                $target = '{dcOrDomain}'
                Write-Host '1. Uji Resolusi Nama & Ping...'
                $ping = Test-Connection -ComputerName $target -Count 2 -Quiet
                if ($ping) {{ Write-Host '  [OK] Ping ke ' $target ' BERHASIL' }} else {{ Write-Host '  [WARNING] Ping ke ' $target ' Gagal / RTO' }}

                Write-Host '2. Uji Port Layanan Kunci Active Directory:'
                $ports = @(
                    @{{ Name = 'DNS'; Port = 53 }},
                    @{{ Name = 'Kerberos Authentication'; Port = 88 }},
                    @{{ Name = 'LDAP Directory'; Port = 389 }},
                    @{{ Name = 'SMB / RPC'; Port = 445 }},
                    @{{ Name = 'LDAP SSL (LDAPS)'; Port = 636 }},
                    @{{ Name = 'Global Catalog'; Port = 3268 }}
                )

                foreach ($p in $ports) {{
                    $sock = New-Object System.Net.Sockets.TcpClient
                    $async = $sock.BeginConnect($target, $p.Port, $null, $null)
                    $wait = $async.AsyncWaitHandle.WaitOne(2000, $false)
                    if ($wait -and $sock.Connected) {{
                        Write-Host ('  [OK] Port ' + $p.Port + ' (' + $p.Name + '): TERBUKA')
                        $sock.Close()
                    }} else {{
                        Write-Host ('  [FAIL] Port ' + $p.Port + ' (' + $p.Name + '): TERTUTUP / BLOCKED')
                        $sock.Close()
                    }}
                }}
            ";

            await CommandRunner.RunPowerShellAsync(ps, line => Logger.Log(line, LogType.Info));
            Logger.Log("✅ Diagnosa Domain Controller Selesai.", LogType.Success);
        }

        private async Task OpenSysdmCplAsync()
        {
            Logger.Log("Membuka System Properties (sysdm.cpl)...", LogType.Info);
            await CommandRunner.RunCmdAsync("start sysdm.cpl", null);
            Logger.Log("✅ Jendela System Properties terbuka pada tab Computer Name.", LogType.Success);
        }
    }
}
