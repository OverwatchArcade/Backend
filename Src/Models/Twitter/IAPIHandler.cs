using System.Collections.Generic;
using System.Threading.Tasks;
using OWArcadeBackend.Factories;

namespace OWArcadeBackend.Models.Twitter
{
    public interface IApiHandler
    {
        public Task<string> RequestApiOAuthAsync(string url, OAuthRequestFactory.HttpRequestMethods method, Dictionary<string, object> body);
    }
}
