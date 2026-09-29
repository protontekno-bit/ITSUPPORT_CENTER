using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ITSupportCenter.Core
{
    public static class CommandRunner
    {
        private static readonly AsyncLocal<CancellationToken> _asyncLocalToken = new();

        /// <summary>
        /// Token pembatalan aktif yang berlaku secara asinkron untuk operasi command runner saat ini.
        /// </summary>
        public static CancellationToken CurrentToken
        {
            get => _asyncLocalToken.Value;
            set => _asyncLocalToken.Value = value;
        }

        public static async Task<int> RunCmdAsync(string command, Action<string>? onOutput = null, CancellationToken cancellationToken = default)
        {
            var effectiveToken = cancellationToken != default ? cancellationToken : CurrentToken;

            if (effectiveToken.IsCancellationRequested)
            {
                onOutput?.Invoke("[DIBATALKAN] Operasi dibatalkan sebelum dijalankan.");
                return -999;
            }

            return await Task.Run(async () =>
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

                    using var registration = effectiveToken.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                onOutput?.Invoke("[DIBATALKAN] Menghentikan proses cmd.exe di latar belakang...");
                                process.Kill(entireProcessTree: true);
                            }
                        }
                        catch { }
                    });

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    try
                    {
                        await process.WaitForExitAsync(effectiveToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return -999;
                    }

                    if (effectiveToken.IsCancellationRequested)
                        return -999;

                    return process.ExitCode;
                }
                catch (Exception ex)
                {
                    if (effectiveToken.IsCancellationRequested)
                    {
                        onOutput?.Invoke("[DIBATALKAN] Proses dihentikan.");
                        return -999;
                    }
                    onOutput?.Invoke("[EXCEPTION] " + ex.Message);
                    return -1;
                }
            }, effectiveToken);
        }

        public static async Task<int> RunPowerShellAsync(string psScript, Action<string>? onOutput = null, CancellationToken cancellationToken = default)
        {
            var effectiveToken = cancellationToken != default ? cancellationToken : CurrentToken;

            if (effectiveToken.IsCancellationRequested)
            {
                onOutput?.Invoke("[DIBATALKAN] Operasi dibatalkan sebelum dijalankan.");
                return -999;
            }

            return await Task.Run(async () =>
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

                    using var registration = effectiveToken.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                onOutput?.Invoke("[DIBATALKAN] Menghentikan proses PowerShell di latar belakang...");
                                process.Kill(entireProcessTree: true);
                            }
                        }
                        catch { }
                    });

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    try
                    {
                        await process.WaitForExitAsync(effectiveToken);
                    }
                    catch (OperationCanceledException)
                    {
                        return -999;
                    }

                    if (effectiveToken.IsCancellationRequested)
                        return -999;

                    return process.ExitCode;
                }
                catch (Exception ex)
                {
                    if (effectiveToken.IsCancellationRequested)
                    {
                        onOutput?.Invoke("[DIBATALKAN] Proses dihentikan.");
                        return -999;
                    }
                    onOutput?.Invoke("[EXCEPTION] " + ex.Message);
                    return -1;
                }
            }, effectiveToken);
        }
    }
}
