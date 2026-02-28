namespace VideoDownloader.YoutubeServices
{
    public interface IYouTubeDownloadService
    {
        Task<string> DownloadToFileAsync(string youtubeUrl, CancellationToken ct);
    }
}
