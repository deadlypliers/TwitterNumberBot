using System;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Meta
{
    public abstract class BaseTwitterMeta : TwitterEntityBase
    {
        [JsonProperty("sent")]
        public DateTime Sent { get; set; }

        [JsonProperty("summary")]
        public MetaSummary Summary { get; set; }
    }
}
