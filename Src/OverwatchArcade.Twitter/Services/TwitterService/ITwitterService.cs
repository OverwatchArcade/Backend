using OverwatchArcade.Twitter.Dtos;

namespace OverwatchArcade.Twitter.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task PostTweet(CreateTweetDto createTweetDto);
        public Task DeleteLastTweet();
    }
}
