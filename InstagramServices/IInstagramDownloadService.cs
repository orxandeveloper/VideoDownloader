namespace VideoDownloader.InstagramServices
{
    public interface IInstagramDownloadService
    {
        Task<string> DownloadToFileAsync(string instagramUrl, CancellationToken ct);
    }
}
