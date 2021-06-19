using Newtonsoft.Json;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Media
{
    public class Media : TwitterEntityBase
    {
        [JsonProperty("media_key")]
        public string MediaKey { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("duration_ms")]
        public int DurationMilliseconds { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("non_public_metrics")]
        public MediaMetrics NonPublicMetrics { get; set; }

        [JsonProperty("organic_metrics")]
        public MediaMetrics NonMediaMetrics { get; set; }

        [JsonProperty("preview_image_url")]
        public string PreviewImageUrl { get; set; }

        [JsonProperty("promoted_metrics")]
        public MediaMetrics PromotedMetrics { get; set; }

        [JsonProperty("public_metrics")]
        public MediaMetrics PublicMetrics { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
