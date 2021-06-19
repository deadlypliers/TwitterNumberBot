using System.Collections.Generic;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;
using TwitterBot.Api.Domain.Model.Polls;
using TwitterBot.Api.Domain.Model.Users;

namespace TwitterBot.Api.Domain.Model.Tweets
{
    public class Includes : TwitterEntityBase
    {
        [JsonProperty("tweets")]
        public List<Tweet> Tweets { get; set; }

        [JsonProperty("users")]
        public List<User> Users { get; set; }

        [JsonProperty("places")]
        public List<Place> Places { get; set; }

        [JsonProperty("media")]
        public List<Media.Media> Media { get; set; }

        [JsonProperty("polls")]
        public List<Poll> Polls { get; set; }
    }
}
