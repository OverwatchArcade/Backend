using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OverwatchArcade.API.Dtos.Discord;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models.Constants;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Utility;

public class DiscordApiClientTest
{
    private readonly Mock<ILogger<DiscordApiClient>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    private readonly DiscordApiClient _discordApiClient;

    private IConfigurationRoot _configuration;
    private const string DiscordBearerToken = "12345";
    private const string DiscordRedirectUri = "owarcade.local/redirect";
    public DiscordApiClientTest()
    {
        _loggerMock = new Mock<ILogger<DiscordApiClient>>();
        _configurationMock = new Mock<IConfiguration>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _discordApiClient = new DiscordApiClient(_loggerMock.Object, _configurationMock.Object, _httpClientFactoryMock.Object);
    }

    [Fact]
    public void Constructor()
    {
        var constructor = new AuthenticationToken(_configurationMock.Object);
        Assert.NotNull(constructor);
    }
        
    [Fact]
    public void ConstructorFunction_throws_Exception()
    {
        Should.Throw<ArgumentNullException>(() => new AuthenticationToken(
            null
        ));
    }

    [Fact]
    public async Task GetDiscordToken_Returns_Token()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(DiscordConstants.DiscordTokenUrl)
            .Respond("application/json", "{\"access_token\":\"12345\"}");
        var client = mockHttp.ToHttpClient();
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())) 
            .Returns(client).Verifiable();
        
        var inMemorySettings = new Dictionary<string, string>
        {
            { "client_id", "12345" },
            { "client_secret", "1111"},
            { "redirect_uri", DiscordRedirectUri},
            { "grant_type", "authorization_code"},
            {"code", DiscordBearerToken}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var discordApiClient = new DiscordApiClient(_loggerMock.Object, _configuration, _httpClientFactoryMock.Object);
        var result = await discordApiClient.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri);
        
        // Assert
        result.ShouldNotBeNull();
        result.AccessToken.ShouldBe("12345");
    }
    
    [Fact]
    public async Task GetDiscordToken_Invalid_ThrowsException()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(DiscordConstants.DiscordTokenUrl)
            .Respond(HttpStatusCode.ServiceUnavailable);
        var client = mockHttp.ToHttpClient();
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())) 
            .Returns(client).Verifiable();
        
        var inMemorySettings = new Dictionary<string, string>
        {
            { "client_id", "12345" },
            { "client_secret", "1111"},
            { "redirect_uri", DiscordRedirectUri},
            { "grant_type", "authorization_code"},
            {"code", DiscordBearerToken}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act & Assert
        var discordApiClient = new DiscordApiClient(_loggerMock.Object, _configuration, _httpClientFactoryMock.Object);
        await Should.ThrowAsync<HttpRequestException>(async () => await discordApiClient.GetDiscordToken(DiscordBearerToken, DiscordRedirectUri));
    }
    
    [Fact]
    public async Task MakeDiscordOAuthCall_Returns_Token()
    {
        // Arrange
        var discordToken = "123";
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(DiscordConstants.UserInfoUrl)
            .Respond("application/json", "{\"username\":\"system\"}");
        var client = mockHttp.ToHttpClient();
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())) 
            .Returns(client).Verifiable();

        // Act
        var result = await _discordApiClient.MakeDiscordOAuthCall(discordToken);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<DiscordLoginDto>();
    }
    
    [Fact]
    public async Task MakeDiscordOAuthCall_Invalid_ThrowsException()
    {
        // Arrange
        var discordToken = "123";
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(DiscordConstants.UserInfoUrl)
            .Respond(HttpStatusCode.ServiceUnavailable);
        var client = mockHttp.ToHttpClient();
        _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())) 
            .Returns(client).Verifiable();

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await _discordApiClient.MakeDiscordOAuthCall(discordToken));
    }
}