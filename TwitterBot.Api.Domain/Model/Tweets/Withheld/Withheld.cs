using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Withheld
{
    public class Withheld : TwitterEntityBase
    {
        [JsonProperty("copyright")]
        public bool Copyright { get; set; }

        [JsonProperty("country_codes")]
        public List<string> CountryCodes { get; set; }

        [JsonProperty("scope")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WithheldScope Scope { get; set; }
    }
}
