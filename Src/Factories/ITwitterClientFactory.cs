using Tweetinvi;

namespace OWArcadeBackend.Factories
{
    public interface ITwitterClientFactory
    {
        public ITwitterClient Create();
    }
}