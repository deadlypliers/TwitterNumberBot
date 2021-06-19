using Newtonsoft.Json;
using System.Collections.Generic;
using TwitterBot.Api.Domain.Model.Error;
using TwitterBot.Api.Domain.Model.Meta;
using TwitterBot.Api.Domain.Model.Tweets;
using TwitterBot.Api.Domain.Model.Tweets.Search.Stream;

namespace TwitterBot.Api.Domain.DTO
{
    public class ResponseWrapper<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("includes")]
        public Includes Includes { get; set; }

        [JsonProperty("meta")]
        public BaseTwitterMeta Meta { get; set; }

        [JsonProperty("errors")]
        public List<TwitterError> Errors { get; set; }
    }
}
