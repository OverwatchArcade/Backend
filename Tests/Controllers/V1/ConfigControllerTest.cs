using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OWArcadeBackend.Controllers.V1.Contributor;
using OWArcadeBackend.Models;
using OWArcadeBackend.Services.ConfigService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Controllers.V1
{
    public class ConfigControllerTest
    {
        private Mock<IConfigService> _configServiceMock;

        public ConfigControllerTest()
        {
            _configServiceMock = new Mock<IConfigService>();
        }
        
        [Fact]
        public void TestConstructor()
        {
            var constructor = new ConfigController(_configServiceMock.Object);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ConfigController(
                null
            ));
        }
        
        private static void AssertActionResult<T>(ObjectResult result, ServiceResponse<T> expectedResponse)
        {
            result.ShouldNotBeNull();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<ServiceResponse<T>>();
            result.Value.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task TestGetCountries()
        {
            // Arrange
            var date = DateTime.Parse("03-20-1994");
            var serviceResponse = new ServiceResponse<IEnumerable<ConfigCountries>>()
            {
                Data = new[]
                {
                    new ConfigCountries()
                    {
                        Code = "NL",
                        Name = "The Netherlands"
                    }
                },
                Time = date
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<ConfigCountries>>()
            {
                Data = new[]
                {
                    new ConfigCountries()
                    {
                        Code = "NL",
                        Name = "The Netherlands"
                    }
                },
                Time = date
            };
            _configServiceMock.Setup(x => x.GetCountries()).ReturnsAsync(serviceResponse);
            
            // Act
            var actionResult = await new ConfigController(_configServiceMock.Object).GetCountries();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public async Task TestGetOverwatchHeroes()
        {
            // Arrange
            var date = DateTime.Parse("03-20-1994");
            var serviceResponse = new ServiceResponse<IEnumerable<ConfigOverwatchHero>>()
            {
                Data = new[]
                {
                    new ConfigOverwatchHero()
                    {
                        Name = "Ana"
                    }
                },
                Time = date
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<ConfigOverwatchHero>>()
            {
                Data = new[]
                {
                    new ConfigOverwatchHero()
                    {
                        Name = "Ana"
                    }
                },
                Time = date
            };
            _configServiceMock.Setup(x => x.GetOverwatchHeroes()).ReturnsAsync(serviceResponse);
            
            // Act
            var actionResult = await new ConfigController(_configServiceMock.Object).GetOverwatchHeroes();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public async Task TestGetOverwatchMaps()
        {
            // Arrange
            var date = DateTime.Parse("03-20-1994");
            var serviceResponse = new ServiceResponse<IEnumerable<ConfigOverwatchMap>>()
            {
                Data = new[]
                {
                    new ConfigOverwatchMap()
                    {
                        Name = "Ayutthaya"
                    }
                },
                Time = date
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<ConfigOverwatchMap>>()
            {
                Data = new[]
                {
                    new ConfigOverwatchMap()
                    {
                        Name = "Ayutthaya"
                    }
                },
                Time = date
            };
            _configServiceMock.Setup(x => x.GetOverwatchMaps()).ReturnsAsync(serviceResponse);
            
            // Act
            var actionResult = await new ConfigController(_configServiceMock.Object).GetOverwatchMaps();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
    }
}