﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TwitterBot.Api.Domain.Base;
using TwitterBot.Api.Domain.Model.Tweets.ContextAnnotations;

namespace TwitterBot.Api.Domain.Model.Tweets
{
    public class Tweet : TwitterEntityBase
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("author_id")]
        public string AuthorId { get; set; }

        [JsonProperty("context_annotations")]
        public List<ContextAnnotation> ContextAnnotations { get; set; }

        [JsonProperty("conversation_id")]
        public string ConversationId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("entities")]
        public List<Entity> Entities { get; set; }

        [JsonProperty("geo")]
        public Geo.Geo Geo { get; set; }

        [JsonProperty("in_reply_to_user_id")]
        public string InReplyToUserId { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("non_public_metrics")]
        public TweetMetrics NonPublicTweetMetrics { get; set; }

        [JsonProperty("organic_metrics")]
        public TweetMetrics OrganicTweetMetrics { get; set; }

        [JsonProperty("possibly_sensitive")]
        public bool PossiblySensitive { get; set; }

        [JsonProperty("promoted_metrics")]
        public TweetMetrics PromotedTweetMetrics { get; set; }

        [JsonProperty("public_metrics")]
        public TweetMetrics PublicTweetMetrics { get; set; }

        [JsonProperty("referenced_tweets")]
        public List<Tweet> ReferencedTweets { get; set; }

        [JsonProperty("reply_settings")]
        public string ReplySettings { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("withheld")]
        public Withheld.Withheld Withheld { get; set; }
    }
}
