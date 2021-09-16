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
using OWArcadeBackend.Models;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Services.ConfigService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Services
{
    public class ConfigServiceTest
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<ConfigService>> _loggerMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly MemoryCache _memoryCache;

        public ConfigServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ConfigService>>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new ConfigService(
                null,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _loggerMock.Object,
                _memoryCache
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _unitOfWorkMock.Object,
                null,
                _webHostEnvironmentMock.Object,
                _loggerMock.Object,
                _memoryCache
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                null,
                _loggerMock.Object,
                _memoryCache
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                null,
                _memoryCache
            ));

            Should.Throw<ArgumentNullException>(() => new ConfigService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _webHostEnvironmentMock.Object,
                _loggerMock.Object,
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
            var serviceResponse = new ServiceResponse<IEnumerable<ConfigCountries>>
            {
                Data = new List<ConfigCountries>
                {
                    new()
                    {
                        Name = "The Netherlands",
                        Code = "NL"
                    } 
                } 
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.COUNTRIES.ToString())).ReturnsAsync(configCountries);

            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetCountries();

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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(x => x.Key == ConfigKeys.COUNTRIES.ToString())).ReturnsAsync(configCountries);

            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetCountries();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Message.ShouldBeEquivalentTo($"Config {ConfigKeys.COUNTRIES.ToString()} not found");
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
            IEnumerable<ConfigOverwatchHero> listConfigHeroes = new []
            {
                new ConfigOverwatchHero()
                {
                    Name = "Zenyatta",
                    Image = "image.jpg"
                }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<ConfigOverwatchHero>>
            {
                Data = listConfigHeroes.ToList()
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OW_HEROES.ToString())).ReturnsAsync(configHeroes);
            _mapperMock.Setup(x => x.Map<List<ConfigOverwatchHero>>(It.IsAny<IEnumerable<ConfigOverwatchHero>>())).Returns(listConfigHeroes.ToList());
            
            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetOverwatchHeroes();

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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OW_HEROES.ToString())).ReturnsAsync(configHeroes);


            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetOverwatchHeroes();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Message.ShouldBeEquivalentTo($"Config {ConfigKeys.OW_HEROES.ToString()} not found");
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
            IEnumerable<ConfigOverwatchMap> listConfigMaps = new []
            {
                new ConfigOverwatchMap()
                {
                    Name = "Ayutthaya",
                    Image = "image.jpg"
                }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<ConfigOverwatchMap>>
            {
                Data = listConfigMaps.ToList()
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OW_MAPS.ToString())).ReturnsAsync(configMaps);
            _mapperMock.Setup(x => x.Map<List<ConfigOverwatchMap>>(It.IsAny<IEnumerable<ConfigOverwatchMap>>())).Returns(listConfigMaps.ToList());
            
            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetOverwatchMaps();

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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OW_HEROES.ToString())).ReturnsAsync(configMaps);


            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetOverwatchMaps();

            // Assert
            result.StatusCode.ShouldBe(500);
            result.Message.ShouldBeEquivalentTo($"Config {ConfigKeys.OW_MAPS.ToString()} not found");
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
            _unitOfWorkMock.Setup(x => x.ConfigRepository.SingleOrDefaultASync(result => result.Key == ConfigKeys.OW_CURRENT_EVENT.ToString())).ReturnsAsync(configOverwatchEvent);

            // Act
            var result = await new ConfigService(_unitOfWorkMock.Object, _mapperMock.Object, _webHostEnvironmentMock.Object, _loggerMock.Object, _memoryCache).GetCurrentOverwatchEvent();

            // Assert
            result.Data.ShouldBeEquivalentTo(serviceResponse.Data);
        }
        
    }
}