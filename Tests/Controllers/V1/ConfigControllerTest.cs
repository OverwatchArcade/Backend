using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OverwatchArcade.API.Controllers.V1.Contributor;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Controllers.V1
{
    public class ConfigControllerTest
    {
        private readonly IMemoryCache _memoryCache;

        public ConfigControllerTest()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
        
        [Fact]
        public void TestConstructor()
        {
            var constructor = new ConfigController(_memoryCache);
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
        public void TestGetCountries()
        {
            // Arrange
            var date = DateTime.Parse("03-20-1994");
            var serviceResponse = new ServiceResponse<IEnumerable<Country>>()
            {
                Data = new[]
                {
                    new Country()
                    {
                        Code = "NL",
                        Name = "The Netherlands"
                    }
                }
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<Country>>()
            {
                Data = new[]
                {
                    new Country()
                    {
                        Code = "NL",
                        Name = "The Netherlands"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.Countries, serviceResponse);
            
            // Act
            var actionResult = new ConfigController(_memoryCache).GetCountries();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public void TestGetOverwatchHeroes()
        {
            // Arrange
            var date = DateTime.Parse("03-20-1994");
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
            var actionResult = new ConfigController(_memoryCache).GetOverwatchHeroes();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
        [Fact]
        public void TestGetOverwatchMaps()
        {
            // Arrange
            var date = DateTime.Parse("03-20-1994");
            var serviceResponse = new ServiceResponse<IEnumerable<MapPortrait>>()
            {
                Data = new[]
                {
                    new MapPortrait()
                    {
                        Name = "Ayutthaya"
                    }
                }
            };
            
            var expectedResponse = new ServiceResponse<IEnumerable<MapPortrait>>()
            {
                Data = new[]
                {
                    new MapPortrait()
                    {
                        Name = "Ayutthaya"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.ConfigOverwatchMaps, serviceResponse);
            
            // Act
            var actionResult = new ConfigController(_memoryCache).GetOverwatchMaps();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
    }
}