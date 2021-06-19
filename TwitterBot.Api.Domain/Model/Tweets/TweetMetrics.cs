using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets
{
    public class TweetMetrics : TwitterEntityBase
    {
        [JsonProperty("retweet_count")]
        public int? RetweetCount { get; set; }

        [JsonProperty("reply_count")]
        public int? ReplyCount { get; set; }

        [JsonProperty("like_count")]
        public int? LikeCount { get; set; }

        [JsonProperty("quote_count")]
        public int? QuoteCount { get; set; }

        [JsonProperty("impression_count")]
        public int? ImpressionCount { get; set; }

        [JsonProperty("url_link_clicks")]
        public int? UrlLinkClicks { get; set; }

        [JsonProperty("user_profile_clicks")]
        public int? UserProfileClicks { get; set; }
    }
}
