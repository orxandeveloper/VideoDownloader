using System.Text.Json.Serialization;

namespace VideoDownloader.TikTokEntities
{
    public class CommerceInfo
    {
        [JsonPropertyName("adv_promotable")]
        public bool AdvPromotable { get; set; }

        [JsonPropertyName("auction_ad_invited")]
        public bool AuctionAdInvited { get; set; }

        [JsonPropertyName("branded_content_type")]
        public int BrandedContentType { get; set; }

        [JsonPropertyName("is_diversion_ad")]
        public int IsDiversionAd { get; set; }

        [JsonPropertyName("organic_log_extra")]
        public string OrganicLogExtra { get; set; }

        [JsonPropertyName("with_comment_filter_words")]
        public bool WithCommentFilterWords { get; set; }
    }
}
