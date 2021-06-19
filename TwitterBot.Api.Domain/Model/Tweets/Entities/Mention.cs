using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Entities
{
    public class Mention : TwitterEntityBase
    {
        [JsonProperty("start")]
        public int? Start { get; set; }

        [JsonProperty("end")]
        public int? End { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
