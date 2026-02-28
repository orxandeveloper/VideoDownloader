using System.Diagnostics;
using System.Text.RegularExpressions;
using VideoDownloader.Services;
using System.Text.RegularExpressions;

namespace VideoDownloader.YoutubeServices
{
    public class YouTubeDownloadService : IYouTubeDownloadService
    {
        public async Task<string> DownloadToFileAsync(string youtubeUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(youtubeUrl))
                throw new ArgumentException("url is required");

            youtubeUrl = Regex.Replace(youtubeUrl, @"\s+", "");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Youtube_Videos");
            Directory.CreateDirectory(folder);

            var outTemplate = Path.Combine(folder, "%(title).80s [%(id)s].%(ext)s");

            // Telegram üçün daha təhlükəsiz: default 720p (istəsən bunu "best" edərik)
            var args = new[]
            {
            "--no-playlist",
            "-o", outTemplate,
            "--merge-output-format", "mp4",
            "-f", "bv*[height<=720][ext=mp4]+ba[ext=m4a]/b[height<=720]/b",
            youtubeUrl
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
