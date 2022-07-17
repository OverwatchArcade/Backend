using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OverwatchArcade.Twitter.Factories;
using OverwatchArcade.Twitter.Services.ScreenshotService;
using Polly;
using Polly.Retry;
using Tweetinvi.Parameters;

namespace OverwatchArcade.Twitter.Services.TwitterService
{
    public class TwitterService : ITwitterService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TwitterService> _logger;
        private readonly IScreenshotService _screenshotService;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ITwitterClientFactory _twitterClientFactory;

        public TwitterService(IConfiguration configuration, ILogger<TwitterService> logger, IScreenshotService screenshotService, ITwitterClientFactory twitterClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _screenshotService = screenshotService ?? throw new ArgumentNullException(nameof(screenshotService));
            _twitterClientFactory = twitterClientFactory ?? throw new ArgumentNullException(nameof(twitterClientFactory));
            
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(30)
                });
        }

        private string CreateTweetText(string currentEvent)
        {
            if (!currentEvent.Equals("default", StringComparison.CurrentCultureIgnoreCase))
            {
                return $"Today's Overwatch Arcademodes, (Event: {currentEvent}) - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade";
            }

            return $"Today's Overwatch Arcademodes - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade";
        }
        
        public async Task PostTweet(string screenshotUrl, string currentEvent)
        {
            await _retryPolicy.ExecuteAsync(() => PostTweetAction(screenshotUrl, currentEvent));
        }

        private async Task PostTweetAction(string screenshotUrl, string currentEvent)
        {
            try
            {
                var client = _twitterClientFactory.Create();
                var screenshot = await _screenshotService.CreateScreenshot(screenshotUrl);
                var uploadedImage = await client.Upload.UploadTweetImageAsync(screenshot);
                await client.Tweets.PublishTweetAsync(new PublishTweetParameters
                {
                    Text = CreateTweetText(currentEvent),
                    Medias = { uploadedImage }
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"Couldn't Post tweet - {e.Message}");
                throw;
            }
        }

        public async Task DeleteLastTweet()
        {
            await _retryPolicy.ExecuteAsync(DeleteLastTweetAction);
        }
        
        private async Task DeleteLastTweetAction()
        {
            try
            {
                var client = _twitterClientFactory.Create();
                var twitterUsername = _configuration.GetSection("TwitterUsername").Value ?? throw new Exception("Twitter username not found");
                var tweets = await client.Timelines.GetUserTimelineAsync(twitterUsername);
                var tweetsToBeDeleted = tweets.FirstOrDefault(tweet => tweet.CreatedAt >= DateTime.Today.ToUniversalTime());
                if (tweetsToBeDeleted is null)
                {
                    _logger.LogInformation("No tweet found to be deleted");
                }
                else
                {
                    await client.Tweets.DestroyTweetAsync(tweetsToBeDeleted);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("I tried to delete a Tweet that couldn't be found. Hard deleting without a tweet sent?");
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}