using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace VideoDownloader.Controllers
{
    public class TelegramPollingController : Controller
    {
        private static int _offset = 0; // sadə demo üçün (memory-də)
        private readonly ITelegramBotClient _bot;

        public TelegramPollingController(ITelegramBotClient bot)
        {
            _bot = bot;
        }
        [HttpPost("poll")]
        public async Task<IActionResult> Poll()
        {
            var updates = await _bot.GetUpdatesAsync(
                offset: _offset,
                timeout: 0,
                allowedUpdates: new[] { UpdateType.Message }
            );

            foreach (var u in updates)
            {
                _offset = Math.Max(_offset, u.Id + 1);

                var msg = u.Message;
                if (msg?.Text is null) continue;

                var chatId = msg.Chat.Id;
                var text = msg.Text;

                await _bot.SendTextMessageAsync(chatId, $"Sən yazdın: {text}");
            }

            return Ok(new { received = updates.Length, nextOffset = _offset });
        }
    }
}
