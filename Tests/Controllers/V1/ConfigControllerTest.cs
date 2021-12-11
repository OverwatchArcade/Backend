﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OWArcadeBackend.Controllers.V1.Contributor;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Controllers.V1
{
    public class ConfigControllerTest
    {
        private IMemoryCache _memoryCache;

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
            _memoryCache.Set(CacheKeys.OverwatchHeroes, serviceResponse);
            
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
            _memoryCache.Set(CacheKeys.OverwatchMaps, serviceResponse);
            
            // Act
            var actionResult = new ConfigController(_memoryCache).GetOverwatchMaps();
            
            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
        
    }
}