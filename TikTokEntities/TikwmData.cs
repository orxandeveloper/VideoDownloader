using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{
    public class TikwmData
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("play")]
        public string Play { get; set; }      // watermark olmayan mp4 (çox vaxt)

        [JsonPropertyName("wmplay")]
        public string WmPlay { get; set; }    // watermarklı mp4
    }
}
