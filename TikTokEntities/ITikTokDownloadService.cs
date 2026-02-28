namespace VideoDownloader.TikTokEntities
{
    public interface ITikTokDownloadService
    {
        public Task<string> DownloadViaLocalApiAsync(string tiktokUrl, CancellationToken ct);
    }
}
