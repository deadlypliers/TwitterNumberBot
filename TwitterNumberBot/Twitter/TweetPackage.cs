using System.Linq;
using Tweetinvi.Models.V2;

namespace TwitterNumberBot.Twitter
{
    public class TweetPackage
    {
        public TweetPackage(TweetV2 tweet, TweetIncludesV2 includes, string phoneNumber)
        {
            Tweet = tweet;
            _includes = includes;
            User = includes.Users.First(u => u.Id == tweet.AuthorId);
            UserRepliedTo = includes.Users.FirstOrDefault(u => u.Id == tweet.InReplyToUserId);
            PhoneNumber = phoneNumber;
            _includes = includes;
        }

        public TweetV2 Tweet { get; }

        public UserV2 User { get; }

        public UserV2 UserRepliedTo { get; }

        public string PhoneNumber { get; }

        private TweetIncludesV2 _includes;
    }
}
