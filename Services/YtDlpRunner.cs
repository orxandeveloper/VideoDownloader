using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace VideoDownloader.Services
{

    using System.Diagnostics;

    public static class YtDlpRunner
    {
        public static async Task RunAsync(IEnumerable<string> args, CancellationToken ct)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            foreach (var a in args)
                psi.ArgumentList.Add(a);

            using var p = Process.Start(psi) ?? throw new Exception("Failed to start yt-dlp");

            var stdoutTask = p.StandardOutput.ReadToEndAsync(ct);
            var stderrTask = p.StandardError.ReadToEndAsync(ct);

            await p.WaitForExitAsync(ct);

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            if (p.ExitCode != 0)
                throw new Exception($"yt-dlp failed (exit {p.ExitCode}).\n{stderr}\n{stdout}");
        }
    }
}
