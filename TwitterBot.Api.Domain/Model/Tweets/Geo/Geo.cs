using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Geo
{
    public class Geo : TwitterEntityBase
    {
        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }
    }
}
