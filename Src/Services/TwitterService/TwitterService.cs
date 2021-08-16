using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Twitter;
using OWArcadeBackend.Services.ConfigService;

namespace OWArcadeBackend.Services.TwitterService
{
    public class TwitterService : ITwitterService
    {
        private readonly ILogger<TwitterService> _logger;
        private readonly IConfigService _configService;
        private readonly IOperations _operations;
        private readonly IConfiguration _configuration;

        private const string URL_CONFIGURATION_KEY = "OWScreenshotUrl";

        public TwitterService(ILogger<TwitterService> logger, IConfigService configService, IOperations operations, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _operations = operations ?? throw new ArgumentNullException(nameof(operations));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

        public async Task CreateScreenshot()
        {
            var url = _configuration.GetValue<string>(URL_CONFIGURATION_KEY);
            if (String.IsNullOrWhiteSpace(url))
            {
                _logger.LogError($"URL Configuration is empty: {url}");
                throw new Exception("URL Configuration is empty");
            }
            
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                // This will write shared memory files into /tmp instead of /dev/shm  https://bugs.chromium.org/p/chromium/issues/detail?id=736452
                Args = new[] { "--disable-dev-shm-usage" }
            });
            var page = await browser.NewPageAsync(new BrowserNewPageOptions()
            {
                ViewportSize = new ViewportSize()
                {
                    Height = 1080,
                    Width = 1920
                }
            });
            try
            {
                await page.GotoAsync(url);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = ImageConstants.IMG_OW_SCREENSHOT, FullPage = true});
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception was thrown when making a screenshot: {e.Message}");
                throw;
            }
        }

        public async Task Handle(Game overwatchType)
        {
            await CreateScreenshot();
            var media = _operations.UploadImageFromPath(ImageConstants.IMG_OW_SCREENSHOT);
            await _operations.PostTweetWithMedia(await CreateTweetText(), media);
        }
    }
}