using System.Collections.Generic;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets
{
    public class Attachment : TwitterEntityBase
    {
        [JsonProperty("media_keys")]
        public List<string> MediaKeys { get; set; }

        [JsonProperty("poll_ids")]
        public List<string> PollIds { get; set; }
    }
}
