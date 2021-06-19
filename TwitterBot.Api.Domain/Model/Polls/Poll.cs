using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Polls
{
    public class Poll : TwitterEntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("options")]
        public List<PollOption> PollOptions { get; set; }

        [JsonProperty("duration_minutes")]
        public int DurationMinutes { get; set; }

        [JsonProperty("end_datetime")]
        public DateTime? EndDateTime { get; set; }

        [JsonProperty("voting_status")]
        public string VotingStatus { get; set; }
    }
}
