using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using VideoDownloader.TikTokEntities;

namespace VideoDownloader.Controllers
{
     

        [ApiController]
        [Route("api/[controller]")]
        public class TikTokController : ControllerBase
        {
            private readonly HttpClient _httpClient;

            public TikTokController(IHttpClientFactory factory)
            {
                _httpClient = factory.CreateClient();
            }

            [HttpGet("DownloadTikTokVideo")]
            public async Task<IActionResult> DownloadTikTokVideo([FromQuery] string url)
            {
                if (string.IsNullOrWhiteSpace(url))
                    return BadRequest("url is required");

                // URL-i tam təmizlə (səndə bu problem çox olub)
                url = Regex.Replace(url, @"\s+", "");

                if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                    return BadRequest("Invalid url");

                // tikwm API
                var apiUrl = $"https://www.tikwm.com/api/?url={Uri.EscapeDataString(url)}";

                using var resp = await _httpClient.GetAsync(apiUrl);
                var json = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                    return Problem($"tikwm http {(int)resp.StatusCode}: {json}");

                var model = JsonSerializer.Deserialize<TikwmResponse>(json);
                if (model is null)
                    return Problem("Cannot parse tikwm response");

                if (model.Code != 0 || model.Data is null)
                    return Problem($"tikwm error: code={model.Code}, msg={model.Msg}");

                var mp4Url = model.Data.Play ?? model.Data.WmPlay;
                if (string.IsNullOrWhiteSpace(mp4Url))
                    return Problem("tikwm did not return play/wmplay url");

                // Yazılacaq qovluq (IIS publish folder-də işləyəcək)
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "TikTok_Videos");
                Directory.CreateDirectory(folder);

                var safeTitle = MakeSafeFileName(model.Data.Title ?? "tiktok");
                var filePath = Path.Combine(folder, $"{safeTitle}.mp4");

                // MP4 endir
                await using var s = await _httpClient.GetStreamAsync(mp4Url);
                await using var f = System.IO.File.Create(filePath);
                await s.CopyToAsync(f);

                return Ok(new
                {
                    savedTo = filePath,
                    title = model.Data.Title,
                    mp4Url
                });
            }

            private static string MakeSafeFileName(string name)
            {
                foreach (var c in Path.GetInvalidFileNameChars())
                    name = name.Replace(c, '_');

                name = name.Trim();
                if (name.Length == 0) name = "tiktok";
                return name.Length > 80 ? name[..80] : name;
            }
        }

    
}
