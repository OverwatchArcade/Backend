using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Utility;
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
using OverwatchArcade.Domain.Models.Overwatch;
using OverwatchArcade.Persistence;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Services
{
    public class ConfigServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ConfigService>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<IFileProvider> _fileProviderMock;

        private readonly ConfigService _configService;

        public ConfigServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ConfigService>>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _fileProviderMock = new Mock<IFileProvider>();

            _configService = new ConfigService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _webHostEnvironmentMock.Object, _fileProviderMock.Object);
        }

        [Fact]
        public void Constructor()
        {
            var constructor = new ConfigService(_loggerMock.Object, _unitOfWorkMock.Object,  _memoryCache, _webHostEnvironmentMock.Object, _fileProviderMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                null,
                _unitOfWorkMock.Object,
                _memoryCache,
                _webHostEnvironmentMock.Object,
                _fileProviderMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                null,
                _memoryCache,
                _webHostEnvironmentMock.Object,
                _fileProviderMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                null,
                _webHostEnvironmentMock.Object,
                _fileProviderMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _memoryCache,
                null,
                _fileProviderMock.Object
            ));
            
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _memoryCache,
                _webHostEnvironmentMock.Object,
                null
            ));
        }

        [Fact]
        public async Task GetCountries_Returns_ListOfCountries()
        {
            // Arrange
            Config configCountries = new Config
            {
                Id = 1,
                JsonValue = JArray.Parse("[{\"Name\":\"The Netherlands\",\"Code\":\"NL\"}]")
            };
            var serviceResponse = new ServiceResponse<IEnumerable<Country>>
            {
                Data = new List<Country>
                {
                    new()
                    {
                        Name = "The Netherlands",
                        Code = "NL"
                    } 
                } 
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(y => y.Key == ConfigKeys.Countries.ToString())).ReturnsAsync(configCountries);

            // Act
            var result = await _configService.GetCountries();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
        /// <summary>
        /// Returns an error on serviceResponse when no Config is found
        /// </summary>
        [Fact]
        public async Task GetCountries_Returns_Error()
        {
            // Arrange
            Config configCountries = new Config
            {
                Id = 1,
                JsonValue = null
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(y => y.Key == ConfigKeys.Countries.ToString())).ReturnsAsync(configCountries);

            // Act
            var result = await _configService.GetCountries();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public async Task GetOverwatchHeroes_Returns_ListOfHeroes()
        {
            // Arrange
            Config configHeroes = new Config
            {
                Id = 1,
                JsonValue = JArray.Parse("[{\"Name\":\"Zenyatta\",\"Image\":\"image.jpg\"}]")
            };
            IEnumerable<HeroPortrait> listConfigHeroes = new []
            {
                new HeroPortrait()
                {
                    Name = "Zenyatta",
                    Image = "image.jpg"
                }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<HeroPortrait>>
            {
                Data = listConfigHeroes.ToList()
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(result => result.Key == ConfigKeys.OwHeroes.ToString())).ReturnsAsync(configHeroes);
            _mapperMock.Setup(x => x.Map<List<HeroPortrait>>(It.IsAny<IEnumerable<HeroPortrait>>())).Returns(listConfigHeroes.ToList());
            
            // Act
            var result = await _configService.GetOverwatchHeroes();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
        /// <summary>
        /// Returns an error on serviceResponse when no Config is found
        /// </summary>
        [Fact]
        public async Task GetHeroes_Returns_Error()
        {
            // Arrange
            Config configHeroes = new Config
            {
                Id = 1,
                JsonValue = null
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(result => result.Key == ConfigKeys.OwHeroes.ToString())).ReturnsAsync(configHeroes);


            // Act
            var result = await _configService.GetOverwatchHeroes();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public async Task GetOverwatchMaps_Returns_ListOfOverwatchMaps()
        {
            // Arrange
            Config configMaps = new Config
            {
                Id = 1,
                JsonValue = JArray.Parse("[{\"Name\":\"Ayutthaya\",\"Image\":\"image.jpg\"}]")
            };
            IEnumerable<MapPortrait> listConfigMaps = new []
            {
                new MapPortrait()
                {
                    Name = "Ayutthaya",
                    Image = "image.jpg"
                }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<MapPortrait>>
            {
                Data = listConfigMaps.ToList()
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(result => result.Key == ConfigKeys.OwMaps.ToString())).ReturnsAsync(configMaps);
            _mapperMock.Setup(x => x.Map<List<MapPortrait>>(It.IsAny<IEnumerable<MapPortrait>>())).Returns(listConfigMaps.ToList());
            
            // Act
            var result = await _configService.GetOverwatchMaps();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
        /// <summary>
        /// Returns an error on serviceResponse when no Config is found
        /// </summary>
        [Fact]
        public async Task GetOverwatchMaps_Returns_Error()
        {
            // Arrange
            Config configMaps = new Config
            {
                Id = 1,
                JsonValue = null
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(result => result.Key == ConfigKeys.OwHeroes.ToString())).ReturnsAsync(configMaps);


            // Act
            var result = await _configService.GetOverwatchMaps();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public async Task GetCurrentOverwatchEvent_Returns_OverwatchEvent()
        {
            // Arrange
            Config configOverwatchEvent = new Config
            {
                Id = 1,
                Value = "Default"
            };
            var serviceResponse = new ServiceResponse<string>
            {
                Data = configOverwatchEvent.Value
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(result => result.Key == ConfigKeys.OwCurrentEvent.ToString())).ReturnsAsync(configOverwatchEvent);

            // Act
            var result = await _configService.GetCurrentOverwatchEvent();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
        [Fact]
        public void GetArcadeModes_Returns_ArcadeModes()
        {
            // Arrange
            var arcadeModes = new List<ArcadeMode>()
            {
                new()
                {
                    Name = "Total Mayhem"
                }
            };
            
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.GetArcadeModes()).Returns(arcadeModes);

            // Act
            var result = _configService.GetArcadeModes();

            // Assert
            result.Data.ShouldBeOfType<List<ArcadeMode>>();
            Assert.Single(result.Data);
            Assert.Equal("Total Mayhem", result.Data.First().Name);
        }
        
        [Fact]
        public async Task GetOverwatchEventWallpaper_Returns_WallpaperUrl()
        {
            // Arrange
            var configOverwatchEvent = new Config
            {
                Value = "Default"
            };
            var wallpapers = new[]
            {
                "wallpaper-1",
                "wallpaper-2"
            };

            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(result => result.Key == ConfigKeys.OwCurrentEvent.ToString())).ReturnsAsync(configOverwatchEvent);
            _fileProviderMock.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(wallpapers);
            // Act
            var result = await _configService.GetOverwatchEventWallpaper();

            // Assert
            result.Data.ShouldBeOfType<string>();
            result.Data.ShouldStartWith("/images/overwatch/events/");
        }
        
        [Fact]
        public async Task GetOverwatchEventWallpaper_Returns_Error()
        {
            // Arrange
            _fileProviderMock.Setup(x => x.GetFiles(It.IsAny<string>())).Throws<Exception>();
            
            // Act
            var result = await _configService.GetOverwatchEventWallpaper();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public void GetOverwatchEvents_Returns_Events()
        {
            // Arrange
            var events = new[]
            {
                "default",
                "halloween"
            };
            
            _fileProviderMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(events);
            
            // Act
            var result = _configService.GetOverwatchEvents();

            // Assert
            result.Data.ShouldBeOfType<string[]>();
            result.Data.ShouldBeEquivalentTo(new[]{"default", "halloween"});
        }

        [Fact]
        public async Task PostOverwatchEvent_Returns_Error_UnknownEvent()
        {
            // Arrange
            var unknownEvent = "unknown";
            var events = new[]
            {
                "default",
                "halloween"
            };

            _fileProviderMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(events);

            // Act
            var result = await _configService.PostOverwatchEvent(unknownEvent);

            // Assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
        }
        
        [Fact]
        public async Task PostOverwatchEvent_Returns_Error_NoConfigFound()
        {
            // Arrange
            var newEvent = "halloween";
            var events = new[]
            {
                "default",
                "halloween"
            };

            _fileProviderMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(events);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(y => y.Key == ConfigKeys.OwCurrentEvent.ToString())).ReturnsAsync(new Config());

            // Act
            var result = await _configService.PostOverwatchEvent(newEvent);
            
            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessages.First().ShouldBe($"Config {ConfigKeys.OwCurrentEvent.ToString()} not found");
            result.StatusCode.ShouldBe(500);
        }
        
        [Fact]
        public async Task PostOverwatchEvent_Returns_SavedEvent()
        {
            // Arrange
            var newEvent = "halloween";
            var events = new[]
            {
                "default",
                "halloween"
            };
            var currentConfig = new Config()
            {
                Key = ConfigKeys.OwCurrentEvent.ToString(),
                Value = "default"
            };

            _fileProviderMock.Setup(x => x.GetDirectories(It.IsAny<string>())).Returns(events);
            _unitOfWorkMock.Setup(x => x.ConfigRepository.FirstOrDefaultASync(y => y.Key == ConfigKeys.OwCurrentEvent.ToString())).ReturnsAsync(currentConfig);

            // Act
            var result = await _configService.PostOverwatchEvent(newEvent);
            
            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeOfType<string>();
            result.Data.ShouldBe("halloween");
        }
    }
}