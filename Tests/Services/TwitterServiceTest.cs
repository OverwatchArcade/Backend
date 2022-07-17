using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OverwatchArcade.Twitter.Factories;
using OverwatchArcade.Twitter.Services.ScreenshotService;
using OverwatchArcade.Twitter.Services.TwitterService;
using Shouldly;
using Tweetinvi;
using Tweetinvi.Parameters;
using Xunit;

namespace OverwatchArcade.Tests.Services;

public class TwitterServiceTest
{
    private IConfiguration _configuration;
    private readonly Mock<ILogger<TwitterService>> _loggerMock;
    private readonly Mock<IScreenshotService> _screenshotServiceMock;
    private readonly Mock<ITwitterClientFactory> _twitterClientFactoryMock;

    private readonly TwitterService _twitterService;
    private const string TwitterUsername = "TwitterUsername";

    public TwitterServiceTest()
    {
        _configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        _loggerMock = new Mock<ILogger<TwitterService>>();
        _screenshotServiceMock = new Mock<IScreenshotService>();
        _twitterClientFactoryMock = new Mock<ITwitterClientFactory>();

        _twitterService = new TwitterService(_configuration, _loggerMock.Object, _screenshotServiceMock.Object, _twitterClientFactoryMock.Object);
    }

    [Fact]
    public void Constructor()
    {
        var constructor = new TwitterService(_configuration, _loggerMock.Object, _screenshotServiceMock.Object, _twitterClientFactoryMock.Object);
        Assert.NotNull(constructor);
    }

    [Fact]
    public void ConstructorFunction_throws_Exception()
    {
        Should.Throw<ArgumentNullException>(() => new TwitterService(null, _loggerMock.Object, _screenshotServiceMock.Object, _twitterClientFactoryMock.Object));
        Should.Throw<ArgumentNullException>(() => new TwitterService(_configuration, null, _screenshotServiceMock.Object, _twitterClientFactoryMock.Object));
        Should.Throw<ArgumentNullException>(() => new TwitterService(_configuration, _loggerMock.Object, null, _twitterClientFactoryMock.Object));
        Should.Throw<ArgumentNullException>(() => new TwitterService(_configuration, _loggerMock.Object, _screenshotServiceMock.Object, null));
    }

    [Fact]
    public async Task PostTweetAction()
    {
        // Arrange
        var screenshotUrl = "https://overwatcharcade.local";
        var currentEvent = "Halloween";
        var twitterClient = new Mock<ITwitterClient>();
        var screenshot = Array.Empty<byte>();
        
        twitterClient.Setup(x => x.Upload.UploadTweetImageAsync(screenshot));
        twitterClient.Setup(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>()));
        _twitterClientFactoryMock.Setup(x => x.Create()).Returns(twitterClient.Object);
        _screenshotServiceMock.Setup(x => x.CreateScreenshot(screenshotUrl)).ReturnsAsync(screenshot);

        // Act
        await _twitterService.PostTweet(screenshotUrl, currentEvent);

        // Assert
        _twitterClientFactoryMock.Verify(x => x.Create(), Times.Once);
        _screenshotServiceMock.Verify(x => x.CreateScreenshot(screenshotUrl), Times.Once);
        twitterClient.Verify(x => x.Upload.UploadTweetImageAsync(screenshot), Times.Once);
        twitterClient.Verify(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTweet_NoTweetToBeDeleted()
    {
        // Arrange
        var twitterUsername = "owarcade";
        var twitterClient = new Mock<ITwitterClient>();
        var inMemorySettings = new Dictionary<string, string>
        {
            { TwitterUsername, twitterUsername },
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        twitterClient.Setup(x => x.Tweets.PublishTweetAsync(It.IsAny<IPublishTweetParameters>()));
        twitterClient.Setup(x => x.Timelines.GetUserTimelineAsync(It.IsAny<string>()));
        _twitterClientFactoryMock.Setup(x => x.Create()).Returns(twitterClient.Object);

        // Act
        var twitterService = new TwitterService(_configuration, _loggerMock.Object, _screenshotServiceMock.Object, _twitterClientFactoryMock.Object);
        await twitterService.DeleteLastTweet();

        // Assert
        twitterClient.Verify(x => x.Timelines.GetUserTimelineAsync(twitterUsername), Times.Once);
        _twitterClientFactoryMock.Verify(x => x.Create(), Times.Once);
    }
}