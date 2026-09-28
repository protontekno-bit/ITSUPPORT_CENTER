using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ITSupportCenter.Core;

namespace ITSupportCenter.Services
{
    public static class ReportExporter
    {
        public static string GenerateAndOpenHtmlReport()
        {
            var osInfo = OsDetector.GetOsInfo();
            var history = SessionHistoryTracker.GetSessionItems();
            string timestamp = DateTime.Now.ToString("dd MMMM yyyy, HH:mm:ss");
            string reportFileName = $"IT_Service_Report_{DateTime.Now:yyyyMMdd_HHmmss}.html";
            string reportPath = Path.Combine(Path.GetTempPath(), reportFileName);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='id'>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset='UTF-8'>");
            sb.AppendLine("  <title>Laporan Servis IT & Pemeliharaan Sistem</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    * { box-sizing: border-box; margin: 0; padding: 0; font-family: 'Segoe UI', Arial, sans-serif; }");
            sb.AppendLine("    body { background-color: #f4f6f9; color: #333; padding: 30px; line-height: 1.6; }");
            sb.AppendLine("    .container { max-width: 900px; margin: 0 auto; background: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 15px rgba(0,0,0,0.08); }");
            sb.AppendLine("    .header { border-bottom: 2px solid #2980b9; padding-bottom: 15px; margin-bottom: 25px; display: flex; justify-content: space-between; align-items: center; }");
            sb.AppendLine("    .header h1 { font-size: 22px; color: #2c3e50; }");
            sb.AppendLine("    .header .tag { background: #2980b9; color: #fff; padding: 4px 12px; border-radius: 4px; font-size: 13px; }");
            sb.AppendLine("    .meta-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; background: #f8fafc; padding: 15px; border-radius: 6px; border: 1px solid #e2e8f0; margin-bottom: 25px; }");
            sb.AppendLine("    .meta-item { font-size: 14px; }");
            sb.AppendLine("    .meta-item strong { color: #475569; display: inline-block; width: 140px; }");
            sb.AppendLine("    .section-title { font-size: 17px; font-weight: bold; color: #1e293b; margin-bottom: 12px; border-left: 4px solid #3498db; padding-left: 10px; }");
            sb.AppendLine("    table { width: 100%; border-collapse: collapse; margin-bottom: 25px; font-size: 13.5px; }");
            sb.AppendLine("    th, td { padding: 10px 12px; text-align: left; border-bottom: 1px solid #e2e8f0; }");
            sb.AppendLine("    th { background: #f1f5f9; color: #334155; }");
            sb.AppendLine("    .status-badge { padding: 3px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
            sb.AppendLine("    .status-Success { background: #dcfce7; color: #15803d; }");
            sb.AppendLine("    .status-Warning { background: #fef3c7; color: #b45309; }");
            sb.AppendLine("    .status-Error { background: #fee2e2; color: #b91c1c; }");
            sb.AppendLine("    .footer { margin-top: 30px; padding-top: 15px; border-top: 1px solid #e2e8f0; font-size: 12px; color: #94a3b8; display: flex; justify-content: space-between; }");
            sb.AppendLine("    .btn-print { background: #27ae60; color: #fff; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; font-size: 14px; margin-bottom: 15px; }");
            sb.AppendLine("    @media print { .btn-print { display: none; } body { padding: 0; background: #fff; } .container { box-shadow: none; padding: 0; } }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("  <div class='container'>");
            sb.AppendLine("    <button class='btn-print' onclick='window.print()'>🖨️ Cetak / Simpan PDF</button>");
            sb.AppendLine("    <div class='header'>");
            sb.AppendLine("      <div>");
            sb.AppendLine("        <h1>LAPORAN SERVIS & PEMELIHARAAN IT</h1>");
            sb.AppendLine("        <p style='font-size: 13px; color: #64748b;'>IT Support & Security Center 2026 - Workstation Audit Report</p>");
            sb.AppendLine("      </div>");
            sb.AppendLine($"      <div class='tag'>{timestamp}</div>");
            sb.AppendLine("    </div>");

            sb.AppendLine("    <div class='section-title'>Identifikasi Perangkat Target</div>");
            sb.AppendLine("    <div class='meta-grid'>");
            sb.AppendLine($"      <div class='meta-item'><strong>Nama Komputer:</strong> {Environment.MachineName}</div>");
            sb.AppendLine($"      <div class='meta-item'><strong>User Login:</strong> {Environment.UserName}</div>");
            sb.AppendLine($"      <div class='meta-item'><strong>Sistem Operasi:</strong> {osInfo.OsName} ({osInfo.Architecture})</div>");
            sb.AppendLine($"      <div class='meta-item'><strong>Versi / Build:</strong> Build {osInfo.BuildNumber}</div>");
            sb.AppendLine($"      <div class='meta-item'><strong>Kapasitas RAM:</strong> {osInfo.TotalRamGb:F1} GB</div>");
            sb.AppendLine($"      <div class='meta-item'><strong>Penyimpanan C:\\:</strong> {osInfo.DriveCFreeGb:F1} GB Bebas / {osInfo.DriveCTotalGb:F1} GB ({osInfo.DriveCType})</div>");
            sb.AppendLine("    </div>");

            sb.AppendLine("    <div class='section-title'>Riwayat Tindakan & Hasil Perbaikan Sesi Ini</div>");
            if (history.Count == 0)
            {
                sb.AppendLine("    <p style='padding: 15px; background: #f8fafc; border-radius: 6px; color: #64748b;'>Belum ada modul tindakan yang dieksekusi pada sesi ini.</p>");
            }
            else
            {
                sb.AppendLine("    <table>");
                sb.AppendLine("      <thead>");
                sb.AppendLine("        <tr>");
                sb.AppendLine("          <th style='width: 70px;'>Waktu</th>");
                sb.AppendLine("          <th>Nama Modul / Perbaikan</th>");
                sb.AppendLine("          <th style='width: 140px;'>Kategori</th>");
                sb.AppendLine("          <th style='width: 80px;'>Durasi</th>");
                sb.AppendLine("          <th style='width: 90px;'>Status</th>");
                sb.AppendLine("        </tr>");
                sb.AppendLine("      </thead>");
                sb.AppendLine("      <tbody>");

                foreach (var h in history)
                {
                    sb.AppendLine("        <tr>");
                    sb.AppendLine($"          <td>{h.ExecutedAt:HH:mm:ss}</td>");
                    sb.AppendLine($"          <td><strong>{h.ToolTitle}</strong></td>");
                    sb.AppendLine($"          <td>{h.Category}</td>");
                    sb.AppendLine($"          <td>{h.DurationSeconds:F1}s</td>");
                    sb.AppendLine($"          <td><span class='status-badge status-{h.Status}'>{h.Status}</span></td>");
                    sb.AppendLine("        </tr>");
                }

                sb.AppendLine("      </tbody>");
                sb.AppendLine("    </table>");
            }

            sb.AppendLine("    <div class='footer'>");
            sb.AppendLine($"      <span>Dokumen ini dihasilkan secara otomatis oleh IT Support Center v6.0</span>");
            sb.AppendLine("      <span>Tanda Tangan Teknisi: ____________________</span>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            File.WriteAllText(reportPath, sb.ToString(), Encoding.UTF8);

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = reportPath,
                    UseShellExecute = true
                });
            }
            catch { }

            return reportPath;
        }
    }
}
