using Newtonsoft.Json;
using System.Collections.Generic;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Error
{
    public class TwitterError : TwitterEntityBase
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("details")]
        public List<string> Details { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
