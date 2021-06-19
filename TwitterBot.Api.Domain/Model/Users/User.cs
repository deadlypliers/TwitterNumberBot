using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TwitterBot.Api.Domain.Base;
using TwitterBot.Api.Domain.Model.Tweets.ContextAnnotations;
using TwitterBot.Api.Domain.Model.Tweets.Withheld;

namespace TwitterBot.Api.Domain.Model.Users
{
    public class User : TwitterEntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("entities")]
        public List<Entity> Entities { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("pinned_tweet_id")]
        public string PinnedTweetId { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("protected")]
        public bool Protected { get; set; }

        [JsonProperty("public_metrics")]
        public UserMetrics PublicTweetMetrics { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("withheld")]
        public Withheld Withheld { get; set; }
    }
}
