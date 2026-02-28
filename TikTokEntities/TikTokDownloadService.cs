
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace VideoDownloader.TikTokEntities
{
    public class TikTokDownloadService : ITikTokDownloadService
    {
        private readonly HttpClient _http;

        public TikTokDownloadService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.Timeout = TimeSpan.FromMinutes(10);
        }

        public async  Task<string> DownloadViaLocalApiAsync(string tiktokUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(tiktokUrl))
                throw new ArgumentException("url is required");

            tiktokUrl = Regex.Replace(tiktokUrl, @"\s+", "");

            if (!Uri.TryCreate(tiktokUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid url");

            // 1) tikwm API (burada localhost YOX!)
            var tikwmUrl = $"https://www.tikwm.com/api/?url={Uri.EscapeDataString(tiktokUrl)}";

            using var apiResp = await _http.GetAsync(tikwmUrl, HttpCompletionOption.ResponseHeadersRead, ct);
            var apiJson = await apiResp.Content.ReadAsStringAsync(ct);

            if (!apiResp.IsSuccessStatusCode)
                throw new Exception($"tikwm http {(int)apiResp.StatusCode}: {apiJson}");

            var model = JsonSerializer.Deserialize<TikwmResponse>(apiJson)
                        ?? throw new Exception("Cannot parse tikwm response");

            if (model.Code != 0 || model.Data is null)
                throw new Exception($"tikwm error: code={model.Code}, msg={model.Msg}");

            var mp4Url = model.Data.Play ?? model.Data.WmPlay;
            if (string.IsNullOrWhiteSpace(mp4Url))
                throw new Exception("tikwm did not return play/wmplay url");

            // 2) save
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "TikTok_Videos");
            Directory.CreateDirectory(folder);

            var safeTitle = MakeSafeFileName(model.Data.Title ?? "tiktok");
            var filePath = Path.Combine(folder, $"{safeTitle}.mp4");

            // 3) mp4 download
            using var videoResp = await _http.GetAsync(mp4Url, HttpCompletionOption.ResponseHeadersRead, ct);
            videoResp.EnsureSuccessStatusCode();

            await using var input = await videoResp.Content.ReadAsStreamAsync(ct);
            await using var output = System.IO.File.Create(filePath);
            await input.CopyToAsync(output, ct);

            return filePath;
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
