@echo off
title Asisten Active Directory ^& Join Domain Kantor
color 09
echo ============================================================================================
echo                    ASISTEN ACTIVE DIRECTORY ^& JOIN DOMAIN KANTOR
echo ============================================================================================
echo.
echo  PILIH OPERASI ACTIVE DIRECTORY / DOMAIN:
echo  [1] Audit Status Domain ^& Nama Komputer Saat Ini
echo  [2] Ganti Nama Komputer (Rename Computer)
echo  [3] Gabung ke Domain Active Directory (Join Domain)
echo  [4] Keluar dari Domain (Unjoin / Pindah ke Workgroup)
echo  [5] Diagnosa Koneksi ^& Port ke Domain Controller (DC Test)
echo  [6] Buka Pengaturan Nama Komputer Klasik (sysdm.cpl)
echo  [0] Batal / Kembali
echo.
set /p "adchoice= Masukkan pilihan [0-6]: "

if "%adchoice%"=="1" (
    echo.
    echo [*] Mengaudit status Domain dan Nama Komputer...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "$cs = Get-CimInstance Win32_ComputerSystem; Write-Host '=================================================='; Write-Host ('Nama Komputer      : ' + $env:COMPUTERNAME); Write-Host ('Part of Domain     : ' + $cs.PartOfDomain); Write-Host ('Domain / Workgroup : ' + $cs.Domain); Write-Host ('Logon Server (DC)  : ' + $env:LOGONSERVER); Write-Host ('User Aktif         : ' + $env:USERDOMAIN + '\' + $env:USERNAME); Write-Host '=================================================='"
) else if "%adchoice%"=="2" (
    echo.
    echo Nama Komputer Saat Ini: [%COMPUTERNAME%]
    set /p "newname= Masukkan Nama Komputer Baru (maksimal 15 karakter): "
    if not "%newname%"=="" (
        echo [*] Mengganti nama komputer menjadi [%newname%]...
        powershell -NoProfile -ExecutionPolicy Bypass -Command "Rename-Computer -NewName '%newname%' -Force"
        echo [OK] Nama komputer berhasil diubah! Silakan RESTART komputer agar berlaku.
    )
) else if "%adchoice%"=="3" (
    echo.
    set /p "domname= Masukkan Nama Domain Lengkap (contoh corp.internal.net): "
    if not "%domname%"=="" (
        set /p "domadmin= Masukkan Username Domain Admin (contoh Administrator atau domain\user): "
        echo [*] Menghubungkan komputer ke Domain [%domname%]...
        powershell -NoProfile -ExecutionPolicy Bypass -Command "$cred = Get-Credential -UserName '%domadmin%' -Message 'Masukkan Password Domain Admin'; Add-Computer -DomainName '%domname%' -Credential $cred -Restart:$false -Force"
        echo [OK] Perintah Join Domain selesai dijalankan. Silakan RESTART komputer.
    )
) else if "%adchoice%"=="4" (
    echo.
    set /p "wgname= Masukkan Nama Workgroup Baru [Default: WORKGROUP]: "
    if "%wgname%"=="" set "wgname=WORKGROUP"
    echo [*] Memindahkan komputer ke Workgroup [%wgname%]...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Remove-Computer -WorkgroupName '%wgname%' -Restart:$false -Force"
    echo [OK] Komputer telah dipindahkan ke Workgroup. Silakan RESTART dan login dengan akun lokal.
) else if "%adchoice%"=="5" (
    echo.
    set /p "targetdc= Masukkan Domain / IP Domain Controller (contoh corp.internal.net atau 192.168.1.10): "
    if not "%targetdc%"=="" (
        echo [*] Menguji konektivitas ke [%targetdc%]...
        ping -n 2 %targetdc%
        echo.
        echo [*] Menguji port Active Directory (DNS 53, Kerberos 88, LDAP 389, SMB 445)...
        powershell -NoProfile -ExecutionPolicy Bypass -Command "$ports = @(53, 88, 389, 445, 3268); foreach($p in $ports){ $s = New-Object Net.Sockets.TcpClient; $a = $s.BeginConnect('%targetdc%', $p, $null, $null); if($a.AsyncWaitHandle.WaitOne(1500, $false) -and $s.Connected){ Write-Host ('  [OK] Port ' + $p + ' : TERBUKA') } else { Write-Host ('  [FAIL] Port ' + $p + ' : TERTUTUP') }; $s.Close() }"
    )
) else if "%adchoice%"=="6" (
    start sysdm.cpl
)
echo.
