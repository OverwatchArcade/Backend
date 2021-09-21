using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using OWArcadeBackend.Dtos;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Overwatch;
using OWArcadeBackend.Persistence;
using OWArcadeBackend.Services.OverwatchService;
using OWArcadeBackend.Services.TwitterService;
using Shouldly;
using Xunit;

namespace OWArcadeBackend.Tests.Services
{
    public class OverwatchServiceTest
    {
        private readonly Mock<ILogger<OverwatchService>> _loggerMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ITwitterService> _twitterServiceMock;

        private Contributor _contributor;
        private Daily _daily;

        public OverwatchServiceTest()
        {
            _loggerMock = new Mock<ILogger<OverwatchService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _configurationMock = new Mock<IConfiguration>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _twitterServiceMock = new Mock<ITwitterService>();

            ConfigureMocks();
        }

        private void ConfigureMocks()
        {
            _contributor = new Contributor()
            {
                Id = new Guid("12D20088-719F-48A3-859D-A255CDFD1273"),
                Email = "info@overwatcharcade.today",
                Username = "System",
                Group = ContributorGroup.Developer,
                Avatar = "default.jpg"
            };
            _daily = new Daily()
            {
                Game = Game.OVERWATCH,
                CreatedAt = DateTime.Parse("03-20-2000"),
                TileModes = new List<TileMode>()
                {
                    new()
                    {
                        TileId = 1,
                        ArcadeModeId = 1,
                        DailyId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 2,
                        ArcadeModeId = 2,
                        DailyId = 1,
                        LabelId = 1,
                    },
                    new()
                    {
                        TileId = 3,
                        ArcadeModeId = 3,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 4,
                        ArcadeModeId = 4,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 5,
                        ArcadeModeId = 5,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 6,
                        ArcadeModeId = 6,
                        DailyId = 1,
                        LabelId = 2,
                    },
                    new()
                    {
                        TileId = 7,
                        ArcadeModeId = 7,
                        DailyId = 1,
                        LabelId = 2,
                    }
                }
            };
        }

        [Fact]
        public void TestConstructor()
        {
            var constructor = new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new OverwatchService(null, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, null, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, null, _twitterServiceMock.Object, _configurationMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, null, _configurationMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, null));
        }


        [Fact]
        public async Task TestSubmit_DailyAlreadySubmtited()
        {
            // arrange
            var owTilesConfigKey = new Config()
            {
                Value = "7"
            };
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == "OW_TILES")).Returns(new List<Config> { owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(true);

            // act
            var result = await new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object).Submit(_daily, Game.OVERWATCH, _contributor.Id);

            // assert
            result.StatusCode.ShouldBe(409);
            result.Message.ShouldBe("Daily has already been submitted");
        }

        [Fact]
        public async Task TestSubmit_SubmitsDaily()
        {
            var owTilesConfigKey = new Config()
            {
                Value = "7"
            };
            var dailyDto = new DailyDto()
            {
                Contributor = new ContributorDto()
                {
                    Username = "System"
                }
            };
            var expectedDailyDto = new DailyDto()
            {
                Contributor = new ContributorDto()
                {
                    Username = "System"
                }
            };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == "OW_TILES")).Returns(new List<Config> { owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily(Game.OVERWATCH)).ReturnsAsync(dailyDto);
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object)
                .Submit(_daily, Game.OVERWATCH, _contributor.Id);

            // assert
            _unitOfWorkMock.Verify(x => x.DailyRepository.Add(_daily));
            _unitOfWorkMock.Verify(x => x.Save());
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeOfType<DailyDto>();
            result.Data.ShouldBeEquivalentTo(expectedDailyDto);
        }

        [Fact]
        public async Task TestUndo_NotSubmittedYet()
        {
            // arrange
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(false);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object)
                .Undo(Game.OVERWATCH, contributorId);

            // assert
            result.StatusCode.ShouldBe(500);
            result.Message.ShouldBe("Daily has not been submitted yet");
        }

        [Fact]
        public async Task TestUndo_Undo()
        {
            // arrange
            const Game gameType = Game.OVERWATCH;
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object)
                .Undo(gameType, contributorId);

            // assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.RemoveRange(dailySubmits));
            _unitOfWorkMock.Verify(x => x.Save());
        }

        [Fact]
        public async Task TestGetDaily_ReturnsDaily()
        {
            // arrange
            var dailyDto = new DailyDto()
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
                Contributor = new ContributorDto()
                {
                    Username = "System",
                    Avatar = "default.jpg",
                    Stats = new ContributorStats()
                    {
                        FavouriteContributionDay = "Saturday",
                        LastContributedAt = DateTime.Parse("03-21-2021")
                    }
                }
            };
            var expectedDailyDto = JsonConvert.DeserializeObject<DailyDto>(JsonConvert.SerializeObject(dailyDto)); // Ez deep clone
            const Game gameType = Game.OVERWATCH;
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily(gameType)).ReturnsAsync(dailyDto);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object)
                .GetDaily();

            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedDailyDto);
        }

        [Fact]
        public void TestGetArcadeModes_ReturnsListOfArcadeModes()
        {
            // arrange
            const Game gameType = Game.OVERWATCH;
            var arcadeModeList = new List<ArcadeModeDto>()
            {
                new()
                {
                    Image = "image.jpg",
                    Name = "Total Mayhem",
                    Description = "Everybody loves total mayhem!",
                    Players = "6v6"
                },
                new()
                {
                    Image = "image.jpg",
                    Name = "CTF",
                    Description = "Capture the flag!",
                    Players = "6v6"
                }
            };
            var expectedArcadeModeList = JsonConvert.DeserializeObject<List<ArcadeModeDto>>(JsonConvert.SerializeObject(arcadeModeList)); // Ez deep clone
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.GetArcadeModes(gameType)).Returns(arcadeModeList);
            
            // act
            var result = new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object)
                .GetArcadeModes();
            
            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedArcadeModeList);
        }

        [Fact]
        public void TestGetLabels_ReturnsListOfLabels()
        {
            // arrange
            var labels = new List<Label>()
            {
                new()
                {
                    Value = null
                },
                new()
                {
                    Value = "Daily"
                },
                new()
                {
                    Value = "Weekly"
                },
                new()
                {
                    Value = "Permanent"
                }
            };
            var expectedLabels = JsonConvert.DeserializeObject<List<Label>>(JsonConvert.SerializeObject(labels));
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.GetLabels()).Returns(labels);
            
            // act
            var result = new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object)
                .GetLabels();
            
            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedLabels);
        }
    }
}