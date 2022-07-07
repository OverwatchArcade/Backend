using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using OverwatchArcade.API.Controllers.V1;
using OverwatchArcade.API.Dtos;
using OverwatchArcade.API.Dtos.Contributor;
using OverwatchArcade.API.Dtos.Overwatch;
using OverwatchArcade.API.Services.ConfigService;
using OverwatchArcade.API.Services.OverwatchService;
using OverwatchArcade.Domain.Models.Constants;
using OverwatchArcade.Domain.Models.Overwatch;
using Shouldly;
using Xunit;

namespace OverwatchArcade.Tests.Controllers.V1
{
    public class OverwatchControllerTest
    {
        private readonly Mock<IOverwatchService> _overwatchServiceMock;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly MemoryCache _memoryCache;

        private Guid _userId;
        private ClaimsPrincipal _claimsPrincipalUser;
        private OverwatchController _overwatchController;

        public OverwatchControllerTest()
        {
            _overwatchServiceMock = new Mock<IOverwatchService>();
            _configServiceMock = new Mock<IConfigService>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            PrepareMock();
        }

        private void PrepareMock()
        {
            _userId = new Guid("78B994EC-9AD4-41B6-B059-761C1887DE0F");
            _claimsPrincipalUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
            }));

            _overwatchController = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = _claimsPrincipalUser }
                }
            };
        }

        private static void AssertActionResult<T>(ObjectResult result, ServiceResponse<T> expectedResponse)
        {
            result.ShouldNotBeNull();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<ServiceResponse<T>>();

            var serviceResponseResult = (ServiceResponse<T>)result.Value;
            serviceResponseResult.Data.ShouldBeEquivalentTo(expectedResponse.Data);
        }

        [Fact]
        public void _Constructor()
        {
            var constructor = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void _ConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new OverwatchController(
                null,
                _configServiceMock.Object,
                _memoryCache
            ));

            Should.Throw<ArgumentNullException>(() => new OverwatchController(
                _overwatchServiceMock.Object,
                null,
                _memoryCache
            ));

            Should.Throw<ArgumentNullException>(() => new OverwatchController(
                _overwatchServiceMock.Object,
                _configServiceMock.Object,
                null
            ));
        }

        [Fact]
        public async Task PostOverwatchDaily_Submits_Daily()
        {
            // Arrange
            var createDailyDto = new CreateDailyDto()
            {
                TileModes = new List<CreateTileModeDto>()
                {
                    new()
                    {
                        LabelId = 1,
                        TileId = 1,
                        ArcadeModeId = 1
                    }
                }
            };
            var dailyDto = new DailyDto
            {
                Modes = new List<TileModeDto>()
                {
                    new()
                    {
                        Name = "Total Mayhem",
                        Description = "Everybody likes mayhem, right?",
                        Players = "6v6",
                        Image = "image.jpg",
                        Label = "Daily"
                    }
                },
                CreatedAt = DateTime.Parse("03-20-2021"),
                Contributor = new ContributorDto(),
                IsToday = true
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };

            _overwatchServiceMock.Setup(x => x.Submit(createDailyDto, _userId)).ReturnsAsync(serviceResponse);

            // Act
            var actionResult = await _overwatchController.PostOverwatchDaily(createDailyDto);

            // Assert
            _overwatchServiceMock.Verify(x => x.Submit(createDailyDto, _userId));
            var result = actionResult.Result as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task UndoOverwatchDaily_Undoes_Daily()
        {
            // Arrange
            var dailyDto = new DailyDto
            {
                IsToday = false
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };

            _overwatchServiceMock.Setup(x => x.Undo(_userId, true)).ReturnsAsync(serviceResponse);

            // Act
            var actionResult = await _overwatchController.UndoOverwatchDaily(true);

            // Assert
            _overwatchServiceMock.Verify(x => x.Undo(_userId, true));
            var result = actionResult.Result as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetDaily_HasNoCache()
        {
            // Arrange
            var dailyDto = new DailyDto
            {
                IsToday = true
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };

            _overwatchServiceMock.Setup(x => x.GetDaily()).Returns(serviceResponse);

            // Act
            var actionResult = _overwatchController.GetDaily();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetDaily());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetDaily_Returns_Daily()
        {
            // Arrange
            var dailyDto = new DailyDto
            {
                IsToday = true
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto
            };
            _memoryCache.Set(CacheKeys.OverwatchDaily, serviceResponse);

            // Act
            var actionResult = _overwatchController.GetDaily();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetDaily(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetEvent_Returns_Event()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Default"
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Default"
            };
            
            _memoryCache.Set(CacheKeys.ConfigOverwatchEvent, serviceResponse);

            var httpContext = new DefaultHttpContext();

            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            var actionResult = controller.GetEvent();

            // Assert
            _configServiceMock.Verify(x => x.GetCurrentOverwatchEvent(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task PostEvent()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Default"
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Default"
            };
            _configServiceMock.Setup(x => x.PostOverwatchEvent("Default")).ReturnsAsync(serviceResponse);

            // Act
            var actionResult = await new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache).PostEvent("Default");

            // Assert
            _configServiceMock.Verify(x => x.PostOverwatchEvent("Default"));
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task GetEventWallpaperUrl_Returns_WallpaperUrl()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Wallpaper_URL"
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Wallpaper_URL"
            };

            _configServiceMock.Setup(x => x.GetOverwatchEventWallpaper()).ReturnsAsync(serviceResponse);

            // Act
            var actionResult = await new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache).GetEventWallpaperUrl();

            // Assert
            _configServiceMock.Verify(x => x.GetOverwatchEventWallpaper());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetEvents()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<string[]>
            {
                Data = new[] { "Event_A", "Event_B" }
            };
            var expectedResponse = new ServiceResponse<string[]>
            {
                Data = new[] { "Event_A", "Event_B" }
            };

            _memoryCache.Set(CacheKeys.ConfigOverwatchEvents, serviceResponse);
            var httpContext = new DefaultHttpContext();

            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            var actionResult = controller.GetEvents();

            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetArcadeModes()
        {
            // Arrange
            var gamemodeDto = new ArcadeModeDto()
            {
                Name = "Total Mayhem",
                Players = "6v6"
            };
            var serviceResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto> { gamemodeDto }
            };
            var expectedResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto> { gamemodeDto }
            };
            _memoryCache.Set(CacheKeys.OverwatchArcadeModes, serviceResponse);

            // Act


            var actionResult = _overwatchController.GetArcadeModes();

            // Assert
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void GetLabels()
        {
            // Arrange
            var serviceResponse = new ServiceResponse<List<Label>>
            {
                Data = new List<Label>
                {
                    new()
                    {
                        Id = 1,
                        Value = "Daily"
                    }
                }
            };
            var expectedResponse = new ServiceResponse<List<Label>>
            {
                Data = new List<Label>
                {
                    new()
                    {
                        Id = 1,
                        Value = "Daily"
                    }
                }
            };
            _memoryCache.Set(CacheKeys.OverwatchLabels, serviceResponse);

            // Act
            var actionResult = _overwatchController.GetLabels();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetLabels(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
    }
}