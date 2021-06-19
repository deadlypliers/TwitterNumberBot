using System.Collections.Generic;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Search.Stream.Rules
{
    public class RulesRequest : TwitterEntityBase
    {
        [JsonProperty("add")]
        public List<RuleTrack> Add { get; set; }

        [JsonProperty("delete")]
        public Delete Delete { get; set; }

        public RulesRequest()
        {
            Add = new List<RuleTrack>();
        }

        public RulesRequest(List<RuleTrack> add)
        {
            Add = add;
        }

        public RulesRequest(Delete delete)
        {
            Delete = delete;
        }
    }
}
