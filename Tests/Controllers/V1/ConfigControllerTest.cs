using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Controllers.V1.Contributor;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Controllers.V1
{
    public class ConfigControllerTest
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConfigController _configController;

        public ConfigControllerTest()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _configController = new ConfigController(_memoryCache);
        }
        
        [Fact]
        public void Constructor()
        {
            var constructor = new ConfigController(_memoryCache);
            Assert.NotNull(constructor);
        }
        
        [Fact]
        public void ConstructorFunction_throws_Exception()
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
        public void GetCountries()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<IEnumerable<Country>>
            {
                Data = new[]
                {
                    new Country
                    {
                        Code = "NL",
                        Name = "The Netherlands"
                    }
                }
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<Country>>
            {
                Data = new[]
                {
                    new Country
                    {
                        Code = "NL",
                        Name = "The Netherlands"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.Countries, serviceResponse);
            
            // Act
            var actionResult = _configController.GetCountries();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public void GetOverwatchHeroes()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<IEnumerable<HeroPortrait>>()
            {
                Data = new[]
                {
                    new HeroPortrait()
                    {
                        Name = "Ana"
                    }
                }
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<HeroPortrait>>()
            {
                Data = new[]
                {
                    new HeroPortrait()
                    {
                        Name = "Ana"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.ConfigOverwatchHeroes, serviceResponse);
            
            // Act
            var actionResult = _configController.GetOverwatchHeroes();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public void GetOverwatchMaps()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<IEnumerable<MapPortrait>>
            {
                Data = new[]
                {
                    new MapPortrait
                    {
                        Name = "Ayutthaya"
                    }
                }
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<MapPortrait>>
            {
                Data = new[]
                {
                    new MapPortrait
                    {
                        Name = "Ayutthaya"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.ConfigOverwatchMaps, serviceResponse);
            
            // Act
            var actionResult = _configController.GetOverwatchMaps();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetOverwatchArcadeModes()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<IEnumerable<ArcadeMode>>
            {
                Data = new[]
                {
                    new ArcadeMode
                    {
                        Name = "Total Mayhem"
                    }
                }
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<ArcadeMode>>
            {
                Data = new[]
                {
                    new ArcadeMode
                    {
                        Name = "Total Mayhem"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.ConfigOverwatchArcadeModes, serviceResponse);
            
            // Act
            var actionResult = _configController.GetOverwatchArcadeModes();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

    }
}