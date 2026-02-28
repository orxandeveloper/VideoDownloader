using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{
    public class TikTokResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("processed_time")]
        public double ProcessedTime { get; set; }

        [JsonPropertyName("data")]
        public TikTokData Data { get; set; }
    }
}
