using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace VideoDownloader.Controllers
{
    [ApiController]
    [Route("telegram")]
    public class TelegramSetupController : ControllerBase
    {
        private readonly ITelegramBotClient _bot;
        private readonly IConfiguration _config;

        public TelegramSetupController(ITelegramBotClient bot, IConfiguration config)
        {
            _bot = bot;
            _config = config;
        }

        [HttpPost("set-webhook")]
        public async Task<IActionResult> SetWebhook(CancellationToken ct)
        {
            var publicUrl = _config["Telegram:PublicBaseUrl"]; // məsələn https://xxx.onrender.com
            if (string.IsNullOrWhiteSpace(publicUrl))
                return BadRequest("Telegram:PublicBaseUrl is missing");

            var hook = publicUrl.TrimEnd('/') + "/telegram/update";
            await _bot.SetWebhookAsync(hook, cancellationToken: ct);

            return Ok(new { webhook = hook });
        }

        [HttpPost("delete-webhook")]
        public async Task<IActionResult> DeleteWebhook(CancellationToken ct)
        {
            await _bot.DeleteWebhookAsync(cancellationToken: ct);
            return Ok();
        }
    }
}
