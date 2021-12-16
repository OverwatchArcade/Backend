using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OWArcadeBackend.Factories;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
using OWArcadeBackend.Services.ConfigService;
using OWArcadeBackend.Services.TwitterService;
using Shouldly;
using Tweetinvi;
using Tweetinvi.Parameters;
using Xunit;

namespace OWArcadeBackend.Tests.Services
{
    public class TwitterServiceTest
    {
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly Mock<ITwitterClientFactory> _twitterClientFactoryMock;
        private readonly Mock<ILogger<TwitterService>> _loggerMock;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        
        public TwitterServiceTest()
        {
            _configServiceMock = new Mock<IConfigService>();
            _configuration = new Mock<IConfiguration>();
            _twitterClientFactoryMock = new Mock<ITwitterClientFactory>();
            _loggerMock = new Mock<ILogger<TwitterService>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new TwitterService(_loggerMock.Object, _configServiceMock.Object, _httpClientFactoryMock.Object, _twitterClientFactoryMock.Object, _configuration.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new TwitterService(
                null,
                _configServiceMock.Object,
                _httpClientFactoryMock.Object,
                _twitterClientFactoryMock.Object,
                _configuration.Object
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                null,
                _httpClientFactoryMock.Object,
                _twitterClientFactoryMock.Object,
                _configuration.Object
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                _configServiceMock.Object,
                null,
                _twitterClientFactoryMock.Object,
                _configuration.Object
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                _configServiceMock.Object,
                _httpClientFactoryMock.Object,
                null,
                _configuration.Object
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                _configServiceMock.Object,
                _httpClientFactoryMock.Object,
                _twitterClientFactoryMock.Object,
                null
            ));
        }

        [Fact]
        public async Task TestHandle_CreatesScreenshot_DefaultEvent()
        {
            // arrange
            var serviceResponse = new ServiceResponse<string>()
            {
                Time = DateTime.Parse("03-20-2000"),
                Data = "default"
            };
            _configServiceMock.Setup(x => x.GetCurrentOverwatchEvent()).ReturnsAsync(serviceResponse);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };
            IUploadTweetImageParameters uploadedImage = null;
            IPublishTweetParameters tweet = null;
            
            var getScreenshotUrlConfigurationSection = new Mock<IConfigurationSection>();
            getScreenshotUrlConfigurationSection.Setup(x => x.Value).Returns("https://overwatcharcade.today/overwatch");
            _configuration.Setup(x => x.GetSection("ScreenshotUrl")).Returns(getScreenshotUrlConfigurationSection.Object);
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            var twitterClient = new Mock<ITwitterClient>();
            _twitterClientFactoryMock.Setup(x => x.Create()).Returns(twitterClient.Object);
            twitterClient.Setup(x => x.Upload.UploadTweetImageAsync(It.IsAny<IUploadTweetImageParameters>())).Callback<IUploadTweetImageParameters>(parameters => { uploadedImage = parameters; });
            twitterClient.Setup(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>())).Callback<IPublishTweetParameters>(parameters => { tweet = parameters; });

            // act & assert
            await Should.NotThrowAsync(() => new TwitterService(_loggerMock.Object,
                _configServiceMock.Object,
                _httpClientFactoryMock.Object,
                _twitterClientFactoryMock.Object,
                _configuration.Object).PostTweet(Game.OVERWATCH));
            twitterClient.Verify(x => x.Upload.UploadTweetImageAsync(It.IsAny<byte[]>()));
            twitterClient.Verify(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>()));
            tweet.Text.ShouldBeEquivalentTo($"Today's Overwatch Arcademodes - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade");
            tweet.HasMedia.ShouldBeTrue();
            tweet.Medias[0].ShouldBeEquivalentTo(uploadedImage);
        }

        [Fact]
        public async Task TestHandle_CreatesScreenshot_HalloweenEvent()
        {
            // arrange
            var serviceResponse = new ServiceResponse<string>()
            {
                Time = DateTime.Parse("03-20-2000"),
                Data = "Halloween"
            };
            _configServiceMock.Setup(x => x.GetCurrentOverwatchEvent()).ReturnsAsync(serviceResponse);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };
            IUploadTweetImageParameters uploadedImage = null;
            IPublishTweetParameters tweet = null;

            var getScreenshotUrlConfigurationSection = new Mock<IConfigurationSection>();
            getScreenshotUrlConfigurationSection.Setup(x => x.Value).Returns("https://overwatcharcade.today/overwatch");
            _configuration.Setup(x => x.GetSection("ScreenshotUrl")).Returns(getScreenshotUrlConfigurationSection.Object);
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            var twitterClient = new Mock<ITwitterClient>();
            _twitterClientFactoryMock.Setup(x => x.Create()).Returns(twitterClient.Object);
            twitterClient.Setup(x => x.Upload.UploadTweetImageAsync(It.IsAny<IUploadTweetImageParameters>())).Callback<IUploadTweetImageParameters>(parameters => { uploadedImage = parameters; });
            twitterClient.Setup(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>())).Callback<IPublishTweetParameters>(parameters => { tweet = parameters; });

            // act & assert
            await Should.NotThrowAsync(() => new TwitterService(_loggerMock.Object,
                _configServiceMock.Object,
                _httpClientFactoryMock.Object,
                _twitterClientFactoryMock.Object,
                _configuration.Object).PostTweet(Game.OVERWATCH));
            
            twitterClient.Verify(x => x.Upload.UploadTweetImageAsync(It.IsAny<byte[]>()));
            twitterClient.Verify(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>()));
            tweet.Text.ShouldBeEquivalentTo($"Today's Overwatch Arcademodes, (Event: Halloween) - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade");
            tweet.HasMedia.ShouldBeTrue();
            tweet.Medias[0].ShouldBeEquivalentTo(uploadedImage);
        }

        [Fact]
        public async Task TestHandle_CreatesScreenshot_Throws_Exception()
        {
            // arrange
            var serviceResponse = new ServiceResponse<string>()
            {
                Time = DateTime.Parse("03-20-2000"),
                Data = "default"
            };
            _configServiceMock.Setup(x => x.GetCurrentOverwatchEvent()).ReturnsAsync(serviceResponse);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.TooManyRequests,
            };

            var getScreenshotUrlConfigurationSection = new Mock<IConfigurationSection>();
            getScreenshotUrlConfigurationSection.Setup(x => x.Value).Returns("https://overwatcharcade.today/overwatch");
            _configuration.Setup(x => x.GetSection("ScreenshotUrl")).Returns(getScreenshotUrlConfigurationSection.Object);
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // act & assert
            await Should.ThrowAsync<HttpRequestException>(() => new TwitterService(_loggerMock.Object,
                _configServiceMock.Object,
                _httpClientFactoryMock.Object,
                _twitterClientFactoryMock.Object,
                _configuration.Object).PostTweet(Game.OVERWATCH));
        }
    }
}