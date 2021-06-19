using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterBot.Api.Domain.Base;

namespace TwitterBot.Api.Domain.Model.Tweets.Search.Stream
{
    public class StreamRequest
    {
        public List<string> Expansions { get; set; }

        public List<string> MediaFields { get; set; }

        public List<string> PlaceFields { get; set; }

        public List<string> PollFields { get; set; }

        public List<string> TweetFields { get; set; }

        public List<string> UserFields { get; set; }
    }
}
