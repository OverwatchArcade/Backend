using OverwatchArcade.Domain.Models.Constants;

namespace OverwatchArcade.API.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task PostTweet(Game overwatchType);
        public Task DeleteLastTweet();
    }
}
