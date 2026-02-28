using Telegram.Bot;
using VideoDownloader.InstagramServices;
using VideoDownloader.TelegramBackgroundService;
using VideoDownloader.TikTokEntities;
using VideoDownloader.YoutubeServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["Telegram:BotToken"]!)
);

//builder.Services.AddHostedService<TelegramBotWorker>();
builder.Services.AddSingleton<ITikTokDownloadService, TikTokDownloadService>();
builder.Services.AddSingleton<IInstagramDownloadService, InstagramDownloadService>();
builder.Services.AddSingleton<IYouTubeDownloadService, YouTubeDownloadService>();

builder.Services.AddHttpClient();
var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
