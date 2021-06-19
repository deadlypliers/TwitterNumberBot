using System.Collections.Generic;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;
using TwitterBot.Api.Domain.Model.Tweets.Geo;

namespace TwitterBot.Api.Domain.Model
{
    public class Place : TwitterEntityBase
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("contained_within")]
        public List<Place> ContainedWithin { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("geo")]
        public Geo Geo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("place_type")]
        public string PlaceType { get; set; }
    }
}
