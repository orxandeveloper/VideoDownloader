using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VideoDownloader.InstagramServices;
using VideoDownloader.TikTokEntities;
using VideoDownloader.YoutubeServices;


namespace VideoDownloader.TelegramBackgroundService
{
    public class TelegramBotWorker : BackgroundService
    {
        private readonly ITelegramBotClient _bot;
        private readonly ILogger<TelegramBotWorker> _logger;

        private int _offset = 0;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ITikTokDownloadService _tikTokService;
        private readonly IInstagramDownloadService _instagram;
        private readonly IYouTubeDownloadService _youtube;

        public TelegramBotWorker(
            ITelegramBotClient bot,
            ILogger<TelegramBotWorker> logger,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IInstagramDownloadService instagram,
            IYouTubeDownloadService youtube,
            ITikTokDownloadService tikTokService)
        {
            _bot = bot;
            _logger = logger;
            _config = config;
            _instagram = instagram;
            _youtube = youtube; 
            _httpClientFactory = httpClientFactory;
            _tikTokService = tikTokService;
        }
        public TelegramBotWorker(ITelegramBotClient bot, ILogger<TelegramBotWorker> logger)
        {
            _bot = bot;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var me = await _bot.GetMeAsync(stoppingToken);
            _logger.LogInformation("Telegram bot started: @{Username} ({Id})", me.Username, me.Id);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Long polling: Telegram-dan update-ləri götür
                    Update[] updates = await _bot.GetUpdatesAsync(
                        offset: _offset,
                        limit: 50,
                        timeout: 30, // 30 sec long poll
                        allowedUpdates: new[] { UpdateType.Message },
                        cancellationToken: stoppingToken
                    );

                    foreach (var u in updates)
                    {
                        _offset = Math.Max(_offset, u.Id + 1);

                        var msg = u.Message;
                        if (msg?.Text is null) continue;

                        await HandleMessageAsync(msg, stoppingToken);
                    }
                }
                catch (ApiRequestException ex)
                {
                    _logger.LogError(ex, "Telegram API error: {Message}", ex.Message);
                    await Task.Delay(2000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // normal shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error");
                    await Task.Delay(2000, stoppingToken);
                }
            }

            _logger.LogInformation("Telegram bot stopped.");
        }

        private async Task HandleMessageAsync(Message msg, CancellationToken ct)
        {
            var chatId = msg.Chat.Id;
            var text = (msg.Text ?? "").Trim();

            _logger.LogInformation("Message from {ChatId}: {Text}", chatId, text);

            if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
            {
                await _bot.SendTextMessageAsync(
                    chatId,
                    "Salam! Link göndər (YouTube/TikTok/Instagram). Məs: https://youtu.be/....",
                    cancellationToken: ct
                );
                return;
            }

            if (text.Equals("/help", StringComparison.OrdinalIgnoreCase))
            {
                await _bot.SendTextMessageAsync(
                    chatId,
                    "Komandalar:\n/start\n/help\n\nSadəcə link göndər.",
                    cancellationToken: ct
                );
                return;
            }

            // 1) Mesajdan URL tap
            var url = ExtractFirstUrl(text);
            if (url is null)
            {
                await _bot.SendTextMessageAsync(
                    chatId,
                    "Mən link görmədim 🙂 Zəhmət olmasa bir YouTube/TikTok/Instagram linki göndər.",
                    cancellationToken: ct
                );
                return;
            }

            // 2) Platform detection
            var platform = DetectPlatform(url);
            if (platform == "TikTok")
            {
                await _bot.SendTextMessageAsync(chatId, "TikTok linki qəbul olundu. İndirirəm...", cancellationToken: ct);

                try
                {
                    var filePath = await _tikTokService.DownloadViaLocalApiAsync(url, ct);// DownloadViaLocalApiAsync(url, ct);

                    await using var fs = System.IO.File.OpenRead(filePath);

                    await _bot.SendVideoAsync(
                        chatId: chatId,
                        video: new Telegram.Bot.Types.InputFileStream(fs, Path.GetFileName(filePath)),
                        caption: "Hazır ✅",
                        cancellationToken: ct
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "TikTok flow failed");
                    await _bot.SendTextMessageAsync(chatId, $"Alınmadı ❌\n{ex.Message}", cancellationToken: ct);
                }

                return;
            }
            else if (platform == "Instagram")
            {
                await _bot.SendTextMessageAsync(chatId, "Instagram linki qəbul olundu. İndirirəm...", cancellationToken: ct);

                try
                {
                    var filePath = await _instagram.DownloadToFileAsync(url, ct);

                    await using var fs = System.IO.File.OpenRead(filePath);

                    await _bot.SendVideoAsync(
                        chatId,
                        video: new Telegram.Bot.Types.InputFileStream(fs, Path.GetFileName(filePath)),
                        caption: "Hazır ✅",
                        cancellationToken: ct
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Instagram download failed");
                    await _bot.SendTextMessageAsync(chatId,
                        "Alınmadı ❌\nInstagram bəzən login/cookie tələb edir.\nXəta: " + ex.Message,
                        cancellationToken: ct);
                }

                return;
            }
            else if (platform == "YouTube")
            {
                await _bot.SendTextMessageAsync(chatId, "YouTube linki qəbul olundu. İndirirəm...", cancellationToken: ct);

                try
                {
                    var filePath = await _youtube.DownloadToFileAsync(url, ct);

                    await using var fs = System.IO.File.OpenRead(filePath);

                    await _bot.SendVideoAsync(
                        chatId,
                        video: new Telegram.Bot.Types.InputFileStream(fs, Path.GetFileName(filePath)),
                        caption: "Hazır ✅",
                        cancellationToken: ct
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "YouTube download failed");
                    await _bot.SendTextMessageAsync(chatId, $"Alınmadı ❌\n{ex.Message}", cancellationToken: ct);
                }

                return;
            }
        }
        
        private static string? ExtractFirstUrl(string text)
        {
            // Sadə URL regex (Telegram mesajları üçün kifayətdir)
            var m = Regex.Match(text, @"https?://[^\s]+", RegexOptions.IgnoreCase);
            if (!m.Success) return null;

            // URL sonda ) , . kimi simvollarla gələ bilər → təmizlə
            var url = m.Value.Trim().TrimEnd(')', ']', '}', '.', ',', ';', '"', '\'');

            // whitespace-ləri də sil (səndə \n problemi olmuşdu)
            url = Regex.Replace(url, @"\s+", "");

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
