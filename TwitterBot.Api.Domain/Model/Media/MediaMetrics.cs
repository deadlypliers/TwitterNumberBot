using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Media
{
    public class MediaMetrics : TwitterEntityBase
    {
        [JsonProperty("playback_0_count")]
        public int? Playback0Count { get; set; }

        [JsonProperty("playback_25_count")]
        public int? Playback25Count { get; set; }

        [JsonProperty("playback_50_count")]
        public int? Playback50Count { get; set; }

        [JsonProperty("playback_75_count")]
        public int? Playback75Count { get; set; }

        [JsonProperty("playback_100_count")]
        public int? Playback100Count { get; set; }

        [JsonProperty("view_count")]
        public int? ViewCount { get; set; }
    }
}
