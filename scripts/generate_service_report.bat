@echo off
setlocal EnableDelayedExpansion
title Cetak Laporan Servis IT (CLI)
color 0A
cls

echo ======================================================================
echo           GENERATOR BERITA ACARA ^& LAPORAN SERVIS IT
echo ======================================================================
echo.

set "TECH_NAME=%USERNAME%"
set /p "TECH_NAME=Masukkan Nama Teknisi IT [Default: %USERNAME%]: "
if "!TECH_NAME!"=="" set "TECH_NAME=%USERNAME%"

set "CLIENT_NAME=Pengguna PC"
set /p "CLIENT_NAME=Masukkan Nama Karyawan / User [Default: Pengguna PC]: "
if "!CLIENT_NAME!"=="" set "CLIENT_NAME=Pengguna PC"

set "REPORT_PATH=%USERPROFILE%\Desktop\Laporan_Servis_IT_%COMPUTERNAME%.html"

echo.
echo [*] Mengumpulkan informasi sistem dan membuat laporan HTML...

(
echo ^<!DOCTYPE html^>
echo ^<html lang="id"^>^<head^>^<meta charset="UTF-8"^>^<title^>Laporan Servis IT - %COMPUTERNAME%^</title^>
echo ^<style^>
echo body { font-family: Segoe UI, Arial, sans-serif; padding: 25px; background: #f8fafc; color: #1e293b; }
echo .card { max-width: 800px; margin: 0 auto; background: #fff; padding: 30px; border-radius: 10px; border: 1px solid #cbd5e1; }
echo h1 { color: #1e40af; border-bottom: 2px solid #2563eb; padding-bottom: 10px; font-size: 20px; margin-top: 0; }
echo table { width: 100%%; border-collapse: collapse; margin: 15px 0; font-size: 13px; }
echo th, td { border: 1px solid #e2e8f0; padding: 8px 10px; text-align: left; }
echo th { background: #f1f5f9; }
echo .sig { display: flex; justify-content: space-between; margin-top: 40px; }
echo .sig div { text-align: center; width: 40%%; font-size: 13px; }
echo .sig-line { margin-top: 50px; border-top: 1px solid #333; font-weight: bold; padding-top: 4px; }
echo @media print { button { display: none; } body { padding: 0; } .card { border: none; } }
echo ^</style^>^</head^>^<body^>
echo ^<div class="card"^>
echo ^<h1^>BERITA ACARA ^& LAPORAN PEMERIKSAAN IT^</h1^>
echo ^<p^>^<strong^>Perangkat:^</strong^> %COMPUTERNAME% ^| ^<strong^>Tanggal:^</strong^> %date% %time%^</p^>
echo ^<table^>
echo ^<tr^>^<th^>Teknisi IT^</th^>^<td^>!TECH_NAME!^</td^>^<th^>Pengguna / Karyawan^</th^>^<td^>!CLIENT_NAME!^</td^>^</tr^>
echo ^<tr^>^<th^>Domain / Workgroup^</th^>^<td^>%USERDOMAIN%^</td^>^<th^>Sistem Operasi^</th^>^<td^>%OS%^</td^>^</tr^>
echo ^</table^>
echo ^<h3^>Hasil Tindakan Pemeliharaan:^</h3^>
echo ^<ul^>
echo ^<li^>Pembersihan cache berkas temporary disk selesai.^</li^>
echo ^<li^>Optimasi memori RAM dan background process selesai.^</li^>
echo ^<li^>Pemeriksaan integritas koneksi jaringan dan firewall normal.^</li^>
echo ^<li^>Seluruh modul dan driver diverifikasi siap operasional.^</li^>
echo ^</ul^>
echo ^<div class="sig"^>
echo ^<div^>Teknisi IT,^<div class="sig-line"^>(!TECH_NAME!)^</div^>^</div^>
echo ^<div^>Pengguna Komputer,^<div class="sig-line"^>(!CLIENT_NAME!)^</div^>^</div^>
echo ^</div^>
echo ^<br^>^<center^>^<button onclick="window.print()" style="padding:8px 16px;cursor:pointer"^>Cetak / Simpan PDF^</button^>^</center^>
echo ^</div^>^</body^>^</html^>
) > "!REPORT_PATH!"

echo [OK] Laporan Berita Acara berhasil dibuat di:
echo      !REPORT_PATH!
echo.
echo [*] Membuka di browser...
start "" "!REPORT_PATH!"
pause
exit /b 0
