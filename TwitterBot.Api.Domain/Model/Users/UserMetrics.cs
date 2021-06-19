using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Users
{
    public class UserMetrics : TwitterEntityBase
    {
        [JsonProperty("followers_count")]
        public int? FollowersCount { get; set; }

        [JsonProperty("following_count")]
        public int? FollowingCount { get; set; }

        [JsonProperty("tweet_count")]
        public int? TweetCount { get; set; }

        [JsonProperty("listed_count")]
        public int? ListedCount { get; set; }
    }
}
