using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ITSupportCenter.Core
{
    public enum LogType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public static class Logger
    {
        private static RichTextBox? _logBox;

        public static void Initialize(RichTextBox logBox)
        {
            _logBox = logBox;
        }

        public static void Log(string message, LogType type = LogType.Info)
        {
            if (_logBox == null) return;

            if (_logBox.InvokeRequired)
            {
                _logBox.BeginInvoke(new Action(() => Log(message, type)));
                return;
            }

            try
            {
                if (_logBox.IsDisposed) return;

                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                Color color = type switch
                {
                    LogType.Success => Color.FromArgb(46, 204, 113),   // Emerald Green
                    LogType.Warning => Color.FromArgb(241, 196, 15),   // Sunflower Yellow
                    LogType.Error => Color.FromArgb(231, 76, 60),      // Alizarin Red
                    _ => Color.FromArgb(189, 195, 199)                  // Light Silver
                };

                string prefix = type switch
                {
                    LogType.Success => "[SUKSES] ",
                    LogType.Warning => "[WARN]   ",
                    LogType.Error => "[ERROR]  ",
                    _ => "[INFO]   "
                };

                _logBox.SelectionStart = _logBox.TextLength;
                _logBox.SelectionLength = 0;
                _logBox.SelectionColor = Color.FromArgb(127, 140, 141);
                _logBox.AppendText($"[{timestamp}] ");

                _logBox.SelectionColor = color;
                _logBox.AppendText(prefix);

                _logBox.SelectionColor = Color.FromArgb(236, 240, 241);
                _logBox.AppendText(message + Environment.NewLine);

                _logBox.ScrollToCaret();
            }
            catch { }
        }

        public static void Clear()
        {
            if (_logBox != null)
            {
                if (_logBox.InvokeRequired)
                {
                    _logBox.BeginInvoke(new Action(Clear));
                    return;
                }
                _logBox.Clear();
            }
        }

        public static string ExportLogToFile()
        {
            if (_logBox == null || string.IsNullOrWhiteSpace(_logBox.Text))
                return "";

            string logsDir;
            try
            {
                logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Laporan_Log");
                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);
            }
            catch
            {
                logsDir = Path.Combine(Path.GetTempPath(), "ITSupportCenter_Logs");
                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);
            }

            string fileName = $"Laporan_Perbaikan_{Environment.MachineName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            string filePath = Path.Combine(logsDir, fileName);

            try
            {
                File.WriteAllText(filePath, _logBox.Text, Encoding.UTF8);
            }
            catch
            {
                filePath = Path.Combine(Path.GetTempPath(), fileName);
                File.WriteAllText(filePath, _logBox.Text, Encoding.UTF8);
            }

            return filePath;
        }
    }
}
