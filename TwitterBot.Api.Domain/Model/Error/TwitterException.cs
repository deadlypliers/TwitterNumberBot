using System;

namespace TwitterBot.Api.Domain.Model.Error
{
    public class TwitterException : Exception
    {
        public string Endpoint { get; set; }

        public TwitterError Error { get; set; }
    }
}
