using OAuth;
using static OWArcadeBackend.Factories.OAuthRequestFactory;

namespace OWArcadeBackend.Factories.Interfaces
{
    public interface IOAuthRequestFactory
    {
        public OAuthRequest CreateTwitterOAuthRequest(string url, HttpRequestMethods method);
    }
}