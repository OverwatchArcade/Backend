using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using OWArcadeBackend.Factories.Interfaces;
using static OWArcadeBackend.Factories.OAuthRequestFactory;

namespace OWArcadeBackend.Models.Twitter
{
    // Source from: 
    // https://github.com/salivosa/TwitterOps

    public class ApiHandler : IApiHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOAuthRequestFactory _oAuthRequestFactory;
        
        public ApiHandler(IHttpClientFactory httpClientFactory, IOAuthRequestFactory oAuthRequestFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _oAuthRequestFactory = oAuthRequestFactory ?? throw new ArgumentNullException(nameof(oAuthRequestFactory));
        }
        
        /// <summary>
        /// Make an API request with OAuth 1.0 and body parameters (defined key:value in dictionary)
        /// </summary>
        public async Task<string> RequestApiOAuthAsync(string url, HttpRequestMethods method, Dictionary<string, object> body)
        {
            // Generate OAuthRequest data (library used: OAuth.DotNetCore from Robert Hargreaves)
            var client = _oAuthRequestFactory.CreateTwitterOAuthRequest(url, method);

            // Format authorization as header string
            var auth = client.GetAuthorizationHeader();
            var httpClient = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(new HttpMethod(client.Method), client.RequestUrl);
            request.Headers.TryAddWithoutValidation("Authorization", auth);

            var multipartFormDataContent = new MultipartFormDataContent();

            foreach (var (key, value) in body)
                multipartFormDataContent.Add(new StringContent(value.ToString() ?? throw new InvalidOperationException()), $"\"{key}\"");

            request.Content = multipartFormDataContent;
            var response = await httpClient.SendAsync(request);

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}