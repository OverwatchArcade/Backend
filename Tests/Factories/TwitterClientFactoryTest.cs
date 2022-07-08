using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using OverwatchArcade.Twitter.Factories;
using Shouldly;
using Tweetinvi;
using Xunit;

namespace OverwatchArcade.Tests.Factories;

public class TwitterClientFactoryTest
{
    private readonly Mock<IConfiguration> _configurationMock;

    private const string TwitterConsumerKey = "Twitter:ConsumerKey";
    private const string TwitterConsumerSecret = "Twitter:ConsumerSecret";
    private const string TwitterTokenValue = "Twitter:TokenValue";
    private const string TwitterTokenSecret = "Twitter:TokenSecret";

    public TwitterClientFactoryTest()
    {
        _configurationMock = new Mock<IConfiguration>();
    }

    [Fact]
    public void Constructor()
    {
        var constructor = new TwitterClientFactory(_configurationMock.Object);
        Assert.NotNull(constructor);
    }

    [Fact]
    public void ConstructorFunction_throws_Exception()
    {
        Should.Throw<ArgumentNullException>(() => (new TwitterClientFactory(null)));
    }

    [Fact]
    public void Create_CreatesClient()
    {
        // Arrange
        // Doing it this way takes a lot of less LoC than mocking
        var inMemorySettings = new Dictionary<string, string> {
            {TwitterConsumerKey, "111"},
            {TwitterConsumerSecret, "222"},
            {TwitterTokenValue, "333"},
            {TwitterTokenSecret, "444"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var result = new TwitterClientFactory(configuration).Create();

        // Assert
        result.ShouldBeOfType<TwitterClient>();
    }
    
    [InlineData(null, "222", "333", "444")]
    [InlineData("111", null, "333", "444")]
    [InlineData("111", "222", null, "444")]
    [InlineData("111", "222", "333", null)]
    [Theory]
    public void Create_InvalidConfiguration_Throws_ArgumentNullException(string consumerKey, string consumerSecret, string tokenValue, string tokenSecret)
    {
        // Arrange
        // Doing it this way takes a lot of less LoC than mocking
        var inMemorySettings = new Dictionary<string, string> {
            {TwitterConsumerKey, consumerKey},
            {TwitterConsumerSecret, consumerSecret},
            {TwitterTokenValue, tokenValue},
            {TwitterTokenSecret, tokenSecret}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new TwitterClientFactory(configuration).Create());
    }
}