using System;
using Microsoft.Extensions.Configuration;
using OAuth;
using OWArcadeBackend.Factories.Interfaces;

namespace OWArcadeBackend.Factories
{
    public class OAuthRequestFactory : IOAuthRequestFactory
    {
        private readonly IConfiguration _configuration;

        public OAuthRequestFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public enum HttpRequestMethods
        {
            POST
        }
        
        public OAuthRequest CreateTwitterOAuthRequest(string url, HttpRequestMethods method)
        {
            //Generate OAuthRequest data (library used: OAuth.DotNetCore from Robert Hargreaves)
            return new OAuthRequest
            {
                Method = method.ToString(),
                SignatureTreatment = OAuthSignatureTreatment.Escaped,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = _configuration["Twitter:ConsumerKey"],
                ConsumerSecret = _configuration["Twitter:ConsumerSecret"],
                Token = _configuration["Twitter:TokenValue"],
                TokenSecret = _configuration["Twitter:TokenSecret"],
                RequestUrl = url,
                Version = "1.0"
            };
        }
    }
}