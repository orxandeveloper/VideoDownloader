using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using VideoDownloader.TikTokEntities;

namespace VideoDownloader.Controllers
{
    public class DownloadController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient();





        //[HttpPost]
        //[Route("TikTok_Videos")]
        //public async Task<IActionResult> DownloadTikTokVideo(string url)
        //{
        //    try
        //    {
        //        // TikTok video indirme API'si
        //        // Not: Bu API'yi kendi API anahtarınızla değiştirmeniz gerekebilir
        //        var apiUrl = $"https://www.tikwm.com/api/?url={url}";

        //        var response = await _httpClient.GetAsync(apiUrl);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(json);
        //            var result = JsonSerializer.Deserialize<TikTokResponse>(json);

        //            if (result?.Data?.Play != null)
        //            {
        //                // Videoyu indir
        //                var videoUrl = result.Data.Play;
        //                var videoData = await _httpClient.GetByteArrayAsync(videoUrl);

        //                // Geçici dosya adı oluştur
        //                //var tempPath = Path.Combine(Path.GetTempPath(), $"tiktok_{Guid.NewGuid()}.mp4");
        //                var baseDirectory = Directory.GetParent(AppContext.BaseDirectory)!
        //                               .Parent!
        //                               .Parent!
        //                               .Parent!
        //                               .FullName;//AppContext.BaseDirectory;
        //                var tikTokVideosDir = Path.Combine(baseDirectory, "TikTok_Videos");

        //                // Klasör yoksa oluştur
        //                if (!Directory.Exists(tikTokVideosDir))
        //                {
        //                    Directory.CreateDirectory(tikTokVideosDir);
        //                }

        //                // Benzersiz dosya adı oluştur
        //                var fileName = $"tiktok_{Guid.NewGuid()}.mp4";
        //                var videoPath = Path.Combine(tikTokVideosDir, fileName);


        //                // Videoyu kaydet
        //                await System.IO.File.WriteAllBytesAsync(videoPath, videoData);

        //                return Ok(videoPath);
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Video indirme hatası: {ex.Message}");
        //        return null;
        //    }
        //}


        [HttpPost("Youtube_Videos")]
        public async Task<IActionResult> DownloadYoutubeVideo([FromBody] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL is required");

            // Project root yolunu tap
            var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!
                                       .Parent!
                                       .Parent!
                                       .Parent!
                                       .FullName;

            var youtubeFolder = Path.Combine(projectRoot, "Youtube_Videos");

            if (!Directory.Exists(youtubeFolder))
                Directory.CreateDirectory(youtubeFolder);

            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            psi.ArgumentList.Add("-f");
            psi.ArgumentList.Add("best");
            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add(Path.Combine(youtubeFolder, "%(title)s.%(ext)s"));
            psi.ArgumentList.Add(url);

            using var process = Process.Start(psi)!;

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                return Problem(error);

            return Ok("Downloaded to Youtube_Videos folder");
        }


        [HttpPost("Instagram_Videos")]
        public async Task<IActionResult> DownloadInstagramVideo([FromBody] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("URL is required");

            // Project root yolunu tap
            var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!
                                       .Parent!
                                       .Parent!
                                       .Parent!
                                       .FullName;

            var youtubeFolder = Path.Combine(projectRoot, "Instagram_Videos");

            if (!Directory.Exists(youtubeFolder))
                Directory.CreateDirectory(youtubeFolder);

            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            psi.ArgumentList.Add("-f");
            psi.ArgumentList.Add("best");
            psi.ArgumentList.Add("-o");
            psi.ArgumentList.Add(Path.Combine(youtubeFolder, "%(title)s.%(ext)s"));
            psi.ArgumentList.Add(url);

            using var process = Process.Start(psi)!;

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                return Problem(error);

            return Ok("Downloaded to Instagram_Videos folder");
        }
    }
}
