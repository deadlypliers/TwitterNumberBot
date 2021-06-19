using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Events.V2;
using Tweetinvi.Parameters.V2;
using Tweetinvi.Streaming.V2;

namespace TwitterNumberBot.Twitter
{
    public class TwitterWrapper
    {
        public long TweetsReceived { get; private set; }
        public long LastSampledTweetCount { get; set; }
        public long TweetsWithValidNumbers { get; private set; }
        public long Reconnects { get; private set; }
        public ConcurrentQueue<TweetPackage> TweetQueue { get; }
        public Queue<LogTweet> BadTweetQueue { get; }
        
        private static TwitterClient _tweetClient;
        private static IFilteredStreamV2 _filteredStream;
        
        private const string RuleBase = "\"phone number\" OR (call (number OR phone OR cell)) OR (give \"a call\" -us) OR (\"call me\" at) OR (\"text me\" at) OR (\"contact me\" at) ";

        private const string RuleExclusions =
            "lang:en -has:media -#NiteFlirt -\"RT\" -whatsapp -india -\"+91\" -delhi -\"real estate\" -crisis -(realtor OR realty) -(cashapp OR \"cash app\") -mortgage -property -\"housing market\" -\"new listing\" -(sale property) -#homesforsale -#realestate -is:retweet";

        public string TweetLogPath { get; }

        #region Constructor
        public TwitterWrapper(string apiKey, string secretKey, string bearerToken)
        {
            TweetsReceived = 0;
            TweetsWithValidNumbers = 0;

            Reconnects = 0;

            TweetQueue = new ConcurrentQueue<TweetPackage>();
            BadTweetQueue = new Queue<LogTweet>();

            _tweetClient = new TwitterClient(apiKey, secretKey, bearerToken);

            TweetLogPath += $"D:\\TweetLogs\\{DateTime.Now.ToShortDateString().Replace("/", string.Empty).Replace("\\", string.Empty)}.csv";
        }

        public async Task ConfigureRules()
        {
            // Get Rules
            var rules = await _tweetClient.StreamsV2.GetRulesForFilteredStreamV2Async();

            var ruleIds = new string[rules.Rules.Length];
            for (var i = 0; i < rules.Rules.Length; i++)
            {
                ruleIds[i] = rules.Rules[i].Id;
            }

            //Delete rules and set up fresh
            if (ruleIds.Length > 0)
                await _tweetClient.StreamsV2.DeleteRulesFromFilteredStreamAsync(ruleIds);

            // Set up rules
            var response = await _tweetClient.StreamsV2.AddRulesToFilteredStreamAsync(new List<FilteredStreamRuleConfig>{
                //new FilteredStreamRuleConfig($"{RuleBase} {RuleExclusions}"),
                new FilteredStreamRuleConfig("\"call me\" (at OR on OR please OR number) -us -has:media -whatsapp -mobile -dhani -rs -(sir OR madam) -india -delhi -\"this article\" -\"real estate\" -\"buying or selling\" -(work home) -#NiteFlirt -emergency -suicide -crisis -scholarship -(realtor OR realty) -(cashapp OR \"cash app\") -DOJ -\"housing market\" -\"new listing\" -(home (escrow OR showing OR market OR selling OR client OR buying OR seller OR owning OR listing)) -(sale property) -#homesforsale -#realestate -is:retweet -gofundme -(break promise) -paper", "CallMe"),
                new FilteredStreamRuleConfig("\"phone number\" (my OR me) -hacker -has:media -whatsapp -\"what's app\" -mobile -dhani -rs -(sir OR madam) -india -delhi -\"this article\" -\"real estate\" -\"buying or selling\" -(work home) -#NiteFlirt -emergency -suicide -crisis -scholarship -(realtor OR realty) -(cashapp OR \"cash app\") -DOJ -\"housing market\" -\"new listing\" -(home (escrow OR showing OR market OR selling OR client OR buying OR seller OR owning OR listing)) -(sale property) -#homesforsale -#realestate -is:retweet -gofundme -(break promise) -paper", "MyPhoneNumber"),
                new FilteredStreamRuleConfig("\"give me\" \"a call\" -has:media -whatsapp -\"what's app\" -mobile -dhani -rs -(sir OR madam) -india -delhi -\"this article\" -\"real estate\" -\"buying or selling\" -(work home) -#NiteFlirt -emergency -suicide -crisis -scholarship -(realtor OR realty) -(cashapp OR \"cash app\") -DOJ -\"housing market\" -\"new listing\" -(home (escrow OR showing OR market OR selling OR client OR buying OR seller OR owning OR listing)) -(sale property) -#homesforsale -#realestate -is:retweet -gofundme -(break promise) -lyrics -paper", "GiveACall")
            }.ToArray());
        }

        public void ConfigureStream()
        {
            _filteredStream = _tweetClient.StreamsV2.CreateFilteredStream();
            _filteredStream.TweetReceived += GotTweet;
        }

        #endregion

        #region Public Methods

        public async Task SampleStream()
        {
            Reconnects++;
            await _filteredStream.StartAsync();
        }

        public void StopStream()
        {
            _filteredStream.StopStream();
        }

        #endregion

        #region Private Methods
        
        private void GotTweet(object sender, FilteredStreamTweetV2EventArgs e)
        {
            if (e.Tweet == null)
                return;

            TweetsReceived++;

            var phoneNumbers = Regex.Match(e.Tweet.Text, @"((^(\+?\1)?)|\b[1]?)\D?\(?([2-9]{1}[0-9]{2})\)?\D?([1-9]{1}[0-9]{2})\D?([0-9]{4})(?![-●\d])");

            if (string.IsNullOrWhiteSpace(phoneNumbers.Value))
            {
                BadTweetQueue.Enqueue(new LogTweet()
                {
                    Timestamp = e.Tweet.CreatedAt.DateTime,
                    Username = e.Includes.Users.First(u => u.Id == e.Tweet.AuthorId).Username,
                    TweetText = e.Tweet.Text
                });
                return;
            }

            TweetsWithValidNumbers++;

            var phoneNumber = string.Empty;
            foreach (var c in phoneNumbers.Value)
            {
                if (int.TryParse(c.ToString(), out var digit))
                    phoneNumber += digit;
            }

            var user = e.Includes.Users.First(u => u.Id == e.Tweet.AuthorId);
            var replyToUser = string.IsNullOrWhiteSpace(e.Tweet.InReplyToUserId)
                ? null
                : e.Includes.Users.FirstOrDefault(u => u.Id == e.Tweet.InReplyToUserId);

            if (replyToUser != null && replyToUser.Name.IsNullOrEmpty())
                replyToUser = null;

            TweetQueue.Enqueue(new TweetPackage(e.Tweet, e.Includes, phoneNumber));
        }

        #endregion
    }
}
