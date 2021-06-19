using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TwitterBot.Api.Domain.DTO;
using TwitterBot.Api.Domain.Model.Tweets.Search.Stream.Rules;

namespace TwitterBot.Api.Client.Tweets.Search.Stream
{
    public class RulesClient : BaseApiClient
    {
        private const string Endpoint = "2/tweets/search/stream/rules";

        public RulesClient(string token, string apiUrl) : base(token, apiUrl) { }

        public async Task<ResponseWrapper<List<RulesResponse>>> AddRules(RulesRequest request)
        {
            var url = $"{Endpoint}";

            var content = new StringContent(request.ToJsonString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await Post<List<RulesResponse>>(url, content);
        }

        public async Task<ResponseWrapper<List<RulesResponse>>> ValidateRules(RulesRequest request)
        {
            var url = $"{Endpoint}?dry_run=true";

            var content = new StringContent(request.ToJsonString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await Post<List<RulesResponse>>(url, content);
        }

        public async Task<ResponseWrapper<string>> DeleteRules(RulesRequest request)
        {
            var url = $"{Endpoint}";

            var content = new StringContent(request.ToJsonString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await Post<string>(url, content);
        }

        public async Task<ResponseWrapper<List<RulesResponse>>> GetRules(List<string> ids = null)
        {
            var url = $"{Endpoint}";

            if (ids == null) return await Get<List<RulesResponse>>(url);

            url += "?ids="; //add query string
            url = ids.Aggregate(url, (current, id) => current + $"{id},"); // aggregate all ids
            url = url.Remove(url.Length - 1, 1); //remove trailing comma

            return await Get<List<RulesResponse>>(url);
        }
    }
}
