
using System.Diagnostics;
using System.Text.RegularExpressions;
using VideoDownloader.Services;

namespace VideoDownloader.InstagramServices
{
    public class InstagramDownloadService : IInstagramDownloadService
    {
        public async Task<string> DownloadToFileAsync(string instagramUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(instagramUrl))
                throw new ArgumentException("url is required");

            instagramUrl = Regex.Replace(instagramUrl, @"\s+", "");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Instagram_Videos");
            Directory.CreateDirectory(folder);

            var outTemplate = Path.Combine(folder, "%(title).80s [%(id)s].%(ext)s");

            // Instagram üçün də Telegram-a uyğun: 720p limit (istəsən çıxardarıq)
            var args = new[]
            {
            "--no-playlist",
            "-o", outTemplate,
            "--merge-output-format", "mp4",
            "-f", "bv*[height<=720][ext=mp4]+ba[ext=m4a]/b[height<=720]/b",
            instagramUrl
        };

            await YtDlpRunner.RunAsync(args, ct);

            var file = new DirectoryInfo(folder)
                .GetFiles("*.mp4")
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault();

            if (file is null)
                throw new Exception("Download finished but mp4 file not found");

            return file.FullName;
        }
    }
}
