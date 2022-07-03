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
using OverwatchArcade.Domain.Models;
using OverwatchArcade.Domain.Models.ContributorInformation.Game.Overwatch.Portraits;
using OverwatchArcade.Domain.Models.ContributorInformation.Personal;
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

        private ConfigService _configService;

        public ConfigServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ConfigService>>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _configService = new ConfigService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _webHostEnvironmentMock.Object);
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new ConfigService(_loggerMock.Object, _unitOfWorkMock.Object,  _memoryCache, _webHostEnvironmentMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                null,
                _unitOfWorkMock.Object,
                _memoryCache,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                null,
                _memoryCache,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                null,
                _webHostEnvironmentMock.Object
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _memoryCache,
                null
            ));
        }

        [Fact]
        public async Task TestGetCountries_Returns_ListOfCountries()
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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(y => y.Key == ConfigKeys.Countries.ToString())).ReturnsAsync(configCountries);

            // Act
            var result = await _configService.GetCountries();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
        /// <summary>
        /// Returns an error on serviceResponse when no Config is found
        /// </summary>
        [Fact]
        public async Task TestGetCountries_Returns_Error()
        {
            // Arrange
            Config configCountries = new Config
            {
                Id = 1,
                JsonValue = null
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(y => y.Key == ConfigKeys.Countries.ToString())).ReturnsAsync(configCountries);

            // Act
            var result = await _configService.GetCountries();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public async Task TestGetOverwatchHeroes_Returns_ListOfHeroes()
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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OwHeroes.ToString())).ReturnsAsync(configHeroes);
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
        public async Task TestGetHeroes_Returns_Error()
        {
            // Arrange
            Config configHeroes = new Config
            {
                Id = 1,
                JsonValue = null
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OwHeroes.ToString())).ReturnsAsync(configHeroes);


            // Act
            var result = await _configService.GetOverwatchHeroes();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public async Task TestGetOverwatchMaps_Returns_ListOfOverwatchMaps()
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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OwMaps.ToString())).ReturnsAsync(configMaps);
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
        public async Task TestGetOverwatchMaps_Returns_Error()
        {
            // Arrange
            Config configMaps = new Config
            {
                Id = 1,
                JsonValue = null
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OwHeroes.ToString())).ReturnsAsync(configMaps);


            // Act
            var result = await _configService.GetOverwatchMaps();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Data.ShouldBeNull();
        }
        
        [Fact]
        public async Task TestGetCurrentOverwatchEvent_Returns_OverwatchEvent()
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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OwCurrentEvent.ToString())).ReturnsAsync(configOverwatchEvent);

            // Act
            var result = await _configService.GetCurrentOverwatchEvent();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
    }
}