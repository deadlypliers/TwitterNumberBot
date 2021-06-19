using Newtonsoft.Json;

namespace TwitterBot.Api.Domain.Model.Meta
{
    public class DeleteRuleTwitterMeta : BaseTwitterMeta
    {
        [JsonProperty("summary")]
        public MetaSummary Summary { get; set; }
    }
}
