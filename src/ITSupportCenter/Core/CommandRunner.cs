using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ITSupportCenter.Core
{
    public static class CommandRunner
    {
        public static async Task<int> RunCmdAsync(string command, Action<string>? onOutput = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c {command}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    };

                    using var process = new Process { StartInfo = psi };
                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            onOutput?.Invoke(e.Data);
                    };
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            onOutput?.Invoke("[STDERR] " + e.Data);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    return process.ExitCode;
                }
                catch (Exception ex)
                {
                    onOutput?.Invoke("[EXCEPTION] " + ex.Message);
                    return -1;
                }
            });
        }

        public static async Task<int> RunPowerShellAsync(string psScript, Action<string>? onOutput = null)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psScript}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8,
                        StandardErrorEncoding = Encoding.UTF8
                    };

                    using var process = new Process { StartInfo = psi };
                    process.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            onOutput?.Invoke(e.Data);
                    };
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            onOutput?.Invoke("[STDERR] " + e.Data);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    return process.ExitCode;
                }
                catch (Exception ex)
                {
                    onOutput?.Invoke("[EXCEPTION] " + ex.Message);
                    return -1;
                }
            });
        }
    }
}
