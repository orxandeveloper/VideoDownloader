using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using VideoDownloader.InstagramServices;
using VideoDownloader.TikTokEntities;
using VideoDownloader.YoutubeServices;

namespace VideoDownloader.Controllers
{
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ITelegramBotClient _bot;
        private readonly ITikTokDownloadService _tiktok;
        private readonly IYouTubeDownloadService _youtube;
        private readonly IInstagramDownloadService _instagram;

        public TelegramWebhookController(
            ITelegramBotClient bot,
            ITikTokDownloadService tiktok,
            IYouTubeDownloadService youtube,
            IInstagramDownloadService instagram)
        {
            _bot = bot;
            _tiktok = tiktok;
            _youtube = youtube;
            _instagram = instagram;
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Update update, CancellationToken ct)
        {
            if (update.Message?.Text is null)
                return Ok();

            var chatId = update.Message.Chat.Id;
            var text = update.Message.Text.Trim();

            if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
            {
                await _bot.SendTextMessageAsync(chatId, "Salam! Link göndər (TikTok/YouTube).", cancellationToken: ct);
                return Ok();
            }

            var url = ExtractFirstUrl(text);
            if (url is null)
            {
                await _bot.SendTextMessageAsync(chatId, "Link görmədim. Zəhmət olmasa link göndər 🙂", cancellationToken: ct);
                return Ok();
            }

            var platform = DetectPlatform(url);

            await _bot.SendTextMessageAsync(chatId, $"{platform} linki qəbul olundu. İndirirəm...", cancellationToken: ct);

            try
            {
                string filePath = platform switch
                {
                    "TikTok" => await _tiktok.DownloadViaLocalApiAsync(url, ct),
                    "YouTube" => await _youtube.DownloadToFileAsync(url, ct),
                    "Instagram"=> await _instagram.DownloadToFileAsync(url,ct),
                    _ => throw new Exception("Bu platform hələ dəstəklənmir.")
                };

                await using var fs = System.IO.File.OpenRead(filePath);

                await _bot.SendVideoAsync(
                    chatId,
                    video: new Telegram.Bot.Types.InputFileStream(fs, Path.GetFileName(filePath)),
                    caption: "Hazır ✅",
                    cancellationToken: ct
                );

                // istəsən sonra sil:
                // System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                await _bot.SendTextMessageAsync(chatId, "Alınmadı ❌\n" + ex.Message, cancellationToken: ct);
            }

            return Ok();
        }

        private static string? ExtractFirstUrl(string text)
        {
            var m = System.Text.RegularExpressions.Regex.Match(text, @"https?://[^\s]+",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!m.Success) return null;

            var url = m.Value.Trim().TrimEnd(')', ']', '}', '.', ',', ';', '"', '\'');
            url = System.Text.RegularExpressions.Regex.Replace(url, @"\s+", "");
            return url;
        }
        private static string DetectPlatform(string url)
        {
            var u = url.ToLowerInvariant();
            if (u.Contains("tiktok.com")) return "TikTok";
            if (u.Contains("youtu.be") || u.Contains("youtube.com")) return "YouTube";
            if (u.Contains("instagram.com")) return "Instagram";
            return "Unknown";
        }
    }
}
