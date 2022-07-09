using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Factories;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Factories;

public class ServiceResponseFactoryTest
{
    [Fact]
    public void Create()
    {
       // Arrange
       var dailyDto = new DailyDto();
       var serviceResponseFactory = new ServiceResponseFactory<DailyDto>();

       // Act
       var result = serviceResponseFactory.Create(dailyDto);
       
       // Assert
       result.Data.ShouldBeOfType<DailyDto>();
       result.Success.ShouldBeTrue();
    }
    
    [Fact]
    public void Error_SingleErrorMessage()
    {
        // Arrange
        var serviceResponseFactory = new ServiceResponseFactory<DailyDto>();
        var statusCode = 500;
        var errorMessage = "Unit test failed!";
        var amountOfErrorMessages = 1;

        // Act
        var result = serviceResponseFactory.Error(statusCode, errorMessage);
       
        // Assert
        result.Data.ShouldBeNull();
        result.Success.ShouldBeFalse();
        result.StatusCode.ShouldBe(statusCode);
        Assert.Equal(amountOfErrorMessages, result.ErrorMessages.Length);
        result.ErrorMessages[0].ShouldBeEquivalentTo(errorMessage);
    }
    
    [Fact]
    public void Error_MultipleErrorMessage()
    {
        // Arrange
        var serviceResponseFactory = new ServiceResponseFactory<DailyDto>();
        var statusCode = 500;
        var errorMessages =  new[] { "Unit test failed!", "And another one"};
        var amountOfErrorMessages = 2;

        // Act
        var result = serviceResponseFactory.Error(statusCode, errorMessages);
       
        // Assert
        result.Data.ShouldBeNull();
        result.Success.ShouldBeFalse();
        result.StatusCode.ShouldBe(statusCode);
        result.ErrorMessages.ShouldBeEquivalentTo(errorMessages);
        Assert.Equal(amountOfErrorMessages, result.ErrorMessages.Length);
    }
}