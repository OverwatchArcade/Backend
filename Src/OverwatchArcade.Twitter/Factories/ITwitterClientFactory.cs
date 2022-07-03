using Tweetinvi;

namespace OverwatchArcade.Twitter.Factories
{
    public interface ITwitterClientFactory
    {
        public ITwitterClient Create();
    }
}