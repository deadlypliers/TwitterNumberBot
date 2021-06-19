using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Entities
{
    public class Annotation : TwitterEntityBase
    {
        [JsonProperty("start")]
        public int? Start { get; set; }

        [JsonProperty("end")]
        public int? End { get; set; }

        [JsonProperty("probability")]
        public float? Probability { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("normalized_text")]
        public string NormalizedText { get; set; }
    }
}
