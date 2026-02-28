using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{ 
    public class Author
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("unique_id")]
        public string UniqueId { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }
}
