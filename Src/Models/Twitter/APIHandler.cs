using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OAuth;

namespace OWArcadeBackend.Models.Twitter
{
    // Source from: 
    // https://github.com/salivosa/TwitterOps

    public class ApiHandler : IApiHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        //API keys of Twitter App
        private string ConsumerKey { get; set; }
        private string ConsumerSecret { get; set; }

        //Tokens of user (can be genenerated and obtained by twurl)
        private string TokenValue { get; set; }
        private string TokenSecret { get; set; }

        //Enum of Methods used in Requests
        public enum Method
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        /// <summary>
        /// Load class with API and user's keys data
        /// </summary>
        public ApiHandler(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            ConsumerKey = configuration["Twitter:ConsumerKey"];
            ConsumerSecret = configuration["Twitter:ConsumerSecret"];
            TokenValue = configuration["Twitter:TokenValue"];
            TokenSecret = configuration["Twitter:TokenSecret"];
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// Make an API request with OAuth 1.0
        /// </summary>
        public async Task<string> RequestApioAuthAsync(string url, Method method)
        {
            //Generate OAuthRequest data (library used: OAuth.DotNetCore from Robert Hargreaves)
            OAuthRequest client = new OAuthRequest
            {
                Method = method.ToString(),
                SignatureTreatment = OAuthSignatureTreatment.Escaped,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = ConsumerKey,
                ConsumerSecret = ConsumerSecret,
                Token = TokenValue,
                TokenSecret = TokenSecret,
                RequestUrl = url,
                Version = "1.0"
            };

            // Format authorization as header string
            string auth = client.GetAuthorizationHeader();
            var httpClient = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(new HttpMethod(client.Method), client.RequestUrl);
            request.Headers.TryAddWithoutValidation("Authorization", auth);

            var response = await httpClient.SendAsync(request);

            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Make an API request with OAuth 1.0 and body parameters (defined key:value in dictionary)
        /// </summary>
        public async Task<string> RequestApioAuthAsync(string url, Method method, Dictionary<string, object> body)
        {
            //Generate OAuthRequest data (library used: OAuth.DotNetCore from Robert Hargreaves)
            OAuthRequest client = new OAuthRequest
            {
                Method = method.ToString(),
                SignatureTreatment = OAuthSignatureTreatment.Escaped,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = ConsumerKey,
                ConsumerSecret = ConsumerSecret,
                Token = TokenValue,
                TokenSecret = TokenSecret,
                RequestUrl = url,
                Version = "1.0"
            };

            // Format authorization as header string
            string auth = client.GetAuthorizationHeader();
            var httpClient = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(new HttpMethod(client.Method), client.RequestUrl);
            request.Headers.TryAddWithoutValidation("Authorization", auth);

            var multipartFormDataContent = new MultipartFormDataContent();

            foreach (var item in body)
                multipartFormDataContent.Add(new StringContent(item.Value.ToString()), string.Format("\"{0}\"", item.Key));

            request.Content = multipartFormDataContent;

            var response = await httpClient.SendAsync(request);

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}