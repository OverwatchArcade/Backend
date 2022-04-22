﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using OWArcadeBackend.Dtos.Contributor;
using OWArcadeBackend.Dtos.Contributor.Stats;
using OWArcadeBackend.Dtos.Overwatch;
using OWArcadeBackend.Models;
using OWArcadeBackend.Models.Constants;
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
        private readonly Mock<IMapper> _mapperMock;

        private Contributor _contributor;
        private Daily _daily;

        public OverwatchServiceTest()
        {
            _loggerMock = new Mock<ILogger<OverwatchService>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _configurationMock = new Mock<IConfiguration>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _twitterServiceMock = new Mock<ITwitterService>();
            _mapperMock = new Mock<IMapper>();

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
            var constructor = new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object);
            Assert.NotNull(constructor);
        }

        [Fact]
        public void TestConstructorFunction_throws_Exception()
        {
            Should.Throw<ArgumentNullException>(() => new OverwatchService(null, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, null, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, null, _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, null, _configurationMock.Object, _mapperMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, null, _mapperMock.Object));
            Should.Throw<ArgumentNullException>(() => new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object, null));
        }


        [Fact]
        public async Task TestSubmit_DailyAlreadySubmitted()
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
            var result = await new OverwatchService(_loggerMock.Object, _unitOfWorkMock.Object, _memoryCache, _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object).Submit(_daily, Game.OVERWATCH, _contributor.Id);

            // assert
            result.StatusCode.ShouldBe(409);
            result.Message.ShouldBe("Daily has already been submitted");
        }
        
        [Fact]
        public async Task TestSubmit_Throws_Exception()
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
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.ConfigRepository.Find(y => y.Key == "OW_TILES")).Returns(new List<Config> { owTilesConfigKey });
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.Exists(It.IsAny<Expression<Func<ArcadeMode, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.LabelRepository.Exists(It.IsAny<Expression<Func<Label, bool>>>())).Returns(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.DailyRepository.GetDaily(Game.OVERWATCH)).ReturnsAsync(dailyDto);
            _unitOfWorkMock.Setup(x => x.Save()).Throws<DbUpdateException>();
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
                .Submit(_daily, Game.OVERWATCH, _contributor.Id);

            // assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
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
            _twitterServiceMock.Setup(x => x.PostTweet(Game.OVERWATCH));
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
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
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
                .Undo(Game.OVERWATCH, contributorId, true);

            // assert
            result.StatusCode.ShouldBe(500);
            result.Message.ShouldBe("Daily has not been submitted yet");
        }

        [Fact]
        public async Task TestUndo_Undo_Delete()
        {
            // arrange
            const Game gameType = Game.OVERWATCH;
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
                .Undo(gameType, contributorId, true);

            // assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.RemoveRange(dailySubmits));
            _unitOfWorkMock.Verify(x => x.Save());
        }
        
        [Fact]
        public async Task TestUndo_Undo_SoftDelete()
        {
            // arrange
            const Game gameType = Game.OVERWATCH;
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);

            // act
            var result = await new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
                .Undo(gameType, contributorId, false);

            // assert
            result.StatusCode.ShouldBe(200);
            _unitOfWorkMock.Verify(x => x.DailyRepository.RemoveRange(dailySubmits), Times.Never);
            dailySubmits.ShouldAllBe(x => x.MarkedOverwrite.Equals(true));
            _unitOfWorkMock.Verify(x => x.Save());
        }
        
        [Fact]
        public async Task TestUndo_Throws_Exception()
        {
            // arrange
            const Game gameType = Game.OVERWATCH;
            var contributorId = new Guid("9725B478-B92E-4453-B10D-D7DA61A1F6E8");
            var daily = new Daily();
            var dailySubmits = new List<Daily>() { daily };
            var connectToTwitterConfigurationSection = new Mock<IConfigurationSection>();
            connectToTwitterConfigurationSection.Setup(x => x.Value).Returns("false");
            _unitOfWorkMock.Setup(x => x.DailyRepository.HasDailySubmittedToday(Game.OVERWATCH, null)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(x => x.DailyRepository.Find(It.IsAny<Expression<Func<Daily, bool>>>())).Returns(dailySubmits);
            _unitOfWorkMock.Setup(x => x.Save()).Throws<DbUpdateException>();
            _configurationMock.Setup(x => x.GetSection("connectToTwitter")).Returns(connectToTwitterConfigurationSection.Object);


            // act
            var service = new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object);
            var result = await service.Undo(gameType, contributorId, false);
            
            // assert
            result.Success.ShouldBeFalse();
            result.StatusCode.ShouldBe(500);
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
                    Stats = new ContributorStatsDto()
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
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
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
            var arcadeModeList = new List<ArcadeMode>()
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
            var arcadeModeDtoList = new List<ArcadeModeDto>()
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
                    Name = "Total Mayhem",
                    Description = "Everybody loves total mayhem!",
                    Players = "6v6"
                }
            };
            
            _mapperMock.Setup(x => x.Map<List<ArcadeModeDto>>(arcadeModeList)).Returns(arcadeModeDtoList);
            _unitOfWorkMock.Setup(x => x.OverwatchRepository.GetArcadeModes(gameType)).Returns(arcadeModeList);
            
            var expectedArcadeModeList = JsonConvert.DeserializeObject<List<ArcadeModeDto>>(JsonConvert.SerializeObject(arcadeModeDtoList)); // Ez deep clone

            // act
            var result = new OverwatchService(
                    _loggerMock.Object, _unitOfWorkMock.Object, _memoryCache,
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
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
                    _twitterServiceMock.Object, _configurationMock.Object, _mapperMock.Object)
                .GetLabels();
            
            // assert
            result.StatusCode.ShouldBe(200);
            result.Data.ShouldBeEquivalentTo(expectedLabels);
        }
    }
}