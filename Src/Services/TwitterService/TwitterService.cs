using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
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

        public void CreateScreenshot()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--headless");
            chromeOptions.AddArgument("--window-size=1920,1080");
            _logger.LogInformation(Environment.CurrentDirectory);
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Environment.CurrentDirectory);
            chromeDriverService.HideCommandPromptWindow = true;
            var driver = new ChromeDriver(chromeDriverService, chromeOptions);
            try
            {
                var url = _configuration.GetValue<string>(URL_CONFIGURATION_KEY);
                if (String.IsNullOrWhiteSpace(url))
                {
                    _logger.LogError($"URL Configuration is empty: {url}");
                    throw new Exception("URL Configuration is empty");
                }

                driver.Navigate().GoToUrl(url);
                Thread.Sleep(5000);
                driver.GetScreenshot().SaveAsFile(ImageConstants.IMG_OW_SCREENSHOT);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception was thrown when making a screenshot: {e.Message}");
                throw;
            }
            finally
            {
                driver.Close();
                driver.Quit();
            }
        }

        public async Task Handle(Game overwatchType)
        {
            CreateScreenshot();
            var media = _operations.UploadImageFromPath(ImageConstants.IMG_OW_SCREENSHOT);
            await _operations.PostTweetWithMedia(await CreateTweetText(), media);
        }
    }
}