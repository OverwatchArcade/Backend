using Tweetinvi;

namespace OverwatchArcade.API.Factories
{
    public interface ITwitterClientFactory
    {
        public ITwitterClient Create();
    }
}