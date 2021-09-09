using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Twitter;
using OWArcadeBackend.Services.ConfigService;
using OWArcadeBackend.Services.TwitterService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Services
{
    public class TwitterServiceTest
    {
        private Mock<IConfigService> _configServiceMock;
        private Mock<IOperations> _operationsMock;
        private Mock<ILogger<TwitterService>> _loggerMock;
        private Mock<IConfiguration> _configuration;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        public TwitterServiceTest()
        {
            _configServiceMock = new Mock<IConfigService>();
            _configuration = new Mock<IConfiguration>();
            _operationsMock = new Mock<IOperations>();
            _loggerMock = new Mock<ILogger<TwitterService>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new TwitterService(_loggerMock.Object, _configServiceMock.Object, _operationsMock.Object, _configuration.Object, _httpClient);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new TwitterService(
                null,
                _configServiceMock.Object,
                _operationsMock.Object,
                _configuration.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                null,
                _operationsMock.Object,
                _configuration.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                _configServiceMock.Object,
                null,
                _configuration.Object,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                _configServiceMock.Object,
                _operationsMock.Object,
                null,
                _httpClient
            ));

            Should.Throw<ArgumentNullException>(() => new TwitterService(
                _loggerMock.Object,
                _configServiceMock.Object,
                _operationsMock.Object,
                _configuration.Object,
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

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // act & assert
            await Should.NotThrowAsync(() =>  new TwitterService(_loggerMock.Object, _configServiceMock.Object, _operationsMock.Object, _configuration.Object, _httpClient).Handle(Game.OVERWATCH));
            _operationsMock.Verify(x => x.PostTweetWithMedia($"Today's Overwatch Arcademodes - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade", It.IsAny<Media>()));
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

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // act & assert
            await Should.NotThrowAsync(() =>  new TwitterService(_loggerMock.Object, _configServiceMock.Object, _operationsMock.Object, _configuration.Object, _httpClient).Handle(Game.OVERWATCH));
            _operationsMock.Verify(x => x.PostTweetWithMedia($"Today's Overwatch Arcademodes, (Event: {serviceResponse.Data}) - {DateTime.Now:dddd, d MMMM} \n#overwatch #owarcade", It.IsAny<Media>()));
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

            _httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // act & assert
            await Should.ThrowAsync<HttpRequestException>(() =>  new TwitterService(_loggerMock.Object, _configServiceMock.Object, _operationsMock.Object, _configuration.Object, _httpClient).Handle(Game.OVERWATCH));
        }
    }
}