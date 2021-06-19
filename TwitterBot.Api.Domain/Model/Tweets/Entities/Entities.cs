using System.Collections.Generic;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Entities
{
    public class Entities : TwitterEntityBase
    {
        [JsonProperty("annotations")]
        public List<Annotation> Annotations { get; set; }

        [JsonProperty("urls")]
        public List<TweetUrl> Urls { get; set; }

        [JsonProperty("hashtags")]
        public List<Hashtag> Hashtags { get; set; }

        [JsonProperty("mentions")]
        public List<Mention> Mentions { get; set; }

        [JsonProperty("cashtags")]
        public List<Cashtag> Cashtags { get; set; }
    }
}
