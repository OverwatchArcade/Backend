using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OWArcadeBackend.Factories;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Services.ConfigService;
using Tweetinvi.Parameters;

namespace OWArcadeBackend.Services.TwitterService
{
    public class TwitterService : ITwitterService
    {
        private readonly ILogger<TwitterService> _logger;
        private readonly IConfigService _configService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITwitterClientFactory _twitterClientFactory;
        private readonly IConfiguration _configuration;
        
        public TwitterService(ILogger<TwitterService> logger, IConfigService configService, IHttpClientFactory httpClientFactory, ITwitterClientFactory twitterClientFactory, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _twitterClientFactory = twitterClientFactory ?? throw new ArgumentNullException(nameof(twitterClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private static Dictionary<string, string> CreateHttpParams(IConfiguration configuration)
        {
            return new Dictionary<string, string>
            {
                { "access_key", configuration["APIFlash:Key"] },
                { "url", configuration["APIFlash:URL"] },
                { "user_agent", configuration["APIFlash:UA"] },
                { "ttl", "0" },
                { "fresh", "True" },
            };
        }

        private static string QueryString(IDictionary<string, string> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                list.Add(item.Key + "=" + item.Value);
            }

            return string.Join("&", list);
        }

        private async Task<string> CreateTweetText()
        {
            var currentEvent = await _configService.GetCurrentOverwatchEvent();
            if (!currentEvent.Data.Equals("default", StringComparison.CurrentCultureIgnoreCase))
            {
                return $"Today's Overwatch Arcademodes, (Event: {currentEvent.Data}) - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade";
            }

            return $"Today's Overwatch Arcademodes - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade";
        }

        private async Task CreateScreenshot()
        {
            var fileInfo = new FileInfo(ImageConstants.OwScreenshot);

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(TwitterConstants.ScreenshotServiceUrl + QueryString(CreateHttpParams(_configuration)));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Couldn't reach screenshot service APIFlash (Http code {response.StatusCode})");
                    throw new HttpRequestException();
                }

                await using var ms = await response.Content.ReadAsStreamAsync();
                await using var fs = File.Create(fileInfo.FullName);
                ms.Seek(0, SeekOrigin.Begin);
                await ms.CopyToAsync(fs);
            }
            catch (Exception e)
            {
                _logger.LogError($"CreateScreenshot failed  {e.Message}");
                throw;
            }
        }

        public async Task PostTweet(Game overwatchType)
        {
            await CreateScreenshot();
            var screenshot = await File.ReadAllBytesAsync(ImageConstants.OwScreenshot);
            var client = _twitterClientFactory.Create();
            var uploadedImage = await client.Upload.UploadTweetImageAsync(screenshot);
            await client.Tweets.PublishTweetAsync(new PublishTweetParameters
            {
                Text = await CreateTweetText(),
                Medias = { uploadedImage }
            });
        }

        public async Task DeleteLastTweet()
        {
            var client = _twitterClientFactory.Create();
            var twitterUsername = _configuration.GetValue<string>("TwitterUsername");
            var tweets = await client.Timelines.GetUserTimelineAsync(twitterUsername);
            try
            {
                var tweetsToBeDeleted = tweets.First(tweet => tweet.CreatedAt >= DateTime.Today.ToUniversalTime());
                await client.Tweets.DestroyTweetAsync(tweetsToBeDeleted);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}