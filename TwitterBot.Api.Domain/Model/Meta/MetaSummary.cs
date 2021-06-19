using Newtonsoft.Json;

namespace TwitterBot.Api.Domain.Model.Meta
{
    public class MetaSummary
    {
        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("not_created")]
        public int NotCreated { get; set; }

        [JsonProperty("valid")]
        public int Valid { get; set; }

        [JsonProperty("invalid")]
        public int Invalid { get; set; }

        [JsonProperty("deleted")]
        public int Deleted { get; set; }

        [JsonProperty("not_deleted")]
        public int NotDeleted { get; set; }
    }
}
