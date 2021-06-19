using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Entities
{
    public class TweetUrl : TwitterEntityBase
    {
        [JsonProperty("start")]
        public int? Start { get; set; }

        [JsonProperty("end")]
        public int? End { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; set; }

        [JsonProperty("displayed_url")]
        public string DisplayedUrl { get; set; }

        [JsonProperty("unwound_url")]
        public string UnwoundUrl { get; set; }
    }
}
