using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Security;
using System.Text.Json;
using System.Text.RegularExpressions;
using VideoDownloader.TikTokEntities;

namespace VideoDownloader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

       

        
   
        
        //public record DownloadRequest(string Url);
        //[HttpPost] 
        // public async Task<IActionResult> Download([FromBody] DownloadRequest req)
        //{
        //    var urlRaw = req?.Url ?? "";
        //    var url = Regex.Replace(urlRaw, @"\s+", ""); // bütün whitespace sil

        //    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        //        return BadRequest(new { error = "Invalid URL", urlRaw, url });

        //    var baseDir = AppContext.BaseDirectory;
        //    var outDir = Path.Combine(baseDir, "downloads");
        //    Directory.CreateDirectory(outDir);

        //    var ytdlp = Path.Combine(baseDir, "yt-dlp.exe");
        //    if (!System.IO.File.Exists(ytdlp))
        //        return Problem($"yt-dlp.exe not found in: {baseDir}");

        //    var psi = new ProcessStartInfo
        //    {
        //        FileName = ytdlp,
        //        WorkingDirectory = baseDir,
        //        RedirectStandardOutput = true,
        //        RedirectStandardError = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true
        //    };

        //    // arguments (TikTok üçün)
        //    psi.ArgumentList.Add("-v");
        //    psi.ArgumentList.Add("--no-playlist");
        //    psi.ArgumentList.Add("-f");
        //    psi.ArgumentList.Add("best[ext=mp4]/best");
        //    psi.ArgumentList.Add("-P");
        //    psi.ArgumentList.Add(outDir);
        //    psi.ArgumentList.Add("-o");
        //    psi.ArgumentList.Add("%(title).200s [%(id)s].%(ext)s");
        //    psi.ArgumentList.Add(url);

        //    using var p = Process.Start(psi)!;
        //    var stdout = await p.StandardOutput.ReadToEndAsync();
        //    var stderr = await p.StandardError.ReadToEndAsync();
        //    await p.WaitForExitAsync();

        //    if (p.ExitCode != 0)
        //        return Problem($"ExitCode={p.ExitCode}\nSTDERR:\n{stderr}\n\nSTDOUT:\n{stdout}");

        //    var lastFile = Directory.GetFiles(outDir)
        //        .Select(f => new FileInfo(f))
        //        .OrderByDescending(f => f.CreationTimeUtc)
        //        .FirstOrDefault();

        //    return Ok(new
        //    {
        //        message = "Downloaded",
        //        cleanedUrl = url,
        //        outputDir = outDir,
        //        lastFile = lastFile?.FullName,
        //        stdout
        //    });
        //}

       
    }
}
