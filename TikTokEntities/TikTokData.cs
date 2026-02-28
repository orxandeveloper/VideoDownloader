using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{
    public class TikTokData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("content_desc")]
        public List<string> ContentDesc { get; set; }

        [JsonPropertyName("cover")]
        public string Cover { get; set; }

        [JsonPropertyName("ai_dynamic_cover")]
        public string AiDynamicCover { get; set; }

        [JsonPropertyName("origin_cover")]
        public string OriginCover { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("play")]
        public string Play { get; set; }

        [JsonPropertyName("wmplay")]
        public string WmPlay { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("wm_size")]
        public long WmSize { get; set; }

        [JsonPropertyName("music")]
        public string Music { get; set; }

        [JsonPropertyName("music_info")]
        public MusicInfo MusicInfo { get; set; }

        [JsonPropertyName("play_count")]
        public long PlayCount { get; set; }

        [JsonPropertyName("digg_count")]
        public long DiggCount { get; set; }

        [JsonPropertyName("comment_count")]
        public long CommentCount { get; set; }

        [JsonPropertyName("share_count")]
        public long ShareCount { get; set; }

        [JsonPropertyName("download_count")]
        public long DownloadCount { get; set; }

        [JsonPropertyName("collect_count")]
        public long CollectCount { get; set; }

        [JsonPropertyName("create_time")]
        public long CreateTime { get; set; }

        [JsonPropertyName("is_ad")]
        public bool IsAd { get; set; }

        [JsonPropertyName("commerce_info")]
        public CommerceInfo CommerceInfo { get; set; }

        [JsonPropertyName("author")]
        public Author Author { get; set; }
    }
}
