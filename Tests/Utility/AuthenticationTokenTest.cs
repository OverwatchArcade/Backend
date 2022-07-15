using System;
using Microsoft.Extensions.Configuration;
using Moq;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.Constants;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Utility;

public class AuthenticationTokenTest
{
    private readonly Mock<IConfiguration> _configurationMock;
    
    private readonly AuthenticationToken _authenticationToken;

    public AuthenticationTokenTest()
    {
        _configurationMock = new Mock<IConfiguration>();
        
        _authenticationToken = new AuthenticationToken(_configurationMock.Object);
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
    public void CreateJwtToken_Creates_Token()
    {
        // Arrange
        var contributor = new Contributor()
        {
            Id = new Guid("7691AE27-ACE3-44FA-9BF0-683CC07E075E"),
            Username = "System",
            Group = ContributorGroup.Contributor
        };
        var configurationSectionMock = new Mock<IConfigurationSection>();
        configurationSectionMock.Setup(x => x.Value).Returns("gpJUr1uZEgwtzIeSChrOX8FM3DCckd80");
        _configurationMock.Setup(x => x.GetSection("Jwt:Token")).Returns(configurationSectionMock.Object);
        
        // Act
        var result = _authenticationToken.CreateJwtToken(contributor);

        // Assert
        result.ShouldStartWith("ey");
    }
}