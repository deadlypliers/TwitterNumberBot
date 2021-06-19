using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TwitterBot.Api.Domain.DTO;
using TwitterBot.Api.Domain.Model.Error;
using TwitterBot.Api.Domain.Model.Meta;
using TwitterBot.Api.Domain.Model.Tweets;

namespace TwitterBot.Api.Client
{
    public abstract class BaseApiClient
    {
        protected HttpClient Client;

        protected BaseApiClient(string token, string apiUrl)
        {
            Client = new HttpClient {BaseAddress = new Uri(apiUrl)};

            if (token != null)
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        #region HttpMethods

        protected async Task<ResponseWrapper<T>> Get<T>(string endpoint)
        {
            var response = await Client.GetAsync(endpoint);

            return await ParseResponse<T>(response);
        }

        protected async Task<ResponseWrapper<T>> Post<T>(string endpoint, HttpContent content)
        {
            var response = await Client.PostAsync(endpoint, content);

            return await ParseResponse<T>(response);
        }

        protected async Task<ResponseWrapper<T>> Put<T>(string endpoint, HttpContent content)
        {
            var response = await Client.PutAsync(endpoint, content);

            return await ParseResponse<T>(response);
        }

        protected async Task<ResponseWrapper<T>> Patch<T>(string endpoint, HttpContent content)
        {
            var response = await Client.PatchAsync(endpoint, content);

            return await ParseResponse<T>(response);
        }

        protected async Task<ResponseWrapper<T>> Delete<T>(string endpoint)
        {
            var response = await Client.DeleteAsync(endpoint);

            return await ParseResponse<T>(response);
        }

        private static async Task<ResponseWrapper<T>> ParseResponse<T>(HttpResponseMessage response)
        {
            // Handle HTTP errors
            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new WebException("API Responded with HTTP Status 400 (Bad Request)");
                    case HttpStatusCode.Unauthorized:
                        throw new WebException("API Responded with HTTP Status 401 (Unauthorized)");
                    case HttpStatusCode.Forbidden:
                        throw new WebException("API Responded with HTTP Status 403 (Forbidden)");
                    case HttpStatusCode.NotFound:
                        throw new WebException("API Responded with HTTP Status 404 (Not Found)");
                    case HttpStatusCode.MethodNotAllowed:
                        throw new WebException("API Responded with HTTP Status 405 (Not Allowed)");
                    case HttpStatusCode.NotAcceptable:
                        throw new WebException("API Responded with HTTP Status 406 (Not Acceptable)");
                    case HttpStatusCode.RequestTimeout:
                        throw new WebException("API Responded with HTTP Status 408 (Request Timeout)");
                    case HttpStatusCode.Conflict:
                        throw new WebException("API Responded with HTTP Status 409 (Conflict)");
                    case HttpStatusCode.InternalServerError:
                        throw new WebException("API Responded with HTTP Status 500 (Internal Server Error)");
                    case HttpStatusCode.ServiceUnavailable:
                        throw new WebException("API Responded with HTTP Status 503 (Service Unavailable)");
                    default:
                        throw new WebException("An unknown error was returned from the API: HTTP Status " + response.StatusCode);
                }
            }

            var responseWrapper = new ResponseWrapper<T>();
            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
                return default;

            var parsedJson = JObject.Parse(json);

            responseWrapper.Errors = parsedJson["errors"] != null ?
                JsonConvert.DeserializeObject<List<TwitterError>>(parsedJson["errors"].ToString()) :
                default;

            responseWrapper.Data = parsedJson["data"] != null ?
                JsonConvert.DeserializeObject<T>(parsedJson["data"].ToString()) :
                default;

            responseWrapper.Includes = parsedJson["includes"] != null ?
                JsonConvert.DeserializeObject<Includes>(parsedJson["includes"].ToString()) :
                default;

            responseWrapper.Meta = parsedJson["meta"] != null ?
                JsonConvert.DeserializeObject<BaseTwitterMeta>(parsedJson["meta"].ToString()) :
                default;

            return responseWrapper;
        }

        #endregion
    }
}
