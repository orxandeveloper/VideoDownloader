using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{
    public class MusicInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("play")]
        public string Play { get; set; }

        [JsonPropertyName("cover")]
        public string Cover { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("original")]
        public bool Original { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("album")]
        public string Album { get; set; }
    }
}
