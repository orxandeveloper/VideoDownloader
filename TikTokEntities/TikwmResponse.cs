using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{
    public class TikwmResponse
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("data")]
        public TikwmData Data { get; set; }
    }
}
