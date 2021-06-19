using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.ContextAnnotations
{
    public class ContextAnnotation : TwitterEntityBase
    {
        [JsonProperty("domain")]
        public Domain Domain { get; set; }

        [JsonProperty("entity")]
        public Entity Entity { get; set; }
    }
}
