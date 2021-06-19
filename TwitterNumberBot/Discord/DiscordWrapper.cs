using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwitterNumberBot.Twitter;

namespace TwitterNumberBot.Discord
{
    public class DiscordWrapper
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly string _token;
        private readonly ulong _guildId;
        private readonly ulong _channelId;
        private bool _connected;

        #region Constructor

        public DiscordWrapper(string token, string guildId, string channelId)
        {
            this._token = token;
            this._guildId = Convert.ToUInt64(guildId);
            this._channelId = Convert.ToUInt64(channelId);

            _discordClient = new DiscordSocketClient();
        }

        #endregion

        #region Public Methods

        public async Task Login()
        {
            await _discordClient.LoginAsync(TokenType.Bot, _token);

            _discordClient.Connected += DiscordConnected;
            _discordClient.Disconnected += DiscordDisconnected;
            await _discordClient.StartAsync();
        }

        public async Task Logout()
        {
            await _discordClient.StopAsync();
            await _discordClient.LogoutAsync();
        }

        public async Task WritePost(TweetPackage tweetDetails)
        {
            if (_discordClient.LoginState != LoginState.LoggedIn)
                await Login();

            var retries = 0;
            var maxRetries = 3;

            while (!_connected && retries < maxRetries)
            {
                retries++;
                Thread.Sleep(2000);
            }

            var guild = _discordClient.GetGuild(_guildId);
            var channel = guild.GetTextChannel(_channelId);

            var embedAuthorBuilder = new EmbedAuthorBuilder()
            {
                IconUrl = tweetDetails.User.ProfileImageUrl,
                Name = $"@{tweetDetails.User.Username}",
                Url = $"https://www.twitter.com/peepeepoopoo/status/{tweetDetails.Tweet.Id}"
            };

            var embedFieldsBuilders = new List<EmbedFieldBuilder>();

            if (tweetDetails.UserRepliedTo != null)
            {
                var tweetReplyToField = new EmbedFieldBuilder()
                {
                    IsInline = false,
                    Name = "Tweet Reply To",
                    Value = $"@{tweetDetails.UserRepliedTo.Username}"
                };
                embedFieldsBuilders.Add(tweetReplyToField);
            }

            var tweetNumberField = new EmbedFieldBuilder()
            {
                IsInline = false,
                Name = "Phone Number",
                Value = tweetDetails.PhoneNumber
            };
            embedFieldsBuilders.Add(tweetNumberField);

            var embedBuilder = new EmbedBuilder
            {
                Author = embedAuthorBuilder,
                Color = Color.Default,
                Description = tweetDetails.Tweet.Text,
                Fields = embedFieldsBuilders,
                Footer = null,
                ThumbnailUrl = tweetDetails.User.ProfileImageUrl,
                Timestamp = tweetDetails.Tweet.CreatedAt,
            };

            var embed = embedBuilder.Build();

            await channel.SendMessageAsync(null, false, embed, null, AllowedMentions.None);
        }

        #endregion

        #region Private Methods

        private Task DiscordDisconnected(Exception arg)
        {
            _connected = false;
            return Task.CompletedTask;
        }

        private Task DiscordConnected()
        {
            _connected = true;
            return Task.CompletedTask;
        }

        #endregion
    }
}
