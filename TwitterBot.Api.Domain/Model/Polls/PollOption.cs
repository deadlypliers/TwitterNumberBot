using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Polls
{
    public class PollOption : TwitterEntityBase
    {
        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("votes")]
        public string Votes { get; set; }
    }
}
