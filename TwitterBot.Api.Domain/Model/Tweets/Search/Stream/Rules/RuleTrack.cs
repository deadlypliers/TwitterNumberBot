using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Search.Stream.Rules
{
    public class RuleTrack : TwitterEntityBase
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("delete")]
        public Delete Delete { get; set; }
    }
}
