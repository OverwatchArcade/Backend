using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using OWArcadeBackend.Controllers.V1;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Services.ConfigService;
using OWArcadeBackend.Services.OverwatchService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Controllers.V1
{
    public class OverwatchControllerTest
    {
        private readonly Mock<IOverwatchService> _overwatchServiceMock;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly MemoryCache _memoryCache;

        private Guid _userId;
        private ClaimsPrincipal _claimsPrincipalUser;

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
        }

        private static void AssertActionResult<T>(ObjectResult result, ServiceResponse<T> expectedResponse)
        {
            result.ShouldNotBeNull();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<ServiceResponse<T>>();
            result.Value.ShouldBeEquivalentTo(expectedResponse);
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
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
        public async Task TestPostOverwatchDaily_SubmitDaily()
        {
            // Arrange
            var daily = new Daily();
            var date = DateTime.Parse("03-20-2021");
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
                Data = dailyDto,
                Time = date
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };

            _overwatchServiceMock.Setup(x => x.Submit(daily, Game.OVERWATCH, _userId)).ReturnsAsync(serviceResponse);

            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = _claimsPrincipalUser}
                }
            };

            var actionResult = await controller.PostOverwatchDaily(daily);

            // Assert
            _overwatchServiceMock.Verify(x => x.Submit(daily, Game.OVERWATCH, _userId));
            var result = actionResult.Result as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestUndoOverwatchDaily()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var dailyDto = new DailyDto
            {
                IsToday = false
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };

            _overwatchServiceMock.Setup(x => x.Undo(Game.OVERWATCH, _userId, true)).ReturnsAsync(serviceResponse);

            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext {User = _claimsPrincipalUser}
                }
            };

            var actionResult = await controller.UndoOverwatchDaily(true);

            // Assert
            _overwatchServiceMock.Verify(x => x.Undo(Game.OVERWATCH, _userId, true));
            var result = actionResult.Result as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestGetDaily_HasNoCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var dailyDto = new DailyDto
            {
                IsToday = true
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };

            _overwatchServiceMock.Setup(x => x.GetDaily()).ReturnsAsync(serviceResponse);
            var httpContext = new DefaultHttpContext();
            
            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
            
            var actionResult = await controller.GetDaily();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetDaily());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestGetDaily_HasCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var dailyDto = new DailyDto
            {
                IsToday = true
            };
            var serviceResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };
            var expectedResponse = new ServiceResponse<DailyDto>
            {
                Data = dailyDto,
                Time = date
            };
            _memoryCache.Set(CacheKeys.OverwatchDaily, serviceResponse);
            var httpContext = new DefaultHttpContext();
            
            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
            
            var actionResult = await controller.GetDaily();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetDaily(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestGetEvent_HasNoCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Default",
                Time = date
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Default",
                Time = date
            };

            _configServiceMock.Setup(x => x.GetCurrentOverwatchEvent()).ReturnsAsync(serviceResponse);
            var httpContext = new DefaultHttpContext();
            
            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
            
            var actionResult = await controller.GetEvent();

            // Assert
            _configServiceMock.Verify(x => x.GetCurrentOverwatchEvent());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestGetEvent_HasCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Default",
                Time = date
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Default",
                Time = date
            };
            _memoryCache.Set(CacheKeys.OverwatchEvent, serviceResponse);

            var httpContext = new DefaultHttpContext();
            
            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
            
            var actionResult = await controller.GetEvent();

            // Assert
            _configServiceMock.Verify(x => x.GetCurrentOverwatchEvent(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public async Task TestPostEvent_HasNoCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Default",
                Time = date
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Default",
                Time = date
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
        public async Task TestGetEventWallpaperUrl()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<string>
            {
                Data = "Wallpaper_URL",
                Time = date
            };
            var expectedResponse = new ServiceResponse<string>
            {
                Data = "Wallpaper_URL",
                Time = date
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
        public void TestGetEvents_HasCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<string[]>
            {
                Data = new[] {"Event_A", "Event_B"},
                Time = date
            };
            var expectedResponse = new ServiceResponse<string[]>
            {
                Data = new[] {"Event_A", "Event_B"},
                Time = date
            };
            _memoryCache.Set(CacheKeys.OverwatchEvents, serviceResponse);
            _configServiceMock.Setup(x => x.GetOverwatchEvents()).Returns(serviceResponse);
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
            _configServiceMock.Verify(x => x.GetCurrentOverwatchEvent(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void TestGetEvents_HasNoCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<string[]>
            {
                Data = new[] {"Event_A", "Event_B"},
                Time = date
            };
            var expectedResponse = new ServiceResponse<string[]>
            {
                Data = new[] {"Event_A", "Event_B"},
                Time = date
            };
            _configServiceMock.Setup(x => x.GetOverwatchEvents()).Returns(serviceResponse);
            var httpContext = new DefaultHttpContext();
            
            // Act
            var controller = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
            
            var actionResult =  controller.GetEvents();

            // Assert
            _configServiceMock.Verify(x => x.GetOverwatchEvents());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void TestGetArcadeModes_HasCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Total Mayhem",
                        Description = "Everybody likes mayhem, right?",
                        Image = "image.jpg",
                        Players = "6v6"
                    }
                },
                Time = date
            };
            var expectedResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Total Mayhem",
                        Description = "Everybody likes mayhem, right?",
                        Image = "image.jpg",
                        Players = "6v6"
                    }
                },
                Time = date
            };
            _memoryCache.Set(CacheKeys.OverwatchArcademodes, serviceResponse);
            _overwatchServiceMock.Setup(x => x.GetArcadeModes()).Returns(serviceResponse);


            // Act
            var actionResult = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache).GetArcadeModes();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetArcadeModes(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void TestGetArcadeModes_HasNoCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto>
                {
                    new()
                    {
                        Id = 1,
                        Description = "Everybody likes mayhem, right?"
                    }
                },
                Time = date
            };
            var expectedResponse = new ServiceResponse<List<ArcadeModeDto>>
            {
                Data = new List<ArcadeModeDto>
                {
                    new()
                    {
                        Id = 1,
                        Description = "Everybody likes mayhem, right?"
                    }
                },
                Time = date
            };
            _overwatchServiceMock.Setup(x => x.GetArcadeModes()).Returns(serviceResponse);

            // Act
            var actionResult = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache).GetArcadeModes();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetArcadeModes());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void TestGetLabels_HasCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<List<Label>>
            {
                Data = new List<Label>
                {
                    new()
                    {
                        Id = 1,
                        Value = "Daily"
                    }
                },
                Time = date
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
                },
                Time = date
            };
            _memoryCache.Set(CacheKeys.OverwatchLabels, serviceResponse);
            _overwatchServiceMock.Setup(x => x.GetLabels()).Returns(serviceResponse);


            // Act
            var actionResult = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache).GetLabels();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetLabels(), Times.Never);
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }

        [Fact]
        public void TestGetLabels_HasNoCache()
        {
            // Arrange
            var date = DateTime.Parse("03-20-2021");
            var serviceResponse = new ServiceResponse<List<Label>>
            {
                Data = new List<Label>
                {
                    new()
                    {
                        Id = 1,
                        Value = "Daily"
                    }
                },
                Time = date
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
                },
                Time = date
            };
            _overwatchServiceMock.Setup(x => x.GetLabels()).Returns(serviceResponse);

            // Act
            var actionResult = new OverwatchController(_overwatchServiceMock.Object, _configServiceMock.Object, _memoryCache).GetLabels();

            // Assert
            _overwatchServiceMock.Verify(x => x.GetLabels());
            var result = actionResult as ObjectResult;
            AssertActionResult(result, expectedResponse);
        }
    }
}