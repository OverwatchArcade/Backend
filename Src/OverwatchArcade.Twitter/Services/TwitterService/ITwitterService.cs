namespace OverwatchArcade.Twitter.Services.TwitterService
{
    public interface ITwitterService
    {
        public Task PostTweet(string screenshotUrl, string currentEvent);
        public Task DeleteLastTweet();
    }
}
