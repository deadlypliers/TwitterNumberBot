using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitterBot.Api.Domain.Model
{
    public class Delete
    {
        [JsonProperty("ids")]
        private List<string> Ids { get; set; }
    }
}
