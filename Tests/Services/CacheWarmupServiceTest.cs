using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.CachingService;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class CacheWarmupServiceTest
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly Mock<ILogger<CacheWarmupService>> _logger;
        private readonly Mock<IOverwatchService> _overwatchService;

        public CacheWarmupServiceTest()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _configServiceMock = new Mock<IConfigService>();
            _logger = new Mock<ILogger<CacheWarmupService>>();
            _overwatchService = new Mock<IOverwatchService>();
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new CacheWarmupService(_memoryCache, _configServiceMock.Object, _logger.Object, _overwatchService.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new CacheWarmupService(
               null, _configServiceMock.Object, _logger.Object, _overwatchService.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new CacheWarmupService(
                _memoryCache, null, _logger.Object, _overwatchService.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new CacheWarmupService(
                _memoryCache, _configServiceMock.Object, null, _overwatchService.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new CacheWarmupService(
                _memoryCache, _configServiceMock.Object, _logger.Object, null
            ));
        }

        [Fact]
        public async Task Run()
        {
            // Arrange
            var countries = new ServiceResponse<IEnumerable<Country>>
            {
                Data = new[]
                {
                    new Country()
                    {
                        Name = "The Netherlands",
                        Code = "NL"
                    }
                }
            };
            var owHeroes = new ServiceResponse<IEnumerable<HeroPortrait>>
            {
                Data = new[]
                {
                    new HeroPortrait()
                    {
                        Name = "Ana"
                    }
                }
            };
            var owMaps = new ServiceResponse<IEnumerable<MapPortrait>>
            {
                Data = new[]
                {
                    new MapPortrait()
                    {
                        Name = "Ayuthaya"
                    }
                }
            };
            var owEvent = new ServiceResponse<string>
            {
                Data = "Default"
            };
            var owEvents = new ServiceResponse<string[]>
            {
                Data = new[]
                {
                    "Default",
                    "Summergames"
                }
            };
            var owDaily = new ServiceResponse<DailyDto>
            {
                Data = new DailyDto
                {
                    IsToday = false,
                    Modes = null,
                    CreatedAt = default,
                    Contributor = null
                }
            };
            var owLabels = new ServiceResponse<List<Label>>()
            {
                Data = new List<Label>()
                {
                    new()
                    {
                        Value = "Daily"
                    }
                }
            };
            var owArcadeModes = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto>()
                {
                    new()
                    {
                        Id = 0,
                        Name = null,
                        Players = null,
                        Description = null,
                        Image = null
                    }
                }
            };
            _configServiceMock.Setup(x => x.GetCountries()).ReturnsAsync(countries);
            _configServiceMock.Setup(x => x.GetOverwatchHeroes()).ReturnsAsync(owHeroes);
            _configServiceMock.Setup(x => x.GetOverwatchMaps()).ReturnsAsync(owMaps);
            _configServiceMock.Setup(x => x.GetCurrentOverwatchEvent()).ReturnsAsync(owEvent);
            _configServiceMock.Setup(x => x.GetOverwatchEvents()).Returns(owEvents);
            
            _overwatchService.Setup(x => x.GetDaily()).Returns(owDaily);
            _overwatchService.Setup(x => x.GetLabels()).Returns(owLabels);
            _overwatchService.Setup(x => x.GetArcadeModes()).Returns(owArcadeModes);

            // Act
            await new CacheWarmupService(_memoryCache, _configServiceMock.Object, _logger.Object, _overwatchService.Object).Run();
            
            // Assert
            _configServiceMock.Verify(x => x.GetCountries());
            _configServiceMock.Verify(x => x.GetOverwatchHeroes());
            _configServiceMock.Verify(x => x.GetOverwatchMaps());
            _configServiceMock.Verify(x => x.GetCurrentOverwatchEvent());
            _configServiceMock.Verify(x => x.GetOverwatchEvents());
            
            _overwatchService.Verify(x => x.GetDaily());
            _overwatchService.Verify(x => x.GetLabels());
            _overwatchService.Verify(x => x.GetArcadeModes());
        }
    }
}